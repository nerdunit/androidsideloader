using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient.Receivers;
using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal class ADB
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;
        public static string adbFolderPath = Path.Combine(Environment.CurrentDirectory, "platform-tools");
        public static string adbFilePath = Path.Combine(adbFolderPath, "adb.exe");
        public static string DeviceID = "";
        public static string package = "";
        public static bool wirelessadbON;

        // AdbClient for direct protocol communication
        private static AdbClient _adbClient;
        private static DeviceData _currentDevice;

        // Gets or initializes the AdbClient instance
        private static AdbClient GetAdbClient()
        {
            if (_adbClient == null)
            {
                // Ensure ADB server is started
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

        // Gets the current device for AdbClient operations
        private static DeviceData GetCurrentDevice()
        {
            var client = GetAdbClient();
            var devices = client.GetDevices();

            if (devices == null || !devices.Any())
            {
                Logger.Log("No devices found via AdbClient", LogLevel.WARNING);
                return default;
            }

            // If DeviceID is set, find that specific device
            if (!string.IsNullOrEmpty(DeviceID) && DeviceID.Length > 1)
            {
                var device = devices.FirstOrDefault(d => d.Serial == DeviceID || d.Serial.StartsWith(DeviceID));
                if (device.Serial != null)
                {
                    _currentDevice = device;
                    return device;
                }
            }

            // Otherwise return the first available device
            _currentDevice = devices.First();
            return _currentDevice;
        }

        public static ProcessOutput RunAdbCommandToString(string command, bool suppressLogging = false)
        {
            if (!File.Exists(adbFilePath))
            {
                return new ProcessOutput("", "adb.exe not found");
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
                if (logcmd.Contains(Environment.CurrentDirectory))
                {
                    logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
                }
                _ = Logger.Log($"Running command: {logcmd}");
            }

            bool isConnectCommand = command.Contains("connect");
            int timeoutMs = isConnectCommand ? 5000 : -1; // 5 second timeout for connect commands

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
                        // For connect commands, we use async reading with timeout to avoid blocking on TCP timeout
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
                            // Process exited within timeout, safe to read output
                            output = outputTask.Result;
                            error = errorTask.Result;
                        }
                    }
                    else
                    {
                        // For non-connect commands, read output normally
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
                    _ = FlexibleMessageBox.Show(Program.form, "There is not enough room on your device to install this package. Please clear AT LEAST 2x the amount of the app you are trying to install.");
                }
                if (!suppressLogging && !output.Contains("version") && !output.Contains("KEYCODE_WAKEUP") && !output.Contains("Filesystem") && !output.Contains("package:") && !output.Equals(null))
                {
                    _ = Logger.Log(output);
                }

                _ = Logger.Log(error, LogLevel.ERROR);
                return new ProcessOutput(output, error);
            }
        }

        // Executes a shell command on the device.
        private static void ExecuteShellCommand(AdbClient client, DeviceData device, string command)
        {
            var receiver = new ConsoleOutputReceiver();
            client.ExecuteRemoteCommand(command, device, receiver);
        }

        // Copies and installs an APK with real-time progress reporting using AdvancedSharpAdbClient
        public static async Task<ProcessOutput> SideloadWithProgressAsync(
            string path,
            Action<float, TimeSpan?> progressCallback = null,
            Action<string> statusCallback = null,
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

                statusCallback?.Invoke("Installing APK...");

                // Throttle UI updates to prevent lag
                DateTime lastProgressUpdate = DateTime.MinValue;
                float lastReportedPercent = -1;
                const int ThrottleMs = 100; // Update UI every 100ms

                // Shared ETA engine (percent-units)
                var eta = new EtaEstimator(alpha: 0.05, reanchorThreshold: 0.20);

                // Create install progress handler
                Action<InstallProgressEventArgs> installProgress = (args) =>
                {
                    float percent = 0;
                    string status = null;
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

                            // Update ETA engine using percent as units (0..100)
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

                        // ETA goes back via progress callback (label); status remains percent-only string for inner bar
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

                // Signature mismatches and version downgrades can be fixed by reinstalling
                bool isReinstallEligible = ex.Message.Contains("signatures do not match") ||
                                           ex.Message.Contains("INSTALL_FAILED_VERSION_DOWNGRADE") ||
                                           ex.Message.Contains("failed to install");

                // For insufficient storage, offer reinstall if it's an upgrade
                // As uninstalling old version frees space for the new one
                bool isStorageIssue = ex.Message.Contains("INSUFFICIENT_STORAGE");
                bool isUpgrade = !string.IsNullOrEmpty(packagename) &&
                                 settings.InstalledApps.Contains(packagename);

                if (isStorageIssue && isUpgrade)
                {
                    isReinstallEligible = true;
                }

                if (isReinstallEligible)
                {
                    bool cancelClicked = false;

                    if (!settings.AutoReinstall)
                    {
                        string message = isStorageIssue
                            ? "Installation failed due to insufficient storage. Since this is an upgrade, Rookie can uninstall the old version first to free up space, then install the new version.\n\nRookie will also attempt to backup your save data and reinstall the game automatically, however some games do not store their saves in an accessible location (less than 5%). Continue with reinstall?"
                            : "In place upgrade has failed. Rookie will attempt to backup your save data and reinstall the game automatically, however some games do not store their saves in an accessible location (less than 5%). Continue with reinstall?";

                        string title = isStorageIssue ? "Insufficient Storage" : "In place upgrade failed";

                        Program.form.Invoke(() =>
                        {
                            DialogResult dialogResult1 = FlexibleMessageBox.Show(Program.form,
                                message, title, MessageBoxButtons.OKCancel);
                            if (dialogResult1 == DialogResult.Cancel)
                                cancelClicked = true;
                        });
                    }

                    if (cancelClicked)
                        return new ProcessOutput("", "Installation cancelled by user");

                    statusCallback?.Invoke("Performing reinstall...");

                    try
                    {
                        await Task.Run(() =>
                        {
                            var device = GetCurrentDevice();
                            var client = GetAdbClient();
                            var packageManager = new PackageManager(client, device);

                            statusCallback?.Invoke("Backing up save data...");
                            _ = RunAdbCommandToString($"pull \"/sdcard/Android/data/{packagename}\" \"{Environment.CurrentDirectory}\"");

                            statusCallback?.Invoke("Uninstalling old version...");
                            packageManager.UninstallPackage(packagename);

                            statusCallback?.Invoke("Reinstalling game...");
                            Action<InstallProgressEventArgs> reinstallProgress = (args) =>
                            {
                                if (args.State == PackageInstallProgressState.Uploading)
                                {
                                    progressCallback?.Invoke((float)args.UploadProgress, null);
                                }
                            };
                            packageManager.InstallPackage(path, reinstallProgress);

                            statusCallback?.Invoke("Restoring save data...");
                            _ = RunAdbCommandToString($"push \"{Environment.CurrentDirectory}\\{packagename}\" /sdcard/Android/data/");

                            string directoryToDelete = Path.Combine(Environment.CurrentDirectory, packagename);
                            if (Directory.Exists(directoryToDelete) && directoryToDelete != Environment.CurrentDirectory)
                            {
                                FileSystemUtilities.TryDeleteDirectory(directoryToDelete);
                            }
                        });

                        progressCallback?.Invoke(100, null);
                        return new ProcessOutput($"{gameName}: Reinstall: Success\n", "");
                    }
                    catch (Exception reinstallEx)
                    {
                        return new ProcessOutput("", $"{gameName}: Reinstall Failed: {reinstallEx.Message}\n");
                    }
                }

                // Return the error message so it's displayed to the user
                return new ProcessOutput("", $"\n{gameName}: {ex.Message}");
            }
        }

        // Copies OBB folder with real-time progress reporting using AdvancedSharpAdbClient
        public static async Task<ProcessOutput> CopyOBBWithProgressAsync(
            string localPath,
            Action<float, TimeSpan?> progressCallback = null,
            Action<string> statusCallback = null,
            string gameName = "")
        {
            string folderName = Path.GetFileName(localPath);

            if (!folderName.Contains("."))
            {
                return new ProcessOutput("No OBB Folder found");
            }

            try
            {
                var device = GetCurrentDevice();
                if (device.Serial == null)
                {
                    return new ProcessOutput("", "No device connected");
                }

                var client = GetAdbClient();
                string remotePath = $"/sdcard/Android/obb/{folderName}";

                statusCallback?.Invoke($"Preparing: {folderName}");
                progressCallback?.Invoke(0, null);

                // Delete existing OBB folder and create new one
                ExecuteShellCommand(client, device, $"rm -rf \"{remotePath}\"");
                ExecuteShellCommand(client, device, $"mkdir -p \"{remotePath}\"");

                // Get all files to push and calculate total size
                var files = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);
                long totalBytes = files.Sum(f => new FileInfo(f).Length);
                long transferredBytes = 0;

                // Throttle UI updates to prevent lag
                DateTime lastProgressUpdate = DateTime.MinValue;
                float lastReportedPercent = -1;
                const int ThrottleMs = 100; // Update UI every 100ms

                // Shared ETA engine (bytes-units)
                var eta = new EtaEstimator(alpha: 0.10, reanchorThreshold: 0.20);

                statusCallback?.Invoke($"Copying: {folderName}");

                using (var syncService = new SyncService(client, device))
                {
                    foreach (var file in files)
                    {
                        string relativePath = file.Substring(localPath.Length)
                                                  .TrimStart('\\', '/')
                                                  .Replace('\\', '/');
                        string remoteFilePath = $"{remotePath}/{relativePath}";
                        string fileName = Path.GetFileName(file);

                        // Ensure remote directory exists
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

                            // Update ETA engine in bytes
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
                                statusCallback?.Invoke(fileName);
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
            if (logcmd.Contains(Environment.CurrentDirectory))
            {
                logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
            }

            _ = Logger.Log($"Running command: {logcmd}");

            using (var adb = new Process())
            {
                adb.StartInfo.FileName = "cmd.exe";
                adb.StartInfo.RedirectStandardError = true;
                adb.StartInfo.RedirectStandardInput = true;
                adb.StartInfo.RedirectStandardOutput = true;
                adb.StartInfo.CreateNoWindow = true;
                adb.StartInfo.UseShellExecute = false;
                adb.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
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
            if (logcmd.Contains(Environment.CurrentDirectory))
            {
                logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
            }

            Logger.Log($"Running command: {logcmd}");

            try
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = $@"{Path.GetPathRoot(Environment.SystemDirectory)}\Windows\System32\cmd.exe";
                    proc.StartInfo.Arguments = command;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);

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
            Program.form.Invoke(() =>
            {
                DialogResult dialogResult = FlexibleMessageBox.Show(Program.form,
                    "If this is your first time, accept the prompt on your headset (\"Always Allow\").\r\n\r\nIf you don't see a prompt:\r\n  • Make sure developer mode is enabled\r\n  • Make sure your USB cable supports data\r\n  • Try a different USB port or cable\r\n  • Restart your headset\r\n  • If nothing else helped, click RESET ADB AUTH in the left panel under MISCELLANEOUS",
                    "ADB Debugging not enabled.", MessageBoxButtons.OKCancel);
                settings.AdbDebugWarned = true;
                settings.Save();
            });
        }

        public static string ResetAdbAuthorization()
        {
            try
            {
                // Kill the running ADB server
                RunAdbCommandToString("kill-server");
                _adbClient = null;
                _currentDevice = default;

                // Remove stale ADB vendor keys so a fresh RSA pair is generated on next start
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

                // Restart the ADB server so new keys are generated
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

        public static ProcessOutput UninstallPackage(string package)
        {
            ProcessOutput output = new ProcessOutput("", "");
            output += RunAdbCommandToString($"shell pm uninstall {package}");

            // Prefix the output with the simple game name
            string label = Sideloader.gameNameToSimpleName(Sideloader.PackageNametoGameName(package));

            if (!string.IsNullOrEmpty(output.Output))
            {
                output.Output = $"{label}: {output.Output}";
            }

            if (!string.IsNullOrEmpty(output.Error))
            {
                output.Error = $"{label}: {output.Error}";
            }

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
        private readonly double _alpha;                  // EWMA smoothing
        private readonly double _reanchorThreshold;      // % difference required to re-anchor
        private readonly double _minSampleSeconds;       // ignore too-short dt

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

        // Updates internal rate estimate and re-anchors ETA
        // totalUnits: total work units (e.g., 100 for percent, or totalBytes for bytes)
        // doneUnits:  completed work units so far (e.g., percent, or bytes transferred)
        public void Update(long totalUnits, long doneUnits)
        {
            var now = DateTime.UtcNow;
            if (totalUnits <= 0) return;

            doneUnits = Math.Max(0, Math.Min(totalUnits, doneUnits));

            long remainingUnits = Math.Max(0, totalUnits - doneUnits);

            double dt = (now - _lastSampleTimeUtc).TotalSeconds;
            long dUnits = doneUnits - _lastSampleDoneUnits;

            if (dt >= _minSampleSeconds && dUnits > 0)
            {
                double instUnitsPerSecond = dUnits / dt;

                if (_smoothedUnitsPerSecond <= 0)
                    _smoothedUnitsPerSecond = instUnitsPerSecond;
                else
                    _smoothedUnitsPerSecond = _alpha * instUnitsPerSecond + (1 - _alpha) * _smoothedUnitsPerSecond;

                _lastSampleTimeUtc = now;
                _lastSampleDoneUnits = doneUnits;
            }

            if (_smoothedUnitsPerSecond > 1e-6 && remainingUnits > 0)
            {
                var newEta = TimeSpan.FromSeconds(remainingUnits / _smoothedUnitsPerSecond);
                if (newEta < TimeSpan.Zero) newEta = TimeSpan.Zero;

                if (!_etaAnchorValue.HasValue)
                {
                    _etaAnchorValue = newEta;
                    _etaAnchorTimeUtc = now;
                }
                else
                {
                    // What countdown would currently show
                    var predictedNow = _etaAnchorValue.Value - (now - _etaAnchorTimeUtc);
                    if (predictedNow < TimeSpan.Zero) predictedNow = TimeSpan.Zero;

                    double baseSeconds = Math.Max(1, predictedNow.TotalSeconds);
                    double diffRatio = Math.Abs(newEta.TotalSeconds - predictedNow.TotalSeconds) / baseSeconds;

                    if (diffRatio > _reanchorThreshold)
                    {
                        _etaAnchorValue = newEta;
                        _etaAnchorTimeUtc = now;
                    }
                }
            }
        }

        // Returns a countdown ETA for UI display
        public TimeSpan? GetDisplayEta()
        {
            if (!_etaAnchorValue.HasValue) return null;

            var remaining = _etaAnchorValue.Value - (DateTime.UtcNow - _etaAnchorTimeUtc);
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

            return TimeSpan.FromSeconds(Math.Ceiling(remaining.TotalSeconds));
        }
    }
}