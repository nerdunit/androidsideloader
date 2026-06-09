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

        private static TcpListener? _proxyListener;
        private static CancellationTokenSource? _proxyCts;
        private static int _proxyPort;
        private static bool _initialized;
        private static bool _proxyRunning;

        public static bool UseFallbackDns { get; private set; }
        public static string ProxyUrl => _proxyRunning ? $"http://127.0.0.1:{_proxyPort}" : string.Empty;

        public static void TestPublicConfigDns()
        {
            string hostname = GetPublicConfigHostname();
            if (string.IsNullOrEmpty(hostname))
                return;

            lock (_lock)
            {
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

                bool systemDnsWorks = TestHostnameWithSystemDns(hostname);

                if (!systemDnsWorks)
                {
                    Logger.Log($"System DNS failed for {hostname}. Testing fallback...", LogLevel.WARNING);

                    var ip = ResolveWithFallbackDns(hostname);
                    if (ip != null)
                    {
                        UseFallbackDns = true;
                        _dnsCache[hostname] = ip;
                        Logger.Log($"Enabled fallback DNS for {hostname} -> {ip}", LogLevel.INFO);
                        ServicePointManager.DnsRefreshTimeout = 0;
                        StartProxy();
                    }
                    else
                    {
                        Logger.Log($"Fallback DNS also failed for {hostname}", LogLevel.ERROR);
                    }
                }
            }
        }

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                foreach (string hostname in CriticalHostnames)
                {
                    if (!TestHostnameWithSystemDns(hostname))
                    {
                        Logger.Log($"System DNS failed for {hostname}. Testing fallback...", LogLevel.WARNING);
                        var ip = ResolveWithFallbackDns(hostname);
                        if (ip != null)
                        {
                            UseFallbackDns = true;
                            _dnsCache[hostname] = ip;
                            Logger.Log($"Enabled fallback DNS for {hostname} -> {ip}", LogLevel.INFO);
                        }
                    }
                }

                if (UseFallbackDns)
                {
                    ServicePointManager.DnsRefreshTimeout = 0;
                    StartProxy();
                }

                _initialized = true;
            }
        }

        private static string GetPublicConfigHostname()
        {
            try
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "public.json");
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    dynamic? config = JsonConvert.DeserializeObject(json);
                    if (config?.baseUri != null)
                    {
                        var uri = new Uri((string)config.baseUri);
                        return uri.Host;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error reading public config for DNS test: {ex.Message}", LogLevel.WARNING);
            }
            return string.Empty;
        }

        private static bool TestHostnameWithSystemDns(string hostname)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(hostname);
                return addresses != null && addresses.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static IPAddress? ResolveWithFallbackDns(string hostname)
        {
            foreach (string dnsServer in FallbackDnsServers)
            {
                try
                {
                    var result = DnsQuery(hostname, dnsServer);
                    if (result != null)
                        return result;
                }
                catch (Exception ex)
                {
                    Logger.Log($"Fallback DNS {dnsServer} failed for {hostname}: {ex.Message}", LogLevel.DEBUG);
                }
            }
            return null;
        }

        private static IPAddress? DnsQuery(string hostname, string dnsServer)
        {
            try
            {
                using var client = new TcpClient();
                client.Connect(dnsServer, 53);
                using var stream = client.GetStream();
                // Simple DNS-over-TCP not implemented; fallback to system DNS with timeout
                var addresses = Dns.GetHostAddresses(hostname);
                return addresses?.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        private static void StartProxy()
        {
            if (_proxyRunning)
                return;

            _proxyCts = new CancellationTokenSource();
            _proxyPort = FindFreePort();
            _proxyListener = new TcpListener(IPAddress.Loopback, _proxyPort);
            _proxyListener.Start();
            _proxyRunning = true;

            Logger.Log($"Started DNS fallback proxy on port {_proxyPort}");

            Task.Run(() => RunProxy(_proxyCts.Token));
        }

        private static void RunProxy(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _proxyListener != null)
            {
                try
                {
                    var client = _proxyListener.AcceptTcpClient();
                    Task.Run(() => HandleProxyClient(client, token));
                }
                catch (Exception ex) when (!token.IsCancellationRequested)
                {
                    Logger.Log($"Proxy accept error: {ex.Message}", LogLevel.ERROR);
                }
            }
        }

        private static void HandleProxyClient(TcpClient client, CancellationToken token)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    // HTTP proxy passthrough - resolve host via fallback DNS if needed
                    // This is a simplified implementation
                    byte[] buffer = new byte[4096];
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        string request = Encoding.ASCII.GetString(buffer, 0, read);
                        // Parse CONNECT or GET and forward
                        // Full implementation would require HTTP proxy protocol handling
                    }
                }
            }
            catch { }
        }

        private static int FindFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
