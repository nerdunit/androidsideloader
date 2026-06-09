using AndroidSideloader.Utilities;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    public class Sideloader
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;
        public static string TempFolder = Path.Combine(AppContext.BaseDirectory, "temp");
        public static string CrashLogPath = "crashlog.txt";

        public static void PushUserJsons(string[] userJsons)
        {
            foreach (string userJson in userJsons)
            {
                if (File.Exists(userJson))
                {
                    _ = ADB.RunAdbCommandToString("push \"" + Path.Combine(AppContext.BaseDirectory, userJson) + "\" /sdcard/");
                    File.Delete(userJson);
                }
            }
        }

        public static ProcessOutput RemoveFolder(string path)
        {
            if (path == "/sdcard/Android/obb/" || path == "sdcard/Android/data/")
            {
                return new ProcessOutput("", "");
            }
            return ADB.RunAdbCommandToString($"shell rm -r \"{path}\"");
        }

        public static ProcessOutput RemoveFile(string path)
        {
            return ADB.RunAdbCommandToString($"shell rm -f \"{path}\"");
        }

        public static ProcessOutput RunADBCommandsFromFile(string path)
        {
            ProcessOutput output = new ProcessOutput();
            string[] commands = File.ReadAllLines(path);
            string currfolder = Path.GetDirectoryName(path) ?? AppContext.BaseDirectory;
            string[] zipz = Directory.GetFiles(currfolder, "*.7z", SearchOption.AllDirectories);
            foreach (string zip in zipz)
            {
                Utilities.Zip.ExtractFile(zip, currfolder);
            }
            foreach (string cmd in commands)
            {
                if (cmd.StartsWith("adb"))
                {
                    string replacement = "";
                    string pattern = "adb";
                    replacement = ADB.DeviceID.Length > 1
                        ? $"{settings.ADBPath} -s {ADB.DeviceID}"
                        : $"{settings.ADBPath}";
                    Regex rgx = new Regex(pattern);
                    string result = rgx.Replace(cmd, replacement);
                    AppState.ChangeTitle($"Running {result}");
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

        public static ProcessOutput RecursiveOutput = new ProcessOutput();

        public static async Task RecursiveSideloadAsync(
            string FolderPath,
            Action<float, TimeSpan?>? progressCallback = null,
            Action<string>? statusCallback = null)
        {
            try
            {
                foreach (string f in Directory.GetFiles(FolderPath))
                {
                    if (Path.GetExtension(f) == ".apk")
                    {
                        string gameName = Path.GetFileNameWithoutExtension(f);
                        statusCallback?.Invoke(gameName);
                        RecursiveOutput += await ADB.SideloadWithProgressAsync(f, progressCallback, statusCallback, "", gameName);
                    }
                }

                foreach (string d in Directory.GetDirectories(FolderPath))
                {
                    await RecursiveSideloadAsync(d, progressCallback, statusCallback);
                }
            }
            catch (Exception ex) { _ = Logger.Log(ex.Message, LogLevel.ERROR); }
        }

        public static async Task RecursiveCopyOBBAsync(
            string FolderPath,
            Action<float, TimeSpan?>? progressCallback = null,
            Action<string>? statusCallback = null)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(FolderPath))
                {
                    string folderName = Path.GetFileName(d);
                    if (folderName.Contains("."))
                    {
                        statusCallback?.Invoke(folderName);
                        string packageName = folderName;
                        RecursiveOutput += await ADB.CopyOBBWithProgressAsync(folderName, d, packageName, progressCallback, statusCallback);
                    }
                    else
                    {
                        await RecursiveCopyOBBAsync(d, progressCallback, statusCallback);
                    }
                }
            }
            catch (Exception ex) { _ = Logger.Log(ex.Message, LogLevel.ERROR); }
        }

        public static ProcessOutput UninstallGame(string packagename)
        {
            ProcessOutput output = ADB.UninstallPackage(packagename);
            AppState.ChangeTitle("");
            _ = Sideloader.RemoveFolder("/sdcard/Android/obb/" + packagename);
            _ = Sideloader.RemoveFolder("/sdcard/Android/data/" + packagename);
            return output;
        }

        public static string GetEffectiveBackupDir()
        {
            return settings.GetEffectiveBackupDir();
        }

        public static void BackupGame(string packagename)
        {
            string backupFolder = settings.GetEffectiveBackupDir();
            if (!Directory.Exists(backupFolder))
            {
                _ = Directory.CreateDirectory(backupFolder);
            }
            AppState.ChangeTitle($"Attempting to backup any savedata to {backupFolder}/Rookie Backups...");
            string date_str = DateTime.Today.ToString("yyyy.MM.dd");
            string CurrBackups = Path.Combine(backupFolder, date_str);
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
            output = Sideloader.RemoveFile($"/sdcard/Android/data/{packageName}/private/Config.Json");
            return output;
        }

        public static ProcessOutput getApk(string GameName)
        {
            _ = new ProcessOutput("", "");
            string packageName = Sideloader.gameNameToPackageName(GameName);
            ProcessOutput output = ADB.RunAdbCommandToString("shell pm path " + packageName);
            string apkPath = output.Output;
            apkPath = apkPath.Remove(apkPath.Length - 1);
            apkPath = apkPath.Remove(0, 8);
            apkPath = apkPath.Remove(apkPath.Length - 1);

            string baseApkPath = Path.Combine(settings.ADBFolder, "base.apk");
            if (File.Exists(baseApkPath))
            {
                File.Delete(baseApkPath);
            }

            string targetDir = Path.Combine(settings.MainDir, packageName);
            string targetApk = Path.Combine(targetDir, $"{packageName}.apk");
            if (File.Exists(targetApk))
            {
                File.Delete(targetApk);
            }

            output += ADB.RunAdbCommandToString("pull " + apkPath);

            if (Directory.Exists(targetDir))
            {
                FileSystemUtilities.TryDeleteDirectory(targetDir);
            }
            _ = Directory.CreateDirectory(targetDir);

            if (File.Exists(baseApkPath))
            {
                File.Move(baseApkPath, targetApk);
            }
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

        public static string gameNameToVersionCode(string gameName)
        {
            foreach (string[] game in SideloaderRCLONE.games)
            {
                if (gameName.Equals(game[SideloaderRCLONE.GameNameIndex]) || gameName.Equals(game[SideloaderRCLONE.ReleaseNameIndex]))
                    return game[SideloaderRCLONE.VersionCodeIndex];
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
    }
}
