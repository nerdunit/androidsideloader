using JR.Utils.GUI.Forms;
using System;
using System.IO;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.CurrentLogName))
                textBox1.Text = Properties.Settings.Default.CurrentCrashName;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.CurrentLogPath))
                    DebugID.Text = Properties.Settings.Default.CurrentLogName;
            debuglogID.Text = "This is your DebugLogID. Click on your DebugLogID to copy it to your clipboard.";
            intSettings();
            intToolTips();
        }

        //Init form objects with values from settings
        private void intSettings()
        {
            CblindBox.Checked = Properties.Settings.Default.QblindOn;
            checkForUpdatesCheckBox.Checked = Properties.Settings.Default.checkForUpdates;
            enableMessageBoxesCheckBox.Checked = Properties.Settings.Default.enableMessageBoxes;
            deleteAfterInstallCheckBox.Checked = Properties.Settings.Default.deleteAllAfterInstall;
            userJsonOnGameInstall.Checked = Properties.Settings.Default.userJsonOnGameInstall;
            nodevicemodeBox.Checked = Properties.Settings.Default.nodevicemode;
            bmbfBox.Checked = Properties.Settings.Default.BMBFchecked;
            AutoReinstBox.Checked = Properties.Settings.Default.AutoReinstall;

            if (Properties.Settings.Default.BandwithLimit.Length > 1)
            {
                BandwithTextbox.Text = Properties.Settings.Default.BandwithLimit.Remove(Properties.Settings.Default.BandwithLimit.Length - 1);
                BandwithComboBox.Text = Properties.Settings.Default.BandwithLimit[Properties.Settings.Default.BandwithLimit.Length - 1].ToString();
            }

        }

        void intToolTips()
        {
            ToolTip checkForUpdatesToolTip = new ToolTip();
            checkForUpdatesToolTip.SetToolTip(this.checkForUpdatesCheckBox, "If this is checked, the software will check for available updates");
            ToolTip enableMessageBoxesToolTip = new ToolTip();
            enableMessageBoxesToolTip.SetToolTip(this.enableMessageBoxesCheckBox, "If this is checked, the software will display message boxes after every completed task");
            ToolTip deleteAfterInstallToolTip = new ToolTip();
            deleteAfterInstallToolTip.SetToolTip(this.deleteAfterInstallCheckBox, "If this is checked, the software will delete all game files after downloading and installing a game from a remote server");
        }





        public void DebugLogCopy_click(object sender, EventArgs e)
        {
            if (File.Exists($"{Properties.Settings.Default.CurrentLogPath}"))
            {
                RCLONE.runRcloneCommand($"copy \"{Properties.Settings.Default.CurrentLogPath}\" VRP-debuglogs:DebugLogs");
                Clipboard.SetText(DebugID.Text);
                MessageBox.Show($"Your debug log has been copied to the server. Please mention your DebugLog ID ({Properties.Settings.Default.CurrentLogName}) to the Mods (it has been automatically copied to your clipboard).");
            }
        }


        public void button1_click(object sender, EventArgs e)
        {

            if (File.Exists($"{Properties.Settings.Default.CurrentLogPath}"))
                File.Delete($"{Properties.Settings.Default.CurrentLogPath}");
            if (File.Exists($"{Environment.CurrentDirectory}\\debuglog.txt"))
                File.Delete($"{Environment.CurrentDirectory}\\debuglog.txt");


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
                Properties.Settings.Default.Save();
                DebugID.Text = combined;
                this.Close();
                SettingsForm Form = new SettingsForm();
                Form.Show();
            }

        }



        //Apply settings
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (BandwithTextbox.Text.Length > 0 && BandwithTextbox.Text != "0")
                if (BandwithComboBox.SelectedIndex == -1)
                {
                    FlexibleMessageBox.Show("You need to select something from the combobox");
                    return;
                }
                else
                {
                    Properties.Settings.Default.BandwithLimit = $"{BandwithTextbox.Text.Replace(" ", "")}{BandwithComboBox.Text}";
                }
            else
                Properties.Settings.Default.BandwithLimit = "";

            Properties.Settings.Default.Save();
            FlexibleMessageBox.Show("Settings applied!");
        }

        private void checkForUpdatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.checkForUpdates = checkForUpdatesCheckBox.Checked;
        }

        private void enableMessageBoxesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.enableMessageBoxes = enableMessageBoxesCheckBox.Checked;
        }

        private void resetSettingsButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            intSettings();
        }

        private void deleteAfterInstallCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.deleteAllAfterInstall = deleteAfterInstallCheckBox.Checked;
        }

        private void userJsonOnGameInstall_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.userJsonOnGameInstall = userJsonOnGameInstall.Checked;
        }

        private void SettingsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void SettingsForm_Leave(object sender, EventArgs e)
        {
            this.Close();
        }


        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void DebugID_Click(object sender, EventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.CurrentLogName + ".txt"))
                Clipboard.SetText(DebugID.Text);
            MessageBox.Show("DebugLogID copied to clipboard! Paste it to a moderator/helper for assistance!");
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.CurrentCrashName))
            {
                Clipboard.SetText(textBox1.Text);
                MessageBox.Show("CrashLogID copied to clipboard! Paste it to a moderator/helper for assistance!");
            }
        }

        private void CblindBox_CheckedChanged(object sender, EventArgs e)
        {

            Properties.Settings.Default.QblindOn = CblindBox.Checked;
            Properties.Settings.Default.Save();
            
        }

        private void CblindBox_Click(object sender, EventArgs e)
        {
            if (CblindBox.Checked)
                MessageBox.Show("You must restart Rookie's Sideloader OR click Refresh Updates List for changes to take effect.\n\nNOTE: Colors in the legend at the top right of the main window of Rookie won't update until you restart the program.");
        }

        private void nodevicemodeBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.nodevicemode = nodevicemodeBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void bmbfBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BMBFchecked = bmbfBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void AutoReinstBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoReinstall = AutoReinstBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void AutoReinstBox_Click(object sender, EventArgs e)
        {
            if (AutoReinstBox.Checked)
            {
                DialogResult dialogResult = FlexibleMessageBox.Show("WARNING: This box enables automatic reinstall when installs fail,\ndue to some games not allowing " +
                    "access to their save data (less than 5%) this\noption can lead to losing your progress." +
                    " However with this option\nchecked when installs fail you won't have to agree to a prompt to preform\nthe reinstall. " +
                    "(ideal when installing from a queue).\n\nNOTE: If your usb/wireless adb connection is extremely slow this option can\ncause larger" +
                    "apk file installations to fail. Enable anyway?", "WARNING", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                    AutoReinstBox.Checked = false;
            }

        }
    }
}

