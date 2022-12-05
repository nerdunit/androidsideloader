using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AndroidSideloader.Utilities
{
    internal class GeneralUtilities
    {
        public static long GetDirectorySize(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        public static string RandomPackageName()
        {
            return $"com.{GeneralUtilities.randomString(rand.Next(3, 8))}.{GeneralUtilities.randomString(rand.Next(3, 8))}";
        }
        public static string CommandOutput = "";
        public static string CommandError = "";

        public static void ExecuteCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            Process process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                CommandOutput += e.Data;
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                CommandError += e.Data;
            process.BeginErrorReadLine();

            process.WaitForExit();

            process.Close();
        }

        public static void Melt()
        {
            _ = Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 5 & Del \"" + Application.ExecutablePath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            Environment.Exit(0);
        }

        private static readonly Random rand = new Random();
        public static string randomString(int length)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder res = new StringBuilder();

            int randomInteger = rand.Next(0, valid.Length);
            while (0 < length--)
            {
                _ = res.Append(valid[randomInteger]);
                randomInteger = rand.Next(0, valid.Length);
            }
            return res.ToString();
        }

        public static ProcessOutput startProcess(string process, string path, string command)
        {
            _ = Logger.Log($"Ran process {process} with command {command} in path {path}");
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.WorkingDirectory = path;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            _ = cmd.Start();
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string error = cmd.StandardError.ReadToEnd();
            string output = cmd.StandardOutput.ReadToEnd();
            _ = Logger.Log($"Output: {output}");
            _ = Logger.Log($"Error: {error}");
            return new ProcessOutput(output, error);
        }

    }
}
