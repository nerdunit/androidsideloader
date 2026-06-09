using AndroidSideloader.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AndroidSideloader
{
    public class RCLONE
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;
        private static readonly List<int> _trackedRclonePids = new List<int>();

        public static string RcloneDirectory => Path.Combine(AppContext.BaseDirectory, "rclone");
        public static string RcloneExePath => Path.Combine(RcloneDirectory, PlatformHelpers.RcloneBinaryName);

        public static void Init()
        {
            string pwTxtPath = Path.Combine(RcloneDirectory, "pw.txt");
            if (File.Exists(pwTxtPath))
            {
                rclonepw = File.ReadAllText(pwTxtPath);
            }
        }

        public static string downloadConfigPath = "download.config";
        public static string uploadConfigPath = "upload.config";
        public static string rclonepw = "";

        public static void KillRclone()
        {
            lock (_trackedRclonePids)
            {
                foreach (int pid in _trackedRclonePids)
                {
                    try
                    {
                        var proc = Process.GetProcessById(pid);
                        if (proc != null && !proc.HasExited)
                        {
                            proc.Kill();
                            proc.WaitForExit(2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to kill rclone pid {pid}: {ex.Message}", LogLevel.WARNING);
                    }
                }
                _trackedRclonePids.Clear();
            }

            // Fallback: kill any rclone processes by name
            try
            {
                string procName = PlatformHelpers.IsWindows ? "rclone" : "rclone";
                foreach (var proc in Process.GetProcessesByName(procName))
                {
                    try { proc.Kill(); } catch { }
                }
            }
            catch { }
        }

        private static Process StartRcloneProcess(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = RcloneExePath,
                Arguments = command,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = RcloneDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8
            };

            SetRcloneProxy(psi);

            if (AppState.DebugMode)
            {
                psi.CreateNoWindow = false;
            }

            var proc = Process.Start(psi);
            if (proc != null)
            {
                lock (_trackedRclonePids)
                {
                    _trackedRclonePids.Add(proc.Id);
                }
            }
            return proc;
        }

        private static void SetRcloneProxy(ProcessStartInfo psi)
        {
            string proxyUrl = DnsHelper.ProxyUrl;

            if (settings.useProxy)
            {
                var url = $"http://{settings.ProxyAddress}:{settings.ProxyPort}";
                psi.EnvironmentVariables["HTTP_PROXY"] = url;
                psi.EnvironmentVariables["HTTPS_PROXY"] = url;
                psi.EnvironmentVariables["http_proxy"] = url;
                psi.EnvironmentVariables["https_proxy"] = url;
            }
            else if (!string.IsNullOrEmpty(proxyUrl))
            {
                psi.EnvironmentVariables["HTTP_PROXY"] = proxyUrl;
                psi.EnvironmentVariables["HTTPS_PROXY"] = proxyUrl;
                psi.EnvironmentVariables["http_proxy"] = proxyUrl;
                psi.EnvironmentVariables["https_proxy"] = proxyUrl;
            }
            else
            {
                psi.EnvironmentVariables.Remove("HTTP_PROXY");
                psi.EnvironmentVariables.Remove("HTTPS_PROXY");
                psi.EnvironmentVariables.Remove("http_proxy");
                psi.EnvironmentVariables.Remove("https_proxy");
            }
        }

        public static ProcessOutput RunRcloneCommand_DownloadConfig(string command)
        {
            if (AppState.IsOffline)
            {
                return new ProcessOutput("", "No internet");
            }

            ProcessOutput prcoutput = new ProcessOutput();
            string originalCommand = command;

            if (downloadConfigPath.Length > 0)
            {
                command += $" --config {downloadConfigPath}";
            }

            command += " --inplace";

            if (AppState.IsManualMirrorSwitch)
            {
                command += " --contimeout 5s --timeout 10s --retries 0 --low-level-retries 1";
            }

            if (rclonepw.Length > 0)
            {
                command += " --ask-password=false";
            }

            string logcmd = Utilities.StringUtilities.RemoveEverythingBeforeFirst(command, PlatformHelpers.RcloneBinaryName);
            if (logcmd.Contains($"\"{settings.CurrentLogPath}\""))
            {
                logcmd = logcmd.Replace($"\"{settings.CurrentLogPath}\"", $"\"{settings.CurrentLogName}\"");
            }
            if (logcmd.Contains(AppContext.BaseDirectory))
            {
                logcmd = logcmd.Replace($"{AppContext.BaseDirectory}", $"BaseDirectory");
            }

            Logger.Log($"Running Rclone command: {logcmd}");

            using (var rclone = StartRcloneProcess(command))
            {
                if (rclone == null)
                    return new ProcessOutput("", "Failed to start rclone process");

                rclone.StandardInput.WriteLine(command);
                rclone.StandardInput.Close();

                string output = rclone.StandardOutput.ReadToEnd();
                string error = rclone.StandardError.ReadToEnd();
                rclone.WaitForExit();

                if (error.Contains("There is not enough space"))
                {
                    AppState.ShowMessageBox("NOT ENOUGH SPACE",
                        $"There isn't enough disk space to download this game.\r\nPlease ensure you have at least 200MB more than the game size available in {settings.DownloadDir} and try again.");
                    return new ProcessOutput("Download failed.", "");
                }

                if (error.Contains("400 Bad Request") || error.Contains("cannot fetch token") || error.Contains("authError") || error.Contains("quota") || error.Contains("exceeded") || error.Contains("directory not found") || error.Contains("Failed to"))
                {
                    if (AppState.IsManualMirrorSwitch)
                    {
                        prcoutput.Output = output;
                        prcoutput.Error = error;
                    }
                    else
                    {
                        string oldRemote = AppState.CurrentRemote;
                        bool retSM = false;
                        try
                        {
                            retSM = SwitchMirrors();
                        }
                        catch
                        {
                            return new ProcessOutput("All mirrors are on quota or down...", "All mirrors are on quota or down...");
                        }
                        if (retSM)
                        {
                            string newCommand = !string.IsNullOrEmpty(oldRemote)
                                ? originalCommand.Replace(oldRemote, AppState.CurrentRemote)
                                : originalCommand;
                            prcoutput = RunRcloneCommand_DownloadConfig(newCommand);
                        }
                    }
                }
                else
                {
                    prcoutput.Output = output;
                    prcoutput.Error = error;
                }

                if (!output.Contains("Game Name;Release Name;") && !output.Contains("package:") && !output.Contains(".meta"))
                {
                    if (!string.IsNullOrWhiteSpace(error))
                        Logger.Log($"Rclone error: {error}\n", LogLevel.ERROR);
                    if (!string.IsNullOrWhiteSpace(output))
                        Logger.Log($"Rclone Output: {output}");
                }

                return prcoutput;
            }
        }

        public static ProcessOutput RunRcloneCommand_UploadConfig(string command)
        {
            ProcessOutput prcoutput = new ProcessOutput();

            if (uploadConfigPath.Length > 0)
            {
                command += $" --config {uploadConfigPath}";
            }

            string logcmd = Utilities.StringUtilities.RemoveEverythingBeforeFirst(command, PlatformHelpers.RcloneBinaryName);
            if (logcmd.Contains($"\"{settings.CurrentLogPath}\""))
            {
                logcmd = logcmd.Replace($"\"{settings.CurrentLogPath}\"", $"\"{settings.CurrentLogName}\"");
            }
            if (logcmd.Contains(AppContext.BaseDirectory))
            {
                logcmd = logcmd.Replace($"{AppContext.BaseDirectory}", $"BaseDirectory");
            }

            Logger.Log($"Running Rclone command: {logcmd}");

            command += " --checkers 1 --retries 2 --inplace";

            using (var rclone = StartRcloneProcess(command))
            {
                if (rclone == null)
                    return new ProcessOutput("", "Failed to start rclone process");

                rclone.StandardInput.WriteLine(command);
                rclone.StandardInput.Close();

                string output = rclone.StandardOutput.ReadToEnd();
                string error = rclone.StandardError.ReadToEnd();
                rclone.WaitForExit();

                if (error.Contains("400 Bad Request") || error.Contains("cannot fetch token") || error.Contains("authError") || error.Contains("quota") || error.Contains("exceeded") || error.Contains("directory not found") || error.Contains("Failed to"))
                {
                    Logger.Log(error, LogLevel.ERROR);
                    return new ProcessOutput("Upload Failed.", "Upload failed.");
                }
                else
                {
                    prcoutput.Output = output;
                    prcoutput.Error = error;
                }

                if (!output.Contains("Game Name;Release Name;") && !output.Contains("package:") && !output.Contains(".meta"))
                {
                    if (!string.IsNullOrWhiteSpace(error))
                        Logger.Log($"Rclone error: {error}\n", LogLevel.ERROR);
                    if (!string.IsNullOrWhiteSpace(output))
                        Logger.Log($"Rclone Output: {output}");
                }

                return prcoutput;
            }
        }

        public static ProcessOutput RunRcloneCommand_PublicConfig(string command)
        {
            if (AppState.IsOffline)
            {
                return new ProcessOutput("", "No internet");
            }

            ProcessOutput prcoutput = new ProcessOutput();

            string logcmd = Utilities.StringUtilities.RemoveEverythingBeforeFirst(command, PlatformHelpers.RcloneBinaryName);
            if (logcmd.Contains($"\"{settings.CurrentLogPath}\""))
            {
                logcmd = logcmd.Replace($"\"{settings.CurrentLogPath}\"", $"\"{settings.CurrentLogName}\"");
            }
            if (logcmd.Contains(AppContext.BaseDirectory))
            {
                logcmd = logcmd.Replace($"{AppContext.BaseDirectory}", $"BaseDirectory");
            }

            command += " --inplace";

            Logger.Log($"Running Rclone command: {logcmd}");

            if (AppState.PublicConfigFile?.BaseUri != null)
            {
                command += $" --http-url {AppState.PublicConfigFile.BaseUri} {AppState.PublicMirrorExtraArgs}";
            }

            using (var rclone = StartRcloneProcess(command))
            {
                if (rclone == null)
                    return new ProcessOutput("", "Failed to start rclone process");

                rclone.StandardInput.WriteLine(command);
                rclone.StandardInput.Close();

                string output = rclone.StandardOutput.ReadToEnd();
                string error = rclone.StandardError.ReadToEnd();
                rclone.WaitForExit();

                if (error.Contains("There is not enough space"))
                {
                    AppState.ShowMessageBox("NOT ENOUGH SPACE",
                        $"There isn't enough disk space to download this game.\r\nPlease ensure you have at least 2x the game size available in {settings.DownloadDir} and try again.");
                    return new ProcessOutput("Download failed.", string.Empty);
                }

                if (error.Contains("Only one usage of each socket address (protocol/network address/port) is normally permitted"))
                {
                    Logger.Log(error, LogLevel.ERROR);
                    return new ProcessOutput("Failed to fetch from public mirror.", "Failed to fetch from public mirror.\nYou may have a running RCLONE Task!\nCheck your Task Manager and kill the process Rsync for Cloud Storage/Rclone");
                }
                else if (error.Contains("400 Bad Request")
                    || error.Contains("cannot fetch token")
                    || error.Contains("authError")
                    || error.Contains("quota")
                    || error.Contains("exceeded")
                    || error.Contains("directory not found")
                    || error.Contains("Failed to"))
                {
                    Logger.Log(error, LogLevel.ERROR);
                    return new ProcessOutput("Failed to fetch from public mirror.", "Failed to fetch from public mirror.");
                }
                else
                {
                    prcoutput.Output = output;
                    prcoutput.Error = error;
                }

                if (!output.Contains("Game Name;Release Name;") && !output.Contains("package:") && !output.Contains(".meta"))
                {
                    if (!string.IsNullOrWhiteSpace(error))
                        Logger.Log($"Rclone error: {error}\n", LogLevel.ERROR);
                    if (!string.IsNullOrWhiteSpace(output))
                        Logger.Log($"Rclone Output: {output}");
                }

                return prcoutput;
            }
        }

        private static bool SwitchMirrors()
        {
            // TODO: implement mirror switching logic
            // This was previously tied to MainForm UI; for now return false
            Logger.Log("Mirror switch requested but not yet implemented in cross-platform build", LogLevel.WARNING);
            return false;
        }
    }
}
