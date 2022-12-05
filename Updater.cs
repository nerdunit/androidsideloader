using System;
using System.Text;
using System.Diagnostics;
using JR.Utils.GUI.Forms;
using System.Net;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using AndroidSideloader;

namespace AndroidSideloader
{
    class Updater
    {
        public static string AppName { get; set; }
        public static string Repostory { get; set; }
        private static string RawGitHubUrl;
        private static string GitHubUrl;

        static readonly public string LocalVersion = "2.17";
        public static string currentVersion = string.Empty;
        public static string changelog = string.Empty;

        //Check if there is a new version of the sideloader
        private static bool IsUpdateAvailable()
        {
            HttpClient client = new HttpClient();
            try
            {
                currentVersion = client.GetStringAsync($"{RawGitHubUrl}/master/version").Result;
                changelog = client.GetStringAsync($"{RawGitHubUrl}/master/changelog.txt").Result;
                client.Dispose();
                currentVersion = currentVersion.Trim();
            }
            catch { return false; }
            return LocalVersion.Trim() != currentVersion;
        }

        //Call this to ask the user if they want to update
        public static void Update()
        {
            RawGitHubUrl = $"https://raw.githubusercontent.com/nerdunit/androidsideloader";
            GitHubUrl = $"https://github.com/nerdunit/androidsideloader";
            if (IsUpdateAvailable())
            {
                UpdateForm upForm = new UpdateForm();
                upForm.ShowDialog(); ;
            }


        }

        //If the user wants to update
        public static void doUpdate()
        {
            try
            {
                ADB.RunAdbCommandToString("kill-server");
                var fileClient = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Logger.Log($"Downloading update from {GitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe to {AppName} v{currentVersion}.exe");
                fileClient.DownloadFile($"{GitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe", $"{AppName} v{currentVersion}.exe");
                fileClient.Dispose();
                Logger.Log($"Starting {AppName} v{currentVersion}.exe");
                Process.Start($"{AppName} v{currentVersion}.exe");
                //Delete current version
                AndroidSideloader.Utilities.GeneralUtilities.Melt();
            }
            catch { }
        }
    }
}
