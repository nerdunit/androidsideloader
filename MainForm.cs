using AndroidSideloader.Models;
using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using SergeUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
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

        private readonly ListViewColumnSorter lvwColumnSorter;

#if DEBUG
        public static bool debugMode = true;
        public bool DeviceConnected = false;
        public bool keyheld;
        public bool keyheld2;
        public static string CurrAPK;
        public static string CurrPCKG;
        List<UploadGame> gamesToUpload = new List<UploadGame>();


        public static string currremotesimple = "";
#else
        public bool keyheld;
        public static string CurrAPK;
        public static string CurrPCKG;
        private readonly List<UploadGame> gamesToUpload = new List<UploadGame>();
        public static bool debugMode = false;
        public bool DeviceConnected = false;


        public static string currremotesimple = "";

#endif

        private bool isLoading = true;
        public static bool isOffline = false;
        public static bool hasPublicConfig = false;
        public static bool enviromentCreated = false;
        public static PublicConfig PublicConfigFile;
        public static string PublicMirrorExtraArgs = " --tpslimit 1.0 --tpslimit-burst 3";
        private bool manualIP;
        public MainForm()
        {
            // check for offline mode
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg == "--offline")
                {
                    isOffline = true;
                }
            }
            if (isOffline)
            {
                _ = FlexibleMessageBox.Show(Program.form, "Offline mode activated. You can't download games in this mode, only do local stuff.");
            }

            InitializeComponent();
            gamesQueListBox.DataSource = gamesQueueList;
            //Time between asking for new apps if user clicks No. 96,0,0 DEFAULT
            TimeSpan newDayReference = new TimeSpan(96, 0, 0);
            //Time between asking for updates after uploading. 72,0,0 DEFAULT
            TimeSpan newDayReference2 = new TimeSpan(72, 0, 0);
            TimeSpan comparison;
            TimeSpan comparison2;

            //These two variables set to show difference.
            DateTime A = Properties.Settings.Default.LastLaunch;
            DateTime B = DateTime.Now;
            DateTime C = Properties.Settings.Default.LastLaunch2;
            comparison = B - A;
            comparison2 = B - C;
            // If enough time has passed reset property containing packagenames
            if (comparison > newDayReference)
            {
                Properties.Settings.Default.ListUpped = false;
                Properties.Settings.Default.NonAppPackages = "";
                Properties.Settings.Default.AppPackages = "";
                Properties.Settings.Default.LastLaunch = DateTime.Now;
                Properties.Settings.Default.Save();
            }
            if (comparison2 > newDayReference2)
            {
                Properties.Settings.Default.LastLaunch2 = DateTime.Now;
                Properties.Settings.Default.SubmittedUpdates = "";
                Properties.Settings.Default.Save();
            }
            //Time for debuglog
            string launchtime = DateTime.Now.ToString("hh:mmtt(UTC)");
            _ = Logger.Log($"\n------\n------\nProgram Launched at: {launchtime}\n------\n------");
            if (string.IsNullOrEmpty(Properties.Settings.Default.CurrentLogPath))
            {
                Properties.Settings.Default.CurrentLogPath = $"{Environment.CurrentDirectory}\\debuglog.txt";
            }
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer
            {
                Interval = 840000 // 14 mins between wakeup commands
            };
            t.Tick += new EventHandler(timer_Tick);
            t.Start();
            System.Windows.Forms.Timer t2 = new System.Windows.Forms.Timer
            {
                Interval = 300 // 30ms
            };
            t2.Tick += new EventHandler(timer_Tick2);
            t2.Start();

            lvwColumnSorter = new ListViewColumnSorter();
            gamesListView.ListViewItemSorter = lvwColumnSorter;
            if (searchTextBox.Visible)
            {
                _ = searchTextBox.Focus();
            }
        }

        public static string DonorApps = "";
        private string oldTitle = "";
        public static bool updatesnotified = false;
        public static string BackupFolder;

        private async void Form1_Load(object sender, EventArgs e)
        {
            Splash splash = new Splash();
            splash.Show();

            if (!isOffline)
            {
                if (File.Exists($"{Environment.CurrentDirectory}\\vrp-public.json"))
                {
                    Thread worker = new Thread(() =>
                    {
                        SideloaderRCLONE.updatePublicConfig();
                    });
                    worker.Start();
                    while (worker.IsAlive)
                    {
                        Thread.Sleep(10);
                    }

                    try
                    {
                        string configFileData =
                            File.ReadAllText($"{Environment.CurrentDirectory}\\vrp-public.json");
                        PublicConfig config = JsonConvert.DeserializeObject<PublicConfig>(configFileData);

                        if (config != null
                            && !string.IsNullOrWhiteSpace(config.BaseUri)
                            && !string.IsNullOrWhiteSpace(config.Password))
                        {
                            PublicConfigFile = config;
                            hasPublicConfig = true;
                        }
                    }
                    catch
                    {
                        hasPublicConfig = false;
                    }

                    if (!hasPublicConfig)
                    {
                        _ = FlexibleMessageBox.Show(Program.form, "Failed to fetch public mirror config, and the current one is unreadable.\r\nPlease ensure you can access https://wiki.vrpirates.club/ in your browser.", "Config Update Failed", MessageBoxButtons.OK);
                    }

                    if (Directory.Exists(@"C:\RSL\EBWebView"))
                    {
                        Directory.Delete(@"C:\RSL\EBWebView", true);
                    }
                }
            }

            if (File.Exists("C:\\RSL\\platform-tools\\adb.exe"))
            {
                _ = ADB.RunAdbCommandToString("kill-server");
                _ = ADB.RunAdbCommandToString("start-server");
            }
            Properties.Settings.Default.MainDir = Environment.CurrentDirectory;
            Properties.Settings.Default.Save();
            Sideloader.downloadFiles();
            await Task.Delay(100);
            if (Directory.Exists(Sideloader.TempFolder))

            {
                Directory.Delete(Sideloader.TempFolder, true);
                _ = Directory.CreateDirectory(Sideloader.TempFolder);
            }

            //Delete the Debug file if it is more than 5MB
            if (File.Exists($"{Properties.Settings.Default.CurrentLogPath}"))
            {
                long length = new System.IO.FileInfo(Properties.Settings.Default.CurrentLogPath).Length;
                if (length > 5000000)
                {
                    File.Delete($"{Properties.Settings.Default.CurrentLogPath}");
                }
            }
            if (!isOffline)
            {
                RCLONE.Init();
            }
            try { Spoofer.spoofer.Init(); } catch { }

            if (Properties.Settings.Default.CallUpgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.CallUpgrade = false;
                Properties.Settings.Default.Save();
            }
            CenterToScreen();
            gamesListView.View = View.Details;
            gamesListView.FullRowSelect = true;
            gamesListView.GridLines = false;
            etaLabel.Text = "";
            speedLabel.Text = "";
            diskLabel.Text = "";
            verLabel.Text = Updater.LocalVersion;
            if (File.Exists("crashlog.txt"))
            {
                if (File.Exists(Properties.Settings.Default.CurrentCrashPath))
                {
                    File.Delete(Properties.Settings.Default.CurrentCrashPath);
                }

                DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, $"Sideloader crashed during your last use.\nPress OK if you'd like to send us your crash log.\n\n NOTE: THIS CAN TAKE UP TO 30 SECONDS.", "Crash Detected", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    if (File.Exists($"{Environment.CurrentDirectory}\\crashlog.txt"))
                    {
                        string UUID = SideloaderUtilities.UUID();
                        System.IO.File.Move("crashlog.txt", $"{Environment.CurrentDirectory}\\{UUID}.log");
                        Properties.Settings.Default.CurrentCrashPath = $"{Environment.CurrentDirectory}\\{UUID}.log";
                        Properties.Settings.Default.CurrentCrashName = UUID;
                        Properties.Settings.Default.Save();

                        Clipboard.SetText(UUID);
                        _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{Properties.Settings.Default.CurrentCrashPath}\" RSL-gameuploads:CrashLogs");
                        _ = FlexibleMessageBox.Show(Program.form, $"Your CrashLog has been copied to the server.\nPlease mention your CrashLogID ({Properties.Settings.Default.CurrentCrashName}) to the Mods.\nIt has been automatically copied to your clipboard.");
                        Clipboard.SetText(Properties.Settings.Default.CurrentCrashName);
                    }
                }
                else
                {
                    File.Delete($"{Environment.CurrentDirectory}\\crashlog.txt");
                }
            }

            if (hasPublicConfig)
            {
                lblMirror.Text = " Public Mirror";
                remotesList.Size = Size.Empty;
            }
            if (isOffline)
            {
                lblMirror.Text = " Offline Mode";
                remotesList.Size = Size.Empty;
            }

            splash.Close();
        }


        private async void Form1_Shown(object sender, EventArgs e)
        {
            EnterInstallBox.Checked = Properties.Settings.Default.EnterKeyInstall;
            new Thread(() =>
            {
                Thread.Sleep(10000);
                freeDisclaimer.Invoke(() => { freeDisclaimer.Dispose(); });
            }).Start();

            progressBar.Style = ProgressBarStyle.Marquee;
            Thread t1 = new Thread(() =>
            {
                if (!debugMode && Properties.Settings.Default.checkForUpdates)
                {
                    Updater.AppName = "AndroidSideloader";
                    Updater.Repostory = "nerdunit/androidsideloader";
                    Updater.Update();
                }
                progressBar.Invoke(() => { progressBar.Style = ProgressBarStyle.Marquee; });

                progressBar.Style = ProgressBarStyle.Marquee;
                if (!isOffline)
                {
                    ChangeTitle("Initializing Servers...");
                    initMirrors(true);
                    if (Properties.Settings.Default.autoUpdateConfig)
                    {
                        ChangeTitle("Checking for a new Configuration File...");
                        SideloaderRCLONE.updateDownloadConfig();
                    }

                    SideloaderRCLONE.updateUploadConfig();

                    if (!hasPublicConfig)
                    {
                        ChangeTitle("Grabbing the Games List...");
                        SideloaderRCLONE.initGames(currentRemote);
                    }
                }
                else
                {
                    ChangeTitle("Offline mode enabled, no Rclone");
                }

            });
            t1.SetApartmentState(ApartmentState.STA);
            t1.IsBackground = true;

            if (!isOffline)
            {
                t1.Start();
            }

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            Thread t5 = new Thread(() =>
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.IPAddress))
                {
                    string path = "C:\\RSL\\platform-tools\\adb.exe";
                    ProcessOutput wakeywakey = ADB.RunCommandToString("C:\\RSL\\platform-tools\\adb.exe shell input keyevent KEYCODE_WAKEUP", path);
                    if (wakeywakey.Output.Contains("more than one"))
                    {
                        Properties.Settings.Default.Wired = true;
                        Properties.Settings.Default.Save();
                    }
                    else if (wakeywakey.Output.Contains("found"))
                    {
                        ADB.WakeDevice();
                        Properties.Settings.Default.Wired = false;
                        Properties.Settings.Default.Save();
                    }
                }

                if (File.Exists(@"C:\RSL\platform-tools\StoredIP.txt") && !Properties.Settings.Default.Wired)
                {
                    string IPcmndfromtxt = File.ReadAllText(@"C:\RSL\platform-tools\StoredIP.txt");
                    Properties.Settings.Default.IPAddress = IPcmndfromtxt;
                    Properties.Settings.Default.Save();
                    ProcessOutput IPoutput = ADB.RunAdbCommandToString(IPcmndfromtxt);
                    if (IPoutput.Output.Contains("attempt failed") || IPoutput.Output.Contains("refused"))
                    {
                        _ = FlexibleMessageBox.Show(Program.form, "Attempt to connect to saved IP has failed. This is usually due to rebooting the device or not having a STATIC IP set in your router.\nYou must enable Wireless ADB again!");
                        Properties.Settings.Default.IPAddress = "";
                        Properties.Settings.Default.Save();
                        File.Delete("C:\\RSL\\platform-tools\\StoredIP.txt");
                    }
                    else
                    {
                        _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                        _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                    }
                }
                else if (!File.Exists(@"C:\RSL\platform-tools\StoredIP.txt"))
                {
                    Properties.Settings.Default.IPAddress = "";
                    Properties.Settings.Default.Save();
                }
            })
            {
                IsBackground = true
            };
            t5.Start();
            while (t5.IsAlive)
            {
                await Task.Delay(100);
            }

            if (hasPublicConfig)
            {
                Thread t2 = new Thread(() =>
                {
                    ChangeTitle("Updating Metadata...");
                    SideloaderRCLONE.UpdateMetadataFromPublic();

                    ChangeTitle("Processing Metadata...");
                    SideloaderRCLONE.ProcessMetadataFromPublic();
                })
                {
                    IsBackground = true
                };
                if (!isOffline)
                {
                    t2.Start();
                }

                while (t2.IsAlive)
                {
                    await Task.Delay(50);
                }
            }
            else
            {

                Thread t2 = new Thread(() =>
                {
                    ChangeTitle("Updating Game Notes...");
                    SideloaderRCLONE.UpdateGameNotes(currentRemote);
                });

                Thread t3 = new Thread(() =>
                {
                    ChangeTitle("Updating Game Thumbnails (This may take a minute or two)...");
                    SideloaderRCLONE.UpdateGamePhotos(currentRemote);
                });

                Thread t4 = new Thread(() =>
                {
                    SideloaderRCLONE.UpdateNouns(currentRemote);
                    if (!Directory.Exists(SideloaderRCLONE.ThumbnailsFolder) ||
                        !Directory.Exists(SideloaderRCLONE.NotesFolder))
                    {
                        _ = FlexibleMessageBox.Show(Program.form,
                            "It seems you are missing the thumbnails and/or notes database, the first start of the sideloader takes a bit more time, so dont worry if it looks stuck!");
                    }
                });
                t2.IsBackground = true;
                t3.IsBackground = true;
                t4.IsBackground = true;

                if (!isOffline)
                {
                    t2.Start();
                }

                while (t2.IsAlive)
                {
                    await Task.Delay(50);
                }

                if (!isOffline)
                {
                    t3.Start();
                }

                while (t3.IsAlive)
                {
                    await Task.Delay(50);
                }

                if (!isOffline)
                {
                    t4.Start();
                }

                while (t4.IsAlive)
                {
                    await Task.Delay(50);
                }
            }

            progressBar.Style = ProgressBarStyle.Marquee;

            ChangeTitle("Populating Game Update List, Almost There!");

            _ = await CheckForDevice();
            if (ADB.DeviceID.Length < 5)
            {
                nodeviceonstart = true;
            }
            listappsbtn();
            showAvailableSpace();
            downloadInstallGameButton.Enabled = true;
            isLoading = false;
            initListView();
            string[] files = Directory.GetFiles(Environment.CurrentDirectory);
            foreach (string file in files)
            {
                string fileName = file;
                while (fileName.Contains("\\"))
                {
                    fileName = fileName.Substring(fileName.IndexOf("\\") + 1);
                }
                if (!fileName.Contains(Properties.Settings.Default.CurrentLogName) && !fileName.Contains(Properties.Settings.Default.CurrentCrashName))
                {
                    if (!fileName.Contains("debuglog") && fileName.EndsWith(".txt"))
                    {
                        System.IO.File.Delete(fileName);
                    }
                }
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

        public async void ChangeTitle(string txt, bool reset = true)
        {
            try
            {
                if (ProgressText.IsDisposed)
                {
                    return;
                }

                this.Invoke(() => { oldTitle = txt; Text = "Rookie Sideloader v" + Updater.LocalVersion + " | " + txt; });
                ProgressText.Invoke(() =>
                {
                    if (!ProgressText.IsDisposed)
                    {
                        ProgressText.Text = txt;
                    }
                });
                if (!reset)
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
                this.Invoke(() => { Text = "Rookie Sideloader v" + Updater.LocalVersion + " | " + oldTitle; });
                ProgressText.Invoke(() =>
                {
                    if (!ProgressText.IsDisposed)
                    {
                        ProgressText.Text = oldTitle;
                    }
                });
            }
            catch
            {

            }
        }

        private void ShowSubMenu(Panel subMenu)
        {
            subMenu.Visible = subMenu.Visible == false;
        }

        private async void startsideloadbutton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
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

            Thread t1 = new Thread(() =>
            {
                output += ADB.Sideload(path);
            })
            {
                IsBackground = true
            };
            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            showAvailableSpace();

            ShowPrcOutput(output);
        }

        public void ShowPrcOutput(ProcessOutput prcout)
        {
            string message = $"Output: {prcout.Output}";
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
            string output = string.Empty;
            string error = string.Empty;
            string battery = string.Empty;
            ADB.DeviceID = GetDeviceID();
            Thread t1 = new Thread(() =>
            {
                output = ADB.RunAdbCommandToString("devices").Output;
            });


            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            string[] line = output.Split('\n');

            int i = 0;

            devicesComboBox.Items.Clear();

            _ = Logger.Log("Devices:");
            foreach (string currLine in line)
            {
                if (i > 0 && currLine.Length > 0)
                {
                    Devices.Add(currLine.Split('	')[0]);
                    _ = devicesComboBox.Items.Add(currLine.Split('	')[0]);
                    _ = Logger.Log(currLine.Split('	')[0] + "\n", "INFO", false);
                }
                Debug.WriteLine(currLine);
                i++;
            }



            if (devicesComboBox.Items.Count > 0)
            {
                devicesComboBox.SelectedIndex = 0;
            }

            battery = ADB.RunAdbCommandToString("shell dumpsys battery").Output;
            battery = Utilities.StringUtilities.RemoveEverythingBeforeFirst(battery, "level:");
            battery = Utilities.StringUtilities.RemoveEverythingAfterFirst(battery, "\n");
            battery = Utilities.StringUtilities.KeepOnlyNumbers(battery);
            BatteryLbl.Text = battery + "%";
            return devicesComboBox.SelectedIndex;



        }

        public async void devicesbutton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            _ = await CheckForDevice();

            ChangeTitlebarToDevice();

            showAvailableSpace();
        }

        public static void notify(string message)
        {
            if (Properties.Settings.Default.enableMessageBoxes == true)
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
            ProcessOutput output = new ProcessOutput("", "");
            FolderSelectDialog dialog = new FolderSelectDialog
            {
                Title = "Select OBB folder (must be direct OBB folder, E.G: com.Company.AppName)"
            };

            ADB.WakeDevice();
            if (dialog.Show(Handle))
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                string path = dialog.FileName;
                ChangeTitle($"Copying {path} obb to device...");
                Thread t1 = new Thread(() =>
                {
                    output += output += ADB.CopyOBB(path);
                })
                {
                    IsBackground = true
                };
                t1.Start();

                while (t1.IsAlive)
                {
                    await Task.Delay(100);
                }
                Program.form.ChangeTitle("Done.");
                showAvailableSpace();

                ShowPrcOutput(output);
                Program.form.ChangeTitle("");
            }
        }

        public void ChangeTitlebarToDevice()
        {
            if (!Devices.Contains("unauthorized"))
            {
                if (Devices[0].Length > 1 && Devices[0].Contains("unauthorized"))
                {
                    DeviceConnected = false;
                    this.Invoke(() =>
                    {
                        Text = "Device Not Authorized";
                        DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "Device not authorized, be sure to authorize computer on device.", "Not Authorized", MessageBoxButtons.RetryCancel);
                        if (dialogResult == DialogResult.Retry)
                        {
                            devicesbutton.PerformClick();
                            ;
                        }
                        else
                        {
                            return;
                        }

                    });
                }
                else if (Devices[0].Length > 1)
                {
                    this.Invoke(() => { Text = "Device Connected with ID | " + Devices[0].Replace("device", ""); });
                    DeviceConnected = true;
                }
                else
                {
                    this.Invoke(() =>
                    {
                        DeviceConnected = false;
                        Text = "No Device Connected";
                        if (!Properties.Settings.Default.nodevicemode)
                        {
                            DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "No device found. Please ensure the following: \n\n -Developer mode is enabled. \n -ADB drivers are installed. \n -ADB connection is enabled on your device (this can reset). \n -Your device is plugged in.\n\nThen press \"Retry\"", "No device found.", MessageBoxButtons.RetryCancel);
                            if (dialogResult == DialogResult.Retry)
                            {
                                devicesbutton.PerformClick();
                            }
                            else
                            {
                                return;
                            }
                        }


                    });
                }
            }
        }


        public async void showAvailableSpace()
        {
            string AvailableSpace = string.Empty;
            if (!Properties.Settings.Default.nodevicemode || DeviceConnected)
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
                }
                catch (Exception ex)
                {
                    _ = Logger.Log($"Unable to get available space with the exception: {ex}", "ERROR");

                }
            }
        }

        public string GetDeviceID()
        {
            string deviceId = string.Empty;
            int index = -1;
            devicesComboBox.Invoke(() => { index = devicesComboBox.SelectedIndex; });
            if (index != -1)
            {
                devicesComboBox.Invoke(() => { deviceId = devicesComboBox.SelectedItem.ToString(); });
            }

            return deviceId;
        }

        public static string taa = "";
        private async void backupbutton_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.customBackupDir)
            {
                BackupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Rookie Backups");
            }
            else
            {
                BackupFolder = Path.Combine((Properties.Settings.Default.backupDir), $"Rookie Backups");
            }
            if (!Directory.Exists(BackupFolder))
            {
                _ = Directory.CreateDirectory(BackupFolder);
            }
            ProcessOutput output = new ProcessOutput("", "");
            Thread t1 = new Thread(() =>
            {
                ADB.WakeDevice();
                string date_str = DateTime.Today.ToString("yyyy.MM.dd");
                string CurrBackups = Path.Combine(BackupFolder, date_str);
                _ = FlexibleMessageBox.Show(Program.form, $"This may take up to a minute. Backing up gamesaves to {BackupFolder}\\{date_str} (year.month.date)");
                _ = Directory.CreateDirectory(CurrBackups);
                output = ADB.RunAdbCommandToString($"pull \"/sdcard/Android/data\" \"{CurrBackups}\"");
                ChangeTitle("Backing up gamedatas...");
                try
                {
                    Directory.Move(ADB.adbFolderPath + "\\data", CurrBackups + "\\data");
                }
                catch (Exception ex)
                {
                    _ = Logger.Log($"Exception on backup: {ex}", "ERROR");
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
            ShowPrcOutput(output);
            ChangeTitle("                         \n\n");
        }

        private async void restorebutton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            ProcessOutput output = new ProcessOutput("", "");
            FolderSelectDialog dialog = new FolderSelectDialog
            {
                Title = "Select full backup or packagename backup folder"
            };
            if (dialog.Show(Handle))
            {
                string path = dialog.FileName;
                Thread t1 = new Thread(() =>
                {
                    if (path.Contains("data"))
                    {
                        output += ADB.RunAdbCommandToString($"push \"{path}\" /sdcard/Android/");
                    }
                    else
                    {
                        output += ADB.RunAdbCommandToString($"push \"{path}\" /sdcard/Android/data/");
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
            else
            {
                return;
            }

            ShowPrcOutput(output);
        }

        private string listapps()
        {
            ADB.DeviceID = GetDeviceID();
            return ADB.RunAdbCommandToString("shell pm list packages -3").Output;
        }

        public void listappsbtn()
        {
            m_combo.Invoke(() => { m_combo.Items.Clear(); });

            string[] line = listapps().Split('\n');

            string forsettings = string.Join("", line);
            Properties.Settings.Default.InstalledApps = forsettings;
            Properties.Settings.Default.Save();

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Length > 9)
                {
                    line[i] = line[i].Remove(0, 8);
                    line[i] = line[i].Remove(line[i].Length - 1);
                    foreach (string[] game in SideloaderRCLONE.games)
                    {
                        if (line[i].Length > 0 && game[2].Contains(line[i]))
                        {
                            line[i] = game[0];
                        }
                    }
                }
            }


            Array.Sort(line);

            foreach (string game in line)
            {
                if (game.Length > 0)
                {
                    m_combo.Invoke(() => { _ = m_combo.Items.Add(game); });
                }
            }

            m_combo.Invoke(() => { m_combo.MatchingMethod = StringMatchingMethod.NoWildcards; });
        }
        public static bool isuploading = false;
        public static bool isworking = false;
        private async void getApkButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            if (isOffline)
            {
                notify("You are not connected to the Internet!");
                return;
            }

            if (m_combo.SelectedIndex == -1)
            {
                notify("Please select an app first");
                return;
            }
            DialogResult dialogResult1 = FlexibleMessageBox.Show(Program.form, $"Do you want to upload {m_combo.SelectedItem} now?", "Upload app?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (!isworking)
            {
                isworking = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                string HWID = SideloaderUtilities.UUID();
                string GameName = m_combo.SelectedItem.ToString();
                string packageName = Sideloader.gameNameToPackageName(GameName);
                string InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packageName} | grep versionCode -F\"").Output;
                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                ulong VersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));

                string gameName = $"{GameName} v{VersionInt} {packageName} {HWID.Substring(0, 1)}";
                string gameZipName = $"{gameName}.zip";

                // delete the zip and txt if they exist from a previously failed upload
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\{gameZipName}"))
                {
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{gameZipName}");
                }

                if (File.Exists($"{Properties.Settings.Default.MainDir}\\{gameName}.txt"))
                {
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{gameName}.txt");
                }

                ProcessOutput output = new ProcessOutput("", "");
                ChangeTitle("Extracting APK....");

                _ = Directory.CreateDirectory($"{Properties.Settings.Default.MainDir}\\{packageName}");

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

                ChangeTitle("Extracting obb if it exists....");
                Thread t2 = new Thread(() =>
                {
                    output += ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packageName}\" \"{Properties.Settings.Default.MainDir}\\{packageName}\"");
                })
                {
                    IsBackground = true
                };
                t2.Start();

                while (t2.IsAlive)
                {
                    await Task.Delay(100);
                }

                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{packageName}\\HWID.txt", HWID);
                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{packageName}\\uploadMethod.txt", "manual");
                ChangeTitle("Zipping extracted application...");
                string cmd = $"7z a -mx1 \"{gameZipName}\" .\\{packageName}\\*";
                string path = $"{Properties.Settings.Default.MainDir}\\7z.exe";
                progressBar.Style = ProgressBarStyle.Continuous;
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

                ChangeTitle("Uploading to server, you can continue to use Rookie while it uploads in the background.");
                ULGif.Visible = true;
                ULLabel.Visible = true;
                ULGif.Enabled = true;
                isworking = false;
                isuploading = true;
                Thread t3 = new Thread(() =>
                {
                    string currentlyuploading = GameName;
                    ChangeTitle("Uploading to server, you can continue to use Rookie while it uploads in the background.");

                    // get size of pending zip upload and write to text file
                    long zipSize = new FileInfo($"{Properties.Settings.Default.MainDir}\\{gameZipName}").Length;
                    File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{gameName}.txt", zipSize.ToString());
                    // upload size file
                    _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{Properties.Settings.Default.MainDir}\\{gameName}.txt\" RSL-gameuploads:");
                    // upload zip
                    _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{Properties.Settings.Default.MainDir}\\{gameZipName}\" RSL-gameuploads:");

                    // deleting uploaded files
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{gameName}.txt");
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{gameZipName}");

                    this.Invoke(() => FlexibleMessageBox.Show(Program.form, $"Upload of {currentlyuploading} is complete! Thank you for your contribution!"));
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
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

                ChangeTitle("                         \n\n");
                isuploading = false;
                ULGif.Visible = false;
                ULLabel.Visible = false;
                ULGif.Enabled = false;
            }
            else
            {
                _ = MessageBox.Show("You must wait until each app is finished uploading to start another.");
            }
        }

        private async void uninstallAppButton_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.customBackupDir)
            {
                BackupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Rookie Backups");
            }
            else
            {
                BackupFolder = Path.Combine((Properties.Settings.Default.backupDir), $"Rookie Backups");
            }
            string packagename;
            ADB.WakeDevice();
            if (m_combo.SelectedIndex == -1)
            {
                _ = FlexibleMessageBox.Show(Program.form, "Please select an app first");
                return;
            }
            string GameName = m_combo.SelectedItem.ToString();
            DialogResult dialogresult = FlexibleMessageBox.Show($"Are you sure you want to uninstall {GameName}?", "Proceed with uninstall?", MessageBoxButtons.YesNo);
            if (dialogresult == DialogResult.No)
            {
                return;
            }
            DialogResult dialogresult2 = FlexibleMessageBox.Show($"Do you want to attempt to automatically backup any saves to {BackupFolder}\\(TodaysDate)", "Attempt Game Backup?", MessageBoxButtons.YesNo);
            packagename = !GameName.Contains(".") ? Sideloader.gameNameToPackageName(GameName) : GameName;
            if (dialogresult2 == DialogResult.Yes)
            {
                Sideloader.BackupGame(packagename);
            }
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.Style = ProgressBarStyle.Marquee;
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
            progressBar.Style = ProgressBarStyle.Continuous;
            m_combo.Items.RemoveAt(m_combo.SelectedIndex);
        }


        private async void copyBulkObbButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            FolderSelectDialog dialog = new FolderSelectDialog
            {
                Title = "Select your folder with OBBs"
            };
            if (dialog.Show(Handle))
            {
                Thread t1 = new Thread(() =>
                {
                    Sideloader.RecursiveOutput = new ProcessOutput("", "");
                    Sideloader.RecursiveCopyOBB(dialog.FileName);
                })
                {
                    IsBackground = true
                };
                t1.Start();

                showAvailableSpace();

                while (t1.IsAlive)
                {
                    await Task.Delay(100);
                }

                ShowPrcOutput(Sideloader.RecursiveOutput);
            }
        }

        private async void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (nodeviceonstart && !updatesnotified)
            {
                _ = await CheckForDevice();
                ChangeTitlebarToDevice();
                showAvailableSpace();
                ChangeTitle("Device now detected... refreshing update list.");
                listappsbtn();
                initListView();
            }

            Program.form.ChangeTitle($"Processing dropped file. If Rookie freezes, please wait. Do not close Rookie!");

            DragDropLbl.Visible = false;
            ProcessOutput output = new ProcessOutput("", "");
            ADB.WakeDevice();
            ADB.DeviceID = GetDeviceID();
            progressBar.Style = ProgressBarStyle.Marquee;
            CurrPCKG = "";
            string[] datas = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string data in datas)
            {
                string directory = Path.GetDirectoryName(data);
                //if is directory
                string dir = Path.GetDirectoryName(data);
                string path = $"{dir}\\Install.txt";
                if (Directory.Exists(data))
                {
                    if (!data.Contains("+") && !data.Contains("_") && data.Contains("."))
                    {
                        _ = Logger.Log($"Copying {data} to device");
                        Program.form.ChangeTitle($"Copying {data} to device...");

                        Thread t2 = new Thread(() =>

                        {
                            output += ADB.CopyOBB(data);
                        })
                        {
                            IsBackground = true
                        };
                        t2.Start();

                        while (t2.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        Program.form.ChangeTitle("");
                        Properties.Settings.Default.CurrPckg = dir;
                        Properties.Settings.Default.Save();
                    }
                    Program.form.ChangeTitle($"");
                    string extension = Path.GetExtension(data);
                    string[] files = Directory.GetFiles(data);
                    foreach (string file2 in files)
                    {
                        if (File.Exists(file2))
                        {
                            if (file2.EndsWith(".apk"))
                            {
                                string pathname = Path.GetDirectoryName(file2);
                                string filename = file2.Replace($"{pathname}\\", "");

                                string cmd = $"C:\\RSL\\platform-tools\\aapt.exe\" dump badging \"{file2}\" | findstr -i \"package: name\"";
                                _ = Logger.Log($"Running adb command-{cmd}");
                                string cmdout = ADB.RunCommandToString(cmd, file2).Output;
                                cmdout = Utilities.StringUtilities.RemoveEverythingBeforeFirst(cmdout, "=");
                                cmdout = Utilities.StringUtilities.RemoveEverythingAfterFirst(cmdout, " ");
                                cmdout = cmdout.Replace("'", "");
                                cmdout = cmdout.Replace("=", "");
                                CurrPCKG = cmdout;
                                CurrAPK = file2;
                                System.Windows.Forms.Timer t3 = new System.Windows.Forms.Timer
                                {
                                    Interval = 150000 // 180 seconds to fail
                                };
                                t3.Tick += timer_Tick4;
                                t3.Start();
                                Program.form.ChangeTitle($"Sideloading apk ({filename})");

                                Thread t2 = new Thread(() =>
                                {
                                    output += ADB.Sideload(file2);
                                })
                                {
                                    IsBackground = true
                                };
                                t2.Start();
                                while (t2.IsAlive)
                                {
                                    await Task.Delay(100);
                                }

                                t3.Stop();
                                if (Directory.Exists($"{pathname}\\{cmdout}"))
                                {
                                    _ = Logger.Log($"Copying obb folder to device- {cmdout}");
                                    Program.form.ChangeTitle($"Copying obb folder to device...");
                                    Thread t1 = new Thread(() =>
                                    {
                                        _ = ADB.RunAdbCommandToString($"push \"{pathname}\\{cmdout}\" /sdcard/Android/obb/");
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
                            }

                            if (file2.EndsWith(".zip") && Properties.Settings.Default.BMBFchecked)
                            {
                                string datazip = file2;
                                string zippath = Path.GetDirectoryName(data);
                                datazip = datazip.Replace(zippath, "");
                                datazip = Utilities.StringUtilities.RemoveEverythingAfterFirst(datazip, ".");
                                datazip = datazip.Replace(".", "");
                                string command2 = $"\"{Properties.Settings.Default.MainDir}\\7z.exe\" e \"{file2}\" -o\"{zippath}\\{datazip}\\\"";

                                Thread t2 = new Thread(() =>

                                {
                                    _ = ADB.RunCommandToString(command2, file2);
                                    output += ADB.RunAdbCommandToString($"push \"{zippath}\\{datazip}\" /sdcard/ModData/com.beatgames.beatsaber/Mods/SongLoader/CustomLevels/");
                                })
                                {
                                    IsBackground = true
                                };
                                t2.Start();

                                while (t2.IsAlive)
                                {
                                    await Task.Delay(100);
                                }

                                Directory.Delete($"{zippath}\\{datazip}", true);
                            }
                        }
                    }
                    string[] folders = Directory.GetDirectories(data);
                    foreach (string folder in folders)
                    {
                        _ = Logger.Log($"Copying {folder} to device");
                        Program.form.ChangeTitle($"Copying {folder} to device...");

                        Thread t2 = new Thread(() =>

                        {
                            output += ADB.CopyOBB(folder);
                        })
                        {
                            IsBackground = true
                        };
                        t2.Start();

                        while (t2.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        Program.form.ChangeTitle("");
                        Properties.Settings.Default.CurrPckg = dir;
                        Properties.Settings.Default.Save();
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
                                ChangeTitle("Sideloading custom install.txt automatically.");

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

                                ChangeTitle(" \n\n");

                            }
                        }
                        else
                        {
                            string pathname = Path.GetDirectoryName(data);
                            string dataname = data.Replace($"{pathname}\\", "");
                            string cmd = $"\"C:\\RSL\\platform-tools\\aapt.exe\" dump badging \"{data}\" | findstr -i \"package: name\"";
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

                            ChangeTitle($"Installing {dataname}...");

                            Thread t1 = new Thread(() =>
                            {
                                output += ADB.Sideload(data);
                            })
                            {
                                IsBackground = true
                            };
                            t1.Start();
                            while (t1.IsAlive)
                            {
                                await Task.Delay(100);
                            }

                            timer.Stop();

                            if (Directory.Exists($"{pathname}\\{cmdout}"))
                            {
                                _ = Logger.Log($"Copying obb folder to device- {cmdout}");
                                Program.form.ChangeTitle($"Copying obb folder to device...");
                                Thread t2 = new Thread(() =>
                                {
                                    _ = ADB.RunAdbCommandToString($"push \"{pathname}\\{cmdout}\" /sdcard/Android/obb/");
                                })
                                {
                                    IsBackground = true
                                };
                                t2.Start();
                                while (t2.IsAlive)
                                {
                                    await Task.Delay(100);
                                }

                                ChangeTitle(" \n\n");
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
                        foldername = Environment.CurrentDirectory + "\\" + foldername;
                        _ = Directory.CreateDirectory(foldername);
                        File.Copy(data, foldername + "\\" + filename);
                        path = foldername;

                        Thread t1 = new Thread(() =>
                        {
                            output += ADB.CopyOBB(path);
                        })
                        {
                            IsBackground = true
                        };
                        _ = Logger.Log($"Copying obb folder to device- {path}");
                        Program.form.ChangeTitle($"Copying obb folder to device ({filename})");
                        t1.Start();

                        while (t1.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        Directory.Delete(foldername, true);
                        ChangeTitle(" \n\n");
                    }
                    // BMBF Zip extraction then push to BMBF song folder on Quest.
                    else if (extension == ".zip" && Properties.Settings.Default.BMBFchecked)
                    {
                        string datazip = data;
                        string zippath = Path.GetDirectoryName(data);
                        datazip = datazip.Replace(zippath, "");
                        datazip = Utilities.StringUtilities.RemoveEverythingAfterFirst(datazip, ".");
                        datazip = datazip.Replace(".", "");

                        string command = $"\"{Properties.Settings.Default.MainDir}\\7z.exe\" e \"{data}\" -o\"{zippath}\\{datazip}\\\"";

                        Thread t1 = new Thread(() =>

                        {
                            _ = ADB.RunCommandToString(command, data);
                            output += ADB.RunAdbCommandToString($"push \"{zippath}\\{datazip}\" /sdcard/ModData/com.beatgames.beatsaber/Mods/SongLoader/CustomLevels/");
                        })
                        {
                            IsBackground = true
                        };
                        t1.Start();

                        while (t1.IsAlive)
                        {
                            await Task.Delay(100);
                        }

                        Directory.Delete($"{zippath}\\{datazip}", true);
                    }
                    else if (extension == ".txt")
                    {
                        ChangeTitle("Sideloading custom install.txt automatically.");

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

                        ChangeTitle(" \n\n");
                    }
                }
            }

            progressBar.Style = ProgressBarStyle.Continuous;

            showAvailableSpace();

            DragDropLbl.Visible = false;

            ShowPrcOutput(output);
            listappsbtn();

        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }

            DragDropLbl.Visible = true;
            DragDropLbl.Text = "Drag apk or obb";
            ChangeTitle(DragDropLbl.Text);
        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {
            DragDropLbl.Visible = false;
            DragDropLbl.Text = "";

            ChangeTitle(" \n\n");
        }

        private List<string> newGamesList = new List<string>();
        private List<string> newGamesToUploadList = new List<string>();

        private readonly List<UpdateGameData> gamesToAskForUpdate = new List<UpdateGameData>();
        public static bool loaded = false;
        public static string rookienamelist;
        public static string rookienamelist2;
        private bool errorOnList;
        public static bool updates = false;
        public static bool newapps = false;
        public static int newint = 0;
        public static int updint = 0;
        public static bool nodeviceonstart = false;
        public static bool either = false;

        private async void initListView()
        {
            rookienamelist = "";
            loaded = false;
            string lines = Properties.Settings.Default.InstalledApps;
            string pattern = "package:";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(lines, replacement);
            char[] delims = new[] { '\r', '\n' };
            string[] packageList = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            string[] blacklist = new string[] { };
            string[] whitelist = new string[] { };
            if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt"))
            {
                blacklist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt");
            }
            if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt"))
            {
                whitelist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt");
            }

            List<ListViewItem> GameList = new List<ListViewItem>();

            List<string> rookieList = new List<string>();
            List<string> installedGames = packageList.ToList();
            List<string> blacklistItems = blacklist.ToList();
            List<string> whitelistItems = whitelist.ToList();
            errorOnList = false;
            //This is for black list, but temporarly will be whitelist
            //this list has games that we are actually going to upload
            newGamesToUploadList = whitelistItems.Intersect(installedGames).ToList();
            progressBar.Style = ProgressBarStyle.Marquee;
            if (SideloaderRCLONE.games.Count > 5)
            {
                Thread t1 = new Thread(() =>
                {
                    foreach (string[] release in SideloaderRCLONE.games)
                    {
                        rookieList.Add(release[SideloaderRCLONE.PackageNameIndex].ToString());
                        if (!rookienamelist.Contains(release[SideloaderRCLONE.GameNameIndex].ToString()))
                        {
                            rookienamelist += release[SideloaderRCLONE.GameNameIndex].ToString() + "\n";
                            rookienamelist2 += release[SideloaderRCLONE.GameNameIndex].ToString() + ", ";
                        }

                        ListViewItem Game = new ListViewItem(release);

                        Color colorFont_installedGame = ColorTranslator.FromHtml("#3c91e6");
                        lblUpToDate.ForeColor = colorFont_installedGame;
                        Color colorFont_updateAvailable = ColorTranslator.FromHtml("#4daa57");
                        lblUpdateAvailable.ForeColor = colorFont_updateAvailable;
                        Color colorFont_donateGame = ColorTranslator.FromHtml("#cb9cf2");
                        lblNeedsDonate.ForeColor = colorFont_donateGame;
                        Color colorFont_error = ColorTranslator.FromHtml("#f52f57");

                        foreach (string packagename in packageList)
                        {
                            if (string.Equals(release[SideloaderRCLONE.PackageNameIndex], packagename))
                            {
                                Game.ForeColor = colorFont_installedGame;

                                string InstalledVersionCode;
                                InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename} | grep versionCode -F\"").Output;
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                try
                                {
                                    ulong installedVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                    ulong cloudVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));

                                    _ = Logger.Log($"Checked game {release[SideloaderRCLONE.GameNameIndex]}; cloudversion={cloudVersionInt} localversion={installedVersionInt}");
                                    if (installedVersionInt < cloudVersionInt)
                                    {
                                        Game.ForeColor = colorFont_updateAvailable;
                                    }

                                    if (installedVersionInt > cloudVersionInt)
                                    {
                                        bool dontget = false;
                                        if (blacklist.Contains(packagename))
                                        {
                                            dontget = true;
                                        }

                                        if (!dontget)
                                        {
                                            Game.ForeColor = colorFont_donateGame;
                                        }

                                        string RlsName = Sideloader.PackageNametoGameName(packagename);
                                        string GameName = Sideloader.gameNameToSimpleName(RlsName);

                                        if (!dontget && !updatesnotified && !isworking && updint < 6 && !Properties.Settings.Default.SubmittedUpdates.Contains(packagename))
                                        {
                                            either = true;
                                            updates = true;
                                            updint++;
                                            UpdateGameData gameData = new UpdateGameData(GameName, packagename, installedVersionInt);
                                            gamesToAskForUpdate.Add(gameData);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Game.ForeColor = colorFont_error;
                                    _ = Logger.Log($"An error occured while rendering game {release[SideloaderRCLONE.GameNameIndex]} in ListView", "ERROR");
                                    _ = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename}\"");
                                    _ = Logger.Log($"ExMsg: {ex.Message}Installed:\"{InstalledVersionCode}\" Cloud:\"{Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex])}\"", "ERROR");
                                }
                            }
                        }
                        GameList.Add(Game);
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
            else if (!isOffline)
            {
                SwitchMirrors();
                initListView();
            }


            if (blacklistItems.Count == 0 && GameList.Count == 0 && !Properties.Settings.Default.nodevicemode && !isOffline)
            {
                //This means either the user does not have headset connected or the blacklist
                //did not load, so we are just going to skip everything
                errorOnList = true;
                _ = FlexibleMessageBox.Show(Program.form, $"Rookie seems to have failed to load all resources. Please try restarting Rookie a few times.\nIf error still persists please disable any VPN or firewalls (rookie uses direct download so a VPN is not needed)\nIf this error still persists try a system reboot, reinstalling the program, and lastly posting the problem on telegram.", "Error loading blacklist or game list!");
            }
            newGamesList = installedGames.Except(rookieList).Except(blacklistItems).ToList();
            int topItemIndex = 0;
            try
            {
                if (gamesListView.Items.Count > 1)
                {
                    topItemIndex = gamesListView.TopItem.Index;
                }
            }
            catch (Exception ex)
            {
                _ = FlexibleMessageBox.Show(Program.form, $"Error building game list: {ex.Message}");
            }

            try
            {
                if (topItemIndex != 0)
                {
                    gamesListView.TopItem = gamesListView.Items[topItemIndex];
                }
            }
            catch (Exception ex)
            {
                _ = FlexibleMessageBox.Show(Program.form, $"Error building game list: {ex.Message}");
            }
            Thread t2 = new Thread(() =>
            {
                if (!errorOnList)
                {

                    //This is for games that we already have on rookie and user has an update
                    if (blacklistItems.Count > 100 && rookieList.Count > 100)
                    {
                        foreach (UpdateGameData gameData in gamesToAskForUpdate)
                        {
                            if (!updatesnotified && !Properties.Settings.Default.SubmittedUpdates.Contains(gameData.Packagename))
                            {
                                either = true;
                                updates = true;
                                DonorApps += gameData.GameName + ";" + gameData.Packagename + ";" + gameData.InstalledVersionInt + ";" + "Update" + "\n";
                            }

                        }
                    }

                    //This is for WhiteListed Games, they will be asked for first, if we don't get many bogus prompts we can remove this entire duplicate section.
                    /* foreach (string newGamesToUpload in newGamesToUploadList)
                       {
                           string RlsName = Sideloader.PackageNametoGameName(newGamesToUpload);

                           //start of code to get official Release Name from APK by first extracting APK then running AAPT on it.
                           string apppath = ADB.RunAdbCommandToString($"shell pm path {newGamesToUpload}").Output;
                           apppath = Utilities.StringUtilities.RemoveEverythingBeforeFirst(apppath, "/");
                           apppath = Utilities.StringUtilities.RemoveEverythingAfterFirst(apppath, "\r\n");
                           if (File.Exists($"C:\\RSL\\platform-tools\\base.apk"))
                               File.Delete($"C:\\RSL\\platform-tools\\base.apk");
                           ADB.RunAdbCommandToString($"pull \"{apppath}\"");
                           string cmd = $"\"C:\\RSL\\platform-tools\\aapt.exe\" dump badging \"C:\\RSL\\platform-tools\\base.apk\" | findstr -i \"application-label\"";
                           string workingpath = "C:\\RSL\\platform-tools\\aapt.exe";
                           string ReleaseName = ADB.RunCommandToString(cmd, workingpath).Output;
                           ReleaseName = Utilities.StringUtilities.RemoveEverythingBeforeFirst(ReleaseName, "'");
                           ReleaseName = Utilities.StringUtilities.RemoveEverythingAfterFirst(ReleaseName, "\r\n");
                           ReleaseName = ReleaseName.Replace("'", "");
                           File.Delete($"C:\\RSL\\platform-tools\\base.apk");
                           //end

                           string GameName = Sideloader.gameNameToSimpleName(RlsName);
                           Logger.Log(newGamesToUpload);
                           if (!updatesnotified)
                           {
                               DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, $"You have an in demand game:\n\n{ReleaseName}\n\nRSL can AUTOMATICALLY UPLOAD the clean files to a shared drive in the background,\nthis is the only way to keep the apps up to date for everyone.\n\nNOTE: Rookie will only extract the APK/OBB which contain NO personal information whatsoever.", "CONTRIBUTE CLEAN FILES?", MessageBoxButtons.YesNo);
                               if (dialogResult == DialogResult.Yes)
                               {
                                   string InstalledVersionCode;
                                   InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {newGamesToUpload} | grep versionCode -F\"").Output;
                                   InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                   InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                   ulong installedVersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                               }
                               else
                               {
                                   return;
                               }
                           }
                       }*/
                    //This is for games that are not blacklisted and we dont have on rookie
                    if (blacklistItems.Count > 100 && rookieList.Count > 100)
                    {

                        foreach (string newGamesToUpload in newGamesList)
                        {
                            ChangeTitle("Unrecognized App Found. Downloading APK to take a closer look. (This may take a minute)");
                            bool onapplist = false;
                            string NewApp = Properties.Settings.Default.NonAppPackages + "\n" + Properties.Settings.Default.AppPackages;
                            if (NewApp.Contains(newGamesToUpload))
                            {
                                onapplist = true;
                            }

                            string RlsName = Sideloader.PackageNametoGameName(newGamesToUpload);

                            if (!updatesnotified && !onapplist && newint < 6)
                            {
                                either = true;
                                newapps = true;
                                //start of code to get official Release Name from APK by first extracting APK then running AAPT on it.
                                string apppath = ADB.RunAdbCommandToString($"shell pm path {newGamesToUpload}").Output;
                                apppath = Utilities.StringUtilities.RemoveEverythingBeforeFirst(apppath, "/");
                                apppath = Utilities.StringUtilities.RemoveEverythingAfterFirst(apppath, "\r\n");
                                if (File.Exists($"C:\\RSL\\platform-tools\\base.apk"))
                                {
                                    File.Delete($"C:\\RSL\\platform-tools\\base.apk");
                                }

                                _ = ADB.RunAdbCommandToString($"pull \"{apppath}\"");
                                string cmd = $"\"C:\\RSL\\platform-tools\\aapt.exe\" dump badging \"C:\\RSL\\platform-tools\\base.apk\" | findstr -i \"application-label\"";
                                string workingpath = $"C:\\RSL\\platform-tools\\aapt.exe";
                                string ReleaseName = ADB.RunCommandToString(cmd, workingpath).Output;
                                ReleaseName = Utilities.StringUtilities.RemoveEverythingBeforeFirst(ReleaseName, "'");
                                ReleaseName = Utilities.StringUtilities.RemoveEverythingAfterFirst(ReleaseName, "\r\n");
                                ReleaseName = ReleaseName.Replace("'", "");
                                File.Delete($"C:\\RSL\\platform-tools\\base.apk");
                                if (ReleaseName.Contains("Microsoft Windows"))
                                {
                                    ReleaseName = RlsName;
                                }
                                //end
                                string GameName = Sideloader.gameNameToSimpleName(RlsName);
                                //Logger.Log(newGamesToUpload);
                                string InstalledVersionCode;
                                InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {newGamesToUpload} | grep versionCode -F\"").Output;
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                ulong installedVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                DonorApps += ReleaseName + ";" + newGamesToUpload + ";" + installedVersionInt + ";" + "New App" + "\n";
                                newint++;
                            }
                        }
                    }
                }
            })
            {
                IsBackground = true
            };
            t2.Start();
            while (t2.IsAlive)
            {
                await Task.Delay(100);
            }
            progressBar.Style = ProgressBarStyle.Continuous;

            if (either && !updatesnotified)
            {
                ChangeTitle("                                                \n\n");
                DonorsListViewForm DonorForm = new DonorsListViewForm();
                _ = DonorForm.ShowDialog();
                _ = Focus();
            }
            ChangeTitle("Populating update list...                               \n\n");
            ListViewItem[] arr = GameList.ToArray();
            gamesListView.BeginUpdate();
            gamesListView.Items.Clear();
            gamesListView.Items.AddRange(arr);
            gamesListView.EndUpdate();
            ChangeTitle("                                                \n\n");
            loaded = true;
        }

        public static async void DoUpload()
        {
            Program.form.ChangeTitle("Uploading to server, you can continue to use Rookie while it uploads in the background.");
            Program.form.ULGif.Visible = true;
            Program.form.ULLabel.Visible = true;
            Program.form.ULGif.Enabled = true;
            isworking = true;

            foreach (UploadGame game in Program.form.gamesToUpload)
            {

                Thread t3 = new Thread(() =>
                {
                    string packagename = game.Pckgcommand;
                    string gameName = $"{game.Uploadgamename} v{game.Uploadversion} {game.Pckgcommand} {SideloaderUtilities.UUID().Substring(0, 1)}";
                    string gameZipName = $"{gameName}.zip";

                    // delete the zip and txt if they exist from a previously failed upload
                    if (File.Exists($"{Properties.Settings.Default.MainDir}\\{gameZipName}"))
                    {
                        File.Delete($"{Properties.Settings.Default.MainDir}\\{gameZipName}");
                    }

                    if (File.Exists($"{Properties.Settings.Default.MainDir}\\{gameName}.txt"))
                    {
                        File.Delete($"{Properties.Settings.Default.MainDir}\\{gameName}.txt");
                    }

                    string path = $"{Properties.Settings.Default.MainDir}\\7z.exe";
                    string cmd = $"7z a -mx1 \"{Properties.Settings.Default.MainDir}\\{gameZipName}\" .\\{game.Pckgcommand}\\*";
                    Program.form.ChangeTitle("Zipping extracted application...");
                    _ = ADB.RunCommandToString(cmd, path);
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{game.Pckgcommand}", true);
                    Program.form.ChangeTitle("Uploading to server, you may continue to use Rookie while it uploads.");

                    // get size of pending zip upload and write to text file
                    long zipSize = new FileInfo($"{Properties.Settings.Default.MainDir}\\{gameZipName}").Length;
                    File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{gameName}.txt", zipSize.ToString());
                    // upload size file
                    _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{Properties.Settings.Default.MainDir}\\{gameName}.txt\" RSL-gameuploads:");
                    // upload zip
                    _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{Properties.Settings.Default.MainDir}\\{gameZipName}\" RSL-gameuploads:");

                    if (game.isUpdate)
                    {
                        Properties.Settings.Default.SubmittedUpdates += game.Pckgcommand + "\n";
                        Properties.Settings.Default.Save();
                    }

                    // deleting uploaded files
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{gameName}.txt");
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{gameZipName}");

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
            Program.form.ULGif.Visible = false;
            Program.form.ULLabel.Visible = false;
            Program.form.ULGif.Enabled = false;
            Program.form.ChangeTitle(" \n\n");
        }

        public static async void newpackageupload()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.NonAppPackages) && !Properties.Settings.Default.ListUpped)
            {
                Random r = new Random();
                int x = r.Next(9999);
                int y = x;
                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\FreeOrNonVR{y}.txt", Properties.Settings.Default.NonAppPackages);
                string path = $"{Properties.Settings.Default.MainDir}\\rclone\\rclone.exe";

                Thread t1 = new Thread(() =>
                {
                    _ = ADB.RunCommandToString($"\"{Properties.Settings.Default.MainDir}\\rclone\\rclone.exe\" copy \"{Properties.Settings.Default.MainDir}\\FreeOrNonVR{y}.txt\" VRP-debuglogs:InstalledGamesList", path);
                    File.Delete($"{Properties.Settings.Default.MainDir}\\FreeOrNonVR{y}.txt");
                })
                {
                    IsBackground = true
                };
                t1.Start();

                while (t1.IsAlive)
                {
                    await Task.Delay(100);
                }

                Properties.Settings.Default.ListUpped = true;
                Properties.Settings.Default.Save();

            }
        }


        public async Task extractAndPrepareGameToUploadAsync(string GameName, string packagename, ulong installedVersionInt, bool isupdate)
        {
            progressBar.Style = ProgressBarStyle.Marquee;

            Thread t1 = new Thread(() =>
            {
                _ = Sideloader.getApk(packagename);
            });
            ChangeTitle("Extracting APK file....");
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            ChangeTitle("Extracting obb if it exists....");
            Thread t2 = new Thread(() =>
            {
                _ = ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packagename}\" \"{Properties.Settings.Default.MainDir}\\{packagename}\"");
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
            File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{packagename}\\HWID.txt", HWID);
            progressBar.Style = ProgressBarStyle.Continuous;
            UploadGame game = new UploadGame
            {
                isUpdate = isupdate,
                Pckgcommand = packagename,
                Uploadgamename = GameName,
                Uploadversion = installedVersionInt
            };
            gamesToUpload.Add(game);
        }
        private void initMirrors(bool random)
        {
            int index = 0;
            remotesList.Invoke(() => { index = remotesList.SelectedIndex; remotesList.Items.Clear(); });

            string[] mirrors = RCLONE.runRcloneCommand_DownloadConfig("listremotes").Output.Split('\n');

            _ = Logger.Log("Loaded following mirrors: ");
            int itemsCount = 0;
            foreach (string mirror in mirrors)
            {
                if (mirror.Contains("mirror"))
                {


                    _ = Logger.Log(mirror.Remove(mirror.Length - 1));
                    remotesList.Invoke(() => { _ = remotesList.Items.Add(mirror.Remove(mirror.Length - 1).Replace("VRP-mirror", "")); });
                    itemsCount++;

                }
            }

            if (itemsCount > 0)
            {

                Random rand = new Random();
                // Code that implements a randomized mirror.  The rotation logic (the rotation) is reported as being bugged so I just disabled as a workaround ~pmow
                //  if (random == true && index < itemsCount)
                //    index = rand.Next(0, itemsCount);
                //    remotesList.Invoke(() =>
                // {
                //     remotesList.SelectedIndex = index;
                //     remotesList.SelectedIndex = 0;
                //    currentRemote = "VRP-mirror" + remotesList.SelectedItem.ToString();
                //  });

                remotesList.Invoke(() =>
            {
                remotesList.SelectedIndex = 0; //set mirror to first
                currentRemote = "VRP-mirror" + remotesList.SelectedItem.ToString();
            });


            };
        }

        public static string processError = string.Empty;

        public static string currentRemote = string.Empty;

        private readonly string wrDelimiter = "-------";

        private void sideloadContainer_Click(object sender, EventArgs e)
        {
            ShowSubMenu(sideloadContainer);
            sideloadDrop.Text = (sideloadDrop.Text == "▼ SIDELOAD ▼") ? "▶ SIDELOAD ◀" : "▼ SIDELOAD ▼";
        }


        private void backupDrop_Click(object sender, EventArgs e)
        {
            ShowSubMenu(backupContainer);
            backupDrop.Text = (backupDrop.Text == "▼ BACKUP / RESTORE ▼") ? "▶ BACKUP / RESTORE ◀" : "▼ BACKUP / RESTORE ▼";
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show(Program.form);
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            string about = $@"Version: {Updater.LocalVersion}

 - Software orignally coded by rookie.wtf
 - Thanks to pmow for all of his work, including rclone, wonka and other projects, and for scripting the backend
without him none of this would be possible
 - Thanks to HarryEffinPotter for all his Rookie improvements
 - Thanks to the VRP Mod Staff, data team, and anyone else I missed!
 - Thanks to VRP members of the past and present: Roma/Rookie, Flow, Ivan, Kaladin, John, Sam Hoque
 
 - Additional Thanks and Credits:
 - -- rclone https://rclone.org/
 - -- 7zip https://www.7-zip.org/
 - -- badcoder5000: for help with the UI Redesign
 - -- Verb8em: for drawning the New Icon
 - -- ErikE: Folder Browser Dialog Code (https://stackoverflow.com/users/57611/erike)
 - -- Serge Weinstock: for developing SergeUtils, which is used to search the combobox
 - -- Mike Gold: for the scrollable message box (https://www.c-sharpcorner.com/members/mike-gold2)
 ";

            _ = FlexibleMessageBox.Show(Program.form, about);
        }

        private async void ADBWirelessEnable_Click(object sender, EventArgs e)
        {
            bool Manual;
            DialogResult res = FlexibleMessageBox.Show("Do you want Rookie to find the IP or enter it manually\nYes = Automatic\nNo = Manual", "Automatic/Manual", MessageBoxButtons.YesNo);
            Manual = res == DialogResult.No;
            if (Manual)
            {
                ADBcommandbox.Visible = true;
                ADBcommandbox.Clear();
                lblAdbCommand.Visible = true;
                lblAdbCommand.Text = "Enter your Quests\r\nIP Address";
                lblAdbCommand.Location = new System.Drawing.Point(531, 213);
                label2.Visible = true;
                manualIP = true;
                _ = ADBcommandbox.Focus();
                Program.form.ChangeTitle("Attempting manual connection...", false);
            }
            else
            {
                ADB.WakeDevice();
                DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "Make sure your Quest is plugged in VIA USB then press OK, if you need a moment press Cancel and come back when you're ready.", "Connect Quest now.", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }

                _ = ADB.RunAdbCommandToString("devices");
                _ = ADB.RunAdbCommandToString("tcpip 5555");

                _ = FlexibleMessageBox.Show(Program.form, "Press OK to get your Quest's local IP address.", "Obtain local IP address", MessageBoxButtons.OKCancel);
                Thread.Sleep(1000);
                string input = ADB.RunAdbCommandToString("shell ip route").Output;

                Properties.Settings.Default.WirelessADB = true;
                Properties.Settings.Default.Save();
                _ = new string[] { "" };
                string[] strArrayOne = input.Split(' ');
                if (strArrayOne[0].Length > 7)
                {
                    string IPaddr = strArrayOne[8];
                    string IPcmnd = "connect " + IPaddr + ":5555";
                    _ = FlexibleMessageBox.Show(Program.form, $"Your Quest's local IP address is: {IPaddr}\n\nPlease disconnect your Quest then wait 2 seconds.\nOnce it is disconnected hit OK", "", MessageBoxButtons.OK);
                    Thread.Sleep(2000);
                    _ = ADB.RunAdbCommandToString(IPcmnd);
                    _ = await Program.form.CheckForDevice();
                    Program.form.ChangeTitlebarToDevice();
                    Program.form.showAvailableSpace();
                    Properties.Settings.Default.IPAddress = IPcmnd;
                    Properties.Settings.Default.Save();
                    File.WriteAllText($"C:\\RSL\\platform-tools\\StoredIP.txt", IPcmnd);
                    ADB.wirelessadbON = true;
                    _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                    _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                }
                else
                {
                    _ = FlexibleMessageBox.Show(Program.form, "No device connected! Connect quest via USB and start again!");
                }
            }
        }

        private async void listApkButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            ChangeTitle("Refreshing connected devices, installed apps and update list...");
            if (isLoading)
            {
                return;
            }

            isLoading = true;

            progressBar.Style = ProgressBarStyle.Marquee;
            devicesbutton_Click(sender, e);

            Thread t1 = new Thread(() =>
            {
                initMirrors(false);
                if (!hasPublicConfig)
                {
                    SideloaderRCLONE.initGames(currentRemote);
                }
                listappsbtn();
            })
            {
                IsBackground = false
            };
            t1.Start();
            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            initListView();
            isLoading = false;

            ChangeTitle(" \n\n");
        }

        private static readonly HttpClient client = new HttpClient();
        public static bool reset = false;
        public static bool updatedConfig = false;
        public static int steps = 0;
        public static bool gamesAreDownloading = false;
        private readonly BindingList<string> gamesQueueList = new BindingList<string>();
        public static int quotaTries = 0;
        public static bool timerticked = false;
        public static bool skiponceafterremove = false;


        public void SwitchMirrors()
        {
            try
            {
                quotaTries++;
                remotesList.Invoke(() =>
                {
                    if (quotaTries > remotesList.Items.Count)
                    {
                        ShowError_QuotaExceeded();

                        DialogResult om = MessageBox.Show("Relaunch Rookie in Offline Mode?", "Offline Mode?", MessageBoxButtons.YesNo);
                        if (om == DialogResult.Yes)
                        {
                            Process pr = new Process();
                            pr.StartInfo.WorkingDirectory = Application.StartupPath;
                            pr.StartInfo.FileName = System.AppDomain.CurrentDomain.FriendlyName;
                            pr.StartInfo.Arguments = "--offline";
                            _ = pr.Start();
                            Process.GetCurrentProcess().Kill();
                        }

                        if (System.Windows.Forms.Application.MessageLoop)
                        {
                            Process.GetCurrentProcess().Kill();
                        }

                    }
                    if (remotesList.SelectedIndex + 1 == remotesList.Items.Count)
                    {
                        reset = true;
                        for (int i = 0; i < steps; i++)
                        {
                            remotesList.SelectedIndex--;
                        }
                    }
                    if (reset)
                    {
                        remotesList.SelectedIndex--;
                    }
                    if (remotesList.Items.Count > remotesList.SelectedIndex && !reset)
                    {
                        remotesList.SelectedIndex++;
                        steps++;
                    }
                });
            }
            catch { }
        }

        private static void ShowError_QuotaExceeded()
        {
            const string errorMessage =
@"Unable to connect to Remote Server. Rookie is unable to connect to our Servers.

First time launching Rookie? Please relaunch and try again.

Things you can try:
1) Use a third party config from the wiki (https://wiki.vrpirates.club/general_information/third-party-rclone-configs)
2) Use Resilio for p2p downloads (https://wiki.vrpirates.club/en/Howto/Resilio-Sync-setup-guide)
3) Sponsor a private server (https://wiki.vrpirates.club/en/Howto/sponsored-mirrors)
";

            _ = FlexibleMessageBox.Show(Program.form, errorMessage, "Unable to connect to Remote Server");
        }

        public async void cleanupActiveDownloadStatus()
        {
            speedLabel.Text = "";
            etaLabel.Text = "";
            progressBar.Value = 0;
            gamesQueueList.RemoveAt(0);
        }

        public bool isinstalling = false;
        public static bool removedownloading = false;
        public async void downloadInstallGameButton_Click(object sender, EventArgs e)
        {
            {
                if (!Properties.Settings.Default.customDownloadDir)
                {
                    Properties.Settings.Default.downloadDir = Environment.CurrentDirectory.ToString();
                }
                bool obbsMismatch = false;
                if (nodeviceonstart && !updatesnotified)
                {
                    _ = await CheckForDevice();
                    ChangeTitlebarToDevice();
                    showAvailableSpace();
                    listappsbtn();
                }
                progressBar.Style = ProgressBarStyle.Marquee;
                if (gamesListView.SelectedItems.Count == 0)
                {
                    progressBar.Style = ProgressBarStyle.Continuous;
                    ChangeTitle("You must select a game from the Game List!");
                    return;
                }
                string namebox = gamesListView.SelectedItems[0].ToString();
                string nameboxtranslated = Sideloader.gameNameToSimpleName(namebox);
                int count = 0;
                string[] gamesToDownload;
                if (gamesListView.SelectedItems.Count > 0)
                {
                    count = gamesListView.SelectedItems.Count;
                    gamesToDownload = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        gamesToDownload[i] = gamesListView.SelectedItems[i].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                    }
                }
                else
                {
                    return;
                }

                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Continuous;
                string game = gamesToDownload.Length == 1 ? $"\"{gamesToDownload[0]}\"" : "the selected games";
                isinstalling = true;
                //Add games to the queue
                for (int i = 0; i < gamesToDownload.Length; i++)
                {
                    gamesQueueList.Add(gamesToDownload[i]);
                }

                if (gamesAreDownloading)
                {
                    return;
                }

                gamesAreDownloading = true;


                //Do user json on firsttime
                if (Properties.Settings.Default.userJsonOnGameInstall)
                {
                    Thread userJsonThread = new Thread(() => { ChangeTitle("Pushing user.json"); Sideloader.PushUserJsons(); })
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
                    string dir = Path.GetDirectoryName(gameName);
                    string gameDirectory = Properties.Settings.Default.downloadDir + "\\" + gameName;
                    string path = gameDirectory;

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
                    if (Properties.Settings.Default.singleThreadMode)
                    {
                        extraArgs = "--transfers 1 --multi-thread-streams 0";
                    }
                    if (hasPublicConfig)
                    {
                        bool doDownload = true;
                        if (Directory.Exists(gameDirectory))
                        {
                            DialogResult res = FlexibleMessageBox.Show(
                                $"{gameName} exists in destination directory.\r\nWould you like to overwrite it?",
                                "Download again?", MessageBoxButtons.YesNo);

                            doDownload = res == DialogResult.Yes;

                            if (doDownload)
                            {
                                // only delete after extraction; allows for resume if the fetch fails midway.
                                if (Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameName}"))
                                {
                                    Directory.Delete($"{Properties.Settings.Default.downloadDir}\\{gameName}", true);
                                }
                            }
                        }

                        if (doDownload)
                        {
                            _ = Logger.Log($"rclone copy \"Public:{SideloaderRCLONE.RcloneGamesFolder}/{gameName}\"");
                            t1 = new Thread(() =>
                            {
                                string rclonecommand =
                                $"copy \":http:/{gameNameHash}/\" \"{Properties.Settings.Default.downloadDir}\\{gameNameHash}\" {extraArgs} --progress --rc";
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
                        _ = Logger.Log($"rclone copy \"{currentRemote}:{SideloaderRCLONE.RcloneGamesFolder}/{gameName}\"");
                        t1 = new Thread(() =>
                        {
                            gameDownloadOutput = RCLONE.runRcloneCommand_DownloadConfig($"copy \"{currentRemote}:{SideloaderRCLONE.RcloneGamesFolder}/{gameName}\" \"{Properties.Settings.Default.downloadDir}\\{gameName}\" {extraArgs} --progress --rc --retries 1 --low-level-retries 1");
                        });
                    }

                    t1.IsBackground = true;
                    t1.Start();

                    ChangeTitle("Downloading game " + gameName, false);

                    int i = 0;
                    //Download
                    while (t1.IsAlive)
                    {
                        try
                        {
                            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:5572/core/stats", null);
                            string foo = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine("RESP CONTENT " + foo);
                            dynamic results = JsonConvert.DeserializeObject<dynamic>(foo);

                            if (results["transferring"] != null)
                            {
                                long allSize = 0;
                                long downloaded = 0;

                                foreach (dynamic obj in results.transferring)
                                {
                                    allSize += obj["size"].ToObject<long>();
                                    downloaded += obj["bytes"].ToObject<long>();
                                }

                                float downloadSpeed = results.speed.ToObject<float>() / 1000000;
                                allSize /= 1000000;
                                downloaded /= 1000000;

                                Debug.WriteLine("Allsize: " + allSize + "\nDownloaded: " + downloaded + "\nValue: " + (downloaded / (double)allSize * 100));

                                progressBar.Style = ProgressBarStyle.Continuous;
                                progressBar.Value = Convert.ToInt32(downloaded / (double)allSize * 100);

                                i++;
                                if (i == 4)
                                {
                                    i = 0;
                                    float seconds = (allSize - downloaded) / downloadSpeed;
                                    TimeSpan time = TimeSpan.FromSeconds(seconds);
                                    etaLabel.Text = "ETA: " + time.ToString(@"hh\:mm\:ss") + " left";
                                }

                                speedLabel.Text = "DLS: " + string.Format("{0:0.00}", downloadSpeed) + " MB/s";
                            }
                        }
                        catch { }

                        await Task.Delay(100);

                    }

                    if (removedownloading)
                    {
                        ChangeTitle("Deleting game files", false);
                        try
                        {
                            cleanupActiveDownloadStatus();
                            if (hasPublicConfig)
                            {
                                if (Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameNameHash}"))
                                {
                                    Directory.Delete($"{Properties.Settings.Default.downloadDir}\\{gameNameHash}", true);
                                }

                                if (Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameName}"))
                                {
                                    Directory.Delete($"{Properties.Settings.Default.downloadDir}\\{gameName}", true);
                                }
                            }
                            else
                            {
                                Directory.Delete(Properties.Settings.Default.downloadDir + "\\" + gameName, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = FlexibleMessageBox.Show($"Error deleting game files: {ex.Message}");
                        }
                        ChangeTitle("");
                        break;
                    }
                    {
                        //Quota Errors
                        bool isinstalltxt = false;
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

                                cleanupActiveDownloadStatus();
                            }
                            else if (!gameDownloadOutput.Error.Contains("Serving remote control on http://127.0.0.1:5572/"))
                            {
                                otherError = true;

                                //Remove current game
                                cleanupActiveDownloadStatus();

                                _ = FlexibleMessageBox.Show($"Rclone error: {gameDownloadOutput.Error}");
                                output += new ProcessOutput("", "Download Failed");
                            }
                        }

                        if (hasPublicConfig && otherError == false && gameDownloadOutput.Output != "Download skipped.")
                        {
                            try
                            {
                                Thread extractionThread = new Thread(() =>
                                {
                                    ChangeTitle("Extracting " + gameName, false);
                                    Zip.ExtractFile($"{Properties.Settings.Default.downloadDir}\\{gameNameHash}\\{gameNameHash}.7z.001", $"{Properties.Settings.Default.downloadDir}", PublicConfigFile.Password);
                                    Program.form.ChangeTitle("");
                                })
                                {
                                    IsBackground = true
                                };
                                extractionThread.Start();

                                while (extractionThread.IsAlive)
                                {
                                    await Task.Delay(100);
                                }

                                if (Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameNameHash}"))
                                {
                                    Directory.Delete($"{Properties.Settings.Default.downloadDir}\\{gameNameHash}", true);
                                }
                            }
                            catch (Exception ex)
                            {
                                otherError = true;
                                _ = FlexibleMessageBox.Show($"7zip error: {ex.Message}");
                                output += new ProcessOutput("", "Extract Failed");
                            }
                        }

                        if (quotaError == false && otherError == false)
                        {
                            ADB.WakeDevice();
                            ADB.DeviceID = GetDeviceID();
                            quotaTries = 0;
                            progressBar.Value = 0;
                            progressBar.Style = ProgressBarStyle.Continuous;
                            ChangeTitle("Installing game apk " + gameName, false);
                            etaLabel.Text = "ETA: Wait for install...";
                            speedLabel.Text = "DLS: Finished";
                            if (File.Exists(Properties.Settings.Default.downloadDir + "\\" + gameName + "\\install.txt"))
                            {
                                isinstalltxt = true;
                            }

                            if (File.Exists(Properties.Settings.Default.downloadDir + "\\" + gameName + "\\Install.txt"))
                            {
                                isinstalltxt = true;
                            }

                            string[] files = Directory.GetFiles(Properties.Settings.Default.downloadDir + "\\" + gameName);

                            Debug.WriteLine("Game Folder is: " + Properties.Settings.Default.downloadDir + "\\" + gameName);
                            Debug.WriteLine("FILES IN GAME FOLDER: ");
                            foreach (string file in files)
                            {
                                Debug.WriteLine(file);
                                string extension = Path.GetExtension(file);
                                if (extension == ".txt")
                                {
                                    if (!Properties.Settings.Default.nodevicemode | !nodeviceonstart & DeviceConnected)
                                    {
                                        string fullname = Path.GetFileName(file);
                                        if (fullname.Equals("install.txt") || fullname.Equals("Install.txt"))
                                        {
                                            Thread installtxtThread = new Thread(() =>
                                            {
                                                output += Sideloader.RunADBCommandsFromFile(file);

                                                ChangeTitle(" \n\n");
                                            });

                                            installtxtThread.Start();
                                            while (installtxtThread.IsAlive)
                                            {
                                                await Task.Delay(100);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        output.Output = "All tasks finished. \n";
                                    }
                                }
                                if (!isinstalltxt)
                                {
                                    if (!Properties.Settings.Default.nodevicemode | !nodeviceonstart & DeviceConnected)
                                    {
                                        if (extension == ".apk")
                                        {
                                            CurrAPK = file;
                                            CurrPCKG = packagename;
                                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer
                                            {
                                                Interval = 150000 // 150 seconds to fail
                                            };
                                            t.Tick += new EventHandler(timer_Tick4);
                                            t.Start();
                                            Thread apkThread = new Thread(() =>
                                            {
                                                Program.form.ChangeTitle($"Sideloading apk...");
                                                output += ADB.Sideload(file, packagename);
                                            })
                                            {
                                                IsBackground = true
                                            };
                                            apkThread.Start();
                                            while (apkThread.IsAlive)
                                            {
                                                await Task.Delay(100);
                                            }

                                            t.Stop();
                                        }

                                        Debug.WriteLine(wrDelimiter);
                                        if (Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameName}\\{packagename}"))
                                        {
                                            if (!Properties.Settings.Default.nodevicemode | !nodeviceonstart & DeviceConnected)
                                            {
                                                deleteOBB(packagename);
                                                Thread obbThread = new Thread(() =>
                                                {
                                                    ChangeTitle($"Copying {packagename} obb to device...");
                                                    output += ADB.RunAdbCommandToString($"push \"{Properties.Settings.Default.downloadDir}\\{gameName}\\{packagename}\" \"/sdcard/Android/obb\"");
                                                    Program.form.ChangeTitle("");
                                                })
                                                {
                                                    IsBackground = true
                                                };
                                                obbThread.Start();

                                                while (obbThread.IsAlive)
                                                {
                                                    await Task.Delay(100);
                                                }
                                                if (!nodeviceonstart | DeviceConnected)
                                                {
                                                    if (!output.Output.Contains("offline"))
                                                    {
                                                        try
                                                        {
                                                            obbsMismatch = await compareOBBSizes(packagename, gameName, output);
                                                        }
                                                        catch (Exception ex) { _ = FlexibleMessageBox.Show($"Error comparing OBB sizes: {ex.Message}"); }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        output.Output = "All tasks finished. \n";
                                    }
                                }
                                ChangeTitle($"Installation of {gameName} completed.");
                            }
                            if (Properties.Settings.Default.deleteAllAfterInstall)
                            {
                                ChangeTitle("Deleting game files", false);
                                try { Directory.Delete(Properties.Settings.Default.downloadDir + "\\" + gameName, true); } catch (Exception ex) { _ = FlexibleMessageBox.Show($"Error deleting game files: {ex.Message}"); }
                            }

                            //Remove current game
                            cleanupActiveDownloadStatus();
                        }
                    }
                }
                if (removedownloading)
                {
                    removedownloading = false;
                    gamesAreDownloading = false;
                    isinstalling = false;
                    return;
                }
                if (!obbsMismatch)
                {
                    ChangeTitle("Refreshing games list, please wait...         \n");
                    showAvailableSpace();
                    listappsbtn();
                    if (!updateAvailableClicked && !upToDate_Clicked && !NeedsDonation_Clicked && !Properties.Settings.Default.nodevicemode && !gamesQueueList.Any())
                    {
                        initListView();
                    }
                    ShowPrcOutput(output);
                    progressBar.Style = ProgressBarStyle.Continuous;
                    etaLabel.Text = "ETA: Finished Queue";
                    speedLabel.Text = "DLS: Finished Queue";
                    ProgressText.Text = "";
                    gamesAreDownloading = false;
                    isinstalling = false;

                    ChangeTitle(" \n\n");
                }
            }
        }

        private void deleteOBB(string packagename)
        {
            ADB.WakeDevice();
            ChangeTitle("Deleting old OBB Folder...");
            Logger.Log("Attempting to delete old OBB Folder");
            ADB.WakeDevice();
            ADB.RunAdbCommandToString($"shell rm -rf /sdcard/Android/obb/{packagename}");
        }

        private async Task<bool> compareOBBSizes(string packagename, string gameName, ProcessOutput output)
        {
            if (!Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameName}\\{packagename}"))
            {
                return false;
            }
            try
            {
                ChangeTitle("Comparing obbs...");
                Logger.Log("Comparing OBBs");
                ADB.WakeDevice();
                DirectoryInfo localFolder = new DirectoryInfo($"{Properties.Settings.Default.downloadDir}/{gameName}/{packagename}/");
                long totalLocalFolderSize = localFolderSize(localFolder) / (1024 * 1024);
                string totalRemoteFolderSize = ADB.RunAdbCommandToString($"shell du -m /sdcard/Android/obb/{packagename}").Output;
                string firstreplacedtotalRemoteFolderSize = Regex.Replace(totalRemoteFolderSize, "[^c]*$", "");
                string secondreplacedtotalRemoteFolderSize = Regex.Replace(firstreplacedtotalRemoteFolderSize, "[^0-9]", "");
                int localOBB = (int)totalLocalFolderSize;
                int remoteOBB = Convert.ToInt32(secondreplacedtotalRemoteFolderSize);
                Logger.Log("Total local folder size in bytes: " + totalLocalFolderSize + " Remote Size: " + secondreplacedtotalRemoteFolderSize);
                if (remoteOBB < localOBB)
                {
                    DialogResult om = MessageBox.Show("Warning! It seems like the OBB wasnt pushed correctly, this means that the game may not launch correctly.\n Do you want to retry the push?", "OBB Size Mismatch!", MessageBoxButtons.YesNo);
                    if (om == DialogResult.Yes)
                    {
                        ChangeTitle("Retrying push");
                        if (Directory.Exists($"{Properties.Settings.Default.downloadDir}\\{gameName}\\{packagename}"))
                        {
                            Thread obbThread = new Thread(() =>
                            {
                                ChangeTitle($"Copying {packagename} obb to device...");
                                output += ADB.RunAdbCommandToString($"push \"{Properties.Settings.Default.downloadDir}\\{gameName}\\{packagename}\" \"/sdcard/Android/obb\"");
                                Program.form.ChangeTitle("");
                            })
                            {
                                IsBackground = true
                            };
                            obbThread.Start();

                            while (obbThread.IsAlive)
                            {
                                await Task.Delay(100);
                            }
                            await compareOBBSizes(packagename, gameName, output);
                            return true;
                        }
                    }
                    else
                    {
                        ChangeTitle("Refreshing games list, please wait...         \n");
                        showAvailableSpace();
                        listappsbtn();
                        if (!updateAvailableClicked && !upToDate_Clicked && !NeedsDonation_Clicked && !Properties.Settings.Default.nodevicemode && !gamesQueueList.Any())
                        {
                            initListView();
                        }
                        ShowPrcOutput(output);
                        progressBar.Style = ProgressBarStyle.Continuous;
                        etaLabel.Text = "ETA: Finished Queue";
                        speedLabel.Text = "DLS: Finished Queue";
                        ProgressText.Text = "";
                        gamesAreDownloading = false;
                        isinstalling = false;
                        ChangeTitle(" \n\n");
                        return await Task.FromResult(true);
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                string inputstringerror = "Input string";
                if (ex.Message.Contains(inputstringerror))
                {
                    _ = FlexibleMessageBox.Show("The OBB Folder on the Quest seems to not exist or be empty\nPlease redownload the game or sideload the obb manually.", "OBB Size Undetectable!", MessageBoxButtons.OK);
                    return false;
                }
                else
                {
                    _ = Logger.Log("Unable to compare obbs with the exception" + ex.Message, "ERROR");
                    _ = FlexibleMessageBox.Show($"Error comparing OBB sizes: {ex.Message}");
                    return false;
                }
            }
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
                if (Properties.Settings.Default.InstalledApps.Contains(CurrPCKG))
                {
                    isinstalled = true;
                }
                if (isinstalled)
                {
                    if (!Properties.Settings.Default.AutoReinstall)
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
                    ChangeTitle("Performing reinstall, please wait...");
                    _ = ADB.RunAdbCommandToString("kill-server");
                    _ = ADB.RunAdbCommandToString("devices");
                    _ = ADB.RunAdbCommandToString($"pull /sdcard/Android/data/{CurrPCKG} \"{Environment.CurrentDirectory}\"");
                    _ = Sideloader.UninstallGame(CurrPCKG);
                    ChangeTitle("Reinstalling Game");
                    _ = ADB.RunAdbCommandToString($"install -g \"{CurrAPK}\"");
                    _ = ADB.RunAdbCommandToString($"push \"{Environment.CurrentDirectory}\\{CurrPCKG}\" /sdcard/Android/data/");

                    timerticked = false;
                    if (Directory.Exists($"{Environment.CurrentDirectory}\\{CurrPCKG}"))
                    {
                        Directory.Delete($"{Environment.CurrentDirectory}\\{CurrPCKG}", true);
                    }

                    ChangeTitle(" \n\n");
                    return;
                }
                else
                {
                    DialogResult dialogResult2 = FlexibleMessageBox.Show(Program.form, "This install is taking an unusual amount of time, you can keep waiting or cancel the install.\n" +
                        "Would you like to cancel the installation?", "Cancel install?", MessageBoxButtons.YesNo);
                    if (dialogResult2 == DialogResult.Yes)
                    {
                        ChangeTitle("Stopping Install...");
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
            if (isinstalling)
            {
                DialogResult res1 = FlexibleMessageBox.Show(Program.form, "There are downloads and/or installations in progress,\nif you exit now you'll have to start the entire process over again.\nAre you sure you want to exit?", "Still downloading/installing.",
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

        private void disPosed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ADBWirelessDisable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "Are you sure you want to delete your saved Quest IP address/command?", "Remove saved IP address?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                _ = FlexibleMessageBox.Show(Program.form, "Saved IP data reset cancelled.");
                return;
            }
            else
            {
                ADB.wirelessadbON = false;
                _ = FlexibleMessageBox.Show(Program.form, "Make sure your device is not connected to USB and press OK.");
                _ = ADB.RunAdbCommandToString("devices");
                _ = ADB.RunAdbCommandToString("shell USB");
                Thread.Sleep(2000);
                _ = ADB.RunAdbCommandToString("disconnect");
                Thread.Sleep(2000);
                _ = ADB.RunAdbCommandToString("kill-server");
                Thread.Sleep(2000);
                _ = ADB.RunAdbCommandToString("start-server");
                Properties.Settings.Default.IPAddress = "";
                Properties.Settings.Default.Save();
                _ = Program.form.GetDeviceID();
                Program.form.ChangeTitlebarToDevice();
                _ = FlexibleMessageBox.Show(Program.form, "Relaunch Rookie to complete the process and switch back to USB adb.");
                if (File.Exists("C:\\RSL\\platform-tools\\StoredIP.txt"))
                {
                    File.Delete("C:\\RSL\\platform-tools\\StoredIP.txt");
                }
            }
        }
        private void EnablePassthroughAPI_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            _ = ADB.RunAdbCommandToString("shell setprop debug.oculus.experimentalEnabled 1");
            _ = FlexibleMessageBox.Show(Program.form, "Passthrough API enabled.");

        }

        private async void killRcloneButton_Click(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }

            RCLONE.killRclone();
            ADBWirelessDisable.Text = "Start Movie Stream";
            ChangeTitle("Killed Rclone");
            _ = await CheckForDevice();
            ChangeTitlebarToDevice();
        }

        private void otherDrop_Click(object sender, EventArgs e)
        {
            ShowSubMenu(otherContainer);
            otherDrop.Text = (otherDrop.Text == "▼ OTHER ▼") ? "▶ OTHER ◀" : "▼ OTHER ▼";
        }

        private void gamesQueListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (gamesQueListBox.SelectedIndex == 0 && gamesQueueList.Count == 1)
            {
                removedownloading = true;
                RCLONE.killRclone();
            }
            if (gamesQueListBox.SelectedIndex != -1 && gamesQueListBox.SelectedIndex != 0)
            {
                _ = gamesQueueList.Remove(gamesQueListBox.SelectedItem.ToString());
            }

        }

        private void devicesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            showAvailableSpace();
        }

        private void remotesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (remotesList.SelectedItem != null)
            {
                remotesList.Invoke(() => { currentRemote = "VRP-mirror" + remotesList.SelectedItem.ToString(); });
            }
        }

        private void QuestOptionsButton_Click(object sender, EventArgs e)
        {
            QuestForm Form = new QuestForm();
            Form.Show(Program.form);
        }

        private void SpoofFormButton_Click(object sender, EventArgs e)
        {
            SpoofForm Form = new SpoofForm();
            Form.Show();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                lvwColumnSorter.Order = lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = e.Column == 4 ? SortOrder.Descending : SortOrder.Ascending;
            }
            // Perform the sort with these new sort options.
            gamesListView.Sort();
        }

        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (searchTextBox.Visible)
                {
                    if (Properties.Settings.Default.EnterKeyInstall)
                    {
                        if (gamesListView.SelectedItems.Count > 0)
                        {
                            downloadInstallGameButton_Click(sender, e);
                        }
                    }
                }
                searchTextBox.Visible = false;
                label2.Visible = false;
                lblSearchHelp.Visible = false;
                lblShortcutsF2.Visible = false;

                if (ADBcommandbox.Visible)
                {
                    ChangeTitle($"Entered command: ADB {ADBcommandbox.Text}");
                    _ = ADB.RunAdbCommandToString(ADBcommandbox.Text);
                    ChangeTitle(" \n\n");
                }
                ADBcommandbox.Visible = false;
                lblAdbCommand.Visible = false;
                lblShortcutCtrlR.Visible = false;
                label2.Visible = false;

            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                searchTextBox.Visible = false;
                label2.Visible = false;
                lblSearchHelp.Visible = false;
                lblShortcutsF2.Visible = false;
                ADBcommandbox.Visible = false;
                lblAdbCommand.Visible = false;
                lblShortcutCtrlR.Visible = false;
                label2.Visible = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                //show search
                searchTextBox.Clear();
                searchTextBox.Visible = true;
                label2.Visible = true;
                lblSearchHelp.Visible = true;
                lblShortcutsF2.Visible = true;
                _ = searchTextBox.Focus();
            }
            if (keyData == (Keys.Control | Keys.L))
            {
                if (loaded)
                {
                    Clipboard.SetText(rookienamelist);
                    _ = MessageBox.Show("Entire game list copied as a line by line list to clipboard!\nPress CTRL+V to paste it anywhere!");
                }

            }
            if (keyData == (Keys.Alt | Keys.L))
            {
                if (loaded)
                {
                    Clipboard.SetText(rookienamelist2);
                    _ = MessageBox.Show("Entire game list copied as a paragraph to clipboard!\nPress CTRL+V to paste it anywhere!");
                }

            }
            if (keyData == (Keys.Control | Keys.H))
            {
                string HWID = SideloaderUtilities.UUID();
                Clipboard.SetText(HWID);
                _ = FlexibleMessageBox.Show(Program.form, $"Your unique HWID is:\n\n{HWID}\n\nThis has been automatically copied to your clipboard. Press CTRL+V in a message to send it.");
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                ADBcommandbox.Visible = true;
                ADBcommandbox.Clear();
                lblAdbCommand.Visible = true;
                lblShortcutCtrlR.Visible = true;
                label2.Visible = true;
                _ = ADBcommandbox.Focus();
            }
            if (keyData == Keys.F2)
            {
                searchTextBox.Clear();
                searchTextBox.Visible = true;
                label2.Visible = true;
                lblSearchHelp.Visible = true;
                lblShortcutsF2.Visible = true;
                _ = searchTextBox.Focus();
            }
            if (keyData == (Keys.Control | Keys.F4))
            {
                try
                {
                    //run the program again and close this one
                    _ = Process.Start(Application.StartupPath + "\\Sideloader Launcher.exe");
                    //or you can use Application.ExecutablePath

                    //close this one
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
                ADB.WakeDevice();
                _ = GetDeviceID();
                _ = FlexibleMessageBox.Show(Program.form, "If your device is not Connected, hit reconnect first or it won't work!\nNOTE: THIS MAY TAKE UP TO 60 SECONDS.\nThere will be a Popup text window with all updates available when it is done!", "Is device connected?", MessageBoxButtons.OKCancel);
                listappsbtn();
                initListView();
            }
            bool dialogisup = false;
            if (keyData == Keys.F1 && !dialogisup)
            {
                _ = FlexibleMessageBox.Show(Program.form, "Shortcuts:\nF1 -------- Shortcuts List\nF2 --OR-- CTRL+F: QuickSearch\nF3 -------- Quest Options\nF4 -------- Rookie Settings\nF5 -------- Refresh Gameslist\n\nCTRL+R - Run custom ADB command.\nCTRL+L - Copy entire list of Game Names to clipboard seperated by new lines.\nALT+L - Copy entire list of Game Names to clipboard seperated by commas(in a paragraph).CTRL+P - Copy packagename to clipboard on game select.\nCTRL + F4 - Instantly relaunch Rookie Sideloader.");
            }
            if (keyData == (Keys.Control | Keys.P))
            {
                DialogResult dialogResult = FlexibleMessageBox.Show(Program.form, "Do you wish to copy Package Name of games selected from list to clipboard?", "Copy package to clipboard?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Properties.Settings.Default.PackageNameToCB = true;
                    Properties.Settings.Default.Save();
                }
                if (dialogResult == DialogResult.No)
                {
                    Properties.Settings.Default.PackageNameToCB = false;
                    Properties.Settings.Default.Save();
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            gamesListView.SelectedItems.Clear();
            searchTextBox.KeyPress += new
            System.Windows.Forms.KeyPressEventHandler(CheckEnter);
            if (gamesListView.Items.Count > 0)
            {
                ListViewItem foundItem = gamesListView.Items.Cast<ListViewItem>()
                .FirstOrDefault(i => i.Text.IndexOf(searchTextBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);
                if (foundItem != null)
                {
                    foundItem.Selected = true;
                    gamesListView.TopItem = foundItem;
                    if (foundItem == gamesListView.TopItem)
                    {
                        gamesListView.TopItem.Selected = true;

                    }
                    else
                    {
                        foundItem.Selected = true;
                    }

                    _ = searchTextBox.Focus();
                }
            }
        }

        private void ADBcommandbox_Enter(object sender, EventArgs e)
        {
            _ = ADBcommandbox.Focus();


        }

        private bool fullScreen = false;
        [DefaultValue(false)]
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                fullScreen = value;
                if (value)
                {
                    MainForm.ActiveForm.FormBorderStyle = FormBorderStyle.None;
                    webView21.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                    webView21.Location = new Point(0, 0);
                    webView21.Size = MainForm.ActiveForm.Size;
                }
                else
                {
                    MainForm.ActiveForm.FormBorderStyle = FormBorderStyle.Sizable;
                    webView21.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom);
                    webView21.Location = gamesPictureBox.Location;
                    webView21.Size = new Size(374, 214);
                }
            }
        }

        static string ExtractVideoUrl(string html)
        {
            Match match = Regex.Match(html, @"url""\:\""/watch\?v\=(.*?(?=""))");
            if (!match.Success)
            {
                return "";
            }

            string url = match.Groups[1].Value;
            return $"https://www.youtube.com/embed/{url}?autoplay=1&mute=1&enablejsapi=1&modestbranding=1";
        }

        private async Task CreateEnviroment()
        {
            string appDataLocation = @"C:\RSL\";
            var webView2Environment = await CoreWebView2Environment.CreateAsync(userDataFolder: appDataLocation);
            await webView21.EnsureCoreWebView2Async(webView2Environment);
        }

        private async Task WebView_CoreWebView2ReadyAsync(string videoUrl)
        {
            try
            {
                // Load the video URL in the web browser control
                webView21.CoreWebView2.Navigate(videoUrl);
                webView21.CoreWebView2.ContainsFullScreenElementChanged += (obj, args) =>
                {
                    this.FullScreen = webView21.CoreWebView2.ContainsFullScreenElement;
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public async void gamesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gamesListView.SelectedItems.Count < 1)
            {
                return;
            }
            string CurrentPackageName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.PackageNameIndex].Text;
            string CurrentReleaseName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            string CurrentGameName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.GameNameIndex].Text;
            Console.WriteLine(CurrentGameName);

            if (!Properties.Settings.Default.TrailersOn)
            {
                webView21.Hide();
                if (!keyheld)
                {
                    if (Properties.Settings.Default.PackageNameToCB)
                    {
                        Clipboard.SetText(CurrentPackageName);
                    }

                    keyheld = true;
                }

                string[] imageExtensions = { ".jpg", ".png" };
                string ImagePath = "";

                foreach (string extension in imageExtensions)
                {
                    string path = Path.Combine(SideloaderRCLONE.ThumbnailsFolder, $"{CurrentPackageName}{extension}");
                    if (File.Exists(path))
                    {
                        ImagePath = path;
                        break;
                    }
                }

                if (gamesPictureBox.BackgroundImage != null)
                {
                    gamesPictureBox.BackgroundImage.Dispose();
                }

                gamesPictureBox.BackgroundImage = File.Exists(ImagePath) ? Image.FromFile(ImagePath) : new Bitmap(367, 214);

                string NotePath = $"{SideloaderRCLONE.NotesFolder}\\{CurrentReleaseName}.txt";
                notesRichTextBox.Text = File.Exists(NotePath) ? File.ReadAllText(NotePath) : "";
            }
            else
            {
                if (!Directory.Exists(Environment.CurrentDirectory + "\\runtimes"))
                {
                    WebClient client = new WebClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    try
                    {
                        client.DownloadFile("https://wiki.vrpirates.club/downloads/runtimes.7z", "runtimes.7z");
                        Utilities.Zip.ExtractFile(Environment.CurrentDirectory + "\\runtimes.7z", Environment.CurrentDirectory);
                        File.Delete("runtimes.7z");
                    }
                    catch (Exception ex)
                    {
                        _ = FlexibleMessageBox.Show($"You are unable to access the wiki page with the Exception: {ex.Message}\n");
                        _ = FlexibleMessageBox.Show("Required files for the Trailers were unable to be downloaded, please use Thumbnails instead");
                        enviromentCreated = true;
                        webView21.Hide();
                    }
                }
                if (!enviromentCreated)
                {
                    await CreateEnviroment();
                    enviromentCreated = true;
                }
                webView21.Show();
                string query = $"{CurrentGameName} VR trailer"; // Create the search query by appending " VR trailer" to the current game name
                string encodedQuery = WebUtility.UrlEncode(query);
                string url = $"https://www.youtube.com/results?search_query={encodedQuery}";

                string videoUrl; 
                using (var client = new WebClient()) // Create a WebClient to download the search results page HTML
                {
                    videoUrl = ExtractVideoUrl(client.DownloadString(url)); // Download the HTML and extract the first video URL
                }
                if (videoUrl == "")
                {
                    MessageBox.Show("No video URL found in search results.");
                    return;
                }

                await WebView_CoreWebView2ReadyAsync(videoUrl);
            }
        }

        public void UpdateGamesButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            _ = GetDeviceID();
            _ = FlexibleMessageBox.Show(Program.form, "If your device is not Connected, hit reconnect first or it won't work!\nNOTE: THIS MAY TAKE UP TO 60 SECONDS.\nThere will be a Popup text window with all updates available when it is done!", "Is device connected?", MessageBoxButtons.OKCancel);
            listappsbtn();
            initListView();

            if (SideloaderRCLONE.games.Count < 1)
            {
                _ = FlexibleMessageBox.Show(Program.form, "There are no games in rclone, please check your internet connection and check if the config is working properly");
                return;
            }

            // if (gamesToUpdate.Length > 0)
            //     FlexibleMessageBox.Show(Program.form, gamesToUpdate);
            //  else
            //     FlexibleMessageBox.Show(Program.form, "All your games are up to date!");
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
            ADB.WakeDevice();

            _ = ADB.RunAdbCommandToString("shell svc usb setFunctions mtp true");
        }

        private void freeDisclaimer_Click(object sender, EventArgs e)
        {
            _ = Process.Start("https://github.com/nerdunit/androidsideloader");
        }

        private async void removeQUSetting_Click(object sender, EventArgs e)
        {

            if (m_combo.SelectedIndex == -1)
            {
                _ = FlexibleMessageBox.Show(Program.form, "Please select an app first");
                return;
            }
            ADB.WakeDevice();
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.Style = ProgressBarStyle.Marquee;

            string GameName = m_combo.SelectedItem.ToString();

            Thread t1 = new Thread(() =>
            {
                output += Sideloader.DeleteFile(GameName);
            });
            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            showAvailableSpace();

            progressBar.Style = ProgressBarStyle.Continuous;
            m_combo.Items.RemoveAt(m_combo.SelectedIndex);
            ShowPrcOutput(output);
            listappsbtn();
            initListView();
        }

        private async void InstallQUset_Click(object sender, EventArgs e)
        {

            if (m_combo.SelectedIndex == -1)
            {
                _ = FlexibleMessageBox.Show(Program.form, "Please select an app first");
                return;
            }
            ADB.WakeDevice();
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.Style = ProgressBarStyle.Marquee;

            string GameName = m_combo.SelectedItem.ToString();
            string pckg = Sideloader.gameNameToPackageName(GameName);
            Thread t1 = new Thread(() =>
            {
                _ = ADB.RunAdbCommandToString($"shell mkdir sdcard/android/data/{pckg}");
                _ = ADB.RunAdbCommandToString($"shell mkdir sdcard/android/data/{pckg}/private");
                Random r = new Random();

                int x = r.Next(999999999);
                int y = r.Next(9999999);

                long sum = (y * (long)1000000000) + x;

                int x2 = r.Next(999999999);
                int y2 = r.Next(9999999);

                long sum2 = (y2 * (long)1000000000) + x2;

                Properties.Settings.Default.QUStringF = $"{{\"user_id\":{sum},\"app_id\":\"{sum2}\",";
                Properties.Settings.Default.Save();
                File.WriteAllText("delete_settings", "");
                string boff = Properties.Settings.Default.QUStringF + Properties.Settings.Default.QUString;
                File.WriteAllText("config.json", boff);
                output += ADB.RunAdbCommandToString($"push \"{Properties.Settings.Default.MainDir}\\delete_settings\" /sdcard/android/data/{pckg}/private/delete_settings");
                output += ADB.RunAdbCommandToString($"push \"{Environment.CurrentDirectory}\\config.json\" /sdcard/android/data/{pckg}/private/");

            });
            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            showAvailableSpace();

            progressBar.Style = ProgressBarStyle.Continuous;
            m_combo.Items.RemoveAt(m_combo.SelectedIndex);
            ShowPrcOutput(output);
            listappsbtn();
            initListView();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            searchTextBox.Clear();
            searchTextBox.Visible = true;
            label2.Visible = true;
            lblSearchHelp.Visible = true;
            lblShortcutsF2.Visible = true;
            _ = searchTextBox.Focus();
        }
        private void EnterInstallBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnterKeyInstall = EnterInstallBox.Checked;
        }
        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            if (searchTextBox.Visible)
            {
                searchTextBox.Visible = false;
                label2.Visible = false;
                lblSearchHelp.Visible = false;
                lblShortcutsF2.Visible = false;
            }
            else
            {
                _ = gamesListView.Focus();
            }
        }

        private void gamesListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (Properties.Settings.Default.EnterKeyInstall)
                {
                    if (gamesListView.SelectedItems.Count > 0)
                    {
                        downloadInstallGameButton_Click(sender, e);
                    }
                }
            }
        }

        bool updateAvailableClicked = false;
        private async void updateAvailable_Click(object sender, EventArgs e)
        {
            lblUpToDate.Click -= lblUpToDate_Click;
            lblUpdateAvailable.Click -= updateAvailable_Click;
            lblNeedsDonate.Click -= lblNeedsDonate_Click;
            ChangeTitle("Filtering Game List... This may take a few seconds...  \n\n");
            if (upToDate_Clicked || NeedsDonation_Clicked)
            {
                upToDate_Clicked = false;
                NeedsDonation_Clicked = false;
                updateAvailableClicked = false;
            }
            if (!updateAvailableClicked)
            {
                updateAvailableClicked = true;
                rookienamelist = "";
                loaded = false;
                string lines = Properties.Settings.Default.InstalledApps;
                string pattern = "package:";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(lines, replacement);
                char[] delims = new[] { '\r', '\n' };
                string[] packageList = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                string[] blacklist = new string[] { };
                string[] whitelist = new string[] { };
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt"))
                {
                    blacklist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt");
                }
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt"))
                {
                    whitelist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt");
                }

                List<ListViewItem> GameList = new List<ListViewItem>();

                List<string> rookieList = new List<string>();
                List<string> installedGames = packageList.ToList();
                List<string> blacklistItems = blacklist.ToList();
                List<string> whitelistItems = whitelist.ToList();
                errorOnList = false;
                //This is for black list, but temporarly will be whitelist
                //this list has games that we are actually going to upload
                newGamesToUploadList = whitelistItems.Intersect(installedGames).ToList();
                progressBar.Style = ProgressBarStyle.Marquee;
                if (SideloaderRCLONE.games.Count > 5)
                {
                    Thread t1 = new Thread(() =>
                    {
                        foreach (string[] release in SideloaderRCLONE.games)
                        {
                            rookieList.Add(release[SideloaderRCLONE.PackageNameIndex].ToString());
                            if (!rookienamelist.Contains(release[SideloaderRCLONE.GameNameIndex].ToString()))
                            {
                                rookienamelist += release[SideloaderRCLONE.GameNameIndex].ToString() + "\n";
                                rookienamelist2 += release[SideloaderRCLONE.GameNameIndex].ToString() + ", ";
                            }

                            ListViewItem Game = new ListViewItem(release);

                            Color colorFont_installedGame = ColorTranslator.FromHtml("#3c91e6");
                            lblUpToDate.ForeColor = colorFont_installedGame;
                            Color colorFont_updateAvailable = ColorTranslator.FromHtml("#4daa57");
                            lblUpdateAvailable.ForeColor = colorFont_updateAvailable;
                            Color colorFont_donateGame = ColorTranslator.FromHtml("#cb9cf2");
                            lblNeedsDonate.ForeColor = colorFont_donateGame;
                            Color colorFont_error = ColorTranslator.FromHtml("#f52f57");

                            foreach (string packagename in packageList)
                            {
                                if (string.Equals(release[SideloaderRCLONE.PackageNameIndex], packagename))
                                {
                                    Game.ForeColor = colorFont_installedGame;

                                    string InstalledVersionCode;
                                    InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename} | grep versionCode -F\"").Output;
                                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                    try
                                    {
                                        ulong installedVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                        ulong cloudVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));

                                        _ = Logger.Log($"Checked game {release[SideloaderRCLONE.GameNameIndex]}; cloudversion={cloudVersionInt} localversion={installedVersionInt}");
                                        if (installedVersionInt < cloudVersionInt)
                                        {
                                            Game.ForeColor = colorFont_updateAvailable;
                                            GameList.Add(Game);
                                        }
                                        else
                                        {
                                            GameList.Remove(Game);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Game.ForeColor = colorFont_error;
                                        _ = Logger.Log($"An error occured while rendering game {release[SideloaderRCLONE.GameNameIndex]} in ListView", "ERROR");
                                        _ = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename}\"");
                                        _ = Logger.Log($"ExMsg: {ex.Message}Installed:\"{InstalledVersionCode}\" Cloud:\"{Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex])}\"", "ERROR");
                                    }
                                }
                            }
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
                progressBar.Style = ProgressBarStyle.Continuous;
                ListViewItem[] arr = GameList.ToArray();
                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(arr);
                gamesListView.EndUpdate();
                ChangeTitle("                                                \n\n");
                loaded = true;
            }
            else
            {
                updateAvailableClicked = false;
                initListView();
            }
            lblUpToDate.Click += lblUpToDate_Click;
            lblUpdateAvailable.Click += updateAvailable_Click;
            lblNeedsDonate.Click += lblNeedsDonate_Click;
        }

        private void EnterInstallBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnterKeyInstall = EnterInstallBox.Checked;
            Properties.Settings.Default.Save();
        }

        private async void ADBcommandbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            searchTextBox.KeyPress += new
            System.Windows.Forms.KeyPressEventHandler(CheckEnter);
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (manualIP)
                {
                    string IPaddr;
                    IPaddr = ADBcommandbox.Text;
                    string IPcmnd = "connect " + IPaddr + ":5555";
                    Thread.Sleep(1000);
                    string errorChecker = ADB.RunAdbCommandToString(IPcmnd).Output;
                    if (errorChecker.Contains("cannot resolve host") | errorChecker.Contains("cannot connect to"))
                    {
                        ChangeTitle("");
                        _ = FlexibleMessageBox.Show("Manual ADB over WiFi Connection failed\nExiting...", "Manual IP Connection Failed!", MessageBoxButtons.OK);
                        manualIP = false;
                        ADBcommandbox.Visible = false;
                        lblAdbCommand.Visible = false;
                        lblShortcutCtrlR.Visible = false;
                        label2.Visible = false;
                        lblAdbCommand.Text = "Type command without\r\n\"adb\" prefix.\r\n\r\n\r\n";
                        lblAdbCommand.Location = new System.Drawing.Point(514, 206);
                        _ = gamesListView.Focus();
                    }
                    else
                    {
                        _ = await Program.form.CheckForDevice();
                        Program.form.showAvailableSpace();
                        Properties.Settings.Default.IPAddress = IPcmnd;
                        Properties.Settings.Default.Save();
                        File.WriteAllText($"C:\\RSL\\platform-tools\\StoredIP.txt", IPcmnd);
                        ADB.wirelessadbON = true;
                        _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                        _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                        manualIP = false;
                        ADBcommandbox.Visible = false;
                        lblAdbCommand.Visible = false;
                        lblShortcutCtrlR.Visible = false;
                        label2.Visible = false;
                        lblAdbCommand.Text = "Type command without\r\n\"adb\" prefix.\r\n\r\n\r\n";
                        lblAdbCommand.Location = new System.Drawing.Point(514, 206);
                        ChangeTitle("");
                        Program.form.ChangeTitlebarToDevice();
                        _ = gamesListView.Focus();
                    }
                }
                else
                {
                    Program.form.ChangeTitle($"Running adb command: ADB {ADBcommandbox.Text}");
                    string output = ADB.RunAdbCommandToString(ADBcommandbox.Text).Output;
                    _ = FlexibleMessageBox.Show(Program.form, $"Ran adb command: ADB {ADBcommandbox.Text}, Output: {output}");
                    ADBcommandbox.Visible = false;
                    lblAdbCommand.Visible = false;
                    lblShortcutCtrlR.Visible = false;
                    label2.Visible = false;
                    _ = gamesListView.Focus();
                    Program.form.ChangeTitle("");
                }
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                ADBcommandbox.Visible = false;
                lblShortcutCtrlR.Visible = false;
                lblAdbCommand.Visible = false;
                label2.Visible = false;
                _ = gamesListView.Focus();
            }
        }

        private void ADBcommandbox_Leave(object sender, EventArgs e)
        {
            label2.Visible = false;
            ADBcommandbox.Visible = false;
            lblAdbCommand.Visible = false;
            lblShortcutCtrlR.Visible = false;
        }

        private void gamesQueListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (gamesQueListBox.SelectedItem == null)
            {
                return;
            }

            _ = gamesQueListBox.DoDragDrop(gamesQueListBox.SelectedItem, DragDropEffects.Move);
        }

        private void gamesQueListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private async void pullAppToDesktopBtn_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            if (m_combo.SelectedIndex == -1)
            {
                notify("Please select an app first");
                return;
            }
            DialogResult dialogResult1 = FlexibleMessageBox.Show(Program.form, $"Do you want to extract {m_combo.SelectedItem}'s apk and obb to a folder on your desktop now?", "Extract app?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (!isworking)
            {
                isworking = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                string HWID = SideloaderUtilities.UUID();
                string GameName = m_combo.SelectedItem.ToString();
                string packageName = Sideloader.gameNameToPackageName(GameName);
                string InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packageName} | grep versionCode -F\"").Output;
                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                ulong VersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                if (Directory.Exists($"{Properties.Settings.Default.MainDir}\\{packageName}"))
                {
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
                }

                ProcessOutput output = new ProcessOutput("", "");
                ChangeTitle("Extracting APK....");

                _ = Directory.CreateDirectory($"{Properties.Settings.Default.MainDir}\\{packageName}");

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

                ChangeTitle("Extracting obb if it exists....");
                Thread t2 = new Thread(() =>
                {
                    output += ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packageName}\" \"{Properties.Settings.Default.MainDir}\\{packageName}\"");
                })
                {
                    IsBackground = true
                };
                t2.Start();

                while (t2.IsAlive)
                {
                    await Task.Delay(100);
                }

                if (File.Exists($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip"))
                {
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");
                }

                string path = $"{Properties.Settings.Default.MainDir}\\7z.exe";
                string cmd = $"7z a -mx1 \"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip\" .\\{packageName}\\*";
                Program.form.ChangeTitle("Zipping extracted application...");
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

                File.Copy($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip", $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip");
                File.Delete($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");
                Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
                isworking = false;
                Program.form.ChangeTitle("                                   \n\n");
                progressBar.Style = ProgressBarStyle.Continuous;
                _ = FlexibleMessageBox.Show(Program.form, $"{GameName} pulled to:\n\n{GameName} v{VersionInt} {packageName}.zip\n\nOn your desktop!");
            }
        }

        bool upToDate_Clicked = false;
        private async void lblUpToDate_Click(object sender, EventArgs e)
        {
            lblUpToDate.Click -= lblUpToDate_Click;
            lblUpdateAvailable.Click -= updateAvailable_Click;
            lblNeedsDonate.Click -= lblNeedsDonate_Click;
            ChangeTitle("Filtering Game List... This may take a few seconds...  \n\n");
            if (updateAvailableClicked || NeedsDonation_Clicked)
            {
                updateAvailableClicked = false;
                NeedsDonation_Clicked = false;
                upToDate_Clicked = false;
            }
            if (!upToDate_Clicked)
            {
                upToDate_Clicked = true;
                rookienamelist = "";
                loaded = false;
                string lines = Properties.Settings.Default.InstalledApps;
                string pattern = "package:";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(lines, replacement);
                char[] delims = new[] { '\r', '\n' };
                string[] packageList = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                string[] blacklist = new string[] { };
                string[] whitelist = new string[] { };
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt"))
                {
                    blacklist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt");
                }
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt"))
                {
                    whitelist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt");
                }

                List<ListViewItem> GameList = new List<ListViewItem>();

                List<string> rookieList = new List<string>();
                List<string> installedGames = packageList.ToList();
                List<string> blacklistItems = blacklist.ToList();
                List<string> whitelistItems = whitelist.ToList();
                errorOnList = false;
                //This is for black list, but temporarly will be whitelist
                //this list has games that we are actually going to upload
                newGamesToUploadList = whitelistItems.Intersect(installedGames).ToList();
                progressBar.Style = ProgressBarStyle.Marquee;
                if (SideloaderRCLONE.games.Count > 5)
                {
                    Thread t1 = new Thread(() =>
                    {
                        foreach (string[] release in SideloaderRCLONE.games)
                        {
                            rookieList.Add(release[SideloaderRCLONE.PackageNameIndex].ToString());
                            if (!rookienamelist.Contains(release[SideloaderRCLONE.GameNameIndex].ToString()))
                            {
                                rookienamelist += release[SideloaderRCLONE.GameNameIndex].ToString() + "\n";
                                rookienamelist2 += release[SideloaderRCLONE.GameNameIndex].ToString() + ", ";
                            }

                            ListViewItem Game = new ListViewItem(release);

                            Color colorFont_installedGame = ColorTranslator.FromHtml("#3c91e6");
                            lblUpToDate.ForeColor = colorFont_installedGame;
                            Color colorFont_updateAvailable = ColorTranslator.FromHtml("#4daa57");
                            lblUpdateAvailable.ForeColor = colorFont_updateAvailable;
                            Color colorFont_donateGame = ColorTranslator.FromHtml("#cb9cf2");
                            lblNeedsDonate.ForeColor = colorFont_donateGame;
                            Color colorFont_error = ColorTranslator.FromHtml("#f52f57");

                            foreach (string packagename in packageList)
                            {
                                if (string.Equals(release[SideloaderRCLONE.PackageNameIndex], packagename))
                                {
                                    Game.ForeColor = colorFont_installedGame;

                                    string InstalledVersionCode;
                                    InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename} | grep versionCode -F\"").Output;
                                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                    try
                                    {
                                        ulong installedVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                        ulong cloudVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));

                                        _ = Logger.Log($"Checked game {release[SideloaderRCLONE.GameNameIndex]}; cloudversion={cloudVersionInt} localversion={installedVersionInt}");
                                        if (installedVersionInt == cloudVersionInt)
                                        {
                                            Game.ForeColor = colorFont_installedGame;
                                            GameList.Add(Game);
                                        }
                                        else
                                        {
                                            GameList.Remove(Game);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Game.ForeColor = colorFont_error;
                                        _ = Logger.Log($"An error occured while rendering game {release[SideloaderRCLONE.GameNameIndex]} in ListView", "ERROR");
                                        _ = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename}\"");
                                        _ = Logger.Log($"ExMsg: {ex.Message}Installed:\"{InstalledVersionCode}\" Cloud:\"{Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex])}\"", "ERROR");
                                    }
                                }
                            }
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
                progressBar.Style = ProgressBarStyle.Continuous;
                ListViewItem[] arr = GameList.ToArray();
                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(arr);
                gamesListView.EndUpdate();
                ChangeTitle("                                                \n\n");
                loaded = true;
            }
            else
            {
                upToDate_Clicked = false;
                initListView();
            }   
            lblUpToDate.Click += lblUpToDate_Click;
            lblUpdateAvailable.Click += updateAvailable_Click;
            lblNeedsDonate.Click += lblNeedsDonate_Click;
        }

        bool NeedsDonation_Clicked = false;
        private async void lblNeedsDonate_Click(object sender, EventArgs e)
        {
            lblUpToDate.Click -= lblUpToDate_Click;
            lblUpdateAvailable.Click -= updateAvailable_Click;
            lblNeedsDonate.Click -= lblNeedsDonate_Click;
            ChangeTitle("Filtering Game List... This may take a few seconds...  \n\n");
            if (updateAvailableClicked || upToDate_Clicked)
            {
                updateAvailableClicked = false;
                upToDate_Clicked = false;
                NeedsDonation_Clicked = false;
            }
            if (!NeedsDonation_Clicked)
            {
                NeedsDonation_Clicked = true;
                rookienamelist = "";
                loaded = false;
                string lines = Properties.Settings.Default.InstalledApps;
                string pattern = "package:";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(lines, replacement);
                char[] delims = new[] { '\r', '\n' };
                string[] packageList = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                string[] blacklist = new string[] { };
                string[] whitelist = new string[] { };
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt"))
                {
                    blacklist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\blacklist.txt");
                }
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt"))
                {
                    whitelist = File.ReadAllLines($"{Properties.Settings.Default.MainDir}\\nouns\\whitelist.txt");
                }

                List<ListViewItem> GameList = new List<ListViewItem>();

                List<string> rookieList = new List<string>();
                List<string> installedGames = packageList.ToList();
                List<string> blacklistItems = blacklist.ToList();
                List<string> whitelistItems = whitelist.ToList();
                errorOnList = false;
                //This is for black list, but temporarly will be whitelist
                //this list has games that we are actually going to upload
                newGamesToUploadList = whitelistItems.Intersect(installedGames).ToList();
                progressBar.Style = ProgressBarStyle.Marquee;
                if (SideloaderRCLONE.games.Count > 5)
                {
                    Thread t1 = new Thread(() =>
                    {
                        foreach (string[] release in SideloaderRCLONE.games)
                        {
                            rookieList.Add(release[SideloaderRCLONE.PackageNameIndex].ToString());
                            if (!rookienamelist.Contains(release[SideloaderRCLONE.GameNameIndex].ToString()))
                            {
                                rookienamelist += release[SideloaderRCLONE.GameNameIndex].ToString() + "\n";
                                rookienamelist2 += release[SideloaderRCLONE.GameNameIndex].ToString() + ", ";
                            }

                            ListViewItem Game = new ListViewItem(release);

                            Color colorFont_installedGame = ColorTranslator.FromHtml("#3c91e6");
                            lblUpToDate.ForeColor = colorFont_installedGame;
                            Color colorFont_updateAvailable = ColorTranslator.FromHtml("#4daa57");
                            lblUpdateAvailable.ForeColor = colorFont_updateAvailable;
                            Color colorFont_donateGame = ColorTranslator.FromHtml("#cb9cf2");
                            lblNeedsDonate.ForeColor = colorFont_donateGame;
                            Color colorFont_error = ColorTranslator.FromHtml("#f52f57");

                            foreach (string packagename in packageList)
                            {
                                if (string.Equals(release[SideloaderRCLONE.PackageNameIndex], packagename))
                                {
                                    Game.ForeColor = colorFont_installedGame;

                                    string InstalledVersionCode;
                                    InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename} | grep versionCode -F\"").Output;
                                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                    InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                    try
                                    {
                                        ulong installedVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                        ulong cloudVersionInt = ulong.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));

                                        _ = Logger.Log($"Checked game {release[SideloaderRCLONE.GameNameIndex]}; cloudversion={cloudVersionInt} localversion={installedVersionInt}");
                                        if (installedVersionInt > cloudVersionInt)
                                        {
                                            bool dontget = false;
                                            if (blacklist.Contains(packagename))
                                            {
                                                dontget = true;
                                            }

                                            if (!dontget)
                                            {
                                                Game.ForeColor = colorFont_donateGame;
                                                GameList.Add(Game);
                                            }
                                        }
                                        else
                                        {
                                            GameList.Remove(Game);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Game.ForeColor = colorFont_error;
                                        _ = Logger.Log($"An error occured while rendering game {release[SideloaderRCLONE.GameNameIndex]} in ListView", "ERROR");
                                        _ = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename}\"");
                                        _ = Logger.Log($"ExMsg: {ex.Message}Installed:\"{InstalledVersionCode}\" Cloud:\"{Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex])}\"", "ERROR");
                                    }
                                }
                            }
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
                progressBar.Style = ProgressBarStyle.Continuous;
                ListViewItem[] arr = GameList.ToArray();
                gamesListView.BeginUpdate();
                gamesListView.Items.Clear();
                gamesListView.Items.AddRange(arr);
                gamesListView.EndUpdate();
                ChangeTitle("                                                \n\n");
                loaded = true;
            }
            else
            {
                NeedsDonation_Clicked = false;
                initListView();
            }  
            lblUpToDate.Click += lblUpToDate_Click;
            lblUpdateAvailable.Click += updateAvailable_Click;
            lblNeedsDonate.Click += lblNeedsDonate_Click;
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
    }
}