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
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
    }

    public class ChunkedDownloader : IDisposable
    {
        private readonly string _baseUrl;
        private readonly string _destinationDir;
        private readonly HttpClient _httpClient;
        private readonly SettingsManager _settings;
        private Process? _rcloneProxyProcess;
        private int _completedFiles;
        private const int MaxRetries = 3;
        private const int BufferSize = 81920;

        private static readonly Regex HrefRegex = new Regex(
            @"href=""([^""]+)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ProxyPortRegex = new Regex(
            @"HTTP Server started on \[(http://127\.0\.0\.1:\d+/)\]",
            RegexOptions.Compiled);

        public ChunkedDownloader(string baseUrl, string destinationDir)
        {
            _destinationDir = destinationDir;
            _settings = SettingsManager.Instance;

            var uri = new Uri(baseUrl.TrimEnd('/'));
            string baseOrigin = uri.GetLeftPart(UriPartial.Authority);
            string remotePath = uri.AbsolutePath.TrimStart('/');

            string extraArgs = AppState.PublicMirrorExtraArgs ?? "";
            string rcloneArgs = $"serve http \":http:/{remotePath}\" --http-url {baseOrigin} --addr 127.0.0.1:0 --read-only {extraArgs}";

            _baseUrl = StartRcloneProxy(rcloneArgs, null);

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
        }

        public ChunkedDownloader(string remotePath, string destinationDir, string configPath, string? password = null)
        {
            _destinationDir = destinationDir;
            _settings = SettingsManager.Instance;

            string extraArgs = AppState.PublicMirrorExtraArgs ?? "";
            string pwArg = !string.IsNullOrEmpty(password) ? $" --ask-password=false" : "";
            string rcloneArgs = $"serve http \"{remotePath}\" --config \"{configPath}\" --addr 127.0.0.1:0 --read-only {extraArgs}{pwArg}";

            _baseUrl = StartRcloneProxy(rcloneArgs, password);

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
        }

        private string StartRcloneProxy(string args, string? password)
        {
            string rclonePath = Path.Combine(AppContext.BaseDirectory, "rclone", PlatformHelpers.RcloneBinaryName);

            var psi = new ProcessStartInfo(rclonePath, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "rclone")
            };

            if (!string.IsNullOrEmpty(password))
            {
                psi.EnvironmentVariables["RCLONE_CONFIG_PASS"] = password;
            }

            _rcloneProxyProcess = Process.Start(psi);
            if (_rcloneProxyProcess == null)
                throw new InvalidOperationException("Failed to start rclone proxy");

            // Read output until we find the proxy URL
            string proxyUrl = "";
            var startTime = DateTime.Now;
            while (string.IsNullOrEmpty(proxyUrl) && (DateTime.Now - startTime).TotalSeconds < 30)
            {
                string line = _rcloneProxyProcess.StandardOutput.ReadLine();
                if (line != null)
                {
                    var match = ProxyPortRegex.Match(line);
                    if (match.Success)
                    {
                        proxyUrl = match.Groups[1].Value;
                        break;
                    }
                }
                Thread.Sleep(100);
            }

            if (string.IsNullOrEmpty(proxyUrl))
                throw new TimeoutException("Rclone proxy did not start within 30 seconds");

            return proxyUrl;
        }

        public async Task<List<RemoteFileInfo>> ListFilesAsync()
        {
            var files = new List<RemoteFileInfo>();
            string html = await _httpClient.GetStringAsync(_baseUrl);

            var matches = HrefRegex.Matches(html);
            foreach (Match match in matches)
            {
                string href = match.Groups[1].Value;
                if (href == "../" || href.StartsWith("?"))
                    continue;

                string url = _baseUrl.TrimEnd('/') + "/" + href;
                string name = Uri.UnescapeDataString(href);

                files.Add(new RemoteFileInfo
                {
                    Name = name,
                    Url = url,
                    Size = await GetFileSizeAsync(url)
                });
            }

            return files;
        }

        private async Task<long> GetFileSizeAsync(string url)
        {
            try
            {
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                if (response.Content.Headers.ContentLength.HasValue)
                    return response.Content.Headers.ContentLength.Value;
            }
            catch { }
            return -1;
        }

        public async Task DownloadFileAsync(RemoteFileInfo file, IProgress<DownloadProgressInfo>? progress = null, CancellationToken cancellationToken = default)
        {
            string destinationPath = Path.Combine(_destinationDir, file.Name);
            Directory.CreateDirectory(_destinationDir);

            for (int attempt = 0; attempt < MaxRetries; attempt++)
            {
                try
                {
                    using var response = await _httpClient.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? file.Size;
                    long downloadedBytes = 0;
                    var stopwatch = Stopwatch.StartNew();

                    using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

                    byte[] buffer = new byte[BufferSize];
                    int bytesRead;
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        downloadedBytes += bytesRead;

                        if (progress != null && totalBytes > 0)
                        {
                            float percent = (float)(downloadedBytes * 100.0 / totalBytes);
                            double speed = downloadedBytes / stopwatch.Elapsed.TotalSeconds;
                            TimeSpan eta = speed > 0 ? TimeSpan.FromSeconds((totalBytes - downloadedBytes) / speed) : TimeSpan.Zero;

                            progress.Report(new DownloadProgressInfo
                            {
                                TotalBytes = totalBytes,
                                DownloadedBytes = downloadedBytes,
                                OverallPercent = percent,
                                SpeedBytesPerSecond = speed,
                                Eta = eta
                            });
                        }
                    }

                    Interlocked.Increment(ref _completedFiles);
                    return;
                }
                catch (Exception ex) when (attempt < MaxRetries - 1)
                {
                    Logger.Log($"Download attempt {attempt + 1} failed for {file.Name}: {ex.Message}", LogLevel.WARNING);
                    await Task.Delay(1000 * (attempt + 1), cancellationToken);
                }
            }

            throw new Exception($"Failed to download {file.Name} after {MaxRetries} attempts");
        }

        public async Task DownloadAllAsync(IProgress<DownloadProgressInfo>? progress = null, CancellationToken cancellationToken = default)
        {
            var files = await ListFilesAsync();
            int totalFiles = files.Count;
            long totalBytes = files.Sum(f => Math.Max(f.Size, 0));
            long downloadedBytes = 0;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var fileProgress = new Progress<DownloadProgressInfo>(info =>
                {
                    long overallDownloaded = downloadedBytes + info.DownloadedBytes;
                    float overallPercent = totalBytes > 0 ? (float)(overallDownloaded * 100.0 / totalBytes) : 0;

                    progress?.Report(new DownloadProgressInfo
                    {
                        TotalBytes = totalBytes,
                        DownloadedBytes = overallDownloaded,
                        CurrentFileIndex = i + 1,
                        TotalFiles = totalFiles,
                        OverallPercent = overallPercent,
                        SpeedBytesPerSecond = info.SpeedBytesPerSecond,
                        Eta = info.Eta
                    });
                });

                await DownloadFileAsync(file, fileProgress, cancellationToken);
                downloadedBytes += Math.Max(file.Size, 0);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            try
            {
                _rcloneProxyProcess?.Kill();
                _rcloneProxyProcess?.Dispose();
            }
            catch { }
        }
    }
}
