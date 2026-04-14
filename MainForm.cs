using AndroidSideloader.Models;
using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using SergeUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class MainForm : Form
    {
        public static string repo = "nerdunit/androidsideloader"; // GitHub repo
        public static string repo_branch = "master"; // GitHub branch
#if DEBUG
        public static bool debugMode = true;
        public bool DeviceConnected;
        public bool keyheld;
        public bool keyheld2;
        public static string CurrAPK;
        public static string CurrPCKG;
        List<UploadGame> gamesToUpload = new List<UploadGame>();
        public static string currremotesimple = String.Empty;
#else
        public bool keyheld;
        public static string CurrAPK;
        public static string CurrPCKG;
        private readonly List<UploadGame> gamesToUpload = new List<UploadGame>();
        public static bool debugMode = false;
        public bool DeviceConnected = false;
        public static string currremotesimple = "";
#endif
        private readonly ListViewColumnSorter lvwColumnSorter;
        private static readonly SettingsManager settings = SettingsManager.Instance;
        private double _totalQueueSizeMB = 0;
        private double _effectiveQueueSizeMB = 0;
        private Dictionary<string, double> _queueEffectiveSizes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        private long _deviceFreeSpaceMB = 0;
        // Shared sort state between Gallery and List views
        private SortField _sharedSortField = SortField.Name;
        private SortDirection _sharedSortDirection = SortDirection.Ascending;
        private const int BottomMargin = 8;
        private const int RightMargin = 12;
        private const int PanelSpacing = 10;
        private const int BottomPanelHeight = 217;
        private const int ChildTopMargin = 10;
        private const int ChildHorizontalPadding = 12;   // default left/right
        private const int NotesLeftMargin = 6;           // special left margin for notes
        private const int ChildRightMargin = 12;
        private const int LabelHeight = 20;
        private const int LabelBottomOffset = 4;         // space from label bottom to panel bottom
        private const int ReservedLabelHeight = 25;
        private Task _adbInitTask;
        public static readonly Color ColorInstalled = ColorTranslator.FromHtml("#3c91e6");
        public static readonly Color ColorUpdateAvailable = ColorTranslator.FromHtml("#4daa57");
        public static readonly Color ColorDonateGame = ColorTranslator.FromHtml("#cb9cf2");
        private static readonly Color ColorError = ColorTranslator.FromHtml("#f52f57");
        public static readonly Color ColorDownloaded = ColorTranslator.FromHtml("#67c7b1");
        private bool downloadedFilter_Clicked = false;
        private Panel _listViewUninstallButton;
        private bool _listViewUninstallButtonHovered = false;
        private ListViewItem _hoveredItemForDeleteBtn;
        private bool _folderIconHovered = false;
        private bool isGalleryView;  // Will be set from settings in constructor
        private List<ListViewItem> _galleryDataSource;
        private FastGalleryPanel _fastGallery;
        private const int TILE_WIDTH = 180;
        private const int TILE_HEIGHT = 125;
        private const int TILE_SPACING = 10;
        private string freeSpaceText = "";
        private string freeSpaceTextDetailed = "";
        private int _questStorageProgress = 0;
        private Color _mirrorPillColor = Color.FromArgb(32, 36, 44);
        private DateTime _mirrorMenuClosedAt = DateTime.MinValue;
        private Color _devicePillColor = Color.FromArgb(32, 36, 44);
        private DateTime _deviceMenuClosedAt = DateTime.MinValue;
        private bool _trailerPlayerInitialized;          // player.html created and loaded
        private bool _trailerHtmlLoaded;                 // initial navigation completed
        private static readonly Dictionary<string, string> _videoIdCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // per game cache
        private bool isLoading = true;
        private bool _suppressMirrorRefresh = false;
        public static bool _isManualMirrorSwitch = false;
        public static bool isOffline = false;
        public static bool noRcloneUpdating;
        public static bool noAppCheck = false;
        public static bool hasPublicConfig = false;
        public static bool hasUploadConfig = false;
        public static bool UsingPublicConfig = false;
        public static bool enviromentCreated = false;
        public static PublicConfig PublicConfigFile;
        public static string PublicMirrorExtraArgs = " --tpslimit 1.0 --tpslimit-burst 3";
        public static string storedIpPath;
        public static string aaptPath;
        private System.Windows.Forms.Timer _debounceTimer;
        private CancellationTokenSource _cts;
        private List<ListViewItem> _allItems;
        private Dictionary<string, List<ListViewItem>> _searchIndex;

        public MainForm()
        {
            storedIpPath = Path.Combine(Environment.CurrentDirectory, "platform-tools", "StoredIP.txt");
            aaptPath = Path.Combine(Environment.CurrentDirectory, "platform-tools", "aapt.exe");
            InitializeComponent();
            this.Opacity = 0;
            InitializeModernPanels(); // Initialize modern rounded panels for notes and queue
            // Center the initial placeholder text in notes panel
            notesRichTextBox.SelectAll();
            notesRichTextBox.SelectionAlignment = HorizontalAlignment.Center;
            notesRichTextBox.DeselectAll();
            Logger.Initialize();
            InitializeTimeReferences();
            CheckCommandLineArguments();

            // Use same icon as the executable
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Load user's preferred view from settings
            isGalleryView = settings.UseGalleryView;

            // Always start with ListView visible so selections work properly
            // We'll switch to gallery view after initListView completes if needed
            gamesListView.Visible = true;
            gamesGalleryView.Visible = false;
            btnViewToggle.Text = isGalleryView ? "LIST" : "GALLERY";

            favoriteGame.Renderer = new CenteredMenuRenderer();

            // Set initial wireless ADB button text based on current state
            UpdateWirelessADBButtonText();

            _debounceTimer = new System.Windows.Forms.Timer { Interval = 100, Enabled = false };
            _debounceTimer.Tick += async (sender, e) => await RunSearch();

            SetCurrentLogPath();
            StartTimers();

            lvwColumnSorter = new ListViewColumnSorter();
            gamesListView.ListViewItemSorter = lvwColumnSorter;

            // Initialize modern ListView renderer
            _listViewRenderer = new ModernListView(gamesListView, lvwColumnSorter);

            // Set a larger item height for increased spacing between rows
            ImageList rowSpacingImageList = new ImageList();
            rowSpacingImageList.ImageSize = new Size(1, 28);
            gamesListView.SmallImageList = rowSpacingImageList;

            SubscribeToHoverEvents(questInfoPanel);

            this.Resize += MainForm_Resize;

            // Style the downloads folder icon and overlay it on the left side of btnDownloaded
            openDownloadsFolderIcon.Paint += OpenDownloadsFolderIcon_Paint;
            openDownloadsFolderIcon.MouseEnter += (s, ev) => { _folderIconHovered = true; openDownloadsFolderIcon.Invalidate(); };
            openDownloadsFolderIcon.MouseLeave += (s, ev) => { _folderIconHovered = false; openDownloadsFolderIcon.Invalidate(); };
            openDownloadsFolderIcon.Parent = btnDownloaded;
            openDownloadsFolderIcon.Location = new Point(0, 0);
            openDownloadsFolderIcon.Size = new Size(28, 28);
            openDownloadsFolderIcon.BringToFront();
            btnDownloaded.TextXOffset = 24;

            // Create an uninstall button overlay for list view
            _listViewUninstallButton = new Panel
            {
                Size = new Size(22, 22),
                BackColor = Color.Transparent,
                Visible = false,
                Cursor = Cursors.Hand
            };
            _listViewUninstallButton.Paint += ListViewUninstallButton_Paint;
            _listViewUninstallButton.MouseEnter += (s, ev) => { _listViewUninstallButtonHovered = true; _listViewUninstallButton.Invalidate(); };
            _listViewUninstallButton.MouseLeave += (s, ev) => { _listViewUninstallButtonHovered = false; _listViewUninstallButton.Invalidate(); };
            _listViewUninstallButton.Click += ListViewUninstallButton_Click;
            gamesListView.Controls.Add(_listViewUninstallButton);

            // Timer to keep button position synced with the selected item
            var uninstallButtonTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60fps
            uninstallButtonTimer.Tick += (s, ev) =>
            {
                if (_listViewUninstallButton == null)
                    return;

                var item = _hoveredItemForDeleteBtn;

                // Hide if no item is hovered
                if (item == null || !gamesListView.Items.Contains(item))
                {
                    _listViewUninstallButton.Visible = false;
                    return;
                }

                // Check if item is installed
                bool isInstalled = item.ForeColor.ToArgb() == ColorInstalled.ToArgb() ||
                                   item.ForeColor.ToArgb() == ColorUpdateAvailable.ToArgb() ||
                                   item.ForeColor.ToArgb() == ColorDonateGame.ToArgb();

                if (!isInstalled)
                {
                    _listViewUninstallButton.Visible = false;
                    return;
                }

                // Calculate header height (items start below the header)
                int headerHeight = 0;
                if (gamesListView.View == View.Details && gamesListView.HeaderStyle != ColumnHeaderStyle.None)
                {
                    headerHeight = gamesListView.Font.Height;
                }

                // Calculate button position based on item bounds
                Rectangle itemBounds = item.Bounds;
                int buttonX = gamesListView.ClientSize.Width - _listViewUninstallButton.Width - 5;
                int buttonY = itemBounds.Top + (itemBounds.Height - _listViewUninstallButton.Height) / 2;

                // Check if item is within visible bounds (below header and above bottom)
                bool isVisible = itemBounds.Top >= headerHeight &&
                                 buttonY >= headerHeight &&
                                 buttonY + _listViewUninstallButton.Height <= gamesListView.ClientSize.Height;

                if (isVisible)
                {
                    _listViewUninstallButton.Location = new Point(buttonX, buttonY);
                    _listViewUninstallButton.Tag = item; // Store reference for click handler
                    if (!_listViewUninstallButton.Visible)
                    {
                        _listViewUninstallButton.Visible = true;
                    }
                }
                else
                {
                    _listViewUninstallButton.Visible = false;
                }
            };
            uninstallButtonTimer.Start();

            gamesListView.MouseMove += (s, ev) =>
            {
                var hitTest = gamesListView.HitTest(ev.Location);
                _hoveredItemForDeleteBtn = hitTest.Item;
            };

            gamesListView.MouseLeave += (s, ev) =>
            {
                // Clear hover if mouse left the ListView bounds
                Point clientPoint = gamesListView.PointToClient(Control.MousePosition);
                if (!gamesListView.ClientRectangle.Contains(clientPoint))
                {
                    _hoveredItemForDeleteBtn = null;
                }
            };

            // Set data that apparently can't be set in designer
            // We do it here so it doesn't get overwritten by designer
            batteryLevImg.Parent = questStorageProgressBar;
            batteryLabel.Parent = batteryLevImg;
            diskLabel.Parent = questStorageProgressBar;
            questInfoLabel.Parent = questStorageProgressBar;

            // Subscribe to click events to unfocus search text box
            this.Click += UnfocusSearchTextBox;

            // Load saved window state
            LoadWindowState();
        }

        private void CheckCommandLineArguments()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg == "--offline")
                {
                    isOffline = true;
                }
                if (arg == "--no-rclone-update")
                {
                    noRcloneUpdating = true;
                }
                if (arg == "--disable-app-check")
                {
                    noAppCheck = true;
                }
            }
        }

        private void InitializeTimeReferences()
        {
            // Initialize time references
            TimeSpan newDayReference = new TimeSpan(96, 0, 0); // Time between asking for new apps if user clicks No. (DEFAULT: 96 hours)
            TimeSpan newDayReference2 = new TimeSpan(72, 0, 0); // Time between asking for updates after uploading. (DEFAULT: 72 hours)

            // Calculate time differences
            DateTime A = settings.LastLaunch;
            DateTime B = DateTime.Now;
            DateTime C = settings.LastLaunch2;
            TimeSpan comparison = B - A;
            TimeSpan comparison2 = B - C;

            // Reset properties if enough time has passed
            if (comparison > newDayReference)
            {
                ResetPropertiesAfterTimePassed();
            }
            if (comparison2 > newDayReference2)
            {
                ResetProperties2AfterTimePassed();
            }
        }

        private void ResetPropertiesAfterTimePassed()
        {
            settings.ListUpped = false;
            settings.NonAppPackages = String.Empty;
            settings.AppPackages = String.Empty;
            settings.LastLaunch = DateTime.Now;
            settings.Save();
        }

        private void ResetProperties2AfterTimePassed()
        {
            settings.LastLaunch2 = DateTime.Now;
            settings.SubmittedUpdates = String.Empty;
            settings.Save();
        }

        private void SetCurrentLogPath()
        {
            if (string.IsNullOrEmpty(settings.CurrentLogPath))
            {
                settings.CurrentLogPath = Path.Combine(Environment.CurrentDirectory, "debuglog.txt");
            }
        }

        private void StartTimers()
        {
            // Start timers
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer
            {
                Interval = 840000 // 14 mins between wakeup commands
            };
            t.Tick += new EventHandler(timer_Tick);
            t.Start();

            System.Windows.Forms.Timer t2 = new System.Windows.Forms.Timer
            {
                Interval = 300 // 300ms
            };
            t2.Tick += new EventHandler(timer_Tick2);
            t2.Start();

            // Device connection check timer, runs every second
            System.Windows.Forms.Timer deviceCheckTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // 1 second
            };
            deviceCheckTimer.Tick += new EventHandler(timer_DeviceCheck);
            deviceCheckTimer.Start();
        }

        public static string donorApps = String.Empty;
        private string oldTitle = String.Empty;
        public static bool updatesNotified = false;
        public static string backupFolder;

        private static void KillAdbProcesses()
        {
            try
            {
                foreach (var p in Process.GetProcessesByName("adb"))
                {
                    try
                    {
                        if (!p.HasExited)
                        {
                            p.Kill();
                            p.WaitForExit(3000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to kill adb process (PID {p.Id}): {ex.Message}", LogLevel.WARNING);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error enumerating adb processes: {ex.Message}", LogLevel.WARNING);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _ = Logger.Log("Starting AndroidSideloader Application");

            // Hard kill any lingering adb.exe instances to avoid port/handle conflicts
            KillAdbProcesses();

            // ADB initialization in background
            _adbInitTask = Task.Run(() =>
            {
                _ = Logger.Log("Attempting to Initialize ADB Server");
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, "platform-tools", "adb.exe")))
                {
                    _ = ADB.RunAdbCommandToString("start-server");
                }
            });

            // Basic UI setup - only center if no saved position
            if (this.StartPosition != FormStartPosition.Manual)
            {
                CenterToScreen();
            }
            gamesListView.View = View.Details;
            gamesListView.FullRowSelect = true;
            gamesListView.GridLines = false;
            speedLabel.Text = String.Empty;
            diskLabel.Text = String.Empty;

            settings.MainDir = Environment.CurrentDirectory;
            settings.Save();

            changeTitle(isOffline ? "Starting in Offline Mode..." : "Initializing...");

            // Non-blocking WebView cleanup
            _ = Task.Run(() =>
            {
                try
                {
                    string webViewDirectoryPath = Path.Combine(Environment.CurrentDirectory, "WebView2Cache");
                    if (Directory.Exists(webViewDirectoryPath))
                    {
                        FileSystemUtilities.TryDeleteDirectory(webViewDirectoryPath);
                    }
                }
                catch { }
            });

            // Non-blocking background cleanup
            _ = Task.Run(() =>
            {
                try
                {
                    if (Directory.Exists(Sideloader.TempFolder))
                    {
                        FileSystemUtilities.TryDeleteDirectory(Sideloader.TempFolder);
                        _ = Directory.CreateDirectory(Sideloader.TempFolder);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error cleaning temp folder: {ex.Message}", LogLevel.WARNING);
                }
            });

            // Non-blocking log file cleanup
            _ = Task.Run(() =>
            {
                try
                {
                    string logFilePath = settings.CurrentLogPath;
                    if (File.Exists(logFilePath))
                    {
                        FileInfo fileInfo = new FileInfo(logFilePath);
                        if (fileInfo.Length > 5 * 1024 * 1024)
                        {
                            File.Delete(logFilePath);
                        }
                    }
                }
                catch { }
            });

            // Ensure bottom panels are properly laid out
            LayoutBottomPanels();

            webView21.Visible = settings.TrailersEnabled;

            // Continue with Form1_Shown
            this.Form1_Shown(sender, e);
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            //searchTextBox.Enabled = false;

            // Disclaimer thread
            new Thread(() =>
            {
                Thread.Sleep(5000);
                freeDisclaimer.Invoke(() =>
                {
                    freeDisclaimer.Dispose();
                    freeDisclaimer.Enabled = false;
                });
            }).Start();

            // Startup dialog:
            // The dialog lets the user choose online (with a config URL) or offline mode.
            // If a valid public.json exists and the server is reachable,
            // skip the dialog entirely and go straight to online mode.
            // The --offline flag bypasses the dialog entirely.
            if (!isOffline)
            {
                // Try to auto-load existing config and skip the dialog
                bool configAutoLoaded = false;
                try
                {
                    string configFilePath = Path.Combine(Environment.CurrentDirectory, "public.json");
                    if (File.Exists(configFilePath))
                    {
                        string configFileData = File.ReadAllText(configFilePath);
                        PublicConfig config = JsonConvert.DeserializeObject<PublicConfig>(configFileData);

                        if (config != null &&
                            !string.IsNullOrWhiteSpace(config.BaseUri) &&
                            !string.IsNullOrWhiteSpace(config.Password))
                        {
                            // Config file is structurally valid — test if server is reachable
                            string hostname = null;
                            string baseUri = config.BaseUri;
                            if (!baseUri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                                !baseUri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            {
                                baseUri = "https://" + baseUri;
                            }
                            if (Uri.TryCreate(baseUri, UriKind.Absolute, out Uri uri))
                            {
                                hostname = uri.Host;
                            }

                            if (!string.IsNullOrEmpty(hostname))
                            {
                                bool serverReachable = false;
                                try
                                {
                                    var addresses = Dns.GetHostAddresses(hostname);
                                    serverReachable = addresses != null && addresses.Length > 0;
                                }
                                catch
                                {
                                    serverReachable = false;
                                }

                                if (serverReachable)
                                {
                                    PublicConfigFile = config;
                                    hasPublicConfig = true;
                                    configAutoLoaded = true;
                                    DnsHelper.TestPublicConfigDns();
                                    Logger.Log($"Auto-loaded valid config — server {hostname} is reachable, skipping startup dialog");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Auto-load config check failed: {ex.Message}", LogLevel.WARNING);
                }

                // If public.json auto-load failed, check for rclone/download.config with mirrors
                if (!configAutoLoaded)
                {
                    try
                    {
                        string downloadConfigFile = Path.Combine(Environment.CurrentDirectory, "rclone", "download.config");
                        if (File.Exists(downloadConfigFile))
                        {
                            string configText = File.ReadAllText(downloadConfigFile);
                            if (Regex.IsMatch(configText, @"\[.*mirror.*\]", RegexOptions.IgnoreCase))
                            {
                                configAutoLoaded = true;
                                Logger.Log("Found rclone/download.config with mirror remotes, skipping startup dialog");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Rclone config check failed: {ex.Message}", LogLevel.WARNING);
                    }
                }

                // If auto-load failed, show the startup dialog as usual
                if (!configAutoLoaded)
                {
                    using (var startupDialog = new StartupDialog())
                    {
                        var dialogResult = startupDialog.ShowDialog(this);

                        if (dialogResult != DialogResult.OK || startupDialog.Choice == StartupDialog.StartupChoice.None)
                        {
                            Application.Exit();
                            return;
                        }

                        if (startupDialog.Choice == StartupDialog.StartupChoice.Offline)
                        {
                            isOffline = true;
                            Logger.Log("User chose offline mode from startup dialog");
                        }
                        else if (startupDialog.Choice == StartupDialog.StartupChoice.RcloneConfig)
                        {
                            // User provided an rclone download config — proceed online without public config
                            Logger.Log("User provided rclone download.config from startup dialog");
                        }
                        else
                        {
                            // Online — the StartupDialog already validated, downloaded, and wrote public.json
                            // Load it into memory
                            Logger.Log("User chose online mode from startup dialog");
                            try
                            {
                                string configFilePath = Path.Combine(Environment.CurrentDirectory, "public.json");
                                if (File.Exists(configFilePath))
                                {
                                    string configFileData = File.ReadAllText(configFilePath);
                                    PublicConfig config = JsonConvert.DeserializeObject<PublicConfig>(configFileData);

                                    if (config != null &&
                                        !string.IsNullOrWhiteSpace(config.BaseUri) &&
                                        !string.IsNullOrWhiteSpace(config.Password))
                                    {
                                        PublicConfigFile = config;
                                        hasPublicConfig = true;

                                        // Test DNS for the public config hostname
                                        DnsHelper.TestPublicConfigDns();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"Failed to load config after startup dialog: {ex.Message}", LogLevel.ERROR);
                            }

                            if (!hasPublicConfig)
                            {
                                // Should rarely happen since the dialog validated it,
                                // but handle gracefully.
                                isOffline = true;
                                Logger.Log("Config invalid after dialog — falling back to offline mode", LogLevel.WARNING);
                            }
                        }
                    }
                }

                // Pre-initialize trailer player in background
                if (!isOffline)
                {
                    try
                    {
                        await EnsureTrailerEnvironmentAsync();
                    }
                    catch { /* swallow – prewarm should never crash startup */ }
                }
            }

            // Show the main form now that startup dialog is done
            this.Opacity = 1;

            // UI setup
            remotesList.Items.Clear();
            if (hasPublicConfig)
            {
                UsingPublicConfig = true;
                _ = Logger.Log($"Using Public Mirror");
            }
            if (isOffline)
            {
                remotesList.Size = System.Drawing.Size.Empty;
                _ = Logger.Log($"Using Offline Mode");
            }
            if (settings.NodeviceMode)
            {
                btnNoDevice.Text = "ENABLE SIDELOADING";
            }

            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Loading";

            // Always download dependencies (adb, aapt, 7z, runtimes) regardless of mode
            await Task.Run(() =>
            {
                changeTitle("Downloading Dependencies...");
                GetDependencies.downloadFiles();
            });

            // Init RCLONE only in online mode
            if (!isOffline)
            {
                await Task.Run(() =>
                {
                    changeTitle("Initializing RCLONE...");
                    RCLONE.Init();
                });
            }

            // Update check
            if (!debugMode && settings.CheckForUpdates && !isOffline)
            {
                Updater.AppName = "AndroidSideloader";
                Updater.Repository = repo;
                await Updater.Update();
            }

            if (!isOffline)
            {
                _ = Logger.Log("Initializing Servers");
                changeTitle("Initializing Servers...");

                await initMirrors();
            }
            else
            {
                changeTitle("Offline mode // Scanning local library...");

                // Determine the scan directory; prompt the user on first run
                // Self-healing: if DownloadDir is set but CustomDownloadDir was lost, recover
                bool hasValidDir = !string.IsNullOrEmpty(settings.DownloadDir) && Directory.Exists(settings.DownloadDir);
                if (hasValidDir && !settings.CustomDownloadDir)
                {
                    settings.CustomDownloadDir = true;
                    settings.Save();
                }

                string dlDir = hasValidDir ? settings.DownloadDir : Environment.CurrentDirectory;

                if (!settings.CustomDownloadDir)
                {
                    var folderDialog = new FolderSelectDialog
                    {
                        Title = "Select the folder containing your downloaded games",
                        InitialDirectory = dlDir
                    };

                    if (folderDialog.Show(Handle))
                    {
                        dlDir = folderDialog.FileName;
                        settings.DownloadDir = dlDir;
                        settings.CustomDownloadDir = true;
                        settings.Save();
                    }
                }

                // In offline mode, scan the local download directory for games
                await Task.Run(() =>
                {
                    SideloaderRCLONE.ScanLocalGames(dlDir);
                });
            }

            // Device connection and Metadata can run simultaneously
            Task metadataTask = null;
            Task deviceConnectionTask = null;

            // Start device connection task
            deviceConnectionTask = Task.Run(() =>
            {
                changeTitle("Connecting to device...");
                if (!string.IsNullOrEmpty(settings.IPAddress))
                {
                    string path = Path.Combine(Environment.CurrentDirectory, "platform-tools", "adb.exe");
                    ProcessOutput wakeywakey = ADB.RunCommandToString($"\"{path}\" shell input keyevent KEYCODE_WAKEUP", path);
                    if (wakeywakey.Output.Contains("more than one"))
                    {
                        settings.Wired = true;
                        settings.Save();
                    }
                    else if (wakeywakey.Output.Contains("found"))
                    {
                        settings.Wired = false;
                        settings.Save();
                    }
                }

                if (File.Exists(storedIpPath) && !settings.Wired)
                {
                    string IPcmndfromtxt = File.ReadAllText(storedIpPath);
                    settings.IPAddress = IPcmndfromtxt;
                    settings.Save();
                    ProcessOutput IPoutput = ADB.RunAdbCommandToString(IPcmndfromtxt);
                    if (IPoutput.Output.Contains("attempt failed") || IPoutput.Output.Contains("refused"))
                    {
                        this.Invoke(() =>
                        {
                            _ = FlexibleMessageBox.Show(Program.form,
                                "Attempt to connect to saved IP has failed. This is usually due to rebooting the device or not having a STATIC IP set in your router.\nYou must enable Wireless ADB again!");
                        });
                        settings.IPAddress = "";
                        settings.Save();
                        try { File.Delete(storedIpPath); }
                        catch (Exception ex) { Logger.Log($"Unable to delete StoredIP.txt due to {ex.Message}", LogLevel.ERROR); }
                    }
                    else
                    {
                        _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                        _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                    }
                }
                else if (!File.Exists(storedIpPath))
                {
                    settings.IPAddress = "";
                    settings.Save();
                }
            });

            // Start metadata task in parallel
            if (!isOffline)
            {
                metadataTask = Task.Run(() =>
                {
                    if (hasPublicConfig)
                    {
                        changeTitle("Updating Metadata...");
                        SideloaderRCLONE.UpdateMetadataFromPublic();

                        changeTitle("Processing Metadata...");
                        SideloaderRCLONE.ProcessMetadataFromPublic();
                    }

                    if (!UsingPublicConfig)
                    {
                        changeTitle("Updating Game Notes...");
                        SideloaderRCLONE.UpdateGameNotes(currentRemote);

                        changeTitle("Updating Game Thumbnails...");
                        SideloaderRCLONE.UpdateGamePhotos(currentRemote);

                        SideloaderRCLONE.UpdateNouns(currentRemote);

                        if (!Directory.Exists(SideloaderRCLONE.ThumbnailsFolder) ||
                            !Directory.Exists(SideloaderRCLONE.NotesFolder))
                        {
                            this.Invoke(() =>
                            {
                                _ = FlexibleMessageBox.Show(Program.form,
                                    "It seems you are missing the thumbnails and/or notes database, the first start of the sideloader takes a bit more time, so dont worry if it looks stuck!");
                            });
                        }
                    }
                });
            }

            // Wait for both tasks to complete
            var tasksToWait = new List<Task>();
            if (deviceConnectionTask != null) tasksToWait.Add(deviceConnectionTask);
            if (metadataTask != null) tasksToWait.Add(metadataTask);

            if (tasksToWait.Count > 0)
            {
                await Task.WhenAll(tasksToWait);
            }

            // For non-Public mirrors, load the game list AFTER metadata processing
            // completes. ProcessMetadataFromPublic() overwrites the game list with
            // public data, so initGames must run afterwards to get the final word.
            if (!isOffline && !UsingPublicConfig)
            {
                changeTitle("Grabbing the Games List...");
                await Task.Run(() => SideloaderRCLONE.initGames(currentRemote));
            }

            string uploadConfigPath = Path.Combine(Environment.CurrentDirectory, "rclone", "upload.config");
            if (File.Exists(uploadConfigPath))
                hasUploadConfig = true;

            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Loading";
            changeTitle("Populating Game List...");

            _ = await CheckForDevice();
            if (ADB.DeviceID.Length < 5)
            {
                nodeviceonstart = true;
            }

            // Parallel execution
            await Task.WhenAll(
                Task.Run(() => listAppsBtn())
            );

            isLoading = false;

            // Initialize list view
            initListView(false);

            // Cleanup in background
            _ = Task.Run(() =>
            {
                string[] files = Directory.GetFiles(Environment.CurrentDirectory);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (!fileName.Contains(settings.CurrentLogName) &&
                        !fileName.Contains(settings.CurrentCrashName) &&
                        !fileName.Contains("debuglog") &&
                        fileName.EndsWith(".txt"))
                    {
                        try { System.IO.File.Delete(file); } catch { }
                    }
                }
            });

            searchTextBox.Enabled = true;

            if (isOffline)
            {
                remotesList.Size = System.Drawing.Size.Empty;
                _ = Logger.Log($"Using Offline Mode");
            }

            changeTitlebarToDevice();
            UpdateStatusLabels();

            // Load saved download queue and offer to resume
            LoadQueueFromSettings();
            if (gamesQueueList.Count > 0 && !isOffline)
            {
                await ResumeQueuedDownloadsAsync();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _ = ADB.RunAdbCommandToString("shell input keyevent KEYCODE_WAKEUP");
        }

        private void timer_Tick2(object sender, EventArgs e)
        {
            keyheld = false;
        }

        public async void changeTitle(string txt, bool reset = false)
        {
            try
            {
                string titleSuffix = string.IsNullOrWhiteSpace(txt) ? "" : " | " + txt;
                this.Invoke(() =>
                {
                    Text = "Rookie Sideloader " + Updater.LocalVersion + titleSuffix;
                    rookieStatusLabel.Text = txt;
                });

                if (!reset)
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
                // Reset to base title without any status message
                this.Invoke(() =>
                {
                    Text = "Rookie Sideloader " + Updater.LocalVersion;
                    rookieStatusLabel.Text = "";
                });
            }
            catch
            {
            }
        }

        private async void startsideloadbutton_Click(object sender, EventArgs e)
        {
            ProcessOutput output = new ProcessOutput("", "");
            string path = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Android apps (*.apk)|*.apk";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }
            ADB.DeviceID = GetDeviceID();

            string filename = Path.GetFileName(path);
            changeTitle($"Installing {filename}...");
            progressBar.IsIndeterminate = false;
            progressBar.OperationType = "Installing";
            progressBar.Value = 0;
            progressBar.StatusText = "Preparing...";

            output = await ADB.SideloadWithProgressAsync(
                path,
                (percent, eta) => this.Invoke(() =>
                {
                    if (percent == 0)
                    {
                        progressBar.IsIndeterminate = true;
                        progressBar.OperationType = "Installing";
                    }
                    else
                    {
                        progressBar.IsIndeterminate = false;
                        progressBar.Value = percent;
                    }
                    UpdateProgressStatus("Installing", percent: (int)Math.Round(percent), eta: eta);
                    progressBar.StatusText = $"Installing · {percent:0.0}%";
                }),
                status => this.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        if (status.Contains("Completing Installation"))
                        {
                            speedLabel.Text = status;
                        }
                        progressBar.StatusText = status;
                    }
                }),
                "",
                filename);

            // Reset UI on completion
            progressBar.Value = 0;
            progressBar.StatusText = "";
            progressBar.IsIndeterminate = false;
            speedLabel.Text = "";
            changeTitle("");

            showAvailableSpace();
            ShowPrcOutput(output);
        }

        public void ShowPrcOutput(ProcessOutput prcout)
        {
            string message = $"{prcout.Output}";
            if (prcout.Error.Length != 0)
            {
                message += $"\nError: {prcout.Error}";
            }
            _ = FlexibleMessageBox.Show(Program.form, message);
        }

        public List<string> Devices = new List<string>();

        public async Task<int> CheckForDevice()
        {
            Devices.Clear();
            ADB.DeviceID = GetDeviceID();
            string output = await Task.Run(() => ADB.RunAdbCommandToString("devices").Output); // Run off UI thread

            string[] line = output.Split('\n');
            int i = 0;

            devicesComboBox.Items.Clear();

            _ = Logger.Log("Devices:");
            foreach (string currLine in line)
            {
                if (i > 0 && currLine.Length > 0)
                {
                    string deviceId = currLine.Split('\t')[0];
                    Devices.Add(deviceId);
                    _ = devicesComboBox.Items.Add(deviceId);
                    _ = Logger.Log(deviceId + "\n", LogLevel.INFO, false);
                }
                Debug.WriteLine(currLine);
                i++;
            }

            if (devicesComboBox.Items.Count > 0)
            {
                devicesComboBox.SelectedIndex = 0;
                string battery = await Task.Run(() => ADB.RunAdbCommandToString("shell dumpsys battery").Output); // Run off UI thread
                battery = Utilities.StringUtilities.RemoveEverythingBeforeFirst(battery, "level:");
                battery = Utilities.StringUtilities.RemoveEverythingAfterFirst(battery, "\n");
                battery = Utilities.StringUtilities.KeepOnlyNumbers(battery);
                batteryLabel.Text = battery;
            }

            UpdateQuestInfoPanel();

            return devicesComboBox.SelectedIndex;
        }

        public async void devicesbutton_Click(object sender, EventArgs e)
        {
            _ = await CheckForDevice();

            changeTitlebarToDevice();
            showAvailableSpace();
        }

        private async void resetAdbAuthButton_Click(object sender, EventArgs e)
        {
            DialogResult confirm = FlexibleMessageBox.Show(this,
                "This will kill the ADB server, remove your ADB authorization keys, and restart.\n\n" +
                "After reset, you will need to approve the USB debugging prompt on your device.\n\nContinue?",
                "Reset ADB Authorization", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            string result = await Task.Run(() => ADB.ResetAdbAuthorization());

            FlexibleMessageBox.Show(this, result + "\n\nPlease reconnect your device and accept the USB debugging prompt.",
                "ADB Authorization Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);

            devicesbutton_Click(sender, e);
        }

        public static void notify(string message)
        {
            if (settings.EnableMessageBoxes)
            {
                _ = FlexibleMessageBox.Show(new Form
                {
                    TopMost = true,
                    StartPosition = FormStartPosition.CenterScreen
                }, message);
            }
        }

        private async void obbcopybutton_Click(object sender, EventArgs e)
        {
            ProcessOutput output = new ProcessOutput(String.Empty, String.Empty);
            FolderSelectDialog dialog = new FolderSelectDialog
            {
                Title = "Select OBB folder (must be direct OBB folder, E.G: com.Company.AppName)"
            };

            if (dialog.Show(Handle))
            {
                string path = dialog.FileName;
                string folderName = Path.GetFileName(path);

                changeTitle($"Copying {folderName} OBB to device...");
                progressBar.IsIndeterminate = false;
                progressBar.Value = 0;
                progressBar.OperationType = "Copying OBB";
                progressBar.StatusText = "Preparing...";

                string currentStatusBase = string.Empty;

                output = await ADB.CopyOBBWithProgressAsync(
                    path,
                    (percent, eta) => this.Invoke(() =>
                    {
                        progressBar.Value = percent;
                        UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                        if (!string.IsNullOrEmpty(currentStatusBase))
                        {
                            progressBar.StatusText = $"{currentStatusBase} · {percent:0.0}%";
                        }
                        else
                        {
                            progressBar.StatusText = $"Copying · {percent:0.0}%";
                        }
                    }),
                    status => this.Invoke(() =>
                    {
                        currentStatusBase = status ?? string.Empty;
                        if (!string.IsNullOrEmpty(status))
                        {
                            progressBar.StatusText = status;
                        }
                    }),
                    folderName);

                // Reset UI on completion
                progressBar.Value = 0;
                progressBar.StatusText = "";
                progressBar.IsIndeterminate = false;
                speedLabel.Text = "";
                changeTitle("");

                showAvailableSpace();
                ShowPrcOutput(output);
            }
        }

        public void changeTitlebarToDevice()
        {
            if (Devices.Contains("unauthorized"))
            {
                DeviceConnected = false;
                this.Invoke(() =>
                {
                    Text = "Rookie Sideloader " + Updater.LocalVersion + " | Device Not Authorized";
                    DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "Please check inside your headset for ADB DEBUGGING prompt/notification, check the box \"Always allow from this computer.\" and hit OK.", "Not Authorized", MessageBoxButtons.RetryCancel);
                    if (dialogResult == DialogResult.Retry)
                    {
                        devicesbutton.PerformClick();
                    }
                });
            }
            else if (Devices.Count > 0 && Devices[0].Length > 1) // Check if Devices list is not empty and the first device has a valid length
            {
                this.Invoke(() => { Text = "Rookie Sideloader " + Updater.LocalVersion + " | Device Connected: " + Devices[0].Replace("device", String.Empty).Trim(); });
                DeviceConnected = true;
                nodeviceonstart = false; // Device connected, clear the flag
            }
            else
            {
                this.Invoke(() =>
                {
                    DeviceConnected = false;
                    Text = "No Device Connected";
                    if (!settings.NodeviceMode)
                    {
                        // Explicitly update sideloading UI when the dialog is shown
                        UpdateSideloadingUI(true);

                        DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "No device found. Please ensure the following:\n\n - Developer mode is enabled\n - ADB drivers are installed\n - ADB connection is enabled on your device (this can reset)\n - Your device is plugged in\n\nThen press \"Retry\"", "No device found.", MessageBoxButtons.RetryCancel);
                        if (dialogResult == DialogResult.Retry)
                        {
                            devicesbutton.PerformClick();
                        }
                        else
                        {
                            return;
                        }
                    }
                    nodeviceonstart = true;
                    Text = "Rookie Sideloader " + Updater.LocalVersion + " | No Device (Download-Only Mode)";
                });
            }

            UpdateQuestInfoPanel();
            UpdateStatusLabels();
        }

        public async void showAvailableSpace()
        {
            string AvailableSpace = string.Empty;
            if (!settings.NodeviceMode || DeviceConnected)
            {
                try
                {
                    ADB.DeviceID = GetDeviceID();
                    Thread t1 = new Thread(() =>
                    {
                        AvailableSpace = ADB.GetAvailableSpace();
                    });
                    t1.Start();

                    while (t1.IsAlive)
                    {
                        await Task.Delay(100);
                    }

                    diskLabel.Invoke(() => { diskLabel.Text = AvailableSpace; });

                    UpdateQuestInfoPanel();
                }
                catch (Exception ex)
                {
                    _ = Logger.Log($"Unable to get available space with the exception: {ex}", LogLevel.ERROR);
                }
            }
        }

        public string GetDeviceID()
        {
            string deviceId = string.Empty;
            int index = -1;
            int itemCount = 0;

            devicesComboBox.Invoke(() =>
            {
                index = devicesComboBox.SelectedIndex;
                itemCount = devicesComboBox.Items.Count;
            });

            if (index != -1)
            {
                devicesComboBox.Invoke(() => { deviceId = devicesComboBox.SelectedItem.ToString(); });
            }
            else if (itemCount > 1)
            {
                // Multiple devices but none selected - prompt user
                deviceId = ShowDeviceSelector("Multiple devices detected - Select a device");
            }
            else if (itemCount == 1)
            {
                // Only one device, select it automatically
                devicesComboBox.Invoke(() =>
                {
                    devicesComboBox.SelectedIndex = 0;
                    deviceId = devicesComboBox.SelectedItem.ToString();
                });
            }

            return deviceId ?? string.Empty;
        }

        private async void backupadbbutton_Click(object sender, EventArgs e)
        {
            string selectedApp = ShowInstalledAppSelector("Select an app to backup with ADB");
            if (selectedApp == null) return;

            backupFolder = settings.GetEffectiveBackupDir();
            string date_str = "ab." + DateTime.Today.ToString("yyyy.MM.dd");
            string CurrBackups = Path.Combine(backupFolder, date_str);

            Directory.CreateDirectory(CurrBackups);

            string packageName = Sideloader.gameNameToPackageName(selectedApp);
            string backupFile = Path.Combine(CurrBackups, $"{packageName}.ab");

            FlexibleMessageBox.Show(Program.form,
                $"Backing up {selectedApp} to:\n{backupFile}\n\nClick OK, then on your Quest:\n1. Unlock device\n2. Click 'Back Up My Data'");

            changeTitle($"Backing up {selectedApp}...");
            progressBar.IsIndeterminate = true;

            var output = await Task.Run(() =>
                ADB.RunAdbCommandToString($"backup -f \"{backupFile}\" {packageName}")
            );

            progressBar.IsIndeterminate = false;
            changeTitle("");

            // Success = file exists, has content, no errors
            bool fileExists = File.Exists(backupFile);
            bool hasContent = fileExists && new FileInfo(backupFile).Length > 0;
            bool hasErrors = !string.IsNullOrEmpty(output.Error);

            if (hasContent && !hasErrors)
            {
                Logger.Log($"Successfully backed up {selectedApp} to {backupFile}", LogLevel.INFO);
                FlexibleMessageBox.Show(Program.form,
                    $"Backup successful!\n\nApp: {selectedApp}\nFile: {backupFile}\nSize: {new FileInfo(backupFile).Length / 1024} KB",
                    "Backup Complete");
            }
            else
            {
                // Cleanup failed backup file
                if (File.Exists(backupFile))
                    File.Delete(backupFile);

                string errorMsg = hasErrors ? output.Error : "No backup data created";
                Logger.Log($"Failed to backup {selectedApp}: {errorMsg}", LogLevel.ERROR);
                FlexibleMessageBox.Show(Program.form,
                    $"Backup failed!\n\nApp: {selectedApp}\nError: {errorMsg}",
                    "Backup Failed");
            }
        }

        private async void backupbutton_Click(object sender, EventArgs e)
        {
            backupFolder = settings.GetEffectiveBackupDir();
            string date_str = DateTime.Today.ToString("yyyy.MM.dd");
            string CurrBackups = Path.Combine(backupFolder, date_str);

            DialogResult dialogResult1 = FlexibleMessageBox.Show(Program.form,
                $"Do you want to attempt to backup all gamesaves to:\n{CurrBackups}\\",
                "Backup All Gamesaves",
                MessageBoxButtons.YesNo);

            if (dialogResult1 == DialogResult.No || dialogResult1 == DialogResult.Cancel) return;

            Directory.CreateDirectory(CurrBackups); // Create parent dir if needed

            changeTitle("Backing up gamesaves...");
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Backing Up";

            var successList = new List<string>();
            var failedList = new List<string>();
            int totalGames = 0;
            int processedGames = 0;

            await Task.Run(() =>
            {
                // Get all game folders in /sdcard/Android/data
                var listOutput = ADB.RunAdbCommandToString("shell ls -1 /sdcard/Android/data", suppressLogging: true);

                if (string.IsNullOrEmpty(listOutput.Output) || !string.IsNullOrEmpty(listOutput.Error))
                {
                    Logger.Log($"Failed to list game folders: {listOutput.Error}", LogLevel.ERROR);
                    return;
                }

                var gameFolders = listOutput.Output
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(f => !string.IsNullOrWhiteSpace(f) && f.Contains("."))
                    .Select(f => f.Trim())
                    .ToList();

                totalGames = gameFolders.Count;

                foreach (var gameFolder in gameFolders)
                {
                    processedGames++;
                    this.Invoke(() => changeTitle($"Backing up {gameFolder} ({processedGames}/{totalGames})..."));

                    string gamePath = $"/sdcard/Android/data/{gameFolder}";
                    string backupPath = Path.Combine(CurrBackups, gameFolder);

                    var pullOutput = ADB.RunAdbCommandToString($"pull \"{gamePath}\" \"{backupPath}\"", suppressLogging: true);

                    // Success = no errors and has content
                    bool hasContent = Directory.Exists(backupPath) && Directory.GetFileSystemEntries(backupPath).Length > 0;
                    bool hasErrors = !string.IsNullOrEmpty(pullOutput.Error);

                    if (hasContent && !hasErrors)
                    {
                        successList.Add(gameFolder);
                        Logger.Log($"Successfully backed up: {gameFolder}", LogLevel.INFO);
                    }
                    else if (hasErrors)
                    {
                        // Cleanup empty/failed directory
                        if (Directory.Exists(backupPath))
                            Directory.Delete(backupPath, true);

                        failedList.Add($"{gameFolder}: {pullOutput.Error.Split('\n')[0].Trim()}");
                        Logger.Log($"Failed to backup {gameFolder}: {pullOutput.Error}", LogLevel.WARNING);
                    }
                    else
                    {
                        // No content but no errors = app has no save data (not a failure)
                        if (Directory.Exists(backupPath))
                            Directory.Delete(backupPath, true);

                        Logger.Log($"No save data for: {gameFolder}", LogLevel.INFO);
                    }
                }
            });

            progressBar.IsIndeterminate = false;
            changeTitle("");

            // Build summary
            var summary = new StringBuilder();
            summary.AppendLine($"Backup completed to:\n{CurrBackups}\\\n");
            summary.AppendLine($"Successfully backed up: {successList.Count} games");

            if (failedList.Count > 0)
            {
                summary.AppendLine($"Failed to backup: {failedList.Count} games\n");
                summary.AppendLine("Failed games:");
                foreach (var failed in failedList)
                    summary.AppendLine($" • {failed}");
            }

            summary.AppendLine("\nNote: Some games may not support bulk backup as they require special permissions.");
            summary.AppendLine("In this case, use 'BACKUP GAMESAVE WITH ADB' to backup the game individually.");

            FlexibleMessageBox.Show(Program.form, summary.ToString(), "Bulk Backup Complete");
        }

        private async void restorebutton_Click(object sender, EventArgs e)
        {
            backupFolder = settings.GetEffectiveBackupDir();

            // Create restore method dialog
            string restoreMethod = null;
            using (Form dialog = new Form())
            {
                dialog.Text = "Restore Gamesaves";
                dialog.Size = new Size(340, 130);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = "Choose restore source:",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var btnFolder = CreateStyledButton("From Folder", DialogResult.None, new Point(15, 45));
                btnFolder.Size = new Size(145, 32);
                btnFolder.Click += (s, ev) => { restoreMethod = "folder"; dialog.DialogResult = DialogResult.OK; };

                var btnAbFile = CreateStyledButton("From .ab File", DialogResult.None, new Point(170, 45));
                btnAbFile.Size = new Size(145, 32);
                btnAbFile.Click += (s, ev) => { restoreMethod = "ab"; dialog.DialogResult = DialogResult.OK; };

                dialog.Controls.AddRange(new Control[] { label, btnFolder, btnAbFile });

                if (dialog.ShowDialog(this) != DialogResult.OK || restoreMethod == null) return;
            }

            // .ab file restore
            if (restoreMethod == "ab")
            {
                using (var fileDialog = new OpenFileDialog())
                {
                    fileDialog.Title = "Select Android Backup (.ab) file";
                    fileDialog.InitialDirectory = backupFolder;
                    fileDialog.Filter = "Android Backup Files (*.ab)|*.ab|All Files (*.*)|*.*";

                    if (fileDialog.ShowDialog() != DialogResult.OK) return;

                    Logger.Log($"Selected .ab file: {fileDialog.FileName}");
                    FlexibleMessageBox.Show(Program.form,
                        "Click OK, then on your Quest:\n1. Unlock device\n2. Confirm 'Restore My Data'");

                    var output = ADB.RunAdbCommandToString($"restore \"{fileDialog.FileName}\"");
                    FlexibleMessageBox.Show(Program.form,
                        string.IsNullOrEmpty(output.Error) ? "Restore completed" : $"Restore result:\n{output.Output}\n{output.Error}",
                        "Restore Complete");
                }
                return;
            }

            // Folder restore: find newest date folder to preselect
            string initialPath = backupFolder;
            if (Directory.Exists(backupFolder))
            {
                var newestDateFolder = Directory.GetDirectories(backupFolder)
                    .Select(d => new DirectoryInfo(d))
                    .Where(d => Regex.IsMatch(d.Name, @"^\d{4}\.\d{2}\.\d{2}$"))
                    .OrderByDescending(d => d.Name)
                    .FirstOrDefault();

                if (newestDateFolder != null)
                    initialPath = newestDateFolder.FullName;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a date folder (e.g., 2026.01.01) to restore ALL gamesaves,\nor a specific game folder (e.g., com.game.name) to restore just that game.";
                folderDialog.SelectedPath = initialPath;
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() != DialogResult.OK) return;

                string selectedFolder = folderDialog.SelectedPath;
                string folderName = Path.GetFileName(selectedFolder);
                Logger.Log($"Selected folder: {selectedFolder}");

                // Determine if this is a date folder or a single game folder
                bool isDateFolder = Regex.IsMatch(folderName, @"^\d{4}\.\d{2}\.\d{2}$");
                bool isGameFolder = folderName.Contains(".") && !isDateFolder;

                List<string> gameFoldersToRestore;

                if (isGameFolder)
                {
                    // Single game folder selected: restore just this one
                    gameFoldersToRestore = new List<string> { folderName };
                    // Parent folder becomes the source
                    selectedFolder = Path.GetDirectoryName(selectedFolder);
                }
                else
                {
                    // Date folder or other: get all game subfolders
                    gameFoldersToRestore = Directory.GetDirectories(selectedFolder)
                        .Select(Path.GetFileName)
                        .Where(f => !string.IsNullOrWhiteSpace(f) && f.Contains("."))
                        .ToList();
                }

                if (gameFoldersToRestore.Count == 0)
                {
                    FlexibleMessageBox.Show(Program.form, "No game folders found in the selected directory.", "Nothing to Restore");
                    return;
                }

                changeTitle("Restoring gamesaves...");
                progressBar.IsIndeterminate = true;
                progressBar.OperationType = "Restoring";

                var successList = new List<string>();
                var failedList = new List<string>();
                int totalGames = gameFoldersToRestore.Count;
                int processedGames = 0;

                await Task.Run(() =>
                {
                    foreach (var gameFolder in gameFoldersToRestore)
                    {
                        processedGames++;
                        this.Invoke(() => changeTitle($"Restoring {gameFolder} ({processedGames}/{totalGames})..."));

                        string sourcePath = Path.Combine(selectedFolder, gameFolder);
                        string targetPath = $"/sdcard/Android/data/{gameFolder}";

                        var pushOutput = ADB.RunAdbCommandToString($"push \"{sourcePath}\" \"{targetPath}\"", suppressLogging: true);

                        if (string.IsNullOrEmpty(pushOutput.Error))
                        {
                            successList.Add(gameFolder);
                            Logger.Log($"Successfully restored: {gameFolder}", LogLevel.INFO);
                        }
                        else
                        {
                            failedList.Add($"{gameFolder}: {pushOutput.Error.Split('\n')[0].Trim()}");
                            Logger.Log($"Failed to restore {gameFolder}: {pushOutput.Error}", LogLevel.WARNING);
                        }
                    }
                });

                progressBar.IsIndeterminate = false;
                changeTitle("");

                var summary = new StringBuilder();
                summary.AppendLine($"Restore completed from:\n{selectedFolder}\\\n");
                summary.AppendLine($"Successfully restored: {successList.Count} game(s)");

                if (failedList.Count > 0)
                {
                    summary.AppendLine($"Failed to restore: {failedList.Count} game(s)\n");
                    summary.AppendLine("Failed games:");
                    foreach (var failed in failedList)
                        summary.AppendLine($" • {failed}");
                }

                FlexibleMessageBox.Show(Program.form, summary.ToString(), "Restore Complete");
            }
        }

        private string listApps()
        {
            ADB.DeviceID = GetDeviceID();
            return ADB.RunAdbCommandToString("shell pm list packages -3").Output;
        }

        public void listAppsBtn()
        {
            m_combo.Invoke(() => { m_combo.Items.Clear(); });

            string[] packages = listApps()
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.StartsWith("package:") ? p.Substring(8).Trim() : p.Trim())
                .ToArray();

            // Save the full string for settings
            settings.InstalledApps = string.Join("\n", packages);
            settings.Save();

            List<string> displayNames = new List<string>();

            foreach (string pkg in packages)
            {
                string name = pkg;

                foreach (string[] game in SideloaderRCLONE.games)
                {
                    if (game.Length > 2 && game[2].Equals(pkg, StringComparison.OrdinalIgnoreCase))
                    {
                        name = game[0]; // Friendly game name
                        break;
                    }
                }

                displayNames.Add(name);
            }

            // Sort and populate combo
            foreach (string name in displayNames.OrderBy(n => n))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    m_combo.Invoke(() => { _ = m_combo.Items.Add(name); });
                }
            }

            m_combo.Invoke(() => { m_combo.MatchingMethod = StringMatchingMethod.NoWildcards; });
        }

        public static bool isuploading = false;
        public static bool isworking = false;
        private async void getApkButton_Click(object sender, EventArgs e)
        {
            if (isOffline)
            {
                notify("You are not connected to the Internet!");
                return;
            }

            string selectedApp = ShowInstalledAppSelector("Select an app to share/upload");
            if (selectedApp == null)
            {
                return;
            }

            DialogResult dialogResult1 = FlexibleMessageBox.Show(Program.form, $"Do you want to upload {selectedApp} now?", "Upload app?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            string deviceCodeName = ADB.RunAdbCommandToString("shell getprop ro.product.device").Output.ToLower().Trim();
            string codeNamesLink = $"https://raw.githubusercontent.com/{repo}/{repo_branch}/codenames";
            bool codenameExists = false;
            try
            {
                codenameExists = HttpClient.GetStringAsync(codeNamesLink).Result.Contains(deviceCodeName);
            }
            catch
            {
                _ = Logger.Log("Unable to download Codenames file.");
                FlexibleMessageBox.Show(Program.form, $"Error downloading Codenames File from Github", "Verification Error", MessageBoxButtons.OK);
            }

            _ = Logger.Log($"Found Device Code Name: {deviceCodeName}");
            _ = Logger.Log($"Identified as Meta Device: {codenameExists}");

            if (codenameExists)
            {
                if (!isworking)
                {
                    isworking = true;
                    progressBar.IsIndeterminate = true;
                    progressBar.OperationType = "Loading";
                    string HWID = SideloaderUtilities.UUID();
                    string GameName = selectedApp;
                    string packageName = Sideloader.gameNameToPackageName(GameName);
                    string InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packageName} | grep versionCode -F\"").Output;
                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                    ulong VersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));

                    string gameName = $"{GameName} v{VersionInt} {packageName} {HWID.Substring(0, 1)} {deviceCodeName}";
                    string gameZipName = $"{gameName}.zip";

                    // Delete both zip & txt if the files exist, most likely due to a failed upload.
                    if (File.Exists($"{settings.MainDir}\\{gameZipName}"))
                    {
                        File.Delete($"{settings.MainDir}\\{gameZipName}");
                    }

                    if (File.Exists($"{settings.MainDir}\\{gameName}.txt"))
                    {
                        File.Delete($"{settings.MainDir}\\{gameName}.txt");
                    }

                    ProcessOutput output = new ProcessOutput("", "");
                    changeTitle("Extracting APK....");

                    _ = Directory.CreateDirectory($"{settings.MainDir}\\{packageName}");

                    Thread t1 = new Thread(() =>
                    {
                        output = Sideloader.getApk(GameName);
                    })
                    {
                        IsBackground = true
                    };
                    t1.Start();

                    while (t1.IsAlive)
                    {
                        await Task.Delay(100);
                    }

                    changeTitle("Extracting OBB if it exists....");
                    Thread t2 = new Thread(() =>
                    {
                        output += ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packageName}\" \"{settings.MainDir}\\{packageName}\"");
                    })
                    {
                        IsBackground = true
                    };
                    t2.Start();

                    while (t2.IsAlive)
                    {
                        await Task.Delay(100);
                    }

                    File.WriteAllText($"{settings.MainDir}\\{packageName}\\HWID.txt", HWID);
                    File.WriteAllText($"{settings.MainDir}\\{packageName}\\uploadMethod.txt", "manual");
                    changeTitle("Zipping extracted application...");
                    string cmd = $"7z a -mx1 \"{gameZipName}\" .\\{packageName}\\*";
                    string path = $"{settings.MainDir}\\7z.exe";
                    progressBar.IsIndeterminate = false;
                    Thread t4 = new Thread(() =>
                    {
                        _ = ADB.RunCommandToString(cmd, path);
                    })
                    {
                        IsBackground = true
                    };
                    t4.Start();
                    while (t4.IsAlive)
                    {
                        await Task.Delay(100);
                    }

                    changeTitle("Uploading to server, you can continue to use Rookie while it uploads.");
                    ULLabel.Visible = true;
                    isworking = false;
                    isuploading = true;
                    Thread t3 = new Thread(() =>
                    {
                        string currentlyUploading = GameName;
                        changeTitle("Uploading to server, you can continue to use Rookie while it uploads.");

                        // Get size of pending zip upload and write to text file
                        long zipSize = new FileInfo($"{settings.MainDir}\\{gameZipName}").Length;
                        File.WriteAllText($"{settings.MainDir}\\{gameName}.txt", zipSize.ToString());
                        // Upload size file.
                        _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{settings.MainDir}\\{gameName}.txt\" RSL-gameuploads:");
                        // Upload zip.
                        _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{settings.MainDir}\\{gameZipName}\" RSL-gameuploads:");

                        // Delete files once uploaded.
                        File.Delete($"{settings.MainDir}\\{gameName}.txt");
                        File.Delete($"{settings.MainDir}\\{gameZipName}");

                        this.Invoke(() => FlexibleMessageBox.Show(Program.form, $"Upload of {currentlyUploading} is complete! Thank you for your contribution!"));
                        FileSystemUtilities.TryDeleteDirectory($"{settings.MainDir}\\{packageName}");
                    })
                    {
                        IsBackground = true
                    };
                    t3.Start();
                    isuploading = true;

                    while (t3.IsAlive)
                    {
                        await Task.Delay(100);
                    }

                    changeTitle("");
                    isuploading = false;
                    ULLabel.Visible = false;
                }
                else
                {
                    _ = MessageBox.Show("You must wait until each app is finished uploading to start another.");
                }
            }
            else
            {
                FlexibleMessageBox.Show(Program.form, $"You are attempting to upload from an unknown device, please connect a Meta Quest device to upload", "Unknown Device", MessageBoxButtons.OK);
            }
        }

        private async void uninstallAppButton_Click(object sender, EventArgs e)
        {
            string selectedApp = ShowInstalledAppSelector("Select an app to uninstall");
            if (selectedApp == null)
            {
                return;
            }

            backupFolder = settings.GetEffectiveBackupDir();

            string packagename;
            string GameName = selectedApp;
            DialogResult dialogresult = FlexibleMessageBox.Show($"Are you sure you want to uninstall {GameName}?", "Proceed with uninstall?", MessageBoxButtons.YesNo);
            if (dialogresult == DialogResult.No)
            {
                return;
            }
            DialogResult dialogresult2 = FlexibleMessageBox.Show($"Do you want to attempt to automatically backup any saves to {backupFolder}\\{DateTime.Today.ToString("yyyy.MM.dd")}\\", "Attempt Game Backup?", MessageBoxButtons.YesNo);
            packagename = !GameName.Contains(".") ? Sideloader.gameNameToPackageName(GameName) : GameName;
            if (dialogresult2 == DialogResult.Yes)
            {
                Sideloader.BackupGame(packagename);
            }
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Loading";
            Thread t1 = new Thread(() =>
            {
                output += Sideloader.UninstallGame(packagename);
            });
            t1.Start();
            t1.IsBackground = true;
            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            ShowPrcOutput(output);
            showAvailableSpace();
            progressBar.IsIndeterminate = false;
        }

        private async void copyBulkObbButton_Click(object sender, EventArgs e)
        {
            FolderSelectDialog dialog = new FolderSelectDialog
            {
                Title = "Select your folder with OBBs"
            };
            if (dialog.Show(Handle))
            {
                changeTitle("Copying OBB folders to device...");
                progressBar.IsIndeterminate = false;
                progressBar.Value = 0;
                progressBar.OperationType = "Copying OBB";
                progressBar.StatusText = "Preparing...";

                Sideloader.RecursiveOutput = new ProcessOutput(String.Empty, String.Empty);

                string currentStatusBase = string.Empty;

                await Sideloader.RecursiveCopyOBBAsync(
                    dialog.FileName,
                    (percent, eta) => this.Invoke(() =>
                    {
                        progressBar.Value = percent;
                        UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                        if (!string.IsNullOrEmpty(currentStatusBase))
                        {
                            progressBar.StatusText = $"{currentStatusBase} · {percent:0.0}%";
                        }
                        else
                        {
                            progressBar.StatusText = $"Copying · {percent:0.0}%";
                        }
                    }),
                    status => this.Invoke(() =>
                    {
                        currentStatusBase = status ?? string.Empty;
                        if (!string.IsNullOrEmpty(status))
                        {
                            changeTitle($"Copying: {status}");
                        }
                    }));

                // Reset UI on completion
                progressBar.Value = 0;
                progressBar.StatusText = "";
                progressBar.IsIndeterminate = false;
                speedLabel.Text = "";
                changeTitle("");

                showAvailableSpace();
                ShowPrcOutput(Sideloader.RecursiveOutput);
            }
        }

        private async void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (nodeviceonstart && !updatesNotified)
            {
                _ = await CheckForDevice();
                changeTitlebarToDevice();
                showAvailableSpace();
                changeTitle("Device detected... refreshing update list.");
                listAppsBtn();
                initListView(false);
            }

            changeTitle($"Processing dropped file. If Rookie freezes, please wait. Do not close Rookie!");
            ProcessOutput output = new ProcessOutput(String.Empty, String.Empty);
            ADB.DeviceID = GetDeviceID();
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Loading";
            CurrPCKG = String.Empty;
            string[] datas = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string data in datas)
            {
                string directory = Path.GetDirectoryName(data);
                //if is directory
                string dir = Path.GetDirectoryName(data);
                string path = $"{dir}\\Install.txt";
                if (Directory.Exists(data))
                {
                    string installFilePath = Directory.GetFiles(data)
                        .FirstOrDefault(f => string.Equals(Path.GetFileName(f), "install.txt", StringComparison.OrdinalIgnoreCase));

                    if (installFilePath != null)
                    {
                        // Run commands from install.txt
                        output += Sideloader.RunADBCommandsFromFile(installFilePath);
                        continue; // Skip further processing if install.txt is found
                    }

                    if (!data.Contains("+") && !data.Contains("_") && data.Contains("."))
                    {
                        _ = Logger.Log($"Copying {data} to device");
                        string folderName = Path.GetFileName(data);
                        changeTitle($"Copying {folderName} to device...");

                        progressBar.IsIndeterminate = false;
                        progressBar.Value = 0;
                        progressBar.OperationType = "Copying OBB";
                        progressBar.StatusText = "Preparing...";

                        string currentStatusBase = string.Empty;

                        output += await ADB.CopyOBBWithProgressAsync(
                            data,
                            (percent, eta) => this.Invoke(() =>
                            {
                                progressBar.Value = percent;
                                UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                                if (!string.IsNullOrEmpty(currentStatusBase))
                                {
                                    progressBar.StatusText = $"{currentStatusBase} · {percent:0.0}%";
                                }
                                else
                                {
                                    progressBar.StatusText = $"Copying · {percent:0.0}%";
                                }
                            }),
                            status => this.Invoke(() =>
                            {
                                currentStatusBase = status ?? string.Empty;
                                if (!string.IsNullOrEmpty(status))
                                {
                                    changeTitle($"Copying: {status}");
                                }
                            }),
                            folderName);

                        // Reset UI after this operation
                        progressBar.StatusText = "";
                        speedLabel.Text = "";
                        changeTitle("");
                        settings.CurrPckg = dir;
                        settings.Save();
                    }

                    changeTitle("");
                    string extension = Path.GetExtension(data);
                    string[] files = Directory.GetFiles(data);

                    foreach (string file2 in files)
                    {
                        if (File.Exists(file2))
                        {
                            if (file2.EndsWith(".apk"))
                            {
                                string pathname = Path.GetDirectoryName(file2);
                                string filename = file2.Replace($"{pathname}\\", String.Empty);

                                string cmd = $"\"{aaptPath}\" dump badging \"{file2}\" | findstr -i \"package: name\"";

                                _ = Logger.Log($"Running adb command-{cmd}");
                                string cmdout = ADB.RunCommandToString(cmd, file2).Output;
                                cmdout = Utilities.StringUtilities.RemoveEverythingBeforeFirst(cmdout, "=");
                                cmdout = Utilities.StringUtilities.RemoveEverythingAfterFirst(cmdout, " ");
                                cmdout = cmdout.Replace("'", String.Empty);
                                cmdout = cmdout.Replace("=", String.Empty);
                                CurrPCKG = cmdout;
                                CurrAPK = file2;
                                System.Windows.Forms.Timer t3 = new System.Windows.Forms.Timer
                                {
                                    Interval = 150000 // 150 seconds to fail
                                };
                                t3.Tick += timer_Tick4;
                                t3.Start();

                                changeTitle($"Sideloading APK ({filename})");
                                progressBar.IsIndeterminate = false;
                                progressBar.Value = 0;
                                progressBar.OperationType = "Installing";
                                progressBar.StatusText = "Preparing...";

                                output += await ADB.SideloadWithProgressAsync(
                                    file2,
                                    (percent, eta) => this.Invoke(() =>
                                    {
                                        if (percent == 0)
                                        {
                                            progressBar.IsIndeterminate = true;
                                            progressBar.OperationType = "Installing";
                                        }
                                        else
                                        {
                                            progressBar.IsIndeterminate = false;
                                            progressBar.Value = percent;
                                        }
                                        UpdateProgressStatus("Installing", percent: (int)Math.Round(percent), eta: eta);
                                        progressBar.StatusText = $"Installing · {percent:0.0}%";
                                    }),
                                    status => this.Invoke(() =>
                                    {
                                        if (!string.IsNullOrEmpty(status))
                                        {
                                            if (status.Contains("Completing Installation"))
                                            {
                                                speedLabel.Text = status;
                                            }
                                            progressBar.StatusText = status;
                                        }
                                    }),
                                    cmdout,
                                    filename);

                                t3.Stop();

                                // Reset after APK install
                                progressBar.StatusText = "";
                                speedLabel.Text = "";

                                if (Directory.Exists($"{pathname}\\{cmdout}"))
                                {
                                    _ = Logger.Log($"Copying OBB folder to device- {cmdout}");
                                    changeTitle($"Copying OBB folder to device...");

                                    progressBar.IsIndeterminate = false;
                                    progressBar.Value = 0;
                                    progressBar.OperationType = "Copying OBB";
                                    progressBar.StatusText = "Preparing...";

                                    string obbStatusBase = string.Empty;

                                    output += await ADB.CopyOBBWithProgressAsync(
                                        $"{pathname}\\{cmdout}",
                                        (percent, eta) => this.Invoke(() =>
                                        {
                                            progressBar.Value = percent;
                                            UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                                            if (!string.IsNullOrEmpty(obbStatusBase))
                                            {
                                                progressBar.StatusText = $"{obbStatusBase} · {percent:0.0}%";
                                            }
                                            else
                                            {
                                                progressBar.StatusText = $"Copying · {percent:0.0}%";
                                            }
                                        }),
                                        status => this.Invoke(() =>
                                        {
                                            obbStatusBase = status ?? string.Empty;
                                        }),
                                        cmdout);

                                    // Reset after OBB copy
                                    progressBar.StatusText = "";
                                    speedLabel.Text = "";
                                    changeTitle("");
                                }
                            }

                            if (file2.EndsWith(".zip") && settings.BMBFChecked)
                            {
                                string datazip = file2;
                                string zippath = Path.GetDirectoryName(data);
                                datazip = datazip.Replace(zippath, "");
                                datazip = Utilities.StringUtilities.RemoveEverythingAfterFirst(datazip, ".");
                                datazip = datazip.Replace(".", "");
                                string command2 = $"\"{settings.MainDir}\\7z.exe\" e \"{file2}\" -o\"{zippath}\\{datazip}\\\"";

                                Thread t2 = new Thread(() =>

                                {
                                    _ = ADB.RunCommandToString(command2, file2);
                                    output += ADB.RunAdbCommandToString($"push \"{zippath}\\{datazip}\" \"/sdcard/ModData/com.beatgames.beatsaber/Mods/SongLoader/CustomLevels/\"");
                                })
                                {
                                    IsBackground = true
                                };
                                t2.Start();

                                while (t2.IsAlive)
                                {
                                    await Task.Delay(100);
                                }

                                FileSystemUtilities.TryDeleteDirectory($"{zippath}\\{datazip}");
                            }
                        }
                    }
                    string[] folders = Directory.GetDirectories(data);
                    foreach (string folder in folders)
                    {
                        string folderName = Path.GetFileName(folder);
                        _ = Logger.Log($"Copying {folder} to device");
                        changeTitle($"Copying {folderName} to device...");

                        progressBar.IsIndeterminate = false;
                        progressBar.Value = 0;
                        progressBar.OperationType = "Copying OBB";
                        progressBar.StatusText = "Preparing...";

                        string folderStatusBase = string.Empty;

                        output += await ADB.CopyOBBWithProgressAsync(
                            folder,
                            (percent, eta) => this.Invoke(() =>
                            {
                                progressBar.Value = percent;
                                UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                                if (!string.IsNullOrEmpty(folderStatusBase))
                                {
                                    progressBar.StatusText = $"{folderStatusBase} · {percent:0.0}%";
                                }
                                else
                                {
                                    progressBar.StatusText = $"Copying · {percent:0.0}%";
                                }
                            }),
                            status => this.Invoke(() =>
                            {
                                folderStatusBase = status ?? string.Empty;
                                if (!string.IsNullOrEmpty(status))
                                {
                                    changeTitle($"Copying: {status}");
                                }
                            }),
                            folderName);

                        // Reset after folder copy
                        progressBar.StatusText = "";
                        speedLabel.Text = "";
                        changeTitle("");
                        settings.CurrPckg = dir;
                        settings.Save();
                    }
                }
                //if it's a file
                else if (File.Exists(data))
                {

                    string extension = Path.GetExtension(data);
                    if (extension == ".apk")
                    {
                        if (File.Exists($"{dir}\\Install.txt"))
                        {
                            DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "Special instructions have been found with this file, would you like to run them automatically?", "Special Instructions found!", MessageBoxButtons.OKCancel);
                            if (dialogResult == DialogResult.Cancel)
                            {
                                return;
                            }
                            else
                            {
                                _ = Logger.Log($"Sideloading custom install.txt");
                                changeTitle("Sideloading custom install.txt automatically.");

                                Thread t1 = new Thread(() =>

                                {
                                    output += Sideloader.RunADBCommandsFromFile(path);
                                })
                                {
                                    IsBackground = true
                                };
                                t1.Start();

                                while (t1.IsAlive)
                                {
                                    await Task.Delay(100);
                                }

                                changeTitle("");
                            }
                        }
                        else
                        {
                            string pathname = Path.GetDirectoryName(data);
                            string dataname = data.Replace($"{pathname}\\", "");
                            string cmd = $"\"{aaptPath}\" dump badging \"{data}\" | findstr -i \"package: name\"";
                            _ = Logger.Log($"Running adb command-{cmd}");
                            string cmdout = ADB.RunCommandToString(cmd, data).Output;
                            cmdout = Utilities.StringUtilities.RemoveEverythingBeforeFirst(cmdout, "=");
                            cmdout = Utilities.StringUtilities.RemoveEverythingAfterFirst(cmdout, " ");
                            cmdout = cmdout.Replace("'", "");
                            cmdout = cmdout.Replace("=", "");
                            CurrPCKG = cmdout;
                            CurrAPK = data;
                            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
                            {
                                Interval = 150000 // 150 seconds to fail
                            };
                            timer.Tick += timer_Tick4;
                            timer.Start();

                            changeTitle($"Installing {dataname}...");
                            progressBar.IsIndeterminate = false;
                            progressBar.Value = 0;
                            progressBar.OperationType = "Installing";
                            progressBar.StatusText = "Preparing...";

                            output += await ADB.SideloadWithProgressAsync(
                                data,
                                (percent, eta) => this.Invoke(() =>
                                {
                                    if (percent == 0)
                                    {
                                        progressBar.IsIndeterminate = true;
                                        progressBar.OperationType = "Installing";
                                    }
                                    else
                                    {
                                        progressBar.IsIndeterminate = false;
                                        progressBar.Value = percent;
                                    }
                                    UpdateProgressStatus("Installing", percent: (int)Math.Round(percent), eta: eta);
                                    progressBar.StatusText = $"Installing · {percent:0.0}%";
                                }),
                                status => this.Invoke(() =>
                                {
                                    if (!string.IsNullOrEmpty(status))
                                    {
                                        if (status.Contains("Completing Installation"))
                                        {
                                            speedLabel.Text = status;
                                        }
                                        progressBar.StatusText = status;
                                    }
                                }),
                                cmdout,
                                dataname);

                            timer.Stop();

                            // Reset after APK install
                            progressBar.StatusText = "";
                            speedLabel.Text = "";

                            if (Directory.Exists($"{pathname}\\{cmdout}"))
                            {
                                _ = Logger.Log($"Copying OBB folder to device- {cmdout}");
                                changeTitle($"Copying OBB folder to device...");

                                progressBar.IsIndeterminate = false;
                                progressBar.Value = 0;
                                progressBar.OperationType = "Copying OBB";
                                progressBar.StatusText = "Preparing...";

                                string obbStatusBase = string.Empty;

                                output += await ADB.CopyOBBWithProgressAsync(
                                    $"{pathname}\\{cmdout}",
                                    (percent, eta) => this.Invoke(() =>
                                    {
                                        progressBar.Value = percent;
                                        UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                                        if (!string.IsNullOrEmpty(obbStatusBase))
                                        {
                                            progressBar.StatusText = $"{obbStatusBase} · {percent:0.0}%";
                                        }
                                        else
                                        {
                                            progressBar.StatusText = $"Copying · {percent:0.0}%";
                                        }
                                    }),
                                    status => this.Invoke(() =>
                                    {
                                        obbStatusBase = status ?? string.Empty;
                                    }),
                                    cmdout);

                                // Reset after OBB copy
                                progressBar.StatusText = "";
                                speedLabel.Text = "";
                                changeTitle("");
                            }
                        }
                    }
                    //If obb is dragged and dropped alone onto Rookie, Rookie will recreate its obb folder automatically with this code.
                    else if (extension == ".obb")
                    {
                        string filename = Path.GetFileName(data);
                        string foldername = filename.Substring(filename.IndexOf('.') + 1);
                        foldername = foldername.Substring(foldername.IndexOf('.') + 1);
                        foldername = foldername.Replace(".obb", "");
                        foldername = Path.Combine(Environment.CurrentDirectory, foldername);
                        _ = Directory.CreateDirectory(foldername);
                        File.Copy(data, Path.Combine(foldername, filename));
                        path = foldername;

                        _ = Logger.Log($"Copying OBB folder to device- {path}");
                        changeTitle($"Copying OBB folder to device ({filename})");

                        progressBar.IsIndeterminate = false;
                        progressBar.Value = 0;
                        progressBar.OperationType = "Copying OBB";
                        progressBar.StatusText = "Preparing...";

                        string obbStatusBase = string.Empty;

                        output += await ADB.CopyOBBWithProgressAsync(
                            path,
                            (percent, eta) => this.Invoke(() =>
                            {
                                progressBar.Value = percent;
                                UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(percent), eta: eta);

                                if (!string.IsNullOrEmpty(obbStatusBase))
                                {
                                    progressBar.StatusText = $"{obbStatusBase} · {percent:0.0}%";
                                }
                                else
                                {
                                    progressBar.StatusText = $"Copying · {percent:0.0}%";
                                }
                            }),
                            status => this.Invoke(() =>
                            {
                                obbStatusBase = status ?? string.Empty;
                            }),
                            filename);

                        // Reset after OBB copy
                        progressBar.StatusText = "";
                        speedLabel.Text = "";

                        FileSystemUtilities.TryDeleteDirectory(foldername);
                        changeTitle("");
                    }
                    // BMBF Zip extraction then push to BMBF song folder on Quest.
                    else if (extension == ".zip" && settings.BMBFChecked)
                    {
                        string datazip = data;
                        string zippath = Path.GetDirectoryName(data);
                        datazip = datazip.Replace(zippath, "");
                        datazip = Utilities.StringUtilities.RemoveEverythingAfterFirst(datazip, ".");
                        datazip = datazip.Replace(".", "");

                        string command = $"\"{settings.MainDir}\\7z.exe\" e \"{data}\" -o\"{zippath}\\{datazip}\\\"";

                        Thread t1 = new Thread(() =>

                        {
                            _ = ADB.RunCommandToString(command, data);
                            output += ADB.RunAdbCommandToString($"push \"{zippath}\\{datazip}\" \"/sdcard/ModData/com.beatgames.beatsaber/Mods/SongLoader/CustomLevels/\"");
                        })
                        {
                            IsBackground = true
                        };
                        t1.Start();

                        while (t1.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        FileSystemUtilities.TryDeleteDirectory($"{zippath}\\{datazip}");
                    }
                    else if (extension == ".txt")
                    {
                        changeTitle("Sideloading custom install.txt automatically.");

                        Thread t1 = new Thread(() =>

                        {
                            output += Sideloader.RunADBCommandsFromFile(path);
                        })
                        {
                            IsBackground = true
                        };
                        t1.Start();

                        while (t1.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        changeTitle("");
                    }
                }
            }

            // Final reset of all UI elements
            progressBar.Value = 0;
            progressBar.StatusText = "";
            progressBar.IsIndeterminate = false;
            speedLabel.Text = "";

            showAvailableSpace();
            ShowPrcOutput(output);
            listAppsBtn();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {
            changeTitle("");
        }

        private List<string> newGamesList = new List<string>();
        private List<string> newGamesToUploadList = new List<string>();

        private readonly List<UpdateGameData> gamesToAskForUpdate = new List<UpdateGameData>();
        public static bool loaded = false;
        public static string rookienamelist;
        public static bool updates = false;
        public static bool newapps = false;
        public static int newint = 0;
        public static int updint = 0;
        public static bool nodeviceonstart = false;
        public static bool either = false;
        private bool _allItemsInitialized = false;
        private bool _suppressDonorsDialog = false;

        private async void initListView(bool favoriteView)
        {
            var sw = Stopwatch.StartNew();
            Logger.Log("initListView started");

            int upToDateCount = 0;
            int updateAvailableCount = 0;
            int newerThanListCount = 0;
            loaded = false;

            // Reset donation/update tracking so stale values from a prior call
            // don't trigger the donors dialog after an offline→online switch.
            either = false;
            updates = false;
            newapps = false;
            updint = 0;
            newint = 0;
            donorApps = "";
            gamesToAskForUpdate.Clear();

            var rookienameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rookienameListBuilder = new StringBuilder();

            string installedApps = settings.InstalledApps;
            string[] packageList = installedApps.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (packageList.Length == 0)
            {
                Logger.Log("No installed packages found, continuing in download-only mode");
                nodeviceonstart = true;
            }

            string[] blacklist = new string[] { };
            string[] whitelist = new string[] { };

            // Load blacklists/whitelists concurrently
            await Task.WhenAll(
                Task.Run(() =>
                {
                    if (File.Exists($"{settings.MainDir}\\nouns\\blacklist.txt"))
                    {
                        blacklist = File.ReadAllLines($"{settings.MainDir}\\nouns\\blacklist.txt");
                    }

                    string localBlacklistPath = Path.Combine(settings.MainDir, "blacklist.json");
                    if (File.Exists(localBlacklistPath))
                    {
                        try
                        {
                            string jsonContent = File.ReadAllText(localBlacklistPath);
                            string[] localBlacklist = JsonConvert.DeserializeObject<string[]>(jsonContent);
                            if (localBlacklist != null && localBlacklist.Length > 0)
                            {
                                var combined = new List<string>(blacklist);
                                combined.AddRange(localBlacklist);
                                blacklist = combined.ToArray();
                                Logger.Log($"Loaded {localBlacklist.Length} entries from local blacklist");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Error loading local blacklist: {ex.Message}", LogLevel.WARNING);
                        }
                    }
                }),
                Task.Run(() =>
                {
                    if (File.Exists($"{settings.MainDir}\\nouns\\whitelist.txt"))
                    {
                        whitelist = File.ReadAllLines($"{settings.MainDir}\\nouns\\whitelist.txt");
                    }
                })
            );

            Logger.Log($"Blacklist/Whitelist loaded in {sw.ElapsedMilliseconds}ms");

            int expectedGameCount = SideloaderRCLONE.games.Count > 0 ? SideloaderRCLONE.games.Count : 500;
            var GameList = new List<ListViewItem>(expectedGameCount);
            var rookieList = new List<string>(expectedGameCount);

            var installedGamesSet = new HashSet<string>(packageList, StringComparer.OrdinalIgnoreCase);
            var blacklistSet = new HashSet<string>(blacklist, StringComparer.OrdinalIgnoreCase);
            var whitelistSet = new HashSet<string>(whitelist, StringComparer.OrdinalIgnoreCase);

            newGamesToUploadList = whitelistSet.Intersect(installedGamesSet, StringComparer.OrdinalIgnoreCase).ToList();

            if (SideloaderRCLONE.games.Count > 5)
            {
                progressBar.IsIndeterminate = true;
                progressBar.OperationType = "Loading";

                // Use full dumpsys to get all version codes at once
                Dictionary<string, ulong> installedVersions = new Dictionary<string, ulong>(packageList.Length, StringComparer.OrdinalIgnoreCase);

                await Task.Run(() =>
                {
                    Logger.Log("Fetching version codes via full dumpsys...");
                    var versionSw = Stopwatch.StartNew();

                    try
                    {
                        var dump = ADB.RunAdbCommandToString("shell dumpsys package").Output;
                        Logger.Log($"Dumpsys returned {dump.Length} chars in {versionSw.ElapsedMilliseconds}ms");
                        versionSw.Restart();

                        string currentPkg = null;

                        foreach (var rawLine in dump.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var line = rawLine.TrimStart();

                            if (line.StartsWith("Package [", StringComparison.Ordinal))
                            {
                                var start = line.IndexOf('[');
                                var end = line.IndexOf(']');
                                if (start >= 0 && end > start)
                                {
                                    currentPkg = line.Substring(start + 1, end - start - 1);
                                }
                                else
                                {
                                    currentPkg = null;
                                }
                                continue;
                            }

                            if (currentPkg != null && line.StartsWith("versionCode=", StringComparison.Ordinal))
                            {
                                var after = line.Substring(12);
                                int spaceIdx = after.IndexOf(' ');
                                var digits = spaceIdx > 0 ? after.Substring(0, spaceIdx) : after;
                                if (ulong.TryParse(digits, out var v))
                                {
                                    // Only store if it's an installed package we care about
                                    if (installedGamesSet.Contains(currentPkg))
                                    {
                                        installedVersions[currentPkg] = v;
                                    }
                                }
                                currentPkg = null;
                                continue;
                            }
                        }

                        Logger.Log($"Parsed {installedVersions.Count} version codes in {versionSw.ElapsedMilliseconds}ms");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"'dumpsys package' failed: {ex.Message}", LogLevel.ERROR);
                    }

                    Logger.Log($"Version fetch total: {versionSw.ElapsedMilliseconds}ms");
                });

                Logger.Log($"Version codes collected in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                // Precompute cloud max version per package
                var cloudMaxVersionByPackage = new Dictionary<string, ulong>(SideloaderRCLONE.games.Count, StringComparer.OrdinalIgnoreCase);
                foreach (var release in SideloaderRCLONE.games)
                {
                    string pkg = release[SideloaderRCLONE.PackageNameIndex];
                    ulong v = 0;
                    try
                    {
                        v = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));
                    }
                    catch
                    {
                        v = 0;
                    }

                    if (cloudMaxVersionByPackage.TryGetValue(pkg, out var existing))
                    {
                        if (v > existing) cloudMaxVersionByPackage[pkg] = v;
                    }
                    else
                    {
                        cloudMaxVersionByPackage[pkg] = v;
                    }
                }

                Logger.Log($"Cloud versions precomputed in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                // Calculate popularity rankings
                var popularityScores = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                foreach (string[] release in SideloaderRCLONE.games)
                {
                    string packagename = release[SideloaderRCLONE.PackageNameIndex];

                    // Parse popularity score from column 6
                    if (release.Length > 6 && StringUtilities.TryParseDouble(release[6], out double score))
                    {
                        // Track the highest score per package
                        if (popularityScores.TryGetValue(packagename, out var existing))
                        {
                            if (score > existing)
                                popularityScores[packagename] = score;
                        }
                        else
                        {
                            popularityScores[packagename] = score;
                        }
                    }
                }

                // Sort packages by popularity (descending) and assign rankings
                var rankedPackages = popularityScores
                    .Where(kvp => kvp.Value > 0) // Exclude 0.00 scores
                    .OrderByDescending(kvp => kvp.Value)
                    .Select((kvp, index) => new { Package = kvp.Key, Rank = index + 1 })
                    .ToDictionary(x => x.Package, x => x.Rank, StringComparer.OrdinalIgnoreCase);

                Logger.Log($"Popularity rankings calculated for {rankedPackages.Count} games in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                // Build MR-Fix lookup. map base game name to whether an MR-Fix exists
                var mrFixGames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                const string MrFixTag = "(MR-Fix)";

                foreach (string[] release in SideloaderRCLONE.games)
                {
                    string gameName = release[SideloaderRCLONE.GameNameIndex];

                    // Check if game name contains "(MR-Fix)" using IndexOf for case-insensitive search
                    int mrFixIndex = gameName.IndexOf(MrFixTag, StringComparison.OrdinalIgnoreCase);
                    if (mrFixIndex >= 0)
                    {
                        string baseGameName =
                            (gameName.Substring(0, mrFixIndex) +
                             gameName.Substring(mrFixIndex + MrFixTag.Length)).Trim();

                        mrFixGames.Add(baseGameName);
                    }
                }

                Logger.Log($"MR-Fix lookup built with {mrFixGames.Count} games in {sw.ElapsedMilliseconds}ms");
                sw.Restart();

                // Process games on background thread
                await Task.Run(() =>
                {
                    foreach (string[] release in SideloaderRCLONE.games)
                    {
                        string packagename = release[SideloaderRCLONE.PackageNameIndex];
                        rookieList.Add(packagename);

                        string gameName = release[SideloaderRCLONE.GameNameIndex];
                        if (rookienameSet.Add(gameName))
                        {
                            rookienameListBuilder.Append(gameName).Append('\n');
                        }

                        var item = new ListViewItem(release);

                        // Check if this is a 0 MB entry that should be excluded
                        bool shouldSkip = false;
                        if (release.Length > 5 && StringUtilities.TryParseDouble(release[5], out double sizeInMB))
                        {
                            // If size is 0 MB and this is not already an MR-Fix version
                            if (sizeInMB == 0 && gameName.IndexOf("(MR-Fix)", StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                // Check if there's an MR-Fix version of this game
                                if (mrFixGames.Contains(gameName))
                                {
                                    shouldSkip = true;
                                }
                            }
                        }

                        if (shouldSkip)
                        {
                            continue; // Skip this entry
                        }

                        // Show the installed version
                        ulong installedVersion = 0;

                        if (installedVersions.TryGetValue(packagename, out ulong installedVersionInt))
                        {
                            item.ForeColor = ColorInstalled;
                            installedVersion = installedVersionInt;

                            try
                            {
                                cloudMaxVersionByPackage.TryGetValue(packagename, out ulong cloudVersionInt);

                                if (installedVersionInt == cloudVersionInt)
                                {
                                    upToDateCount++;
                                }
                                else if (installedVersionInt < cloudVersionInt)
                                {
                                    item.ForeColor = ColorUpdateAvailable;
                                    updateAvailableCount++;
                                }
                                else if (installedVersionInt > cloudVersionInt)
                                {
                                    bool dontget = blacklistSet.Contains(packagename);

                                    newerThanListCount++;
                                    item.ForeColor = ColorDonateGame;

                                    // Only prompt for upload if not blacklisted
                                    if (!dontget && !updatesNotified && !isworking && updint < 6 && !settings.SubmittedUpdates.Contains(packagename))
                                    {
                                        either = true;
                                        updates = true;
                                        updint++;

                                        string RlsName = Sideloader.PackageNametoGameName(packagename);
                                        string GameName = Sideloader.gameNameToSimpleName(RlsName);
                                        var gameData = new UpdateGameData(GameName, packagename, installedVersionInt);
                                        gamesToAskForUpdate.Add(gameData);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                item.ForeColor = ColorError;
                                Logger.Log($"An error occurred while rendering game {release[SideloaderRCLONE.GameNameIndex]} in ListView", LogLevel.ERROR);
                                Logger.Log($"ExMsg: {ex.Message}", LogLevel.ERROR);
                            }
                        }

                        if (installedVersion != 0)
                        {
                            // Show the installed version and attach 'v' to both versions
                            item.SubItems[3].Text = $"v{item.SubItems[3].Text} / v{installedVersion}";
                        }
                        else
                        {
                            // Attach 'v' to remote version
                            item.SubItems[3].Text = $"v{item.SubItems[3].Text}";
                        }

                        // Remove ' UTC' from last updated
                        item.SubItems[4].Text = item.SubItems[4].Text.Replace(" UTC", "");

                        // Convert size to GB or MB
                        if (StringUtilities.TryParseDouble(item.SubItems[5].Text, out double itemSizeInMB))
                        {
                            if (itemSizeInMB >= 1024)
                            {
                                double sizeInGB = itemSizeInMB / 1024;
                                item.SubItems[5].Text = $"{sizeInGB:F2} GB";
                            }
                            else
                            {
                                item.SubItems[5].Text = $"{itemSizeInMB:F0} MB";
                            }
                        }

                        // Replace popularity score with ranking
                        if (rankedPackages.TryGetValue(packagename, out int rank))
                        {
                            item.SubItems[6].Text = $"#{rank}";
                        }
                        else
                        {
                            // Unranked (0.00 popularity or not found)
                            item.SubItems[6].Text = "-";
                        }

                        if (favoriteView)
                        {
                            if (settings.FavoritedGames.Contains(item.SubItems[1].Text))
                            {
                                GameList.Add(item);
                            }
                        }
                        else
                        {
                            GameList.Add(item);
                        }
                    }
                });

                rookienamelist = rookienameListBuilder.ToString();

                Logger.Log($"Game processing completed in {sw.ElapsedMilliseconds}ms");
                sw.Restart();
            }
            else if (!isOffline && !_isManualMirrorSwitch)
            {
                SwitchMirrors();
                if (!isOffline)
                {
                    // If user provided a new config (quotaTries was reset to 0),
                    // do a full reconnect before retrying
                    if (quotaTries == 0)
                    {
                        // Reset mirror cycling state left over from exhaustion
                        reset = false;
                        steps = 0;

                        // Prevent remotesList_SelectedIndexChanged → refreshCurrentMirror
                        // from running concurrently (same guard used during startup)
                        isLoading = true;
                        loaded = false;
                        _suppressDonorsDialog = true;

                        // Clear stale data so the recursive call rebuilds everything
                        _allItems?.Clear();
                        _allItemsInitialized = false;
                        _galleryDataSource = null;
                        _searchIndex?.Clear();
                        SideloaderRCLONE.games.Clear();

                        RCLONE.killRclone();
                        await Task.Run(() =>
                        {
                            changeTitle("Reloading...");
                            GetDependencies.downloadFiles();
                            RCLONE.Init();
                        });

                        await initMirrors();

                        if (UsingPublicConfig)
                        {
                            changeTitle("Updating Metadata...");
                            await Task.Run(() =>
                            {
                                SideloaderRCLONE.UpdateMetadataFromPublic();
                                SideloaderRCLONE.ProcessMetadataFromPublic();
                            });
                        }
                        else
                        {
                            changeTitle("Grabbing the Games List...");
                            await Task.Run(() => SideloaderRCLONE.initGames(currentRemote));
                        }

                        isLoading = false;
                    }

                    initListView(false);
                }
                return;
            }
            else
            {
                // Offline mode with local games scanned
                // Build items from SideloaderRCLONE.games (populated by ScanLocalGames)
                if (SideloaderRCLONE.games.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        foreach (string[] release in SideloaderRCLONE.games)
                        {
                            string packagename = release[SideloaderRCLONE.PackageNameIndex];
                            string gameName = release[SideloaderRCLONE.GameNameIndex];
                            if (rookienameSet.Add(gameName))
                            {
                                rookienameListBuilder.Append(gameName).Append('\n');
                            }

                            var item = new ListViewItem(release);

                            // Attach 'v' to version
                            item.SubItems[3].Text = $"v{item.SubItems[3].Text}";

                            // Convert size to GB or MB
                            if (StringUtilities.TryParseDouble(item.SubItems[5].Text, out double itemSizeInMB))
                            {
                                if (itemSizeInMB >= 1024)
                                {
                                    double sizeInGB = itemSizeInMB / 1024;
                                    item.SubItems[5].Text = $"{sizeInGB:F2} GB";
                                }
                                else
                                {
                                    item.SubItems[5].Text = $"{itemSizeInMB:F0} MB";
                                }
                            }

                            // Local-only: mark as downloaded
                            item.ForeColor = ColorDownloaded;
                            item.SubItems[6].Text = "Local";

                            if (favoriteView)
                            {
                                if (settings.FavoritedGames.Contains(item.SubItems[1].Text))
                                {
                                    GameList.Add(item);
                                }
                            }
                            else
                            {
                                GameList.Add(item);
                            }
                        }
                    });

                    rookienamelist = rookienameListBuilder.ToString();
                }
            }

            if (blacklistSet.Count == 0 && GameList.Count == 0 && !settings.NodeviceMode && !isOffline)
            {
                _ = FlexibleMessageBox.Show(Program.form,
                    "Rookie seems to have failed to load all resources. Please try restarting Rookie a few times.\nIf error still persists please disable any VPN or firewalls (rookie uses direct download so a VPN is not needed)\nIf this error still persists try a system reboot, reinstalling the program, and lastly posting the problem on telegram.\n\nTip: You can restart in offline mode (--offline) to browse your downloaded library.",
                    "Error loading blacklist or game list!");
            }

            var rookieSet = new HashSet<string>(rookieList, StringComparer.OrdinalIgnoreCase);
            newGamesList = installedGamesSet
                .Except(rookieSet, StringComparer.OrdinalIgnoreCase)
                .Except(blacklistSet, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (blacklistSet.Count > 100 && rookieList.Count > 100)
            {
                await ProcessNewApps(newGamesList, blacklistSet.ToList());
            }

            if (either && !updatesNotified && !noAppCheck && hasUploadConfig && !isOffline && !_suppressDonorsDialog
                && !string.IsNullOrWhiteSpace(donorApps))
            {
                changeTitle("");
                DonorsListViewForm donorForm = new DonorsListViewForm();
                _ = donorForm.ShowDialog(this);
                _ = Focus();
            }
            _suppressDonorsDialog = false;

            // Count downloaded titles by checking for matching folders in the download directory
            int downloadedCount = 0;
            string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
            if (Directory.Exists(dlDir))
            {
                var localDirNames = new HashSet<string>(
                    Directory.GetDirectories(dlDir).Select(d => Path.GetFileName(d)),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var item in GameList)
                {
                    if (item.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex) continue;
                    string releaseName = item.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                    if (!localDirNames.Contains(releaseName)) continue;
                    string folderPath = Path.Combine(dlDir, releaseName);
                    try
                    {
                        if (Directory.GetFiles(folderPath, "*.apk").Length > 0)
                            downloadedCount++;
                    }
                    catch { }
                }
            }

            // Update UI with computed list
            this.Invoke(() =>
            {
                changeTitle("Populating update list...\n\n");
                int installedTotal = upToDateCount + updateAvailableCount;
                btnInstalled.Text = $"{installedTotal} INSTALLED";
                btnInstalled.ForeColor = ColorInstalled;
                if (updateAvailableCount != 1) btnUpdateAvailable.Text = $"{updateAvailableCount} UPDATES";
                else btnUpdateAvailable.Text = $"{updateAvailableCount} UPDATE";
                btnUpdateAvailable.ForeColor = ColorUpdateAvailable;
                btnNewerThanList.Text = $"{newerThanListCount} AHEAD OF LIST";
                btnNewerThanList.ForeColor = ColorDonateGame;
                btnDownloaded.Text = $"{downloadedCount} DOWNLOADED";
                btnDownloaded.ForeColor = ColorDownloaded;

                ListViewItem[] arr = GameList.ToArray();
                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(arr);
                gamesListView.EndUpdate();
            });

            Logger.Log($"UI updated in {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            changeTitle("");

            if (!_allItemsInitialized)
            {
                _allItems = gamesListView.Items.Cast<ListViewItem>().ToList();

                _searchIndex = new Dictionary<string, List<ListViewItem>>(_allItems.Count * 2, StringComparer.OrdinalIgnoreCase);

                foreach (var item in _allItems)
                {
                    string gameNameKey = item.Text;
                    if (!_searchIndex.TryGetValue(gameNameKey, out var list))
                    {
                        list = new List<ListViewItem>(1);
                        _searchIndex[gameNameKey] = list;
                    }
                    list.Add(item);

                    if (item.SubItems.Count > 1)
                    {
                        string releaseName = item.SubItems[1].Text;
                        if (!_searchIndex.TryGetValue(releaseName, out var releaseList))
                        {
                            releaseList = new List<ListViewItem>(1);
                            _searchIndex[releaseName] = releaseList;
                        }
                        releaseList.Add(item);
                    }
                }

                _allItemsInitialized = true;
            }

            loaded = true;
            isLoading = false;
            Logger.Log($"initListView total completed in {sw.ElapsedMilliseconds}ms");

            // Loading is fully complete — stop the progress bar
            progressBar.IsIndeterminate = false;

            // Show header now that loading is complete
            if (_listViewRenderer != null)
            {
                _listViewRenderer.SuppressHeader = false;
            }

            // Now that ListView is fully populated and _allItems is initialized,
            // switch to the user's preferred view
            this.Invoke(() =>
            {
                if (isGalleryView)
                {
                    // Now it's safe to switch - ListView has been visible and populated
                    gamesListView.Visible = false;
                    gamesGalleryView.Visible = true;
                    _galleryDataSource = null;
                    PopulateGalleryView();
                }
            });
        }

        private async Task ProcessNewApps(List<string> newGamesList, List<string> blacklistItems)
        {
            await Task.Run(() =>
            {
                foreach (UpdateGameData gameData in gamesToAskForUpdate)
                {
                    if (!updatesNotified && !settings.SubmittedUpdates.Contains(gameData.Packagename))
                    {
                        either = true;
                        updates = true;
                        donorApps += gameData.GameName + ";" + gameData.Packagename + ";" + gameData.InstalledVersionInt + ";" + "Update" + "\n";
                    }
                }

                string baseApkPath = Path.Combine(Environment.CurrentDirectory, "platform-tools", "base.apk");
                if (blacklistItems.Count > 100 && !noAppCheck)
                {
                    foreach (string newGamesToUpload in newGamesList)
                    {
                        try
                        {
                            bool onapplist = false;
                            string NewApp = settings.NonAppPackages + "\n" + settings.AppPackages;
                            if (NewApp.Contains(newGamesToUpload))
                            {
                                onapplist = true;
                                Logger.Log($"App '{newGamesToUpload}' found in app list.", LogLevel.INFO);
                            }

                            string RlsName = Sideloader.PackageNametoGameName(newGamesToUpload);
                            Logger.Log($"Release name obtained: {RlsName}", LogLevel.INFO);

                            if (!updatesNotified && !onapplist && newint < 6 && hasUploadConfig)
                            {
                                changeTitle("Unrecognized App found. Downloading APK to take a closer look. (This may take a minute)");

                                either = true;
                                newapps = true;
                                Logger.Log($"New app detected: {newGamesToUpload}, starting APK extraction and AAPT process.", LogLevel.INFO);

                                string apppath = ADB.RunAdbCommandToString($"shell pm path {newGamesToUpload}").Output;
                                Logger.Log($"ADB command 'pm path' executed. Path: {apppath}", LogLevel.INFO);
                                apppath = Utilities.StringUtilities.RemoveEverythingBeforeFirst(apppath, "/");
                                apppath = Utilities.StringUtilities.RemoveEverythingAfterFirst(apppath, "\r\n");

                                if (File.Exists(baseApkPath))
                                {
                                    File.Delete(baseApkPath);
                                    Logger.Log("Old base.apk file deleted.", LogLevel.INFO);
                                }

                                Logger.Log($"Pulling APK from path: {apppath}", LogLevel.INFO);
                                _ = ADB.RunAdbCommandToString($"pull \"{apppath}\"");

                                string cmd = $"\"{aaptPath}\" dump badging \"{baseApkPath}\" | findstr -i \"application-label\"";
                                Logger.Log($"Running AAPT command: {cmd}", LogLevel.INFO);
                                string ReleaseName = ADB.RunCommandToString(cmd, aaptPath).Output;
                                Logger.Log($"AAPT command output: {ReleaseName}", LogLevel.INFO);

                                ReleaseName = Utilities.StringUtilities.RemoveEverythingBeforeFirst(ReleaseName, "'");
                                ReleaseName = Utilities.StringUtilities.RemoveEverythingAfterFirst(ReleaseName, "\r\n");
                                ReleaseName = ReleaseName.Replace("'", "");
                                File.Delete(baseApkPath);
                                Logger.Log("Base.apk deleted after extracting release name.", LogLevel.INFO);

                                if (ReleaseName.Contains("Microsoft Windows"))
                                {
                                    ReleaseName = RlsName;
                                    Logger.Log("Release name fallback to RlsName due to Microsoft Windows detection.", LogLevel.INFO);
                                }

                                Logger.Log($"Final Release Name: {ReleaseName}", LogLevel.INFO);

                                string GameName = Sideloader.gameNameToSimpleName(RlsName);
                                Logger.Log($"Fetching version code for app: {newGamesToUpload}", LogLevel.INFO);

                                string InstalledVersionCode;
                                InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {newGamesToUpload} | grep versionCode -F\"").Output;
                                Logger.Log($"Version code command output: {InstalledVersionCode}", LogLevel.INFO);
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                ulong installedVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                Logger.Log($"Parsed installed version code: {installedVersionInt}", LogLevel.INFO);

                                donorApps += ReleaseName + ";" + newGamesToUpload + ";" + installedVersionInt + ";" + "New App" + "\n";
                                Logger.Log($"Donor app info updated: {ReleaseName}; {newGamesToUpload}; {installedVersionInt}", LogLevel.INFO);
                                newint++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Exception occured in ProcessNewApps: {ex.Message}", LogLevel.ERROR);
                        }
                    }
                }
            });
        }

        private static readonly HttpClient HttpClient = new HttpClient();
        public static async void doUpload()
        {
            Program.form.changeTitle("Uploading to server, you can continue to use Rookie while it uploads.");
            Program.form.ULLabel.Visible = true;
            isworking = true;
            string deviceCodeName = ADB.RunAdbCommandToString("shell getprop ro.product.device").Output.ToLower().Trim();
            string codeNamesLink = $"https://raw.githubusercontent.com/{repo}/{repo_branch}/codenames";
            bool codenameExists = false;
            try
            {
                codenameExists = HttpClient.GetStringAsync(codeNamesLink).Result.Contains(deviceCodeName);
            }
            catch
            {
                _ = Logger.Log("Unable to download Codenames file.");
                FlexibleMessageBox.Show(Program.form, $"Error downloading Codenames File from Github", "Verification Error", MessageBoxButtons.OK);
            }

            if (codenameExists)
            {
                foreach (UploadGame game in Program.form.gamesToUpload)
                {

                    Thread t3 = new Thread(() =>
                    {
                        string packagename = game.Pckgcommand;
                        string gameName = $"{game.Uploadgamename} v{game.Uploadversion} {game.Pckgcommand} {SideloaderUtilities.UUID().Substring(0, 1)} {deviceCodeName}";
                        string gameZipName = $"{gameName}.zip";

                        // Delete both zip & txt if the files exist, most likely due to a failed upload.
                        if (File.Exists($"{settings.MainDir}\\{gameZipName}"))
                        {
                            File.Delete($"{settings.MainDir}\\{gameZipName}");
                        }

                        if (File.Exists($"{settings.MainDir}\\{gameName}.txt"))
                        {
                            File.Delete($"{settings.MainDir}\\{gameName}.txt");
                        }

                        string path = $"{settings.MainDir}\\7z.exe";
                        string cmd = $"7z a -mx1 \"{settings.MainDir}\\{gameZipName}\" .\\{game.Pckgcommand}\\*";
                        Program.form.changeTitle("Zipping extracted application...");
                        _ = ADB.RunCommandToString(cmd, path);
                        if (Directory.Exists($"{settings.MainDir}\\{game.Pckgcommand}"))
                        {
                            FileSystemUtilities.TryDeleteDirectory($"{settings.MainDir}\\{game.Pckgcommand}");
                        }
                        Program.form.changeTitle("Uploading to server, you can continue to use Rookie while it uploads.");

                        // Get size of pending zip upload and write to text file
                        long zipSize = new FileInfo($"{settings.MainDir}\\{gameZipName}").Length;
                        File.WriteAllText($"{settings.MainDir}\\{gameName}.txt", zipSize.ToString());
                        // Upload size file.
                        _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{settings.MainDir}\\{gameName}.txt\" RSL-gameuploads:");
                        // Upload zip.
                        _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{settings.MainDir}\\{gameZipName}\" RSL-gameuploads:");

                        if (game.isUpdate)
                        {
                            settings.SubmittedUpdates += game.Pckgcommand + "\n";
                            settings.Save();
                        }

                        // Delete files once uploaded.
                        if (File.Exists($"{settings.MainDir}\\{gameName}.txt"))
                        {
                            File.Delete($"{settings.MainDir}\\{gameName}.txt");
                        }
                        if (File.Exists($"{settings.MainDir}\\{gameZipName}"))
                        {
                            File.Delete($"{settings.MainDir}\\{gameZipName}");
                        }

                    })
                    {
                        IsBackground = true
                    };
                    t3.Start();
                    while (t3.IsAlive)
                    {
                        isuploading = true;
                        await Task.Delay(100);
                    }
                }

                Program.form.gamesToUpload.Clear();
                isworking = false;
                isuploading = false;
                Program.form.ULLabel.Visible = false;
                Program.form.changeTitle("");
            }
            else
            {
                FlexibleMessageBox.Show(Program.form, $"You are attempting to upload from an unknown device, please connect a Meta Quest device to upload", "Unknown Device", MessageBoxButtons.OK);
            }
        }

        public static async void newPackageUpload()
        {
            if (!string.IsNullOrEmpty(settings.NonAppPackages) && !settings.ListUpped)
            {
                Random r = new Random();
                int x = r.Next(9999);
                int y = x;
                File.WriteAllText($"{settings.MainDir}\\FreeOrNonVR{y}.txt", settings.NonAppPackages);
                string path = $"{settings.MainDir}\\rclone\\rclone.exe";

                Thread t1 = new Thread(() =>
                {
                    _ = ADB.RunCommandToString($"\"{settings.MainDir}\\rclone\\rclone.exe\" copy \"{settings.MainDir}\\FreeOrNonVR{y}.txt\" debuglogs:InstalledGamesList", path);
                    File.Delete($"{settings.MainDir}\\FreeOrNonVR{y}.txt");
                })
                {
                    IsBackground = true
                };
                t1.Start();

                while (t1.IsAlive)
                {
                    await Task.Delay(100);
                }

                settings.ListUpped = true;
                settings.Save();

            }
        }


        public async Task extractAndPrepareGameToUploadAsync(string GameName, string packagename, ulong installedVersionInt, bool isupdate)
        {
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "";

            Thread t1 = new Thread(() =>
            {
                _ = Sideloader.getApk(packagename);
            });
            changeTitle("Extracting APK file....");
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            changeTitle("Extracting OBB if it exists....");
            Thread t2 = new Thread(() =>
            {
                _ = ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packagename}\" \"{settings.MainDir}\\{packagename}\"");
            })
            {
                IsBackground = true
            };
            t2.Start();

            while (t2.IsAlive)
            {
                await Task.Delay(100);
            }

            string HWID = SideloaderUtilities.UUID();
            File.WriteAllText($"{settings.MainDir}\\{packagename}\\HWID.txt", HWID);
            progressBar.IsIndeterminate = false;
            UploadGame game = new UploadGame
            {
                isUpdate = isupdate,
                Pckgcommand = packagename,
                Uploadgamename = GameName,
                Uploadversion = installedVersionInt
            };
            gamesToUpload.Add(game);
        }

        /// <summary>
        /// Parses the rclone download config and returns the set of top-level
        /// remote names — remotes that are NOT referenced as dependencies by
        /// other remotes.  In rclone configs, a remote can reference another
        /// using the "remotename:path" syntax (e.g. alias, crypt, union, cache).
        /// Those dependency remotes are helpers and should not appear as mirrors.
        /// </summary>
        private static HashSet<string> GetTopLevelRemotes()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                string configPath = Path.Combine(Environment.CurrentDirectory, "rclone", RCLONE.downloadConfigPath);
                if (!File.Exists(configPath))
                    return result;

                string[] lines = File.ReadAllLines(configPath);

                // First pass: collect section names and their config values
                var sections = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                string currentSection = null;

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        currentSection = trimmed.Substring(1, trimmed.Length - 2).Trim();
                        if (!sections.ContainsKey(currentSection))
                            sections[currentSection] = new List<string>();
                    }
                    else if (currentSection != null && trimmed.Contains("="))
                    {
                        int eqIdx = trimmed.IndexOf('=');
                        string value = trimmed.Substring(eqIdx + 1).Trim();
                        sections[currentSection].Add(value);
                    }
                }

                // Second pass: find which remotes are referenced by others
                // via the standard rclone "remotename:" pattern in values
                var referenced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in sections)
                {
                    foreach (string value in kvp.Value)
                    {
                        foreach (string remoteName in sections.Keys)
                        {
                            if (string.Equals(kvp.Key, remoteName, StringComparison.OrdinalIgnoreCase))
                                continue; // skip self-references

                            if (value.IndexOf(remoteName + ":", StringComparison.OrdinalIgnoreCase) >= 0)
                                referenced.Add(remoteName);
                        }
                    }
                }

                // Top-level = all sections that are not referenced by others
                foreach (string name in sections.Keys)
                {
                    if (!referenced.Contains(name))
                        result.Add(name);
                }

                Logger.Log($"Top-level remotes: {string.Join(", ", result)}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to parse rclone config for top-level remotes: {ex.Message}", LogLevel.WARNING);
            }

            return result;
        }

        private async Task initMirrors()
        {
            _ = Logger.Log("Looking for Additional Mirrors...");

            int index = 0;
            await Task.Run(() => remotesList.Invoke(() =>
            {
                index = remotesList.SelectedIndex;
                remotesList.Items.Clear();
            }));

            // Retry logic for RCLONE availability
            string[] mirrors = null;
            int maxRetries = 10;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    mirrors = await Task.Run(() => RCLONE.runRcloneCommand_DownloadConfig("listremotes").Output.Split('\n'));
                    break; // Success, exit retry loop
                }
                catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 2) // File not found
                {
                    retryCount++;
                    Logger.Log($"RCLONE not ready yet, attempt {retryCount}/{maxRetries}. Waiting...", LogLevel.WARNING);

                    if (retryCount >= maxRetries)
                    {
                        Logger.Log("RCLONE failed to initialize after multiple attempts", LogLevel.ERROR);
                        _ = FlexibleMessageBox.Show(Program.form,
                            "RCLONE could not be initialized. Please check your internet connection and restart the application.\n\nIf the problem persists, try running as Administrator.",
                            "RCLONE Initialization Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        // Fallback: Add only public mirror if available
                        if (hasPublicConfig)
                        {
                            await Task.Run(() => remotesList.Invoke(() =>
                            {
                                _ = remotesList.Items.Add("Public");
                                remotesList.SelectedIndex = 0;
                            }));
                            currentRemote = "Public";
                            UsingPublicConfig = true;
                        }
                        return;
                    }

                    // Wait before retry (exponential backoff: 500ms, 1s, 2s, 4s, ...)
                    await Task.Delay(Math.Min(500 * (int)Math.Pow(2, retryCount - 1), 5000));
                }
                catch (Exception ex)
                {
                    Logger.Log($"Unexpected error initializing mirrors: {ex.Message}", LogLevel.ERROR);
                    _ = FlexibleMessageBox.Show(Program.form,
                        $"Error initializing mirrors: {ex.Message}",
                        "Mirror Initialization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            if (mirrors == null)
            {
                Logger.Log("Failed to retrieve mirrors list", LogLevel.ERROR);
                return;
            }

            _ = Logger.Log("Loaded following mirrors: ");
            int itemsCount = 0;

            if (hasPublicConfig)
            {
                _ = remotesList.Items.Add("Public");
                itemsCount++;
            }

            // Parse the config to find which remotes are top-level (not
            // referenced as dependencies by other remotes).  Helper remotes
            // like [localfs] that only exist as building blocks are excluded.
            var topLevel = GetTopLevelRemotes();

            foreach (string mirror in mirrors)
            {
                string name = mirror.TrimEnd(':', ' ', '\r', '\n');
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                // When we successfully parsed the config, skip helpers
                if (topLevel.Count > 0 && !topLevel.Contains(name))
                {
                    _ = Logger.Log($"Skipping helper remote: {name}");
                    continue;
                }

                _ = Logger.Log(name);
                await Task.Run(() => remotesList.Invoke(() =>
                {
                    _ = remotesList.Items.Add(name);
                }));
                itemsCount++;
            }

            if (itemsCount > 0)
            {
                await Task.Run(() => remotesList.Invoke(() =>
                {
                    // Suppress SelectedIndexChanged — we set currentRemote ourselves below
                    _suppressMirrorRefresh = true;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(settings.selectedMirror))
                        {
                            int i = remotesList.Items.IndexOf(settings.selectedMirror);
                            if (i >= 0)
                                remotesList.SelectedIndex = i;
                            else
                                remotesList.SelectedIndex = 0;
                        }

                        if (remotesList.SelectedIndex < 0 && remotesList.Items.Count > 0)
                        {
                            remotesList.SelectedIndex = 0;
                        }

                        string selectedRemote = remotesList.SelectedItem.ToString();
                        if (selectedRemote == "Public")
                        {
                            UsingPublicConfig = true;
                        }
                        else
                        {
                            UsingPublicConfig = false;
                            currentRemote = selectedRemote;
                        }
                    }
                    finally
                    {
                        _suppressMirrorRefresh = false;
                    }
                }));
            }
        }

        public static string processError = string.Empty;
        public static string currentRemote = string.Empty;
        private readonly string wrDelimiter = "-------";

        private void deviceDropContainer_Click(object sender, EventArgs e)
        {
            ToggleContainer(deviceDropContainer, deviceDrop);
        }

        private void sideloadContainer_Click(object sender, EventArgs e)
        {
            ToggleContainer(sideloadContainer, sideloadDrop);
        }

        private void installedAppsMenuContainer_Click(object sender, EventArgs e)
        {
            ToggleContainer(installedAppsMenuContainer, installedAppsMenu);
        }

        private void backupDrop_Click(object sender, EventArgs e)
        {
            ToggleContainer(backupContainer, backupDrop);
        }

        private void otherDrop_Click(object sender, EventArgs e)
        {
            ToggleContainer(otherContainer, otherDrop);
        }

        private async void AnimateContainerHeight(Panel container, bool expand)
        {
            // Disable AutoSize during animation
            container.AutoSize = false;

            // Store the target height before any changes
            int targetHeight = expand ? container.PreferredSize.Height : 0;
            int startHeight = expand ? 0 : container.Height;

            // For collapsing: hide immediately if already at 0
            if (!expand && container.Height == 0)
            {
                container.Visible = false;
                container.AutoSize = true;
                return;
            }

            // Set height before making visible to prevent flicker on expand
            container.Height = startHeight;

            // Only show if expanding (collapsing container is already visible)
            if (expand)
            {
                container.Visible = true;
            }

            // Suspend layout to prevent child controls from flickering
            container.SuspendLayout();

            // Stopwatch for consistent timing, 1.5ms per pixel height
            int durationMs = (int)Math.Round(container.PreferredSize.Height * 1.5);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.ElapsedMilliseconds < durationMs)
            {
                float progress = (float)stopwatch.ElapsedMilliseconds / durationMs;
                progress = Math.Min(1f, progress);

                // Ease-out curve
                float easedProgress = 1f - (1f - progress) * (1f - progress);

                int newHeight = (int)(startHeight + (targetHeight - startHeight) * easedProgress);
                container.Height = Math.Max(0, newHeight);

                // Yield to UI thread, but don't rely on delay accuracy
                await Task.Delay(1);
            }

            // Ensure final state
            container.Height = targetHeight;

            // Resume layout before hiding to ensure clean state
            container.ResumeLayout(false);

            if (!expand)
            {
                container.Visible = false;
            }

            container.AutoSize = true;
        }

        private void CollapseAllContainersInstant(Panel exceptThis = null)
        {
            var containers = new[]
            {
        deviceDropContainer,
        sideloadContainer,
        installedAppsMenuContainer,
        backupContainer,
        otherContainer
    };

            foreach (var container in containers)
            {
                if (container != exceptThis && container.Visible)
                {
                    // Hide before any layout changes to prevent flicker
                    container.Visible = false;
                    container.SuspendLayout();
                    container.AutoSize = false;
                    container.Height = 0;
                    container.ResumeLayout(false);
                    container.AutoSize = true;
                }
            }
        }

        private void ToggleContainer(Panel containerToToggle, Button dropButton)
        {
            // Collapse all other containers instantly
            CollapseAllContainersInstant(containerToToggle);

            // Check if we're collapsing (container is currently visible and has height)
            bool isExpanding = !containerToToggle.Visible || containerToToggle.Height == 0;

            if (isExpanding)
            {
                // Animate expansion
                AnimateContainerHeight(containerToToggle, true);
            }
            else
            {
                // Close instantly without animation - hide before any layout changes
                containerToToggle.Visible = false;
                containerToToggle.SuspendLayout();
                containerToToggle.AutoSize = false;
                containerToToggle.Height = 0;
                containerToToggle.ResumeLayout(false);
                containerToToggle.AutoSize = true;
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show(Program.form);
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            string about = $@"
    VERSION {Updater.LocalVersion}

    THIS SOFTWARE IS FREE
    HTTPS://GITHUB.COM/{repo.ToUpper()}

    CREDITS & ACKNOWLEDGEMENTS
    —————————————————
    • SOFTWARE ORIGINALLY DEVELOPED BY ROOKIE.WTF
    • SPECIAL THANKS TO FENOPY, MAXINE, JARJARBLINKZ, PMOW, SYTHEZN, FLOW, 
       IVAN, KALADIN, HARRYEFFINPOTTER, JOHN, SAM HOQUE, JP AND ALL CONTRIBUTORS

    ADDITIONAL THANKS & RESOURCES
    —————————————————
    • RCLONE — HTTPS://RCLONE.ORG
    • 7-ZIP — HTTPS://WWW.7-ZIP.ORG
    • ERIKE — HTTPS://STACKOVERFLOW.COM/USERS/57611/ERIKE
    • SERGE WEINSTOCK — SERGEUTILS
    • MIKE GOLD — HTTPS://WWW.C-SHARPCORNER.COM/MEMBERS/MIKE-GOLD2
    ";
            _ = FlexibleMessageBox.Show(Program.form, about);
        }

        private async void listApkButton_Click(object sender, EventArgs e)
        {
            string titleMessage = "Refreshing devices, apps and update list...";
            changeTitle(titleMessage);
            if (isLoading) { return; }
            isLoading = true;

            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Refreshing";
            devicesbutton_Click(sender, e);

            await initMirrors();

            isLoading = false;
            await refreshCurrentMirror(titleMessage);
        }

        private async Task refreshCurrentMirror(string titleMessage)
        {
            changeTitle(titleMessage);
            if (isLoading) { return; }
            isLoading = true;
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Refreshing";

            try
            {
                Thread t1 = new Thread(() =>
                {
                    try
                    {
                        if (!UsingPublicConfig)
                        {
                            SideloaderRCLONE.initGames(currentRemote);
                        }
                        listAppsBtn();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error during mirror refresh: {ex.Message}", LogLevel.ERROR);
                    }
                })
                {
                    IsBackground = true
                };
                t1.Start();
                while (t1.IsAlive)
                {
                    await Task.Delay(100);
                }
            }
            finally
            {
                isLoading = false;
            }

            // Use RefreshGameListAsync to preserve filter state
            await RefreshGameListAsync();
            changeTitle("");
        }

        private static readonly HttpClient client = new HttpClient();
        public static bool reset = false;
        public static bool updatedConfig = false;
        public static int steps = 0;
        public static bool gamesAreDownloading = false;
        private ModernQueuePanel _queuePanel;
        private readonly BindingList<string> gamesQueueList = new BindingList<string>();
        public static int quotaTries = 0;
        public static bool timerticked = false;
        public static bool skiponceafterremove = false;


        public bool SwitchMirrors()
        {
            bool success = true;
            try
            {
                quotaTries++;

                // If we've tried all remotes, show quota exceeded error
                if (quotaTries > remotesList.Items.Count)
                {
                    ShowError_QuotaExceeded();

                    // If the user provided a new config, quotaTries was reset to 0 — let the caller retry
                    if (quotaTries == 0)
                        return true;

                    // User cancelled or chose offline — go offline
                    isOffline = true;
                    return false;
                }

                remotesList.Invoke((MethodInvoker)delegate
                {
                    _suppressMirrorRefresh = true;
                    try
                    {
                        int count = remotesList.Items.Count;
                        if (count == 0) return;

                        // Simple round-robin: move to next mirror
                        int nextIndex = (remotesList.SelectedIndex + 1) % count;
                        remotesList.SelectedIndex = nextIndex;

                        string selected = remotesList.SelectedItem?.ToString();
                        if (selected == "Public")
                        {
                            UsingPublicConfig = true;
                        }
                        else
                        {
                            UsingPublicConfig = false;
                            currentRemote = selected;
                        }
                    }
                    finally
                    {
                        _suppressMirrorRefresh = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in SwitchMirrors: {ex.Message}", LogLevel.ERROR);
                success = false;
            }

            return success;
        }

        private void ShowError_QuotaExceeded()
        {
            // Show the startup dialog so the user can reconfigure or go offline.
            // Use Invoke to ensure we're on the UI thread (SwitchMirrors may be
            // called from a background thread).
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ShowError_QuotaExceeded));
                return;
            }

            using (var startupDialog = new StartupDialog("Config server is not reachable — check URL and your connection"))
            {
                var dialogResult = startupDialog.ShowDialog(this);

                if (dialogResult != DialogResult.OK || startupDialog.Choice == StartupDialog.StartupChoice.None)
                {
                    // User cancelled — caller will set isOffline = true
                    return;
                }

                if (startupDialog.Choice == StartupDialog.StartupChoice.Offline)
                {
                    // User explicitly chose offline — caller will set isOffline = true
                    return;
                }

                if (startupDialog.Choice == StartupDialog.StartupChoice.RcloneConfig)
                {
                    // User provided rclone config — will be picked up on next mirror init
                    quotaTries = 0;
                    return;
                }

                if (startupDialog.Choice == StartupDialog.StartupChoice.Online)
                {
                    // User provided a new config — reload it
                    try
                    {
                        string configFilePath = Path.Combine(Environment.CurrentDirectory, "public.json");
                        if (File.Exists(configFilePath))
                        {
                            string configFileData = File.ReadAllText(configFilePath);
                            PublicConfig config = JsonConvert.DeserializeObject<PublicConfig>(configFileData);

                            if (config != null &&
                                !string.IsNullOrWhiteSpace(config.BaseUri) &&
                                !string.IsNullOrWhiteSpace(config.Password))
                            {
                                PublicConfigFile = config;
                                hasPublicConfig = true;
                                DnsHelper.TestPublicConfigDns();
                                quotaTries = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to reload config after quota error: {ex.Message}", LogLevel.ERROR);
                    }
                }
            }
        }

        public async void cleanupActiveDownloadStatus()
        {
            speedLabel.Text = String.Empty;
            progressBar.Value = 0;
            _pausedProgressPercent = 0;
            _pausedTransfersComplete = 0;
            _pausedFileCount = 0;

            if (gamesQueueList.Count > 0)
            {
                string removedGame = gamesQueueList[0];

                // Subtract effective size from running total
                if (_queueEffectiveSizes.TryGetValue(removedGame, out double effectiveSize))
                {
                    _effectiveQueueSizeMB -= effectiveSize;
                    _queueEffectiveSizes.Remove(removedGame);
                }

                gamesQueueList.RemoveAt(0);
            }

            _queuePanel?.Invalidate();

            UpdateQueueLabel();

            // Save updated queue state
            SaveQueueToSettings();
        }

        public void SetProgress(float progress)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => progressBar.Value = progress));
            }
            else
            {
                progressBar.Value = progress;
            }
        }

        public bool isinstalling = false;
        public static bool isInDownloadExtract = false;
        public static bool removedownloading = false;
        private bool _downloadPaused;
        private float _pausedProgressPercent;
        private long _pausedTransfersComplete;
        private long _pausedFileCount;
        private TaskCompletionSource<bool> _pauseTcs;
        public async void downloadInstallGameButton_Click(object sender, EventArgs e)
        {
            // Helper to format sizes
            string FormatSize(double mb)
            {
                return mb >= 1024 ? $"{(mb / 1024.0):F2} GB" : $"{mb:F0} MB";
            }

            if (!settings.CustomDownloadDir)
            {
                settings.DownloadDir = Environment.CurrentDirectory.ToString();
            }
            bool obbsMismatch = false;
            if (nodeviceonstart && !updatesNotified)
            {
                _ = await CheckForDevice();
                changeTitlebarToDevice();
                showAvailableSpace();
                listAppsBtn();
            }
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Downloading";

            // Check if we're resuming (queue already has items) or starting fresh
            bool isResuming = gamesQueueList.Count > 0 && gamesAreDownloading == false;

            string[] gamesToDownload;

            if (!isResuming)
            {
                // Normal flow: add selected games to queue
                if (gamesListView.SelectedItems.Count == 0)
                {
                    progressBar.IsIndeterminate = false;
                    changeTitle("You must select a game from the game list!");
                    return;
                }

                int count = gamesListView.SelectedItems.Count;
                gamesToDownload = new string[count];
                for (int i = 0; i < count; i++)
                {
                    gamesToDownload[i] = gamesListView.SelectedItems[i].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                }
            }
            else
            {
                // Resuming: just recalculate tracking values
                progressBar.OperationType = "Validating";
                changeTitle("Validating resumed queue...");

                // Refresh device space info
                if (!settings.NodeviceMode && DeviceConnected)
                {
                    showAvailableSpace();
                    await Task.Delay(500);
                }

                // Recalculate queue sizes
                _queueEffectiveSizes.Clear();
                _effectiveQueueSizeMB = 0;
                _totalQueueSizeMB = 0;

                foreach (string releaseName in gamesQueueList)
                {
                    foreach (string[] gameData in SideloaderRCLONE.games)
                    {
                        if (gameData.Length > SideloaderRCLONE.ReleaseNameIndex &&
                            gameData[SideloaderRCLONE.ReleaseNameIndex].Equals(releaseName, StringComparison.OrdinalIgnoreCase))
                        {
                            double sizeMB = 0;
                            if (gameData.Length > 5)
                                StringUtilities.TryParseDouble(gameData[5], out sizeMB);
                            _queueEffectiveSizes[releaseName] = sizeMB;
                            _effectiveQueueSizeMB += sizeMB;
                            _totalQueueSizeMB += sizeMB;
                            break;
                        }
                    }
                }

                gamesToDownload = new string[0]; // Skip validation loop for resume
            }

            progressBar.Value = 0;
            progressBar.IsIndeterminate = false;
            isinstalling = true;

            // Validation for new downloads
            List<string> skippedDuplicates = new List<string>();
            string spaceError = null;

            // Get available space on download drive
            string downloadPath = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
            long downloadDirFreeMB = -1;
            try
            {
                DriveInfo drive = new DriveInfo(Path.GetPathRoot(downloadPath));
                if (drive.IsReady)
                    downloadDirFreeMB = drive.AvailableFreeSpace / (1024 * 1024);
            }
            catch { }

            // Track max game size for disk space calculation
            double maxQueuedGameSizeMB = 0;
            foreach (string queuedGame in gamesQueueList)
            {
                foreach (string[] gameData in SideloaderRCLONE.games)
                {
                    if (gameData.Length > SideloaderRCLONE.ReleaseNameIndex &&
                        gameData[SideloaderRCLONE.ReleaseNameIndex].Equals(queuedGame, StringComparison.OrdinalIgnoreCase))
                    {
                        if (gameData.Length > 5 && StringUtilities.TryParseDouble(gameData[5], out double sizeMB))
                            maxQueuedGameSizeMB = Math.Max(maxQueuedGameSizeMB, sizeMB);
                        break;
                    }
                }
            }

            for (int i = 0; i < gamesToDownload.Length; i++)
            {
                string releaseName = gamesToDownload[i];

                // Skip duplicates
                if (gamesQueueList.Contains(releaseName))
                {
                    skippedDuplicates.Add(Sideloader.gameNameToSimpleName(releaseName));
                    continue;
                }

                // Get game metadata
                double gameSizeMB = 0;
                string packagename = null;
                foreach (string[] gameData in SideloaderRCLONE.games)
                {
                    if (gameData.Length > SideloaderRCLONE.ReleaseNameIndex &&
                        gameData[SideloaderRCLONE.ReleaseNameIndex].Equals(releaseName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (gameData.Length > 5)
                            StringUtilities.TryParseDouble(gameData[5], out gameSizeMB);
                        if (gameData.Length > SideloaderRCLONE.PackageNameIndex)
                            packagename = gameData[SideloaderRCLONE.PackageNameIndex];
                        break;
                    }
                }

                string gameDisplayName = Sideloader.gameNameToSimpleName(releaseName);

                // Check disk storage (skip already downloaded content)
                if (downloadDirFreeMB > 0 && gameSizeMB > 0 && !downloadedFilter_Clicked)
                {
                    double largestGame = Math.Max(maxQueuedGameSizeMB, gameSizeMB);
                    double requiredMB;

                    if (settings.DeleteAllAfterInstall && !settings.NodeviceMode && DeviceConnected)
                        requiredMB = largestGame * 2.2;
                    else
                    {
                        double otherGamesSize = _totalQueueSizeMB + gameSizeMB - largestGame;
                        requiredMB = otherGamesSize + (largestGame * 2.2);
                    }

                    if (downloadDirFreeMB < requiredMB)
                    {
                        string driveLetter = Path.GetPathRoot(downloadPath);
                        spaceError = $"Not enough disk space on {driveLetter} for \"{gameDisplayName}\"\n\n" +
                                    $"Available: {(downloadDirFreeMB / 1024.0):F2} GB\n" +
                                    $"Required: {(requiredMB / 1024.0):F2} GB\n\n" +
                                    $"Free up space or change download directory in Settings.";
                        break;
                    }
                }

                // Calculate effective device space
                // Also account for updates replacing existing installs
                double effectiveRequiredMB = gameSizeMB;

                if (!settings.NodeviceMode && DeviceConnected && !string.IsNullOrEmpty(packagename) &&
                    settings.InstalledApps.Contains(packagename))
                {
                    try
                    {
                        long installedSizeKB = 0;

                        // Get installed APK size
                        string pmPathOutput = ADB.RunAdbCommandToString($"shell pm path {packagename}", suppressLogging: true).Output;
                        foreach (string line in pmPathOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (line.StartsWith("package:"))
                            {
                                string apkPath = line.Substring(8).Trim();
                                if (!string.IsNullOrEmpty(apkPath))
                                {
                                    string statOutput = ADB.RunAdbCommandToString($"shell stat -c %s \"{apkPath}\"", suppressLogging: true).Output.Trim();
                                    if (long.TryParse(statOutput, out long apkBytes))
                                        installedSizeKB += apkBytes / 1024;
                                }
                            }
                        }

                        // Get installed OBB size
                        string obbOutput = ADB.RunAdbCommandToString($"shell du -s /sdcard/Android/obb/{packagename} 2>/dev/null", suppressLogging: true).Output.Trim();
                        if (!string.IsNullOrEmpty(obbOutput) && !obbOutput.Contains("No such file"))
                        {
                            string[] parts = obbOutput.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 1 && long.TryParse(parts[0], out long obbKB))
                                installedSizeKB += obbKB;
                        }

                        if (installedSizeKB > 0)
                        {
                            double installedSizeMB = installedSizeKB / 1024.0;
                            effectiveRequiredMB = Math.Max(0, gameSizeMB - installedSizeMB);

                            Logger.Log($"Updating {packagename}: installed: {installedSizeMB:F0}MB, " +
                                        $"new: {gameSizeMB:F0}MB, effective required: {effectiveRequiredMB:F0}MB",
                                        LogLevel.DEBUG);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error getting installed size for {packagename}: {ex.Message}", LogLevel.WARNING);
                        effectiveRequiredMB = gameSizeMB;
                    }
                }

                // Check device space
                if (_deviceFreeSpaceMB > 0 && gameSizeMB > 0 && !settings.NodeviceMode && DeviceConnected)
                {
                    double totalDeviceRequiredMB = _effectiveQueueSizeMB + effectiveRequiredMB;

                    if (_deviceFreeSpaceMB < totalDeviceRequiredMB)
                    {
                        double neededMB = totalDeviceRequiredMB - _deviceFreeSpaceMB;

                        string availableText = FormatSize(_deviceFreeSpaceMB);
                        string gameText = FormatSize(effectiveRequiredMB);
                        string queueText = FormatSize(_effectiveQueueSizeMB);
                        string neededText = FormatSize(neededMB);

                        string details;
                        if (_effectiveQueueSizeMB > 0)
                        {
                            string totalRequiredText = FormatSize(totalDeviceRequiredMB);
                            details = $"Available: {availableText}\n" +
                                        $"Required: {totalRequiredText} (Game: {gameText}, Queue: {queueText})";
                        }
                        else
                        {
                            details = $"Available: {availableText}\n" +
                                        $"Required: {gameText}";
                        }

                        spaceError = $"Not enough space on your Quest for \"{gameDisplayName}\"\n\n" +
                                    $"{details}\n\n" +
                                    $"Free up {neededText} on your Quest, or disable sideloading to download only.";
                        break;
                    }
                }

                // Add to queue
                gamesQueueList.Add(releaseName);
                _queueEffectiveSizes[releaseName] = effectiveRequiredMB;
                _effectiveQueueSizeMB += effectiveRequiredMB;
                _totalQueueSizeMB += gameSizeMB;
                maxQueuedGameSizeMB = Math.Max(maxQueuedGameSizeMB, gameSizeMB);
            }

            // Handle validation results for new downloads
            if (spaceError != null)
            {
                FlexibleMessageBox.Show(Program.form, spaceError, "Not Enough Space", MessageBoxButtons.OK);
            }

            if (skippedDuplicates.Count > 0)
            {
                string msg = skippedDuplicates.Count == 1
                    ? $"Skipped: {skippedDuplicates[0]} (already in queue)"
                    : $"Skipped {skippedDuplicates.Count} games (already in queue)";
                changeTitle(msg, true);
            }

            if (gamesQueueList.Count == 0)
            {
                progressBar.IsIndeterminate = false;
                isinstalling = false;
                gamesAreDownloading = false;
                if (_queuePanel != null) _queuePanel.IsDownloading = false;
                return;
            }

            if (gamesAreDownloading && !isResuming)
                return;

            UpdateQueueLabel();
            changeTitle("");

            gamesAreDownloading = true;
            if (_queuePanel != null) _queuePanel.IsDownloading = true;

            //Do user json on firsttime
            if (settings.UserJsonOnGameInstall)
            {
                Thread userJsonThread = new Thread(() => { changeTitle("Pushing user.json"); Sideloader.PushUserJsons(); })
                {
                    IsBackground = true
                };
                userJsonThread.Start();

            }

            ProcessOutput output = new ProcessOutput("", "");

            string gameName = "";
            while (gamesQueueList.Count > 0)
            {
                gameName = gamesQueueList.ToArray()[0];
                string packagename = Sideloader.gameNameToPackageName(gameName);
                string gameDisplayName = Sideloader.gameNameToSimpleName(gameName);
                string versioncode = Sideloader.gameNameToVersionCode(gameName);
                string dir = Path.GetDirectoryName(gameName);
                string gameDirectory = Path.Combine(settings.DownloadDir, gameName);
                string downloadDirectory = Path.Combine(settings.DownloadDir, gameName);
                string path = gameDirectory;

                // Check disk space before starting this download
                double currentGameSizeMB = 0;
                foreach (string[] gameData in SideloaderRCLONE.games)
                {
                    if (gameData.Length > SideloaderRCLONE.ReleaseNameIndex &&
                        gameData[SideloaderRCLONE.ReleaseNameIndex].Equals(gameName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (gameData.Length > 5)
                            StringUtilities.TryParseDouble(gameData[5], out currentGameSizeMB);
                        break;
                    }
                }

                if (currentGameSizeMB > 0)
                {
                    // Check disk space
                    long currentDiskFreeMB = -1;
                    try
                    {
                        DriveInfo drive = new DriveInfo(Path.GetPathRoot(downloadPath));
                        if (drive.IsReady)
                            currentDiskFreeMB = drive.AvailableFreeSpace / (1024 * 1024);
                    }
                    catch { }

                    double requiredDiskMB = currentGameSizeMB * 2.2;
                    if (!downloadedFilter_Clicked && currentDiskFreeMB > 0 && currentDiskFreeMB < requiredDiskMB)
                    {
                        string driveLetter = Path.GetPathRoot(downloadPath);

                        DialogResult result = FlexibleMessageBox.Show(Program.form,
                            $"Not enough disk space on {driveLetter} for \"{gameDisplayName}\"\n\n" +
                            $"Available: {FormatSize(currentDiskFreeMB)}\n" +
                            $"Required: {FormatSize(requiredDiskMB)}\n\n" +
                            $"Yes = Skip this game and continue\n" +
                            $"No = Clear entire queue",
                            "Not Enough Space",
                            MessageBoxButtons.YesNo);

                        if (result == DialogResult.Yes || result == DialogResult.Cancel)
                        {
                            // Skip this game
                            if (_queueEffectiveSizes.TryGetValue(gameName, out double effectiveSize))
                            {
                                _effectiveQueueSizeMB -= effectiveSize;
                                _queueEffectiveSizes.Remove(gameName);
                            }
                            gamesQueueList.RemoveAt(0);
                            SaveQueueToSettings();
                            continue;
                        }
                        else
                        {
                            // Clear queue
                            gamesQueueList.Clear();
                            _queueEffectiveSizes.Clear();
                            _effectiveQueueSizeMB = 0;
                            _totalQueueSizeMB = 0;
                            SaveQueueToSettings();
                            gamesAreDownloading = false;
                            if (_queuePanel != null) _queuePanel.IsDownloading = false;
                            isinstalling = false;
                            changeTitle("");
                            return;
                        }
                    }

                    // Check Quest space (only if device connected and sideloading enabled)
                    if (!settings.NodeviceMode && DeviceConnected && _deviceFreeSpaceMB > 0)
                    {
                        double effectiveRequiredMB = currentGameSizeMB;

                        // Check if this is an update
                        if (!string.IsNullOrEmpty(packagename) && settings.InstalledApps.Contains(packagename))
                        {
                            if (_queueEffectiveSizes.TryGetValue(gameName, out double cached))
                            {
                                effectiveRequiredMB = cached;
                            }
                        }

                        if (_deviceFreeSpaceMB < effectiveRequiredMB)
                        {
                            DialogResult result = FlexibleMessageBox.Show(Program.form,
                                $"Not enough space on your Quest for \"{gameDisplayName}\"\n\n" +
                                $"Available: {FormatSize(_deviceFreeSpaceMB)}\n" +
                                $"Required: {FormatSize(effectiveRequiredMB)}\n\n" +
                                $"Yes = Skip this game and continue\n" +
                                $"No = Clear entire queue",
                                "Not Enough Space",
                                MessageBoxButtons.YesNo);

                            if (result == DialogResult.Yes || result == DialogResult.Cancel)
                            {
                                // Skip this game
                                if (_queueEffectiveSizes.TryGetValue(gameName, out double effectiveSize))
                                {
                                    _effectiveQueueSizeMB -= effectiveSize;
                                    _queueEffectiveSizes.Remove(gameName);
                                }
                                gamesQueueList.RemoveAt(0);
                                SaveQueueToSettings();
                                continue;
                            }
                            else
                            {
                                // Clear queue
                                gamesQueueList.Clear();
                                _queueEffectiveSizes.Clear();
                                _effectiveQueueSizeMB = 0;
                                _totalQueueSizeMB = 0;
                                SaveQueueToSettings();
                                gamesAreDownloading = false;
                                if (_queuePanel != null) _queuePanel.IsDownloading = false;
                                isinstalling = false;
                                changeTitle("");
                                return;
                            }
                        }
                    }
                }

                // Download/Offline Filter View: verify downloaded files exist before proceeding
                if (downloadedFilter_Clicked || isOffline)
                {
                    string localGameDir = Path.Combine(settings.DownloadDir, gameName);
                    if (!Directory.Exists(localGameDir) || Directory.GetFiles(localGameDir, "*.apk").Length == 0)
                    {
                        changeTitle($"No downloaded files found for {gameDisplayName}");
                        cleanupActiveDownloadStatus();
                        continue;
                    }
                }

                string gameNameHash = string.Empty;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(gameName + "\n");
                    byte[] hash = md5.ComputeHash(bytes);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        _ = sb.Append(b.ToString("x2"));
                    }

                    gameNameHash = sb.ToString();
                }

                ProcessOutput gameDownloadOutput = new ProcessOutput("", "");

                _ = Logger.Log($"Starting Game Download");

                Thread t1;
                string extraArgs = string.Empty;
                if (settings.SingleThreadMode)
                {
                    extraArgs = "--transfers 1 --multi-thread-streams 0";
                }
                string bandwidthLimit = string.Empty;
                if (settings.BandwidthLimit > 0)
                {
                    bandwidthLimit = $"--bwlimit={settings.BandwidthLimit}M";
                }
                if (downloadedFilter_Clicked || isOffline)
                {
                    // Downloaded/Offline View: skip download, install from downloaded files
                    t1 = new Thread(() => { gameDownloadOutput = new ProcessOutput("Download skipped."); });
                }
                else if (UsingPublicConfig)
                {
                    bool doDownload = true;
                    bool skipRedownload = false;
                    if (settings.UseDownloadedFiles == true)
                    {
                        skipRedownload = true;
                    }

                    if (Directory.Exists(gameDirectory))
                    {
                        if (skipRedownload == true)
                        {
                            if (Directory.Exists($"{settings.DownloadDir}\\{gameName}"))
                            {
                                doDownload = false;
                            }
                        }
                        else
                        {
                            DialogResult res = FlexibleMessageBox.Show(Program.form,
                                $"{gameName} already exists in destination directory.\n\n" +
                                "Yes = Overwrite and re-download.\n" +
                                "No  = Use existing files and install from them.",
                                "Download again?", MessageBoxButtons.YesNo);

                            doDownload = res == DialogResult.Yes;
                        }

                        if (doDownload)
                        {
                            // only delete after extraction; allows for resume if the fetch fails midway.
                            if (Directory.Exists($"{settings.DownloadDir}\\{gameName}"))
                            {
                                FileSystemUtilities.TryDeleteDirectory($"{settings.DownloadDir}\\{gameName}");
                            }
                        }
                    }

                    if (doDownload)
                    {
                        downloadDirectory = $"{settings.DownloadDir}\\{gameNameHash}";
                        _ = Logger.Log($"rclone copy \"Public:{SideloaderRCLONE.RcloneGamesFolder}/{gameName}\"");
                        t1 = new Thread(() =>
                        {
                            string rclonecommand =
                            $"copy \":http:/{gameNameHash}/\" \"{downloadDirectory}\" {extraArgs} --progress --rc {bandwidthLimit}";
                            gameDownloadOutput = RCLONE.runRcloneCommand_PublicConfig(rclonecommand);
                        });
                    }
                    else
                    {
                        t1 = new Thread(() => { gameDownloadOutput = new ProcessOutput("Download skipped."); });
                    }
                }
                else
                {
                    _ = Directory.CreateDirectory(gameDirectory);
                    downloadDirectory = $"{SideloaderRCLONE.RcloneGamesFolder}/{gameName}";
                    _ = Logger.Log($"rclone copy \"{currentRemote}:{downloadDirectory}\"");
                    t1 = new Thread(() =>
                    {
                        gameDownloadOutput = RCLONE.runRcloneCommand_DownloadConfig($"copy \"{currentRemote}:{downloadDirectory}\" \"{settings.DownloadDir}\\{gameName}\" {extraArgs} --progress --rc --retries 2 --low-level-retries 1 --check-first {bandwidthLimit}");
                    });
                }

                if (Directory.Exists(downloadDirectory))
                {
                    string[] partialFiles = Directory.GetFiles($"{downloadDirectory}", "*.partial");
                    foreach (string file in partialFiles)
                    {
                        File.Delete(file);
                        _ = Logger.Log($"Deleted partial file: {file}");
                    }
                }

                t1.IsBackground = true;
                t1.Start();

                if (!downloadedFilter_Clicked && !isOffline)
                {
                    bool isResume = _pausedProgressPercent > 0;
                    changeTitle((isResume ? "Resuming download " : "Downloading game ") + gameName);
                    speedLabel.Text = isResume ? "Resuming download..." : "Starting download...";
                }
                else
                {
                    changeTitle("Installing from downloaded files: " + gameDisplayName);
                    speedLabel.Text = "";
                }

                // Snapshot pause offsets for resume formulas (local copies prevent feedback loops)
                float progressOffset = _pausedProgressPercent;
                long transfersOffset = _pausedTransfersComplete;
                long totalOverride = _pausedFileCount;

                // Track the highest valid progress to prevent brief progress bar flashes during multi-file transfers
                float highestValidPercent = progressOffset;

                if (progressOffset > 0)
                {
                    progressBar.Value = progressOffset;
                }

                // Download
                while (t1.IsAlive)
                {
                    try
                    {
                        HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:5572/core/stats", null);
                        string foo = await response.Content.ReadAsStringAsync();
                        dynamic results = JsonConvert.DeserializeObject<dynamic>(foo);

                        if (results["transferring"] != null)
                        {
                            double totalSize = 0;
                            double downloadedSize = 0;
                            long fileCount = 0;
                            long transfersComplete = 0;
                            long totalChecks = 0;
                            long globalEta = 0;
                            float speed = 0;
                            float downloadSpeed = 0;
                            double estimatedFileCount = 0;

                            totalSize = results["totalBytes"];
                            downloadedSize = results["bytes"];
                            fileCount = results["totalTransfers"];
                            totalChecks = results["totalChecks"];
                            transfersComplete = results["transfers"];
                            globalEta = results["eta"];
                            speed = results["speed"];
                            estimatedFileCount = Math.Ceiling(totalSize / 524288000); // maximum part size

                            if (totalChecks > fileCount)
                            {
                                fileCount = totalChecks;
                            }
                            if (estimatedFileCount > fileCount)
                            {
                                fileCount = (long)estimatedFileCount;
                            }

                            downloadSpeed = speed / 1000000;
                            totalSize /= 1000000;
                            downloadedSize /= 1000000;

                            progressBar.IsIndeterminate = false;

                            float percent = 0;
                            if (totalSize > 0)
                            {
                                percent = (float)(downloadedSize / totalSize * 100);
                                // After resume, rclone reports 0-100% of remaining work.
                                // Map that onto [pausedPercent → 100%] so the bar continues from where it was.
                                if (progressOffset > 0)
                                    percent = progressOffset + (100f - progressOffset) * percent / 100f;
                            }

                            // Clamp to 0-99 while download is in progress to prevent brief 100% flashes
                            percent = Math.Max(0, Math.Min(99, percent));

                            // Only allow progress to increase
                            if (percent >= highestValidPercent)
                            {
                                highestValidPercent = percent;
                            }
                            else
                            {
                                // Progress went backwards? Keep showing the highest valid percent we've seen
                                percent = highestValidPercent;
                            }

                            progressBar.Value = percent;

                            TimeSpan time = TimeSpan.FromSeconds(globalEta);

                            // Offset transfer counts for pause/resume continuity
                            long displayCurrent = transfersComplete + transfersOffset;
                            long displayTotal = totalOverride > 0 ? totalOverride : fileCount;

                            UpdateProgressStatus(
                                "Downloading",
                                (int)Math.Min(displayCurrent + 1, displayTotal),
                                (int)displayTotal,
                                (int)Math.Round(percent),
                                time,
                                downloadSpeed);

                            // Update fields with effective values for cross-session persistence
                            _pausedProgressPercent = highestValidPercent;
                            _pausedTransfersComplete = displayCurrent;
                            _pausedFileCount = displayTotal;
                        }
                    }
                    catch
                    {
                    }
                    await Task.Delay(100);
                }

                // Handle pause: wait for resume, then re-run the same download
                if (_downloadPaused)
                {
                    SaveQueueToSettings();
                    changeTitle($"Paused: {gameDisplayName}");
                    speedLabel.Text = "Download paused";
                    UpdateProgressStatus("Paused", 0, 0, (int)Math.Round(_pausedProgressPercent));
                    _pauseTcs = new TaskCompletionSource<bool>();
                    await _pauseTcs.Task;
                    _pauseTcs = null;
                    changeTitle($"Resuming: {gameDisplayName}");
                    continue; // Re-enter loop; same game is still at index 0, rclone resumes partial files
                }

                if (removedownloading)
                {
                    removedownloading = false;
                    _pausedProgressPercent = 0;
                    _pausedTransfersComplete = 0;
                    _pausedFileCount = 0;

                    // Store game info before removing from queue
                    string cancelledGame = gameName;
                    string cancelledHash = gameNameHash;

                    // Remove the cancelled item from queue
                    if (gamesQueueList.Count > 0)
                    {
                        gamesQueueList.RemoveAt(0);
                    }

                    // Reset progress UI
                    speedLabel.Text = String.Empty;
                    progressBar.Value = 0;

                    // Ask about keeping files
                    changeTitle("Keep game files?");
                    try
                    {
                        DialogResult res = FlexibleMessageBox.Show(
                            $"{cancelledGame} download was cancelled. Do you want to delete the partial files?\n\nClick NO to keep the files if you wish to resume your download later.",
                            "Delete Temporary Files?", MessageBoxButtons.YesNo);

                        if (res == DialogResult.Yes)
                        {
                            changeTitle("Deleting game files...");
                            if (UsingPublicConfig)
                            {
                                if (Directory.Exists($"{settings.DownloadDir}\\{cancelledHash}"))
                                    FileSystemUtilities.TryDeleteDirectory($"{settings.DownloadDir}\\{cancelledHash}");
                                if (Directory.Exists($"{settings.DownloadDir}\\{cancelledGame}"))
                                    FileSystemUtilities.TryDeleteDirectory($"{settings.DownloadDir}\\{cancelledGame}");
                            }
                            else
                            {
                                if (Directory.Exists($"{settings.DownloadDir}\\{cancelledGame}"))
                                    FileSystemUtilities.TryDeleteDirectory($"{settings.DownloadDir}\\{cancelledGame}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = FlexibleMessageBox.Show(Program.form, $"Error deleting game files: {ex.Message}");
                    }

                    changeTitle("");
                    continue; // Continue to next item in queue
                }
                {
                    //Quota Errors
                    bool isinstalltxt = false;
                    string installTxtPath = null;
                    bool quotaError = false;
                    bool otherError = false;
                    if (gameDownloadOutput.Error.Length > 0 && !isOffline)
                    {
                        string err = gameDownloadOutput.Error.ToLower();
                        err += gameDownloadOutput.Output.ToLower();
                        if ((err.Contains("quota") && err.Contains("exceeded")) || err.Contains("directory not found"))
                        {
                            quotaError = true;

                            SwitchMirrors();

                            // If user provided a new config, do a full reconnect
                            if (quotaTries == 0)
                            {
                                reset = false;
                                steps = 0;
                                isLoading = true;

                                RCLONE.killRclone();
                                await Task.Run(() => RCLONE.Init());
                                await initMirrors();

                                isLoading = false;
                            }

                            cleanupActiveDownloadStatus();
                        }
                        else if (!gameDownloadOutput.Error.Contains("Serving remote control on http://127.0.0.1:5572/"))
                        {
                            otherError = true;

                            //Remove current game
                            cleanupActiveDownloadStatus();

                            _ = FlexibleMessageBox.Show(Program.form, $"Rclone error: {gameDownloadOutput.Error}");
                            output += new ProcessOutput("", "Download Failed");
                        }
                    }

                    if (UsingPublicConfig && otherError == false && gameDownloadOutput.Output != "Download skipped.")
                    {
                        // ETA tracking for extraction
                        DateTime extractStart = DateTime.UtcNow;

                        Thread extractionThread = new Thread(() =>
                        {
                            Invoke(new Action(() =>
                            {
                                speedLabel.Text = "Extracting...";
                                progressBar.IsIndeterminate = false;
                                progressBar.Value = 0;
                                progressBar.OperationType = "Extracting";
                                isInDownloadExtract = true;
                            }));

                            // Set up extraction callback
                            Zip.ExtractionProgressCallback = (percent, eta) =>
                            {
                                this.Invoke(() =>
                                {
                                    progressBar.Value = percent;
                                    UpdateProgressStatus("Extracting", percent: (int)Math.Round(percent), eta: eta);

                                    progressBar.StatusText = $"Extracting · {percent:0.0}%";
                                });
                            };

                            try
                            {
                                changeTitle("Extracting " + gameName);
                                Zip.ExtractFile($"{settings.DownloadDir}\\{gameNameHash}\\{gameNameHash}.7z.001", $"{settings.DownloadDir}", PublicConfigFile.Password);
                                changeTitle("");
                            }
                            catch (ExtractionException ex)
                            {
                                Invoke(new Action(() =>
                                {
                                    cleanupActiveDownloadStatus();
                                }));
                                otherError = true;
                                this.Invoke(() => _ = FlexibleMessageBox.Show(Program.form, $"7zip error: {ex.Message}"));
                                output += new ProcessOutput("", "Extract Failed");
                            }
                            finally
                            {
                                // Clear callbacks
                                Zip.ExtractionProgressCallback = null;
                                Zip.ExtractionStatusCallback = null;
                            }
                        })
                        {
                            IsBackground = true
                        };
                        extractionThread.Start();

                        while (extractionThread.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        progressBar.StatusText = ""; // Clear status after extraction
                        speedLabel.Text = "";

                        if (Directory.Exists($"{settings.DownloadDir}\\{gameNameHash}"))
                        {
                            FileSystemUtilities.TryDeleteDirectory($"{settings.DownloadDir}\\{gameNameHash}");
                        }
                    }

                    if (quotaError == false && otherError == false)
                    {
                        ADB.DeviceID = GetDeviceID();
                        quotaTries = 0;
                        progressBar.Value = 0;
                        progressBar.IsIndeterminate = false;
                        changeTitle("Installing game APK " + gameName);
                        if (File.Exists(Path.Combine(settings.DownloadDir, gameName, "install.txt")))
                        {
                            isinstalltxt = true;
                            installTxtPath = Path.Combine(settings.DownloadDir, gameName, "install.txt");
                        }
                        else if (File.Exists(Path.Combine(settings.DownloadDir, gameName, "Install.txt")))
                        {
                            isinstalltxt = true;
                            installTxtPath = Path.Combine(settings.DownloadDir, gameName, "Install.txt");
                        }

                        string[] files = Directory.GetFiles(settings.DownloadDir + "\\" + gameName);

                        Debug.WriteLine("Game Folder is: " + settings.DownloadDir + "\\" + gameName);
                        Debug.WriteLine("FILES IN GAME FOLDER: ");

                        if (isinstalltxt)
                        {
                            // Only sideload if device is connected and sideloading not disabled
                            if (!settings.NodeviceMode && !nodeviceonstart && DeviceConnected)
                            {
                                Thread installtxtThread = new Thread(() =>
                                {
                                    output += Sideloader.RunADBCommandsFromFile(installTxtPath);
                                    changeTitle("");
                                });
                                installtxtThread.Start();
                                while (installtxtThread.IsAlive)
                                {
                                    await Task.Delay(100);
                                }
                            }
                            else
                            {
                                output.Output = "Complete (installation skipped).\n\nConnect a device or enable sideloading to install.";
                            }
                        }
                        else
                        {
                            // Only sideload if device is connected and sideloading not disabled
                            if (!settings.NodeviceMode && !nodeviceonstart && DeviceConnected)
                            {
                                // Find the APK file to install
                                string apkFile = files.FirstOrDefault(file => Path.GetExtension(file) == ".apk");

                                if (apkFile != null)
                                {
                                    CurrAPK = apkFile;
                                    CurrPCKG = packagename;
                                    System.Windows.Forms.Timer t = new System.Windows.Forms.Timer
                                    {
                                        Interval = 150000 // 150 seconds to fail
                                    };
                                    t.Tick += new EventHandler(timer_Tick4);
                                    t.Start();

                                    changeTitle($"Sideloading APK...");
                                    progressBar.IsIndeterminate = false;
                                    progressBar.OperationType = "Installing";
                                    progressBar.Value = 0;

                                    // Use async method with progress
                                    output += await ADB.SideloadWithProgressAsync(
                                        apkFile,
                                        (progress, eta) => this.Invoke(() =>
                                        {
                                            if (progress == 0)
                                            {
                                                progressBar.IsIndeterminate = true;
                                                progressBar.OperationType = "Installing";
                                            }
                                            else
                                            {
                                                progressBar.IsIndeterminate = false;
                                                progressBar.Value = progress;
                                            }
                                            UpdateProgressStatus("Installing", percent: (int)Math.Round(progress), eta: eta);
                                            progressBar.StatusText = $"Installing · {progress:0.0}%";
                                        }),
                                        status => this.Invoke(() =>
                                        {
                                            if (!string.IsNullOrEmpty(status))
                                            {
                                                if (status.Contains("Completing Installation"))
                                                {
                                                    // "Completing Installation..."
                                                    speedLabel.Text = status;
                                                }
                                                progressBar.StatusText = status;
                                            }
                                        }),
                                        packagename,
                                        Sideloader.gameNameToSimpleName(gameName));

                                    t.Stop();
                                    progressBar.IsIndeterminate = false;
                                    progressBar.StatusText = ""; // Clear status after APK install

                                    Debug.WriteLine(wrDelimiter);
                                    if (Directory.Exists($"{settings.DownloadDir}\\{gameName}\\{packagename}"))
                                    {
                                        deleteOBB(packagename);

                                        changeTitle($"Copying {packagename} OBB to device...");
                                        progressBar.Value = 0;
                                        progressBar.OperationType = "Copying OBB";

                                        // Use async method with progress for OBB
                                        string currentObbStatusBase = string.Empty; // phase or filename

                                        output += await ADB.CopyOBBWithProgressAsync(
                                            $"{settings.DownloadDir}\\{gameName}\\{packagename}",
                                            (progress, eta) => this.Invoke(() =>
                                            {
                                                progressBar.Value = progress;
                                                UpdateProgressStatus("Copying OBB", percent: (int)Math.Round(progress), eta: eta);

                                                if (!string.IsNullOrEmpty(currentObbStatusBase))
                                                {
                                                    progressBar.StatusText = $"{currentObbStatusBase} · {progress:0.0}%";
                                                }
                                                else
                                                {
                                                    progressBar.StatusText = $"{progress:0.0}%";
                                                }
                                            }),
                                            status => this.Invoke(() =>
                                            {
                                                currentObbStatusBase = status ?? string.Empty;
                                            }),
                                            Sideloader.gameNameToSimpleName(gameName));

                                        progressBar.StatusText = ""; // Clear status after OBB copy
                                        changeTitle("");

                                        if (!nodeviceonstart | DeviceConnected)
                                        {
                                            if (!output.Output.Contains("offline"))
                                            {
                                                try
                                                {
                                                    obbsMismatch = await compareOBBSizes(packagename, gameName, output);
                                                }
                                                catch (Exception ex) { _ = FlexibleMessageBox.Show(Program.form, $"Error comparing OBB sizes: {ex.Message}"); }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                output.Output = "Complete (installation skipped).\n\nConnect a device or enable sideloading to install.";
                            }
                            changeTitle($"Installation of {gameName} completed.");
                        }
                        // Only delete if setting enabled and device was connected (so we actually installed)
                        // Skip deletion in local library / offline mode
                        if (settings.DeleteAllAfterInstall && !nodeviceonstart && DeviceConnected && !downloadedFilter_Clicked && !isOffline)
                        {
                            changeTitle("Deleting game files");
                            try { FileSystemUtilities.TryDeleteDirectory(settings.DownloadDir + "\\" + gameName); } catch (Exception ex) { _ = FlexibleMessageBox.Show(Program.form, $"Error deleting game files: {ex.Message}"); }
                        }

                        // Update device space after successful installation
                        if (!settings.NodeviceMode && DeviceConnected)
                        {
                            showAvailableSpace();
                        }

                        // Remove current game
                        cleanupActiveDownloadStatus();
                    }
                }
            }
            if (!obbsMismatch)
            {
                changeTitle("Refreshing games list, please wait...\n");
                showAvailableSpace();

                await RefreshGameListAsync();

                // Only show output if there's content
                if (settings.EnableMessageBoxes && !string.IsNullOrWhiteSpace(output.Output + output.Error))
                {
                    ShowPrcOutput(output);
                }

                progressBar.IsIndeterminate = false;
                gamesAreDownloading = false;
                _downloadPaused = false;
                _pausedProgressPercent = 0;
                _pausedTransfersComplete = 0;
                _pausedFileCount = 0;
                if (_queuePanel != null) { _queuePanel.IsDownloading = false; _queuePanel.IsPaused = false; }
                isinstalling = false;

                changeTitle("");
            }
        }

        private void deleteOBB(string packagename)
        {
            changeTitle("Deleting old OBB Folder...");
            Logger.Log("Attempting to delete old OBB Folder");
            ADB.RunAdbCommandToString($"shell rm -rf \"/sdcard/Android/obb/{packagename}\"");
        }

        private const string OBBFolderPath = "/sdcard/Android/obb/";

        // Logic to compare OBB folders.
        private async Task<bool> compareOBBSizes(string packageName, string gameName, ProcessOutput output)
        {
            string localFolderPath = Path.Combine(settings.DownloadDir, gameName, packageName);

            if (!Directory.Exists(localFolderPath))
            {
                return false;
            }

            try
            {
                changeTitle("Comparing OBBs...");
                Logger.Log("Comparing OBBs");

                DirectoryInfo localFolder = new DirectoryInfo(localFolderPath);
                long totalLocalFolderSize = localFolderSize(localFolder) / (1024 * 1024);

                string remoteFolderSizeResult = ADB.RunAdbCommandToString($"shell du -m \"{OBBFolderPath}{packageName}\"").Output;
                string cleanedRemoteFolderSize = cleanRemoteFolderSize(remoteFolderSizeResult);

                int localObbSize = (int)totalLocalFolderSize;
                int remoteObbSize = Convert.ToInt32(cleanedRemoteFolderSize);

                Logger.Log($"Total local folder size in bytes: {totalLocalFolderSize} Remote Size: {cleanedRemoteFolderSize}");

                if (remoteObbSize < localObbSize)
                {
                    return await handleObbSizeMismatchAsync(packageName, gameName, output);
                }

                return false;
            }
            catch (FormatException ex)
            {
                _ = FlexibleMessageBox.Show(Program.form, "The OBB Folder on the Quest seems to not exist or be empty\nPlease redownload the game or sideload the OBB manually.", "OBB Size Undetectable!", MessageBoxButtons.OK);
                Logger.Log($"Unable to compare OBBs with the exception: {ex.Message}", LogLevel.ERROR);
                FlexibleMessageBox.Show($"Error comparing OBB sizes: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error occurred while comparing OBBs: {ex.Message}", LogLevel.ERROR);
                FlexibleMessageBox.Show(Program.form, $"Unexpected error comparing OBB sizes: {ex.Message}");
                return false;
            }
        }

        private string cleanRemoteFolderSize(string rawSize)
        {
            string replaced = Regex.Replace(rawSize, "[^c]*$", "");
            return Regex.Replace(replaced, "[^0-9]", "");
        }

        // Logic to handle mismatches after comparison.
        private async Task<bool> handleObbSizeMismatchAsync(string packageName, string gameName, ProcessOutput output)
        {
            var dialogResult = MessageBox.Show(Program.form, "Warning! It seems like the OBB wasn't pushed correctly, this means that the game may not launch correctly.\n Do you want to retry the push?", "OBB Size Mismatch!", MessageBoxButtons.YesNo);

            if (dialogResult != DialogResult.Yes)
            {
                await refreshGamesListAsync(output);
                return true;
            }

            changeTitle("Retrying push");

            string obbFolderPath = Path.Combine(settings.DownloadDir, gameName, packageName);

            if (!Directory.Exists(obbFolderPath))
            {
                return false;
            }

            await Task.Run(() =>
            {
                changeTitle($"Copying {packageName} OBB to device...");
                output += ADB.RunAdbCommandToString($"push \"{obbFolderPath}\" \"{OBBFolderPath}\"");
                changeTitle("");
            });

            return await compareOBBSizes(packageName, gameName, output);
        }

        private async Task refreshGamesListAsync(ProcessOutput output)
        {
            changeTitle("Refreshing games list, please wait...");

            showAvailableSpace();
            listAppsBtn();

            await RefreshGameListAsync();

            if (settings.EnableMessageBoxes)
            {
                ShowPrcOutput(output);
            }
            progressBar.IsIndeterminate = false;
            gamesAreDownloading = false;
            _downloadPaused = false;
            _pausedProgressPercent = 0;
            _pausedTransfersComplete = 0;
            _pausedFileCount = 0;
            if (_queuePanel != null) { _queuePanel.IsDownloading = false; _queuePanel.IsPaused = false; }
            isinstalling = false;

            changeTitle("");
        }

        static long localFolderSize(DirectoryInfo localFolder)
        {
            long totalLocalFolderSize = 0;

            // Get all files into the directory
            FileInfo[] allFiles = localFolder.GetFiles();

            // Loop through every file and get size of it
            foreach (FileInfo file in allFiles)
            {
                totalLocalFolderSize += file.Length;
            }

            // Find all subdirectories
            DirectoryInfo[] subFolders = localFolder.GetDirectories();

            // Loop through every subdirectory and get size of each
            foreach (DirectoryInfo dir in subFolders)
            {
                totalLocalFolderSize += localFolderSize(dir);
            }

            // Return the total size of folder
            return totalLocalFolderSize;
        }

        private void timer_Tick4(object sender, EventArgs e)
        {
            _ = new ProcessOutput("", "");
            if (!timerticked)
            {
                timerticked = true;
                bool isinstalled = false;
                if (settings.InstalledApps.Contains(CurrPCKG))
                {
                    isinstalled = true;
                }
                if (isinstalled)
                {
                    if (!settings.AutoReinstall)
                    {
                        DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "In place upgrade has failed." +
                            "\n\nThis means the app must be uninstalled first before updating.\nRookie can attempt to " +
                            "do this while retaining your savedata.\nWhile the vast majority of games can be backed up there " +
                            "are some exceptions\n(we don't know which apps can't be backed up as there is no list online)\n\nDo you want " +
                            "Rookie to uninstall and reinstall the app automatically?", "In place upgrade failed", MessageBoxButtons.OKCancel);
                        if (dialogResult == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                    changeTitle("Performing reinstall, please wait...");
                    _ = ADB.RunAdbCommandToString("kill-server");
                    _ = ADB.RunAdbCommandToString("devices");
                    _ = ADB.RunAdbCommandToString($"pull /sdcard/Android/data/{CurrPCKG} \"{Environment.CurrentDirectory}\"");
                    _ = Sideloader.UninstallGame(CurrPCKG);
                    changeTitle("Reinstalling game");
                    _ = ADB.RunAdbCommandToString($"install -g \"{CurrAPK}\"");
                    _ = ADB.RunAdbCommandToString($"push \"{Environment.CurrentDirectory}\\{CurrPCKG}\" /sdcard/Android/data/");

                    timerticked = false;
                    if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, CurrPCKG)))
                    {
                        FileSystemUtilities.TryDeleteDirectory(Path.Combine(Environment.CurrentDirectory, CurrPCKG));
                    }

                    changeTitle("");
                    return;
                }
                else
                {
                    DialogResult dialogResult2 = FlexibleMessageBox.Show(Program.form, "This installation is taking an unusual amount of time, you can keep waiting or abort the installation.\n" +
                        "Would you like to cancel the installation?", "Cancel install?", MessageBoxButtons.YesNo);
                    if (dialogResult2 == DialogResult.Yes)
                    {
                        changeTitle("Stopping installation...");
                        _ = ADB.RunAdbCommandToString("kill-server");
                        _ = ADB.RunAdbCommandToString("devices");
                    }
                    else
                    {
                        timerticked = false;
                        return;
                    }
                }
            }
        }


        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save window state before closing
            SaveWindowState();

            // Persist download progress for cross-session resume
            SaveQueueToSettings();

            // Cleanup DNS helper (stops proxy)
            DnsHelper.Cleanup();

            if (isinstalling)
            {
                DialogResult res1 = FlexibleMessageBox.Show(Program.form, "There are downloads and/or installations in progress.\nYour download queue will be saved, but any ongoing installations will be canceled.\n\nAre you sure you want to exit?", "Still downloading/installing.",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res1 != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    RCLONE.killRclone();
                }
            }
            else if (isuploading)
            {
                DialogResult res = FlexibleMessageBox.Show(Program.form, "There is an upload still in progress, if you exit now\nyou'll have to start the entire process over again.\nAre you sure you want to exit?", "Still uploading.",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    RCLONE.killRclone();
                    _ = ADB.RunAdbCommandToString("kill-server");
                }
            }
            else
            {
                RCLONE.killRclone();
                _ = ADB.RunAdbCommandToString("kill-server");
            }

        }

        private async void ADBWirelessToggle_Click(object sender, EventArgs e)
        {
            // Check if wireless ADB is currently enabled by verifying actual connection
            bool isWirelessEnabled = false;

            if (File.Exists(storedIpPath) && !string.IsNullOrEmpty(settings.IPAddress))
            {
                // Verify we're actually connected wirelessly by checking connected devices
                string devicesOutput = ADB.RunAdbCommandToString("devices").Output;
                string[] lines = devicesOutput.Split('\n');

                foreach (string line in lines)
                {
                    // Wireless devices show as IP:port format (e.g., "192.168.1.100:5555")
                    if (line.Contains(":5555") && line.Contains("device"))
                    {
                        isWirelessEnabled = true;
                        break;
                    }
                }
            }

            // If enabled, offer to disable or switch device
            if (isWirelessEnabled)
            {
                string action = null;
                using (Form dialog = new Form())
                {
                    dialog.Text = "Wireless ADB Options";
                    dialog.Size = new Size(386, 130);
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dialog.MaximizeBox = false;
                    dialog.MinimizeBox = false;
                    dialog.BackColor = Color.FromArgb(20, 24, 29);
                    dialog.ForeColor = Color.White;

                    var label = new Label
                    {
                        Text = "A device is currently connected via Wireless ADB.",
                        ForeColor = Color.White,
                        AutoSize = true,
                        Location = new Point(15, 15)
                    };

                    var btnSwitch = CreateStyledButton("Connect New Device", DialogResult.None, new Point(15, 45));
                    btnSwitch.Size = new Size(170, 32);
                    btnSwitch.Click += (s, ev) => { action = "switch"; dialog.DialogResult = DialogResult.OK; };

                    var btnDisable = CreateStyledButton("Disable Wireless ADB", DialogResult.None, new Point(195, 45));
                    btnDisable.Size = new Size(160, 32);
                    btnDisable.Click += (s, ev) => { action = "disable"; dialog.DialogResult = DialogResult.OK; };

                    dialog.Controls.AddRange(new Control[] { label, btnSwitch, btnDisable });

                    if (dialog.ShowDialog(this) != DialogResult.OK || action == null)
                    {
                        return;
                    }
                }

                // Disable wireless ADB completely
                if (action == "disable")
                {
                    ADB.wirelessadbON = false;
                    changeTitle("Disabling wireless ADB...");
                    progressBar.IsIndeterminate = true;
                    progressBar.OperationType = "";

                    await Task.Run(() =>
                    {
                        ADB.RunAdbCommandToString("disconnect");
                        ADB.RunAdbCommandToString("kill-server");
                        ADB.RunAdbCommandToString("start-server");
                    });

                    settings.IPAddress = string.Empty;
                    settings.Save();

                    if (File.Exists(storedIpPath))
                    {
                        try { File.Delete(storedIpPath); } catch { }
                    }

                    progressBar.IsIndeterminate = false;
                    _ = await CheckForDevice();
                    changeTitlebarToDevice();
                    changeTitle("Wireless ADB disabled.", true);

                    UpdateWirelessADBButtonText();
                    UpdateStatusLabels();
                    return;
                }

                // User chose to switch device: disconnect current wireless connection
                changeTitle("Disconnecting current device...");
                await Task.Run(() => ADB.RunAdbCommandToString("disconnect"));
            }

            // Connect: Show custom dialog with three options
            string connectionMethod = null;
            using (Form dialog = new Form())
            {
                dialog.Text = "Wireless ADB Connection";
                dialog.Size = new Size(456, 130);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = "How would you like to connect?",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var btnScan = CreateStyledButton("Automatic", DialogResult.None, new Point(15, 45));
                btnScan.Size = new Size(130, 32);
                btnScan.Click += (s, ev) => { connectionMethod = "scan"; dialog.DialogResult = DialogResult.OK; };

                var btnUSB = CreateStyledButton("Automatic (USB)", DialogResult.None, new Point(155, 45));
                btnUSB.Size = new Size(130, 32);
                btnUSB.Click += (s, ev) => { connectionMethod = "usb"; dialog.DialogResult = DialogResult.OK; };

                var btnManual = CreateStyledButton("Manual", DialogResult.None, new Point(295, 45));
                btnManual.Size = new Size(130, 32);
                btnManual.Click += (s, ev) => { connectionMethod = "manual"; dialog.DialogResult = DialogResult.OK; };

                dialog.Controls.AddRange(new Control[] { label, btnScan, btnUSB, btnManual });

                if (dialog.ShowDialog(this) != DialogResult.OK || connectionMethod == null)
                {
                    changeTitle("");
                    return;
                }
            }

            string ipAddress = null;

            if (connectionMethod == "scan")
            {
                ipAddress = await ShowNetworkScanDialogAsync();
            }
            else if (connectionMethod == "manual")
            {
                ipAddress = ShowManualIPDialog();
            }
            else if (connectionMethod == "usb")
            {
                // Setup via USB
                DialogResult usbResult = FlexibleMessageBox.Show(
                    Program.form,
                    "Please make sure your Quest is connected to your PC via USB, then click OK.\n" +
                    "If you need more time, click Cancel and return when you're ready.",
                    "Connect Your Quest",
                    MessageBoxButtons.OKCancel);

                if (usbResult == DialogResult.Cancel)
                {
                    changeTitle("");
                    return;
                }

                changeTitle("Setting up wireless ADB via USB...");
                progressBar.IsIndeterminate = true;
                progressBar.OperationType = "";

                // Check for device and enable TCP/IP mode
                await Task.Run(() =>
                {
                    _ = ADB.RunAdbCommandToString("devices");
                    _ = ADB.RunAdbCommandToString("tcpip 5555");
                });

                _ = FlexibleMessageBox.Show(
                    Program.form,
                    "Click OK to retrieve your Quest's local IP address.",
                    "Get Local IP Address",
                    MessageBoxButtons.OK);

                await Task.Delay(1000);

                // Get IP address
                changeTitle("Retrieving IP address...");
                string input = await Task.Run(() => ADB.RunAdbCommandToString("shell ip route").Output);
                string[] strArrayOne = input.Split(' ');

                if (strArrayOne.Length > 8 && strArrayOne[0].Length > 7)
                {
                    string IPaddr = strArrayOne[8];
                    string IPcmnd = "connect " + IPaddr + ":5555";

                    _ = FlexibleMessageBox.Show(
                        Program.form,
                        $"Your Quest's local IP address is: {IPaddr}\n\n" +
                        $"Please disconnect your Quest and wait 2-3 seconds.\n" +
                        $"Once that's done, click OK.",
                        "Disconnect USB",
                        MessageBoxButtons.OK);

                    changeTitle("Connecting wirelessly...");
                    await Task.Delay(2000);

                    // Attempt wireless connection
                    await Task.Run(() => ADB.RunAdbCommandToString(IPcmnd));
                    await Task.Delay(2000);

                    // Verify device is actually connected
                    int deviceIndex = await CheckForDevice();

                    // success
                    if (deviceIndex >= 0)
                    {
                        // Update UI with device info
                        await Task.Run(() =>
                        {
                            changeTitlebarToDevice();
                            showAvailableSpace();
                        });

                        settings.IPAddress = IPcmnd;
                        settings.WirelessADB = true;
                        settings.Save();

                        try { File.WriteAllText(storedIpPath, IPcmnd); }
                        catch (Exception ex) { Logger.Log($"Unable to write to StoredIP.txt: {ex.Message}", LogLevel.ERROR); }

                        ADB.wirelessadbON = true;

                        // Configure WiFi wakeup settings
                        await Task.Run(() =>
                        {
                            _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                            _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                        });

                        progressBar.IsIndeterminate = false;
                        changeTitle("Connected successfully!", true);
                        UpdateWirelessADBButtonText();
                        UpdateStatusLabels();
                    }
                    // failure
                    else
                    {
                        progressBar.IsIndeterminate = false;
                        changeTitle("");
                        _ = FlexibleMessageBox.Show(Program.form, "Device connection failed! Connect Quest via USB and try again.");
                    }
                }
                else
                {
                    progressBar.IsIndeterminate = false;
                    changeTitle("");
                    _ = FlexibleMessageBox.Show(Program.form, "Device connection failed! Connect Quest via USB and try again.");
                }
                return;
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                changeTitle("");
                return;
            }

            // Connect to the device
            changeTitle($"Connecting to {ipAddress}...");
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "";

            string ipCommand = $"connect {ipAddress}:5555";
            string connectResult = await Task.Run(() => ADB.RunAdbCommandToString(ipCommand).Output);

            progressBar.IsIndeterminate = false;

            if (connectResult.Contains("cannot resolve host") ||
                connectResult.Contains("cannot connect to") ||
                connectResult.Contains("failed") ||
                connectResult.Contains("unable"))
            {
                changeTitle("");
                _ = FlexibleMessageBox.Show(
                    Program.form,
                    $"Failed to connect to {ipAddress}\n\nPlease verify:\n" +
                    "- The IP address is correct\n" +
                    "- The device is on the same network\n" +
                    "- Developer mode is enabled\n" +
                    "- Wireless ADB with tcpip 5555 enabled (requires one-time setup*)\n\n" +
                    "* Connect device via USB and run ADB command: tcpip 5555",
                    "Connection Failed",
                    MessageBoxButtons.OK);

                UpdateWirelessADBButtonText();
                UpdateStatusLabels();
                return;
            }

            // Success - save settings and configure device
            _ = await CheckForDevice();
            changeTitlebarToDevice();
            showAvailableSpace();

            settings.IPAddress = ipCommand;
            settings.WirelessADB = true;
            settings.Save();

            try
            {
                File.WriteAllText(storedIpPath, ipCommand);
            }
            catch (Exception ex)
            {
                Logger.Log($"Unable to write to StoredIP.txt: {ex.Message}", LogLevel.ERROR);
            }

            ADB.wirelessadbON = true;

            // Configure wake settings in background
            _ = Task.Run(() =>
            {
                ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
            });

            changeTitle("Connected successfully!", true);
            UpdateWirelessADBButtonText();
            UpdateStatusLabels();
        }

        private string ShowManualIPDialog()
        {
            // Get local subnet prefix to pre-fill
            string subnetPrefix = GetLocalIPv4();

            using (Form dialog = new Form())
            {
                dialog.Text = "Enter Quest IP Address";
                dialog.Size = new Size(350, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = "Enter your Quest's IP Address:",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var textBox = new TextBox
                {
                    Location = new Point(15, 40),
                    Size = new Size(300, 24),
                    BackColor = Color.FromArgb(40, 44, 52),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = subnetPrefix  // Pre-fill with subnet prefix
                };

                // Position cursor at end of pre-filled text
                textBox.SelectionStart = textBox.Text.Length;

                var okButton = CreateStyledButton("OK", DialogResult.OK, new Point(155, 75));
                var cancelButton = CreateStyledButton("Cancel", DialogResult.Cancel, new Point(240, 75), false);

                dialog.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                // Focus the textbox when dialog shows
                dialog.Shown += (s, e) =>
                {
                    textBox.Focus();
                    textBox.SelectionStart = textBox.Text.Length;
                };

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    return textBox.Text.Trim();
                }
            }
            return null;
        }

        private string GetLocalIPv4()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up)
                        continue;

                    var ipProps = ni.GetIPProperties();
                    foreach (var ua in ipProps.UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ua.Address))
                        {

                            string localIp = ua.Address.ToString();
                            string localPrefix = null;

                            if (!string.IsNullOrEmpty(localIp))
                            {
                                var o = localIp.Split('.');
                                if (o.Length == 4)
                                {
                                    localPrefix = $"{o[0]}.{o[1]}.{o[2]}.";
                                }
                            }

                            return localPrefix;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unable to get local IPv4: {ex.Message}", LogLevel.WARNING);
            }

            return null;
        }

        private async Task<List<string>> ScanNetworkForAdbDevicesAsync()
        {
            var foundDevices = new List<string>();
            string localSubnet = GetLocalIPv4();

            if (string.IsNullOrEmpty(localSubnet))
            {
                Logger.Log("Could not determine local subnet for scanning", LogLevel.WARNING);
                return foundDevices;
            }

            changeTitle("Scanning network for ADB devices...");
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "";

            // Scan common IP range (1-254) on port 5555
            var tasks = new List<Task<string>>();

            for (int i = 1; i <= 254; i++)
            {
                string ip = $"{localSubnet}{i}";
                tasks.Add(CheckAdbDeviceAsync(ip, 5555));
            }

            var results = await Task.WhenAll(tasks);
            foundDevices.AddRange(results.Where(r => !string.IsNullOrEmpty(r)));

            progressBar.IsIndeterminate = false;
            changeTitle("");

            return foundDevices;
        }

        private async Task<string> CheckAdbDeviceAsync(string ip, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    // Timeout, 1000ms should be enough for local network
                    var connectTask = client.ConnectAsync(ip, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask)
                    {
                        if (client.Connected)
                        {
                            // Port is open, try ADB connect to verify it's actually an ADB device
                            string result = ADB.RunAdbCommandToString($"connect {ip}:{port}").Output;
                            if (result.Contains("connected") || result.Contains("already"))
                            {
                                // Disconnect immediately, we're just scanning
                                ADB.RunAdbCommandToString($"disconnect {ip}:{port}");
                                return ip;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore connection failures, device not present or not ADB
            }
            return null;
        }

        private async Task<string> ShowNetworkScanDialogAsync()
        {
            var devices = await ScanNetworkForAdbDevicesAsync();

            if (devices.Count == 0)
            {
                FlexibleMessageBox.Show(Program.form,
                    "No ADB devices found on the network.\n\n" +
                    "Please verify:\n" +
                    "- The device is on the same network\n" +
                    "- Developer mode is enabled\n" +
                    "- Wireless ADB with tcpip 5555 enabled (requires one-time setup*)\n\n" +
                    "* Connect device via USB and run ADB command: tcpip 5555",
                    "No Devices Found");
                return null;
            }

            if (devices.Count == 1)
            {
                return devices[0];
            }

            // Multiple devices found - let user choose
            using (Form dialog = new Form())
            {
                dialog.Text = "Select ADB Device";
                dialog.Size = new Size(350, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = $"Found {devices.Count} ADB devices:",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var comboBox = new ComboBox
                {
                    Location = new Point(15, 40),
                    Size = new Size(300, 24),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White
                };

                foreach (var device in devices)
                    comboBox.Items.Add(device);
                comboBox.SelectedIndex = 0;

                var okButton = CreateStyledButton("Connect", DialogResult.OK, new Point(155, 75));
                var cancelButton = CreateStyledButton("Cancel", DialogResult.Cancel, new Point(240, 75), false);

                dialog.Controls.AddRange(new Control[] { label, comboBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                    return comboBox.SelectedItem.ToString();
            }
            return null;
        }

        private void UpdateWirelessADBButtonText()
        {
            bool isWirelessEnabled = File.Exists(storedIpPath) && !string.IsNullOrEmpty(settings.IPAddress);
            ADBWirelessToggle.Text = isWirelessEnabled ? "WIRELESS ADB" : "ENABLE WIRELESS ADB";
        }

        private void devicesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showAvailableSpace();
        }

        private async void remotesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressMirrorRefresh) return;
            if (remotesList.SelectedItem != null)
            {
                string selectedRemote = remotesList.SelectedItem.ToString();
                if (selectedRemote == "Public")
                {
                    UsingPublicConfig = true;
                }
                else
                {
                    UsingPublicConfig = false;
                    remotesList.Invoke(() => { currentRemote = selectedRemote; });
                }

                settings.selectedMirror = selectedRemote;
                settings.Save();

                await refreshCurrentMirror("Refreshing App List...");
                UpdateStatusLabels();
            }
        }

        private void QuestOptionsButton_Click(object sender, EventArgs e)
        {
            QuestForm Form = new QuestForm();
            Form.Show(Program.form);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine sort order
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                lvwColumnSorter.Order = lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order =
                    (e.Column == 4 || e.Column == 5) ? SortOrder.Descending :
                    (e.Column == 6) ? SortOrder.Ascending :
                    SortOrder.Ascending;
            }

            // Update shared sort state
            _sharedSortField = ColumnIndexToSortField(e.Column);
            _sharedSortDirection = lvwColumnSorter.Order == SortOrder.Ascending
                ? SortDirection.Ascending
                : SortDirection.Descending;

            // Suspend drawing during sort
            gamesListView.BeginUpdate();
            try
            {
                gamesListView.Sort();
            }
            finally
            {
                gamesListView.EndUpdate();
            }

            // Invalidate header to update sort indicators
            gamesListView.Invalidate(new Rectangle(0, 0, gamesListView.ClientSize.Width,
                gamesListView.Font.Height + 8));

            // Save sort state
            SaveWindowState();
        }

        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (searchTextBox.Visible)
                {
                    if (gamesListView.SelectedItems.Count > 0)
                    {
                        downloadInstallGameButton_Click(sender, e);
                    }
                }
                searchTextBox.Visible = false;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                searchTextBox.Visible = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                // Show search box.
                searchTextBox.Clear();
                searchTextBox.Visible = true;
                _ = searchTextBox.Focus();
            }
            if (keyData == (Keys.Control | Keys.L))
            {
                if (loaded)
                {
                    StringBuilder copyGamesListView = new StringBuilder();
                    var itemsSource = gamesListView.Items.Cast<ListViewItem>().ToList();

                    foreach (ListViewItem item in itemsSource)
                    {
                        // Assuming the game name is in the first column (subitem index 0)
                        copyGamesListView.Append(item.SubItems[0].Text).Append("\n");
                    }

                    if (copyGamesListView.Length > 0)
                    {
                        Clipboard.SetText(copyGamesListView.ToString());
                        _ = FlexibleMessageBox.Show("Entire game list copied as a paragraph to clipboard!\nPress CTRL+V to paste it anywhere!");
                    }
                    else
                    {
                        changeTitle("No games to copy", true);
                    }
                }
                return true; // Mark as handled
            }
            if (keyData == (Keys.Alt | Keys.L))
            {
                if (loaded)
                {
                    StringBuilder copyGamesListView = new StringBuilder();
                    var itemsSource = gamesListView.Items.Cast<ListViewItem>().ToList();

                    foreach (ListViewItem item in itemsSource)
                    {
                        // Assuming the game name is in the first column (subitem index 0)
                        copyGamesListView.Append(item.SubItems[0].Text).Append(", ");
                    }

                    // Remove the last ", " if there's any content in rookienamelist2
                    if (copyGamesListView.Length > 2)
                    {
                        copyGamesListView.Length -= 2;
                    }

                    if (copyGamesListView.Length > 0)
                    {
                        Clipboard.SetText(copyGamesListView.ToString());
                        _ = FlexibleMessageBox.Show("Entire game list copied as a paragraph to clipboard!\nPress CTRL+V to paste it anywhere!");
                    }
                    else
                    {
                        changeTitle("No games to copy", true);
                    }
                }
                return true; // Mark as handled
            }
            if (keyData == (Keys.Control | Keys.H))
            {
                string HWID = SideloaderUtilities.UUID();
                Clipboard.SetText(HWID);
                _ = FlexibleMessageBox.Show(Program.form, $"Your unique HWID is:\n\n{HWID}\n\nThis has been automatically copied to your clipboard. Press CTRL+V in a message to send it.");
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                btnRunAdbCmd_Click(this, EventArgs.Empty);
            }
            if (keyData == (Keys.Control | Keys.F4))
            {
                try
                {
                    // Relaunch the program using Sideloader Launcher
                    _ = Process.Start(Application.StartupPath + "\\Sideloader Launcher.exe");
                    Process.GetCurrentProcess().Kill();
                }
                catch
                { }
            }

            if (keyData == Keys.F3)
            {
                if (Application.OpenForms.OfType<QuestForm>().Count() == 0)
                {
                    QuestForm Form = new QuestForm();
                    Form.Show(Program.form);
                }

            }
            if (keyData == Keys.F4)
            {
                if (Application.OpenForms.OfType<SettingsForm>().Count() == 0)
                {
                    SettingsForm Form = new SettingsForm();
                    Form.Show(Program.form);
                }
            }
            if (keyData == Keys.F5)
            {
                if (!DeviceConnected && Devices.Count == 0)
                {
                    FlexibleMessageBox.Show(Program.form,
                        "No device connected. Please connect your Quest and click 'RECONNECT DEVICE' first.",
                        "Device Required",
                        MessageBoxButtons.OK);
                    return true;
                }

                changeTitle("Refreshing games list...");
                listAppsBtn();
                // Use RefreshGameListAsync to preserve filter state
                _ = RefreshGameListAsync();
            }
            bool dialogIsUp = false;
            if (keyData == Keys.F1 && !dialogIsUp)
            {
                _ = FlexibleMessageBox.Show(Program.form,
@"Keyboard Shortcuts

F1   - Show shortcuts list
F3   - Open Device Settings
F4   - Open Rookie Settings
F5   - Refresh games list

CTRL + R   - Run custom ADB command
CTRL + L   - Copy all game names (one per line)
ALT + L     - Copy all game names (comma-separated in a single line)
CTRL + P   - Copy package name of selected game
CTRL + F4  - Instantly relaunch Rookie Sideloader");
            }
            if (keyData == (Keys.Control | Keys.P))
            {
                // Immediately copy the package name of the currently selected game
                if (loaded && gamesListView.SelectedItems.Count > 0)
                {
                    var selectedItem = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1];
                    string packageName = selectedItem.SubItems[SideloaderRCLONE.PackageNameIndex].Text;
                    Clipboard.SetText(packageName);
                    changeTitle($"Copied: {packageName}", true);
                }
                else if (loaded && isGalleryView && _fastGallery != null && _fastGallery._selectedIndex >= 0)
                {
                    var galleryItem = _fastGallery.GetItemAtIndex(_fastGallery._selectedIndex);
                    if (galleryItem != null && galleryItem.SubItems.Count > SideloaderRCLONE.PackageNameIndex)
                    {
                        string packageName = galleryItem.SubItems[SideloaderRCLONE.PackageNameIndex].Text;
                        Clipboard.SetText(packageName);
                        changeTitle($"Copied: {packageName}", true);
                    }
                }
                else
                {
                    changeTitle("No game selected", true);
                }
                return true; // Mark as handled
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }

        private async void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private async Task RunSearch()
        {
            _debounceTimer.Stop();

            // Cancel any ongoing searches
            _cts?.Cancel();

            string searchTerm = searchTextBox.Text;

            // Ignore placeholder text
            if (searchTerm == "Search..." || string.IsNullOrWhiteSpace(searchTerm))
            {
                RestoreFullList();
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                // Perform search using index for faster lookups
                var matches = await Task.Run(() =>
                {
                    if (token.IsCancellationRequested) return new List<ListViewItem>();

                    var results = new HashSet<ListViewItem>(); // Avoid duplicates

                    // Try exact match first using index
                    if (_searchIndex != null && _searchIndex.TryGetValue(searchTerm, out var exactMatches))
                    {
                        foreach (var match in exactMatches)
                        {
                            if (token.IsCancellationRequested) return new List<ListViewItem>();
                            results.Add(match);
                        }
                    }

                    // Then do partial matches using index
                    if (_searchIndex != null)
                    {
                        foreach (var kvp in _searchIndex)
                        {
                            if (token.IsCancellationRequested) return new List<ListViewItem>();

                            if (kvp.Key.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                foreach (var item in kvp.Value)
                                {
                                    results.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Fallback to linear search if index not built yet
                        foreach (var item in _allItems)
                        {
                            if (token.IsCancellationRequested) return new List<ListViewItem>();

                            if (item.Text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                (item.SubItems.Count > 1 && item.SubItems[1].Text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0))
                            {
                                results.Add(item);
                            }
                        }
                    }

                    return results.ToList();
                }, token);

                // Check if cancelled before updating UI
                if (token.IsCancellationRequested) return;

                // Update UI on main thread
                gamesListView.BeginUpdate();
                try
                {
                    gamesListView.Items.Clear();
                    if (matches.Count > 0)
                    {
                        gamesListView.Items.AddRange(matches.ToArray());
                    }
                }
                finally
                {
                    gamesListView.EndUpdate();
                }

                // Refresh gallery view if active
                if (isGalleryView && gamesGalleryView.Visible)
                {
                    _galleryDataSource = matches;
                    PopulateGalleryView();
                }
            }
            catch (OperationCanceledException)
            {
                // Search was cancelled - this is expected behavior
            }
            catch (Exception ex)
            {
                Logger.Log($"Error during search: {ex.Message}", LogLevel.ERROR);
            }
        }

        static string ExtractVideoId(string html)
        {
            // We want the first strict 11-char YouTube video ID after /watch?v=
            var m = Regex.Match(html, @"\/watch\?v=([A-Za-z0-9_\-]{11})");
            return m.Success ? m.Groups[1].Value : string.Empty;
        }

        private async Task CreateEnvironment()
        {
            if (!settings.TrailersEnabled) return;

            // Fast path: already initialized
            if (webView21.CoreWebView2 != null) return;

            // Check if WebView2 runtime DLLs are present
            // (downloadFiles() should have already downloaded them, but check anyway)
            string runtimesPath = Path.Combine(Environment.CurrentDirectory, "runtimes");
            string webView2LoaderArm64 = Path.Combine(runtimesPath, "win-arm64", "native", "WebView2Loader.dll");
            string webView2LoaderX86 = Path.Combine(runtimesPath, "win-x86", "native", "WebView2Loader.dll");
            string webView2LoaderX64 = Path.Combine(runtimesPath, "win-x64", "native", "WebView2Loader.dll");

            bool runtimeExists = File.Exists(webView2LoaderX86) && File.Exists(webView2LoaderX64) && File.Exists(webView2LoaderArm64);

            if (!runtimeExists)
            {
                // Runtime wasn't downloaded during startup - disable trailers
                Logger.Log("WebView2 runtime not found, disabling trailer playback", LogLevel.WARNING);
                enviromentCreated = true;
                webView21.Hide();
                return;
            }

            try
            {
                var appDataFolder = Path.Combine(Environment.CurrentDirectory, "WebView2Cache");
                Directory.CreateDirectory(appDataFolder); // Ensure it exists
                var env = await CoreWebView2Environment.CreateAsync(userDataFolder: appDataFolder);

                await webView21.EnsureCoreWebView2Async(env);

                // Map local folder to a trusted origin (https://app.local)
                var webroot = Path.Combine(Environment.CurrentDirectory, "trailer");
                Directory.CreateDirectory(webroot);
                webView21.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "app.local", webroot, CoreWebView2HostResourceAccessKind.Allow);

                // Minimal settings required for the player page
                var s = webView21.CoreWebView2.Settings;
                s.IsScriptEnabled = true;       // allow IFrame API
                s.IsWebMessageEnabled = true;   // allow PostWebMessageAsString from host

                ApplyWebViewRoundedCorners();
            }
            catch (Exception /* ex */)
            {
                enviromentCreated = true;
                webView21.Hide();
            }
        }

        private void InitializeTrailerPlayer()
        {
            if (!settings.TrailersEnabled) return;
            if (_trailerPlayerInitialized) return;
            string webroot = Path.Combine(Environment.CurrentDirectory, "trailer");
            Directory.CreateDirectory(webroot);
            string playerHtml = Path.Combine(webroot, "player.html");

            // Lightweight HTML with YouTube IFrame API and WebView2 message bridge
            var html = @"<!doctype html>
<html>
<head>
<meta charset=""utf-8"">
<meta name=""viewport"" content=""width=device-width,initial-scale=1""/>
<title>Trailer Player</title>
<style>
html,body { margin:0; background:#000; height:100%; overflow:hidden; }
#player { width:100vw; height:100vh; }
</style>
<script src=""https://www.youtube.com/iframe_api""></script>
<script>
let player;
let pendingId = null;
function onYouTubeIframeAPIReady() {
    player = new YT.Player('player', {
        width: '100%',
        height: '100%',
        playerVars: {
            autoplay: 0,
            mute: 1,
            rel: 0,
            iv_load_policy: 3,
            fs: 0
        },
        events: {
            'onReady': () => {
                if (pendingId) {
                    player.loadVideoById(pendingId);
                    pendingId = null;
                }
            }
        }
    });
}
(function(){
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.addEventListener('message', e => {
            const id = (e && e.data) ? String(e.data).trim() : '';
            if (!/^[A-Za-z0-9_\-]{11}$/.test(id)) return;
            if (player && player.loadVideoById) {
                    player.loadVideoById(id);
                } else {
                    pendingId = id;
                }
            });
        }
    })();
</script>
</head>
<body>
<div id=""player""></div>
</body>
</html>";
            File.WriteAllText(playerHtml, html, Encoding.UTF8);
            _trailerPlayerInitialized = true;
        }

        // Ensure environment + initial navigation
        private async Task EnsureTrailerEnvironmentAsync()
        {
            if (!settings.TrailersEnabled) return;

            if (webView21.CoreWebView2 == null)
            {
                await CreateEnvironment();
            }

            // Check again after CreateEnvironment - it may have failed
            if (webView21.CoreWebView2 == null)
            {
                Logger.Log("WebView2 CoreWebView2 is null after CreateEnvironment", LogLevel.WARNING);
                return;
            }

            InitializeTrailerPlayer();

            if (!_trailerHtmlLoaded && webView21.CoreWebView2 != null)
            {
                webView21.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    _trailerHtmlLoaded = true;
                };
                webView21.CoreWebView2.Navigate("https://app.local/player.html");
            }
        }

        private async Task ShowVideoAsync(string videoId)
        {
            if (!settings.TrailersEnabled) return;
            if (string.IsNullOrEmpty(videoId)) return;

            try
            {
                await EnsureTrailerEnvironmentAsync();

                // Check if WebView2 was successfully initialized
                if (webView21.CoreWebView2 == null)
                {
                    return;
                }

                // If first load still in progress, small retry loop
                int tries = 0;
                while (!_trailerHtmlLoaded && tries < 50)
                {
                    await Task.Delay(50);
                    tries++;
                }

                // Double-check after waiting
                if (webView21.CoreWebView2 == null || !_trailerHtmlLoaded)
                {
                    return;
                }

                // Post the raw ID; page builds final URL
                webView21.CoreWebView2.PostWebMessageAsString(videoId);
                HideVideoPlaceholder(); // Video is loading, hide placeholder
            }
            catch (Exception ex)
            {
                Logger.Log($"ShowVideoAsync error: {ex.Message}", LogLevel.WARNING);
            }
        }

        private async Task<string> ResolveVideoIdAsync(string gameName)
        {
            if (!settings.TrailersEnabled) return string.Empty;
            if (string.IsNullOrWhiteSpace(gameName)) return string.Empty;

            if (_videoIdCache.TryGetValue(gameName, out var cached))
                return cached;

            string cleanedName = CleanGameNameForSearch(gameName);
            string searchTerm = $"{cleanedName} VR trailer";

            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; rv:109.0) Gecko/20100101 Firefox/119.0");
                    http.Timeout = TimeSpan.FromSeconds(5);

                    string query = WebUtility.UrlEncode(searchTerm);
                    string searchUrl = $"https://www.youtube.com/results?search_query={query}";

                    var html = await http.GetStringAsync(searchUrl);
                    var videoId = ExtractBestVideoId(html, cleanedName);

                    if (!string.IsNullOrEmpty(videoId))
                    {
                        _videoIdCache[gameName] = videoId;
                        return videoId;
                    }
                }
            }
            catch
            {
                // swallow
            }

            // Cache empty result to prevent repeated lookups
            _videoIdCache[gameName] = string.Empty;
            return string.Empty;
        }

        private static string CleanGameNameForSearch(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return gameName;

            // Clean up game name, remove:
            string[] patternsToRemove = new[]
            {
                @"\s*\([^)]+\)",                 // (anything in parentheses)
                @"\s*\[[^\]]*\]",                // [anything in brackets]
                @"\s+v?\d+\.\d+[\d.]*\b",        // version numbers. v1.0, 1.35.0, 1.37.0 etc.
            };

            string cleaned = gameName;
            foreach (string pattern in patternsToRemove)
                cleaned = Regex.Replace(cleaned, pattern, "", RegexOptions.IgnoreCase);

            // Clean up trailing punctuation and whitespace
            cleaned = Regex.Replace(cleaned, @"[-:,]+$", "");
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

            // If cleaning removed everything, return original
            return string.IsNullOrWhiteSpace(cleaned) ? gameName.Trim() : cleaned;
        }

        private static readonly Regex VideoDataRegex = new Regex(
            @"""videoRenderer""\s*:\s*\{\s*[^}]*?""videoId""\s*:\s*""([A-Za-z0-9_\-]{11})""[\s\S]*?""title""\s*:\s*\{\s*""runs""\s*:\s*\[\s*\{\s*""text""\s*:\s*""([^""]+)""",
            RegexOptions.Compiled);

        private static readonly Regex UnicodeEscapeRegex = new Regex(
            @"\\u([0-9A-Fa-f]{4})",
            RegexOptions.Compiled);

        private static string ExtractBestVideoId(string html, string cleanedGameName)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            var videoMatches = VideoDataRegex.Matches(html);

            // Fallback: no matches found, do simple extraction
            if (videoMatches.Count == 0)
            {
                var simpleMatch = Regex.Match(html, @"\/watch\?v=([A-Za-z0-9_\-]{11})");
                return simpleMatch.Success ? simpleMatch.Groups[1].Value : string.Empty;
            }

            // Normalize: remove apostrophes and convert to lowercase
            string lowerGameName = cleanedGameName.ToLowerInvariant().Replace("'", "");
            var gameWords = lowerGameName
                .Split(new[] { ' ', '-', ':', '&' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            int requiredMatches = gameWords.Count;
            string bestVideoId = null;
            int bestScore = 0;
            int position = 0;

            foreach (Match match in videoMatches)
            {
                string videoId = match.Groups[1].Value;
                string title = match.Groups[2].Value.ToLowerInvariant();

                title = UnicodeEscapeRegex.Replace(title, m =>
                    ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());

                // Normalize title: remove apostrophes for matching
                string normalizedTitle = title.Replace("'", "");

                // All game words must be present
                if (!gameWords.All(w => normalizedTitle.Contains(w)))
                    continue;

                position++;

                // Only process first 10 matches
                if (position > 10)
                    break;

                int score = 0;

                // Position bonus
                if (position == 1) score += 30;
                else if (position == 2) score += 20;
                else if (position == 3) score += 10;

                // Word bonus
                if (title.Contains("trailer") || title.Contains("teaser")) score += 25;
                if (title.Contains("official")) score += 20;
                if (title.Contains("launch") || title.Contains("release")) score += 15;
                if (title.Contains("announce") || title.Contains("reveal")) score += 12; // also includes "announcement"
                if (title.Contains("gameplay") || title.Contains("vr")) score += 5;

                // Noise penalty for extra words
                int totalWords = title.Split(new[] { ' ', '-', '|', ':', '–' },
                    StringSplitOptions.RemoveEmptyEntries).Length;
                int extraWords = totalWords - gameWords.Count;
                score += extraWords * -3;  // -3 per extra word

                // Hard penalties for junk
                if (title.Contains("review") ||
                    title.Contains("tutorial") ||
                    title.Contains("how to") ||
                    title.Contains("install") ||
                    title.Contains("guide") ||
                    title.Contains("setup") ||
                    title.Contains("reaction"))
                    score -= 30;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestVideoId = videoId;
                }
            }

            return bestVideoId ?? string.Empty;
        }

        public async void gamesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide the uninstall button initially
            if (_listViewUninstallButton != null)
            {
                _listViewUninstallButton.Visible = false;
            }

            if (gamesListView.SelectedItems.Count < 1)
            {
                selectedGameLabel.Text = "";
                downloadInstallGameButton.Enabled = false;
                downloadInstallGameButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(67)))), ((int)(((byte)(82)))));
                return;
            }

            var selectedItem = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1];
            string CurrentPackageName = selectedItem.SubItems[SideloaderRCLONE.PackageNameIndex].Text;
            string CurrentReleaseName = selectedItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            string CurrentGameName = selectedItem.SubItems[SideloaderRCLONE.GameNameIndex].Text;
            Console.WriteLine(CurrentGameName);

            downloadInstallGameButton.Enabled = true;
            downloadInstallGameButton.ForeColor = System.Drawing.Color.Black;

            // Update the selected game label in the sidebar
            selectedGameLabel.Text = CurrentGameName;

            // Thumbnail
            if (!keyheld)
            {
                if (settings.PackageNameToCB)
                {
                    Clipboard.SetText(CurrentPackageName);
                }

                keyheld = true;
            }

            string[] imageExtensions = { ".jpg", ".png" };
            string ImagePath = String.Empty;

            foreach (string extension in imageExtensions)
            {
                string path = Path.Combine(SideloaderRCLONE.ThumbnailsFolder, $"{CurrentPackageName}{extension}");
                if (File.Exists(path))
                {
                    ImagePath = path;
                    break;
                }
            }

            // Dispose the old image first
            var oldImage = gamesPictureBox.BackgroundImage;
            gamesPictureBox.BackgroundImage = null;

            if (oldImage != null)
            {
                oldImage.Dispose();
            }

            if (File.Exists(ImagePath))
            {
                gamesPictureBox.BackgroundImage = Image.FromFile(ImagePath);
            }

            // If no image exists, BackgroundImage stays null and the Paint handler draws the placeholder
            gamesPictureBox.Invalidate(); // Force repaint to show placeholder

            // Fast trailer loading path
            if (settings.TrailersEnabled)
            {
                webView21.Enabled = true;
                webView21.Show();

                try
                {
                    var videoId = await ResolveVideoIdAsync(CurrentGameName);
                    if (string.IsNullOrEmpty(videoId))
                    {
                        changeTitle("No Trailer found", true);
                        ShowVideoPlaceholder();
                    }
                    else
                    {
                        await ShowVideoAsync(videoId);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error loading Trailer");
                    Logger.Log(ex.Message);
                    ShowVideoPlaceholder();
                }
            }
            else
            {
                ShowVideoPlaceholder();
            }

            string NotePath = $"{SideloaderRCLONE.NotesFolder}\\{CurrentReleaseName}.txt";

            if (!isGalleryView)
            {
                UpdateReleaseNotes(NotePath);
                UpdateNotesScrollBar();
            }
        }

        private async void ListViewUninstallButton_Click(object sender, EventArgs e)
        {
            var item = _listViewUninstallButton.Tag as ListViewItem;
            if (item == null) return;

            _listViewUninstallButton.Visible = false;
            await UninstallGameAsync(item);
        }

        public void UpdateGamesButton_Click(object sender, EventArgs e)
        {
            if (!DeviceConnected && Devices.Count == 0)
            {
                FlexibleMessageBox.Show(Program.form,
                    "No device connected. Please connect your Quest and click 'RECONNECT DEVICE' first.",
                    "Device Required",
                    MessageBoxButtons.OK);
                return;
            }

            changeTitle("Refreshing installed apps and checking for updates...");
            listAppsBtn();

            if (SideloaderRCLONE.games.Count < 1)
            {
                FlexibleMessageBox.Show(Program.form,
                    "There are no games in rclone, please check your internet connection and verify the config is working properly.");
                return;
            }

            // Use RefreshGameListAsync to preserve filter state
            _ = RefreshGameListAsync();
        }

        private void gamesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (gamesListView.SelectedItems.Count > 0)
            {
                downloadInstallGameButton_Click(sender, e);
            }
        }

        private void MountButton_Click(object sender, EventArgs e)
        {
            _ = ADB.RunAdbCommandToString("shell svc usb setFunctions mtp true");
        }

        private void freeDisclaimer_Click(object sender, EventArgs e)
        {
            _ = Process.Start($"https://github.com/{repo}");
        }

        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "Search...")
            {
                searchTextBox.Text = "";
            }

            searchTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            searchTextBox.ForeColor = Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(218)))), ((int)(((byte)(218)))));
        }

        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = "Search...";
                searchTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            }

            searchTextBox.ForeColor = Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));

            _ = gamesListView.Focus();
        }

        private void gamesListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (gamesListView.SelectedItems.Count > 0)
                {
                    downloadInstallGameButton_Click(sender, e);
                }
            }
        }

        bool updateAvailableClicked = false;
        private void btnUpdateAvailable_Click(object sender, EventArgs e)
        {
            btnInstalled.Click -= btnInstalled_Click;
            btnUpdateAvailable.Click -= btnUpdateAvailable_Click;
            btnNewerThanList.Click -= btnNewerThanList_Click;

            if (downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = false;
            }

            if (upToDate_Clicked || NeedsDonation_Clicked)
            {
                upToDate_Clicked = false;
                NeedsDonation_Clicked = false;
                updateAvailableClicked = false;
            }

            if (!updateAvailableClicked)
            {
                updateAvailableClicked = true;
                FilterListByColor(ColorUpdateAvailable); // Update available color
            }
            else
            {
                updateAvailableClicked = false;
                RestoreFullList();
            }

            // Update button visual states
            UpdateFilterButtonStates();

            // Refresh gallery view if active
            if (isGalleryView)
            {
                PopulateGalleryView();
            }

            btnInstalled.Click += btnInstalled_Click;
            btnUpdateAvailable.Click += btnUpdateAvailable_Click;
            btnNewerThanList.Click += btnNewerThanList_Click;
        }

        private async void pullAppToDesktopBtn_Click(object sender, EventArgs e)
        {
            string selectedApp = ShowInstalledAppSelector("Select an app to pull to desktop");
            if (selectedApp == null)
            {
                return;
            }

            DialogResult dialogResult1 = FlexibleMessageBox.Show(Program.form, $"Do you want to extract {selectedApp}'s APK and OBB to a folder on your desktop now?", "Extract app?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (!isworking)
            {
                isworking = true;
                progressBar.IsIndeterminate = true;
                progressBar.OperationType = "Loading";
                string HWID = SideloaderUtilities.UUID();
                string GameName = selectedApp;
                string packageName = Sideloader.gameNameToPackageName(GameName);
                string InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packageName} | grep versionCode -F\"").Output;
                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                ulong VersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                if (Directory.Exists($"{settings.MainDir}\\{packageName}"))
                {
                    FileSystemUtilities.TryDeleteDirectory($"{settings.MainDir}\\{packageName}");
                }

                ProcessOutput output = new ProcessOutput("", "");
                changeTitle("Extracting APK....");

                _ = Directory.CreateDirectory($"{settings.MainDir}\\{packageName}");

                Thread t1 = new Thread(() =>
                {
                    output = Sideloader.getApk(GameName);
                })
                {
                    IsBackground = true
                };
                t1.Start();

                while (t1.IsAlive)
                {
                    await Task.Delay(100);
                }

                changeTitle("Extracting OBB if it exists....");
                Thread t2 = new Thread(() =>
                {
                    output += ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packageName}\" \"{settings.MainDir}\\{packageName}\"");
                })
                {
                    IsBackground = true
                };
                t2.Start();

                while (t2.IsAlive)
                {
                    await Task.Delay(100);
                }

                if (File.Exists($"{settings.MainDir}\\{GameName} v{VersionInt} {packageName}.zip"))
                {
                    File.Delete($"{settings.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");
                }

                string path = $"{settings.MainDir}\\7z.exe";
                string cmd = $"7z a -mx1 \"{settings.MainDir}\\{GameName} v{VersionInt} {packageName}.zip\" .\\{packageName}\\*";
                changeTitle("Zipping extracted application...");
                Thread t3 = new Thread(() =>
                {
                    _ = ADB.RunCommandToString(cmd, path);
                })
                {
                    IsBackground = true
                };
                t3.Start();

                while (t3.IsAlive)
                {
                    await Task.Delay(100);
                }

                if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip"))
                {
                    File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip");
                }

                File.Copy($"{settings.MainDir}\\{GameName} v{VersionInt} {packageName}.zip", $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip");
                File.Delete($"{settings.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");
                FileSystemUtilities.TryDeleteDirectory($"{settings.MainDir}\\{packageName}");
                isworking = false;
                changeTitle("");
                progressBar.IsIndeterminate = false;
                _ = FlexibleMessageBox.Show(Program.form, $"{GameName} pulled to:\n\n{GameName} v{VersionInt} {packageName}.zip\n\nOn your desktop!");
            }
        }

        bool upToDate_Clicked = false;
        private void btnInstalled_Click(object sender, EventArgs e)
        {
            btnInstalled.Click -= btnInstalled_Click;
            btnUpdateAvailable.Click -= btnUpdateAvailable_Click;
            btnNewerThanList.Click -= btnNewerThanList_Click;

            if (downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = false;
            }

            if (updateAvailableClicked || NeedsDonation_Clicked)
            {
                updateAvailableClicked = false;
                NeedsDonation_Clicked = false;
                upToDate_Clicked = false;
            }

            if (!upToDate_Clicked)
            {
                upToDate_Clicked = true;
                // Filter to show installed, update available and ahead of list entries
                FilterListByColors(new[] { ColorInstalled, ColorUpdateAvailable, ColorDonateGame });
            }
            else
            {
                upToDate_Clicked = false;
                RestoreFullList();
            }

            // Update button visual states
            UpdateFilterButtonStates();

            // Refresh gallery view if active
            if (isGalleryView)
            {
                PopulateGalleryView();
            }

            btnInstalled.Click += btnInstalled_Click;
            btnUpdateAvailable.Click += btnUpdateAvailable_Click;
            btnNewerThanList.Click += btnNewerThanList_Click;
        }

        bool NeedsDonation_Clicked = false;
        private void btnNewerThanList_Click(object sender, EventArgs e)
        {
            btnInstalled.Click -= btnInstalled_Click;
            btnUpdateAvailable.Click -= btnUpdateAvailable_Click;
            btnNewerThanList.Click -= btnNewerThanList_Click;

            if (downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = false;
            }

            if (updateAvailableClicked || upToDate_Clicked)
            {
                updateAvailableClicked = false;
                upToDate_Clicked = false;
                NeedsDonation_Clicked = false;
            }

            if (!NeedsDonation_Clicked)
            {
                NeedsDonation_Clicked = true;
                FilterListByColor(ColorDonateGame); // Needs donation color
            }
            else
            {
                NeedsDonation_Clicked = false;
                RestoreFullList();
            }

            // Update button visual states
            UpdateFilterButtonStates();

            // Refresh gallery view if active
            if (isGalleryView)
            {
                PopulateGalleryView();
            }

            btnInstalled.Click += btnInstalled_Click;
            btnUpdateAvailable.Click += btnUpdateAvailable_Click;
            btnNewerThanList.Click += btnNewerThanList_Click;
        }

        private void btnDownloaded_Click(object sender, EventArgs e)
        {
            if (_allItems == null || _allItems.Count == 0)
            {
                Logger.Log("btnDownloaded_Click: _allItems is null or empty");
                return;
            }

            // Clear other filter states
            updateAvailableClicked = false;
            upToDate_Clicked = false;
            NeedsDonation_Clicked = false;

            if (!downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = true;

                string downloadDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;

                if (!Directory.Exists(downloadDir))
                {
                    changeTitle("Download directory not found");
                    downloadedFilter_Clicked = false;
                    UpdateFilterButtonStates();
                    return;
                }

                changeTitle("Scanning local library...");

                // Get all subdirectories in the download folder
                var localDirs = new HashSet<string>(
                    Directory.GetDirectories(downloadDir)
                        .Select(d => Path.GetFileName(d)),
                    StringComparer.OrdinalIgnoreCase);

                // Filter _allItems to only those whose release name matches a local folder
                // and the folder contains at least one .apk file
                var localItems = _allItems
                    .Where(item =>
                    {
                        if (item.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex) return false;
                        string releaseName = item.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                        if (!localDirs.Contains(releaseName)) return false;
                        string folderPath = Path.Combine(downloadDir, releaseName);
                        try
                        {
                            return Directory.GetFiles(folderPath, "*.apk").Length > 0;
                        }
                        catch { return false; }
                    })
                    .ToList();

                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(localItems.ToArray());
                gamesListView.EndUpdate();

                if (isGalleryView)
                {
                    _galleryDataSource = localItems;
                    PopulateGalleryView();
                }

                changeTitle("");
            }
            else
            {
                downloadedFilter_Clicked = false;
                RestoreFullList();
            }

            UpdateFilterButtonStates();

            if (isGalleryView)
            {
                PopulateGalleryView();
            }
        }

        private void FilterListByColors(Color[] targetColors)
        {
            changeTitle("Filtering Game List...");

            if (_allItems == null || _allItems.Count == 0)
            {
                changeTitle("No games to filter");
                return;
            }

            var targetArgbs = new HashSet<int>(targetColors.Select(c => c.ToArgb()));

            var filteredItems = _allItems
                .Where(item => targetArgbs.Contains(item.ForeColor.ToArgb()))
                .ToList();

            gamesListView.BeginUpdate();
            gamesListView.Items.Clear();
            gamesListView.Items.AddRange(filteredItems.ToArray());
            gamesListView.EndUpdate();

            // Refresh gallery view if active - set data source before calling PopulateGalleryView
            if (isGalleryView)
            {
                _galleryDataSource = filteredItems;
                PopulateGalleryView();
            }

            changeTitle("");
        }

        private void FilterListByColor(Color targetColor)
        {
            changeTitle("Filtering Game List...");

            if (_allItems == null || _allItems.Count == 0)
            {
                changeTitle("No games to filter");
                return;
            }

            var filteredItems = _allItems
                .Where(item => item.ForeColor.ToArgb() == targetColor.ToArgb())
                .ToList();

            gamesListView.BeginUpdate();
            gamesListView.Items.Clear();
            gamesListView.Items.AddRange(filteredItems.ToArray());
            gamesListView.EndUpdate();

            // Refresh gallery view if active - set data source before calling PopulateGalleryView
            if (isGalleryView)
            {
                _galleryDataSource = filteredItems;
                PopulateGalleryView();
            }

            changeTitle("");
        }

        private void RestoreFullList()
        {
            if (_allItems == null || _allItems.Count == 0)
            {
                changeTitle("No games to restore");
                return;
            }

            if (downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = false;
                UpdateFilterButtonStates();
            }

            gamesListView.BeginUpdate();
            gamesListView.Items.Clear();
            gamesListView.Items.AddRange(_allItems.ToArray());
            gamesListView.EndUpdate();

            // Refresh gallery view if active
            if (isGalleryView && gamesGalleryView.Visible)
            {
                _galleryDataSource = _allItems;
                PopulateGalleryView();
            }

            changeTitle("");
        }

        public static void OpenDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                ProcessStartInfo p = new ProcessStartInfo
                {
                    Arguments = directoryPath,
                    FileName = "explorer.exe"
                };
                Process.Start(p);
            }
        }

        private void searchTextBox_Click(object sender, EventArgs e)
        {
            searchTextBox.Clear();
            _ = searchTextBox.Focus();
        }

        private void btnRunAdbCmd_Click(object sender, EventArgs e)
        {
            using (var adbForm = new AdbCommandForm())
            {
                if (adbForm.ShowDialog(this) == DialogResult.OK)
                {
                    string command = adbForm.Command;
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        string sentCommand = command.Replace("adb", "").Trim();
                        changeTitle($"Running ADB command: ADB {sentCommand}");
                        string output = ADB.RunAdbCommandToString(command).Output;

                        if (adbForm.ToggleUpdatesClicked)
                        {
                            bool isNowDisabled = output.Contains("disabled") || command.Contains("disable");
                            string status = isNowDisabled ? "disabled" : "enabled";
                            _ = JR.Utils.GUI.Forms.FlexibleMessageBox.Show(this,
                                $"OS Updates have been {status}.\n\nOutput:\n{output}");
                        }
                        else
                        {
                            _ = JR.Utils.GUI.Forms.FlexibleMessageBox.Show(this,
                                $"Ran ADB command: ADB {sentCommand}\r\nOutput:\r\n{output}");
                        }

                        changeTitle("");
                    }
                }
            }
        }

        private void btnOpenDownloads_Click(object sender, EventArgs e)
        {
            string pathToOpen = settings.CustomDownloadDir ? $"{settings.DownloadDir}" : $"{Environment.CurrentDirectory}";
            OpenDirectory(pathToOpen);
        }

        private void btnNoDevice_Click(object sender, EventArgs e)
        {
            bool currentStatus = settings.NodeviceMode || false;

            if (currentStatus)
            {
                settings.NodeviceMode = false;
                btnNoDevice.Text = "DISABLE SIDELOADING";
                UpdateStatusLabels();

                // No Device Mode is currently On. Toggle it Off (enable sideloading)
                // Ask user about delete after install preference
                DialogResult deleteResult = FlexibleMessageBox.Show(Program.form,
                    "Delete game files after install?",
                    "Sideloading enabled",
                    MessageBoxButtons.YesNo);

                settings.DeleteAllAfterInstall = (deleteResult == DialogResult.Yes);
            }
            else
            {
                settings.NodeviceMode = true;
                settings.DeleteAllAfterInstall = false;
                btnNoDevice.Text = "ENABLE SIDELOADING";
                UpdateStatusLabels();
            }

            settings.Save();
        }

        private async void btnLocalLibrary_Click(object sender, EventArgs e)
        {
            // Always show the startup dialog so the user can change config, switch mode, etc.
            using (var startupDialog = new StartupDialog())
            {
                var dialogResult = startupDialog.ShowDialog(this);

                if (dialogResult != DialogResult.OK || startupDialog.Choice == StartupDialog.StartupChoice.None)
                {
                    // User cancelled — stay in current mode
                    return;
                }

                if (startupDialog.Choice == StartupDialog.StartupChoice.Offline)
                {
                    // User chose offline / local library mode
                    if (isOffline)
                    {
                        // Already offline — nothing to do
                        return;
                    }

                    string dlDir;

                    // If a valid directory is already saved, use it directly
                    if (settings.CustomDownloadDir &&
                        !string.IsNullOrEmpty(settings.DownloadDir) &&
                        Directory.Exists(settings.DownloadDir))
                    {
                        dlDir = settings.DownloadDir;
                    }
                    else
                    {
                        // Prompt the user to choose which directory to scan
                        string initialDir = !string.IsNullOrEmpty(settings.DownloadDir) && Directory.Exists(settings.DownloadDir)
                            ? settings.DownloadDir
                            : Environment.CurrentDirectory;

                        var folderDialog = new FolderSelectDialog
                        {
                            Title = "Select the folder containing your downloaded games",
                            InitialDirectory = initialDir
                        };

                        if (!folderDialog.Show(Handle))
                            return;

                        dlDir = folderDialog.FileName;
                        settings.DownloadDir = dlDir;
                        settings.CustomDownloadDir = true;
                        settings.Save();
                    }

                    isOffline = true;

                    changeTitle("Scanning local library...");
                    progressBar.IsIndeterminate = true;
                    progressBar.OperationType = "Loading";

                    // Clear the game list immediately to signal the switch
                    gamesListView.BeginUpdate();
                    gamesListView.Items.Clear();
                    gamesListView.EndUpdate();
                    if (isGalleryView && _fastGallery != null && !_fastGallery.IsDisposed)
                    {
                        _fastGallery.UpdateItems(new List<ListViewItem>());
                    }

                    // Prevent background timers from repopulating the old list
                    isLoading = true;
                    loaded = false;
                    _allItems?.Clear();
                    _allItemsInitialized = false;
                    _galleryDataSource = null;
                    _searchIndex?.Clear();

                    // Kill rclone since we're going offline
                    RCLONE.killRclone();

                    // Clear existing games and scan the selected directory
                    await Task.Run(() =>
                    {
                        SideloaderRCLONE.games.Clear();
                        SideloaderRCLONE.ScanLocalGames(dlDir);
                    });

                    // Refresh game list
                    _allItemsInitialized = false;
                    _galleryDataSource = null;
                    listAppsBtn();
                    initListView(false);

                    remotesList.Size = System.Drawing.Size.Empty;

                    // Don't reset progressBar here — initListView will do it when truly done
                    UpdateStatusLabels();
                }
                else
                {
                    // User chose online mode (possibly with a new config)
                    // Kill any existing rclone processes so the new config is used cleanly
                    RCLONE.killRclone();

                    changeTitle("Reconnecting...");
                    progressBar.IsIndeterminate = true;
                    progressBar.OperationType = "Loading";

                    // Clear the game list immediately to signal the switch
                    gamesListView.BeginUpdate();
                    gamesListView.Items.Clear();
                    gamesListView.EndUpdate();
                    if (isGalleryView && _fastGallery != null && !_fastGallery.IsDisposed)
                    {
                        _fastGallery.UpdateItems(new List<ListViewItem>());
                    }

                    // Prevent background timers from repopulating the old list
                    // and purge every stale data source so nothing can briefly
                    // rebuild the offline list during the await chain below.
                    isLoading = true;
                    loaded = false;
                    _allItems?.Clear();
                    _allItemsInitialized = false;
                    _galleryDataSource = null;
                    _searchIndex?.Clear();
                    SideloaderRCLONE.games.Clear();

                    isOffline = false;

                    if (startupDialog.Choice == StartupDialog.StartupChoice.Online)
                    {
                        // Load the config that the startup dialog wrote
                        try
                        {
                            string configFilePath = Path.Combine(Environment.CurrentDirectory, "public.json");
                            if (File.Exists(configFilePath))
                            {
                                string configFileData = File.ReadAllText(configFilePath);
                                PublicConfig config = JsonConvert.DeserializeObject<PublicConfig>(configFileData);

                                if (config != null &&
                                    !string.IsNullOrWhiteSpace(config.BaseUri) &&
                                    !string.IsNullOrWhiteSpace(config.Password))
                                {
                                    PublicConfigFile = config;
                                    hasPublicConfig = true;
                                    DnsHelper.TestPublicConfigDns();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Failed to load config: {ex.Message}", LogLevel.ERROR);
                        }

                        if (!hasPublicConfig)
                        {
                            isOffline = true;
                            progressBar.IsIndeterminate = false;
                            changeTitle("Config invalid — still in offline mode");
                            return;
                        }

                        UsingPublicConfig = true;
                    }
                    else
                    {
                        // RcloneConfig — no public.json needed
                        UsingPublicConfig = false;
                        Logger.Log("Reconnecting with rclone download.config");
                    }

                    // Ensure dependencies are available
                    await Task.Run(() =>
                    {
                        changeTitle("Downloading Dependencies...");
                        GetDependencies.downloadFiles();
                        changeTitle("Initializing RCLONE...");
                        RCLONE.Init();
                    });

                    // Re-initialize mirrors and game list
                    await initMirrors();

                    if (UsingPublicConfig)
                    {
                        changeTitle("Updating Metadata...");
                        await Task.Run(() =>
                        {
                            SideloaderRCLONE.UpdateMetadataFromPublic();
                            SideloaderRCLONE.ProcessMetadataFromPublic();
                        });
                    }
                    else if (remotesList.Items.Count > 0)
                    {
                        changeTitle("Grabbing the Games List...");
                        await Task.Run(() => SideloaderRCLONE.initGames(currentRemote));
                    }

                    listAppsBtn();
                    _allItemsInitialized = false;
                    _galleryDataSource = null;
                    _suppressDonorsDialog = true;
                    initListView(false);

                    remotesList.Size = new System.Drawing.Size(121, 21);
                    changeTitlebarToDevice();
                    // Don't reset progressBar here — initListView will do it when truly done
                    UpdateStatusLabels();
                }
            }
        }

        private ListViewItem _rightClickedItem;
        private void gamesListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            _rightClickedItem = gamesListView.GetItemAt(e.X, e.Y);
            if (_rightClickedItem == null) return;

            gamesListView.SelectedItems.Clear();
            _rightClickedItem.Selected = true;

            UpdateFavoriteMenuItemText();
            UpdateOpenFolderMenuItemVisibility();
            favoriteGame.Show(gamesListView, e.Location);
        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {
            if (_rightClickedItem == null) return;

            string packageName = _rightClickedItem.SubItems[1].Text;

            if (settings.FavoritedGames.Contains(packageName))
                settings.RemoveFavoriteGame(packageName);
            else
                settings.AddFavoriteGame(packageName);

            UpdateFavoriteMenuItemText();

            // If currently viewing favorites, refresh the list to reflect the change
            bool isViewingFavorites = favoriteSwitcher.Text == "ALL";
            if (isViewingFavorites)
            {
                var favSet = new HashSet<string>(settings.FavoritedGames, StringComparer.OrdinalIgnoreCase);

                var favoriteItems = _allItems
                    .Where(item => item.SubItems.Count > 1 && favSet.Contains(item.SubItems[1].Text))
                    .ToList();

                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(favoriteItems.ToArray());
                gamesListView.EndUpdate();

                _galleryDataSource = favoriteItems;
                if (isGalleryView && _fastGallery != null)
                {
                    _fastGallery.RefreshFavoritesCache();
                    _fastGallery.UpdateItems(favoriteItems);
                }
            }
        }

        private void UpdateFavoriteMenuItemText()
        {
            if (_rightClickedItem == null) return;
            string packageName = _rightClickedItem.SubItems[1].Text;
            favoriteButton.Text = settings.FavoritedGames.Contains(packageName) ? "Remove from Favorites" : "★ Add to Favorites";
        }

        private void UpdateOpenFolderMenuItemVisibility()
        {
            if (_rightClickedItem == null || _rightClickedItem.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex)
            {
                openFolderButton.Visible = false;
                return;
            }

            string releaseName = _rightClickedItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
            string folderPath = Path.Combine(dlDir, releaseName);
            openFolderButton.Visible = Directory.Exists(folderPath);
        }

        private void openFolderButton_Click(object sender, EventArgs e)
        {
            if (_rightClickedItem == null || _rightClickedItem.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex)
                return;

            string releaseName = _rightClickedItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
            string folderPath = Path.Combine(dlDir, releaseName);
            OpenDirectory(folderPath);
        }

        private void favoriteSwitcher_Click(object sender, EventArgs e)
        {
            // Guard: ensure _allItems is populated
            if (_allItems == null || _allItems.Count == 0)
            {
                Logger.Log("favoriteSwitcher_Click: _allItems is null or empty");
                return;
            }

            bool showFavoritesOnly = favoriteSwitcher.Text == "FAVORITES";

            if (showFavoritesOnly)
            {
                favoriteSwitcher.Text = "ALL";

                var favSet = new HashSet<string>(settings.FavoritedGames, StringComparer.OrdinalIgnoreCase);

                var favoriteItems = _allItems
                    .Where(item => item.SubItems.Count > 1 && favSet.Contains(item.SubItems[1].Text))
                    .ToList();

                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(favoriteItems.ToArray());
                gamesListView.EndUpdate();

                _galleryDataSource = favoriteItems;
                if (isGalleryView && _fastGallery != null)
                {
                    _fastGallery.RefreshFavoritesCache();
                    _fastGallery.UpdateItems(favoriteItems);
                }
            }
            else
            {
                favoriteSwitcher.Text = "FAVORITES";

                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(_allItems.ToArray());
                gamesListView.EndUpdate();

                _galleryDataSource = _allItems;
                if (isGalleryView && _fastGallery != null)
                {
                    _fastGallery.RefreshFavoritesCache();
                    _fastGallery.UpdateItems(_allItems);
                }
            }

            // Clear other filter states
            updateAvailableClicked = false;
            upToDate_Clicked = false;
            NeedsDonation_Clicked = false;
            if (downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = false;
            }
            UpdateFilterButtonStates();
        }

        public async void UpdateQuestInfoPanel()
        {
            // Check if device is actually connected by checking Devices list
            bool hasDevice = Devices != null && Devices.Count > 0 && !Devices.Contains("unauthorized");
            bool bShowStatus = true;

            if ((!settings.NodeviceMode && hasDevice) || DeviceConnected)
            {
                try
                {
                    ADB.DeviceID = GetDeviceID();

                    // Get device model
                    string deviceModel = ADB.RunAdbCommandToString("shell getprop ro.product.model").Output.Trim();
                    if (string.IsNullOrEmpty(deviceModel))
                    {
                        deviceModel = "No Device Found";
                        SetQuestStorageProgress(0);
                        bShowStatus = false;
                    }

                    string firmware = ADB.RunAdbCommandToString("shell getprop ro.build.branch").Output.Trim(); // releases-oculus-14.0-v78
                    if (string.IsNullOrEmpty(firmware))
                    {
                        firmware = string.Empty;
                    }
                    else
                    {
                        firmware = Utilities.StringUtilities.RemoveEverythingBeforeFirst(firmware, "-v");
                        firmware = Utilities.StringUtilities.KeepOnlyNumbers(firmware);
                    }

                    // Get storage info
                    string storageOutput = ADB.RunAdbCommandToString("shell df /data").Output;
                    string[] lines = storageOutput.Split('\n');

                    long totalSpace = 0;
                    long usedSpace = 0;
                    long freeSpace = 0;

                    if (lines.Length > 1)
                    {
                        string[] parts = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 4)
                        {
                            if (long.TryParse(parts[1], out totalSpace) &&
                                long.TryParse(parts[2], out usedSpace) &&
                                long.TryParse(parts[3], out freeSpace))
                            {
                                totalSpace = totalSpace / 1024; // Convert to MB
                                usedSpace = usedSpace / 1024;
                                freeSpace = freeSpace / 1024;

                                // Format free space display
                                if (freeSpace > 1024)
                                {
                                    freeSpaceText = $"{(freeSpace / 1024.0):F2} GB AVAILABLE";
                                }
                                else
                                {
                                    freeSpaceText = $"{freeSpace} MB AVAILABLE";
                                }

                                freeSpaceTextDetailed = $"{(usedSpace / 1024.0):F0} GB OF {(totalSpace / 1024.0):F0} GB USED";

                                // Store raw value for queue space calculations
                                _deviceFreeSpaceMB = freeSpace;
                            }
                        }
                    }

                    // Calculate storage percentage used - clamped to 1%..100%
                    int storagePercentUsed = Math.Min(100, Math.Max(1, (100 - (totalSpace > 0 ? (int)((usedSpace * 100) / totalSpace) : 0))));

                    // Update UI on main thread
                    questInfoPanel.Invoke(() =>
                    {
                        string qinfo = deviceModel;
                        if (!string.IsNullOrEmpty(firmware))
                        {
                            qinfo = $"{qinfo} (v{firmware})";
                        }
                        questInfoLabel.Text = qinfo;
                        diskLabel.Text = freeSpaceText;
                        SetQuestStorageProgress(storagePercentUsed);
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log($"Unable to update quest info panel: {ex.Message}", LogLevel.ERROR);
                    questInfoPanel.Invoke(() =>
                    {
                        questInfoLabel.Text = "No Device Found";
                        SetQuestStorageProgress(0);
                        bShowStatus = false;
                    });
                }
            }
            else
            {
                questInfoPanel.Invoke(() =>
                {
                    questInfoLabel.Text = "No Device Found";
                    SetQuestStorageProgress(0);
                    bShowStatus = false;
                });
            }

            // Toggle visibility atomically on UI thread, based on device status
            questInfoPanel.Invoke(() =>
            {
                questStorageProgressBar.Visible = true;
                batteryLevImg.Visible = bShowStatus;
                batteryLabel.Visible = bShowStatus;
                questInfoLabel.Visible = true;
                diskLabel.Visible = bShowStatus;
            });
        }

        private void QuestInfoHoverEnter(object sender, EventArgs e)
        {
            // Only react when device info is shown
            if (!questStorageProgressBar.Visible) return;

            diskLabel.Text = freeSpaceTextDetailed;
        }

        // Restore the original baseline text ("XX GB FREE")
        private void QuestInfoHoverLeave(object sender, EventArgs e)
        {
            // Only react when device info is shown
            if (!questStorageProgressBar.Visible) return;

            // Ignore leave fired when moving between child controls inside the container
            var panel = questInfoPanel;
            var mouse = panel.PointToClient(MousePosition);
            if (panel.ClientRectangle.Contains(mouse)) return;

            diskLabel.Text = freeSpaceText;
        }

        private void questStorageProgressBar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Clear(questStorageProgressBar.BackColor);

            int w = questStorageProgressBar.ClientSize.Width;
            int h = questStorageProgressBar.ClientSize.Height;
            if (w <= 0 || h <= 0) return;

            // Rounded rectangle parameters (outer + fill share the same geometry).
            int radius = 10;
            Color bgColor = Color.FromArgb(28, 32, 38);
            Color borderColor = Color.FromArgb(60, 65, 75);

            // Modern fill gradient (adjust as desired)
            Color progressStart = Color.FromArgb(43, 160, 140);
            Color progressEnd = Color.FromArgb(30, 110, 95);

            // Build the rounded outer path (used for background and clipping).
            var outer = new RoundedRectangleF(w, h, radius);

            // Paint background
            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor, 1f))
            {
                g.FillPath(bgBrush, outer.Path);
                g.DrawPath(borderPen, outer.Path);
            }

            // Progress fraction and width
            float p = Math.Max(0f, Math.Min(1f, _questStorageProgress / 100f));
            int progressWidth = Math.Max(0, (int)(w * p));
            if (progressWidth <= 0) return;

            // Near-full rounding behavior:
            // As progress approaches 100%, progressively include the outer right-rounded corners.
            // Threshold start at 97%. At 97% -> straight cut; at 100% -> fully rounded outer corners.
            float t = 0f;
            if (p > 0.97f)
            {
                t = Math.Min(1f, (p - 0.97f) / 0.03f); // 0..1 over last 3%
            }

            // Build a clipping region for the fill: intersection of outer rounded rect with
            // the left rectangular portion [0..progressWidth]
            // plus a progressive right-cap region that extends into the rounded corners
            // with width up to 2*radius, scaled by t.
            Region fillClip = new Region(new Rectangle(0, 0, progressWidth, h));
            if (t > 0f)
            {
                int capWidth = (int)(t * (2 * radius));
                if (capWidth > 0)
                {
                    // This rectangle sits inside the area of the right rounded corners,
                    // so union-ing it with the rectangular clip allows the fill to
                    // progressively "wrap" into the curvature.
                    var rightCapRect = new Rectangle(w - (2 * radius), 0, capWidth, h);
                    fillClip.Union(rightCapRect);
                }
            }

            Region prevClip = g.Clip;
            try
            {
                // Final fill region = outer rounded path ∩ fillClip
                using (var outerRegion = new Region(outer.Path))
                {
                    outerRegion.Intersect(fillClip);
                    g.SetClip(outerRegion, CombineMode.Replace);

                    using (var progressBrush = new LinearGradientBrush(
                        new Rectangle(0, 0, Math.Max(1, progressWidth), h),
                        progressStart,
                        progressEnd,
                        LinearGradientMode.Horizontal))
                    {
                        // Fill the outer path; clipping ensures the fill grows left to right,
                        // stays fully flush to the outer geometry, and never exceeds it.
                        g.FillPath(progressBrush, outer.Path);
                    }
                }
            }
            finally
            {
                // Restore clip and re-stroke border to keep outline crisp
                g.Clip = prevClip;
                using (var borderPen = new Pen(borderColor, 1f))
                {
                    g.DrawPath(borderPen, outer.Path);
                }
            }
        }

        private void SetQuestStorageProgress(int percentage)
        {
            _questStorageProgress = Math.Max(0, Math.Min(100, percentage));

            if (questStorageProgressBar.InvokeRequired)
            {
                questStorageProgressBar.Invoke(new Action(() => questStorageProgressBar.Invalidate()));
            }
            else
            {
                questStorageProgressBar.Invalidate();
            }
        }

        private void btnViewToggle_Click(object sender, EventArgs e)
        {
            // Capture currently selected item before switching views
            string selectedReleaseName = null;
            if (gamesListView.SelectedItems.Count > 0)
            {
                var selectedItem = gamesListView.SelectedItems[0];
                if (selectedItem.SubItems.Count > 1)
                {
                    selectedReleaseName = selectedItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                }
            }
            else if (isGalleryView && _fastGallery != null && _fastGallery._selectedIndex >= 0)
            {
                // Capture selection from gallery view
                var galleryItem = _fastGallery.GetItemAtIndex(_fastGallery._selectedIndex);
                if (galleryItem != null && galleryItem.SubItems.Count > 1)
                {
                    selectedReleaseName = galleryItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                }
            }

            // Capture current sort state before switching
            if (isGalleryView && _fastGallery != null)
            {
                // Save gallery sort state
                _sharedSortField = _fastGallery.CurrentSortField;
                _sharedSortDirection = _fastGallery.CurrentSortDirection;

                // Flip popularity direction when going from gallery to list due to flipped underlying logic
                // Gallery: Descending = Most Popular first
                // List: Ascending = Most Popular first
                if (_sharedSortField == SortField.Popularity)
                {
                    _sharedSortDirection = _sharedSortDirection == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
            }
            else if (!isGalleryView && _listViewRenderer != null && lvwColumnSorter != null)
            {
                // Save list view sort state
                _sharedSortField = ColumnIndexToSortField(lvwColumnSorter.SortColumn);
                _sharedSortDirection = lvwColumnSorter.Order == SortOrder.Ascending
                    ? SortDirection.Ascending
                    : SortDirection.Descending;

                // Flip popularity direction when going from list to gallery due to flipped underlying logic
                // List: Ascending = Most Popular first
                // Gallery: Descending = Most Popular first
                if (_sharedSortField == SortField.Popularity)
                {
                    _sharedSortDirection = _sharedSortDirection == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
            }

            isGalleryView = !isGalleryView;

            // Save user preference
            settings.UseGalleryView = isGalleryView;
            settings.Save();

            if (isGalleryView)
            {
                btnViewToggle.Text = "LIST";
                gamesListView.Visible = false;
                gamesGalleryView.Visible = true;

                // Only populate if data is available, otherwise it will be populated when initListView completes
                if (_allItems != null && _allItems.Count > 0)
                {
                    PopulateGalleryView();

                    // Scroll to the previously selected item in gallery view
                    if (!string.IsNullOrEmpty(selectedReleaseName) && _fastGallery != null)
                    {
                        _fastGallery.ScrollToPackage(selectedReleaseName);
                    }
                }
            }
            else
            {
                btnViewToggle.Text = "GALLERY";
                gamesGalleryView.Visible = false;
                gamesListView.Visible = true;
                CleanupGalleryView();

                // Apply shared sort state to list view
                ApplySortToListView();

                // Scroll to the previously selected item in list view
                if (!string.IsNullOrEmpty(selectedReleaseName))
                {
                    foreach (ListViewItem item in gamesListView.Items)
                    {
                        if (item.SubItems.Count > 1 &&
                            item.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text.Equals(selectedReleaseName, StringComparison.OrdinalIgnoreCase))
                        {
                            item.Selected = true;
                            item.Focused = true;
                            item.EnsureVisible();
                            break;
                        }
                    }
                }
            }
        }

        private SortField ColumnIndexToSortField(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0: return SortField.Name;
                case 4: return SortField.LastUpdated;
                case 5: return SortField.Size;
                case 6: return SortField.Popularity;
                default: return SortField.Name;
            }
        }

        private int SortFieldToColumnIndex(SortField field)
        {
            switch (field)
            {
                case SortField.Name: return 0;
                case SortField.LastUpdated: return 4;
                case SortField.Size: return 5;
                case SortField.Popularity: return 6;
                default: return 0;
            }
        }

        private void ApplySortToListView()
        {
            if (_listViewRenderer == null || lvwColumnSorter == null) return;

            int columnIndex = SortFieldToColumnIndex(_sharedSortField);
            SortOrder order = _sharedSortDirection == SortDirection.Ascending
                ? SortOrder.Ascending
                : SortOrder.Descending;

            _listViewRenderer.ApplySort(columnIndex, order);
        }

        private void PopulateGalleryView()
        {
            // If _galleryDataSource was already set (by search or filter), use it
            // Otherwise, determine what to display based on current state
            if (_galleryDataSource == null)
            {
                if (updateAvailableClicked || upToDate_Clicked || NeedsDonation_Clicked)
                {
                    _galleryDataSource = gamesListView.Items.Cast<ListViewItem>().ToList();
                }
                else
                {
                    _galleryDataSource = _allItems ?? gamesListView.Items.Cast<ListViewItem>().ToList();
                }
            }

            if (_galleryDataSource == null)
            {
                _galleryDataSource = new List<ListViewItem>();
            }

            // If gallery already exists, just update the data source
            if (_fastGallery != null && !_fastGallery.IsDisposed)
            {
                _fastGallery.UpdateItems(_galleryDataSource);
                return;
            }

            // First time creation
            CleanupGalleryView();

            int targetWidth = gamesGalleryView.ClientSize.Width > 0 ? gamesGalleryView.ClientSize.Width : gamesGalleryView.Width;
            int targetHeight = gamesGalleryView.ClientSize.Height > 0 ? gamesGalleryView.ClientSize.Height : gamesGalleryView.Height;

            if (targetHeight <= 0) targetHeight = 350;
            if (targetWidth <= 0) targetWidth = 1145;

            gamesGalleryView.AutoScroll = false;
            gamesGalleryView.Padding = Padding.Empty;
            gamesGalleryView.Controls.Clear();

            _fastGallery = new FastGalleryPanel(_galleryDataSource, TILE_WIDTH, TILE_HEIGHT, TILE_SPACING, targetWidth, targetHeight);
            _fastGallery.TileClicked += FastGallery_TileClicked;
            _fastGallery.TileDoubleClicked += FastGallery_TileDoubleClicked;
            _fastGallery.TileDeleteClicked += FastGallery_TileDeleteClicked;
            _fastGallery.SortChanged += FastGallery_SortChanged;
            _fastGallery.TileHovered += FastGallery_TileHovered;

            // Apply current shared sort state to gallery
            _fastGallery.SetSortState(_sharedSortField, _sharedSortDirection);

            gamesGalleryView.Controls.Add(_fastGallery);
            _fastGallery.Anchor = AnchorStyles.None;

            gamesGalleryView.Resize += GamesGalleryView_Resize;
        }

        private async void FastGallery_TileDeleteClicked(object sender, int index)
        {
            if (index < 0 || _fastGallery == null) return;

            var item = _fastGallery.GetItemAtIndex(index);
            if (item == null) return;

            await UninstallGameAsync(item);
        }

        private void FastGallery_SortChanged(object sender, SortField field)
        {
            // Update shared state from gallery
            if (_fastGallery != null)
            {
                _sharedSortField = _fastGallery.CurrentSortField;
                _sharedSortDirection = _fastGallery.CurrentSortDirection;
            }

            // Save sort state
            SaveWindowState();
        }

        private void FastGallery_TileHovered(object sender, string releaseName)
        {
            if (string.IsNullOrEmpty(releaseName)) return;

            string notePath = Path.Combine(SideloaderRCLONE.NotesFolder, $"{releaseName}.txt");
            UpdateReleaseNotes(notePath);
        }

        private void GamesGalleryView_Resize(object sender, EventArgs e)
        {
            if (_fastGallery != null && !_fastGallery.IsDisposed)
            {
                _fastGallery.Size = gamesGalleryView.ClientSize;
            }
        }

        private void CleanupGalleryView()
        {
            gamesGalleryView.Resize -= GamesGalleryView_Resize;

            if (_fastGallery != null)
            {
                _fastGallery.Dispose();
                _fastGallery = null;
            }
        }

        private void FastGallery_TileClicked(object sender, int itemIndex)
        {
            if (itemIndex < 0 || _fastGallery == null) return;

            // Get the actual item from the gallery's current (sorted) list
            var item = _fastGallery.GetItemAtIndex(itemIndex);
            if (item == null || item.SubItems.Count <= 2) return;

            string releaseName = item.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            string gameName = item.SubItems[SideloaderRCLONE.GameNameIndex].Text;

            // Clear all selections first - must deselect each item individually
            // because SelectedItems.Clear() doesn't work reliably when ListView is hidden
            foreach (ListViewItem listItem in gamesListView.Items)
            {
                listItem.Selected = false;
            }

            // Find and select the matching item in gamesListView using release name
            foreach (ListViewItem listItem in gamesListView.Items)
            {
                if (listItem.SubItems.Count > 1 &&
                    listItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text.Equals(releaseName, StringComparison.OrdinalIgnoreCase))
                {
                    listItem.Selected = true;
                    listItem.EnsureVisible();
                    break;
                }
            }

            // Load release notes
            string notePath = Path.Combine(SideloaderRCLONE.NotesFolder, $"{releaseName}.txt");
            UpdateReleaseNotes(notePath);
            UpdateNotesScrollBar();
        }

        private void UpdateReleaseNotes(string notes)
        {
            if (File.Exists(notes))
            {
                // Reset to normal styling for actual content
                notesRichTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                notesRichTextBox.ForeColor = Color.White;
                notesRichTextBox.Text = File.ReadAllText(notes);
                notesRichTextBox.SelectAll();
                notesRichTextBox.SelectionAlignment = HorizontalAlignment.Left;
                notesRichTextBox.DeselectAll();
            }
            else
            {
                // Show placeholder with queue-matching style (grey, italic, centered)
                notesRichTextBox.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
                notesRichTextBox.ForeColor = Color.FromArgb(140, 140, 140);
                notesRichTextBox.Text = "\n\n\n\n\nTip: Press F1 to see all shortcuts\n\nDrag and drop APKs or folders to install";
                notesRichTextBox.SelectAll();
                notesRichTextBox.SelectionAlignment = HorizontalAlignment.Center;
                notesRichTextBox.DeselectAll();
            }
            UpdateNotesScrollBar();
        }

        private void FastGallery_TileDoubleClicked(object sender, int itemIndex)
        {
            if (itemIndex < 0 || _fastGallery == null) return;

            // Get the actual item from the gallery's current (sorted) list
            var item = _fastGallery.GetItemAtIndex(itemIndex);
            if (item == null || item.SubItems.Count <= 2) return;

            // Use release name to match the correct entry
            string releaseName = item.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;

            // Clear all selections first - must deselect each item individually
            // because SelectedItems.Clear() doesn't work reliably when ListView is hidden
            foreach (ListViewItem listItem in gamesListView.Items)
            {
                listItem.Selected = false;
            }

            // Find and select the matching item in gamesListView by release name
            foreach (ListViewItem listItem in gamesListView.Items)
            {
                if (listItem.SubItems.Count > 1 &&
                    listItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text.Equals(releaseName, StringComparison.OrdinalIgnoreCase))
                {
                    listItem.Selected = true;
                    downloadInstallGameButton_Click(downloadInstallGameButton, EventArgs.Empty);
                    break;
                }
            }
        }

        private void OpenDownloadsFolderIcon_Paint(object sender, PaintEventArgs e)
        {
            var pb = (PictureBox)sender;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = pb.Width;
            int h = pb.Height;
            int radius = 5;

            // Read background from btnDownloaded so it stays in sync with filter state
            Color bgColor = btnDownloaded.Inactive1;
            Color strokeColor = btnDownloaded.StrokeColor;

            var roundedRect = new RoundedRectangleF(w, h, radius);
            g.FillRectangle(Brushes.Transparent, pb.ClientRectangle);

            Rectangle rect = new Rectangle(0, 0, w, h);
            using (var fillBrush = new LinearGradientBrush(rect, bgColor, bgColor, 90f))
            {
                g.FillPath(fillBrush, roundedRect.Path);
            }

            using (var pen = new Pen(strokeColor, 1))
            {
                var strokeRect = new RoundedRectangleF(w, h, radius);
                g.DrawPath(pen, strokeRect.Path);
            }

            // Draw folder icon: color matches btnDownloaded text, slight upscale on hover
            var icon = Properties.Resources.FolderIcon;
            if (icon != null)
            {
                int iconSize = _folderIconHovered ? 16 : 14;
                int iconX = (w - iconSize) / 2;
                int iconY = (h - iconSize) / 2;

                Color tintColor = btnDownloaded.ForeColor;

                float r = tintColor.R / 255f;
                float gr = tintColor.G / 255f;
                float b = tintColor.B / 255f;

                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
                {
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { r, gr, b, 0, 1 }
                });

                using (var attributes = new System.Drawing.Imaging.ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(icon,
                        new Rectangle(iconX, iconY, iconSize, iconSize),
                        0, 0, icon.Width, icon.Height,
                        GraphicsUnit.Pixel, attributes);
                }
            }
        }

        private void ListViewUninstallButton_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(gamesListView.BackColor);

            int w = _listViewUninstallButton.Width;
            int h = _listViewUninstallButton.Height;
            var btnRect = new Rectangle(0, 0, w, h);

            // Colors matching GalleryPanel's uninstall button
            Color bgColor = _listViewUninstallButtonHovered
                ? Color.FromArgb(255, 220, 70, 70)   // DeleteButtonHoverBg
                : Color.FromArgb(255, 180, 50, 50);  // DeleteButtonBg

            // Draw rectangle background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, btnRect);
            }

            // Draw trash icon
            int iconPadding = 3;
            int iconX = iconPadding;
            int iconY = iconPadding - 1;
            int iconSize = w - iconPadding * 2;

            using (var pen = new Pen(Color.White, 1.5f))
            {
                // Trash can body
                int bodyTop = iconY + 4;
                int bodyBottom = iconY + iconSize;
                int bodyLeft = iconX + 2;
                int bodyRight = iconX + iconSize - 2;

                // Draw body outline (trapezoid-ish shape)
                g.DrawLine(pen, bodyLeft, bodyTop, bodyLeft + 1, bodyBottom);
                g.DrawLine(pen, bodyLeft + 1, bodyBottom, bodyRight - 1, bodyBottom);
                g.DrawLine(pen, bodyRight - 1, bodyBottom, bodyRight, bodyTop);

                // Draw lid
                g.DrawLine(pen, iconX, bodyTop, iconX + iconSize, bodyTop);

                // Draw handle on lid
                int handleLeft = iconX + iconSize / 2 - 3;
                int handleRight = iconX + iconSize / 2 + 3;
                int handleTop = iconY + 1;
                g.DrawLine(pen, handleLeft, bodyTop, handleLeft, handleTop);
                g.DrawLine(pen, handleLeft, handleTop, handleRight, handleTop);
                g.DrawLine(pen, handleRight, handleTop, handleRight, bodyTop);

                // Draw vertical lines inside trash
                int lineY1 = bodyTop + 3;
                int lineY2 = bodyBottom - 3;
                g.DrawLine(pen, iconX + iconSize / 2, lineY1, iconX + iconSize / 2, lineY2);
                if (iconSize > 10)
                {
                    g.DrawLine(pen, iconX + iconSize / 2 - 4, lineY1, iconX + iconSize / 2 - 4, lineY2);
                    g.DrawLine(pen, iconX + iconSize / 2 + 4, lineY1, iconX + iconSize / 2 + 4, lineY2);
                }
            }
        }

        private void gamesPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // Only draw placeholder if no image is loaded
            if (gamesPictureBox.BackgroundImage != null &&
                gamesPictureBox.BackgroundImage.Width > 1 &&
                gamesPictureBox.BackgroundImage.Height > 1)
            {
                return;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var thumbRect = new Rectangle(0, 0, gamesPictureBox.Width, gamesPictureBox.Height);

            // Draw placeholder background
            using (var brush = new SolidBrush(Color.FromArgb(35, 35, 40)))
            {
                g.FillRectangle(brush, thumbRect);
            }

            // When disclaimer is gone
            if (freeDisclaimer.Enabled == false)
            {
                // Draw emoji placeholder
                using (var textBrush = new SolidBrush(Color.FromArgb(70, 70, 80)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("🎮", new Font("Segoe UI Emoji", 32f), textBrush, thumbRect, sf);
                }
            }
        }

        private void webViewPlaceholderPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int radius = 8;
            var rect = new Rectangle(0, 0, webViewPlaceholderPanel.Width - 1, webViewPlaceholderPanel.Height - 1);
            Color panelColor = Color.FromArgb(24, 26, 30);
            Color cornerBgColor = Color.FromArgb(32, 35, 45);

            // Clear with corner background color first
            g.Clear(cornerBgColor);

            using (var path = CreateRoundedRectPath(rect, radius))
            {
                // Draw rounded background
                using (var brush = new SolidBrush(panelColor))
                {
                    g.FillPath(brush, path);
                }
            }

            // Apply rounded region to clip the panel
            using (var regionPath = CreateRoundedRectPath(new Rectangle(0, 0, webViewPlaceholderPanel.Width, webViewPlaceholderPanel.Height), radius))
            {
                webViewPlaceholderPanel.Region = new Region(regionPath);
            }

            // Draw emoji placeholder
            using (var textBrush = new SolidBrush(Color.FromArgb(60, 65, 70)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("🎮", new Font("Segoe UI Emoji", 32f), textBrush, rect, sf);
            }
        }

        public void ShowVideoPlaceholder()
        {
            webViewPlaceholderPanel.Visible = true;
            webViewPlaceholderPanel.BringToFront();
        }

        public void HideVideoPlaceholder()
        {
            webViewPlaceholderPanel.Visible = false;
        }

        private void SubscribeToHoverEvents(Control parent)
        {
            parent.MouseEnter += QuestInfoHoverEnter;
            parent.MouseLeave += QuestInfoHoverLeave;

            foreach (Control child in parent.Controls)
            {
                SubscribeToHoverEvents(child);
            }
        }

        private string ShowInstalledAppSelector(string promptText = "Select an installed app...")
        {
            // Refresh the list of installed apps
            listAppsBtn();

            if (m_combo.Items.Count == 0)
            {
                FlexibleMessageBox.Show(Program.form, "No installed apps found on the device.");
                return null;
            }

            // Create a dialog to show the combo selection
            using (Form dialog = new Form())
            {
                dialog.Text = promptText;
                dialog.Size = new Size(450, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = promptText,
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var comboBox = new ComboBox
                {
                    Location = new Point(15, 40),
                    Size = new Size(400, 24),
                    DropDownStyle = ComboBoxStyle.DropDown,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Standard
                };

                // Copy items from m_combo
                foreach (var item in m_combo.Items)
                {
                    comboBox.Items.Add(item);
                }

                var okButton = CreateStyledButton("OK", DialogResult.OK, new Point(255, 75));
                var cancelButton = CreateStyledButton("Cancel", DialogResult.Cancel, new Point(340, 75), false);

                dialog.Controls.AddRange(new Control[] { label, comboBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) == DialogResult.OK && comboBox.SelectedIndex != -1)
                {
                    return comboBox.SelectedItem.ToString();
                }
            }

            return null;
        }

        private string ShowDeviceSelector(string promptText = "Select a device")
        {
            // Refresh the list of devices first
            string output = ADB.RunAdbCommandToString("devices").Output;
            string[] lines = output.Split('\n');

            var deviceList = new List<string>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length > 0 && !string.IsNullOrWhiteSpace(line))
                {
                    string deviceId = line.Split('\t')[0];
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        deviceList.Add(deviceId);
                    }
                }
            }

            if (deviceList.Count == 0)
            {
                FlexibleMessageBox.Show(Program.form, "No devices found. Please connect a device and try again.");
                return null;
            }

            // If only one device, return it directly
            if (deviceList.Count == 1)
            {
                // Update internal combo for compatibility
                devicesComboBox.Items.Clear();
                devicesComboBox.Items.Add(deviceList[0]);
                devicesComboBox.SelectedIndex = 0;
                FlexibleMessageBox.Show(this, $"Selected device: {deviceList[0]}\n\nNo other devices detected");
                return deviceList[0];
            }

            // Create a dialog to show the device selection
            using (Form dialog = new Form())
            {
                dialog.Text = promptText;
                dialog.Size = new Size(400, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = promptText,
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var comboBox = new ComboBox
                {
                    Location = new Point(15, 40),
                    Size = new Size(350, 24),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Standard
                };

                // Add devices to combo
                foreach (var device in deviceList)
                {
                    comboBox.Items.Add(device);
                }

                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }

                var okButton = CreateStyledButton("OK", DialogResult.OK, new Point(205, 75));
                var cancelButton = CreateStyledButton("Cancel", DialogResult.Cancel, new Point(290, 75), false);

                dialog.Controls.AddRange(new Control[] { label, comboBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) == DialogResult.OK && comboBox.SelectedIndex != -1)
                {
                    string selectedDevice = comboBox.SelectedItem.ToString();

                    // Update internal combo for compatibility
                    devicesComboBox.Items.Clear();
                    foreach (var device in deviceList)
                    {
                        devicesComboBox.Items.Add(device);
                    }
                    devicesComboBox.SelectedItem = selectedDevice;

                    return selectedDevice;
                }
            }

            return null;
        }

        private void activeMirrorLabel_Click(object sender, EventArgs e)
        {
            if (remotesList.Items.Count <= 1) return;

            // If menu just closed from this same click, treat as toggle-close
            if ((DateTime.Now - _mirrorMenuClosedAt).TotalMilliseconds < 300)
                return;

            mirrorContextMenu.Items.Clear();
            mirrorContextMenu.Renderer = new CenteredMenuRenderer();
            mirrorContextMenu.AutoSize = true;
            mirrorContextMenu.MinimumSize = new Size(activeMirrorLabel.Width, 0);
            mirrorContextMenu.Closed -= mirrorContextMenu_Closed;
            mirrorContextMenu.Closed += mirrorContextMenu_Closed;

            string currentMirror = null;
            if (UsingPublicConfig)
                currentMirror = "Public";
            else if (remotesList.SelectedItem != null)
                currentMirror = remotesList.SelectedItem.ToString();

            int itemCount = 0;
            foreach (var item in remotesList.Items)
            {
                string mirrorName = item.ToString();
                bool isCurrent = mirrorName == currentMirror;
                string displayText = isCurrent ? "Mirror: " + mirrorName : "Mirror: " + mirrorName;
                var menuItem = new ToolStripMenuItem(displayText);
                menuItem.BackColor = Color.FromArgb(40, 42, 48);
                menuItem.ForeColor = isCurrent
                    ? Color.FromArgb(93, 203, 173)
                    : Color.FromArgb(200, 200, 200);
                menuItem.Font = isCurrent
                    ? new Font("Segoe UI", 8F, FontStyle.Bold)
                    : new Font("Segoe UI", 8F);
                menuItem.Padding = new Padding(4, 2, 4, 2);
                menuItem.Tag = mirrorName;
                menuItem.Click += mirrorMenuItem_Click;
                mirrorContextMenu.Items.Add(menuItem);
                itemCount++;
            }

            // Force items to fill full menu width so the entire row is clickable/hoverable
            int menuWidth = activeMirrorLabel.Width;
            foreach (ToolStripItem tsItem in mirrorContextMenu.Items)
            {
                tsItem.AutoSize = false;
                tsItem.Width = menuWidth;
            }

            mirrorContextMenu.Show(activeMirrorLabel, 0, activeMirrorLabel.Height);
        }

        private void mirrorContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _mirrorMenuClosedAt = DateTime.Now;
        }

        private async void mirrorMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem?.Tag == null) return;
            if (isLoading) return;

            string selected = menuItem.Tag.ToString();

            // Find the target index
            int targetIndex = -1;
            for (int i = 0; i < remotesList.Items.Count; i++)
            {
                if (remotesList.Items[i].ToString() == selected)
                {
                    targetIndex = i;
                    break;
                }
            }
            if (targetIndex < 0) return;

            // Save previous working state for revert on failure
            int prevIndex = remotesList.SelectedIndex;
            string prevRemote = currentRemote;
            bool prevUsingPublic = UsingPublicConfig;

            // Reset mirror cycling state since user explicitly chose a mirror
            quotaTries = 0;
            reset = false;
            steps = 0;

            // Set manual switch flag to prevent RCLONE cascade
            _isManualMirrorSwitch = true;

            // Suppress SelectedIndexChanged — we handle the refresh ourselves
            _suppressMirrorRefresh = true;
            try
            {
                remotesList.SelectedIndex = targetIndex;
            }
            finally
            {
                _suppressMirrorRefresh = false;
            }

            // Set mirror state directly
            if (selected == "Public")
            {
                UsingPublicConfig = true;
            }
            else
            {
                UsingPublicConfig = false;
                currentRemote = selected;
            }

            settings.selectedMirror = selected;
            settings.Save();
            UpdateStatusLabels();

            // Cache current game list so revert is instant (no re-download)
            var cachedGames = new List<string[]>(SideloaderRCLONE.games);

            // Try refreshing with the new mirror
            try
            {
                if (UsingPublicConfig)
                {
                    changeTitle("Updating Metadata...");
                    await Task.Run(() =>
                    {
                        SideloaderRCLONE.UpdateMetadataFromPublic();
                        SideloaderRCLONE.ProcessMetadataFromPublic();
                    });
                }

                await refreshCurrentMirror("Refreshing App List...");
            }
            catch (Exception ex)
            {
                Logger.Log($"Mirror switch error: {ex.Message}", LogLevel.ERROR);
            }

            bool success = SideloaderRCLONE.games.Count > 5;

            _isManualMirrorSwitch = false;

            if (!success)
            {
                // Revert to previous working mirror
                _suppressMirrorRefresh = true;
                try
                {
                    remotesList.SelectedIndex = prevIndex;
                }
                finally
                {
                    _suppressMirrorRefresh = false;
                }

                currentRemote = prevRemote;
                UsingPublicConfig = prevUsingPublic;
                isOffline = false;

                settings.selectedMirror = remotesList.SelectedItem?.ToString() ?? "Public";
                settings.Save();

                // Restore cached game list — no network call needed
                SideloaderRCLONE.games.Clear();
                SideloaderRCLONE.games.AddRange(cachedGames);

                // Rebuild UI from cached data
                changeTitle("Restoring previous mirror...");
                await RefreshGameListAsync();
                changeTitle("");
                UpdateStatusLabels();

                FlexibleMessageBox.Show(this,
                    $"Mirror \"{selected}\" is not reachable.\nReverted to the previous working mirror.",
                    "Mirror Unavailable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                UpdateStatusLabels();
            }
        }

        private void activeMirrorLabel_Paint(object sender, PaintEventArgs e)
        {
            var lbl = (Label)sender;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Clear entire label with parent background so corners blend in
            g.Clear(lbl.Parent != null ? lbl.Parent.BackColor : Color.FromArgb(20, 24, 29));

            var rect = new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1);
            using (var brush = new SolidBrush(_mirrorPillColor))
                g.FillRectangle(brush, rect);
            using (var pen = new Pen(Color.FromArgb(50, 55, 65)))
                g.DrawRectangle(pen, rect);

            // Draw text
            TextRenderer.DrawText(g, lbl.Text, lbl.Font, new Rectangle(lbl.Padding.Left, 0, lbl.Width - lbl.Padding.Horizontal, lbl.Height), lbl.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // Always draw dropdown arrow on the right
            int arrowSize = 4;
            int arrowX = lbl.Width - 14;
            int arrowY = (lbl.Height - arrowSize) / 2;
            var arrowPoints = new Point[]
            {
                new Point(arrowX, arrowY),
                new Point(arrowX + arrowSize * 2, arrowY),
                new Point(arrowX + arrowSize, arrowY + arrowSize)
            };
            using (var brush = new SolidBrush(lbl.ForeColor))
                g.FillPolygon(brush, arrowPoints);
        }

        private void activeMirrorLabel_MouseEnter(object sender, EventArgs e)
        {
            _mirrorPillColor = Color.FromArgb(42, 47, 58);
            activeMirrorLabel.ForeColor = Color.FromArgb(130, 225, 200);
            activeMirrorLabel.Invalidate();
        }

        private void activeMirrorLabel_MouseLeave(object sender, EventArgs e)
        {
            _mirrorPillColor = Color.FromArgb(32, 36, 44);
            activeMirrorLabel.ForeColor = Color.FromArgb(93, 203, 173);
            activeMirrorLabel.Invalidate();
        }

        private void deviceIdLabel_Click(object sender, EventArgs e)
        {
            // Refresh the list of devices
            string output = ADB.RunAdbCommandToString("devices").Output;
            string[] lines = output.Split('\n');

            var deviceList = new List<string>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length > 0 && !string.IsNullOrWhiteSpace(line))
                {
                    string deviceId = line.Split('\t')[0];
                    if (!string.IsNullOrEmpty(deviceId))
                        deviceList.Add(deviceId);
                }
            }

            if (deviceList.Count == 0)
            {
                FlexibleMessageBox.Show(Program.form, "No devices found. Please connect a device and try again.");
                return;
            }

            if ((DateTime.Now - _deviceMenuClosedAt).TotalMilliseconds < 300)
                return;

            deviceContextMenu.Items.Clear();
            deviceContextMenu.Renderer = new CenteredMenuRenderer();
            deviceContextMenu.AutoSize = true;
            deviceContextMenu.MinimumSize = new Size(deviceIdLabel.Width, 0);
            deviceContextMenu.Closed -= deviceContextMenu_Closed;
            deviceContextMenu.Closed += deviceContextMenu_Closed;

            string currentDevice = ADB.DeviceID;

            foreach (var device in deviceList)
            {
                bool isCurrent = device == currentDevice;
                string displayText = "Device: " + device;
                var menuItem = new ToolStripMenuItem(displayText);
                menuItem.BackColor = Color.FromArgb(40, 42, 48);
                menuItem.ForeColor = isCurrent
                    ? Color.FromArgb(93, 203, 173)
                    : Color.FromArgb(200, 200, 200);
                menuItem.Font = isCurrent
                    ? new Font("Segoe UI", 8F, FontStyle.Bold)
                    : new Font("Segoe UI", 8F);
                menuItem.Padding = new Padding(4, 2, 4, 2);
                menuItem.Tag = device;
                menuItem.Click += deviceMenuItem_Click;
                deviceContextMenu.Items.Add(menuItem);
            }

            int menuWidth = deviceIdLabel.Width;
            foreach (ToolStripItem tsItem in deviceContextMenu.Items)
            {
                tsItem.AutoSize = false;
                tsItem.Width = menuWidth;
            }

            deviceContextMenu.Show(deviceIdLabel, 0, deviceIdLabel.Height);
        }

        private void deviceContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _deviceMenuClosedAt = DateTime.Now;
        }

        private void deviceMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem?.Tag == null) return;

            string selectedDevice = menuItem.Tag.ToString();
            ADB.DeviceID = selectedDevice;

            devicesComboBox.Items.Clear();
            devicesComboBox.Items.Add(selectedDevice);
            devicesComboBox.SelectedIndex = 0;

            changeTitlebarToDevice();
            showAvailableSpace();
            changeTitle($"Selected device: {selectedDevice}", true);
            UpdateStatusLabels();
        }

        private void deviceIdLabel_Paint(object sender, PaintEventArgs e)
        {
            var lbl = (Label)sender;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            g.Clear(lbl.Parent != null ? lbl.Parent.BackColor : Color.FromArgb(20, 24, 29));

            var rect = new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1);
            using (var brush = new SolidBrush(_devicePillColor))
                g.FillRectangle(brush, rect);
            using (var pen = new Pen(Color.FromArgb(50, 55, 65)))
                g.DrawRectangle(pen, rect);

            TextRenderer.DrawText(g, lbl.Text, lbl.Font, new Rectangle(lbl.Padding.Left, 0, lbl.Width - lbl.Padding.Horizontal, lbl.Height), lbl.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            int arrowSize = 4;
            int arrowX = lbl.Width - 14;
            int arrowY = (lbl.Height - arrowSize) / 2;
            var arrowPoints = new Point[]
            {
                new Point(arrowX, arrowY),
                new Point(arrowX + arrowSize * 2, arrowY),
                new Point(arrowX + arrowSize, arrowY + arrowSize)
            };
            using (var brush = new SolidBrush(lbl.ForeColor))
                g.FillPolygon(brush, arrowPoints);
        }

        private void deviceIdLabel_MouseEnter(object sender, EventArgs e)
        {
            _devicePillColor = Color.FromArgb(42, 47, 58);
            deviceIdLabel.ForeColor = Color.FromArgb(130, 225, 200);
            deviceIdLabel.Invalidate();
        }

        private void deviceIdLabel_MouseLeave(object sender, EventArgs e)
        {
            _devicePillColor = Color.FromArgb(32, 36, 44);
            deviceIdLabel.ForeColor = Color.FromArgb(93, 203, 173);
            deviceIdLabel.Invalidate();
        }

        private string ShowMirrorSelector(string promptText = "Select a mirror")
        {
            if (remotesList.Items.Count == 0)
            {
                FlexibleMessageBox.Show(this, "No mirrors available.");
                return null;
            }

            // If only one mirror, just inform the user
            if (remotesList.Items.Count == 1)
            {
                string onlyMirror = remotesList.Items[0].ToString();
                FlexibleMessageBox.Show(this, $"Currently using mirror: {onlyMirror}\n\nNo other mirrors available");
                return onlyMirror;
            }

            using (Form dialog = new Form())
            {
                dialog.Text = promptText;
                dialog.Size = new Size(350, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = $"{promptText} (Current: {remotesList.SelectedItem ?? "None"})",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var comboBox = new ComboBox
                {
                    Location = new Point(15, 40),
                    Size = new Size(300, 24),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Standard
                };

                // Add mirrors to combo
                foreach (var item in remotesList.Items)
                {
                    comboBox.Items.Add(item.ToString());
                }

                // Select current mirror
                if (remotesList.SelectedIndex >= 0)
                {
                    comboBox.SelectedIndex = remotesList.SelectedIndex;
                }
                else if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }

                var okButton = CreateStyledButton("OK", DialogResult.OK, new Point(155, 75));
                var cancelButton = CreateStyledButton("Cancel", DialogResult.Cancel, new Point(240, 75), false);

                dialog.Controls.AddRange(new Control[] { label, comboBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) == DialogResult.OK && comboBox.SelectedIndex != -1)
                {
                    return comboBox.SelectedItem.ToString();
                }
            }

            return null;
        }

        private Button CreateStyledButton(string text, DialogResult dialogResult, Point location, bool isPrimary = true)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = dialogResult,
                Location = location,
                Size = new Size(75, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(42, 45, 58),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            // Track hover state
            bool isHovered = false;

            button.MouseEnter += (s, e) => { isHovered = true; button.Invalidate(); };
            button.MouseLeave += (s, e) => { isHovered = false; button.Invalidate(); };

            button.Paint += (s, e) =>
            {
                var btn = s as Button;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                int radius = 4;
                Rectangle drawRect = new Rectangle(1, 1, btn.Width - 2, btn.Height - 2);

                // Clear with parent background
                using (SolidBrush clearBrush = new SolidBrush(btn.Parent?.BackColor ?? Color.FromArgb(20, 24, 29)))
                {
                    g.FillRectangle(clearBrush, 0, 0, btn.Width, btn.Height);
                }

                using (GraphicsPath path = CreateRoundedRectPath(drawRect, radius))
                {
                    // Hover: accent color, Normal: dark button color
                    Color bgColor = isHovered
                        ? Color.FromArgb(93, 203, 173)
                        : btn.BackColor;

                    Color textColor = isHovered
                        ? Color.FromArgb(20, 20, 20)
                        : btn.ForeColor;

                    using (SolidBrush brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle border on normal state
                    if (!isHovered)
                    {
                        using (Pen borderPen = new Pen(Color.FromArgb(70, 75, 90), 1))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }

                    TextRenderer.DrawText(g, btn.Text, btn.Font,
                        new Rectangle(0, 0, btn.Width, btn.Height), textColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // Set rounded region
                using (GraphicsPath regionPath = CreateRoundedRectPath(new Rectangle(0, 0, btn.Width, btn.Height), radius))
                {
                    btn.Region = new Region(regionPath);
                }
            };

            return button;
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            diameter = Math.Min(diameter, Math.Min(rect.Width, rect.Height));
            radius = diameter / 2;

            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void UpdateFilterButtonStates()
        {
            Color inactiveStroke = Color.FromArgb(74, 74, 74);
            Color activeBg = Color.FromArgb(40, 45, 55);
            Color inactiveBg = Color.FromArgb(32, 35, 45);

            // btnInstalled state
            if (upToDate_Clicked)
            {
                btnInstalled.StrokeColor = ColorInstalled;
                btnInstalled.Inactive1 = activeBg;
                btnInstalled.Inactive2 = activeBg;
            }
            else
            {
                btnInstalled.StrokeColor = inactiveStroke;
                btnInstalled.Inactive1 = inactiveBg;
                btnInstalled.Inactive2 = inactiveBg;
            }

            // btnUpdateAvailable state
            if (updateAvailableClicked)
            {
                btnUpdateAvailable.StrokeColor = ColorUpdateAvailable;
                btnUpdateAvailable.Inactive1 = activeBg;
                btnUpdateAvailable.Inactive2 = activeBg;
            }
            else
            {
                btnUpdateAvailable.StrokeColor = inactiveStroke;
                btnUpdateAvailable.Inactive1 = inactiveBg;
                btnUpdateAvailable.Inactive2 = inactiveBg;
            }

            // btnNewerThanList state
            if (NeedsDonation_Clicked)
            {
                btnNewerThanList.StrokeColor = ColorDonateGame;
                btnNewerThanList.Inactive1 = activeBg;
                btnNewerThanList.Inactive2 = activeBg;
            }
            else
            {
                btnNewerThanList.StrokeColor = inactiveStroke;
                btnNewerThanList.Inactive1 = inactiveBg;
                btnNewerThanList.Inactive2 = inactiveBg;
            }

            // btnLocalLibrary state
            if (downloadedFilter_Clicked)
            {
                btnDownloaded.StrokeColor = ColorDownloaded;
                btnDownloaded.Inactive1 = activeBg;
                btnDownloaded.Inactive2 = activeBg;
            }
            else
            {
                btnDownloaded.StrokeColor = inactiveStroke;
                btnDownloaded.Inactive1 = inactiveBg;
                btnDownloaded.Inactive2 = inactiveBg;
            }

            // Force repaint
            btnInstalled.Invalidate();
            btnUpdateAvailable.Invalidate();
            btnNewerThanList.Invalidate();
            btnDownloaded.Invalidate();
            openDownloadsFolderIcon.Invalidate();

            // Refresh UI
            UpdateStatusLabels();
        }

        public void RefreshDownloadedState()
        {
            if (_allItems == null || _allItems.Count == 0)
                return;

            string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
            int downloadedCount = 0;

            if (Directory.Exists(dlDir))
            {
                var localDirNames = new HashSet<string>(
                    Directory.GetDirectories(dlDir).Select(d => Path.GetFileName(d)),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var item in _allItems)
                {
                    if (item.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex) continue;
                    string releaseName = item.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                    if (!localDirNames.Contains(releaseName)) continue;
                    string folderPath = Path.Combine(dlDir, releaseName);
                    try
                    {
                        if (Directory.GetFiles(folderPath, "*.apk").Length > 0)
                            downloadedCount++;
                    }
                    catch { }
                }
            }

            btnDownloaded.Text = $"{downloadedCount} DOWNLOADED";

            // If the downloaded filter is currently active, re-apply it
            if (downloadedFilter_Clicked)
            {
                downloadedFilter_Clicked = false;
                btnDownloaded_Click(this, EventArgs.Empty);
            }
        }

        public async void RescanLocalLibrary()
        {
            string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;

            changeTitle("Scanning local library...");
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "Scanning";

            await Task.Run(() =>
            {
                SideloaderRCLONE.games.Clear();
                SideloaderRCLONE.ScanLocalGames(dlDir);
            });

            _allItemsInitialized = false;
            _galleryDataSource = null;
            initListView(false);

            progressBar.IsIndeterminate = false;
            changeTitle("Local library mode active", true);
            UpdateStatusLabels();
        }

        private void UnfocusSearchTextBox(object sender, EventArgs e)
        {
            // Only unfocus if the search text box currently has focus
            if (searchTextBox.Focused)
            {
                // Move focus to the appropriate view
                if (isGalleryView && gamesGalleryView.Visible)
                {
                    gamesGalleryView.Focus();
                }
                else
                {
                    gamesListView.Focus();
                }
            }
        }

        private void UpdateNotesScrollBar()
        {
            // Check if content height exceeds visible height
            int contentHeight = notesRichTextBox.GetPositionFromCharIndex(notesRichTextBox.TextLength).Y
                                + notesRichTextBox.Font.Height;

            if (contentHeight > notesRichTextBox.ClientSize.Height)
            {
                notesRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            }
            else
            {
                notesRichTextBox.ScrollBars = RichTextBoxScrollBars.None;
            }
        }

        public class CenteredMenuRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rect = new Rectangle(Point.Empty, e.Item.Size);
                Color bgColor = e.Item.Selected ? Color.FromArgb(55, 58, 65) : Color.FromArgb(40, 42, 48);
                using (var brush = new SolidBrush(bgColor))
                    e.Graphics.FillRectangle(brush, rect);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                var textRect = new Rectangle(8, 0, e.Item.Width - 16, e.Item.Height);
                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, textRect, e.TextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using (var brush = new SolidBrush(Color.FromArgb(40, 42, 48)))
                    e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using (var pen = new Pen(Color.FromArgb(60, 63, 70)))
                    e.Graphics.DrawRectangle(pen, 0, 0, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1);
            }
        }

        public void SetTrailerVisibility(bool visible)
        {
            webView21.Enabled = visible;
            webView21.Visible = visible;

            if (!visible) ShowVideoPlaceholder();
        }

        private void InitializeModernPanels()
        {
            Color panelColor = Color.FromArgb(24, 26, 30);

            // Initialize modern queue panel
            _queuePanel = new ModernQueuePanel
            {
                Size = new Size(250, 150),  // Placeholder; resized by LayoutBottomPanels
                BackColor = panelColor
            };
            _queuePanel.ItemRemoved += QueuePanel_ItemRemoved;
            _queuePanel.ItemReordered += QueuePanel_ItemReordered;
            _queuePanel.ItemPauseToggled += QueuePanel_ItemPauseToggled;

            // Sync with binding list
            gamesQueueList.ListChanged += (s, e) => SyncQueuePanel();

            // Notes panel
            notesPanel = CreateRoundedPanel(notesRichTextBox, panelColor, 8, true);

            // Queue panel
            queuePanel = CreateQueuePanel(panelColor, 8);

            gamesQueueLabel.BringToFront();
            lblNotes.BringToFront();

            // Trigger initial layout
            LayoutBottomPanels();
        }

        private Panel CreateQueuePanel(Color panelColor, int radius)
        {
            var panel = new Panel
            {
                Location = gamesQueListBox.Location,
                Size = new Size(
                    gamesQueListBox.Width + ChildHorizontalPadding + ChildRightMargin,
                    gamesQueListBox.Height + ReservedLabelHeight
                ),
                BackColor = Color.Transparent,
                Padding = new Padding(ChildHorizontalPadding, ChildTopMargin, ChildRightMargin, ChildTopMargin)
            };

            // Double buffering
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, panel, new object[] { true });

            panel.Paint += (s, e) =>
            {
                var p = (Panel)s;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);

                using (var path = CreateRoundedRectPath(rect, radius))
                using (var brush = new SolidBrush(panelColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (var regionPath = CreateRoundedRectPath(new Rectangle(0, 0, p.Width, p.Height), radius))
                {
                    p.Region = new Region(regionPath);
                }
            };

            var parent = gamesQueListBox.Parent;
            parent.Controls.Remove(gamesQueListBox);
            gamesQueListBox.Dispose();

            _queuePanel.Location = new Point(ChildHorizontalPadding, ChildTopMargin);
            _queuePanel.Anchor = AnchorStyles.None;
            panel.Controls.Add(_queuePanel);
            parent.Controls.Add(panel);
            panel.BringToFront();

            return panel;
        }

        private void SyncQueuePanel()
        {
            if (_queuePanel == null) return;
            _queuePanel.SetItems(gamesQueueList);
            _queuePanel.IsDownloading = gamesAreDownloading && gamesQueueList.Count > 0;
            _queuePanel.IsPaused = _downloadPaused;

            UpdateQueueLabel();

            // Persist queue to settings
            SaveQueueToSettings();
        }

        private void UpdateQueueLabel()
        {
            if (gamesQueueLabel.InvokeRequired)
            {
                gamesQueueLabel.Invoke(new Action(UpdateQueueLabel));
                return;
            }

            if (gamesQueueList.Count == 0)
            {
                gamesQueueLabel.Text = "Download Queue";
                _totalQueueSizeMB = 0;
                _effectiveQueueSizeMB = 0;
                _queueEffectiveSizes.Clear();
                return;
            }

            // Recalculate total size
            _totalQueueSizeMB = 0;
            foreach (string releaseName in gamesQueueList)
            {
                foreach (string[] game in SideloaderRCLONE.games)
                {
                    if (game.Length > SideloaderRCLONE.ReleaseNameIndex &&
                        game[SideloaderRCLONE.ReleaseNameIndex].Equals(releaseName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (game.Length > 5 && StringUtilities.TryParseDouble(game[5], out double sizeMB))
                        {
                            _totalQueueSizeMB += sizeMB;
                        }
                        break;
                    }
                }
            }

            string sizeText = _totalQueueSizeMB >= 1024
                ? $"{(_totalQueueSizeMB / 1024):F2} GB"
                : $"{_totalQueueSizeMB:F0} MB";

            gamesQueueLabel.Text = $"Download Queue ({gamesQueueList.Count}) · {sizeText}";
        }

        private void SaveQueueToSettings()
        {
            settings.QueuedGames = gamesQueueList.ToArray();
            settings.QueuedProgressPercent = _pausedProgressPercent;
            settings.QueuedTransfersComplete = _pausedTransfersComplete;
            settings.QueuedFileCount = _pausedFileCount;
            settings.Save();
        }

        private void LoadQueueFromSettings()
        {
            if (settings.QueuedGames == null || settings.QueuedGames.Length == 0)
                return;

            // Restore progress state BEFORE adding items to queue.
            // Adding items triggers ListChanged → SyncQueuePanel → SaveQueueToSettings,
            // which would overwrite settings with zeros if fields aren't restored first.
            _pausedProgressPercent = settings.QueuedProgressPercent;
            _pausedTransfersComplete = settings.QueuedTransfersComplete;
            _pausedFileCount = settings.QueuedFileCount;

            foreach (string game in settings.QueuedGames)
            {
                if (!string.IsNullOrWhiteSpace(game) && !gamesQueueList.Contains(game))
                {
                    gamesQueueList.Add(game);
                }
            }
        }

        private async Task ResumeQueuedDownloadsAsync()
        {
            if (gamesQueueList.Count == 0)
                return;

            // Ask user if they want to resume
            DialogResult result = FlexibleMessageBox.Show(
                Program.form,
                $"You have {gamesQueueList.Count} game(s) in your download queue from a previous session.\n\nDo you want to resume downloading?",
                "Resume Downloads?",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // Trigger the download process
                downloadInstallGameButton_Click(null, EventArgs.Empty);
            }
            else
            {
                // Clear the queue if user doesn't want to resume
                gamesQueueList.Clear();
                _pausedProgressPercent = 0;
                _pausedTransfersComplete = 0;
                _pausedFileCount = 0;
                SaveQueueToSettings();
            }
        }

        private void QueuePanel_ItemRemoved(object sender, int index)
        {
            if (index == 0 && gamesQueueList.Count >= 1)
            {
                // If paused, clear pause state and wake up the download loop
                if (_downloadPaused)
                {
                    _downloadPaused = false;
                    _queuePanel.IsPaused = false;
                    _queuePanel.Invalidate();
                }
                removedownloading = true;
                RCLONE.killRclone();
                // Wake up the pause wait if it's active
                if (_pauseTcs != null)
                {
                    _pauseTcs.TrySetResult(true);
                    _pauseTcs = null;
                }
            }
            else if (index > 0 && index < gamesQueueList.Count)
            {
                string removedGame = gamesQueueList[index];

                // Subtract effective size from running total
                if (_queueEffectiveSizes.TryGetValue(removedGame, out double effectiveSize))
                {
                    _effectiveQueueSizeMB -= effectiveSize;
                    _queueEffectiveSizes.Remove(removedGame);
                }

                gamesQueueList.RemoveAt(index);
                SaveQueueToSettings();
            }
        }

        private void QueuePanel_ItemReordered(object sender, ReorderEventArgs e)
        {
            if (e.FromIndex <= 0 || e.FromIndex >= gamesQueueList.Count) return;

            var item = gamesQueueList[e.FromIndex];
            gamesQueueList.RemoveAt(e.FromIndex);

            int insertAt = Math.Max(1, Math.Min(e.ToIndex, gamesQueueList.Count));
            gamesQueueList.Insert(insertAt, item);

            SaveQueueToSettings();
        }

        private void QueuePanel_ItemPauseToggled(object sender, bool paused)
        {
            _downloadPaused = paused;
            if (paused)
            {
                RCLONE.killRclone();
            }
            else
            {
                if (_pauseTcs != null)
                {
                    _pauseTcs.TrySetResult(true);
                    _pauseTcs = null;
                }
            }
            _queuePanel.IsPaused = paused;
            _queuePanel.Invalidate();
        }

        private void notesRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to open link: {ex.Message}", LogLevel.WARNING);
            }
        }

        private void ApplyWebViewRoundedCorners()
        {
            if (webView21 == null) return;

            int radius = 8;
            using (var path = CreateRoundedRectPath(new Rectangle(0, 0, webView21.Width, webView21.Height), radius))
            {
                webView21.Region = new Region(path);
            }

            // Re-apply on resize
            webView21.SizeChanged -= WebView21_SizeChanged;
            webView21.SizeChanged += WebView21_SizeChanged;
        }

        private void WebView21_SizeChanged(object sender, EventArgs e)
        {
            if (webView21 == null || webView21.Width <= 0 || webView21.Height <= 0) return;

            int radius = 8;
            using (var path = CreateRoundedRectPath(new Rectangle(0, 0, webView21.Width, webView21.Height), radius))
            {
                webView21.Region = new Region(path);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            LayoutBottomPanels();
        }

        private void LayoutChildInPanel(Panel panel, Control child, bool isNotesPanel)
        {
            int leftMargin = isNotesPanel ? NotesLeftMargin : ChildHorizontalPadding;

            child.Location = new Point(leftMargin, ChildTopMargin);

            // Width: panel width minus left + right margins
            int widthReduction = leftMargin + ChildRightMargin;
            int childWidth = Math.Max(0, panel.Width - widthReduction);

            // Height: panel height minus vertical padding and reserved label area
            int childHeight = Math.Max(0,
                panel.Height - (ChildHorizontalPadding * 2) - ReservedLabelHeight);

            child.Size = new Size(childWidth, childHeight);
        }

        private Panel CreateRoundedPanel(Control childControl, Color panelColor, int radius, bool isNotesPanel)
        {
            // Create wrapper panel
            var panel = new Panel
            {
                Location = childControl.Location,
                Size = new Size(
                    childControl.Width + ChildHorizontalPadding + ChildRightMargin,
                    childControl.Height + ReservedLabelHeight
                ),
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent,
                Padding = new Padding(ChildHorizontalPadding, ChildTopMargin, ChildRightMargin, ChildTopMargin)
            };

            // Enable double buffering
            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, panel, new object[] { true });

            // Add paint handler for rounded corners
            panel.Paint += (sender, e) =>
            {
                var p = (Panel)sender;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);

                using (var path = CreateRoundedRectPath(rect, radius))
                using (var brush = new SolidBrush(panelColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Apply rounded region
                using (var regionPath = CreateRoundedRectPath(
                           new Rectangle(0, 0, p.Width, p.Height),
                           radius))
                {
                    p.Region = new Region(regionPath);
                }
            };

            // Move child control into panel
            var parent = childControl.Parent;
            parent.Controls.Add(panel);
            parent.Controls.Remove(childControl);

            // Layout child inside panel using shared helper
            childControl.Anchor = AnchorStyles.None;
            childControl.BackColor = panelColor;
            LayoutChildInPanel(panel, childControl, isNotesPanel);

            panel.Controls.Add(childControl);
            panel.BringToFront();

            return panel;
        }

        private void LayoutBottomPanels()
        {
            // Skip if panels aren't initialized yet
            if (notesPanel == null || queuePanel == null) return;
            if (notesRichTextBox == null) return;

            // Panels start after webView21 (webView21 ends at 259 + 384 = 643, add spacing)
            int panelsStartX = 654;
            int availableWidth = this.ClientSize.Width - panelsStartX - RightMargin;

            // Queue panel gets fixed width, notes panel fills remaining space
            int desiredQueueWidth = 290;
            int queueWidth = Math.Max(200, Math.Min(desiredQueueWidth, availableWidth / 2));
            int notesWidth = availableWidth - queueWidth - PanelSpacing;
            notesWidth = Math.Max(200, notesWidth);

            int panelY = this.ClientSize.Height - BottomPanelHeight - BottomMargin;

            // Queue panel
            queuePanel.Location = new Point(panelsStartX, panelY);
            queuePanel.Size = new Size(queueWidth, BottomPanelHeight);

            // Layout queue panel child (_queuePanel)
            if (_queuePanel != null)
            {
                _queuePanel.Location = new Point(ChildHorizontalPadding, ChildTopMargin);
                _queuePanel.Size = new Size(
                    queuePanel.Width - ChildHorizontalPadding - ChildRightMargin,
                    queuePanel.Height - ChildTopMargin * 2 - ReservedLabelHeight
                );
            }

            // Notes panel
            int notesX = panelsStartX + queueWidth + PanelSpacing;
            notesPanel.Location = new Point(notesX, panelY);
            notesPanel.Size = new Size(this.ClientSize.Width - notesX - RightMargin, BottomPanelHeight);

            // Layout notes child
            LayoutChildInPanel(notesPanel, notesRichTextBox, isNotesPanel: true);

            // Position labels at bottom of their panels
            gamesQueueLabel.Location = new Point(
                queuePanel.Location.X + ChildHorizontalPadding + 3,
                queuePanel.Location.Y + queuePanel.Height - (LabelHeight + LabelBottomOffset)
            );

            lblNotes.Location = new Point(
                notesPanel.Location.X + ChildHorizontalPadding,
                notesPanel.Location.Y + notesPanel.Height - (LabelHeight + LabelBottomOffset)
            );

            // Ensure labels are visible
            gamesQueueLabel.BringToFront();
            lblNotes.BringToFront();

            // Force repaint of panels to update rounded corners
            queuePanel.Invalidate();
            notesPanel.Invalidate();
        }

        private async Task UninstallGameAsync(ListViewItem item)
        {
            string packageName = item.SubItems.Count > 2 ? item.SubItems[2].Text : "";
            string gameName = item.Text;

            if (string.IsNullOrEmpty(packageName))
                return;

            // Confirm uninstall
            DialogResult dialogresult = FlexibleMessageBox.Show(
                $"Are you sure you want to uninstall {gameName}?",
                "Proceed with uninstall?", MessageBoxButtons.YesNo);

            if (dialogresult == DialogResult.No)
                return;

            // Ask about backup
            backupFolder = settings.GetEffectiveBackupDir();

            DialogResult dialogresult2 = FlexibleMessageBox.Show(
                $"Do you want to attempt to automatically backup any saves to {backupFolder}\\{DateTime.Today.ToString("yyyy.MM.dd")}\\",
                "Attempt Game Backup?", MessageBoxButtons.YesNo);

            if (dialogresult2 == DialogResult.Yes)
            {
                Sideloader.BackupGame(packageName);
            }

            // Perform uninstall
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.IsIndeterminate = true;
            progressBar.OperationType = "";

            await Task.Run(() =>
            {
                output += Sideloader.UninstallGame(packageName);
            });

            ShowPrcOutput(output);
            showAvailableSpace();
            progressBar.IsIndeterminate = false;

            // Remove from combo box
            for (int i = 0; i < m_combo.Items.Count; i++)
            {
                string comboItem = m_combo.Items[i].ToString();
                if (comboItem.Equals(gameName, StringComparison.OrdinalIgnoreCase) ||
                    comboItem.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    m_combo.Items.RemoveAt(i);
                    break;
                }
            }

            await RefreshGameListAsync();
        }

        private async Task RefreshGameListAsync()
        {
            // Save current filter state before refreshing
            bool wasUpdateAvailableClicked = updateAvailableClicked;
            bool wasUpToDateClicked = upToDate_Clicked;
            bool wasNeedsDonationClicked = NeedsDonation_Clicked;
            bool wasDownloadedClicked = downloadedFilter_Clicked;
            bool wasFavoritesView = favoriteSwitcher.Text == "ALL";

            // Save the currently selected package name to restore after refresh
            string selectedPackageName = null;
            if (gamesListView.SelectedItems.Count > 0)
            {
                var selectedItem = gamesListView.SelectedItems[0];
                if (selectedItem.SubItems.Count > 2)
                {
                    selectedPackageName = selectedItem.SubItems[2].Text;
                }
            }
            else if (isGalleryView && _fastGallery != null)
            {
                var galleryItem = _fastGallery.GetItemAtIndex(_fastGallery._selectedIndex);
                if (galleryItem != null && galleryItem.SubItems.Count > 2)
                {
                    selectedPackageName = galleryItem.SubItems[2].Text;
                }
            }

            // Temporarily clear filter states
            updateAvailableClicked = false;
            upToDate_Clicked = false;
            NeedsDonation_Clicked = false;

            // Refresh the list to update installed status
            _allItemsInitialized = false;
            _galleryDataSource = null;
            listAppsBtn();

            bool wasGalleryView = isGalleryView;
            isGalleryView = false;

            initListView(false);

            // Wait for initListView to finish rebuilding _allItems
            while (!_allItemsInitialized || !loaded)
            {
                await Task.Delay(50);
            }

            isGalleryView = wasGalleryView;

            // Reapply the active filter
            if (wasFavoritesView)
            {
                // Reapply favorites filter
                favoriteSwitcher.Text = "FAVORITES"; // Reset text first
                favoriteSwitcher_Click(favoriteSwitcher, EventArgs.Empty); // This will toggle to show favorites and set text to "ALL"
            }

            if (wasUpToDateClicked)
            {
                upToDate_Clicked = true;
                FilterListByColors(new[] { ColorInstalled, ColorUpdateAvailable, ColorDonateGame });
            }
            else if (wasUpdateAvailableClicked)
            {
                updateAvailableClicked = true;
                FilterListByColor(ColorUpdateAvailable);
            }
            else if (wasNeedsDonationClicked)
            {
                NeedsDonation_Clicked = true;
                FilterListByColor(ColorDonateGame);
            }
            else if (wasDownloadedClicked)
            {
                downloadedFilter_Clicked = false;
                if (isGalleryView)
                {
                    gamesListView.Visible = false;
                    gamesGalleryView.Visible = true;
                }
                btnDownloaded_Click(null, EventArgs.Empty);
            }
            else if (isGalleryView)
            {
                gamesListView.Visible = false;
                gamesGalleryView.Visible = true;
                PopulateGalleryView();
            }

            // Restore selection and scroll to the previously selected item
            if (!string.IsNullOrEmpty(selectedPackageName))
            {
                RestoreSelectionByPackageName(selectedPackageName);
            }
        }

        private void RestoreSelectionByPackageName(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            // Restore in ListView
            foreach (ListViewItem item in gamesListView.Items)
            {
                if (item.SubItems.Count > 2 &&
                    item.SubItems[2].Text.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    break;
                }
            }

            // Restore in Gallery view
            if (isGalleryView && _fastGallery != null)
            {
                _fastGallery.ScrollToPackage(packageName);
            }
        }
        private async void timer_DeviceCheck(object sender, EventArgs e)
        {
            // Skip if a device is connected, we're in the middle of loading or other operations
            if (DeviceConnected || isLoading || isinstalling || isuploading) return;

            // Run a quick device check in background
            try
            {
                string output = await Task.Run(() => ADB.RunAdbCommandToString("devices", suppressLogging: true).Output);

                string[] lines = output.Split('\n');
                bool hasDeviceNow = false;

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (line.Length > 0 && !string.IsNullOrWhiteSpace(line) && !line.Contains("unauthorized"))
                    {
                        hasDeviceNow = true;
                        break;
                    }
                }

                // Device state changed
                if (hasDeviceNow)
                {
                    // Device connected - do a full refresh
                    await CheckForDevice();
                    changeTitlebarToDevice();
                    showAvailableSpace();
                    listAppsBtn();
                    // Use RefreshGameListAsync to preserve filter state
                    await RefreshGameListAsync();
                    UpdateStatusLabels();
                }
            }
            catch
            {
                // Silently catch errors
            }
        }

        private void UpdateStatusLabels()
        {
            // Device ID
            if (DeviceConnected && Devices.Count > 0 && !string.IsNullOrEmpty(Devices[0]))
            {
                string deviceId = Devices[0].Replace("device", "").Trim();
                // Truncate if too long
                if (deviceId.Length > 20)
                    deviceId = deviceId.Substring(0, 17) + "...";
                deviceIdLabel.Text = $"Device: {deviceId}";
            }
            else
            {
                deviceIdLabel.Text = "Device: Not connected";
            }

            // Active Mirror
            string mirrorName = "None";
            if (UsingPublicConfig)
                mirrorName = "Public";
            else if (remotesList.SelectedItem != null)
                mirrorName = remotesList.SelectedItem.ToString();
            activeMirrorLabel.Text = $"Mirror: {mirrorName}";

            activeMirrorLabel.Cursor = Cursors.Hand;
            activeMirrorLabel.Invalidate();

            deviceIdLabel.Cursor = Cursors.Hand;
            deviceIdLabel.Invalidate();

            UpdateSideloadingUI();
        }

        public void UpdateSideloadingUI(bool isNoDeviceDialogShown = false)
        {
            // Update the sideload button text
            if (settings.NodeviceMode)
            {
                btnNoDevice.Text = "ENABLE SIDELOADING";
            }
            else
            {
                btnNoDevice.Text = "DISABLE SIDELOADING";
            }

            // Sideloading Status
            if (settings.NodeviceMode)
            {
                sideloadingStatusLabel.Text = "Sideloading: Disabled";
                sideloadingStatusLabel.ForeColor = Color.FromArgb(255, 100, 100); // Red-ish for disabled
                downloadInstallGameButton.Text = downloadedFilter_Clicked || isOffline ? "INSTALL" : "DOWNLOAD";
            }
            else if (isNoDeviceDialogShown || (!DeviceConnected && !isLoading))
            {
                sideloadingStatusLabel.Text = "Sideloading: No Device Connected";
                sideloadingStatusLabel.ForeColor = Color.FromArgb(240, 150, 50); // Orange for no device
                downloadInstallGameButton.Text = downloadedFilter_Clicked || isOffline ? "INSTALL" : "DOWNLOAD";
            }
            else
            {
                sideloadingStatusLabel.Text = "Sideloading: Enabled";
                sideloadingStatusLabel.ForeColor = Color.FromArgb(93, 203, 173); // Accent green for enabled
                downloadInstallGameButton.Text = downloadedFilter_Clicked || isOffline ? "INSTALL" : "DOWNLOAD AND INSTALL";
            }
        }

        public void UpdateProgressStatus(string operation,
            int current = 0,
            int total = 0,
            int percent = 0,
            TimeSpan? eta = null,
            double? speedMBps = null)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => UpdateProgressStatus(operation, current, total, percent, eta, speedMBps));
                return;
            }

            // Sync the progress bar's operation type to the current operation
            progressBar.OperationType = operation;

            var sb = new StringBuilder();

            // Operation name
            sb.Append(operation);

            // Percentage
            if (percent > 0)
            {
                sb.Append($" ({percent}%)");
            }

            // Speed
            if (speedMBps.HasValue && speedMBps.Value > 0)
            {
                sb.Append($" @ {speedMBps.Value:F1} MB/s");
            }

            // File count if applicable
            if (total > 1)
            {
                sb.Append($" ({current}/{total})");
            }

            // ETA
            if (eta.HasValue && eta.Value.TotalSeconds > 0)
            {
                sb.Append($" • ETA: {eta.Value:hh\\:mm\\:ss}");
            }

            speedLabel.Text = sb.ToString();
        }

        public void ClearProgressStatus()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(ClearProgressStatus);
                return;
            }

            speedLabel.Text = "";
        }

        private void SaveWindowState()
        {
            try
            {
                // Save maximized state separately
                settings.WindowMaximized = this.WindowState == FormWindowState.Maximized;

                // Save normal bounds (not maximized bounds)
                if (this.WindowState == FormWindowState.Normal)
                {
                    settings.WindowX = this.Location.X;
                    settings.WindowY = this.Location.Y;
                    settings.WindowWidth = this.Size.Width;
                    settings.WindowHeight = this.Size.Height;
                }
                else if (this.WindowState == FormWindowState.Maximized)
                {
                    settings.WindowX = this.RestoreBounds.X;
                    settings.WindowY = this.RestoreBounds.Y;
                    settings.WindowWidth = this.RestoreBounds.Width;
                    settings.WindowHeight = this.RestoreBounds.Height;
                }

                // Capture current sort state from active view before saving
                if (isGalleryView && _fastGallery != null)
                {
                    _sharedSortField = _fastGallery.CurrentSortField;
                    _sharedSortDirection = _fastGallery.CurrentSortDirection;
                }
                else if (!isGalleryView && lvwColumnSorter != null)
                {
                    _sharedSortField = ColumnIndexToSortField(lvwColumnSorter.SortColumn);
                    SortDirection listDirection = lvwColumnSorter.Order == SortOrder.Ascending
                        ? SortDirection.Ascending
                        : SortDirection.Descending;

                    // Flip popularity when capturing from list view
                    if (_sharedSortField == SortField.Popularity)
                    {
                        _sharedSortDirection = listDirection == SortDirection.Ascending
                            ? SortDirection.Descending
                            : SortDirection.Ascending;
                    }
                    else
                    {
                        _sharedSortDirection = listDirection;
                    }
                }

                // Save sort state
                settings.SortColumn = SortFieldToColumnIndex(_sharedSortField);
                settings.SortAscending = _sharedSortDirection == SortDirection.Ascending;

                settings.Save();
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to save window state: {ex.Message}", LogLevel.WARNING);
            }
        }

        private void LoadWindowState()
        {
            try
            {
                // Load window position and size
                if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
                {
                    // Validate that the saved position is on a visible screen
                    Rectangle savedBounds = new Rectangle(
                        settings.WindowX,
                        settings.WindowY,
                        settings.WindowWidth,
                        settings.WindowHeight);

                    bool isOnScreen = false;
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        if (screen.WorkingArea.IntersectsWith(savedBounds))
                        {
                            isOnScreen = true;
                            break;
                        }
                    }

                    if (isOnScreen)
                    {
                        this.StartPosition = FormStartPosition.Manual;
                        this.Location = new Point(settings.WindowX, settings.WindowY);
                        this.Size = new Size(settings.WindowWidth, settings.WindowHeight);

                        if (settings.WindowMaximized)
                        {
                            this.WindowState = FormWindowState.Maximized;
                        }
                    }
                    else
                    {
                        // Saved position is off-screen, use defaults
                        this.StartPosition = FormStartPosition.CenterScreen;
                    }
                }

                // Load sort state
                _sharedSortField = ColumnIndexToSortField(settings.SortColumn);
                _sharedSortDirection = settings.SortAscending ? SortDirection.Ascending : SortDirection.Descending;

                // Apply to list view sorter (with popularity flip)
                if (settings.SortColumn >= 0 && settings.SortColumn < gamesListView.Columns.Count)
                {
                    lvwColumnSorter.SortColumn = settings.SortColumn;

                    // For popularity, flip direction for list view
                    SortDirection effectiveDirection = _sharedSortDirection;
                    if (_sharedSortField == SortField.Popularity)
                    {
                        effectiveDirection = _sharedSortDirection == SortDirection.Ascending
                            ? SortDirection.Descending
                            : SortDirection.Ascending;
                    }

                    lvwColumnSorter.Order = effectiveDirection == SortDirection.Ascending
                        ? SortOrder.Ascending
                        : SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to load window state: {ex.Message}", LogLevel.WARNING);
                this.StartPosition = FormStartPosition.CenterScreen;
                lvwColumnSorter.SortColumn = 0;
                lvwColumnSorter.Order = SortOrder.Ascending;
                _sharedSortField = SortField.Name;
                _sharedSortDirection = SortDirection.Ascending;
            }
        }
    }

    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                _ = control.Invoke(new MethodInvoker(action), null);
            }
            else
            {
                action.Invoke();
            }
        }

        public static void SetStyle(this Control control, ControlStyles styles, bool value)
        {
            typeof(Control).GetMethod("SetStyle",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(control, new object[] { styles, value });
        }
    }
}
