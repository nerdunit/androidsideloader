using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal class ADB
    {
        private static readonly Process adb = new Process();
        public static string adbFolderPath = "C:\\RSL\\platform-tools";
        public static string adbFilePath = adbFolderPath + "\\adb.exe";
        public static string DeviceID = "";
        public static string package = "";
        public static ProcessOutput RunAdbCommandToString(string command)
        {
            Properties.Settings.Default.ADBFolder = adbFolderPath;
            Properties.Settings.Default.ADBPath = adbFilePath;
            Properties.Settings.Default.Save();
            if (DeviceID.Length > 1)
            {
                command = $" -s {DeviceID} {command}";
            }
            if (!command.Contains("dumpsys") && !command.Contains("shell pm list packages") && !command.Contains("KEYCODE_WAKEUP"))
            {

                string logcmd = command;

                if (logcmd.Contains(Environment.CurrentDirectory))
                {
                    logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
                }

                _ = Logger.Log($"Running command: {logcmd}");
            }
            adb.StartInfo.FileName = adbFilePath;
            adb.StartInfo.Arguments = command;
            adb.StartInfo.RedirectStandardError = true;
            adb.StartInfo.RedirectStandardInput = true;
            adb.StartInfo.RedirectStandardOutput = true;
            adb.StartInfo.CreateNoWindow = true;
            adb.StartInfo.UseShellExecute = false;
            adb.StartInfo.WorkingDirectory = adbFolderPath;
            _ = adb.Start();
            adb.StandardInput.WriteLine(command);
            adb.StandardInput.Flush();
            adb.StandardInput.Close();

            string output = "";
            string error = "";

            try
            {
                output = adb.StandardOutput.ReadToEnd();
                error = adb.StandardError.ReadToEnd();
            }
            catch { }
            if (command.Contains("connect"))
            {
                bool graceful = adb.WaitForExit(3000);  //Wait 3 secs.
                if (!graceful)
                {
                    adb.Kill();
                }
            }
            else
            {
                adb.WaitForExit();
            }

            if (error.Contains("ADB_VENDOR_KEYS") && !Properties.Settings.Default.adbdebugwarned)
            {
                DialogResult dialogResult = FlexibleMessageBox.Show("Please check inside your headset for ADB DEBUGGING prompt, check box to \"Always allow from this computer.\" and hit OK.\nPlease note that even if you have done this\nbefore it will reset itself from time to time.\n\nPress CANCEL if you want to disable this prompt (FOR DEBUGGING ONLY, NOT RECOMMENDED).", "ADB Debugging not enabled.", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    Properties.Settings.Default.adbdebugwarned = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    ADB.WakeDevice();
                }
            }
            if (error.Contains("not enough storage space"))
            {
                _ = FlexibleMessageBox.Show("There is not enough room on your device to install this package. Please clear AT LEAST 2x the amount of the app you are trying to install.");
            }
            if (!output.Contains("version") && !output.Contains("KEYCODE_WAKEUP") && !output.Contains("Filesystem") && !output.Contains("package:") && !output.Equals(null))
            {
                _ = Logger.Log(output);
            }

            _ = Logger.Log(error);
            return new ProcessOutput(output, error);
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
                }
                else
                {
                    adb.WaitForExit();
                }
            }
            else if (command.Contains("install"))
            {
                bool graceful = adb.WaitForExit(120000);
                if (!graceful)
                {
                    adb.Kill();
                }
                else
                {
                    adb.WaitForExit();
                }
            }

            if (error.Contains("ADB_VENDOR_KEYS") && Properties.Settings.Default.adbdebugwarned)
            {
                DialogResult dialogResult = FlexibleMessageBox.Show("Please check inside your headset for ADB DEBUGGING prompt, check box to \"Always allow from this computer.\" and hit OK.\nPlease note that even if you have done this\nbefore it will reset itself from time to time.\n\nPress CANCEL if you want to disable this prompt (FOR DEBUGGING ONLY, NOT RECOMMENDED).", "ADB Debugging not enabled.", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    Properties.Settings.Default.adbdebugwarned = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    ADB.WakeDevice();
                }
            }
            _ = Logger.Log(output);
            _ = Logger.Log(error);
            return new ProcessOutput(output, error);
        }
        public static ProcessOutput RunCommandToString(string result, string path = "")
        {
            string command = result;
            string logcmd = command;
            if (logcmd.Contains(Environment.CurrentDirectory))
            {
                logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
            }

            _ = Logger.Log($"Running command: {logcmd}");
            adb.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            adb.StartInfo.Arguments = command;
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
                }
            }
            else
            {
                adb.WaitForExit();
            }

            if (error.Contains("ADB_VENDOR_KEYS") && Properties.Settings.Default.adbdebugwarned)
            {
                DialogResult dialogResult = FlexibleMessageBox.Show("Please check inside your headset for ADB DEBUGGING prompt, check box to \"Always allow from this computer.\" and hit OK.\nPlease note that even if you have done this\nbefore it will reset itself from time to time.\n\nPress CANCEL if you want to disable this prompt (FOR DEBUGGING ONLY, NOT RECOMMENDED).", "ADB Debugging not enabled.", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    Properties.Settings.Default.adbdebugwarned = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    ADB.WakeDevice();
                }
            }
            _ = Logger.Log(output);
            _ = Logger.Log(error);
            return new ProcessOutput(output, error);
        }


        public static ProcessOutput UninstallPackage(string package)
        {
            WakeDevice();
            ProcessOutput output = new ProcessOutput("", "");
            output += RunAdbCommandToString($"shell pm uninstall {package}");
            return output;
        }

        public static string GetAvailableSpace()
        {
            long totalSize = 0;

            long usedSize = 0;

            long freeSize = 0;
            WakeDevice();
            string[] output = RunAdbCommandToString("shell df").Output.Split('\n');

            foreach (string currLine in output)
            {
                if (currLine.StartsWith("/data/media"))
                {
                    string[] foo = currLine.Split(' ');
                    int i = 0;
                    foreach (string curr in foo)
                    {
                        if (curr.Length > 1)
                        {
                            switch (i)
                            {
                                case 0:
                                    break;
                                case 1:
                                    totalSize = long.Parse(curr) / 1000;
                                    break;
                                case 2:
                                    usedSize = long.Parse(curr) / 1000;
                                    break;
                                case 3:
                                    freeSize = long.Parse(curr) / 1000;
                                    break;
                                default:
                                    break;
                            }
                            i++;
                        }
                    }
                }
            }

            return $"Total space: {string.Format("{0:0.00}", (double)totalSize / 1000)}GB\nUsed space: {string.Format("{0:0.00}", (double)usedSize / 1000)}GB\nFree space: {string.Format("{0:0.00}", (double)freeSize / 1000)}GB";
        }

        public static bool wirelessadbON;

        public static void WakeDevice()
        {
            _ = RunAdbCommandToString("shell input keyevent KEYCODE_WAKEUP");
            if (!string.IsNullOrEmpty(Properties.Settings.Default.IPAddress) && !Properties.Settings.Default.Wired)
            {
                _ = RunAdbCommandToString(Properties.Settings.Default.IPAddress);
            }
        }


        public static ProcessOutput Sideload(string path, string packagename = "")
        {

            WakeDevice();
            ProcessOutput ret = new ProcessOutput();
            ret += RunAdbCommandToString($"install -g \"{path}\"");
            string out2 = ret.Output + ret.Error;
            if (out2.Contains("failed"))
            {
                _ = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Rookie Backups");
                _ = Logger.Log(out2);
                if (out2.Contains("offline") && !Properties.Settings.Default.nodevicemode)
                {
                    DialogResult dialogResult2 = FlexibleMessageBox.Show("Device is offline. Press Yes to reconnect, or if you don't wish to connect and just want to download the game (requires unchecking \"Delete games after install\" from settings menu) then press No.", "Device offline.", MessageBoxButtons.YesNoCancel);
                    if (dialogResult2 == DialogResult.Yes)
                    {
                        ADB.WakeDevice();
                    }
                }
                if (out2.Contains($"signatures do not match previously") || out2.Contains("INSTALL_FAILED_VERSION_DOWNGRADE") || out2.Contains("signatures do not match") || out2.Contains("failed to install"))
                {
                    ret.Error = string.Empty;
                    ret.Output = string.Empty;
                    ADB.WakeDevice();
                    if (!Properties.Settings.Default.AutoReinstall)
                    {
                        DialogResult dialogResult1 = FlexibleMessageBox.Show("In place upgrade has failed. Rookie can attempt to backup your save data and\nreinstall the game automatically, however " +
                            "some games do not store their saves\nin an accessible location(less than 5%). Continue with reinstall?", "In place upgrade failed.", MessageBoxButtons.OKCancel);
                        if (dialogResult1 == DialogResult.Cancel)
                        {
                            return ret;
                        }
                    }

                    Program.form.ChangeTitle("Performing reinstall, please wait...");
                    _ = ADB.RunAdbCommandToString("kill-server");
                    _ = ADB.RunAdbCommandToString("devices");
                    _ = ADB.RunAdbCommandToString($"pull /sdcard/Android/data/{MainForm.CurrPCKG} \"{Environment.CurrentDirectory}\"");
                    Program.form.ChangeTitle("Uninstalling game...");
                    _ = Sideloader.UninstallGame(MainForm.CurrPCKG);
                    Program.form.ChangeTitle("Reinstalling Game");
                    ret += ADB.RunAdbCommandToString($"install -g \"{path}\"");
                    _ = ADB.RunAdbCommandToString($"push \"{Environment.CurrentDirectory}\\{MainForm.CurrPCKG}\" /sdcard/Android/data/");
                    if (Directory.Exists($"{Environment.CurrentDirectory}\\{MainForm.CurrPCKG}"))
                    {
                        Directory.Delete($"{Environment.CurrentDirectory}\\{MainForm.CurrPCKG}", true);
                    }

                    Program.form.ChangeTitle(" \n\n");
                    return ret;
                }
            }
            string gamenameforQU = Sideloader.PackageNametoGameName(packagename);
            if (Properties.Settings.Default.QUturnedon)
            {
                if (gamenameforQU.Contains("-QU") || path.Contains("-QU"))
                {
                    string gameName = packagename;
                    packagename = Sideloader.gameNameToPackageName(gameName);

                    Program.form.ChangeTitle("Pushing Custom QU S3 Config.JSON.");
                    if (!Directory.Exists($"/sdcard/android/data/{packagename}"))
                    {
                        _ = RunAdbCommandToString($"shell mkdir /sdcard/android/data/{packagename}");
                    }

                    if (!Directory.Exists($"/sdcard/android/data/{packagename}/private"))
                    {
                        _ = RunAdbCommandToString($"shell mkdir /sdcard/android/data/{packagename}/private");
                    }

                    Random r = new Random();
                    int x = r.Next(999999999);
                    int y = r.Next(9999999);

                    long sum = (y * (long)1000000000) + x;

                    int x2 = r.Next(999999999);
                    int y2 = r.Next(9999999);

                    long sum2 = (y2 * (long)1000000000) + x2;
                    ADB.WakeDevice();
                    Properties.Settings.Default.QUStringF = $"{{\"user_id\":{sum},\"app_id\":\"{sum2}\",";
                    Properties.Settings.Default.Save();
                    string boff = Properties.Settings.Default.QUStringF + Properties.Settings.Default.QUString;
                    File.WriteAllText($"{Properties.Settings.Default.MainDir}\\config.json", boff);
                    string blank = "";
                    File.WriteAllText($"{Properties.Settings.Default.MainDir}\\delete_settings", blank);
                    ret += ADB.RunAdbCommandToString($"push \"{Properties.Settings.Default.MainDir}\\delete_settings\" /sdcard/android/data/{packagename}/private/delete_settings");
                    ret += ADB.RunAdbCommandToString($"push \"{Properties.Settings.Default.MainDir}\\config.json\" /sdcard/android/data/{packagename}/private/config.json");
                }
            }


            Program.form.ChangeTitle("");
            return ret;
        }

        public static ProcessOutput CopyOBB(string path)
        {
            WakeDevice();

            string folder = Path.GetFileName(path);
            return !folder.Contains("+") && !folder.Contains("_") && folder.Contains(".")
                ? RunAdbCommandToString($"push \"{path}\" \"/sdcard/Android/obb\"")
                : new ProcessOutput();
        }
    }
}
