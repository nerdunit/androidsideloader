using AndroidSideloader.Utilities;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    public class Updater
    {
        public static string AppName { get; set; } = "AndroidSideloader";
        public static string Repository { get; set; } = RepoDefaults.Repo;
        public static string Branch { get; set; } = RepoDefaults.Branch;

        public static readonly string LocalVersion = "3.4";
        public static string currentVersion = string.Empty;
        public static string changelog = string.Empty;

        private static async Task<bool> IsUpdateAvailableAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    currentVersion = await client.GetStringAsync($"https://raw.githubusercontent.com/{Repository}/{Branch}/version");
                    changelog = await client.GetStringAsync($"https://raw.githubusercontent.com/{Repository}/{Branch}/changelog.txt");
                    currentVersion = currentVersion.Trim();
                }
                catch (HttpRequestException)
                {
                    return false;
                }
            }

            return CompareVersions(currentVersion, LocalVersion.Trim()) > 0;
        }

        private static int CompareVersions(string version1, string version2)
        {
            try
            {
                string[] parts1 = version1.Split('.');
                string[] parts2 = version2.Split('.');

                int maxLength = Math.Max(parts1.Length, parts2.Length);
                for (int i = 0; i < maxLength; i++)
                {
                    int v1 = i < parts1.Length && int.TryParse(parts1[i], out int p1) ? p1 : 0;
                    int v2 = i < parts2.Length && int.TryParse(parts2[i], out int p2) ? p2 : 0;

                    if (v1 > v2) return 1;
                    if (v1 < v2) return -1;
                }

                return 0;
            }
            catch
            {
                return string.Compare(version1, version2, StringComparison.Ordinal);
            }
        }

        public static async Task<bool> CheckUpdateAsync()
        {
            return await IsUpdateAvailableAsync();
        }

        public static void DoUpdate()
        {
            try
            {
                ADB.RunAdbCommandToString("kill-server");

                using (WebClient fileClient = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string assetName = PlatformHelpers.IsWindows ? $"{AppName}.exe" : $"{AppName}";
                    string downloadUrl = $"https://github.com/{Repository}/releases/download/v{currentVersion}/{assetName}";
                    string savePath = $"{AppName} v{currentVersion}" + (PlatformHelpers.IsWindows ? ".exe" : "");

                    Logger.Log($"Downloading update from {downloadUrl} to {savePath}");
                    fileClient.DownloadFile(downloadUrl, savePath);

                    Logger.Log($"Starting {savePath}");
                    Process.Start(savePath);
                }

                Utilities.GeneralUtilities.Melt();
            }
            catch (Exception ex)
            {
                Logger.Log($"Update failed: {ex.Message}");
            }
        }
    }
}
