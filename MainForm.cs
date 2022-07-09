using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using Newtonsoft.Json;
using SergeUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AndroidSideloader
{
    public partial class MainForm : Form
    {

        private ListViewColumnSorter lvwColumnSorter;

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
        List<UploadGame> gamesToUpload = new List<UploadGame>();
        public static bool debugMode = false;
        public bool DeviceConnected = false;


        public static string currremotesimple = "";

#endif

        private bool isLoading = true;
        public static bool isOffline = false;
        public MainForm()

        {
            // check for offline mode
            string[] args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg == "--offline")
                {
                    isOffline = true;
                    FlexibleMessageBox.Show("Offline mode activated. You can't download games in this mode, only do local stuff.");
                }
            }
            InitializeComponent();
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
            Logger.Log($"\n------\n------\nProgram Launched at: {launchtime}\n------\n------");
            if (String.IsNullOrEmpty(Properties.Settings.Default.CurrentLogPath))
            {
                if (File.Exists($"{Environment.CurrentDirectory}\\nouns\\nouns.txt"))
                {
                    string[] lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\nouns\\nouns.txt");
                    Random r = new Random();
                    int x = r.Next(6806);
                    int y = r.Next(6806);
                    string randomnoun = lines[new Random(x).Next(lines.Length)];
                    string randomnoun2 = lines[new Random(y).Next(lines.Length)];
                    string combined = randomnoun + "-" + randomnoun2;
                    Properties.Settings.Default.CurrentLogPath = Environment.CurrentDirectory + "\\" + combined + ".txt";
                    Properties.Settings.Default.CurrentLogName = combined;
                    if (File.Exists($"{Environment.CurrentDirectory}\\debuglog.txt"))
                        System.IO.File.Move("debuglog.txt", combined + ".txt");
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.CurrentLogPath = $"{Environment.CurrentDirectory}\\debuglog.txt";
                }

            }
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 840000; // 14 mins between wakeup commands
            t.Tick += new EventHandler(timer_Tick);
            t.Start();
            System.Windows.Forms.Timer t2 = new System.Windows.Forms.Timer();
            t2.Interval = 300; // 30ms
            t2.Tick += new EventHandler(timer_Tick2);
            t2.Start();

            lvwColumnSorter = new ListViewColumnSorter();
            this.gamesListView.ListViewItemSorter = lvwColumnSorter;
            if (searchTextBox.Visible)
                searchTextBox.Focus();
            if (Properties.Settings.Default.QblindOn)
                pictureBox3.Image = global::AndroidSideloader.Properties.Resources.redkey;
            else
                pictureBox3.Image = global::AndroidSideloader.Properties.Resources.orangekey;
            if (Properties.Settings.Default.QblindOn)
                pictureBox4.Image = global::AndroidSideloader.Properties.Resources.bluekey;
            else
                pictureBox4.Image = global::AndroidSideloader.Properties.Resources.greenkey;
        }

        public static string DonorApps = "";
        private string oldTitle = "";
        public static bool updatesnotified = false;

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("C:\\RSL\\platform-tools\\adb.exe"))
            {
                ADB.RunAdbCommandToString("kill-server");
                ADB.RunAdbCommandToString("start-server");
            }
            Properties.Settings.Default.MainDir = Environment.CurrentDirectory;
            Properties.Settings.Default.Save();
            CheckForInternet();
            Sideloader.downloadFiles();
            await Task.Delay(100);
            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);

            if (Directory.Exists(Sideloader.TempFolder))

            {
                Directory.Delete(Sideloader.TempFolder, true);
                Directory.CreateDirectory(Sideloader.TempFolder);
            }

            //Delete the Debug file if it is more than 5MB
            if (File.Exists($"{Properties.Settings.Default.CurrentLogPath}"))
            {
                long length = new System.IO.FileInfo(Properties.Settings.Default.CurrentLogPath).Length;
                if (length > 5000000)
                    File.Delete($"{Properties.Settings.Default.CurrentLogPath}");
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
            this.CenterToScreen();
            gamesListView.View = View.Details;
            gamesListView.FullRowSelect = true;
            gamesListView.GridLines = false;
            etaLabel.Text = "";
            speedLabel.Text = "";
            diskLabel.Text = "";
            if (File.Exists("crashlog.txt"))
            {
                if (File.Exists(Properties.Settings.Default.CurrentCrashPath))
                    File.Delete(Properties.Settings.Default.CurrentCrashPath);
                DialogResult dialogResult = FlexibleMessageBox.Show($"Sideloader crashed during your last use.\nPress OK if you'd like to send us your crash log.\n\n NOTE: THIS CAN TAKE UP TO 30 SECONDS.", "Crash Detected", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    if (File.Exists($"{Environment.CurrentDirectory}\\crashlog.txt") && File.Exists($"{Environment.CurrentDirectory}\\nouns\\nouns.txt"))
                    {
                        Random r = new Random();
                        int x = r.Next(6806);
                        int y = r.Next(6806);

                        string[] lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\nouns\\nouns.txt");
                        string randomnoun = lines[new Random(x).Next(lines.Length)];
                        string randomnoun2 = lines[new Random(y).Next(lines.Length)];
                        string combined = randomnoun + "-" + randomnoun2;

                        System.IO.File.Move("crashlog.txt", $"{Environment.CurrentDirectory}\\{combined}.txt");
                        Properties.Settings.Default.CurrentCrashPath = $"{Environment.CurrentDirectory}\\{combined}.txt";
                        Properties.Settings.Default.CurrentCrashName = combined;
                        Properties.Settings.Default.Save();

                        Clipboard.SetText(combined);
                        RCLONE.runRcloneCommand($"copy \"{Properties.Settings.Default.CurrentCrashPath}\" VRP-debuglogs:CrashLogs");
                        FlexibleMessageBox.Show($"Your CrashLog has been copied to the server. Please mention your CrashLogID ({Properties.Settings.Default.CurrentCrashName}) to the Mods (it has been automatically copied to your clipboard).");
                        Clipboard.SetText(Properties.Settings.Default.CurrentCrashName);


                    }
                }
                else
                {
                    File.Delete($"{Environment.CurrentDirectory}\\crashlog.txt");
                }
            }
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
                    ChangeTitle("Initializing Mirrors");
                    initMirrors(true);
                    ChangeTitle("Checking if config is updated and updating config");
                    SideloaderRCLONE.updateConfig(currentRemote);
                    ChangeTitle("Initializing Games");
                    SideloaderRCLONE.initGames(currentRemote);
                    //ChangeTitle("Syncing Game Photos");
                    //ChangeTitle("Updating list of needed clean apps...");
                    //ChangeTitle("Checking for Updates on server...");
                }
                else
                {
                    ChangeTitle("Offline mode enabled, no Rclone");
                }

            });
            t1.SetApartmentState(ApartmentState.STA);
            t1.IsBackground = true;
            if (HasInternet)
                t1.Start();
            while (t1.IsAlive)
                await Task.Delay(100);
            Thread t5 = new Thread(() =>
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.IPAddress))
                {
                    string path = "C:\\RSL\\platform-tools\\adb.exe";
                    var wakeywakey = ADB.RunCommandToString("C:\\RSL\\platform-tools\\adb.exe shell input keyevent KEYCODE_WAKEUP", path);
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
                    var IPoutput = ADB.RunAdbCommandToString(IPcmndfromtxt);
                    if (IPoutput.Output.Contains("attempt failed") || IPoutput.Output.Contains("refused"))
                    {
                        FlexibleMessageBox.Show("Attempt to connect to saved IP has failed. This is usually due to rebooting the device or not having a STATIC IP set in your router.\nYou must enable Wireless ADB again!");
                        Properties.Settings.Default.IPAddress = "";
                        Properties.Settings.Default.Save();
                        File.Delete("C:\\RSL\\platform-tools\\StoredIP.txt");
                    }
                    else
                    {
                        ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                        ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                    }
                }
                else if (!File.Exists(@"C:\RSL\platform-tools\StoredIP.txt"))
                {
                    Properties.Settings.Default.IPAddress = "";
                    Properties.Settings.Default.Save();
                }
            });
            t5.IsBackground = true;
            t5.Start();
            while (t5.IsAlive)
                await Task.Delay(100);
            Thread t2 = new Thread(() =>
            {
                SideloaderRCLONE.UpdateGameNotes(currentRemote);
            });

            Thread t3 = new Thread(() =>
            {
                SideloaderRCLONE.UpdateGamePhotos(currentRemote);
            });

            Thread t4 = new Thread(() =>
            {
                SideloaderRCLONE.UpdateNouns(currentRemote);
                if (!Directory.Exists(SideloaderRCLONE.ThumbnailsFolder) || !Directory.Exists(SideloaderRCLONE.NotesFolder))
                {
                    FlexibleMessageBox.Show("It seems you are missing the thumbnails and/or notes database, the first start of the sideloader takes a bit more time, so dont worry if it looks stuck!");
                }
            });
            t2.IsBackground = true;
            t3.IsBackground = true;
            t4.IsBackground = true;
            if (HasInternet)
            {
                t2.Start();
                t3.Start();
                t4.Start();
            }
            while (t2.IsAlive)
                await Task.Delay(50);        
            while (t3.IsAlive)
                await Task.Delay(50);         
            while (t4.IsAlive)
                await Task.Delay(50);        

            progressBar.Style = ProgressBarStyle.Marquee;

            ChangeTitle("Populating update list, please wait...\n\n");
          
            await CheckForDevice();
            if (ADB.DeviceID.Length < 5)
            {
                nodeviceonstart = true;
            }
                listappsbtn();
            showAvailableSpace();
            intToolTips();
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
                        System.IO.File.Delete(fileName);
                }
            }
        }



        private void intToolTips()
        {
            ToolTip ListDevicesToolTip = new ToolTip();
            ListDevicesToolTip.SetToolTip(this.devicesbutton, "Lists the devices in a message box, also updates title bar");
            ToolTip SideloadAPKToolTip = new ToolTip();
            SideloadAPKToolTip.SetToolTip(this.startsideloadbutton, "Sideloads/Installs one apk on the android device");
            ToolTip OBBToolTip = new ToolTip();
            OBBToolTip.SetToolTip(this.obbcopybutton, "Copies an obb folder to the Android/Obb folder from the device, some games/apps need this");
            ToolTip BackupGameDataToolTip = new ToolTip();
            BackupGameDataToolTip.SetToolTip(this.backupbutton, "Saves the game and apps data to the sideloader folder, does not save apk's and obb's");
            ToolTip RestoreGameDataToolTip = new ToolTip();
            RestoreGameDataToolTip.SetToolTip(this.restorebutton, "Restores the game and apps data to the device, first use Backup Game Data button");
            ToolTip GetAPKToolTip = new ToolTip();
            GetAPKToolTip.SetToolTip(this.getApkButton, "Saves the selected apk to the folder where the sideloader is");
            ToolTip sideloadFolderToolTip = new ToolTip();
            sideloadFolderToolTip.SetToolTip(this.pullAppToDesktopBtn, "Sideloads every apk from a folder");
            ToolTip uninstallAppToolTip = new ToolTip();
            uninstallAppToolTip.SetToolTip(this.uninstallAppButton, "Uninstalls selected app");
            ToolTip userjsonToolTip = new ToolTip();
            userjsonToolTip.SetToolTip(this.ADBWirelessEnable, "After you enter your username it will create an user.json file needed for some games");
            ToolTip etaToolTip = new ToolTip();
            etaToolTip.SetToolTip(this.etaLabel, "Estimated time when game will finish download, updates every 5 seconds, format is HH:MM:SS");
            ToolTip dlsToolTip = new ToolTip();
            dlsToolTip.SetToolTip(this.speedLabel, "Current download speed, updates every second, in mbps");
        }




        void timer_Tick(object sender, EventArgs e)
        {
            ADB.RunAdbCommandToString("shell input keyevent KEYCODE_WAKEUP");
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            keyheld = false;
        }

        public async void ChangeTitle(string txt, bool reset = true)
        {
            try
            {
                if (ProgressText.IsDisposed) return;
                this.Invoke(() => { oldTitle = txt; this.Text = "Rookie's Sideloader v" + Updater.LocalVersion + " | " + txt; });
                ProgressText.Invoke(() =>
                {
                    if (!ProgressText.IsDisposed)
                        ProgressText.Text = txt;
                });
                if (!reset)
                    return;
                await Task.Delay(TimeSpan.FromSeconds(5));
                this.Invoke(() => { this.Text = "Rookie's Sideloader v" + Updater.LocalVersion + " | " + oldTitle; });
                ProgressText.Invoke(() =>
                {
                    if (!ProgressText.IsDisposed)
                        ProgressText.Text = oldTitle;
                });
            }
            catch
            {

            }
        }



        private void ShowSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
                subMenu.Visible = true;
            else
                subMenu.Visible = false;
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
                    path = openFileDialog.FileName;
                else
                    return;
            }
            ADB.DeviceID = GetDeviceID();

            Thread t1 = new Thread(() =>
            {
                output += ADB.Sideload(path);
            });
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
                await Task.Delay(100);

            showAvailableSpace();

            ShowPrcOutput(output);
        }




        public void ShowPrcOutput(ProcessOutput prcout)
        {
            string message = $"Output: {prcout.Output}";
            if (prcout.Error.Length != 0)
                message += $"\nError: {prcout.Error}";
            FlexibleMessageBox.Show(this, message);
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
                await Task.Delay(100);

            var line = output.Split('\n');

            int i = 0;

            devicesComboBox.Items.Clear();

            Logger.Log("Devices:");
            foreach (string currLine in line)
            {
                if (i > 0 && currLine.Length > 0)
                {
                    Devices.Add(currLine.Split('	')[0]);
                    devicesComboBox.Items.Add(currLine.Split('	')[0]);
                    Logger.Log(currLine.Split('	')[0] + "\n", false);
                }
                Debug.WriteLine(currLine);
                i++;
            }



            if (devicesComboBox.Items.Count > 0)
                devicesComboBox.SelectedIndex = 0;
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

            await CheckForDevice();

            ChangeTitlebarToDevice();

            showAvailableSpace();
        }

        public static void notify(string message)
        {
            if (Properties.Settings.Default.enableMessageBoxes == true)
                FlexibleMessageBox.Show(new Form { TopMost = true, StartPosition = FormStartPosition.CenterScreen }, message);
        }

        private async void obbcopybutton_Click(object sender, EventArgs e)
        {
            ProcessOutput output = new ProcessOutput("", "");
            var dialog = new FolderSelectDialog
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
                });
                t1.IsBackground = true;
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
                        this.Text = "Device Not Authorized";
                        DialogResult dialogResult = FlexibleMessageBox.Show("Device not authorized, be sure to authorize computer on device.", "Not Authorized", MessageBoxButtons.RetryCancel);
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
                    this.Invoke(() => { this.Text = "Device Connected with ID | " + Devices[0].Replace("device", ""); });
                    DeviceConnected = true;
                }
                else
                    this.Invoke(() =>
                    {
                        DeviceConnected = false;
                        this.Text = "No Device Connected";
                        if (!Properties.Settings.Default.nodevicemode)
                        {
                            DialogResult dialogResult = FlexibleMessageBox.Show("No device found. Please ensure the following: \n\n -Developer mode is enabled. \n -ADB drivers are installed. \n -ADB connection is enabled on your device (this can reset). \n -Your device is plugged in.\n\nThen press \"Retry\"", "No device found.", MessageBoxButtons.RetryCancel);
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


        public async void showAvailableSpace()
        {
            string AvailableSpace = string.Empty;
            ADB.DeviceID = GetDeviceID();
            Thread t1 = new Thread(() =>
            {
                AvailableSpace = ADB.GetAvailableSpace();
            });
            t1.Start();

            while (t1.IsAlive)
                await Task.Delay(100);

            diskLabel.Invoke(() => { diskLabel.Text = AvailableSpace; });
        }

        public string GetDeviceID()
        {
            string deviceId = string.Empty;
            int index = -1;
            devicesComboBox.Invoke(() => { index = devicesComboBox.SelectedIndex; });
            if (index != -1)
                devicesComboBox.Invoke(() => { deviceId = devicesComboBox.SelectedItem.ToString(); });
            return deviceId;
        }

        public static bool HasInternet = false;

        public static void CheckForInternet()
        {
            Ping myPing = new Ping();
            String host = "8.8.8.8"; //google dns
            byte[] buffer = new byte[32];
            int timeout = 1000;
            PingOptions pingOptions = new PingOptions();
            try
            {
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    HasInternet = true;
                }
            }
            catch { HasInternet = false; }
        }

        public static string BackupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Rookie Backups");

        public static string taa = "";
        private async void backupbutton_Click(object sender, EventArgs e)
        {
            ProcessOutput output = new ProcessOutput("", "");
            Thread t1 = new Thread(() =>
            {
                ADB.WakeDevice();

                string date_str = DateTime.Today.ToString("yyyy.MM.dd");
                string CurrBackups = Path.Combine(BackupFolder, date_str);
                FlexibleMessageBox.Show($"This may take up to a minute. Backing up gamesaves to Documents\\Rookie Backups\\{date_str} (year.month.date)");
                Directory.CreateDirectory(CurrBackups);
                output = ADB.RunAdbCommandToString($"pull \"/sdcard/Android/data\" \"{CurrBackups}\"");


                try
                {
                    Directory.Move(ADB.adbFolderPath + "\\data", CurrBackups + "\\data");
                }
                catch (Exception ex)
                {
                    Logger.Log($"Exception on backup: {ex.ToString()}");
                }
            });
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
                await Task.Delay(100);

            ShowPrcOutput(output);
        }

        private async void restorebutton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            ProcessOutput output = new ProcessOutput("", "");
            var dialog = new FolderSelectDialog
            {
                Title = "Select full backup or packagename backup folder",
                InitialDirectory = BackupFolder
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
                });
                t1.IsBackground = true;
                t1.Start();

                while (t1.IsAlive)
                    await Task.Delay(100);
            }
            else return;

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

            var line = listapps().Split('\n');

            string forsettings = String.Join("", line);
            Properties.Settings.Default.InstalledApps = forsettings;
            Properties.Settings.Default.Save();

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Length > 9)
                {
                    line[i] = line[i].Remove(0, 8);
                    line[i] = line[i].Remove(line[i].Length - 1);
                    foreach (var game in SideloaderRCLONE.games)
                        if (line[i].Length > 0 && game[2].Contains(line[i]))
                            line[i] = game[0];
                }
            }


            Array.Sort(line);

            foreach (string game in line)
            {
                if (game.Length > 0)
                    m_combo.Invoke(() => { m_combo.Items.Add(game); });
            }

            m_combo.Invoke(() => { m_combo.MatchingMethod = StringMatchingMethod.NoWildcards; });
        }
        public static bool isuploading = false;
        public static bool isworking = false;
        private async void getApkButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            if (m_combo.SelectedIndex == -1)
            {
                notify("Please select an app first");
                return;
            }
            DialogResult dialogResult1 = FlexibleMessageBox.Show($"Do you want to upload {m_combo.SelectedItem.ToString()} now?", "Upload app?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
                return;
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
                ulong VersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                if (Directory.Exists($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}"))
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}", true);
                if (File.Exists($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip"))
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");
                if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip"))
                    File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip");
                ProcessOutput output = new ProcessOutput("", "");
                ChangeTitle("Extracting APK....");

                Directory.CreateDirectory($"{Properties.Settings.Default.MainDir}\\{packageName}");
                
                Thread t1 = new Thread(() =>
                {
                    output = Sideloader.getApk(GameName);
                });
                t1.IsBackground = true;
                t1.Start();

                while (t1.IsAlive)
                    await Task.Delay(100);
                ChangeTitle("Extracting obb if it exists....");
                Thread t2 = new Thread(() =>
                {
                    output += ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packageName}\" \"{Properties.Settings.Default.MainDir}\\{packageName}\"");
                });
                t2.IsBackground = true;
                t2.Start();

                while (t2.IsAlive)
                    await Task.Delay(100);

                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{packageName}\\HWID.txt", HWID);
                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{packageName}\\uploadMethod.txt", "manual");
                ChangeTitle("Zipping extracted application...");
                string cmd = $"7z a -mx1 \"{GameName} v{VersionInt} {packageName}.zip\" .\\{packageName}\\*";
                string path = $"{Properties.Settings.Default.MainDir}\\7z.exe";
                progressBar.Style = ProgressBarStyle.Continuous;
                Thread t4 = new Thread(() =>
                {
                    ADB.RunCommandToString(cmd, path);
                });
                t4.IsBackground = true;
                t4.Start();
                while (t4.IsAlive)
                    await Task.Delay(100);
                ChangeTitle("Uploading to shared drive, you can continue to use Rookie while it uploads in the background.");
                ULGif.Visible = true;
                ULLabel.Visible = true;
                ULGif.Enabled = true;
                isworking = false;
                isuploading = true;
                Thread t3 = new Thread(() =>
                {
                    string currentlyuploading = GameName;
                    ChangeTitle("Uploading to shared drive, you can continue to use Rookie while it uploads in the background.");
                    RCLONE.runRcloneCommand($"copy \"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip\" RSL-gameuploads:");
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");
                    FlexibleMessageBox.Show($"Upload of {currentlyuploading} is complete! Thank you for your contribution!");
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
                });
                t3.IsBackground = true;
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
            else MessageBox.Show("You must wait until each app is finished uploading to start another.");
        }

        private async void uninstallAppButton_Click(object sender, EventArgs e)
        {
            string packagename;
            ADB.WakeDevice();
            if (m_combo.SelectedIndex == -1)
            {
                FlexibleMessageBox.Show("Please select an app first");
                return;
            }
            string GameName = m_combo.SelectedItem.ToString();
            DialogResult dialogresult = FlexibleMessageBox.Show($"Are you sure you want to unintsall {GameName}? Rookie will attempt to automatically backup any saves to Documents\\Rookie Backups\\(TodaysDate)", "Proceed with uninstall?", MessageBoxButtons.YesNo);
            if (dialogresult == DialogResult.No)
            {
                return;
            }
            if (!GameName.Contains("."))
                packagename = Sideloader.gameNameToPackageName(GameName);
            else
                packagename = GameName;
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.Style = ProgressBarStyle.Marquee;
            Thread t1 = new Thread(() =>
            {
                output += Sideloader.UninstallGame(packagename);
            });
            t1.Start();
            t1.IsBackground = true;
            while (t1.IsAlive)
                await Task.Delay(100);
            ShowPrcOutput(output);
            showAvailableSpace();
            progressBar.Style = ProgressBarStyle.Continuous;
            m_combo.Items.RemoveAt(m_combo.SelectedIndex);
        }


        private async void copyBulkObbButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            var dialog = new FolderSelectDialog
            {
                Title = "Select your folder with APKs"
            };
            if (dialog.Show(Handle))
            {
                Thread t1 = new Thread(() =>
                {
                    Sideloader.RecursiveOutput = new ProcessOutput("", "");
                    Sideloader.RecursiveCopyOBB(dialog.FileName);
                });
                t1.IsBackground = true;
                t1.Start();

                showAvailableSpace();

                while (t1.IsAlive)
                    await Task.Delay(100);

                ShowPrcOutput(Sideloader.RecursiveOutput);
            }
        }

        private async void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (nodeviceonstart && !updatesnotified)
            {
                await CheckForDevice();
                ChangeTitlebarToDevice();
                showAvailableSpace();
                ChangeTitle("Device now detected... refreshing update list.");
                listappsbtn();
                initListView();
            }
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
                        Program.form.ChangeTitle($"Copying {data} to device...");

                        Thread t2 = new Thread(() =>

                        {
                            output += ADB.CopyOBB(data);
                        });
                        t2.IsBackground = true;
                        t2.Start();

                        while (t2.IsAlive)
                            await Task.Delay(100);

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
                                string cmdout = ADB.RunCommandToString(cmd, file2).Output;
                                cmdout = Utilities.StringUtilities.RemoveEverythingBeforeFirst(cmdout, "=");
                                cmdout = Utilities.StringUtilities.RemoveEverythingAfterFirst(cmdout, " ");
                                cmdout = cmdout.Replace("'", "");
                                cmdout = cmdout.Replace("=", "");
                                CurrPCKG = cmdout;
                                CurrAPK = file2;
                                System.Windows.Forms.Timer t3 = new System.Windows.Forms.Timer();
                                t3.Interval = 150000; // 180 seconds to fail
                                t3.Tick += timer_Tick4;
                                t3.Start();
                                Program.form.ChangeTitle($"Sideloading apk...");

                                Thread t2 = new Thread(() =>
                                {
                                    output += ADB.Sideload(file2);
                                });
                                t2.IsBackground = true;
                                t2.Start();
                                while (t2.IsAlive)
                                    await Task.Delay(100);
                                t3.Stop();
                                if (Directory.Exists($"{pathname}\\{cmdout}"))
                                {
                                    Program.form.ChangeTitle($"Copying obb folder to device...");
                                    Thread t1 = new Thread(() =>
                                    {
                                        ADB.RunAdbCommandToString($"push \"{pathname}\\{cmdout}\" /sdcard/Android/obb/");
                                    });
                                    t1.IsBackground = true;
                                    t1.Start();
                                    while (t1.IsAlive)
                                        await Task.Delay(100);
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
                                    ADB.RunCommandToString(command2, file2);
                                    output += ADB.RunAdbCommandToString($"push \"{zippath}\\{datazip}\" /sdcard/ModData/com.beatgames.beatsaber/Mods/SongLoader/CustomLevels/");
                                });
                                t2.IsBackground = true;
                                t2.Start();

                                while (t2.IsAlive)
                                    await Task.Delay(100);

                                Directory.Delete($"{zippath}\\{datazip}", true);
                            }
                        }
                    }
                    string[] folders = Directory.GetDirectories(data);
                    foreach (string folder in folders)
                    {
                        Program.form.ChangeTitle($"Copying {folder} to device...");

                        Thread t2 = new Thread(() =>

                        {
                            output += ADB.CopyOBB(folder);
                        });
                        t2.IsBackground = true;
                        t2.Start();

                        while (t2.IsAlive)
                            await Task.Delay(100);

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
                            DialogResult dialogResult = FlexibleMessageBox.Show("Special instructions have been found with this file, would you like to run them automatically?", "Special Instructions found!", MessageBoxButtons.OKCancel);
                            if (dialogResult == DialogResult.Cancel)
                                return;
                            else
                            {
                                ChangeTitle("Sideloading custom install.txt automatically.");

                                Thread t1 = new Thread(() =>

                                {
                                    output += Sideloader.RunADBCommandsFromFile(path);
                                });
                                t1.IsBackground = true;
                                t1.Start();

                                while (t1.IsAlive)
                                    await Task.Delay(100);

                                ChangeTitle(" \n\n");

                            }
                        }
                        else
                        {
                            string pathname = Path.GetDirectoryName(data);
                            string dataname = data.Replace($"{pathname}\\", "");
                            string cmd = $"\"C:\\RSL\\platform-tools\\aapt.exe\" dump badging \"{data}\" | findstr -i \"package: name\"";
                            string cmdout = ADB.RunCommandToString(cmd, data).Output;
                            cmdout = Utilities.StringUtilities.RemoveEverythingBeforeFirst(cmdout, "=");
                            cmdout = Utilities.StringUtilities.RemoveEverythingAfterFirst(cmdout, " ");
                            cmdout = cmdout.Replace("'", "");
                            cmdout = cmdout.Replace("=", "");
                            CurrPCKG = cmdout;
                            CurrAPK = data;
                            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                            timer.Interval = 150000; // 150 seconds to fail
                            timer.Tick += timer_Tick4;
                            timer.Start();

                            ChangeTitle($"Installing {dataname}...");

                            Thread t1 = new Thread(() =>
                            {
                                output += ADB.Sideload(data);
                            });
                            t1.IsBackground = true;
                            t1.Start();
                            while (t1.IsAlive)
                                await Task.Delay(100);
                            timer.Stop();

                            if (Directory.Exists($"{pathname}\\{cmdout}"))
                            {
                                Program.form.ChangeTitle($"Copying obb folder to device...");
                                Thread t2 = new Thread(() =>
                                {
                                    ADB.RunAdbCommandToString($"push \"{pathname}\\{cmdout}\" /sdcard/Android/obb/");
                                });
                                t2.IsBackground = true;
                                t2.Start();
                                while (t2.IsAlive)
                                    await Task.Delay(100);

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
                        Directory.CreateDirectory(foldername);
                        File.Copy(data, foldername + "\\" + filename);
                        path = foldername;

                        Thread t1 = new Thread(() =>

                        {
                            output += ADB.CopyOBB(path);
                        });
                        t1.IsBackground = true;
                        t1.Start();

                        while (t1.IsAlive)
                            await Task.Delay(100);

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
                            ADB.RunCommandToString(command, data);
                            output += ADB.RunAdbCommandToString($"push \"{zippath}\\{datazip}\" /sdcard/ModData/com.beatgames.beatsaber/Mods/SongLoader/CustomLevels/");
                        });
                        t1.IsBackground = true;
                        t1.Start();

                        while (t1.IsAlive)
                            await Task.Delay(100);

                        Directory.Delete($"{zippath}\\{datazip}", true);
                    }
                    else if (extension == ".txt")
                    {
                        ChangeTitle("Sideloading custom install.txt automatically.");

                        Thread t1 = new Thread(() =>

                        {
                            output += Sideloader.RunADBCommandsFromFile(path);
                        });
                        t1.IsBackground = true;
                        t1.Start();

                        while (t1.IsAlive)
                            await Task.Delay(100);


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
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
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
        List<String> newGamesList = new List<string>();
        List<String> newGamesToUploadList = new List<string>();

        private List<UpdateGameData> gamesToAskForUpdate = new List<UpdateGameData>();
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
            List<String> rookieList = new List<String>();
            List<String> installedGames = packageList.ToList();
            List<String> blacklistItems = blacklist.ToList();
            List<String> whitelistItems = whitelist.ToList();
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

                        foreach (string packagename in packageList)
                        {
                            if (string.Equals(release[SideloaderRCLONE.PackageNameIndex], packagename))
                            {
                                if (Properties.Settings.Default.QblindOn)
                                {
                                    Game.BackColor = Color.FromArgb(0, 112, 138);
                                }
                                else
                                {
                                    Game.BackColor = Color.Green;
                                }
                                string InstalledVersionCode;
                                InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename} | grep versionCode -F\"").Output;
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                try
                                {
                                    ulong installedVersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                    ulong cloudVersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));

                                    //Logger.Log($"Checked game {release[SideloaderRCLONE.GameNameIndex]}; cloudversion={cloudVersionInt} localversion={installedVersionInt}");
                                    if (installedVersionInt < cloudVersionInt)
                                    {
                                        if (Properties.Settings.Default.QblindOn)
                                            Game.BackColor = Color.FromArgb(120, 0, 0);
                                        else
                                            Game.BackColor = Color.FromArgb(102, 77, 0);
                                    }

                                    if (installedVersionInt > cloudVersionInt)
                                    {
                                        bool dontget = false;
                                        if (blacklist.Contains(packagename))
                                            dontget = true;
                                        if (!dontget)
                                            Game.BackColor = Color.FromArgb(20, 20, 20);
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
                                    Game.BackColor = Color.FromArgb(121, 25, 194);
                                    Logger.Log($"An error occured while rendering game {release[SideloaderRCLONE.GameNameIndex]} in ListView");
                                    ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename}\"");
                                    Logger.Log($"ExMsg: {ex.Message}Installed:\"{InstalledVersionCode}\" Cloud:\"{Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex])}\"");
                                }
                            }
                        }
                        GameList.Add(Game);
                    }
                });
                t1.IsBackground = true;
                t1.Start();
                while (t1.IsAlive)
                    await Task.Delay(100);
            }
            else if(!isOffline)
            {
                SwitchMirrors();
                initListView();
            }
                

            if (blacklistItems.Count == 0 && GameList.Count == 0 && !Properties.Settings.Default.nodevicemode && !isOffline)
            {
                //This means either the user does not have headset connected or the blacklist
                //did not load, so we are just going to skip everything
                errorOnList = true;
                FlexibleMessageBox.Show($"Rookie seems to have failed to load all resources. Please try restarting Rookie a few times.\nIf error still persists please disable any VPN or firewalls (rookie uses direct download so a VPN is not needed)\nIf this error still persists try a system reboot, reinstalling the program, and lastly posting the problem on telegram.", "Error loading blacklist or game list!");
            }
            newGamesList = installedGames.Except(rookieList).Except(blacklistItems).ToList();
            int topItemIndex = 0;
            try
            {
                if (gamesListView.Items.Count > 1)
                    topItemIndex = gamesListView.TopItem.Index;
            }
            catch (Exception ex)
            { }

            try
            {
                if (topItemIndex != 0)
                    gamesListView.TopItem = gamesListView.Items[topItemIndex];
            }
            catch (Exception ex)
            { }
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
                               DialogResult dialogResult = FlexibleMessageBox.Show($"You have an in demand game:\n\n{ReleaseName}\n\nRSL can AUTOMATICALLY UPLOAD the clean files to a shared drive in the background,\nthis is the only way to keep the apps up to date for everyone.\n\nNOTE: Rookie will only extract the APK/OBB which contain NO personal information whatsoever.", "CONTRIBUTE CLEAN FILES?", MessageBoxButtons.YesNo);
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
                            bool onapplist = false;
                            string NewApp = Properties.Settings.Default.NonAppPackages + "\n" + Properties.Settings.Default.AppPackages;
                            if (NewApp.Contains(newGamesToUpload))
                                onapplist = true;
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
                                    File.Delete($"C:\\RSL\\platform-tools\\base.apk");
                                ADB.RunAdbCommandToString($"pull \"{apppath}\"");
                                string cmd = $"\"C:\\RSL\\platform-tools\\aapt.exe\" dump badging \"C:\\RSL\\platform-tools\\base.apk\" | findstr -i \"application-label\"";
                                string workingpath = $"C:\\RSL\\platform-tools\\aapt.exe";
                                string ReleaseName = ADB.RunCommandToString(cmd, workingpath).Output;
                                ReleaseName = Utilities.StringUtilities.RemoveEverythingBeforeFirst(ReleaseName, "'");
                                ReleaseName = Utilities.StringUtilities.RemoveEverythingAfterFirst(ReleaseName, "\r\n");
                                ReleaseName = ReleaseName.Replace("'", "");
                                File.Delete($"C:\\RSL\\platform-tools\\base.apk");
                                if (ReleaseName.Contains("Microsoft Windows"))
                                    ReleaseName = RlsName;
                                //end
                                string GameName = Sideloader.gameNameToSimpleName(RlsName);
                                //Logger.Log(newGamesToUpload);
                                string InstalledVersionCode;
                                InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {newGamesToUpload} | grep versionCode -F\"").Output;
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                                InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                                ulong installedVersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                                DonorApps += ReleaseName + ";" + newGamesToUpload + ";" + installedVersionInt + ";" + "New App" + "\n";
                                newint++;
                            }
                        }
                    }
                }
            });
            t2.IsBackground = true;
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
                DonorForm.ShowDialog();
                this.Focus();
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
            Program.form.ChangeTitle("Uploading to shared drive, you can continue to use Rookie while it uploads in the background.");
            Program.form.ULGif.Visible = true;
            Program.form.ULLabel.Visible = true;
            Program.form.ULGif.Enabled = true;
            isworking = true;

            foreach (UploadGame game in Program.form.gamesToUpload)
            {

                Thread t3 = new Thread(() =>
                {
                    string packagename = game.Pckgcommand;
                    if (File.Exists($"{Properties.Settings.Default.MainDir}\\{game.Uploadgamename} v{game.Uploadversion}.zip"))
                        File.Delete($"{Properties.Settings.Default.MainDir}\\{game.Uploadgamename} v{game.Uploadversion}.zip");
                    string path = $"{Properties.Settings.Default.MainDir}\\7z.exe";
                    string cmd = $"7z a -mx1 \"{Properties.Settings.Default.MainDir}\\{game.Uploadgamename} v{game.Uploadversion} {game.Pckgcommand}.zip\" .\\{game.Pckgcommand}\\*";
                    Program.form.ChangeTitle("Zipping extracted application...");
                    ADB.RunCommandToString(cmd, path);
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{game.Pckgcommand}", true);
                    Program.form.ChangeTitle("Uploading to drive, you may continue to use Rookie while it uploads.");
                    RCLONE.runRcloneCommand(game.Uploadcommand);
                    if (game.isUpdate)
                    {
                        Properties.Settings.Default.SubmittedUpdates += game.Pckgcommand + ("\n");
                        Properties.Settings.Default.Save();
                    }
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{game.Uploadgamename} v{game.Uploadversion} {game.Pckgcommand}.zip");

                });
                t3.IsBackground = true;
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
              if (!String.IsNullOrEmpty(Properties.Settings.Default.NonAppPackages) && !Properties.Settings.Default.ListUpped)
            {
                Random r = new Random();
                int x = r.Next(9999);
                int y = x;
                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\FreeOrNonVR{y}.txt", Properties.Settings.Default.NonAppPackages);
                string path = $"{Properties.Settings.Default.MainDir}\\rclone\\rclone.exe";

                Thread t1 = new Thread(() =>
                {
                    ADB.RunCommandToString($"\"{Properties.Settings.Default.MainDir}\\rclone\\rclone.exe\" copy \"{Properties.Settings.Default.MainDir}\\FreeOrNonVR{y}.txt\" VRP-debuglogs:InstalledGamesList", path);
                    File.Delete($"{Properties.Settings.Default.MainDir}\\FreeOrNonVR{y}.txt");
                });
                t1.IsBackground = true;
                t1.Start();

                while (t1.IsAlive)
                    await Task.Delay(100);
                Properties.Settings.Default.ListUpped = true;
                Properties.Settings.Default.Save();

            }
        }


        public async Task extractAndPrepareGameToUploadAsync(string GameName, string packagename, ulong installedVersionInt, bool isupdate)
        {
            progressBar.Style = ProgressBarStyle.Marquee;

            Thread t1 = new Thread(() =>
            {
                Sideloader.getApk(packagename);
            });
            ChangeTitle("Extracting APK file....");
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
                await Task.Delay(100);
            ChangeTitle("Extracting obb if it exists....");
            Thread t2 = new Thread(() =>
            {
                ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packagename}\" \"{Properties.Settings.Default.MainDir}\\{packagename}\"");
            });
            t2.IsBackground = true;
            t2.Start();

            while (t2.IsAlive)
                await Task.Delay(100);
            string HWID = SideloaderUtilities.UUID();
            File.WriteAllText($"{Properties.Settings.Default.MainDir}\\{packagename}\\HWID.txt", HWID);
            progressBar.Style = ProgressBarStyle.Continuous;
            UploadGame game = new UploadGame();

            game.isUpdate = isupdate;
            game.Pckgcommand = packagename;
            game.Uploadgamename = GameName;
            game.Uploadversion = installedVersionInt;
            game.Uploadcommand = $"copy \"{Properties.Settings.Default.MainDir}\\{game.Uploadgamename} v{game.Uploadversion} {game.Pckgcommand}.zip\" RSL-gameuploads:";
            gamesToUpload.Add(game);
        }
        private void initMirrors(bool random)
        {
            int index = 0;
            remotesList.Invoke(() => { index = remotesList.SelectedIndex; remotesList.Items.Clear(); });

            string[] mirrors = RCLONE.runRcloneCommand("listremotes").Output.Split('\n');

            Logger.Log("Loaded following mirrors: ");
            int itemsCount = 0;
            foreach (string mirror in mirrors)
            {
                if (mirror.Contains("mirror"))
                {


                    Logger.Log(mirror.Remove(mirror.Length - 1));
                    remotesList.Invoke(() => { remotesList.Items.Add(mirror.Remove(mirror.Length - 1).Replace("VRP-mirror", "")); });
                    itemsCount++;

                }
            }

            if (itemsCount > 0)
            {
                
                var rand = new Random();
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

        private string wrDelimiter = "-------";

        private void sideloadContainer_Click(object sender, EventArgs e)
        {
            ShowSubMenu(sideloadContainer);
        }

        private void backupDrop_Click(object sender, EventArgs e)
        {
            ShowSubMenu(backupContainer);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            string about = $@"Version: {Updater.LocalVersion}

 - Software orignally coded by rookie.wtf
 - Thanks to pmow for all of his work, including rclone, wonka and other projects, and for scripting the backend
without him none of this would be possible
 - Thanks to the data team, they know who they are ;) (Especially HarryEffingPotter, no he did not put this here himself, that is perposterous.)
 - Thanks to flow for being friendly and helping every one, also congrats on being the discord server owner now! :D
 - Thanks to badcoder5000 for helping me redesign the ui
 - Thanks to gotard for the theme changer
 - Thanks to Verb8em for drawing the new icon
 - Thanks to 7zip team for 7zip :)
 - Thanks to rclone team for rclone :D
 - Thanks to https://stackoverflow.com/users/57611/erike for the folder browser dialog code
 - Thanks to Serge Weinstock for developing SergeUtils, which is used to search the combo box
 - Thanks to Mike Gold https://www.c-sharpcorner.com/members/mike-gold2 for the scrollable message box

 - HarryEffinPotter Thanks: Roma/Rookie, Pmow, Ivan, Kaladin, John, Sam Hoque, Flow, and the mod staff!";

            FlexibleMessageBox.Show(about);
        }

        private async void ADBWirelessEnable_Click(object sender, EventArgs e)
        {

            ADB.WakeDevice();
            DialogResult dialogResult = FlexibleMessageBox.Show("Make sure your Quest is plugged in VIA USB then press OK, if you need a moment press Cancel and come back when you're ready.", "Connect Quest now.", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel)
                return;

            ADB.RunAdbCommandToString("devices");
            ADB.RunAdbCommandToString("tcpip 5555");

            FlexibleMessageBox.Show("Press OK to get your Quest's local IP address.", "Obtain local IP address", MessageBoxButtons.OKCancel);
            Thread.Sleep(1000);
            string input = ADB.RunAdbCommandToString("shell ip route").Output;

            Properties.Settings.Default.WirelessADB = true;
            Properties.Settings.Default.Save();
            string[] strArrayOne = new string[] { "" };
            strArrayOne = input.Split(' ');
            if (strArrayOne[0].Length > 7)
            {
                string IPaddr = strArrayOne[8];
                string IPcmnd = "connect " + IPaddr + ":5555";
                FlexibleMessageBox.Show($"Your Quest's local IP address is: {IPaddr}\n\nPlease disconnect your Quest then wait 2 seconds.\nOnce it is disconnected hit OK", "", MessageBoxButtons.OK);
                Thread.Sleep(2000);
                ADB.RunAdbCommandToString(IPcmnd);
                await Program.form.CheckForDevice();
                Program.form.ChangeTitlebarToDevice();
                Program.form.showAvailableSpace();
                Properties.Settings.Default.IPAddress = IPcmnd;
                Properties.Settings.Default.Save();
                File.WriteAllText($"C:\\RSL\\platform-tools\\StoredIP.txt", IPcmnd);
                ADB.wirelessadbON = true;
                ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
            }
            else
                FlexibleMessageBox.Show("No device connected! Connect quest via USB and start again!");

        }

        private async void listApkButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            ChangeTitle("Refreshing connected devices, installed apps and update list...");
            if (isLoading)
                return;
            isLoading = true;

            progressBar.Style = ProgressBarStyle.Marquee;
            CheckForInternet();
            devicesbutton_Click(sender, e);

            Thread t1 = new Thread(() =>
            {
                initMirrors(false);
                SideloaderRCLONE.initGames(currentRemote);
                listappsbtn();
            });
            t1.IsBackground = false;
            t1.Start();
            while (t1.IsAlive)
                await Task.Delay(100);
            initListView();
            isLoading = false;

            ChangeTitle(" \n\n");
        }

        private static readonly HttpClient client = new HttpClient();
        public static bool reset = false;
        public static bool updatedConfig = false;
        public static int steps = 0;
        public static bool gamesAreDownloading = false;
        private List<string> gamesQueueList = new List<string>();
        public static int quotaTries = 0;
        public static bool timerticked = false;
        public static bool skiponceafterremove = false;


        public void SwitchMirrors()
        {
            quotaTries++;
            remotesList.Invoke(() =>
            {
                if (quotaTries > remotesList.Items.Count)
                {
	                var dialog = ShowError_QuotaExceeded();
	                switch (dialog)
	                {
		                case DialogResult.No:
			                Application.Exit();
                            break;

                        case DialogResult.Yes:
	                        isOffline = true;
                            break; 

                        default: throw new ArgumentOutOfRangeException(nameof(dialog));
	                }

	                
                    
                }
                if (remotesList.SelectedIndex + 1 == remotesList.Items.Count)
                {
                    reset = true;
                    for (int i = 0; i < steps; i++)
                        remotesList.SelectedIndex--;

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

        private static DialogResult ShowError_QuotaExceeded()
        {
            const string errorMessage =
@"Quota reached for all mirrors.

This just means that the download quota has been exceeded for the mirror(s).
Unfortunately, this is an error from Google and is outside of our control.

Things you can try:
1) Use a third party config from the wiki (https://wiki.vrpirates.club/general_information/third-party-rclone-configs)
2) Wait for the quota to reset sometime in the next 24 hours.
3) Use Resilio for p2p downloads (https://wiki.vrpirates.club/en/Howto/Resilio-Sync-setup-guide)
4) Sponsor a private server (https://wiki.vrpirates.club/en/Howto/sponsored-mirrors)

Run app in offline mode? If no, app will be exited
";

            return FlexibleMessageBox.Show(errorMessage, "Quota exceeded", MessageBoxButtons.YesNo);
        }

        public bool isinstalling = false;
        public static bool removedownloading = false;
        public async void downloadInstallGameButton_Click(object sender, EventArgs e)
        {
            {
                if (nodeviceonstart && !updatesnotified)
                {
                    await CheckForDevice();
                    ChangeTitlebarToDevice();
                    showAvailableSpace();
                    ChangeTitle("Device now detected... refreshing update list.");
                    listappsbtn();
                    initListView();     
                }
                progressBar.Style = ProgressBarStyle.Marquee;
                if (gamesListView.SelectedItems.Count == 0) return;
                string namebox = gamesListView.SelectedItems[0].ToString();
                string nameboxtranslated = Sideloader.gameNameToSimpleName(namebox);
                int count = 0;
                string[] gamesToDownload;
                if (gamesListView.SelectedItems.Count > 0)
                {
                    count = gamesListView.SelectedItems.Count;
                    gamesToDownload = new string[count];
                    for (int i = 0; i < count; i++)
                        gamesToDownload[i] = gamesListView.SelectedItems[i].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
                }
                else return;
                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Continuous;
                string game;
                if (gamesToDownload.Length == 1)
                {
                    game = $"\"{gamesToDownload[0]}\"";
                }
                else
                {
                    game = "the selected games";
                }
                isinstalling = true;
                //Add games to the queue
                for (int i = 0; i < gamesToDownload.Length; i++)
                    gamesQueueList.Add(gamesToDownload[i]);

                gamesQueListBox.DataSource = null;
                gamesQueListBox.DataSource = gamesQueueList;

                if (gamesAreDownloading)
                    return;
                gamesAreDownloading = true;


                //Do user json on firsttime
                if (Properties.Settings.Default.userJsonOnGameInstall)
                {
                    Thread userJsonThread = new Thread(() => { ChangeTitle("Pushing user.json"); Sideloader.PushUserJsons(); });
                    userJsonThread.IsBackground = true;
                    userJsonThread.Start();

                }

                ProcessOutput output = new ProcessOutput("", "");

                string gameName = "";
                while (gamesQueueList.Count > 0)
                {
                    gameName = gamesQueueList.ToArray()[0];
                    string packagename = Sideloader.gameNameToPackageName(gameName);
                    string dir = Path.GetDirectoryName(gameName);
                    string gameDirectory = Environment.CurrentDirectory + "\\" + gameName;
                    string path = gameDirectory;
                    Directory.CreateDirectory(gameDirectory);
                    ProcessOutput gameDownloadOutput = new ProcessOutput("", "");

                    Thread t1 = new Thread(() =>
                    {
                        gameDownloadOutput = RCLONE.runRcloneCommand($"copy \"{currentRemote}:{SideloaderRCLONE.RcloneGamesFolder}/{gameName}\" \"{Environment.CurrentDirectory}\\{gameName}\" --progress --drive-acknowledge-abuse --rc", Properties.Settings.Default.BandwithLimit);
                    });
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

                            float downloadSpeed = results.speed.ToObject<float>();

                            long allSize = 0;

                            long downloaded = 0;

                            dynamic check = results.transferring;

                            if (results["transferring"] != null)
                            {
                                foreach (var obj in results.transferring)
                                {
                                    allSize += obj["size"].ToObject<long>();
                                    downloaded += obj["bytes"].ToObject<long>();
                                }
                                allSize /= 1000000;
                                downloaded /= 1000000;
                                Debug.WriteLine("Allsize: " + allSize + "\nDownloaded: " + downloaded + "\nValue: " + (((double)downloaded / (double)allSize) * 100));
                                try
                                {
                                    progressBar.Style = ProgressBarStyle.Continuous;
                                    progressBar.Value = Convert.ToInt32((((double)downloaded / (double)allSize) * 100));
                                }
                                catch { }

                                i++;
                                downloadSpeed /= 1000000;
                                if (i == 4)
                                {
                                    i = 0;
                                    float seconds = (allSize - downloaded) / downloadSpeed;
                                    TimeSpan time = TimeSpan.FromSeconds(seconds);
                                    etaLabel.Text = "ETA: " + time.ToString(@"hh\:mm\:ss") + " left";
                                }

                                speedLabel.Text = "DLS: " + String.Format("{0:0.00}", downloadSpeed) + " MB/s";
                            }
                        }
                        catch { }

                        await Task.Delay(100);


                    }
                    if (removedownloading)
                    { 
                            ChangeTitle("Deleting game files", false);
                            try { Directory.Delete(Environment.CurrentDirectory + "\\" + gameName, true); } catch (Exception ex) { FlexibleMessageBox.Show($"Error deleting game files: {ex.Message}"); }
                        ChangeTitle("");
                        break;
                    }
                    {
                        //Quota Errors
                        bool isinstalltxt = false;
                        bool quotaError = false;
                        if (gameDownloadOutput.Error.Length > 0 && !isOffline)
                        {
                            string err = gameDownloadOutput.Error.ToLower();
                            err += gameDownloadOutput.Output.ToLower();
                            if (err.Contains("quota") && err.Contains("exceeded") || err.Contains("directory not found"))
                            {
                                quotaError = true;

                                SwitchMirrors();

                                gamesQueueList.RemoveAt(0);
                                gamesQueListBox.DataSource = null;
                                gamesQueListBox.DataSource = gamesQueueList;
                            }
                            else if (!gameDownloadOutput.Error.Contains("localhost")) FlexibleMessageBox.Show($"Rclone error: {gameDownloadOutput.Error}");
                        }
                        if (quotaError == false)
                        {
                            ADB.WakeDevice();
                            ADB.DeviceID = GetDeviceID();
                            quotaTries = 0;
                            progressBar.Value = 0;
                            progressBar.Style = ProgressBarStyle.Continuous;
                            ChangeTitle("Installing game apk " + gameName, false);
                            etaLabel.Text = "ETA: Wait for install...";
                            speedLabel.Text = "DLS: Finished";
                            if (File.Exists(Environment.CurrentDirectory + "\\" + gameName + "\\install.txt"))
                                isinstalltxt = true;
                            if (File.Exists(Environment.CurrentDirectory + "\\" + gameName + "\\Install.txt"))
                                isinstalltxt = true;
                            string[] files = Directory.GetFiles(Environment.CurrentDirectory + "\\" + gameName);

                            Debug.WriteLine("Game Folder is: " + Environment.CurrentDirectory + "\\" + gameName);
                            Debug.WriteLine("FILES IN GAME FOLDER: ");
                            foreach (string file in files)
                            {
                                Debug.WriteLine(file);
                                string extension = Path.GetExtension(file);
                                if (extension == ".txt")
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
                                            await Task.Delay(100);
                                    }
                                }
                                else if (!isinstalltxt)
                                {
                                    if (extension == ".apk")
                                    {
                                        CurrAPK = file;
                                        CurrPCKG = packagename;
                                        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                                        t.Interval = 150000; // 150 seconds to fail
                                        t.Tick += new EventHandler(timer_Tick4);
                                        t.Start();
                                        Thread apkThread = new Thread(() =>
                                        {
                                            Program.form.ChangeTitle($"Sideloading apk...");
                                            output += ADB.Sideload(file, packagename);
                                        });
                                        apkThread.IsBackground = true;
                                        apkThread.Start();
                                        while (apkThread.IsAlive)
                                            await Task.Delay(100);
                                        t.Stop();
                                    }

                                    Debug.WriteLine(wrDelimiter);
                                    if (Directory.Exists($"{Properties.Settings.Default.MainDir}\\{gameName}\\{packagename}"))
                                    {
                                        Thread obbThread = new Thread(() =>
                                        {

                                            ChangeTitle($"Copying {packagename} obb to device...");
                                            output += ADB.RunAdbCommandToString($"push \"{Properties.Settings.Default.MainDir}\\{gameName}\\{packagename}\" \"/sdcard/Android/obb\"");
                                            Program.form.ChangeTitle("");
                                        });
                                        obbThread.IsBackground = true;
                                        obbThread.Start();

                                        while (obbThread.IsAlive)
                                            await Task.Delay(100);
                                    }

                                }
                                ChangeTitle($"Installation of {gameName} completed.");
                            }
                            if (Properties.Settings.Default.deleteAllAfterInstall)
                            {
                                ChangeTitle("Deleting game files", false);
                                try { Directory.Delete(Environment.CurrentDirectory + "\\" + gameName, true); } catch (Exception ex) { FlexibleMessageBox.Show($"Error deleting game files: {ex.Message}"); }
                            }

                            //Remove current game
                            gamesQueueList.RemoveAt(0);
                            gamesQueListBox.DataSource = null;
                            gamesQueListBox.DataSource = gamesQueueList;
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
                    ChangeTitle("Refreshing games list, please wait...         \n");
                    showAvailableSpace();
                    listappsbtn();
                    initListView();
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

        private void timer_Tick4(object sender, EventArgs e)
        {
            ProcessOutput output = new ProcessOutput("", "");
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
                        DialogResult dialogResult = FlexibleMessageBox.Show("In place upgrade has failed." +
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
                    ADB.RunAdbCommandToString("kill-server");
                    ADB.RunAdbCommandToString("devices");
                    ADB.RunAdbCommandToString($"pull /sdcard/Android/data/{CurrPCKG} \"{Environment.CurrentDirectory}\"");
                    Sideloader.UninstallGame(CurrPCKG);
                    ChangeTitle("Reinstalling Game");
                    output += ADB.RunAdbCommandToString($"install -g \"{CurrAPK}\"");
                    ADB.RunAdbCommandToString($"push \"{Environment.CurrentDirectory}\\{CurrPCKG}\" /sdcard/Android/data/");

                    timerticked = false;
                    if (Directory.Exists($"{Environment.CurrentDirectory}\\{CurrPCKG}"))
                        Directory.Delete($"{Environment.CurrentDirectory}\\{CurrPCKG}", true);

                    ChangeTitle(" \n\n");
                    return;
                }
                else
                {
                    DialogResult dialogResult2 = FlexibleMessageBox.Show("This install is taking an unusual amount of time, you can keep waiting or cancel the install.\n" +
                        "Would you like to cancel the installation?", "Cancel install?", MessageBoxButtons.YesNo);
                    if (dialogResult2 == DialogResult.Yes)
                    {
                        ChangeTitle("Stopping Install...");
                        ADB.RunAdbCommandToString("kill-server");
                        ADB.RunAdbCommandToString("devices");
                    }
                    else
                    {
                        timerticked = false;
                        return;
                    }
                }
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isinstalling)
            {
                var res1 = FlexibleMessageBox.Show(this, "There are downloads and/or installations in progress,\nif you exit now you'll have to start the entire process over again.\nAre you sure you want to exit?", "Still downloading/installing.",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res1 != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (isuploading)
            {
                var res = FlexibleMessageBox.Show(this, "There is an upload still in progress, if you exit now\nyou'll have to start the entire process over again.\nAre you sure you want to exit?", "Still uploading.",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    RCLONE.killRclone();
                    ADB.RunAdbCommandToString("kill-server");
                }
            }
            else
            {
                RCLONE.killRclone();
                ADB.RunAdbCommandToString("kill-server");
            }

        }
        private void ADBWirelessDisable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = FlexibleMessageBox.Show("Are you sure you want to delete your saved Quest IP address/command?", "Remove saved IP address?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                FlexibleMessageBox.Show("Saved IP data reset cancelled.");
                return;
            }
            else
            {
                ADB.wirelessadbON = false;
                FlexibleMessageBox.Show("Make sure your device is not connected to USB and press OK.");
                ADB.RunAdbCommandToString("devices");
                ADB.RunAdbCommandToString("shell USB");
                Thread.Sleep(2000);
                ADB.RunAdbCommandToString("disconnect");
                Thread.Sleep(2000);
                ADB.RunAdbCommandToString("kill-server");
                Thread.Sleep(2000);
                ADB.RunAdbCommandToString("start-server");
                Properties.Settings.Default.IPAddress = "";
                Properties.Settings.Default.Save();
                Program.form.GetDeviceID();
                Program.form.ChangeTitlebarToDevice();
                FlexibleMessageBox.Show("Relaunch Rookie to complete the process and switch back to USB adb.");
                if (File.Exists("C:\\RSL\\platform-tools\\StoredIP.txt"))
                    File.Delete("C:\\RSL\\platform-tools\\StoredIP.txt");
            }
        }
        private void EnablePassthroughAPI_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            ADB.RunAdbCommandToString("shell setprop debug.oculus.experimentalEnabled 1");
            FlexibleMessageBox.Show("Passthrough API enabled.");

        }

        private async void killRcloneButton_Click(object sender, EventArgs e)
        {
            if (isLoading)
                return;
            RCLONE.killRclone();
            ADBWirelessDisable.Text = "Start Movie Stream";
            ChangeTitle("Killed Rclone");
            await CheckForDevice();
            ChangeTitlebarToDevice();
        }

        private void otherDrop_Click(object sender, EventArgs e)
        {
            ShowSubMenu(otherContainer);
        }
        private void gamesQueListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (gamesQueListBox.SelectedIndex == 0 && gamesQueueList.Count == 1)
            {
                speedLabel.Text = "";
                etaLabel.Text = "";
                removedownloading = true;
                RCLONE.killRclone();
                gamesQueueList.Remove(gamesQueListBox.SelectedItem.ToString());
                gamesQueListBox.DataSource = null;
                gamesQueListBox.DataSource = gamesQueueList;
            }
            if (gamesQueListBox.SelectedIndex != -1 && gamesQueListBox.SelectedIndex != 0)
            {
                gamesQueueList.Remove(gamesQueListBox.SelectedItem.ToString());
                gamesQueListBox.DataSource = null;
                gamesQueListBox.DataSource = gamesQueueList;
            }

        }

        private void devicesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            showAvailableSpace();
        }
        private void remotesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            remotesList.Invoke(() => { currentRemote = "VRP-mirror" + remotesList.SelectedItem.ToString(); });
        }
        private void QuestOptionsButton_Click(object sender, EventArgs e)
        {
            QuestForm Form = new QuestForm();
            Form.Show();
        }

        private void SpoofFormButton_Click(object sender, EventArgs e)
        {
            SpoofForm Form = new SpoofForm();
            Form.Show();
        }

        private void ThemeChangerButton_Click(object sender, EventArgs e)
        {
            themeForm Form = new themeForm();
            Form.Show();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                if (e.Column == 4)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            // Perform the sort with these new sort options.
            this.gamesListView.Sort();
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
                            downloadInstallGameButton_Click(sender, e);
                    }
                }
                searchTextBox.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;

                if (ADBcommandbox.Visible)
                {
                    ChangeTitle($"Entered command: ADB {ADBcommandbox.Text}");
                    ADB.RunAdbCommandToString(ADBcommandbox.Text);
                    ChangeTitle(" \n\n");
                }
                ADBcommandbox.Visible = false;
                label9.Visible = false;
                label11.Visible = false;
                label2.Visible = false;

            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                searchTextBox.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                ADBcommandbox.Visible = false;
                label9.Visible = false;
                label11.Visible = false;
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
                label3.Visible = true;
                label4.Visible = true;
                searchTextBox.Focus();
            }
            if (keyData == (Keys.Control | Keys.L))
            {
                if (loaded)
                {
                    Clipboard.SetText(rookienamelist);
                    MessageBox.Show("Entire game list copied as a line by line list to clipboard!\nPress CTRL+V to paste it anywhere!");
                }

            }
            if (keyData == (Keys.Alt | Keys.L))
            {
                if (loaded)
                {
                    Clipboard.SetText(rookienamelist2);
                    MessageBox.Show("Entire game list copied as a paragraph to clipboard!\nPress CTRL+V to paste it anywhere!");
                }

            }
            if (keyData == (Keys.Control | Keys.H))
            {
                string HWID = SideloaderUtilities.UUID();
                Clipboard.SetText(HWID);
                FlexibleMessageBox.Show($"Your unique HWID is:\n\n{HWID}\n\nThis has been automatically copied to your clipboard. Press CTRL+V in a message to send it.");
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                ADBcommandbox.Visible = true;
                ADBcommandbox.Clear();
                label9.Visible = true;
                label11.Visible = true;
                label2.Visible = true;
                ADBcommandbox.Focus();
            }
            if (keyData == (Keys.F2))
            {
                searchTextBox.Clear();
                searchTextBox.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                searchTextBox.Focus();
            }
            if (keyData == (Keys.Control | Keys.F4))
                try
                {
                    //run the program again and close this one
                    Process.Start(Application.StartupPath + "\\Sideloader Launcher.exe");
                    //or you can use Application.ExecutablePath

                    //close this one
                    Process.GetCurrentProcess().Kill();
                }
                catch
                { }
            if (keyData == (Keys.F3))
            {
                if (Application.OpenForms.OfType<QuestForm>().Count() == 0)
                {
                    QuestForm Form = new QuestForm();
                    Form.Show();
                }

            }
            if (keyData == (Keys.F4))
            {
                if (Application.OpenForms.OfType<SettingsForm>().Count() == 0)
                {
                    SettingsForm Form = new SettingsForm();
                    Form.Show();
                }
            }
            if (keyData == (Keys.F5))
            {
                ADB.WakeDevice();
                GetDeviceID();
                FlexibleMessageBox.Show("If your device is not Connected, hit reconnect first or it won't work!\nNOTE: THIS MAY TAKE UP TO 60 SECONDS.\nThere will be a Popup text window with all updates available when it is done!", "Is device connected?", MessageBoxButtons.OKCancel);
                listappsbtn();
                initListView();
            }
            bool dialogisup = false;
            if (keyData == (Keys.F1) && !dialogisup)
            {
                dialogisup = true;
                FlexibleMessageBox.Show("Shortcuts:\nF1 -------- Shortcuts List\nF2 --OR-- CTRL+F: QuickSearch\nF3 -------- Quest Options\nF4 -------- Rookie Settings\nF5 -------- Refresh Gameslist\n\nCTRL+R - Run custom ADB command.\nCTRL+L - Copy entire list of Game Names to clipboard seperated by new lines.\nALT+L - Copy entire list of Game Names to clipboard seperated by commas(in a paragraph).CTRL+P - Copy packagename to clipboard on game select.\nCTRL + F4 - Instantly relaunch Rookie's Sideloader.");
                dialogisup = false;
            }
            if (keyData == (Keys.Control | Keys.P))
            {
                DialogResult dialogResult = FlexibleMessageBox.Show("Do you wish to copy Package Name of games selected from list to clipboard?", "Copy package to clipboard?", MessageBoxButtons.YesNo);
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
            this.searchTextBox.KeyPress += new
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
                        foundItem.Selected = true;


                    searchTextBox.Focus();
                }
            }
        }



        private void ADBcommandbox_Enter(object sender, EventArgs e)
        {
            ADBcommandbox.Focus();


        }


        public void gamesListView_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (gamesListView.SelectedItems.Count < 1)
                return;
            string CurrentPackageName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.PackageNameIndex].Text;
            string CurrentReleaseName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            if (!keyheld)
            {
                if (Properties.Settings.Default.PackageNameToCB)
                    Clipboard.SetText(CurrentPackageName);
                keyheld = true;
            }

            string ImagePath = "";
            if (File.Exists($"{SideloaderRCLONE.ThumbnailsFolder}\\{CurrentPackageName}.jpg"))
                ImagePath = $"{SideloaderRCLONE.ThumbnailsFolder}\\{CurrentPackageName}.jpg";
            else if (File.Exists($"{SideloaderRCLONE.ThumbnailsFolder}\\{CurrentPackageName}.png"))
                ImagePath = $"{SideloaderRCLONE.ThumbnailsFolder}\\{CurrentPackageName}.png";
            if (gamesPictureBox.BackgroundImage != null)
                gamesPictureBox.BackgroundImage.Dispose();
            if (File.Exists(ImagePath))
            {
                gamesPictureBox.BackgroundImage = Image.FromFile(ImagePath);
            }
            else
                gamesPictureBox.BackgroundImage = new Bitmap(367, 214);

            string NotePath = $"{SideloaderRCLONE.NotesFolder}\\{CurrentReleaseName}.txt";
            if (File.Exists(NotePath))
                notesRichTextBox.Text = File.ReadAllText(NotePath);
            else
                notesRichTextBox.Text = "";
        }

        public void UpdateGamesButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            GetDeviceID();
            FlexibleMessageBox.Show("If your device is not Connected, hit reconnect first or it won't work!\nNOTE: THIS MAY TAKE UP TO 60 SECONDS.\nThere will be a Popup text window with all updates available when it is done!", "Is device connected?", MessageBoxButtons.OKCancel);
            listappsbtn();
            initListView();

            if (SideloaderRCLONE.games.Count < 1)
            {
                FlexibleMessageBox.Show("There are no games in rclone, please check your internet connection and check if the config is working properly");
                return;
            }

            // if (gamesToUpdate.Length > 0)
            //     FlexibleMessageBox.Show(gamesToUpdate);
            //  else
            //     FlexibleMessageBox.Show("All your games are up to date!");
        }

        private void gamesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (gamesListView.SelectedItems.Count > 0)
                downloadInstallGameButton_Click(sender, e);
        }

        private void MountButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            ADB.RunAdbCommandToString("shell svc usb setFunctions mtp true");
        }

        private void freeDisclaimer_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nerdunit/androidsideloader");
        }

        private async void removeQUSetting_Click(object sender, EventArgs e)
        {

            if (m_combo.SelectedIndex == -1)
            {
                FlexibleMessageBox.Show("Please select an app first");
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
                await Task.Delay(100);

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
                FlexibleMessageBox.Show("Please select an app first");
                return;
            }
            ADB.WakeDevice();
            ProcessOutput output = new ProcessOutput("", "");
            progressBar.Style = ProgressBarStyle.Marquee;

            string GameName = m_combo.SelectedItem.ToString();
            string pckg = Sideloader.gameNameToPackageName(GameName);
            Thread t1 = new Thread(() =>
            {
                ADB.RunAdbCommandToString($"shell mkdir sdcard/android/data/{pckg}");
                ADB.RunAdbCommandToString($"shell mkdir sdcard/android/data/{pckg}/private");
                Random r = new Random();

                int x = r.Next(999999999);
                int y = r.Next(9999999);

                var sum = ((long)y * (long)1000000000) + (long)x;

                int x2 = r.Next(999999999);
                int y2 = r.Next(9999999);

                var sum2 = ((long)y2 * (long)1000000000) + (long)x2;

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
                await Task.Delay(100);

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
            label3.Visible = true;
            label4.Visible = true;
            searchTextBox.Focus();
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
                label3.Visible = false;
                label4.Visible = false;
            }
            else
                gamesListView.Focus();
        }

        private void gamesListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (Properties.Settings.Default.EnterKeyInstall)
                {
                    if (gamesListView.SelectedItems.Count > 0)
                        downloadInstallGameButton_Click(sender, e);
                }
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void EnterInstallBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnterKeyInstall = EnterInstallBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void ADBcommandbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.searchTextBox.KeyPress += new
            System.Windows.Forms.KeyPressEventHandler(CheckEnter);
            if (e.KeyChar == (char)Keys.Enter)

            {
                Program.form.ChangeTitle($"Running adb command: ADB {ADBcommandbox.Text}");
                string output = ADB.RunAdbCommandToString(ADBcommandbox.Text).Output;
                FlexibleMessageBox.Show($"Ran adb command: ADB {ADBcommandbox.Text}, Output: {output}");
                ADBcommandbox.Visible = false;
                label9.Visible = false;
                label11.Visible = false;
                label2.Visible = false;
                gamesListView.Focus();
                Program.form.ChangeTitle("");
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                ADBcommandbox.Visible = false;
                label11.Visible = false;
                label9.Visible = false;
                label2.Visible = false;
                gamesListView.Focus();
            }
        }

        private void ADBcommandbox_Leave(object sender, EventArgs e)
        {

            label2.Visible = false;
            ADBcommandbox.Visible = false;
            label9.Visible = false;
            label11.Visible = false;
        }

        private void gamesQueListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.gamesQueListBox.SelectedItem == null) return;
            this.gamesQueListBox.DoDragDrop(this.gamesQueListBox.SelectedItem, DragDropEffects.Move);
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
            DialogResult dialogResult1 = FlexibleMessageBox.Show($"Do you want to extract {m_combo.SelectedItem.ToString()}'s apk and obb to a folder on your desktop now?", "Extract app?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
                return;
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
                ulong VersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                if (Directory.Exists($"{Properties.Settings.Default.MainDir}\\{packageName}"))
                    Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
                ProcessOutput output = new ProcessOutput("", "");
                ChangeTitle("Extracting APK....");

                Directory.CreateDirectory($"{Properties.Settings.Default.MainDir}\\{packageName}");

                Thread t1 = new Thread(() =>
                {
                    output = Sideloader.getApk(GameName);
                });
                t1.IsBackground = true;
                t1.Start();

                while (t1.IsAlive)
                    await Task.Delay(100);
                ChangeTitle("Extracting obb if it exists....");
                Thread t2 = new Thread(() =>
                {
                    output += ADB.RunAdbCommandToString($"pull \"/sdcard/Android/obb/{packageName}\" \"{Properties.Settings.Default.MainDir}\\{packageName}\"");
                });
                t2.IsBackground = true;
                t2.Start();

                while (t2.IsAlive)
                    await Task.Delay(100);

                if (File.Exists($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip"))
                    File.Delete($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip");

                string path = $"{Properties.Settings.Default.MainDir}\\7z.exe";
                string cmd = $"7z a -mx1 \"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip\" .\\{packageName}\\*";
                Program.form.ChangeTitle("Zipping extracted application...");
                Thread t3 = new Thread(() =>
                {
                    ADB.RunCommandToString(cmd, path);
                });
                t3.IsBackground = true;
                t3.Start();

                while (t3.IsAlive)
                    await Task.Delay(100);
                if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip"))
                    File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip");
                Directory.Move($"{Properties.Settings.Default.MainDir}\\{GameName} v{VersionInt} {packageName}.zip", $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{GameName} v{VersionInt} {packageName}.zip");
                Directory.Delete($"{Properties.Settings.Default.MainDir}\\{packageName}", true);
                Program.form.ChangeTitle("                                   \n\n");
                progressBar.Style = ProgressBarStyle.Continuous;
                FlexibleMessageBox.Show($"{GameName} pulled to:\n\n{GameName} v{VersionInt} {packageName}.zip\n\nOn your desktop!");
            }
        }
    }

    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(action), null);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
