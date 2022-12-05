using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace AndroidSideloader.Utilities
{
    internal class Zip
    {
        public static void ExtractFile(string sourceArchive, string destination)
        {
            string args = $"x \"{sourceArchive}\" -y -o\"{destination}\"";
            DoExtract(args);
        }

        public static void ExtractFile(string sourceArchive, string destination, string password)
        {
            string args = $"x \"{sourceArchive}\" -y -o\"{destination}\" -p\"{password}\"";
            DoExtract(args);
        }

        private static void DoExtract(string args)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\7z.exe") || !File.Exists(Environment.CurrentDirectory + "\\7z.dll"))
            {
                _ = Logger.Log("Begin download 7-zip");
                WebClient client = new WebClient();
                client.DownloadFile("https://github.com/nerdunit/androidsideloader/raw/master/7z.exe", "7z.exe");
                client.DownloadFile("https://github.com/nerdunit/androidsideloader/raw/master/7z.dll", "7z.dll");
                _ = Logger.Log("Complete download 7-zip");
            }
            ProcessStartInfo pro = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "7z.exe",
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            _ = Logger.Log($"Extract: 7z {string.Join(" ", args.Split(' ').Where(a => !a.StartsWith("-p")))}");

            Process x = Process.Start(pro);
            x.WaitForExit();
            if (x.ExitCode != 0)
            {
                string error = x.StandardError.ReadToEnd();

                if (error.Contains("There is not enough space on the disk"))
                {
                    _ = FlexibleMessageBox.Show($"Not enough space to extract archive.\r\nCheck free space in {Environment.CurrentDirectory} and try again.",
                        "NOT ENOUGH SPACE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                _ = Logger.Log(x.StandardOutput.ReadToEnd());
                _ = Logger.Log(error);
                throw new ApplicationException($"Extracting failed, status code {x.ExitCode}");
            }
        }
    }


}
