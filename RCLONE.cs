using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal class RCLONE
    {
        //Kill all rclone, using a static rclone variable doesn't work for some reason #tofix
        public static void killRclone()
        {
            foreach (Process process in Process.GetProcessesByName("rclone"))
            {
                process.Kill();
            }
        }

        //For custom configs that use a password
        public static void Init()
        {
            string PwTxtPath = Path.Combine(Environment.CurrentDirectory, "rclone\\pw.txt");
            if (File.Exists(PwTxtPath))
            {
                rclonepw = File.ReadAllText(PwTxtPath);
            }
        }

        //Change if you want to use a config
        public static string downloadConfigPath = "vrp.download.config";
        public static string uploadConfigPath = "vrp.upload.config";
        public static string rclonepw = "";


        private static readonly Process rclone = new Process();

        //Run rclone command
        public static ProcessOutput runRcloneCommand_DownloadConfig(string command, string BandwidthLimit = "")
        {
            if (!MainForm.HasInternet || MainForm.isOffline)
            {
                return new ProcessOutput("", "No internet");
            }

            ProcessOutput prcoutput = new ProcessOutput();
            //Rclone output is unicode, else it will show garbage instead of unicode characters
            rclone.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            string originalCommand = command;

            //set bandwidth limit
            if (BandwidthLimit.Length > 0)
            {
                command += $" --bwlimit={BandwidthLimit}";
            }

            //set configpath if there is any
            if (downloadConfigPath.Length > 0)
            {
                command += $" --config {downloadConfigPath}";
            }

            //set rclonepw
            if (rclonepw.Length > 0)
            {
                command += " --ask-password=false";
            }

            string logcmd = Utilities.StringUtilities.RemoveEverythingBeforeFirst(command, "rclone.exe");
            if (logcmd.Contains($"\"{Properties.Settings.Default.CurrentLogPath}\""))
            {
                logcmd = logcmd.Replace($"\"{Properties.Settings.Default.CurrentLogPath}\"", $"\"{Properties.Settings.Default.CurrentLogName}\"");
            }

            if (logcmd.Contains(Environment.CurrentDirectory))
            {
                logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
            }

            _ = Logger.Log($"Running Rclone command: {logcmd}");

            rclone.StartInfo.FileName = Environment.CurrentDirectory + "\\rclone\\rclone.exe";
            rclone.StartInfo.Arguments = command;
            rclone.StartInfo.RedirectStandardInput = true;
            rclone.StartInfo.RedirectStandardError = true;
            rclone.StartInfo.RedirectStandardOutput = true;
            rclone.StartInfo.WorkingDirectory = Environment.CurrentDirectory + "\\rclone";
            rclone.StartInfo.CreateNoWindow = true;
            //On debug we want to see when rclone is open
            if (MainForm.debugMode == true)
            {
                rclone.StartInfo.CreateNoWindow = false;
            }

            rclone.StartInfo.UseShellExecute = false;
            _ = rclone.Start();
            rclone.StandardInput.WriteLine(command);
            rclone.StandardInput.Flush();
            rclone.StandardInput.Close();

            string output = rclone.StandardOutput.ReadToEnd();
            string error = rclone.StandardError.ReadToEnd();
            rclone.WaitForExit();

            if (error.Contains("There is not enough space"))
            {
                _ = FlexibleMessageBox.Show($"There isn't enough disk space to download this game.\r\nPlease ensure you have at least 200MB more the game size available in {Environment.CurrentDirectory} and try again.",
                                        "NOT ENOUGH SPACE",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                return new ProcessOutput("Download failed.", "");
            }

            //if there is one of these errors, we switch the mirrors
            if (error.Contains("400 Bad Request") || error.Contains("cannot fetch token") || error.Contains("authError") || error.Contains("quota") || error.Contains("exceeded") || error.Contains("directory not found") || error.Contains("Failed to"))
            {
                string oldRemote = MainForm.currentRemote;
                try
                {
                    Program.form.SwitchMirrors();

                }
                catch
                {
                    return new ProcessOutput("All mirrors are on quota or down...", "All mirrors are on quota or down...");
                }
                prcoutput = runRcloneCommand_DownloadConfig(originalCommand.Replace(oldRemote, MainForm.currentRemote), BandwidthLimit);
            }
            else
            {
                prcoutput.Output = output;
                prcoutput.Error = error;
            }

            if (!output.Contains("Game Name;Release Name;") && !output.Contains("package:") && !output.Contains(".meta"))
            {
                if (!string.IsNullOrWhiteSpace(error))
                {
                    _ = Logger.Log($"Rclone error: {error}\n");
                }

                if (!string.IsNullOrWhiteSpace(output))
                {
                    _ = Logger.Log($"Rclone Output: {output}");
                }
            }
            return prcoutput;
        }

        public static ProcessOutput runRcloneCommand_UploadConfig(string command, string BandwidthLimit = "")
        {
            if (!MainForm.HasInternet || MainForm.isOffline)
            {
                return new ProcessOutput("", "No internet");
            }

            ProcessOutput prcoutput = new ProcessOutput();
            //Rclone output is unicode, else it will show garbage instead of unicode characters
            rclone.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            //set bandwidth limit
            if (BandwidthLimit.Length > 0)
            {
                command += $" --bwlimit={BandwidthLimit}";
            }

            //set configpath if there is any
            if (uploadConfigPath.Length > 0)
            {
                command += $" --config {uploadConfigPath}";
            }

            string logcmd = Utilities.StringUtilities.RemoveEverythingBeforeFirst(command, "rclone.exe");
            if (logcmd.Contains($"\"{Properties.Settings.Default.CurrentLogPath}\""))
            {
                logcmd = logcmd.Replace($"\"{Properties.Settings.Default.CurrentLogPath}\"", $"\"{Properties.Settings.Default.CurrentLogName}\"");
            }

            if (logcmd.Contains(Environment.CurrentDirectory))
            {
                logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
            }

            _ = Logger.Log($"Running Rclone command: {logcmd}");

            command += " --checkers 0 --no-check-dest --retries 1";

            rclone.StartInfo.FileName = Environment.CurrentDirectory + "\\rclone\\rclone.exe";
            rclone.StartInfo.Arguments = command;
            rclone.StartInfo.RedirectStandardInput = true;
            rclone.StartInfo.RedirectStandardError = true;
            rclone.StartInfo.RedirectStandardOutput = true;
            rclone.StartInfo.WorkingDirectory = Environment.CurrentDirectory + "\\rclone";
            rclone.StartInfo.CreateNoWindow = true;
            //On debug we want to see when rclone is open
            if (MainForm.debugMode == true)
            {
                rclone.StartInfo.CreateNoWindow = false;
            }

            rclone.StartInfo.UseShellExecute = false;
            _ = rclone.Start();
            rclone.StandardInput.WriteLine(command);
            rclone.StandardInput.Flush();
            rclone.StandardInput.Close();

            string output = rclone.StandardOutput.ReadToEnd();
            string error = rclone.StandardError.ReadToEnd();
            rclone.WaitForExit();

            //if there is one of these errors, we switch the mirrors
            if (error.Contains("400 Bad Request") || error.Contains("cannot fetch token") || error.Contains("authError") || error.Contains("quota") || error.Contains("exceeded") || error.Contains("directory not found") || error.Contains("Failed to"))
            {
                _ = Logger.Log(error);
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
                {
                    _ = Logger.Log($"Rclone error: {error}\n");
                }

                if (!string.IsNullOrWhiteSpace(output))
                {
                    _ = Logger.Log($"Rclone Output: {output}");
                }
            }
            return prcoutput;
        }

        public static ProcessOutput runRcloneCommand_PublicConfig(string command, string BandwidthLimit = "")
        {
            if (!MainForm.HasInternet || MainForm.isOffline)
            {
                return new ProcessOutput("", "No internet");
            }

            ProcessOutput prcoutput = new ProcessOutput();
            //Rclone output is unicode, else it will show garbage instead of unicode characters
            rclone.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            //set bandwidth limit
            if (BandwidthLimit.Length > 0)
            {
                command += $" --bwlimit={BandwidthLimit}";
            }

            string logcmd = Utilities.StringUtilities.RemoveEverythingBeforeFirst(command, "rclone.exe");
            if (logcmd.Contains($"\"{Properties.Settings.Default.CurrentLogPath}\""))
            {
                logcmd = logcmd.Replace($"\"{Properties.Settings.Default.CurrentLogPath}\"", $"\"{Properties.Settings.Default.CurrentLogName}\"");
            }

            if (logcmd.Contains(Environment.CurrentDirectory))
            {
                logcmd = logcmd.Replace($"{Environment.CurrentDirectory}", $"CurrentDirectory");
            }

            _ = Logger.Log($"Running Rclone command: {logcmd}");

            //set http source & args
            command += $" --http-url {MainForm.PublicConfigFile.BaseUri} {MainForm.PublicMirrorExtraArgs}";

            rclone.StartInfo.FileName = Environment.CurrentDirectory + "\\rclone\\rclone.exe";
            rclone.StartInfo.Arguments = command;
            rclone.StartInfo.RedirectStandardInput = true;
            rclone.StartInfo.RedirectStandardError = true;
            rclone.StartInfo.RedirectStandardOutput = true;
            rclone.StartInfo.WorkingDirectory = Environment.CurrentDirectory + "\\rclone";
            rclone.StartInfo.CreateNoWindow = true;

            //On debug we want to see when rclone is open
            if (MainForm.debugMode == true)
            {
                rclone.StartInfo.CreateNoWindow = false;
            }

            rclone.StartInfo.UseShellExecute = false;
            _ = rclone.Start();
            rclone.StandardInput.WriteLine(command);
            rclone.StandardInput.Flush();
            rclone.StandardInput.Close();

            string output = rclone.StandardOutput.ReadToEnd();
            string error = rclone.StandardError.ReadToEnd();
            rclone.WaitForExit();

            if (error.Contains("There is not enough space"))
            {
                _ = FlexibleMessageBox.Show($"There isn't enough disk space to download this game.\r\nPlease ensure you have at least 2x the game size available in {Environment.CurrentDirectory} and try again.",
                                        "NOT ENOUGH SPACE",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                return new ProcessOutput("Download failed.", "");
            }

            if (error.Contains("400 Bad Request")
                || error.Contains("cannot fetch token")
                || error.Contains("authError")
                || error.Contains("quota")
                || error.Contains("exceeded")
                || error.Contains("directory not found")
                || error.Contains("Failed to"))
            {
                _ = Logger.Log(error);
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
                {
                    _ = Logger.Log($"Rclone error: {error}\n");
                }

                if (!string.IsNullOrWhiteSpace(output))
                {
                    _ = Logger.Log($"Rclone Output: {output}");
                }
            }

            return prcoutput;
        }
    }
}
