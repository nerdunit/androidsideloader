using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace AndroidSideloader
{
    internal class Updater
    {
        public static string AppName { get; set; }
        public static string Repostory { get; set; }
        private static string RawGitHubUrl;
        private static string GitHubUrl;

        public static readonly string LocalVersion = "2.17";
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
                _ = upForm.ShowDialog(); ;
            }


        }

        //If the user wants to update
        public static void doUpdate()
        {
            try
            {
                _ = ADB.RunAdbCommandToString("kill-server");
                WebClient fileClient = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                _ = Logger.Log($"Downloading update from {GitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe to {AppName} v{currentVersion}.exe");
                fileClient.DownloadFile($"{GitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe", $"{AppName} v{currentVersion}.exe");
                fileClient.Dispose();
                _ = Logger.Log($"Starting {AppName} v{currentVersion}.exe");
                _ = Process.Start($"{AppName} v{currentVersion}.exe");
                //Delete current version
                AndroidSideloader.Utilities.GeneralUtilities.Melt();
            }
            catch { }
        }
    }
}
