using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AndroidSideloader.Utilities
{
    public class SettingsManager : IDisposable
    {
        private static readonly Lazy<SettingsManager> _instance = new Lazy<SettingsManager>(() => new SettingsManager());
        private static readonly string settingsFilePath = Path.Combine(
            Environment.CurrentDirectory,
            "settings.json");

        // Custom converters for special types
        public class FontConverter : JsonConverter<Font>
        {
            public override Font ReadJson(JsonReader reader, Type objectType, Font existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var jo = JObject.Load(reader);
                string fontFamily = jo["FontFamily"]?.Value<string>() ?? "Microsoft Sans Serif";
                float fontSize = jo["Size"]?.Value<float>() ?? 11.25f;
                return new Font(fontFamily, fontSize);
            }

            public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("FontFamily");
                writer.WriteValue(value.FontFamily.Name);
                writer.WritePropertyName("Size");
                writer.WriteValue(value.Size);
                writer.WriteEndObject();
            }
        }

        public class ColorConverter : JsonConverter<Color>
        {
            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var jo = JObject.Load(reader);
                int a = jo["A"]?.Value<int>() ?? 255;
                int r = jo["R"]?.Value<int>() ?? 0;
                int g = jo["G"]?.Value<int>() ?? 0;
                int b = jo["B"]?.Value<int>() ?? 0;
                return Color.FromArgb(a, r, g, b);
            }

            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("A");
                writer.WriteValue(value.A);
                writer.WritePropertyName("R");
                writer.WriteValue(value.R);
                writer.WritePropertyName("G");
                writer.WriteValue(value.G);
                writer.WritePropertyName("B");
                writer.WriteValue(value.B);
                writer.WriteEndObject();
            }
        }

        [JsonConverter(typeof(FontConverter))]
        public Font FontStyle { get; set; } = new Font("Microsoft Sans Serif", 11.25f);
        [JsonConverter(typeof(FontConverter))]
        public Font BigFontStyle { get; set; } = new Font("Microsoft Sans Serif", 14f);
        [JsonConverter(typeof(ColorConverter))]
        public Color FontColor { get; set; } = Color.White;
        [JsonConverter(typeof(ColorConverter))]
        public Color ComboBoxColor { get; set; } = Color.FromArgb(25, 25, 25);
        [JsonConverter(typeof(ColorConverter))]
        public Color SubButtonColor { get; set; } = Color.FromArgb(25, 25, 25);
        [JsonConverter(typeof(ColorConverter))]
        public Color TextBoxColor { get; set; } = Color.FromArgb(25, 25, 25);
        [JsonConverter(typeof(ColorConverter))]
        public Color ButtonColor { get; set; } = Color.Black;
        [JsonConverter(typeof(ColorConverter))]
        public Color BackColor { get; set; } = Color.FromArgb(1, 1, 1);
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
        public string[] FavoritedGames { get; set; } = new string[0];
        public bool useProxy { get; set; } = false;
        public string ProxyAddress { get; set; } = string.Empty;
        public string ProxyPort { get; set; } = string.Empty;
        public string selectedMirror { get; set; } = string.Empty;
        public bool TrailersEnabled { get; set; } = true;
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
        public string[] QueuedGames { get; set; } = new string[0];

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
            FontStyle = new Font("Microsoft Sans Serif", 11.25f);
            BigFontStyle = new Font("Microsoft Sans Serif", 14f);
            FontColor = Color.White;
            ComboBoxColor = Color.FromArgb(25, 25, 25);
            SubButtonColor = Color.FromArgb(25, 25, 25);
            TextBoxColor = Color.FromArgb(25, 25, 25);
            ButtonColor = Color.Black;
            BackColor = Color.FromArgb(1, 1, 1);
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
            FavoritedGames = new string[0];
            useProxy = false;
            ProxyAddress = string.Empty;
            ProxyPort = string.Empty;
            selectedMirror = string.Empty;
            TrailersEnabled = true;
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
            QueuedGames = new string[0];

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
            FontStyle?.Dispose();
            BigFontStyle?.Dispose();
        }
    }
}