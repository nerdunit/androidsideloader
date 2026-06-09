using AndroidSideloader.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    public class GetDependencies
    {
        public static void DownloadFiles()
        {
            DnsHelper.Initialize();

            // Windows-only files: skip on non-Windows
            if (PlatformHelpers.IsWindows)
            {
                DownloadWindowsExtras();
            }

            // Download/update adb platform-tools
            DownloadAdb();

            // Download/update rclone
            DownloadRclone();
        }

        private static void DownloadWindowsExtras()
        {
            try
            {
                using var client = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // These are Windows-specific helper files
                DownloadFileWithDnsFallback(client, $"https://github.com/{RepoDefaults.Repo}/raw/{RepoDefaults.Branch}/Sideloader%20Launcher.exe", "Sideloader Launcher.exe");
                DownloadFileWithDnsFallback(client, $"https://github.com/{RepoDefaults.Repo}/raw/{RepoDefaults.Branch}/Rookie%20Offline.cmd", "Rookie Offline.cmd");
                DownloadFileWithDnsFallback(client, $"https://github.com/{RepoDefaults.Repo}/raw/{RepoDefaults.Branch}/CleanupInstall.cmd", "CleanupInstall.cmd");
                DownloadFileWithDnsFallback(client, $"https://github.com/{RepoDefaults.Repo}/raw/{RepoDefaults.Branch}/AddDefenderExceptions.ps1", "AddDefenderExceptions.ps1");
            }
            catch (Exception ex)
            {
                Logger.Log($"Windows extras download failed: {ex.Message}", LogLevel.WARNING);
            }
        }

        private static void DownloadAdb()
        {
            string adbPath = Path.Combine(AppContext.BaseDirectory, "platform-tools", PlatformHelpers.AdbBinaryName);
            string platformToolsDir = Path.Combine(AppContext.BaseDirectory, "platform-tools");

            try
            {
                if (!File.Exists(adbPath))
                {
                    if (!Directory.Exists(platformToolsDir))
                    {
                        _ = Directory.CreateDirectory(platformToolsDir);
                    }

                    Logger.Log($"Missing adb. Attempting to download...");
                    string url = PlatformHelpers.GetAdbDownloadUrl();
                    string zipPath = Path.Combine(AppContext.BaseDirectory, "platform-tools.zip");

                    using var client = new WebClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    DownloadFileWithDnsFallback(client, url, zipPath);

                    Utilities.Zip.ExtractFile(zipPath, AppContext.BaseDirectory);
                    File.Delete(zipPath);
                    Logger.Log("adb download successful");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ADB download failed: {ex.Message}", LogLevel.ERROR);
                throw;
            }
        }

        private static void DownloadRclone()
        {
            string wantedRcloneVersion = "1.72.1";
            bool success = false;

            success = DownloadRcloneVersion(wantedRcloneVersion, false);
            if (!success)
                success = DownloadRcloneVersion(wantedRcloneVersion, true);

            if (!success)
            {
                Logger.Log("Unable to download rclone", LogLevel.ERROR);
                throw new Exception("Rclone was unable to be downloaded. Please use Offline Mode for manual sideloading if needed.");
            }
        }

        private static bool DownloadRcloneVersion(string wantedVersion, bool useFallback)
        {
            try
            {
                bool updateRclone = false;
                string currentRcloneVersion = "0.0.0";

                string dirRclone = Path.Combine(AppContext.BaseDirectory, "rclone");
                string pathToRclone = Path.Combine(dirRclone, PlatformHelpers.RcloneBinaryName);

                if (File.Exists(pathToRclone))
                {
                    try
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(pathToRclone);
                        currentRcloneVersion = versionInfo.ProductVersion ?? "0.0.0";
                    }
                    catch
                    {
                        // rclone binary may not have version info on unix; try running --version
                        try
                        {
                            var psi = new ProcessStartInfo(pathToRclone, "--version")
                            {
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            using var proc = Process.Start(psi);
                            string output = proc?.StandardOutput.ReadLine() ?? "";
                            proc?.WaitForExit();
                            // Output looks like "rclone v1.72.1"
                            var match = System.Text.RegularExpressions.Regex.Match(output, @"v(\d+\.\d+\.\d+)");
                            if (match.Success)
                                currentRcloneVersion = match.Groups[1].Value;
                        }
                        catch { }
                    }

                    Logger.Log($"Current RCLONE Version {currentRcloneVersion}");
                    if (!AppState.NoRcloneUpdating)
                    {
                        if (currentRcloneVersion != wantedVersion)
                        {
                            updateRclone = true;
                            Logger.Log($"RCLONE Version does not match ({currentRcloneVersion})! Downloading required version ({wantedVersion})");
                        }
                    }
                }
                else
                {
                    updateRclone = true;
                    Logger.Log("RCLONE exe does not exist, attempting to download");
                }

                if (!Directory.Exists(dirRclone))
                {
                    updateRclone = true;
                    Logger.Log("Missing RCLONE Folder, attempting to download");
                    Directory.CreateDirectory(dirRclone);
                }

                if (updateRclone)
                {
                    string configPath = Path.Combine(dirRclone, "download.config");
                    string tempConfigPath = Path.Combine(AppContext.BaseDirectory, "download.config.bak");
                    bool hasConfig = false;

                    if (File.Exists(configPath))
                    {
                        Logger.Log("Preserving download.config before update");
                        File.Copy(configPath, tempConfigPath, true);
                        hasConfig = true;
                    }

                    string url = PlatformHelpers.GetRcloneDownloadUrl(wantedVersion, useFallback);
                    Logger.Log($"Downloading rclone from {url}");

                    using var client = new WebClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string zipPath = Path.Combine(AppContext.BaseDirectory, "rclone.zip");
                    DownloadFileWithDnsFallback(client, url, zipPath);

                    Logger.Log("Extracting rclone...");
                    Utilities.Zip.ExtractFile(zipPath, AppContext.BaseDirectory);
                    File.Delete(zipPath);

                    // Find extracted folder (name varies by OS)
                    string extractedDir = Directory.GetDirectories(AppContext.BaseDirectory)
                        .FirstOrDefault(d => Path.GetFileName(d).StartsWith("rclone-v" + wantedVersion, StringComparison.OrdinalIgnoreCase))
                        ?? Path.Combine(AppContext.BaseDirectory, $"rclone-v{wantedVersion}-windows-amd64");

                    if (Directory.Exists(extractedDir))
                    {
                        foreach (string file in Directory.GetFiles(extractedDir))
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(dirRclone, fileName);
                            if (File.Exists(destFile))
                            {
                                File.Delete(destFile);
                            }
                            File.Move(file, destFile);
                        }

                        // On Unix, make binary executable
                        if (!PlatformHelpers.IsWindows)
                        {
                            string binary = Path.Combine(dirRclone, PlatformHelpers.RcloneBinaryName);
                            if (File.Exists(binary))
                            {
                                try
                                {
                                    Process.Start("chmod", $"+x \"{binary}\"")?.WaitForExit();
                                }
                                catch { }
                            }
                        }

                        FileSystemUtilities.TryDeleteDirectory(extractedDir);
                    }

                    if (hasConfig && File.Exists(tempConfigPath))
                    {
                        Logger.Log("Restoring download.config after update");
                        File.Move(tempConfigPath, configPath);
                    }

                    Logger.Log("rclone download successful");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Unable to download rclone: {ex}", LogLevel.ERROR);
                return false;
            }
        }

        public static void DownloadFileWithDnsFallback(WebClient client, string url, string localPath)
        {
            try
            {
                if (DnsHelper.UseFallbackDns && !string.IsNullOrEmpty(DnsHelper.ProxyUrl))
                {
                    client.Proxy = new WebProxy(DnsHelper.ProxyUrl);
                }

                client.DownloadFile(url, localPath);
            }
            catch (Exception ex)
            {
                Logger.Log($"Download failed for {url}: {ex.Message}", LogLevel.ERROR);
                throw;
            }
            finally
            {
                client.Proxy = null;
            }
        }

        public static void DownloadFileWithDnsFallback(string url, string localPath)
        {
            using var client = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            DownloadFileWithDnsFallback(client, url, localPath);
        }
    }
}
