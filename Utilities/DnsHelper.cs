using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AndroidSideloader.Utilities
{
    // Provides DNS fallback functionality using Cloudflare DNS (1.1.1.1, 1.0.0.1) if system DNS fails to resolve critical hostnames
    // Also provides a proxy for rclone that handles DNS resolution
    public static class DnsHelper
    {
        private static readonly string[] FallbackDnsServers = { "1.1.1.1", "1.0.0.1" };
        private static readonly string[] CriticalHostnames =
        {
            "raw.githubusercontent.com",
            "downloads.rclone.org",
            "github.com"
        };

        private static readonly ConcurrentDictionary<string, IPAddress> _dnsCache =
            new ConcurrentDictionary<string, IPAddress>(StringComparer.OrdinalIgnoreCase);

        private static readonly object _lock = new object();

        // Local proxy for rclone
        private static TcpListener _proxyListener;
        private static CancellationTokenSource _proxyCts;
        private static int _proxyPort;
        private static bool _initialized;
        private static bool _proxyRunning;

        public static bool UseFallbackDns { get; private set; }

        // Gets the proxy URL for rclone to use, or empty string if not needed
        public static string ProxyUrl => _proxyRunning ? $"http://127.0.0.1:{_proxyPort}" : string.Empty;

        // Called after public.json is created/updated to test the hostname
        // Enable fallback DNS if the hostname fails on system DNS but works with fallback DNS
        public static void TestPublicConfigDns()
        {
            string hostname = GetPublicConfigHostname();
            if (string.IsNullOrEmpty(hostname))
                return;

            lock (_lock)
            {
                // If already using fallback, just pre-resolve the new hostname
                if (UseFallbackDns)
                {
                    var ip = ResolveWithFallbackDns(hostname);
                    if (ip != null)
                    {
                        _dnsCache[hostname] = ip;
                        Logger.Log($"Pre-resolved public config hostname {hostname} -> {ip}");
                    }
                    return;
                }

                // Test if system DNS can resolve the public config hostname
                bool systemDnsWorks = TestHostnameWithSystemDns(hostname);

                if (!systemDnsWorks)
                {
                    Logger.Log($"System DNS failed for {hostname}. Testing fallback...", LogLevel.WARNING);

                    // Test if fallback DNS works for this hostname
                    var ip = ResolveWithFallbackDns(hostname);
                    if (ip != null)
                    {
                        UseFallbackDns = true;
                        _dnsCache[hostname] = ip;
                        Logger.Log($"Enabled fallback DNS for {hostname} -> {ip}", LogLevel.INFO);
                        ServicePointManager.DnsRefreshTimeout = 0;

                        // Pre-resolve base hostnames too
                        PreResolveHostnames(CriticalHostnames);

                        // Start proxy if not already running
                        if (!_proxyRunning)
                        {
                            StartProxy();
                        }
                    }
                    else
                    {
                        Logger.Log($"Both system and fallback DNS failed for {hostname}", LogLevel.ERROR);
                    }
                }
                else
                {
                    Logger.Log($"System DNS works for public config hostname: {hostname}");
                }
            }
        }

        private static string GetPublicConfigHostname()
        {
            try
            {
                string configPath = Path.Combine(Environment.CurrentDirectory, "public.json");
                if (!File.Exists(configPath))
                    return null;

                var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configPath));
                if (config != null && config.TryGetValue("baseUri", out string baseUri))
                {
                    return ExtractHostname(baseUri);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to get hostname from public.json: {ex.Message}", LogLevel.WARNING);
            }
            return null;
        }

        private static string[] GetCriticalHostnames()
        {
            var hostnames = new List<string>(CriticalHostnames);

            string host = GetPublicConfigHostname();
            if (!string.IsNullOrWhiteSpace(host) && !hostnames.Contains(host))
            {
                hostnames.Add(host);
                Logger.Log($"Added {host} from public.json to critical hostnames");
            }

            return hostnames.ToArray();
        }

        private static string ExtractHostname(string uriString)
        {
            if (string.IsNullOrWhiteSpace(uriString)) return null;

            if (!uriString.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !uriString.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                uriString = "https://" + uriString;
            }

            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri))
                return uri.Host;

            // Fallback: manual extraction
            string hostname = uriString.Replace("https://", "").Replace("http://", "");
            int idx = hostname.IndexOfAny(new[] { '/', ':' });
            return idx > 0 ? hostname.Substring(0, idx) : hostname;
        }

        public static void Initialize()
        {
            lock (_lock)
            {
                if (_initialized) return;

                Logger.Log("Testing DNS resolution for critical hostnames...");
                var hostnames = GetCriticalHostnames();

                if (TestDns(hostnames, useSystem: true))
                {
                    Logger.Log("System DNS is working correctly.");
                }
                else
                {
                    Logger.Log("System DNS failed. Testing Cloudflare DNS fallback...", LogLevel.WARNING);
                    if (TestDns(hostnames, useSystem: false))
                    {
                        UseFallbackDns = true;
                        Logger.Log("Using Cloudflare DNS fallback.", LogLevel.INFO);
                        PreResolveHostnames(hostnames);
                        ServicePointManager.DnsRefreshTimeout = 0;
                        StartProxy();
                    }
                    else
                    {
                        Logger.Log("Both system and fallback DNS failed.", LogLevel.ERROR);
                    }
                }

                _initialized = true;
            }
        }

        public static void Cleanup() => StopProxy();

        private static bool TestHostnameWithSystemDns(string hostname)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(hostname);
                return addresses?.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool TestDns(string[] hostnames, bool useSystem)
        {
            if (useSystem)
            {
                return hostnames.All(h =>
                {
                    try { return Dns.GetHostAddresses(h)?.Length > 0; }
                    catch { return false; }
                });
            }

            return FallbackDnsServers.Any(server =>
            {
                try { return ResolveWithDns(hostnames[0], server)?.Count > 0; }
                catch { return false; }
            });
        }

        private static void PreResolveHostnames(string[] hostnames)
        {
            foreach (string hostname in hostnames)
            {
                var ip = ResolveWithFallbackDns(hostname);
                if (ip != null)
                {
                    _dnsCache[hostname] = ip;
                    Logger.Log($"Pre-resolved {hostname} -> {ip}");
                }
            }
        }

        private static IPAddress ResolveWithFallbackDns(string hostname)
        {
            foreach (string server in FallbackDnsServers)
            {
                try
                {
                    var addresses = ResolveWithDns(hostname, server);
                    if (addresses?.Count > 0) return addresses[0];
                }
                catch { }
            }
            return null;
        }

        private static List<IPAddress> ResolveWithDns(string hostname, string dnsServer, int timeoutMs = 5000)
        {
            using (var udp = new UdpClient { Client = { ReceiveTimeout = timeoutMs, SendTimeout = timeoutMs } })
            {
                byte[] query = BuildDnsQuery(hostname);
                udp.Send(query, query.Length, new IPEndPoint(IPAddress.Parse(dnsServer), 53));
                IPEndPoint remoteEp = null;
                return ParseDnsResponse(udp.Receive(ref remoteEp));
            }
        }

        private static byte[] BuildDnsQuery(string hostname)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(IPAddress.HostToNetworkOrder((short)new Random().Next(0, ushort.MaxValue)));
                writer.Write(IPAddress.HostToNetworkOrder((short)0x0100)); // Flags
                writer.Write(IPAddress.HostToNetworkOrder((short)1));      // Questions
                writer.Write(IPAddress.HostToNetworkOrder((short)0));      // Answer RRs
                writer.Write(IPAddress.HostToNetworkOrder((short)0));      // Authority RRs
                writer.Write(IPAddress.HostToNetworkOrder((short)0));      // Additional RRs

                foreach (string label in hostname.Split('.'))
                {
                    writer.Write((byte)label.Length);
                    writer.Write(Encoding.ASCII.GetBytes(label));
                }
                writer.Write((byte)0);
                writer.Write(IPAddress.HostToNetworkOrder((short)1)); // Type A
                writer.Write(IPAddress.HostToNetworkOrder((short)1)); // Class IN

                return ms.ToArray();
            }
        }

        private static List<IPAddress> ParseDnsResponse(byte[] response)
        {
            var addresses = new List<IPAddress>();
            if (response.Length < 12) return addresses;

            int pos = 12;
            while (pos < response.Length && response[pos] != 0) pos += response[pos] + 1;
            pos += 5;

            int answerCount = (response[6] << 8) | response[7];
            for (int i = 0; i < answerCount && pos + 12 <= response.Length; i++)
            {
                pos += (response[pos] & 0xC0) == 0xC0 ? 2 : SkipName(response, pos);
                if (pos + 10 > response.Length) break;

                ushort type = (ushort)((response[pos] << 8) | response[pos + 1]);
                ushort rdLength = (ushort)((response[pos + 8] << 8) | response[pos + 9]);
                pos += 10;

                if (pos + rdLength > response.Length) break;
                if (type == 1 && rdLength == 4)
                    addresses.Add(new IPAddress(new[] { response[pos], response[pos + 1], response[pos + 2], response[pos + 3] }));
                pos += rdLength;
            }
            return addresses;
        }

        private static int SkipName(byte[] data, int pos)
        {
            int start = pos;
            while (pos < data.Length && data[pos] != 0) pos += data[pos] + 1;
            return pos - start + 1;
        }

        public static IPAddress ResolveHostname(string hostname, bool alwaysTryFallback = false)
        {
            if (_dnsCache.TryGetValue(hostname, out IPAddress cached))
                return cached;

            try
            {
                var addresses = Dns.GetHostAddresses(hostname);
                if (addresses?.Length > 0)
                {
                    _dnsCache[hostname] = addresses[0];
                    return addresses[0];
                }
            }
            catch { }

            if (alwaysTryFallback || UseFallbackDns || !_initialized)
            {
                var ip = ResolveWithFallbackDns(hostname);
                if (ip != null)
                {
                    _dnsCache[hostname] = ip;
                    return ip;
                }
            }

            return null;
        }

        public static HttpWebRequest CreateWebRequest(string url)
        {
            var uri = new Uri(url);

            if (!UseFallbackDns)
            {
                try
                {
                    Dns.GetHostAddresses(uri.Host);
                    return (HttpWebRequest)WebRequest.Create(url);
                }
                catch
                {
                    if (!_initialized) Initialize();
                }
            }

            if (UseFallbackDns)
            {
                var ip = ResolveHostname(uri.Host, alwaysTryFallback: true);
                if (ip != null)
                {
                    var builder = new UriBuilder(uri) { Host = ip.ToString() };
                    var request = (HttpWebRequest)WebRequest.Create(builder.Uri);
                    request.Host = uri.Host;
                    return request;
                }
            }

            return (HttpWebRequest)WebRequest.Create(url);
        }

        private static void StartProxy()
        {
            try
            {
                // Find an available port
                _proxyListener = new TcpListener(IPAddress.Loopback, 0);
                _proxyListener.Start();
                _proxyPort = ((IPEndPoint)_proxyListener.LocalEndpoint).Port;
                _proxyCts = new CancellationTokenSource();
                _proxyRunning = true;

                Logger.Log($"Started DNS proxy on port {_proxyPort}");

                // Accept connections in background
                Task.Run(() => ProxyAcceptLoop(_proxyCts.Token));
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to start DNS proxy: {ex.Message}", LogLevel.WARNING);
                _proxyRunning = false;
            }
        }

        private static void StopProxy()
        {
            _proxyRunning = false;
            _proxyCts?.Cancel();
            try { _proxyListener?.Stop(); } catch { }
        }

        private static async Task ProxyAcceptLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _proxyRunning)
            {
                try
                {
                    var client = await _proxyListener.AcceptTcpClientAsync();
                    _ = HandleProxyClient(client, ct);
                }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    if (!ct.IsCancellationRequested)
                        Logger.Log($"Proxy accept error: {ex.Message}", LogLevel.WARNING);
                }
            }
        }

        private static async Task HandleProxyClient(TcpClient client, CancellationToken ct)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    client.ReceiveTimeout = client.SendTimeout = 30000;

                    var buffer = new byte[8192];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (bytesRead == 0) return;

                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (lines.Length == 0) return;

                    string[] requestLine = lines[0].Split(' ');
                    if (requestLine.Length < 2) return;

                    if (requestLine[0] == "CONNECT")
                        // HTTPS proxy - tunnel mode
                        await HandleConnectRequest(stream, requestLine[1], ct);
                    else
                        // HTTP proxy - forward mode
                        await HandleHttpRequest(stream, request, requestLine[1], ct);
                }
            }
            catch (Exception ex)
            {
                if (!ct.IsCancellationRequested)
                    Logger.Log($"Proxy client error: {ex.Message}", LogLevel.WARNING);
            }
        }

        private static async Task HandleConnectRequest(NetworkStream clientStream, string target, CancellationToken ct)
        {
            // Parse host:port
            string[] parts = target.Split(':');
            string host = parts[0];
            int port = parts.Length > 1 && int.TryParse(parts[1], out int p) ? p : 443;

            // Resolve hostname
            IPAddress ip = ResolveHostname(host, alwaysTryFallback: true);
            if (ip == null)
            {
                await SendResponse(clientStream, "HTTP/1.1 502 Bad Gateway\r\n\r\n", ct);
                return;
            }

            try
            {
                // Connect to target
                using (var targetClient = new TcpClient())
                {
                    await targetClient.ConnectAsync(ip, port);
                    using (var targetStream = targetClient.GetStream())
                    {
                        // Send 200 OK to client
                        await SendResponse(clientStream, "HTTP/1.1 200 Connection Established\r\n\r\n", ct);
                        // Tunnel data bidirectionally
                        await Task.WhenAny(RelayData(clientStream, targetStream, ct), RelayData(targetStream, clientStream, ct));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"CONNECT tunnel error to {host}: {ex.Message}", LogLevel.WARNING);
                await SendResponse(clientStream, "HTTP/1.1 502 Bad Gateway\r\n\r\n", ct);
            }
        }

        private static async Task HandleHttpRequest(NetworkStream clientStream, string request, string url, CancellationToken ct)
        {
            try
            {
                var uri = new Uri(url);
                IPAddress ip = ResolveHostname(uri.Host, alwaysTryFallback: true);
                if (ip == null)
                {
                    await SendResponse(clientStream, "HTTP/1.1 502 Bad Gateway\r\n\r\n", ct);
                    return;
                }

                using (var targetClient = new TcpClient())
                {
                    await targetClient.ConnectAsync(ip, uri.Port > 0 ? uri.Port : 80);
                    using (var targetStream = targetClient.GetStream())
                    {
                        // Modify request to use relative path
                        string modifiedRequest = request.Replace(url, uri.PathAndQuery);
                        byte[] requestBytes = Encoding.ASCII.GetBytes(modifiedRequest);
                        await targetStream.WriteAsync(requestBytes, 0, requestBytes.Length, ct);

                        // Relay response
                        await RelayData(targetStream, clientStream, ct);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"HTTP proxy error: {ex.Message}", LogLevel.WARNING);
            }
        }

        private static async Task SendResponse(NetworkStream stream, string response, CancellationToken ct)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(response);
            try { await stream.WriteAsync(bytes, 0, bytes.Length, ct); } catch { }
        }

        private static async Task RelayData(NetworkStream from, NetworkStream to, CancellationToken ct)
        {
            byte[] buffer = new byte[8192];
            try
            {
                int bytesRead;
                while ((bytesRead = await from.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                    await to.WriteAsync(buffer, 0, bytesRead, ct);
            }
            catch { }
        }
    }
}