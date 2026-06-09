using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient.Receivers;
using AndroidSideloader.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    public class ADB
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;
        public static string adbFolderPath = Path.Combine(AppContext.BaseDirectory, "platform-tools");
        public static string adbFilePath = Path.Combine(adbFolderPath, PlatformHelpers.AdbBinaryName);
        public static string DeviceID = "";
        public static string package = "";
        public static bool wirelessadbON;

        private static AdbClient? _adbClient;
        private static DeviceData _currentDevice;

        private static AdbClient GetAdbClient()
        {
            if (_adbClient == null)
            {
                if (!AdbServer.Instance.GetStatus().IsRunning)
                {
                    var server = new AdbServer();
                    var result = server.StartServer(adbFilePath, false);
                    Logger.Log($"ADB server start result: {result}");
                }
                _adbClient = new AdbClient();
            }
            return _adbClient;
        }

        private static DeviceData GetCurrentDevice()
        {
            var client = GetAdbClient();
            var devices = client.GetDevices();

            if (devices == null || !devices.Any())
            {
                Logger.Log("No devices found via AdbClient", LogLevel.WARNING);
                return default;
            }

            if (!string.IsNullOrEmpty(DeviceID) && DeviceID.Length > 1)
            {
                var device = devices.FirstOrDefault(d => d.Serial == DeviceID || d.Serial.StartsWith(DeviceID));
                if (device.Serial != null)
                {
                    _currentDevice = device;
                    return device;
                }
            }

            _currentDevice = devices.First();
            return _currentDevice;
        }

        public static ProcessOutput RunAdbCommandToString(string command, bool suppressLogging = false)
        {
            if (!File.Exists(adbFilePath))
            {
                return new ProcessOutput("", $"{PlatformHelpers.AdbBinaryName} not found");
            }

            command = command.Replace("adb", "");

            settings.ADBFolder = adbFolderPath;
            settings.ADBPath = adbFilePath;
            settings.Save();

            if (DeviceID.Length > 1)
            {
                command = $" -s {DeviceID} {command}";
            }

            if (!suppressLogging && !command.Contains("dumpsys") && !command.Contains("shell pm list packages") && !command.Contains("KEYCODE_WAKEUP"))
            {
                string logcmd = command;
                if (logcmd.Contains(AppContext.BaseDirectory))
                {
                    logcmd = logcmd.Replace($"{AppContext.BaseDirectory}", $"BaseDirectory");
                }
                _ = Logger.Log($"Running command: {logcmd}");
            }

            bool isConnectCommand = command.Contains("connect");
            int timeoutMs = isConnectCommand ? 5000 : -1;

            using (Process adb = new Process())
            {
                adb.StartInfo.FileName = adbFilePath;
                adb.StartInfo.Arguments = command;
                adb.StartInfo.RedirectStandardError = true;
                adb.StartInfo.RedirectStandardOutput = true;
                adb.StartInfo.CreateNoWindow = true;
                adb.StartInfo.UseShellExecute = false;
                adb.StartInfo.WorkingDirectory = adbFolderPath;
                _ = adb.Start();

                string output = "";
                string error = "";

                try
                {
                    if (isConnectCommand)
                    {
                        var outputTask = adb.StandardOutput.ReadToEndAsync();
                        var errorTask = adb.StandardError.ReadToEndAsync();
                        bool exited = adb.WaitForExit(timeoutMs);

                        if (!exited)
                        {
                            try { adb.Kill(); } catch { }
                            adb.WaitForExit(1000);
                            output = "Connection timed out";
                            error = "cannot connect: Connection timed out";
                            Logger.Log($"ADB connect command timed out after {timeoutMs}ms", LogLevel.WARNING);
                        }
                        else
                        {
                            output = outputTask.Result;
                            error = errorTask.Result;
                        }
                    }
                    else
                    {
                        output = adb.StandardOutput.ReadToEnd();
                        error = adb.StandardError.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error reading ADB output: {ex.Message}", LogLevel.WARNING);
                }

                if (error.Contains("ADB_VENDOR_KEYS") && !settings.AdbDebugWarned)
                {
                    ADBDebugWarning();
                }
                if (error.Contains("not enough storage space"))
                {
                    AppState.ShowMessageBox("Storage Error", "There is not enough room on your device to install this package. Please clear AT LEAST 2x the amount of the app you are trying to install.");
                }

                if (!suppressLogging && !output.Contains("version") && !output.Contains("KEYCODE_WAKEUP") && !output.Contains("Filesystem") && !output.Contains("package:"))
                {
                    _ = Logger.Log(output);
                }

                _ = Logger.Log(error, LogLevel.ERROR);
                return new ProcessOutput(output, error);
            }
        }

        private static void ExecuteShellCommand(AdbClient client, DeviceData device, string command)
        {
            var receiver = new ConsoleOutputReceiver();
            client.ExecuteRemoteCommand(command, device, receiver);
        }

        public static async Task<ProcessOutput> SideloadWithProgressAsync(
            string path,
            Action<float, TimeSpan?>? progressCallback = null,
            Action<string>? statusCallback = null,
            string packagename = "",
            string gameName = "")
        {
            statusCallback?.Invoke("Installing APK...");
            progressCallback?.Invoke(0, null);

            try
            {
                var device = GetCurrentDevice();
                if (device.Serial == null)
                {
                    return new ProcessOutput("", "No device connected");
                }

                var client = GetAdbClient();
                var packageManager = new PackageManager(client, device);

                DateTime lastProgressUpdate = DateTime.MinValue;
                float lastReportedPercent = -1;
                const int ThrottleMs = 100;
                var eta = new EtaEstimator(alpha: 0.05, reanchorThreshold: 0.20);

                Action<InstallProgressEventArgs> installProgress = (args) =>
                {
                    float percent = 0;
                    string? status = null;
                    TimeSpan? displayEta = null;

                    switch (args.State)
                    {
                        case PackageInstallProgressState.Preparing:
                            percent = 0;
                            status = "Preparing...";
                            eta.Reset();
                            break;

                        case PackageInstallProgressState.Uploading:
                            percent = (float)args.UploadProgress;
                            if (percent > 0 && percent < 100)
                            {
                                eta.Update(totalUnits: 100, doneUnits: (long)Math.Round(percent));
                                displayEta = eta.GetDisplayEta();
                            }
                            else
                            {
                                displayEta = eta.GetDisplayEta();
                            }
                            status = $"Installing · {percent:0.0}%";
                            break;

                        case PackageInstallProgressState.Installing:
                            percent = 100;
                            status = "Completing Installation...";
                            displayEta = null;
                            break;

                        case PackageInstallProgressState.Finished:
                            percent = 100;
                            status = "";
                            displayEta = null;
                            break;

                        default:
                            percent = 100;
                            status = "";
                            displayEta = null;
                            break;
                    }

                    var updateNow = DateTime.UtcNow;
                    bool shouldUpdate = (updateNow - lastProgressUpdate).TotalMilliseconds >= ThrottleMs
                                        || Math.Abs(percent - lastReportedPercent) >= 0.1f
                                        || args.State != PackageInstallProgressState.Uploading;

                    if (shouldUpdate)
                    {
                        lastProgressUpdate = updateNow;
                        lastReportedPercent = percent;
                        progressCallback?.Invoke(percent, displayEta);
                        if (status != null) statusCallback?.Invoke(status);
                    }
                };

                await Task.Run(() =>
                {
                    packageManager.InstallPackage(path, installProgress);
                });

                progressCallback?.Invoke(100, null);
                statusCallback?.Invoke("");

                return new ProcessOutput($"{gameName}: Success\n");
            }
            catch (Exception ex)
            {
                Logger.Log($"SideloadWithProgressAsync error: {ex.Message}", LogLevel.ERROR);
                bool isReinstallEligible = ex.Message.Contains("signatures do not match") ||
                                           ex.Message.Contains("INSTALL_FAILED_VERSION_DOWNGRADE") ||
                                           ex.Message.Contains("failed to install");
                bool isStorageIssue = ex.Message.Contains("INSUFFICIENT_STORAGE");
                bool isUpgrade = !string.IsNullOrEmpty(packagename) &&
                                 settings.InstalledApps.Contains(packagename);

                if (isStorageIssue && isUpgrade)
                {
                    isReinstallEligible = true;
                }

                if (isReinstallEligible)
                {
                    if (!settings.AutoReinstall)
                    {
                        // UI layer handles this via events; for now return error
                        return new ProcessOutput("", $"{gameName}: Install failed (signature/version/storage). Enable AutoReinstall or retry.\n{ex.Message}");
                    }
                    else
                    {
                        return await AutoReinstallPackage(client: GetAdbClient(), packagename, path, gameName, progressCallback, statusCallback);
                    }
                }

                return new ProcessOutput("", $"{gameName}: Failed to install: {ex.Message}\n");
            }
        }

        private static async Task<ProcessOutput> AutoReinstallPackage(AdbClient client, string packageName, string path, string gameName,
            Action<float, TimeSpan?>? progressCallback = null,
            Action<string>? statusCallback = null)
        {
            try
            {
                statusCallback?.Invoke("Backing up saves...");
                var device = GetCurrentDevice();
                if (device.Serial == null)
                    return new ProcessOutput("", "No device connected during reinstall");

                // Backup saves
                string backupPath = $"/sdcard/Android/data/{packageName}";
                string tempBackup = $"/sdcard/{packageName}_backup_{DateTime.Now:yyyyMMddHHmmss}";

                try
                {
                    var receiver = new ConsoleOutputReceiver();
                    client.ExecuteRemoteCommand($"cp -r \"{backupPath}\" \"{tempBackup}\"", device, receiver);
                }
                catch { /* backup may not exist */ }

                statusCallback?.Invoke("Uninstalling old version...");
                var pm = new PackageManager(client, device);
                await Task.Run(() => pm.UninstallPackage(packageName));

                statusCallback?.Invoke("Reinstalling...");
                var result = await SideloadWithProgressAsync(path, progressCallback, statusCallback, packageName, gameName);

                // Restore saves
                try
                {
                    var receiver = new ConsoleOutputReceiver();
                    client.ExecuteRemoteCommand($"mv \"{tempBackup}\" \"{backupPath}\"", device, receiver);
                }
                catch { /* restore may fail */ }

                return result;
            }
            catch (Exception ex)
            {
                return new ProcessOutput("", $"AutoReinstall failed: {ex.Message}");
            }
        }

        public static async Task<ProcessOutput> CopyOBBWithProgressAsync(
            string gameName,
            string obbPath,
            string packageName,
            Action<float, TimeSpan?>? progressCallback = null,
            Action<string>? statusCallback = null)
        {
            statusCallback?.Invoke("Copying OBB files...");
            progressCallback?.Invoke(0, null);

            try
            {
                var device = GetCurrentDevice();
                if (device.Serial == null)
                {
                    return new ProcessOutput("", "No device connected");
                }

                var client = GetAdbClient();
                string remoteBasePath = $"/sdcard/Android/obb/{packageName}";

                var receiver = new ConsoleOutputReceiver();
                client.ExecuteRemoteCommand($"mkdir -p \"{remoteBasePath}\"", device, receiver);

                string[] files = Directory.GetFiles(obbPath, "*", SearchOption.AllDirectories);
                long totalBytes = files.Sum(f => new FileInfo(f).Length);
                long transferredBytes = 0;

                DateTime lastProgressUpdate = DateTime.MinValue;
                float lastReportedPercent = -1;
                const int ThrottleMs = 100;
                var eta = new EtaEstimator(alpha: 0.05, reanchorThreshold: 0.20);

                using (var syncService = new SyncService(client, device))
                {
                    foreach (string file in files)
                    {
                        string relativePath = Path.GetRelativePath(obbPath, file);
                        string remoteFilePath = remoteBasePath.Replace("\\", "/") + "/" + relativePath.Replace("\\", "/");
                        string remoteDir = remoteFilePath.Substring(0, remoteFilePath.LastIndexOf('/'));
                        ExecuteShellCommand(client, device, $"mkdir -p \"{remoteDir}\"");

                        var fileInfo = new FileInfo(file);
                        long fileSize = fileInfo.Length;
                        long capturedTransferredBytes = transferredBytes;

                        Action<SyncProgressChangedEventArgs> progressHandler = (args) =>
                        {
                            long totalProgressBytes = capturedTransferredBytes + args.ReceivedBytesSize;
                            float overallPercent = totalBytes > 0
                                ? (float)(totalProgressBytes * 100.0 / totalBytes)
                                : 0f;
                            overallPercent = Math.Max(0, Math.Min(100, overallPercent));

                            if (totalBytes > 0 && totalProgressBytes > 0 && overallPercent < 100)
                            {
                                eta.Update(totalUnits: totalBytes, doneUnits: totalProgressBytes);
                            }

                            TimeSpan? displayEta = eta.GetDisplayEta();
                            var now2 = DateTime.UtcNow;
                            bool shouldUpdate = (now2 - lastProgressUpdate).TotalMilliseconds >= ThrottleMs
                                                || Math.Abs(overallPercent - lastReportedPercent) >= 0.1f;

                            if (shouldUpdate)
                            {
                                lastProgressUpdate = now2;
                                lastReportedPercent = overallPercent;
                                progressCallback?.Invoke(overallPercent, displayEta);
                                statusCallback?.Invoke(fileInfo.Name);
                            }
                        };

                        using (var stream = File.OpenRead(file))
                        {
                            await Task.Run(() =>
                            {
                                syncService.Push(
                                    stream,
                                    remoteFilePath,
                                    UnixFileStatus.DefaultFileMode,
                                    DateTime.Now,
                                    progressHandler,
                                    false);
                            });
                        }

                        transferredBytes += fileSize;
                    }
                }

                progressCallback?.Invoke(100, null);
                statusCallback?.Invoke("");

                return new ProcessOutput($"{gameName}: OBB transfer: Success\n", "");
            }
            catch (Exception ex)
            {
                Logger.Log($"CopyOBBWithProgressAsync error: {ex.Message}", LogLevel.ERROR);
                return new ProcessOutput("", $"{gameName}: OBB transfer: Failed: {ex.Message}\n");
            }
        }

        public static ProcessOutput RunAdbCommandToStringWOADB(string result, string path)
        {
            string command = result;
            string logcmd = command;
            if (logcmd.Contains(AppContext.BaseDirectory))
            {
                logcmd = logcmd.Replace($"{AppContext.BaseDirectory}", $"BaseDirectory");
            }

            _ = Logger.Log($"Running command: {logcmd}");

            using (var adb = new Process())
            {
                adb.StartInfo.FileName = PlatformHelpers.ShellExecutable;
                adb.StartInfo.Arguments = PlatformHelpers.ShellArgumentsPrefix + command;
                adb.StartInfo.RedirectStandardError = true;
                adb.StartInfo.RedirectStandardInput = true;
                adb.StartInfo.RedirectStandardOutput = true;
                adb.StartInfo.CreateNoWindow = true;
                adb.StartInfo.UseShellExecute = false;
                adb.StartInfo.WorkingDirectory = Path.GetDirectoryName(path) ?? AppContext.BaseDirectory;
                _ = adb.Start();
                adb.StandardInput.WriteLine(command);
                adb.StandardInput.Flush();
                adb.StandardInput.Close();

                string output = "";
                string error = "";

                try
                {
                    output += adb.StandardOutput.ReadToEnd();
                    error += adb.StandardError.ReadToEnd();
                }
                catch { }

                if (command.Contains("connect"))
                {
                    bool graceful = adb.WaitForExit(3000);
                    if (!graceful)
                    {
                        adb.Kill();
                        adb.WaitForExit();
                    }
                }

                if (error.Contains("ADB_VENDOR_KEYS") && !settings.AdbDebugWarned)
                {
                    ADBDebugWarning();
                }

                _ = Logger.Log(output);
                _ = Logger.Log(error, LogLevel.ERROR);
                return new ProcessOutput(output, error);
            }
        }

        public static ProcessOutput RunCommandToString(string result, string path = "")
        {
            string command = result;
            string logcmd = command;
            if (logcmd.Contains(AppContext.BaseDirectory))
            {
                logcmd = logcmd.Replace($"{AppContext.BaseDirectory}", $"BaseDirectory");
            }

            Logger.Log($"Running command: {logcmd}");

            try
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = PlatformHelpers.ShellExecutable;
                    proc.StartInfo.Arguments = PlatformHelpers.ShellArgumentsPrefix + command;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.WorkingDirectory = !string.IsNullOrEmpty(path) ? path : AppContext.BaseDirectory;

                    proc.Start();
                    proc.StandardInput.WriteLine(command);
                    proc.StandardInput.Flush();
                    proc.StandardInput.Close();

                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();

                    if (command.Contains("connect"))
                    {
                        bool graceful = proc.WaitForExit(3000);
                        if (!graceful)
                        {
                            proc.Kill();
                            proc.WaitForExit();
                        }
                    }
                    else
                    {
                        proc.WaitForExit();
                    }

                    if (error.Contains("ADB_VENDOR_KEYS") && !settings.AdbDebugWarned)
                    {
                        ADBDebugWarning();
                    }

                    Logger.Log(output);
                    Logger.Log(error, LogLevel.ERROR);

                    return new ProcessOutput(output, error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in RunCommandToString: {ex.Message}", LogLevel.ERROR);
                return new ProcessOutput("", $"Exception occurred: {ex.Message}");
            }
        }

        public static void ADBDebugWarning()
        {
            settings.AdbDebugWarned = true;
            settings.Save();
            AppState.ShowMessageBox("ADB Debugging",
                "If this is your first time, accept the prompt on your headset (\"Always Allow\").\r\n\r\nIf you don't see a prompt:\r\n  • Make sure developer mode is enabled\r\n  • Make sure your USB cable supports data\r\n  • Try a different USB port or cable\r\n  • Restart your headset\r\n  • If nothing else helped, click RESET ADB AUTH in the left panel under MISCELLANEOUS");
        }

        public static string ResetAdbAuthorization()
        {
            try
            {
                RunAdbCommandToString("kill-server");
                _adbClient = null;
                _currentDevice = default;

                string androidDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".android");
                string[] keyFiles = new[] { "adbkey", "adbkey.pub" };
                foreach (string keyFile in keyFiles)
                {
                    string path = Path.Combine(androidDir, keyFile);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        Logger.Log($"Deleted ADB key file: {path}");
                    }
                }

                settings.AdbDebugWarned = false;
                settings.Save();

                var server = new AdbServer();
                server.StartServer(adbFilePath, true);
                Logger.Log("ADB authorization reset complete. New keys generated.");

                return "ADB authorization reset successfully. Please re-authorize on your device when prompted.";
            }
            catch (Exception ex)
            {
                Logger.Log($"Error resetting ADB authorization: {ex.Message}", LogLevel.ERROR);
                return $"Error resetting ADB authorization: {ex.Message}";
            }
        }

        public static ProcessOutput UninstallPackage(string pkg)
        {
            ProcessOutput output = new ProcessOutput("", "");
            output += RunAdbCommandToString($"shell pm uninstall {pkg}");
            return output;
        }

        public static string GetAvailableSpace()
        {
            long totalSize = 0;
            long usedSize = 0;
            long freeSize = 0;

            string[] output = RunAdbCommandToString("shell df").Output.Split('\n');

            foreach (string currLine in output)
            {
                if (currLine.StartsWith("/dev/fuse") || currLine.StartsWith("/data/media"))
                {
                    string[] foo = currLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (foo.Length >= 4)
                    {
                        totalSize = long.Parse(foo[1]) / 1000;
                        usedSize = long.Parse(foo[2]) / 1000;
                        freeSize = long.Parse(foo[3]) / 1000;
                        break;
                    }
                }
            }

            return $"Total space: {string.Format("{0:0.00}", (double)totalSize / 1000)}GB\nUsed space: {string.Format("{0:0.00}", (double)usedSize / 1000)}GB\nFree space: {string.Format("{0:0.00}", (double)freeSize / 1000)}GB";
        }
    }

    internal class EtaEstimator
    {
        private readonly double _alpha;
        private readonly double _reanchorThreshold;
        private readonly double _minSampleSeconds;

        private DateTime _lastSampleTimeUtc;
        private long _lastSampleDoneUnits;
        private double _smoothedUnitsPerSecond;

        private TimeSpan? _etaAnchorValue;
        private DateTime _etaAnchorTimeUtc;

        public EtaEstimator(double alpha, double reanchorThreshold, double minSampleSeconds = 0.15)
        {
            _alpha = alpha;
            _reanchorThreshold = reanchorThreshold;
            _minSampleSeconds = minSampleSeconds;
            Reset();
        }

        public void Reset()
        {
            _lastSampleTimeUtc = DateTime.UtcNow;
            _lastSampleDoneUnits = 0;
            _smoothedUnitsPerSecond = 0;
            _etaAnchorValue = null;
            _etaAnchorTimeUtc = DateTime.UtcNow;
        }

        public void Update(long totalUnits, long doneUnits)
        {
            if (doneUnits <= 0 || doneUnits >= totalUnits)
                return;

            var now = DateTime.UtcNow;
            double dt = (now - _lastSampleTimeUtc).TotalSeconds;

            if (dt < _minSampleSeconds)
                return;

            long deltaUnits = doneUnits - _lastSampleDoneUnits;
            if (deltaUnits <= 0)
                return;

            double instantRate = deltaUnits / dt;
            if (_smoothedUnitsPerSecond <= 0)
                _smoothedUnitsPerSecond = instantRate;
            else
                _smoothedUnitsPerSecond = (_alpha * instantRate) + ((1.0 - _alpha) * _smoothedUnitsPerSecond);

            _lastSampleTimeUtc = now;
            _lastSampleDoneUnits = doneUnits;

            long remainingUnits = totalUnits - doneUnits;
            if (_smoothedUnitsPerSecond > 0)
            {
                double secondsRemaining = remainingUnits / _smoothedUnitsPerSecond;
                var newEta = TimeSpan.FromSeconds(secondsRemaining);

                if (_etaAnchorValue == null)
                {
                    _etaAnchorValue = newEta;
                    _etaAnchorTimeUtc = now;
                }
                else
                {
                    double elapsedSinceAnchor = (now - _etaAnchorTimeUtc).TotalSeconds;
                    double predictedCurrent = Math.Max(0, _etaAnchorValue.Value.TotalSeconds - elapsedSinceAnchor);
                    double diff = Math.Abs(predictedCurrent - newEta.TotalSeconds);
                    double relativeDiff = predictedCurrent > 0 ? diff / predictedCurrent : 0;

                    if (relativeDiff > _reanchorThreshold)
                    {
                        _etaAnchorValue = newEta;
                        _etaAnchorTimeUtc = now;
                    }
                }
            }
        }

        public TimeSpan? GetDisplayEta()
        {
            if (_etaAnchorValue == null)
                return null;

            double elapsed = (DateTime.UtcNow - _etaAnchorTimeUtc).TotalSeconds;
            double remaining = _etaAnchorValue.Value.TotalSeconds - elapsed;

            if (remaining <= 0)
                return TimeSpan.Zero;

            return TimeSpan.FromSeconds(remaining);
        }
    }
}
