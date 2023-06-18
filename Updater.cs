using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    internal class Updater
    {
        public static string AppName { get; set; }
        public static string Repository { get; set; }
        private static readonly string RawGitHubUrl = "https://raw.githubusercontent.com/nerdunit/androidsideloader";
        private static readonly string GitHubUrl = "https://github.com/nerdunit/androidsideloader";

        public static readonly string LocalVersion = "2.20";
        public static string currentVersion = string.Empty;
        public static string changelog = string.Empty;

        // Check if there is a new version of the sideloader
        private static async Task<bool> IsUpdateAvailableAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    currentVersion = await client.GetStringAsync($"{RawGitHubUrl}/master/version");
                    changelog = await client.GetStringAsync($"{RawGitHubUrl}/master/changelog.txt");
                    currentVersion = currentVersion.Trim();
                }
                catch (HttpRequestException)
                {
                    return false;
                }
            }

            return LocalVersion.Trim() != currentVersion;
        }

        // Call this to ask the user if they want to update
        public static async Task Update()
        {
            if (await IsUpdateAvailableAsync())
            {
                UpdateForm upForm = new UpdateForm();
                _ = upForm.ShowDialog();
            }
        }

        // If the user wants to update
        public static void doUpdate()
        {
            try
            {
                ADB.RunAdbCommandToString("kill-server");

                using (WebClient fileClient = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        
                    Logger.Log($"Downloading update from {GitHubUrl}/releases/download/v{CurrentVersion}/{AppName}.exe to {AppName} v{currentVersion}.exe");
                    fileClient.DownloadFile($"{GitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe", $"{AppName} v{currentVersion}.exe");

                    Logger.Log($"Starting {AppName} v{currentVersion}.exe");
                    Process.Start($"{AppName} v{currentVersion}.exe");
                }

                // Delete current version
                AndroidSideloader.Utilities.GeneralUtilities.Melt();
            }
            catch (Exception ex)
            {
                // Handle specific exceptions that might occur during the update process
                Logger.Log($"Update failed: {ex.Message}");
            }
        }
    }
}