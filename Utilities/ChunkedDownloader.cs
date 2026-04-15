using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidSideloader.Utilities
{
    public class DownloadProgressInfo
    {
        public long TotalBytes { get; set; }
        public long DownloadedBytes { get; set; }
        public int CurrentFileIndex { get; set; }
        public int TotalFiles { get; set; }
        public double SpeedBytesPerSecond { get; set; }
        public float OverallPercent { get; set; }
        public TimeSpan Eta { get; set; }
    }

    public class RemoteFileInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
    }

    public class ChunkedDownloader : IDisposable
    {
        private readonly string _baseUrl;
        private readonly string _destinationDir;
        private readonly HttpClient _httpClient;
        private readonly SettingsManager _settings;
        private Process _rcloneProxyProcess;
        private int _completedFiles;
        private const int MaxRetries = 3;
        private const int BufferSize = 81920;
        private const long FallbackPartSize = 500L * 1024 * 1024;

        private long _resolvedTotalBytes;
        private int _resolvedFileCount;
        private long _globalDownloaded;

        private static readonly string RcloneDir = Path.Combine(Environment.CurrentDirectory, "rclone");
        private static readonly string RclonePath = Path.Combine(RcloneDir, "rclone.exe");

        private static readonly Regex HrefRegex = new Regex(
            @"href=""([^""]+)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ProxyPortRegex = new Regex(
            @"HTTP Server started on \[(http://127\.0\.0\.1:\d+/)\]",
            RegexOptions.Compiled);

        /// <summary>
        /// Public mirror constructor — starts rclone serve http with --http-no-head
        /// so downloads go through a local proxy with byte-level resume (Range headers)
        /// and zero upstream HEAD requests.
        /// </summary>
        public ChunkedDownloader(string baseUrl, string destinationDir)
        {
            _destinationDir = destinationDir;
            _settings = SettingsManager.Instance;
            var uri = new Uri(baseUrl.TrimEnd('/'));
            string origin = uri.GetLeftPart(UriPartial.Authority);
            string remotePath = uri.AbsolutePath.TrimStart('/');
            string extraArgs = (MainForm.PublicMirrorExtraArgs ?? "").Trim();
            _baseUrl = StartRcloneProxy(
                $"serve http \":http:/{remotePath}/\" --http-url {origin} --http-no-head " +
                $"--addr 127.0.0.1:0 --read-only {extraArgs}", null);
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
        }

        /// <summary>Private mirror constructor — starts rclone serve http proxy via config.</summary>
        public ChunkedDownloader(string remotePath, string destinationDir, string configPath, string password = null)
        {
            _destinationDir = destinationDir;
            _settings = SettingsManager.Instance;
            string configArg = !string.IsNullOrEmpty(configPath) ? $"--config \"{configPath}\"" : "";
            string pwArg = !string.IsNullOrEmpty(password) ? "--ask-password=false" : "";
            _baseUrl = StartRcloneProxy(
                $"serve http \"{remotePath}\" --addr 127.0.0.1:0 --read-only {configArg} {pwArg}", password);
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
        }

        private ProcessStartInfo CreateRclonePsi(string args)
        {
            if (!File.Exists(RclonePath))
                throw new FileNotFoundException("rclone.exe not found at: " + RclonePath);

            var psi = new ProcessStartInfo
            {
                FileName = RclonePath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                WorkingDirectory = RcloneDir
            };
            ApplyProxyEnv(psi);
            return psi;
        }

        private void ApplyProxyEnv(ProcessStartInfo psi)
        {
            string proxy = (_settings.useProxy
                && !string.IsNullOrEmpty(_settings.ProxyAddress)
                && !string.IsNullOrEmpty(_settings.ProxyPort))
                    ? $"http://{_settings.ProxyAddress}:{_settings.ProxyPort}"
                    : DnsHelper.ProxyUrl;

            if (!string.IsNullOrEmpty(proxy))
                foreach (var key in new[] { "HTTP_PROXY", "HTTPS_PROXY", "http_proxy", "https_proxy" })
                    psi.EnvironmentVariables[key] = proxy;
        }

        private string StartRcloneProxy(string rcloneArgs, string rclonePassword)
        {
            var psi = CreateRclonePsi(rcloneArgs);
            if (!string.IsNullOrEmpty(rclonePassword))
                psi.EnvironmentVariables["RCLONE_CONFIG_PASS"] = rclonePassword;

            string localUrl = null;
            var portFound = new ManualResetEventSlim(false);

            _rcloneProxyProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _rcloneProxyProcess.ErrorDataReceived += (s, e) =>
            {
                if (e.Data == null) return;
                var match = ProxyPortRegex.Match(e.Data);
                if (match.Success)
                {
                    localUrl = match.Groups[1].Value.TrimEnd('/');
                    portFound.Set();
                }
            };

            _rcloneProxyProcess.Start();
            _rcloneProxyProcess.BeginErrorReadLine();

            if (!portFound.Wait(TimeSpan.FromSeconds(15)))
            {
                StopRcloneProxy();
                throw new TimeoutException("rclone local proxy failed to start within 15 seconds");
            }
            return localUrl;
        }

        private void StopRcloneProxy()
        {
            try
            {
                if (_rcloneProxyProcess != null && !_rcloneProxyProcess.HasExited)
                {
                    _rcloneProxyProcess.Kill();
                    _rcloneProxyProcess.WaitForExit(3000);
                }
            }
            catch (Exception ex) { Logger.Log($"Error stopping rclone proxy: {ex.Message}", LogLevel.WARNING); }
            finally
            {
                _rcloneProxyProcess?.Dispose();
                _rcloneProxyProcess = null;
            }
        }

        public async Task<List<RemoteFileInfo>> ListRemoteFilesAsync(CancellationToken ct)
        {
            var files = new List<RemoteFileInfo>();
            string html = await _httpClient.GetStringAsync(_baseUrl + "/").ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            foreach (Match m in HrefRegex.Matches(html))
            {
                string href = m.Groups[1].Value;
                if (href == "../" || href == ".." || href.StartsWith("?") || href.StartsWith("#") || href.EndsWith("/"))
                    continue;

                string encodedName = href.Contains("/") ? href.Substring(href.LastIndexOf('/') + 1) : href;
                if (string.IsNullOrEmpty(encodedName)) continue;

                string fileName = Uri.UnescapeDataString(encodedName);
                if (string.IsNullOrEmpty(fileName) || fileName == "/") continue;

                files.Add(new RemoteFileInfo { Name = fileName, Url = _baseUrl + "/" + encodedName, Size = 0 });
            }
            return files;
        }

        public async Task<ProcessOutput> DownloadAllAsync(
            Action<DownloadProgressInfo> onProgress,
            CancellationToken ct)
        {
            try
            {
                Directory.CreateDirectory(_destinationDir);
                var files = await ListRemoteFilesAsync(ct).ConfigureAwait(false);
                if (files.Count == 0)
                {
                    Logger.Log("No files found in remote listing", LogLevel.WARNING);
                    return new ProcessOutput("", "directory not found");
                }

                int totalFileCount = files.Count;
                _resolvedTotalBytes = 0;
                _resolvedFileCount = 0;
                _completedFiles = 0;

                long initialExisting = files
                    .Select(f => Path.Combine(_destinationDir, f.Name))
                    .Where(File.Exists)
                    .Sum(p => new FileInfo(p).Length);
                _globalDownloaded = initialExisting;

                // Finish existing partial files first, then start new ones
                files.Sort((a, b) =>
                {
                    bool ea = File.Exists(Path.Combine(_destinationDir, a.Name));
                    bool eb = File.Exists(Path.Combine(_destinationDir, b.Name));
                    return ea == eb ? 0 : ea ? -1 : 1;
                });

                // EWMA speed tracking
                var progressLock = new object();
                double ewmaSpeed = 0;
                const double alpha = 0.3;
                DateTime lastProgressTime = DateTime.UtcNow;
                long lastReportedDownloaded = initialExisting;

                Action<long> bytesCallback = (bytesJustWritten) =>
                {
                    if (bytesJustWritten > 0)
                        Interlocked.Add(ref _globalDownloaded, bytesJustWritten);

                    if ((DateTime.UtcNow - lastProgressTime).TotalSeconds < 0.25) return;

                    lock (progressLock)
                    {
                        DateTime now = DateTime.UtcNow;
                        double elapsed = (now - lastProgressTime).TotalSeconds;
                        if (elapsed < 0.25) return;

                        long downloaded = Interlocked.Read(ref _globalDownloaded);
                        double instantSpeed = elapsed > 0 ? (downloaded - lastReportedDownloaded) / elapsed : 0;
                        ewmaSpeed = ewmaSpeed < 1 ? instantSpeed : (alpha * instantSpeed) + ((1 - alpha) * ewmaSpeed);
                        lastProgressTime = now;
                        lastReportedDownloaded = downloaded;

                        int resolvedCount = Thread.VolatileRead(ref _resolvedFileCount);
                        long resolvedTotal = Interlocked.Read(ref _resolvedTotalBytes);

                        long estimatedTotal = resolvedCount >= totalFileCount ? resolvedTotal
                            : resolvedCount > 0 ? resolvedTotal * totalFileCount / resolvedCount
                            : (long)totalFileCount * FallbackPartSize;

                        float percent = estimatedTotal > 0
                            ? Math.Min(99f, (float)downloaded / estimatedTotal * 100f) : 0f;

                        TimeSpan eta = ewmaSpeed > 0 && estimatedTotal > downloaded
                            ? TimeSpan.FromSeconds((estimatedTotal - downloaded) / ewmaSpeed) : TimeSpan.Zero;

                        onProgress?.Invoke(new DownloadProgressInfo
                        {
                            TotalBytes = estimatedTotal,
                            DownloadedBytes = downloaded,
                            CurrentFileIndex = Math.Min(Thread.VolatileRead(ref _completedFiles) + 1, totalFileCount),
                            TotalFiles = totalFileCount,
                            SpeedBytesPerSecond = ewmaSpeed,
                            OverallPercent = percent,
                            Eta = eta
                        });
                    }
                };

                int maxConcurrency = _settings.SingleThreadMode ? 1 : Math.Min(4, files.Count);
                var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
                var tasks = files.Select(f => DownloadFileParallelAsync(f, semaphore, bytesCallback, ct)).ToList();
                string[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

                var errors = results.Where(r => r != null).ToList();
                if (errors.Count > 0)
                    return new ProcessOutput("", string.Join("\n", errors));

                long finalTotal = Interlocked.Read(ref _resolvedTotalBytes);
                onProgress?.Invoke(new DownloadProgressInfo
                {
                    TotalBytes = finalTotal, DownloadedBytes = finalTotal,
                    CurrentFileIndex = totalFileCount, TotalFiles = totalFileCount,
                    SpeedBytesPerSecond = ewmaSpeed, OverallPercent = 100f, Eta = TimeSpan.Zero
                });
                return new ProcessOutput("OK", "");
            }
            catch (OperationCanceledException) { return new ProcessOutput("", "Cancelled"); }
            catch (HttpRequestException ex)
            {
                Logger.Log($"HttpRequestException during download: {ex}", LogLevel.ERROR);
                string msg = ex.Message.ToLower();
                return (msg.Contains("403") || msg.Contains("429") || msg.Contains("quota"))
                    ? new ProcessOutput("", "quota exceeded")
                    : new ProcessOutput("", $"Download error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error during download: {ex}", LogLevel.ERROR);
                return new ProcessOutput("", $"Download error: {ex.Message}");
            }
        }

        private async Task<string> DownloadFileParallelAsync(
            RemoteFileInfo file, SemaphoreSlim semaphore, Action<long> bytesCallback, CancellationToken ct)
        {
            await semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                string result = await DownloadFileWithRetryAsync(file, bytesCallback, ct).ConfigureAwait(false);
                if (result == null) Interlocked.Increment(ref _completedFiles);
                return result;
            }
            finally { semaphore.Release(); }
        }

        private async Task<string> DownloadFileWithRetryAsync(
            RemoteFileInfo file, Action<long> bytesCallback, CancellationToken ct)
        {
            string destPath = Path.Combine(_destinationDir, file.Name);
            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    await DownloadFileWithResumeAsync(file, destPath, bytesCallback, ct).ConfigureAwait(false);
                    return null;
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    Logger.Log($"Download attempt {attempt + 1}/{MaxRetries + 1} failed for {file.Name}: {ex.Message}", LogLevel.WARNING);
                    if (attempt >= MaxRetries)
                        return $"Failed to download {file.Name} after {MaxRetries + 1} attempts: {ex.Message}";
                    await Task.Delay((int)Math.Pow(2, attempt) * 1000, ct).ConfigureAwait(false);
                }
            }
            return null;
        }

        private async Task DownloadFileWithResumeAsync(
            RemoteFileInfo file, string destPath, Action<long> bytesCallback, CancellationToken ct)
        {
            long existingSize = File.Exists(destPath) ? new FileInfo(destPath).Length : 0;
            if (file.Size > 0 && existingSize >= file.Size) return;

            using (var request = new HttpRequestMessage(HttpMethod.Get, file.Url))
            {
                if (existingSize > 0)
                    request.Headers.Range = new RangeHeaderValue(existingSize, null);

                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                    {
                        if (file.Size == 0)
                        {
                            file.Size = existingSize;
                            Interlocked.Add(ref _resolvedTotalBytes, existingSize);
                            Interlocked.Increment(ref _resolvedFileCount);
                        }
                        bytesCallback?.Invoke(0);
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                    bool isPartial = response.StatusCode == HttpStatusCode.PartialContent;
                    long contentLength = response.Content.Headers.ContentLength ?? 0;

                    // Server doesn't support Range — restart from scratch
                    if (existingSize > 0 && !isPartial)
                    {
                        Interlocked.Add(ref _globalDownloaded, -existingSize);
                        existingSize = 0;
                    }

                    // Resolve file size from GET response if Content-Length is known
                    if (file.Size == 0)
                    {
                        var cr = response.Content.Headers.ContentRange;
                        if (isPartial && cr?.Length.HasValue == true)
                            file.Size = cr.Length.Value;
                        else if (isPartial && contentLength > 0)
                            file.Size = existingSize + contentLength;
                        else if (contentLength > 0)
                            file.Size = contentLength;

                        if (file.Size > 0)
                        {
                            Interlocked.Add(ref _resolvedTotalBytes, file.Size);
                            Interlocked.Increment(ref _resolvedFileCount);
                        }
                    }

                    double bwLimit = _settings.BandwidthLimit > 0 ? _settings.BandwidthLimit * 1024.0 * 1024.0 : 0;

                    using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var fs = new FileStream(destPath, isPartial ? FileMode.Append : FileMode.Create,
                        FileAccess.Write, FileShare.None, BufferSize, true))
                    {
                        var buffer = new byte[BufferSize];
                        int bytesRead;
                        long bytesWritten = 0;
                        long tokenBucket = 0;
                        DateTime lastRefill = DateTime.UtcNow;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
                        {
                            ct.ThrowIfCancellationRequested();
                            await fs.WriteAsync(buffer, 0, bytesRead, ct).ConfigureAwait(false);
                            bytesWritten += bytesRead;
                            bytesCallback?.Invoke(bytesRead);

                            if (bwLimit > 0)
                            {
                                tokenBucket += bytesRead;
                                double elapsed = (DateTime.UtcNow - lastRefill).TotalSeconds;
                                if (elapsed > 0)
                                {
                                    tokenBucket -= (long)(bwLimit * elapsed);
                                    lastRefill = DateTime.UtcNow;
                                    if (tokenBucket < 0) tokenBucket = 0;
                                }
                                if (tokenBucket > 0)
                                {
                                    double sleep = tokenBucket / bwLimit;
                                    if (sleep > 0.01)
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(sleep), ct).ConfigureAwait(false);
                                        tokenBucket = 0;
                                        lastRefill = DateTime.UtcNow;
                                    }
                                }
                            }
                        }

                        // Resolve size from actual bytes if headers didn't provide it
                        if (file.Size == 0 && bytesWritten > 0)
                        {
                            file.Size = existingSize + bytesWritten;
                            Interlocked.Add(ref _resolvedTotalBytes, file.Size);
                            Interlocked.Increment(ref _resolvedFileCount);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            StopRcloneProxy();
        }
    }
}