using JR.Utils.GUI.Forms;
using Newtonsoft.Json;
using SergeUtils;
using Spoofer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;


namespace AndroidSideloader
{
    public partial class MainForm : Form
    {

        private ListViewColumnSorter lvwColumnSorter;

#if DEBUG
        public static bool debugMode = true;
        public bool DeviceConnected = false;
#else
        public static bool debugMode = false;
        public bool DeviceConnected = false;
#endif

        private bool isLoading = true;

        public MainForm()

        {
            InitializeComponent();
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 840000; // 14 mins between wakeup commands
            t.Tick += new EventHandler(timer_Tick);
            t.Start();
            lvwColumnSorter = new ListViewColumnSorter();
            this.gamesListView.ListViewItemSorter = lvwColumnSorter;
            if (searchTextBox.Visible)
                searchTextBox.Focus();

        }

        private string oldTitle = "";


        private async void Form1_Load(object sender, EventArgs e)
        {
            string adbFile = "C:\\RSL\\2.1.1\\adb\\adb.exe";
            string adbDir = "C:\\RSL\\2.1.1\\adb";
            string fileName = "";
            string destFile = "";
            string date_time = DateTime.Now.ToString("dddd, MMMM dd @ hh:mmtt (UTC)");
            Logger.Log($"\n\n##############\n##############\n##############\n\nAPP LAUNCH DATE/TIME: " + date_time + "\n\n##############\n##############\n##############\n\n");
            Properties.Settings.Default.MainDir = Environment.CurrentDirectory;
            Properties.Settings.Default.Save();

            if (!File.Exists(adbFile))
            {
                Directory.CreateDirectory(adbDir);
                if (System.IO.Directory.Exists($"{Environment.CurrentDirectory}\\adb"))
                {
                    string[] ADBfiles = System.IO.Directory.GetFiles($"{Properties.Settings.Default.MainDir}\\adb");

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in ADBfiles)
                    {
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(adbDir, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }

            }
            ADB.RunAdbCommandToString("kill-server");
            Properties.Settings.Default.ADBPath = adbFile;
            Properties.Settings.Default.Save();
            if (File.Exists(Sideloader.CrashLogPath))
            {
                DialogResult dialogResult = FlexibleMessageBox.Show(this, $"Sideloader crashed during your last use.\nPlease report issue @ https://github.com/nerdunit/androidsideloader/issues + send the crashlog to a mod or dev.\n\n(Crashlog.txt is located here: {Path.GetFullPath(Sideloader.CrashLogPath)})\n\nIf you've already sent it press YES to delete it and prevent this message. Press NO if you'd still like to send it.", "Crash Detected", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    File.Delete(Sideloader.CrashLogPath);
                else
                    Process.Start("explorer.exe", Properties.Settings.Default.MainDir);
            }
            CheckForInternet();

            if (HasInternet == true)
                Sideloader.downloadFiles();
            else
                FlexibleMessageBox.Show("Cannot connect to google dns, your internet may be down, won't use rclone or online features!");
            await Task.Delay(100);

            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);

            if (Directory.Exists(Sideloader.TempFolder))
            {
                Directory.Delete(Sideloader.TempFolder, true);
                Directory.CreateDirectory(Sideloader.TempFolder);
            }

            //Delete the Debug file if it is more than 5MB
            if (File.Exists(Logger.logfile))
            {
                long length = new System.IO.FileInfo(Logger.logfile).Length;
                if (length > 5000000) File.Delete(Logger.logfile);
            }

            RCLONE.Init();
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
            gamesListView.GridLines = true;
            etaLabel.Text = "";
            speedLabel.Text = "";
            diskLabel.Text = "";

            try
            {
                ADB.WakeDevice();
                await CheckForDevice();
                ChangeTitlebarToDevice();

            }
            catch { }
        }


        private async void Form1_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(10000);
                freeDisclaimer.Invoke(() => { freeDisclaimer.Dispose(); });
            }).Start();

            Thread t1 = new Thread(() =>
            {
                if (!debugMode && Properties.Settings.Default.checkForUpdates)
                {
                    Updater.AppName = "AndroidSideloader";
                    Updater.Repostory = "nerdunit/androidsideloader";
                    Updater.Update();
                }
                progressBar.Invoke(() => { progressBar.Style = ProgressBarStyle.Marquee; });
                ChangeTitle("Initializing Mirrors");
                initMirrors(true);
                ChangeTitle("Initializing Games");
                SideloaderRCLONE.initGames(currentRemote);
                if (!Directory.Exists(SideloaderRCLONE.ThumbnailsFolder) || !Directory.Exists(SideloaderRCLONE.NotesFolder))
                {
                    MessageBox.Show("It seems you are missing the thumbnails and/or notes database, the first start of the sideloader takes a bit more time, so dont worry if it looks stuck!");
                }
                ChangeTitle("Syncing Game Photos");
                SideloaderRCLONE.UpdateGamePhotos(currentRemote);
                ChangeTitle("Checking for Updates on server...");
                SideloaderRCLONE.UpdateGameNotes(currentRemote);
                listappsbtn();
            });
            t1.SetApartmentState(ApartmentState.STA);
            t1.IsBackground = false;
            if (HasInternet)
                t1.Start();

            showAvailableSpace();

            intToolTips();

            while (t1.IsAlive)
                await Task.Delay(100);
            ChangeTitle("GREEN = Up to date, ORANGE = Out of date - Checking installed app versions,  please wait...");

            initListView();
            ChangeTitle("Loaded");
            downloadInstallGameButton.Enabled = true;

            progressBar.Style = ProgressBarStyle.Continuous;
            isLoading = false;
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
            sideloadFolderToolTip.SetToolTip(this.sideloadFolderButton, "Sideloads every apk from a folder");
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

        public async void ChangeTitle(string txt, bool reset = true)
        {
            this.Invoke(() => { oldTitle = txt; this.Text = "Rookie's Sideloader | " + txt; });
            ProgressText.Invoke(() => { ProgressText.Text = txt; });
            if (!reset)
                return;
            await Task.Delay(TimeSpan.FromSeconds(5));
            this.Invoke(() => { this.Text = "Rookie's Sideloader | " + oldTitle; });
            ProgressText.Invoke(() => { ProgressText.Text = oldTitle; });
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
            if (Properties.Settings.Default.IPAddress.Contains("connect"))
            {
                ADB.RunAdbCommandToString(Properties.Settings.Default.IPAddress);
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
            ADB.DeviceID = GetDeviceID();
            Thread t1 = new Thread(() =>
            {
            output = ADB.RunAdbCommandToString("devices").Output;
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
                Title = "Select your obb folder"
            };
            if (Properties.Settings.Default.IPAddress.Contains("connect"))
                ADB.WakeDevice();
            if (dialog.Show(Handle))
            {
                Thread t1 = new Thread(() =>
                {
                    output += ADB.CopyOBB(dialog.FileName);
                });
                t1.IsBackground = true;
                t1.Start();

                while (t1.IsAlive)
                    await Task.Delay(100);

                showAvailableSpace();

                ShowPrcOutput(output);
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
                        DialogResult dialogResult = MessageBox.Show("Device not authorized, be sure to authorize computer on device.", "Not Authorized", MessageBoxButtons.RetryCancel);
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
                        DialogResult dialogResult = MessageBox.Show("No device found. Please ensure the following: \n\n -Developer mode is enabled. \n -ADB drivers are installed. \n -ADB connection is enabled on your device (this can reset). \n -Your device is plugged in.\n\nThen press \"Retry\"", "No device found.", MessageBoxButtons.RetryCancel);
                        if (dialogResult == DialogResult.Retry)
                        {
                            devicesbutton.PerformClick();
                        }
                        else
                        {
                            return;
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
                MessageBox.Show($"This may take up to a minute. Backing up gamesaves to Documents\\Rookie Backups\\{date_str} (year.month.date)");
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
            ADB.WakeDevice();
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
                    if (!Sideloader.InstalledPackages.ContainsKey(line[i]))
                        Sideloader.InstalledPackages.Add(line[i], "");
                    foreach (var game in SideloaderRCLONE.games)
                        if (line[i].Length > 0 && game[3].Contains(line[i]))
                            line[i] = game[0];
                }
            }

            File.WriteAllText("installedPackages.json", JsonConvert.SerializeObject(Sideloader.InstalledPackages));

            Array.Sort(line);

            foreach (string game in line)
            {
                if (game.Length > 0)
                    m_combo.Invoke(() => { m_combo.Items.Add(game); });
            }

            m_combo.Invoke(() => { m_combo.MatchingMethod = StringMatchingMethod.NoWildcards; });
        }

        private async void getApkButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

            if (m_combo.SelectedIndex == -1)
            {
                notify("Please select an app first");
                return;
            }
            progressBar.Style = ProgressBarStyle.Marquee;

            string GameName = m_combo.SelectedItem.ToString();
            ProcessOutput output = new ProcessOutput("", "");
            ChangeTitle("Extracting APK....");
            Thread t1 = new Thread(() =>
            {
                output = Sideloader.getApk(GameName);
            });
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
                await Task.Delay(100);
            progressBar.Style = ProgressBarStyle.Continuous;
            ChangeTitle("APK Extracted to " + Properties.Settings.Default.MainDir +". Opening folder now.");
            Process.Start("explorer.exe", Properties.Settings.Default.MainDir);
            ShowPrcOutput(output);
        }

        private async void uninstallAppButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            if (m_combo.SelectedIndex == -1)
            {
                FlexibleMessageBox.Show("Please select an app first");
                return;
            }
            string date_str = DateTime.Today.ToString("yyyy.MM.dd");
            string CurrBackups = Path.Combine(BackupFolder, date_str);


            string GameName = m_combo.SelectedItem.ToString();
            string packagename = Sideloader.gameNameToPackageName(GameName);
            MessageBox.Show($"If savedata is found it will be saved to Documents\\Rookie Backups\\{date_str}(YYYY.MM.DD)\\{packagename}", "Attempting Backup...", MessageBoxButtons.OK);

            Directory.CreateDirectory(CurrBackups);
            ADB.RunAdbCommandToString($"pull \"/sdcard/Android/data/{packagename}\" \"{CurrBackups}\"");
            DialogResult dialogResult = MessageBox.Show($"Please check to see if we automatically found savedata in Documents\\Rookie Backups.\nIf there are no new files there is recommended that you do a full backup via Backup Gamedata before continuing.\nNOTE: Some games do not allow backup of savedata.\nContinue with the uninstall?", "Check for backup.", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }

            ProcessOutput output = new ProcessOutput("", "");
            progressBar.Style = ProgressBarStyle.Marquee;
            Thread t1 = new Thread(() =>
            {
                output += Sideloader.UninstallGame(GameName);
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






        private async void sideloadFolderButton_Click(object sender, EventArgs e)
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
                    Sideloader.RecursiveSideload(dialog.FileName);
                });
                t1.IsBackground = true;
                t1.Start();
                while (t1.IsAlive)
                    await Task.Delay(100);
                showAvailableSpace();

                ShowPrcOutput(Sideloader.RecursiveOutput);
            }
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
            ProcessOutput output = new ProcessOutput("", "");
            ADB.WakeDevice();
            ADB.DeviceID = GetDeviceID();
            progressBar.Style = ProgressBarStyle.Marquee;
            Thread t1 = new Thread(() =>

            {
                string[] datas = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string data in datas)
                {
                    string directory = Path.GetDirectoryName(data);
                    //if is directory
                    string dir = Path.GetDirectoryName(data);
                    string file = $"{dir}\\Install.txt";
                    if (Directory.Exists(data))
                    {
                        output += ADB.CopyOBB(data);
                        string extension = Path.GetExtension(data);
                        if (extension == ".apk")
                        {

                            if (File.Exists($"{Environment.CurrentDirectory}\\Install.txt"))
                            {


                                DialogResult dialogResult = MessageBox.Show("Special instructions have been found with this file, would you like to run them automatically?", "Special Instructions found!", MessageBoxButtons.OKCancel);
                                if (dialogResult == DialogResult.Cancel)
                                    return;
                                else
                                    ChangeTitle("Sideloading custom install.txt automatically.");
                                output += Sideloader.RunADBCommandsFromFile(file);
                                if (output.Error.Contains("mkdir"))
                                    output.Error = "";
                                if (output.Error.Contains("reserved"))
                                    output.Output = "";
                                ChangeTitle("Done.");
                            }
                        }
                        string[] files = Directory.GetFiles(data);
                        foreach (string file2 in files)
                        {
                            if (File.Exists(file2))
                                if (file2.EndsWith(".apk"))
                                {
                                    output += ADB.Sideload(file2);
                                }
                        }
                        string[] folders = Directory.GetDirectories(data);
                        foreach (string folder in folders)
                        {
                            output += ADB.CopyOBB(folder);
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
                                file = data;
                                DialogResult dialogResult = MessageBox.Show("Special instructions have been found with this file, would you like to run them automatically?", "Special Instructions found!", MessageBoxButtons.OKCancel);
                                if (dialogResult == DialogResult.Cancel)
                                    return;
                                else
                                {
                                    ChangeTitle("Sideloading custom install.txt automatically.");
                                    output += Sideloader.RunADBCommandsFromFile(file);
                                    ChangeTitle("Done.");
                                }
                            }
                            else
                            {
                                output += ADB.Sideload(data);
                            }
                        }
                        else if (extension == ".obb")
                        {
                            string filename = Path.GetFileName(data);
                            string foldername = filename.Substring(filename.IndexOf('.') + 1);
                            foldername = foldername.Substring(foldername.IndexOf('.') + 1);
                            foldername = foldername.Replace(".obb", "");
                            foldername = Environment.CurrentDirectory + "\\" + foldername;
                            Directory.CreateDirectory(foldername);
                            File.Copy(data, foldername + "\\" + filename);
                            output += ADB.CopyOBB(foldername);
                            Properties.Settings.Default.CurrPckg = foldername;
                            Properties.Settings.Default.Save();
                            Directory.Delete(foldername, true);
                        }

                        if (extension == ".txt")
                        {
                            ChangeTitle("Sideloading custom install.txt automatically.");
                            output += Sideloader.RunADBCommandsFromFile(file);
                            ChangeTitle("Done.");
                        }
                    }
                }
            });
            t1.IsBackground = true;
            t1.Start();

            while (t1.IsAlive)
                await Task.Delay(100);

            progressBar.Style = ProgressBarStyle.Continuous;

            showAvailableSpace();

            DragDropLbl.Visible = false;

            ShowPrcOutput(output);
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
        }
        private void initListView()
        {
            gamesListView.Items.Clear();
            gamesListView.Columns.Clear();
            if (!File.Exists("installedPackages.json"))
                File.Create("installedPackages.json");
            if (File.Exists("instlledPackages.json"))
                Sideloader.InstalledPackages = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("installedPackages.json"));
            foreach (string column in SideloaderRCLONE.gameProperties)
            {
                gamesListView.Columns.Add(column, 150);
            }

            List<ListViewItem> GameList = new List<ListViewItem>();
            foreach (string[] release in SideloaderRCLONE.games)
            {
                ListViewItem Game = new ListViewItem(release);

                string lines = Properties.Settings.Default.InstalledApps;
                string pattern = "package:";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(lines, replacement);
                char[] delims = new[] { '\r', '\n' };
                string[] strings = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                //MessageBox.Show(result);

                foreach (string packagename in strings)
                {
                    if (string.Equals(release[SideloaderRCLONE.PackageNameIndex], packagename))
                    {
                        Game.BackColor = Color.Green;
                        string InstalledVersionCode;
                        if (Sideloader.InstalledPackages.ContainsKey(packagename) && Sideloader.InstalledPackages[packagename] != "")
                        {
                            InstalledVersionCode = Sideloader.InstalledPackages[packagename];
                        }
                        else
                        {
                            InstalledVersionCode = ADB.RunAdbCommandToString($"shell \"dumpsys package {packagename} | grep versionCode -F\"").Output;

                            InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingBeforeFirst(InstalledVersionCode, "versionCode=");
                            InstalledVersionCode = Utilities.StringUtilities.RemoveEverythingAfterFirst(InstalledVersionCode, " ");
                            Sideloader.InstalledPackages[packagename] = InstalledVersionCode;
                        }

                        try
                        {
                            ulong installedVersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(InstalledVersionCode));
                            ulong cloudVersionInt = UInt64.Parse(Utilities.StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex]));

                            //Logger.Log($"Checked game {release[SideloaderRCLONE.GameNameIndex]}; cloudversion={cloudVersionInt} localversion={installedVersionInt}");
                            if (installedVersionInt < cloudVersionInt)
                            {
                                Game.BackColor = Color.FromArgb(102, 77, 0);
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
                File.WriteAllText("installedPackages.json", JsonConvert.SerializeObject(Sideloader.InstalledPackages));
                GameList.Add(Game);
            }

            ListViewItem[] arr = GameList.ToArray();
            gamesListView.BeginUpdate();
            gamesListView.Items.AddRange(arr);
            gamesListView.EndUpdate();
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
                    remotesList.Invoke(() => { remotesList.Items.Add(mirror.Remove(mirror.Length - 1)); });
                    itemsCount++;
                }
            }

            if (itemsCount > 0)
            {
                var rand = new Random();
                if (random == true && index < itemsCount)
                    index = rand.Next(0, itemsCount);
                remotesList.Invoke(() =>
                {
                    remotesList.SelectedIndex = index;
                    currentRemote = remotesList.SelectedItem.ToString();
                });
            }
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
            string about = $@"Finally {Updater.LocalVersion}, with new version comming Soon™
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

 - HFP Thanks to: Roma/Rookie, Pmow, Flow, Kaladin, and the mod staff!";

            FlexibleMessageBox.Show(about);
        }

        private async void ADBWirelessEnable_Click(object sender, EventArgs e)
        {

            ADB.WakeDevice();
            DialogResult dialogResult = MessageBox.Show("Make sure your Quest is plugged in VIA USB then press OK, if you need a moment press Cancel and come back when you're ready.", "Connect Quest now.", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel)
                return;

            ADB.RunAdbCommandToString("devices");
            ADB.RunAdbCommandToString("tcpip 5555");

            MessageBox.Show("Press OK to get your Quest's local IP address.", "Obtain local IP address", MessageBoxButtons.OKCancel);
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
                MessageBox.Show($"Your Quest's local IP address is: {IPaddr}\n\nPlease disconnect your Quest then wait 2 seconds.\nOnce it is disconnected hit OK", "", MessageBoxButtons.OK);
                Thread.Sleep(2000);
                ADB.RunAdbCommandToString(IPcmnd);
                await Program.form.CheckForDevice();
                Program.form.ChangeTitlebarToDevice();
                Program.form.showAvailableSpace();
                Properties.Settings.Default.IPAddress = IPcmnd;
                Properties.Settings.Default.Save();

                MessageBox.Show($"Connected! We can now automatically disable the Quest wifi chip from falling asleep. This makes it so Rookie can work wirelessly even if the device has entered \"sleep mode\". This setting is NOT permanent and resets upon Quest reboot, just like wireless ADB functionality.\n\nNOTE: This may cause the device battery to drain while it is in sleep mode at a very slightly increased rate. We recommend this setting for the majority of users for ease of use purposes. If you click NO you must keep your Quest connected to a charger or wake your device and then put it back on hold before using Rookie wirelessly. Do you want us to stop sleep mode from disabling wireless ADB?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                    ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
                }

                ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
                ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
            }
            else
                MessageBox.Show("No device connected!");

        }

    private async void listApkButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();

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
            t1.IsBackground = true;
            t1.Start();
            while (t1.IsAlive)
                await Task.Delay(100);
            initListView();
            progressBar.Style = ProgressBarStyle.Continuous;
            isLoading = false;
        }

        private static readonly HttpClient client = new HttpClient();

        private bool updatedConfig = false;

        private bool gamesAreDownloading = false;
        private List<string> gamesQueueList = new List<string>();
        private int quotaTries = 0;

        public void SwitchMirrors()
        {
            quotaTries++;
            remotesList.Invoke(() =>
            {
                if (quotaTries > remotesList.Items.Count)
                {
                    FlexibleMessageBox.Show("Quota reached for all mirrors exiting program...");
                    Application.Exit();
                }
                if (remotesList.Items.Count > remotesList.SelectedIndex)
                    remotesList.SelectedIndex++;
                else
                    remotesList.SelectedIndex = 0;
            });
        }

        private async void downloadInstallGameButton_Click(object sender, EventArgs e)
        {
            ChangeTitle("Checking filesize...");
            long selectedGamesSize = 0;
            int count = 0;
            string[] GameSizeGame = new string[1];
            if (gamesListView.SelectedItems.Count > 0)
            {
                count = gamesListView.SelectedItems.Count;
                GameSizeGame = new string[count];
                for (int i = 0; i < count; i++)
                    GameSizeGame[i] = gamesListView.SelectedItems[i].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
            }
            else return;

            bool HadError = false;
            Thread gameSizeThread = new Thread(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    selectedGamesSize += SideloaderRCLONE.GetFolderSize(GameSizeGame[i], currentRemote);
                    if (selectedGamesSize == 0)
                    {
                        FlexibleMessageBox.Show($"Couldnt find release {GameSizeGame[i]} on rclone, please deselect and try again or switch mirrors");
                        HadError = true;
                        return;
                    }
                }

            });
            gameSizeThread.Start();

            while (gameSizeThread.IsAlive)
                await Task.Delay(100);

            if (HadError)
                return;

            string game;
            if (GameSizeGame.Length == 1)
            {
                game = $"\"{GameSizeGame[0]}\"";
            } else
            {
                game = "the selected games";
            }
            DialogResult dialogResult = FlexibleMessageBox.Show($"Are you sure you want to download {game}? The size is {String.Format("{0:0.00}", (double)selectedGamesSize)} MB", "Are you sure?", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes)
            {
                ChangeTitle("");
                return;
            }
            //Add games to the queue
            for (int i = 0; i < gamesListView.SelectedItems.Count; i++)
                gamesQueueList.Add(gamesListView.SelectedItems[i].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text);
            gamesQueListBox.DataSource = null;
            gamesQueListBox.DataSource = gamesQueueList;

            if (gamesAreDownloading)
                return;
            gamesAreDownloading = true;

            if (updatedConfig == false && Properties.Settings.Default.autoUpdateConfig == true) //check for config only once per program open and if setting enabled
            {
                updatedConfig = true;
                ChangeTitle("Checking if config is updated and updating config");
                progressBar.Style = ProgressBarStyle.Marquee;
                await Task.Run(() => SideloaderRCLONE.updateConfig(currentRemote));
                progressBar.Style = ProgressBarStyle.Continuous;
            }

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
                            try { progressBar.Value = Convert.ToInt32((((double)downloaded / (double)allSize) * 100)); } catch { }

                            i++;
                            downloadSpeed /= 1000000;
                            if (i == 4)
                            {
                                i = 0;
                                float seconds = (allSize - downloaded) / downloadSpeed;
                                TimeSpan time = TimeSpan.FromSeconds(seconds);
                                etaLabel.Text = "ETA: " + time.ToString(@"hh\:mm\:ss") + " left";
                            }

                            speedLabel.Text = "DLS: " + String.Format("{0:0.00}", downloadSpeed) + " mbps";
                        }
                    }
                    catch { }

                    await Task.Delay(1000);


                }

                //Quota Errors
                bool quotaError = false;
                if (gameDownloadOutput.Error.Length > 0)
                {
                    string err = gameDownloadOutput.Error.ToLower();
                    if (err.Contains("quota") && err.Contains("exceeded"))
                    {
                        FlexibleMessageBox.Show("The download Quota has been reached for this mirror, trying to switch mirrors...");
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
                    progressBar.Style = ProgressBarStyle.Marquee;
                    ChangeTitle("Installing game apk " + gameName, false);
                    etaLabel.Text = "ETA: Wait for install...";
                    speedLabel.Text = "DLS: Done downloading";

                    string[] files = Directory.GetFiles(Environment.CurrentDirectory + "\\" + gameName);

                    Debug.WriteLine("Game Folder is: " + Environment.CurrentDirectory + "\\" + gameName);
                    Debug.WriteLine("FILES IN GAME FOLDER: ");
                    foreach (string file in files)
                    {
                        Debug.WriteLine(file);
                        string extension = Path.GetExtension(file);
                        if (extension == ".apk")
                        {
                            Thread apkThread = new Thread(() =>
                            {
                                ADB.WakeDevice();
                                output += ADB.Sideload(file, packagename);
                            });

                            apkThread.Start();
                            while (apkThread.IsAlive)
                                await Task.Delay(100);
                        }
                        if (extension == ".txt")
                        {
                            string fullname = Path.GetFileName(file);
                            if (fullname.Equals("install.txt") || fullname.Equals("Install.txt"))
                            {
                                Thread installtxtThread = new Thread(() =>
                                {
                                    ADB.WakeDevice();
                                    output += Sideloader.RunADBCommandsFromFile(file);
                                    ChangeTitle("Sideloading custom install.txt automatically.");
                                });

                                installtxtThread.Start();
                                while (installtxtThread.IsAlive)
                                    await Task.Delay(100);
                            }
                        }
                    }
                    

                    Debug.WriteLine(wrDelimiter);
                    ADB.WakeDevice();
                    string[] folders = Directory.GetDirectories(Environment.CurrentDirectory + "\\" + gameName);

                    foreach (string folder in folders)
                    {
                        ChangeTitle("Installing game obb " + gameName, false);
                        string[] obbs = Directory.GetFiles(folder);

                        foreach (string currObb in obbs)
                        {
                            Thread obbThread = new Thread(() =>
                            {
                                output += ADB.CopyOBB(folder);
                            });
                            obbThread.IsBackground = true;
                            obbThread.Start();

                            while (obbThread.IsAlive)
                                await Task.Delay(100);
                        }
                    }

                    if (Properties.Settings.Default.deleteAllAfterInstall)
                    {
                        ChangeTitle("Deleting game files", false);
                        try { Directory.Delete(Environment.CurrentDirectory + "\\" + gameName, true); } catch (Exception ex) { MessageBox.Show($"Error deleting game files: {ex.Message}"); }
                    }

                    //Remove current game
                    gamesQueueList.RemoveAt(0);
                    gamesQueListBox.DataSource = null;
                    gamesQueListBox.DataSource = gamesQueueList;
                    ChangeTitlebarToDevice();
                    showAvailableSpace();
                }
            }
            progressBar.Style = ProgressBarStyle.Continuous;
            etaLabel.Text = "ETA: Finished Queue";
            speedLabel.Text = "DLS: Finished Queue";
            ProgressText.Text = "";
            await CheckForDevice();
            ChangeTitlebarToDevice();
            gamesAreDownloading = false;
            ShowPrcOutput(output);
            listappsbtn();
            initListView();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RCLONE.killRclone();
            ADB.RunAdbCommandToString("kill-server");
        }

        private void ADBWirelessDisable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete your saved Quest IP address/command?", "Remove saved IP address?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("Saved IP data reset cancelled.");
                return;
            }
            else
            {
                ADB.WakeDevice();
                MessageBox.Show("Make sure your device is not connected to USB and press OK.");
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
                MessageBox.Show("Relaunch Rookie to complete the process and switch back to USB adb.");
            }

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
            remotesList.Invoke(() => { currentRemote = remotesList.SelectedItem.ToString(); });
            if (remotesList.Text.Contains("VRP"))
            {
                string lines = remotesList.Text;
                string pattern = "VRP-mirror";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(lines, replacement);
                char[] delims = new[] { '\r', '\n' };
                string[] strings = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                remotesList.Text = result;
            }
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
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.gamesListView.Sort();
        }

        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                searchTextBox.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                searchTextBox.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
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
            if (keyData == (Keys.Control | Keys.P))
            {
                DialogResult dialogResult = MessageBox.Show("Do you wish to copy Package Name of games selected from list to clipboard?", "Copy package to clipboard?", MessageBoxButtons.YesNo);
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
                ListViewItem foundItem = gamesListView.FindItemWithText(searchTextBox.Text, true, 0, true);
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

                    if (searchTextBox.Text.Length == 0)
                          gamesListView.SelectedItems.Clear();

                    searchTextBox.Focus();
                    this.searchTextBox.KeyPress += new
                    System.Windows.Forms.KeyPressEventHandler(CheckEnter);
                }
                else
                    gamesListView.SelectedItems.Clear();
            }
        }
    

        public void gamesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (gamesListView.SelectedItems.Count < 1)
                return;
            string CurrentPackageName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.PackageNameIndex].Text;
            string CurrentReleaseName = gamesListView.SelectedItems[gamesListView.SelectedItems.Count - 1].SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;

            if(Properties.Settings.Default.PackageNameToCB)
            Clipboard.SetText(CurrentPackageName);

            string ImagePath = $"{SideloaderRCLONE.ThumbnailsFolder}\\{CurrentPackageName}.jpg";
            string NotePath = $"{SideloaderRCLONE.NotesFolder}\\{CurrentReleaseName}.txt";
            
            if (gamesPictureBox.BackgroundImage != null)
                gamesPictureBox.BackgroundImage.Dispose();
            if (File.Exists(ImagePath))
                gamesPictureBox.BackgroundImage = Image.FromFile(ImagePath);
            else
                gamesPictureBox.BackgroundImage = new Bitmap(360, 203);

            if (File.Exists(NotePath))
                notesRichTextBox.Text = File.ReadAllText(NotePath);
            else
                notesRichTextBox.Text = "";
        }

        private void UpdateGamesButton_Click(object sender, EventArgs e)
        {
            ADB.WakeDevice();
            GetDeviceID();
            MessageBox.Show("If your device is not Connected, hit reconnect first or it won't work!\nNOTE: THIS MAY TAKE UP TO 60 SECONDS.\nThere will be a Popup text window with all updates available when it is done!", "Is device connected?", MessageBoxButtons.OKCancel);
            listappsbtn();
            initListView();

            if (SideloaderRCLONE.games.Count<1)
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

        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            if (searchTextBox.Visible)
                searchTextBox.Focus();
            else
                gamesListView.Focus();
        }

        private void gamesListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (gamesListView.SelectedItems.Count > 0)
                    downloadInstallGameButton_Click(sender, e);
            }
            }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (gamesListView.SelectedItems.Count > 0)
                    downloadInstallGameButton_Click(sender, e);
            }
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
