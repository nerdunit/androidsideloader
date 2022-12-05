using AndroidSideloader;
using AndroidSideloader.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace Spoofer
{
    internal class spoofer
    {
        public static string alias = string.Empty;
        public static string password = string.Empty;

        public static void Init()
        {
            //If there is no keystore or details (user and pw for keystore) generate them!
            if ((File.Exists("keystore.key") == false || File.Exists("details.txt") == false) && HasDependencies())
            {
                Random rand = new Random();
                alias = GeneralUtilities.randomString(8);
                password = GeneralUtilities.randomString(16);
                string subject = $"CN = {GeneralUtilities.randomString(rand.Next(2, 6))}, OU = {GeneralUtilities.randomString(rand.Next(2, 6))}, O = {GeneralUtilities.randomString(rand.Next(2, 6))}, L = {GeneralUtilities.randomString(rand.Next(2, 6))}, ST = {GeneralUtilities.randomString(rand.Next(2, 6))}, C = {GeneralUtilities.randomString(rand.Next(2, 6))}";
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                _ = cmd.Start();
                cmd.StandardInput.WriteLine($"keytool -genkeypair -alias {alias} -keyalg RSA -keysize 2048 -keystore keystore.key -keypass {password} -storepass {password} -dname \"{subject}\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                string keyerror = cmd.StandardError.ReadToEnd();
                string keyoutput = cmd.StandardOutput.ReadToEnd();
                _ = Logger.Log($"Output: {keyoutput} Error: {keyerror}");
                File.WriteAllText("details.txt", $"{alias};{password}");
            }
            else
            {
                string[] temp = File.ReadAllText("details.txt").Split(';');
                alias = temp[0];
                password = temp[1];
            }
        }

        public static string folderPath = string.Empty;

        public static string decompiledPath = string.Empty;
        public static string newPackageName = string.Empty;
        public static string originalPackageName = string.Empty;

        public static string spoofedApkPath = string.Empty;

        //public static ProcessOutput ResignAPK(string apkPath)
        //{
        //    string output = "";
        //    string oldGameName = Path.GetFileName(apkPath);
        //    folderPath = apkPath.Replace(Path.GetFileName(apkPath), "");
        //    File.Move(apkPath, $"{folderPath}temp.apk");
        //    apkPath = $"{folderPath}temp.apk";
        //    decompiledPath = apkPath.Replace(".apk", "");
        //    string packagename = PackageName(apkPath);
        //}

        public static ProcessOutput SpoofApk(string apkPath, string newPackageName, string obbPath = "", string spoofedFileName = "spoofed.apk")
        {
            //Rename
            ProcessOutput output = new ProcessOutput("", "");
            _ = Path.GetFileName(apkPath);
            folderPath = apkPath.Replace(Path.GetFileName(apkPath), "");
            File.Move(apkPath, $"{folderPath}temp.apk");
            apkPath = $"{folderPath}temp.apk";
            decompiledPath = apkPath.Replace(".apk", "");
            //newPackageName = $"com.{Utilities.randomString(rand.Next(3, 8))}.{Utilities.randomString(rand.Next(3, 8))}";
            originalPackageName = PackageName(apkPath);
            _ = Logger.Log($"Your app will be spoofed as {newPackageName}");
            _ = Logger.Log($"Folderpath: {folderPath} decompiledPaht: {decompiledPath} ");
            if (obbPath.Length > 1)
            {
                RenameObb(obbPath, newPackageName, originalPackageName);
            }

            output += DecompileApk(apkPath);

            //Rename APK Packagename
            string foo = File.ReadAllText($"{decompiledPath}\\AndroidManifest.xml").Replace(originalPackageName, newPackageName);
            File.WriteAllText($"{decompiledPath}\\AndroidManifest.xml", foo);
            foreach (string file in Directory.EnumerateFiles(decompiledPath, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == "BuildConfig.smali")
                {
                    foo = File.ReadAllText(file).Replace(originalPackageName, newPackageName);
                    File.WriteAllText(file, foo);
                }
            }

            //spoofedApkPath = $"{Path.GetFileName(apkPath).Replace(".apk", "")}_Spoofed as {newPackageName}.apk";
            spoofedApkPath = Path.GetDirectoryName(apkPath) + "\\" + spoofedFileName;
            string apkDecompiledPath = Path.GetFileName(apkPath).Replace(".apk", "");

            output += GeneralUtilities.startProcess("cmd.exe", folderPath, $"apktool b \"{apkDecompiledPath}\" -o \"{spoofedApkPath}\"");

            //Sign the new apk
            if (File.Exists(folderPath + "keystore.key") == false)
            {
                File.Copy("keystore.key", $"{folderPath}keystore.key");
            }

            output += SignApk(spoofedApkPath);

            //Delete the copy of the key and the decompiled apk folder
            if (string.Equals(folderPath, Environment.CurrentDirectory + "\\") == false)
            {
                File.Delete($"{folderPath}keystore.key");
            }

            Directory.Delete(decompiledPath, true);
            File.Delete(apkPath);

            return output;
        }

        public static ProcessOutput SignApk(string path)
        {
            //Logger.Log($"jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore keystore.key \"{path}\" {alias}");
            Process cmdSign = new Process();
            cmdSign.StartInfo.FileName = "cmd.exe";
            cmdSign.StartInfo.RedirectStandardInput = true;
            cmdSign.StartInfo.WorkingDirectory = folderPath;
            cmdSign.StartInfo.CreateNoWindow = true;
            cmdSign.StartInfo.UseShellExecute = false;
            cmdSign.StartInfo.RedirectStandardOutput = true; //
            cmdSign.StartInfo.RedirectStandardError = true; //
            _ = cmdSign.Start();
            cmdSign.StandardInput.WriteLine($"jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore keystore.key \"{path}\" {alias}");
            cmdSign.StandardInput.WriteLine(password);
            cmdSign.StandardInput.Flush();
            cmdSign.StandardInput.Close();
            cmdSign.StandardOutput.Close();
            cmdSign.WaitForExit();
            //For some reason it hangs when also reading output...
            //string output = cmdSign.StandardOutput.ReadToEnd();
            //string error = cmdSign.StandardError.ReadToEnd();
            //Logger.Log("Jarsign Output " + output);
            //Logger.Log("Error: " + error);
            //return new ProcessOutput(output, error);
            return new ProcessOutput("", "");
        }

        public static ProcessOutput DecompileApk(string path)
        {
            ProcessOutput output = GeneralUtilities.startProcess("cmd.exe", folderPath, $"apktool d -f \"{path}\"");
            return output;
        }

        public static bool HasDependencies()
        {
            return ExistsOnPath("jarsigner") && ExistsOnPath("apktool") && ExistsOnPath("aapt");
        }

        public static bool ExistsOnPath(string exeName)
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = "where";
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.Arguments = exeName;
                    _ = p.Start();
                    p.WaitForExit();
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                throw new Exception("'where' command is not on path");
            }
        }

        public static string GetFullPath(string exeName)
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = "where";
                    p.StartInfo.Arguments = exeName;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    _ = p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    {
                        return null;
                    }

                    // just return first match
                    return output.Substring(0, output.IndexOf(Environment.NewLine));
                }
            }
            catch
            {
                throw new Exception("'where' command is not on path");
            }
        }

        //Renames obb to new obb according to packagename
        public static void RenameObb(string obbPath, string newPackageName, string originalPackageName)
        {
            Directory.Move(obbPath, obbPath.Replace(originalPackageName, newPackageName));
            obbPath = obbPath.Replace(originalPackageName, newPackageName);
            foreach (string file in Directory.GetFiles(obbPath))
            {
                if (Path.GetExtension(file) == ".obb")
                {
                    File.Move(file, file.Replace(originalPackageName, newPackageName));
                }
            }
        }

        public static string PackageName(string path)
        {
            _ = Logger.Log($"aapt dump badging \"{path}\"");

            string originalPackageName = GeneralUtilities.startProcess("cmd.exe", path.Replace(Path.GetFileName(path), string.Empty), $"aapt dump badging \"{path}\" | findstr -i \"package: name\"").Output;
            _ = Logger.Log($"originalPackageName: {originalPackageName}");
            try
            {
                originalPackageName = originalPackageName.Substring(originalPackageName.IndexOf("package: name='") + 15);
                originalPackageName = originalPackageName.Substring(0, originalPackageName.IndexOf("'"));
            }
            catch
            {
                return "PackageName ERROR";
            }
            return originalPackageName;
        }
    }
}