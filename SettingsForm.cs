﻿using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
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
            CenterToParent();
            intSettings();
            intToolTips();
        }

        //Init form objects with values from settings
        private void intSettings()
        {
            checkForUpdatesCheckBox.Checked = Properties.Settings.Default.checkForUpdates;
            enableMessageBoxesCheckBox.Checked = Properties.Settings.Default.enableMessageBoxes;
            deleteAfterInstallCheckBox.Checked = Properties.Settings.Default.deleteAllAfterInstall;
            updateConfigCheckBox.Checked = Properties.Settings.Default.autoUpdateConfig;
            userJsonOnGameInstall.Checked = Properties.Settings.Default.userJsonOnGameInstall;
            nodevicemodeBox.Checked = Properties.Settings.Default.nodevicemode;
            bmbfBox.Checked = Properties.Settings.Default.BMBFchecked;
            AutoReinstBox.Checked = Properties.Settings.Default.AutoReinstall;

            if (Properties.Settings.Default.BandwidthLimit.Length > 1)
            {
                txtBandwidth.Text = Properties.Settings.Default.BandwidthLimit.Remove(Properties.Settings.Default.BandwidthLimit.Length - 1);
            }

        }

        private void intToolTips()
        {
            ToolTip checkForUpdatesToolTip = new ToolTip();
            checkForUpdatesToolTip.SetToolTip(checkForUpdatesCheckBox, "If this is checked, the software will check for available updates");
            ToolTip enableMessageBoxesToolTip = new ToolTip();
            enableMessageBoxesToolTip.SetToolTip(enableMessageBoxesCheckBox, "If this is checked, the software will display message boxes after every completed task");
            ToolTip deleteAfterInstallToolTip = new ToolTip();
            deleteAfterInstallToolTip.SetToolTip(deleteAfterInstallCheckBox, "If this is checked, the software will delete all game files after downloading and installing a game from a remote server");
        }

        public void btnUploadDebug_click(object sender, EventArgs e)
        {
            if (File.Exists($"{Properties.Settings.Default.CurrentLogPath}"))
            {
                string UUID = SideloaderUtilities.UUID();
                string debugLogPath = $"{Environment.CurrentDirectory}\\{UUID}.log";
                System.IO.File.Copy("debuglog.txt", debugLogPath);

                Clipboard.SetText(UUID);

                _ = RCLONE.runRcloneCommand_UploadConfig($"copy \"{debugLogPath}\" RSL-gameuploads:DebugLogs");
                _ = MessageBox.Show($"Your debug log has been copied to the server. ID: {UUID}");
            }
        }


        public void btnResetDebug_click(object sender, EventArgs e)
        {
            if (File.Exists($"{Properties.Settings.Default.CurrentLogPath}"))
            {
                File.Delete($"{Properties.Settings.Default.CurrentLogPath}");
            }

            if (File.Exists($"{Environment.CurrentDirectory}\\debuglog.txt"))
            {
                File.Delete($"{Environment.CurrentDirectory}\\debuglog.txt");
            }
        }

        //Apply settings
        private void applyButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BandwidthLimit = txtBandwidth.Text.Length > 0 && txtBandwidth.Text != "0" ? $"{txtBandwidth.Text.Replace(" ", "")}M" : "";

            Properties.Settings.Default.Save();
            _ = FlexibleMessageBox.Show("Settings applied!");
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

        private void updateConfigCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.autoUpdateConfig = updateConfigCheckBox.Checked;
        }

        private void userJsonOnGameInstall_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.userJsonOnGameInstall = userJsonOnGameInstall.Checked;
        }

        private void SettingsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                Close();
            }
        }

        private void SettingsForm_Leave(object sender, EventArgs e)
        {
            Close();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
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
                {
                    AutoReinstBox.Checked = false;
                }
            }

        }

        private void btnOpenDebug_Click(object sender, EventArgs e)
        {
            if (File.Exists($"{Environment.CurrentDirectory}\\debuglog.txt"))
            {
                _ = Process.Start($"{Environment.CurrentDirectory}\\debuglog.txt");
            }
        }
    }
}

