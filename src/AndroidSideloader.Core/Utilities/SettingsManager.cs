using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AndroidSideloader.Utilities;

namespace AndroidSideloader.Utilities
{
    public class SettingsManager : IDisposable
    {
        private static readonly Lazy<SettingsManager> _instance = new Lazy<SettingsManager>(() => new SettingsManager());
        private static readonly string settingsFilePath = Path.Combine(
            PlatformHelpers.GetConfigBasePath(),
            "settings.json");

        // Color stored as ARGB hex string e.g. "#FFFFFFFF"
        public string FontColorHex { get; set; } = "#FFFFFFFF";
        public string ComboBoxColorHex { get; set; } = "#FF191919";
        public string SubButtonColorHex { get; set; } = "#FF191919";
        public string TextBoxColorHex { get; set; } = "#FF191919";
        public string ButtonColorHex { get; set; } = "#FF000000";
        public string BackColorHex { get; set; } = "#FF010101";

        // Font stored as family + size
        public string FontFamily { get; set; } = "Microsoft Sans Serif";
        public float FontSize { get; set; } = 11.25f;
        public string BigFontFamily { get; set; } = "Microsoft Sans Serif";
        public float BigFontSize { get; set; } = 14f;

        public bool CheckForUpdates { get; set; } = true;
        public bool EnableMessageBoxes { get; set; } = true;
        public bool FirstRun { get; set; } = true;
        public bool DeleteAllAfterInstall { get; set; } = true;
        public bool UserJsonOnGameInstall { get; set; } = false;
        public bool CallUpgrade { get; set; } = true;
        public string BackPicturePath { get; set; } = string.Empty;
        public bool SpoofGames { get; set; } = false;
        public bool ResignAPKs { get; set; } = false;
        public string IPAddress { get; set; } = string.Empty;
        public string InstalledApps { get; set; } = string.Empty;
        public string ADBPath { get; set; } = string.Empty;
        public string MainDir { get; set; } = string.Empty;
        public bool Delsh { get; set; } = false;
        public string CurrPckg { get; set; } = string.Empty;
        public string ADBFolder { get; set; } = string.Empty;
        public bool WirelessADB { get; set; } = false;
        public string CurrentGamename { get; set; } = string.Empty;
        public bool PackageNameToCB { get; set; } = false;
        public bool DownUpHeld { get; set; } = false;
        public string CurrentLogPath { get; set; } = string.Empty;
        public string CurrentLogName { get; set; } = string.Empty;
        public string CurrentCrashPath { get; set; } = string.Empty;
        public string CurrentCrashName { get; set; } = string.Empty;
        public bool AdbDebugWarned { get; set; } = false;
        public bool NodeviceMode { get; set; } = false;
        public bool BMBFChecked { get; set; } = true;
        public string GamesList { get; set; } = string.Empty;
        public bool UploadedGameList { get; set; } = false;
        public string GlobalUsername { get; set; } = string.Empty;
        public DateTime LastTimeShared { get; set; } = new DateTime(1969, 4, 20, 16, 20, 0);
        public bool AutoReinstall { get; set; } = false;
        public string NonAppPackages { get; set; } = string.Empty;
        public DateTime LastLaunch { get; set; } = new DateTime(1969, 4, 20, 16, 20, 0);
        public string SubmittedUpdates { get; set; } = string.Empty;
        public bool ListUpped { get; set; } = false;
        public DateTime LastLaunch2 { get; set; } = new DateTime(1969, 4, 20, 16, 20, 0);
        public bool Wired { get; set; } = false;
        public string AppPackages { get; set; } = string.Empty;
        public string DownloadDir { get; set; } = string.Empty;
        public bool CustomDownloadDir { get; set; } = false;
        public bool CustomBackupDir { get; set; } = false;
        public string BackupDir { get; set; } = string.Empty;
        public bool SingleThreadMode { get; set; } = true;
        public bool VirtualFilesystemCompatibility { get; set; } = false;
        public bool UpdateSettings { get; set; } = true;
        public string UUID { get; set; } = Guid.NewGuid().ToString();
        public bool CreatePubMirrorFile { get; set; } = true;
        public bool UseDownloadedFiles { get; set; } = false;
        public float BandwidthLimit { get; set; } = 0f;
        public string[] FavoritedGames { get; set; } = Array.Empty<string>();
        public bool useProxy { get; set; } = false;
        public string ProxyAddress { get; set; } = string.Empty;
        public string ProxyPort { get; set; } = string.Empty;
        public string selectedMirror { get; set; } = string.Empty;
        public bool TrailersEnabled { get; set; } = true;
        public bool ShowAdultContent { get; set; } = false;
        public bool UseGalleryView { get; set; } = true;
        public int GalleryTileSize { get; set; } = 100;
        public string ConfigUrl { get; set; } = string.Empty;

        // Window state persistence
        public int WindowX { get; set; } = -1;
        public int WindowY { get; set; } = -1;
        public int WindowWidth { get; set; } = -1;
        public int WindowHeight { get; set; } = -1;
        public bool WindowMaximized { get; set; } = false;

        // Sort state persistence
        public int SortColumn { get; set; } = 0;
        public bool SortAscending { get; set; } = true;

        // Download queue persistence
        public string[] QueuedGames { get; set; } = Array.Empty<string>();
        public float QueuedProgressPercent { get; set; }
        public long QueuedTransfersComplete { get; set; }
        public long QueuedFileCount { get; set; }

        private SettingsManager()
        {
            Load();
            Save();
        }

        public static SettingsManager Instance => _instance.Value;

        public void Save()
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var json = JsonConvert.SerializeObject(this, settings);
                string? dir = Path.GetDirectoryName(settingsFilePath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private void Load()
        {
            Debug.WriteLine("Loading settings...");
            if (!File.Exists(settingsFilePath))
            {
                CreateDefaultSettings();
                return;
            }

            try
            {
                var json = File.ReadAllText(settingsFilePath);
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        Debug.WriteLine($"Error deserializing setting: {args.ErrorContext.Error.Message}");
                        args.ErrorContext.Handled = true;
                    }
                };

                JsonConvert.PopulateObject(json, this, settings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading settings: {ex.Message}");
                CreateDefaultSettings();
            }
        }

        private void CreateDefaultSettings()
        {
            FontColorHex = "#FFFFFFFF";
            ComboBoxColorHex = "#FF191919";
            SubButtonColorHex = "#FF191919";
            TextBoxColorHex = "#FF191919";
            ButtonColorHex = "#FF000000";
            BackColorHex = "#FF010101";
            FontFamily = "Microsoft Sans Serif";
            FontSize = 11.25f;
            BigFontFamily = "Microsoft Sans Serif";
            BigFontSize = 14f;
            CheckForUpdates = true;
            EnableMessageBoxes = true;
            FirstRun = true;
            DeleteAllAfterInstall = true;
            UserJsonOnGameInstall = false;
            CallUpgrade = true;
            BackPicturePath = string.Empty;
            SpoofGames = false;
            ResignAPKs = false;
            IPAddress = string.Empty;
            InstalledApps = string.Empty;
            ADBPath = string.Empty;
            MainDir = string.Empty;
            Delsh = false;
            CurrPckg = string.Empty;
            ADBFolder = string.Empty;
            WirelessADB = false;
            CurrentGamename = string.Empty;
            PackageNameToCB = false;
            DownUpHeld = false;
            CurrentLogPath = string.Empty;
            CurrentLogName = string.Empty;
            CurrentCrashPath = string.Empty;
            CurrentCrashName = string.Empty;
            AdbDebugWarned = false;
            NodeviceMode = false;
            BMBFChecked = true;
            GamesList = string.Empty;
            UploadedGameList = false;
            GlobalUsername = string.Empty;
            LastTimeShared = new DateTime(1969, 4, 20, 16, 20, 0);
            AutoReinstall = false;
            NonAppPackages = string.Empty;
            LastLaunch = new DateTime(1969, 4, 20, 16, 20, 0);
            SubmittedUpdates = string.Empty;
            ListUpped = false;
            LastLaunch2 = new DateTime(1969, 4, 20, 16, 20, 0);
            Wired = false;
            AppPackages = string.Empty;
            DownloadDir = string.Empty;
            CustomDownloadDir = false;
            CustomBackupDir = false;
            BackupDir = string.Empty;
            SingleThreadMode = true;
            VirtualFilesystemCompatibility = false;
            UpdateSettings = true;
            UUID = Guid.NewGuid().ToString();
            CreatePubMirrorFile = true;
            UseDownloadedFiles = false;
            BandwidthLimit = 0f;
            FavoritedGames = Array.Empty<string>();
            useProxy = false;
            ProxyAddress = string.Empty;
            ProxyPort = string.Empty;
            selectedMirror = string.Empty;
            TrailersEnabled = true;
            ShowAdultContent = false;
            UseGalleryView = true;
            GalleryTileSize = 100;
            ConfigUrl = string.Empty;
            WindowX = -1;
            WindowY = -1;
            WindowWidth = -1;
            WindowHeight = -1;
            WindowMaximized = false;
            SortColumn = 0;
            SortAscending = true;
            QueuedGames = Array.Empty<string>();
            QueuedProgressPercent = 0;
            QueuedTransfersComplete = 0;
            QueuedFileCount = 0;

            Save();
            Debug.WriteLine("Default settings created.");
        }

        public void AddFavoriteGame(string packageName)
        {
            if (!FavoritedGames.Contains(packageName))
            {
                var list = FavoritedGames.ToList();
                list.Add(packageName);
                FavoritedGames = list.ToArray();
                Save();
            }
        }

        public void RemoveFavoriteGame(string packageName)
        {
            if (FavoritedGames.Contains(packageName))
            {
                var list = FavoritedGames.ToList();
                list.Remove(packageName);
                FavoritedGames = list.ToArray();
                Save();
            }
        }

        public string GetEffectiveBackupDir()
        {
            if (CustomBackupDir && Directory.Exists(BackupDir))
            {
                return BackupDir;
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Rookie Backups");
        }

        public void Dispose()
        {
            // no-op now that we don't hold GDI objects
        }
    }
}
