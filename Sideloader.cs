using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal class Sideloader
    {
        public static string TempFolder = Path.Combine(Environment.CurrentDirectory, "temp");
        public static string CrashLogPath = "crashlog.txt";

        //push user.json to device (required for some devices like oculus quest)
        public static void PushUserJsons()
        {
            foreach (string userJson in UsernameForm.userJsons)
            {
                UsernameForm.createUserJsonByName(Utilities.GeneralUtilities.randomString(16), userJson);
                _ = ADB.RunAdbCommandToString("push \"" + Environment.CurrentDirectory + $"\\{userJson}\" " + " /sdcard/");
                File.Delete(userJson);
            }
        }

        //List of all installed package names from connected device
        //public static List<string> InstalledPackageNames = new List<string>();        //Remove folder from device
        public static ProcessOutput RemoveFolder(string path)
        {
            if (path == "/sdcard/Android/obb/" || path == "sdcard/Android/data/")
            {
                return null;
            }
            return ADB.RunAdbCommandToString($"shell rm -r {path}");
        }

        public static ProcessOutput RemoveFile(string path)
        {
            return ADB.RunAdbCommandToString($"shell rm -f {path}");
        }

        //For games that require manual install, like having another folder that isnt an obb
        public static ProcessOutput RunADBCommandsFromFile(string path)
        {
            ProcessOutput output = new ProcessOutput();
            string[] commands = File.ReadAllLines(path);
            string currfolder = Path.GetDirectoryName(path);
            string[] zipz = Directory.GetFiles(currfolder, "*.7z", SearchOption.AllDirectories);
            foreach (string zip in zipz)
            {
                Utilities.Zip.ExtractFile($"{zip}", currfolder);
            }
            foreach (string cmd in commands)
            {
                if (cmd.StartsWith("adb"))
                {
                    string replacement = "";
                    string pattern = "adb";
                    replacement = ADB.DeviceID.Length > 1
                        ? $"{Properties.Settings.Default.ADBPath} -s {ADB.DeviceID}"
                        : $"{Properties.Settings.Default.ADBPath}";
                    Regex rgx = new Regex(pattern);
                    string result = rgx.Replace(cmd, replacement);
                    Program.form.changeTitle($"Running {result}");
                    _ = Logger.Log($"Logging command: {result} from file: {path}");
                    output += ADB.RunAdbCommandToStringWOADB(result, path);
                    if (output.Error.Contains("mkdir"))
                    {
                        output.Error = "";
                    }

                    if (output.Output.Contains("reserved"))
                    {
                        output.Output = "";
                    }
                }
            }
            output.Output += "Custom install successful!";
            return output;
        }





        //Recursive sideload any apk fileD
        public static ProcessOutput RecursiveOutput = new ProcessOutput();
        public static void RecursiveSideload(string FolderPath)
        {
            try
            {
                foreach (string f in Directory.GetFiles(FolderPath))
                {
                    if (Path.GetExtension(f) == ".apk")
                    {
                        RecursiveOutput += ADB.Sideload(f);
                    }
                }

                foreach (string d in Directory.GetDirectories(FolderPath))
                {
                    RecursiveSideload(d);
                }
            }
            catch (Exception ex) { _ = Logger.Log(ex.Message, LogLevel.ERROR); }
        }

        //Recursive copy any obb folder
        public static void RecursiveCopyOBB(string FolderPath)
        {
            try
            {
                foreach (string f in Directory.GetFiles(FolderPath))
                {
                    RecursiveOutput += ADB.CopyOBB(f);
                }

                foreach (string d in Directory.GetDirectories(FolderPath))
                {
                    RecursiveCopyOBB(d);
                }
            }
            catch (Exception ex) { _ = Logger.Log(ex.Message, LogLevel.ERROR); }
        }

        // Removes the game package and its OBB + Data Folders.
        public static ProcessOutput UninstallGame(string packagename)
        {
            ProcessOutput output = ADB.UninstallPackage(packagename);
            Program.form.changeTitle("");
            _ = Sideloader.RemoveFolder("/sdcard/Android/obb/" + packagename);
            _ = Sideloader.RemoveFolder("/sdcard/Android/data/" + packagename);
            return output;
        }

        public static void BackupGame(string packagename)
        {
            if (!Properties.Settings.Default.customBackupDir)
            {
                MainForm.backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Rookie Backups");
            }
            else
            {
                MainForm.backupFolder = Path.Combine((Properties.Settings.Default.backupDir), $"Rookie Backups");
            }
            if (!Directory.Exists(MainForm.backupFolder))
            {
                _ = Directory.CreateDirectory(MainForm.backupFolder);
            }
            Program.form.changeTitle($"Attempting to backup any savedata to {MainForm.backupFolder}\\Rookie Backups...");
            _ = new ProcessOutput("", "");
            string date_str = DateTime.Today.ToString("yyyy.MM.dd");
            string CurrBackups = Path.Combine(MainForm.backupFolder, date_str);
            if (!Directory.Exists(CurrBackups))
            {
                _ = Directory.CreateDirectory(CurrBackups);
            }
            _ = ADB.RunAdbCommandToString($"pull \"/sdcard/Android/data/{packagename}\" \"{CurrBackups}\"");
        }

        public static ProcessOutput DeleteFile(string GameName)
        {
            ProcessOutput output = new ProcessOutput("", "");

            string packageName = Sideloader.gameNameToPackageName(GameName);

            DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, $"Are you sure you want to uninstall custom QU settings for {packageName}? this CANNOT be undone!", "WARNING!", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes)
            {
                return output;
            }

            output = Sideloader.RemoveFile($"/sdcard/Android/data/{packageName}/private/Config.Json");

            return output;
        }

        //Extracts apk from device, saves it by package name to sideloader folder
        public static ProcessOutput getApk(string GameName)
        {
            _ = new ProcessOutput("", "");

            string packageName = Sideloader.gameNameToPackageName(GameName);

            ProcessOutput output = ADB.RunAdbCommandToString("shell pm path " + packageName);

            string apkPath = output.Output; //Get apk

            apkPath = apkPath.Remove(apkPath.Length - 1);
            apkPath = apkPath.Remove(0, 8); //remove package:
            apkPath = apkPath.Remove(apkPath.Length - 1);
            if (File.Exists($"{Properties.Settings.Default.ADBFolder}\\base.apk"))
            {
                File.Delete($"{Properties.Settings.Default.ADBFolder}\\base.apk");
            }

            if (File.Exists($"{Properties.Settings.Default.MainDir}\\{packageName}\\{packageName}.apk"))
            {
                File.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}\\{packageName}.apk");
            }

            output += ADB.RunAdbCommandToString("pull " + apkPath); //pull apk

            if (Directory.Exists($"{Properties.Settings.Default.MainDir}\\{packageName}"))
            {
                Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
            }

            _ = Directory.CreateDirectory($"{Properties.Settings.Default.MainDir}\\{packageName}");

            File.Move($"{Properties.Settings.Default.ADBFolder}\\base.apk", $"{Properties.Settings.Default.MainDir}\\{packageName}\\{packageName}.apk");
            return output;
        }

        public static string gameNameToPackageName(string gameName)
        {
            foreach (string[] game in SideloaderRCLONE.games)
            {
                if (gameName.Equals(game[SideloaderRCLONE.GameNameIndex]) || gameName.Equals(game[SideloaderRCLONE.ReleaseNameIndex]))
                    return game[SideloaderRCLONE.PackageNameIndex];
            }
            return gameName;
        }

        public static string PackageNametoGameName(string packageName)
        {
            foreach (string[] game in SideloaderRCLONE.games)
            {
                if (packageName.Equals(game[SideloaderRCLONE.PackageNameIndex]))
                    return game[SideloaderRCLONE.ReleaseNameIndex];
            }
            return packageName;
        }

        public static string gameNameToSimpleName(string gameName)
        {
            foreach (string[] game in SideloaderRCLONE.games)
            {
                if (gameName.Equals(game[SideloaderRCLONE.GameNameIndex]) || gameName.Equals(game[SideloaderRCLONE.ReleaseNameIndex]))
                    return game[SideloaderRCLONE.GameNameIndex];
            }
            return gameName;
        }

        public static string PackageNameToSimpleName(string packageName)
        {
            foreach (string[] game in SideloaderRCLONE.games)
            {
                if (packageName.Contains(game[SideloaderRCLONE.PackageNameIndex]))
                    return game[SideloaderRCLONE.GameNameIndex];
            }
            return packageName;
        }

        // Download required dependencies.
        public static void downloadFiles()
        {
            WebClient client = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var currentAccessedWebsite = "";
            try
            {
                if (!File.Exists("Sideloader Launcher.exe"))
                {
                    currentAccessedWebsite = "github";
                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/Sideloader%20Launcher.exe", "Sideloader Launcher.exe");
                }

                if (!File.Exists("Rookie Offline.cmd"))
                {
                    currentAccessedWebsite = "github";
                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/Rookie%20Offline.cmd", "Rookie Offline.cmd");
                }

                if (!File.Exists($"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools\\aug2021.txt") || !File.Exists($"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools\\adb.exe")) //if adb is not updated, download and auto extract
                {
                    if (Directory.Exists($"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\2.8.2"))
                    {
                        Directory.Delete($"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\2.8.2", true);
                    }

                    if (Directory.Exists($"{Properties.Settings.Default.MainDir}\\adb"))
                    {
                        Directory.Delete($"{Properties.Settings.Default.MainDir}\\adb", true);
                    }

                    if (!Directory.Exists($"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools"))
                    {
                        _ = Directory.CreateDirectory($"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools");
                    }

                    currentAccessedWebsite = "github";
                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/adb2.zip", "Ad.7z");
                    Utilities.Zip.ExtractFile(Environment.CurrentDirectory + "\\Ad.7z", $"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools");
                    File.Delete("Ad.7z");
                }

                if (!Directory.Exists(Environment.CurrentDirectory + "\\rclone"))
                {
                    currentAccessedWebsite = "rclone";
                    string url = Environment.Is64BitOperatingSystem
                        ? "https://downloads.rclone.org/v1.62.2/rclone-v1.62.2-windows-amd64.zip"
                        : "https://downloads.rclone.org/v1.62.2/rclone-v1.62.2-windows-386.zip";
                    //Since sideloader is build for x86, it should work on both x86 and x64 so we download the according rclone version

                    client.DownloadFile(url, "rclone.zip");

                    Utilities.Zip.ExtractFile(Environment.CurrentDirectory + "\\rclone.zip", Environment.CurrentDirectory);

                    File.Delete("rclone.zip");

                    string[] folders = Directory.GetDirectories(Environment.CurrentDirectory);
                    foreach (string folder in folders)
                    {
                        if (folder.Contains("rclone"))
                        {
                            Directory.Move(folder, "rclone");
                            break; //only 1 rclone folder
                        }
                    }
                }
                else
                {
                    string pathToRclone = Path.Combine(Environment.CurrentDirectory, "rclone", "rclone.exe");
                    if (File.Exists(pathToRclone))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(pathToRclone);
                        string version = versionInfo.ProductVersion;
                        Logger.Log($"Current RCLONE Version {version}");
                        if (!MainForm.noRcloneUpdating)
                        {
                            if (version != "1.62.2")
                            {
                                Logger.Log("RCLONE Version not matching! Downloading required version.", LogLevel.WARNING);
                                File.Delete(pathToRclone);
                                currentAccessedWebsite = "rclone";
                                string architecture = Environment.Is64BitOperatingSystem ? "amd64" : "386";
                                string url = $"https://downloads.rclone.org/v1.62.2/rclone-v1.62.2-windows-{architecture}.zip";
                                client.DownloadFile(url, "rclone.zip");
                                Utilities.Zip.ExtractFile(Path.Combine(Environment.CurrentDirectory, "rclone.zip"), Environment.CurrentDirectory);
                                File.Delete("rclone.zip");
                                string rcloneDirectory = Path.Combine(Environment.CurrentDirectory, $"rclone-v1.62.2-windows-{architecture}");
                                File.Move(Path.Combine(rcloneDirectory, "rclone.exe"), pathToRclone);
                                Directory.Delete(rcloneDirectory, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (currentAccessedWebsite == "github")
                {
                    _ = FlexibleMessageBox.Show($"You are unable to access the raw.githubusercontent.com page with the Exception: {ex.Message}\nSome files may be missing (ADB, Offline Script, Launcher)");
                    _ = FlexibleMessageBox.Show("These required files were unable to be downloaded\nRookie will now close, please use Offline Mode for manual sideloading if needed");
                    Application.Exit();
                }
                if (currentAccessedWebsite == "rclone")
                {
                    _ = FlexibleMessageBox.Show($"You are unable to access the rclone page with the Exception: {ex.Message}\nSome files may be missing (RCLONE)");
                    _ = FlexibleMessageBox.Show("Rclone was unable to be downloaded\nRookie will now close, please use Offline Mode for manual sideloading if needed");
                    Application.Exit();
                }
            }
        }
    }


}
