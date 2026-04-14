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

        public static readonly string LocalVersion = "3.11";
        public static string currentVersion = string.Empty;
        public static string changelog = string.Empty;

        // Check if there is a new version of the sideloader
        private static async Task<bool> IsUpdateAvailableAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    currentVersion = await client.GetStringAsync($"https://raw.githubusercontent.com/{MainForm.repo}/{MainForm.repo_branch}/version");
                    changelog = await client.GetStringAsync($"https://raw.githubusercontent.com/{MainForm.repo}/{MainForm.repo_branch}/changelog.txt");
                    currentVersion = currentVersion.Trim();
                }
                catch (HttpRequestException)
                {
                    return false;
                }
            }

            // Compare versions - only return true if server version is greater than local version
            return CompareVersions(currentVersion, LocalVersion.Trim()) > 0;
        }

        // Compares two semantic version strings (e.g., "2.35")
        // returns: 1 if version1 > version2, -1 if version1 < version2, 0 if equal
        private static int CompareVersions(string version1, string version2)
        {
            try
            {
                // Parse versions into parts
                string[] parts1 = version1.Split('.');
                string[] parts2 = version2.Split('.');

                // Compare each part
                int maxLength = Math.Max(parts1.Length, parts2.Length);
                for (int i = 0; i < maxLength; i++)
                {
                    int v1 = i < parts1.Length && int.TryParse(parts1[i], out int p1) ? p1 : 0;
                    int v2 = i < parts2.Length && int.TryParse(parts2[i], out int p2) ? p2 : 0;

                    if (v1 > v2) return 1;
                    if (v1 < v2) return -1;
                }

                return 0; // Versions are equal
            }
            catch
            {
                // Fallback to string comparison if parsing fails
                return string.Compare(version1, version2, StringComparison.Ordinal);
            }
        }

        // Ask the user if they want to update
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

                    Logger.Log($"Downloading update from https://github.com/{MainForm.repo}/releases/download/v{currentVersion}/{AppName}.exe to {AppName} v{currentVersion}.exe");
                    fileClient.DownloadFile($"https://github.com/{MainForm.repo}/releases/download/v{currentVersion}/{AppName}.exe", $"{AppName} v{currentVersion}.exe");

                    Logger.Log($"Starting {AppName} v{currentVersion}.exe");
                    Process.Start($"{AppName} v{currentVersion}.exe");
                }

                // Delete current version
                Utilities.GeneralUtilities.Melt();
            }
            catch (Exception ex)
            {
                // Handle specific exceptions that might occur during the update process
                Logger.Log($"Update failed: {ex.Message}");
            }
        }
    }
}