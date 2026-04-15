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

        private static readonly Regex HrefRegex = new Regex(
            @"href=""([^""]+)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ProxyPortRegex = new Regex(
            @"HTTP Server started on \[(http://127\.0\.0\.1:\d+/)\]",
            RegexOptions.Compiled);

        /// <summary>
        /// Creates a ChunkedDownloader for public mirrors.
        /// Starts an rclone serve http proxy targeting the <c>:http:</c> backend.
        /// </summary>
        public ChunkedDownloader(string baseUrl, string destinationDir)
        {
            _destinationDir = destinationDir;
            _settings = SettingsManager.Instance;

            var uri = new Uri(baseUrl.TrimEnd('/'));
            string baseOrigin = uri.GetLeftPart(UriPartial.Authority);
            string remotePath = uri.AbsolutePath.TrimStart('/');

            string extraArgs = MainForm.PublicMirrorExtraArgs ?? "";
            string rcloneArgs = $"serve http \":http:/{remotePath}\" --http-url {baseOrigin} --addr 127.0.0.1:0 --read-only {extraArgs}";

            _baseUrl = StartRcloneProxy(rcloneArgs, null);

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
        }

        /// <summary>
        /// Creates a ChunkedDownloader for config-based mirrors (private mirrors).
        /// Starts an rclone serve http proxy targeting the named remote via the given config file.
        /// </summary>
        public ChunkedDownloader(string remotePath, string destinationDir, string configPath, string password = null)
        {
            _destinationDir = destinationDir;
            _settings = SettingsManager.Instance;

            string configArg = !string.IsNullOrEmpty(configPath) ? $"--config \"{configPath}\"" : "";
            string pwArg = !string.IsNullOrEmpty(password) ? "--ask-password=false" : "";
            string rcloneArgs = $"serve http \"{remotePath}\" --addr 127.0.0.1:0 --read-only {configArg} {pwArg}";

            _baseUrl = StartRcloneProxy(rcloneArgs, password);

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
        }

        /// <summary>
        /// Starts <c>rclone serve http</c> on a random localhost port.
        /// Returns the local base URL (e.g. <c>http://127.0.0.1:12345</c>).
        /// rclone handles TLS and proxy.
        /// </summary>
        private string StartRcloneProxy(string rcloneArgs, string rclonePassword)
        {
            string rclonePath = Path.Combine(Environment.CurrentDirectory, "rclone", "rclone.exe");
            if (!File.Exists(rclonePath))
                throw new FileNotFoundException("rclone.exe not found at: " + rclonePath);

            var psi = new ProcessStartInfo
            {
                FileName = rclonePath,
                Arguments = rcloneArgs,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "rclone")
            };

            if (!string.IsNullOrEmpty(rclonePassword))
            {
                psi.EnvironmentVariables["RCLONE_CONFIG_PASS"] = rclonePassword;
            }

            // Mirror rclone proxy setup from RCLONE.setRcloneProxy
            if (_settings.useProxy &&
                !string.IsNullOrEmpty(_settings.ProxyAddress) &&
                !string.IsNullOrEmpty(_settings.ProxyPort))
            {
                string proxyUrl = $"http://{_settings.ProxyAddress}:{_settings.ProxyPort}";
                psi.EnvironmentVariables["HTTP_PROXY"] = proxyUrl;
                psi.EnvironmentVariables["HTTPS_PROXY"] = proxyUrl;
                psi.EnvironmentVariables["http_proxy"] = proxyUrl;
                psi.EnvironmentVariables["https_proxy"] = proxyUrl;
            }
            else
            {
                string dnsProxy = DnsHelper.ProxyUrl;
                if (!string.IsNullOrEmpty(dnsProxy))
                {
                    psi.EnvironmentVariables["HTTP_PROXY"] = dnsProxy;
                    psi.EnvironmentVariables["HTTPS_PROXY"] = dnsProxy;
                    psi.EnvironmentVariables["http_proxy"] = dnsProxy;
                    psi.EnvironmentVariables["https_proxy"] = dnsProxy;
                }
            }

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
            catch (Exception ex)
            {
                Logger.Log($"Error stopping rclone proxy: {ex.Message}", LogLevel.WARNING);
            }
            finally
            {
                _rcloneProxyProcess?.Dispose();
                _rcloneProxyProcess = null;
            }
        }

        /// <summary>
        /// Lists files available at the remote URL by parsing the HTML directory listing.
        /// Resolves file sizes via HEAD requests.
        /// </summary>
        public async Task<List<RemoteFileInfo>> ListRemoteFilesAsync(CancellationToken ct)
        {
            var files = new List<RemoteFileInfo>();
            string listUrl = _baseUrl + "/";

            string html = await _httpClient.GetStringAsync(listUrl).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            // Build the base URI for resolving absolute hrefs
            var baseUri = new Uri(listUrl);

            var matches = HrefRegex.Matches(html);
            foreach (Match m in matches)
            {
                string href = m.Groups[1].Value;

                // Skip parent directory links, query strings, directory entries,
                // fragment-only anchors (rclone serve HTML icons like #file, #zip-folder),
                // and bare ".." entries.
                if (href == "../" || href == ".." ||
                    href.StartsWith("?") || href.StartsWith("#") ||
                    href.EndsWith("/"))
                    continue;

                string fileName;
                string fileUrl;

                if (href.StartsWith("http://") || href.StartsWith("https://"))
                {
                    // Fully qualified URL
                    fileUrl = href;
                    fileName = Uri.UnescapeDataString(href.Substring(href.LastIndexOf('/') + 1));
                }
                else if (href.StartsWith("/"))
                {
                    // Absolute path — resolve against the server origin
                    var resolved = new Uri(baseUri, href);
                    fileUrl = resolved.AbsoluteUri;
                    fileName = Uri.UnescapeDataString(resolved.Segments[resolved.Segments.Length - 1]);
                }
                else
                {
                    // Relative path (most common)
                    fileName = Uri.UnescapeDataString(href);
                    fileUrl = _baseUrl + "/" + href;
                }

                if (string.IsNullOrEmpty(fileName) || fileName == "/")
                    continue;

                files.Add(new RemoteFileInfo
                {
                    Name = fileName,
                    Url = fileUrl,
                    Size = 0
                });
            }

            // Resolve sizes via HEAD requests
            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Head, file.Url))
                    using (var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false))
                    {
                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            file.Size = response.Content.Headers.ContentLength.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"HEAD request failed for {file.Name}: {ex.Message}", LogLevel.WARNING);
                }
            }

            return files;
        }

        /// <summary>
        /// Downloads all remote files to the destination directory with byte-level resume support.
        /// Returns a ProcessOutput compatible with the existing error-handling pipeline.
        /// </summary>
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

                long totalBytes = files.Sum(f => f.Size);
                long alreadyDownloaded = 0;

                // Calculate already-downloaded bytes for resume
                foreach (var file in files)
                {
                    string destPath = Path.Combine(_destinationDir, file.Name);
                    if (File.Exists(destPath))
                    {
                        long existingSize = new FileInfo(destPath).Length;
                        if (file.Size > 0 && existingSize >= file.Size)
                        {
                            alreadyDownloaded += file.Size;
                        }
                        else
                        {
                            alreadyDownloaded += existingSize;
                        }
                    }
                }

                long globalDownloaded = alreadyDownloaded;

                // Count already-complete files for accurate progress on resume
                _completedFiles = 0;
                foreach (var file in files)
                {
                    string destPath = Path.Combine(_destinationDir, file.Name);
                    if (File.Exists(destPath) && file.Size > 0 && new FileInfo(destPath).Length >= file.Size)
                        _completedFiles++;
                }

                // EWMA speed tracking
                double ewmaSpeed = 0;
                const double alpha = 0.3;
                DateTime lastProgressTime = DateTime.UtcNow;
                long bytesSinceLastReport = 0;

                Action<long> bytesCallback = (bytesJustWritten) =>
                {
                    globalDownloaded += bytesJustWritten;
                    bytesSinceLastReport += bytesJustWritten;
                    DateTime now = DateTime.UtcNow;
                    double elapsed = (now - lastProgressTime).TotalSeconds;

                    if (elapsed >= 0.25) // Throttle to ~4 updates/sec
                    {
                        double instantSpeed = bytesSinceLastReport / elapsed;
                        ewmaSpeed = ewmaSpeed < 1 ? instantSpeed : (alpha * instantSpeed) + ((1 - alpha) * ewmaSpeed);
                        lastProgressTime = now;
                        bytesSinceLastReport = 0;

                        float percent = totalBytes > 0
                            ? Math.Min(99f, (float)globalDownloaded / totalBytes * 100f)
                            : 0f;

                        TimeSpan eta = ewmaSpeed > 0
                            ? TimeSpan.FromSeconds((totalBytes - globalDownloaded) / ewmaSpeed)
                            : TimeSpan.Zero;

                        onProgress?.Invoke(new DownloadProgressInfo
                        {
                            TotalBytes = totalBytes,
                            DownloadedBytes = globalDownloaded,
                            CurrentFileIndex = _completedFiles + 1,
                            TotalFiles = files.Count,
                            SpeedBytesPerSecond = ewmaSpeed,
                            OverallPercent = percent,
                            Eta = eta
                        });
                    }
                };

                // Determine concurrency
                int maxConcurrency = _settings.SingleThreadMode ? 1 : Math.Min(4, files.Count);
                var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

                var tasks = new List<Task<string>>();
                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    string destPath = Path.Combine(_destinationDir, file.Name);
                    if (File.Exists(destPath) && file.Size > 0 && new FileInfo(destPath).Length >= file.Size)
                        continue;
                    tasks.Add(DownloadFileParallelAsync(file, semaphore, bytesCallback, ct));
                }

                string[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

                // Collect errors
                var errors = results.Where(r => r != null).ToList();
                if (errors.Count > 0)
                {
                    string combinedError = string.Join("\n", errors);
                    return new ProcessOutput("", combinedError);
                }

                // Final 100% progress
                onProgress?.Invoke(new DownloadProgressInfo
                {
                    TotalBytes = totalBytes,
                    DownloadedBytes = totalBytes,
                    CurrentFileIndex = files.Count,
                    TotalFiles = files.Count,
                    SpeedBytesPerSecond = ewmaSpeed,
                    OverallPercent = 100f,
                    Eta = TimeSpan.Zero
                });

                return new ProcessOutput("OK", "");
            }
            catch (OperationCanceledException)
            {
                return new ProcessOutput("", "Cancelled");
            }
            catch (HttpRequestException ex)
            {
                Logger.Log($"HttpRequestException during download: {ex}", LogLevel.ERROR);
                string msg = ex.Message.ToLower();
                if (msg.Contains("403") || msg.Contains("429") || msg.Contains("quota"))
                {
                    return new ProcessOutput("", "quota exceeded");
                }
                return new ProcessOutput("", $"Download error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error during download: {ex}", LogLevel.ERROR);
                return new ProcessOutput("", $"Download error: {ex.Message}");
            }
        }

        private async Task<string> DownloadFileParallelAsync(
            RemoteFileInfo file,
            SemaphoreSlim semaphore,
            Action<long> bytesCallback,
            CancellationToken ct)
        {
            await semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                string result = await DownloadFileWithRetryAsync(file, bytesCallback, ct).ConfigureAwait(false);
                if (result == null)
                    Interlocked.Increment(ref _completedFiles);
                return result;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<string> DownloadFileWithRetryAsync(
            RemoteFileInfo file,
            Action<long> bytesCallback,
            CancellationToken ct)
        {
            string destPath = Path.Combine(_destinationDir, file.Name);

            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    await DownloadFileWithResumeAsync(file, destPath, bytesCallback, ct).ConfigureAwait(false);
                    return null; // success
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.Log($"Download attempt {attempt + 1}/{MaxRetries + 1} failed for {file.Name}: {ex.Message}", LogLevel.WARNING);

                    if (attempt >= MaxRetries)
                    {
                        return $"Failed to download {file.Name} after {MaxRetries + 1} attempts: {ex.Message}";
                    }

                    // Exponential backoff: 1s, 2s, 4s
                    int delayMs = (int)Math.Pow(2, attempt) * 1000;
                    await Task.Delay(delayMs, ct).ConfigureAwait(false);
                }
            }

            return null;
        }

        private async Task DownloadFileWithResumeAsync(
            RemoteFileInfo file,
            string destPath,
            Action<long> bytesCallback,
            CancellationToken ct)
        {
            long existingSize = 0;
            if (File.Exists(destPath))
            {
                existingSize = new FileInfo(destPath).Length;

                // File already complete
                if (file.Size > 0 && existingSize >= file.Size)
                    return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, file.Url);
            if (existingSize > 0)
            {
                request.Headers.Range = new RangeHeaderValue(existingSize, null);
            }

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                    return;

                response.EnsureSuccessStatusCode();

                bool isPartialContent = response.StatusCode == HttpStatusCode.PartialContent;

                // If server doesn't support Range and we had partial data, restart from scratch
                if (existingSize > 0 && !isPartialContent)
                {
                    existingSize = 0;
                }

                FileMode fileMode = isPartialContent ? FileMode.Append : FileMode.Create;

                // Bandwidth throttle setup
                double bandwidthLimitBps = _settings.BandwidthLimit > 0
                    ? _settings.BandwidthLimit * 1024.0 * 1024.0 // MB/s to bytes/s
                    : 0;

                using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var fileStream = new FileStream(destPath, fileMode, FileAccess.Write, FileShare.None, BufferSize, true))
                {
                    var buffer = new byte[BufferSize];
                    int bytesRead;
                    long tokenBucket = 0;
                    DateTime lastTokenRefill = DateTime.UtcNow;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
                    {
                        ct.ThrowIfCancellationRequested();

                        await fileStream.WriteAsync(buffer, 0, bytesRead, ct).ConfigureAwait(false);
                        bytesCallback?.Invoke(bytesRead);

                        // Token bucket bandwidth throttling
                        if (bandwidthLimitBps > 0)
                        {
                            tokenBucket += bytesRead;
                            DateTime now = DateTime.UtcNow;
                            double elapsed = (now - lastTokenRefill).TotalSeconds;

                            if (elapsed > 0)
                            {
                                double allowedBytes = bandwidthLimitBps * elapsed;
                                tokenBucket -= (long)allowedBytes;
                                lastTokenRefill = now;

                                if (tokenBucket < 0) tokenBucket = 0;
                            }

                            if (tokenBucket > 0)
                            {
                                double sleepSeconds = tokenBucket / bandwidthLimitBps;
                                if (sleepSeconds > 0.01)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(sleepSeconds), ct).ConfigureAwait(false);
                                    tokenBucket = 0;
                                    lastTokenRefill = DateTime.UtcNow;
                                }
                            }
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
