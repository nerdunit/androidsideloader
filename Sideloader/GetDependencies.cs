using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal class GetDependencies
    {
        // Download required dependencies
        public static void downloadFiles()
        {
            // Initialize DNS helper early to detect and configure fallback if needed
            DnsHelper.Initialize();

            WebClient client = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var currentAccessedWebsite = "";
            try
            {
                if (!File.Exists("Sideloader Launcher.exe"))
                {
                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing 'Sideloader Launcher.exe'. Attempting to download from {currentAccessedWebsite}");
                    DownloadFileWithDnsFallback(client, $"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/Sideloader%20Launcher.exe", "Sideloader Launcher.exe");
                    _ = Logger.Log($"'Sideloader Launcher.exe' download successful");
                }

                if (!File.Exists("Rookie Offline.cmd"))
                {
                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing 'Rookie Offline.cmd'. Attempting to download from {currentAccessedWebsite}");
                    DownloadFileWithDnsFallback(client, $"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/Rookie%20Offline.cmd", "Rookie Offline.cmd");
                    _ = Logger.Log($"'Rookie Offline.cmd' download successful");
                }

                if (!File.Exists("CleanupInstall.cmd"))
                {
                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing 'CleanupInstall.cmd'. Attempting to download from {currentAccessedWebsite}");
                    DownloadFileWithDnsFallback(client, $"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/CleanupInstall.cmd", "CleanupInstall.cmd");
                    _ = Logger.Log($"'CleanupInstall.cmd' download successful");
                }

                if (!File.Exists("AddDefenderExceptions.ps1"))
                {
                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing 'AddDefenderExceptions.ps1'. Attempting to download from {currentAccessedWebsite}");
                    DownloadFileWithDnsFallback(client, $"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/AddDefenderExceptions.ps1", "AddDefenderExceptions.ps1");
                    _ = Logger.Log($"'AddDefenderExceptions.ps1' download successful");
                }
            }
            catch (Exception ex)
            {
                _ = FlexibleMessageBox.Show(Program.form, $"You are unable to access raw.githubusercontent.com with the Exception:\n{ex.Message}\n\nSome files may be missing (Offline/Cleanup Script, Launcher)");
            }

            string adbPath = Path.Combine(Environment.CurrentDirectory, "platform-tools", "adb.exe");
            string platformToolsDir = Path.Combine(Environment.CurrentDirectory, "platform-tools");

            try
            {
                if (!File.Exists(adbPath)) //if adb is not updated, download and auto extract
                {
                    if (!Directory.Exists(platformToolsDir))
                    {
                        _ = Directory.CreateDirectory(platformToolsDir);
                    }

                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing adb within {platformToolsDir}. Attempting to download from {currentAccessedWebsite}");
                    DownloadFileWithDnsFallback(client, $"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/dependencies.7z", "dependencies.7z");
                    Utilities.Zip.ExtractFile(Path.Combine(Environment.CurrentDirectory, "dependencies.7z"), platformToolsDir);
                    File.Delete("dependencies.7z");
                    _ = Logger.Log($"adb download successful");
                }
            }
            catch (Exception ex)
            {
                _ = FlexibleMessageBox.Show(Program.form, $"You are unable to access raw.githubusercontent.com page with the Exception:\n{ex.Message}\n\nSome files may be missing (ADB)");
                _ = FlexibleMessageBox.Show(Program.form, "ADB was unable to be downloaded\nRookie will now close.");
                Application.Exit();
            }

            string wantedRcloneVersion = "1.72.1";
            bool rcloneSuccess = false;

            rcloneSuccess = downloadRclone(wantedRcloneVersion, false);
            if (!rcloneSuccess)
            {
                rcloneSuccess = downloadRclone(wantedRcloneVersion, true);
            }
            if (!rcloneSuccess)
            {
                _ = Logger.Log($"Unable to download rclone", LogLevel.ERROR);
                _ = FlexibleMessageBox.Show(Program.form, "Rclone was unable to be downloaded\nRookie will now close, please use Offline Mode for manual sideloading if needed");
                Application.Exit();
            }

            // Download WebView2 runtime if needed
            downloadWebView2Runtime();
        }

        // Downloads a file using the DNS fallback proxy if active
        public static void DownloadFileWithDnsFallback(WebClient client, string url, string localPath)
        {
            try
            {
                // Use DNS fallback proxy if active
                if (DnsHelper.UseFallbackDns && !string.IsNullOrEmpty(DnsHelper.ProxyUrl))
                {
                    client.Proxy = new WebProxy(DnsHelper.ProxyUrl);
                }

                client.DownloadFile(url, localPath);
            }
            catch (Exception ex)
            {
                _ = Logger.Log($"Download failed for {url}: {ex.Message}", LogLevel.ERROR);
                throw;
            }
            finally
            {
                // Reset proxy to avoid affecting other operations
                client.Proxy = null;
            }
        }

        // Overload that creates its own WebClient for convenience
        public static void DownloadFileWithDnsFallback(string url, string localPath)
        {
            using (var client = new WebClient())
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                DownloadFileWithDnsFallback(client, url, localPath);
            }
        }

        // Downloads WebView2 runtime if not present
        private static void downloadWebView2Runtime()
        {
            string runtimesPath = Path.Combine(Environment.CurrentDirectory, "runtimes");
            string webView2LoaderArm64 = Path.Combine(runtimesPath, "win-arm64", "native", "WebView2Loader.dll");
            string webView2LoaderX86 = Path.Combine(runtimesPath, "win-x86", "native", "WebView2Loader.dll");
            string webView2LoaderX64 = Path.Combine(runtimesPath, "win-x64", "native", "WebView2Loader.dll");

            bool runtimeExists = File.Exists(webView2LoaderX86) || File.Exists(webView2LoaderX64) || File.Exists(webView2LoaderArm64);

            if (!runtimeExists)
            {
                string archivePath = Path.Combine(Environment.CurrentDirectory, "runtimes.7z");
                string tempArchivePath = archivePath + ".tmp";

                try
                {
                    _ = Logger.Log("Missing WebView2 runtime. Attempting to download...");

                    // Clean up any leftover archives from a previous failed attempt
                    try { if (File.Exists(archivePath)) File.Delete(archivePath); } catch { }
                    try { if (File.Exists(tempArchivePath)) File.Delete(tempArchivePath); } catch { }

                    // Download to a temp file first so a failed download never leaves
                    // a corrupt archive at the final path
                    DownloadFileWithDnsFallback(
                        $"https://github.com/{MainForm.repo}/raw/{MainForm.repo_branch}/runtimes.7z",
                        tempArchivePath);

                    // Verify the download produced a valid file (runtimes.7z is ~1 MB+)
                    if (!File.Exists(tempArchivePath) || new FileInfo(tempArchivePath).Length < 1024)
                    {
                        _ = Logger.Log("Downloaded runtimes.7z is missing or too small — aborting extraction", LogLevel.ERROR);
                        try { if (File.Exists(tempArchivePath)) File.Delete(tempArchivePath); } catch { }
                        return;
                    }

                    // Rename to final path only after successful download
                    File.Move(tempArchivePath, archivePath);

                    _ = Logger.Log("Extracting WebView2 runtime...");
                    Utilities.Zip.ExtractFile(archivePath, Environment.CurrentDirectory);

                    // Verify extraction actually produced the expected DLLs
                    bool extractionOk = File.Exists(webView2LoaderX86) && File.Exists(webView2LoaderX64) && File.Exists(webView2LoaderArm64);
                    if (extractionOk)
                    {
                        _ = Logger.Log("WebView2 runtime download successful");
                    }
                    else
                    {
                        _ = Logger.Log("Extraction completed but WebView2Loader.dll not found — archive may be corrupt", LogLevel.ERROR);
                    }
                }
                catch (Exception ex)
                {
                    _ = Logger.Log($"Failed to download WebView2 runtime: {ex.Message}", LogLevel.ERROR);
                }
                finally
                {
                    // Always clean up archive files so a corrupt download is never left behind
                    try { if (File.Exists(archivePath)) File.Delete(archivePath); } catch { }
                    try { if (File.Exists(tempArchivePath)) File.Delete(tempArchivePath); } catch { }
                }
            }
        }

        public static bool downloadRclone(string wantedRcloneVersion, bool useFallback = false)
        {
            try
            {
                bool updateRclone = false;
                string currentRcloneVersion = "0.0.0";

                WebClient client = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                _ = Logger.Log($"Checking for Local rclone...");
                string dirRclone = Path.Combine(Environment.CurrentDirectory, "rclone");
                string pathToRclone = Path.Combine(dirRclone, "rclone.exe");
                if (File.Exists(pathToRclone))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(pathToRclone);
                    currentRcloneVersion = versionInfo.ProductVersion;
                    Logger.Log($"Current RCLONE Version {currentRcloneVersion}");
                    if (!MainForm.noRcloneUpdating)
                    {
                        if (currentRcloneVersion != wantedRcloneVersion)
                        {
                            updateRclone = true;
                            _ = Logger.Log($"RCLONE Version does not match ({currentRcloneVersion})! Downloading required version ({wantedRcloneVersion})");
                        }
                    }
                }
                else
                {
                    updateRclone = true;
                    _ = Logger.Log($"RCLONE exe does not exist, attempting to download");
                }

                if (!Directory.Exists(dirRclone))
                {
                    updateRclone = true;
                    _ = Logger.Log($"Missing RCLONE Folder, attempting to download");

                    Directory.CreateDirectory(dirRclone);
                }

                if (updateRclone == true)
                {
                    // Preserve download.config if it exists
                    string configPath = Path.Combine(dirRclone, "download.config");
                    string tempConfigPath = Path.Combine(Environment.CurrentDirectory, "download.config.bak");
                    bool hasConfig = false;

                    if (File.Exists(configPath))
                    {
                        _ = Logger.Log("Preserving download.config before update");
                        File.Copy(configPath, tempConfigPath, true);
                        hasConfig = true;
                    }

                    string architecture = Environment.Is64BitOperatingSystem ? "amd64" : "386";
                    string url = $"https://downloads.rclone.org/v{wantedRcloneVersion}/rclone-v{wantedRcloneVersion}-windows-{architecture}.zip";
                    if (useFallback == true)
                    {
                        _ = Logger.Log($"Using git fallback for rclone download");
                        url = $"https://raw.githubusercontent.com/{MainForm.repo}/{MainForm.repo_branch}/dep/rclone-v{wantedRcloneVersion}-windows-{architecture}.zip";
                    }
                    _ = Logger.Log($"Downloading rclone from {url}");

                    _ = Logger.Log("Begin download rclone");
                    DownloadFileWithDnsFallback(client, url, "rclone.zip");
                    _ = Logger.Log("Complete download rclone");

                    _ = Logger.Log($"Extract {Environment.CurrentDirectory}\\rclone.zip");
                    Utilities.Zip.ExtractFile(Path.Combine(Environment.CurrentDirectory, "rclone.zip"), Environment.CurrentDirectory);
                    string dirExtractedRclone = Path.Combine(Environment.CurrentDirectory, $"rclone-v{wantedRcloneVersion}-windows-{architecture}");
                    File.Delete("rclone.zip");
                    _ = Logger.Log("rclone extracted. Moving files");

                    foreach (string file in Directory.GetFiles(dirExtractedRclone))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(dirRclone, fileName);
                        if (File.Exists(destFile))
                        {
                            File.Delete(destFile);
                        }
                        File.Move(file, destFile);
                    }
                    FileSystemUtilities.TryDeleteDirectory(dirExtractedRclone);

                    // Restore download.config if it was backed up
                    if (hasConfig && File.Exists(tempConfigPath))
                    {
                        _ = Logger.Log("Restoring download.config after update");
                        File.Move(tempConfigPath, configPath);
                    }

                    _ = Logger.Log($"rclone download successful");
                }

                return true;
            }
            catch (Exception ex)
            {
                _ = Logger.Log($"Unable to download rclone: {ex}", LogLevel.ERROR);
                return false;
            }
        }
    }
}