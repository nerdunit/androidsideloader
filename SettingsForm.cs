using AndroidSideloader.Properties;
using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class SettingsForm : Form
    {
        private static readonly SettingsManager _settings = SettingsManager.Instance;

        public SettingsForm()
        {
            InitializeComponent();

            // Use same icon as the executable
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            CenterToParent();
            initSettings();
            initToolTips();
        }

        private void initSettings()
        {
            // Use SetCheckedSilent to avoid triggering events during initialization
            toggleCheckForUpdates.SetCheckedSilent(_settings.CheckForUpdates);
            toggleMessageBoxes.SetCheckedSilent(_settings.EnableMessageBoxes);
            toggleDeleteAfterInstall.SetCheckedSilent(_settings.DeleteAllAfterInstall);
            toggleUserJson.SetCheckedSilent(_settings.UserJsonOnGameInstall);
            toggleNoDeviceMode.SetCheckedSilent(_settings.NodeviceMode);
            toggleBMBF.SetCheckedSilent(_settings.BMBFChecked);
            toggleAutoReinstall.SetCheckedSilent(_settings.AutoReinstall);
            toggleSingleThread.SetCheckedSilent(_settings.SingleThreadMode);
            toggleVirtualFilesystem.SetCheckedSilent(_settings.VirtualFilesystemCompatibility);
            toggleUseDownloadedFiles.SetCheckedSilent(_settings.UseDownloadedFiles);
            toggleTrailers.SetCheckedSilent(_settings.TrailersEnabled);
            toggleShowAdultContent.SetCheckedSilent(_settings.ShowAdultContent);
            bandwidthLimitTextBox.Text = _settings.BandwidthLimit.ToString();

            // Handle no device mode disabling delete after install
            if (toggleNoDeviceMode.Checked)
            {
                toggleDeleteAfterInstall.SetCheckedSilent(false);
                toggleDeleteAfterInstall.Enabled = false;
                lblDeleteAfterInstall.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            }

            toggleProxy.Checked = _settings.useProxy;
            proxyAddressTextBox.Text = _settings.ProxyAddress;
            proxyPortTextBox.Text = _settings.ProxyPort;
        }

        private void initToolTips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(toggleCheckForUpdates, "Check for available application updates on startup");
            toolTip.SetToolTip(lblCheckForUpdates, "Check for available application updates on startup");
            toolTip.SetToolTip(toggleMessageBoxes, "Show message boxes after every completed task");
            toolTip.SetToolTip(lblMessageBoxes, "Show message boxes after every completed task");
            toolTip.SetToolTip(toggleDeleteAfterInstall, "Delete game files after downloading and installing");
            toolTip.SetToolTip(lblDeleteAfterInstall, "Delete game files after downloading and installing");
            toolTip.SetToolTip(toggleUseDownloadedFiles, "Always install downloaded files without prompting to re-download");
            toolTip.SetToolTip(lblUseDownloadedFiles, "Always install downloaded files without prompting to re-download");
            toolTip.SetToolTip(toggleTrailers, "Show game trailers when selecting a game");
            toolTip.SetToolTip(lblTrailers, "Show game trailers when selecting a game");
        }

        private void SaveAllSettings()
        {
            string input = bandwidthLimitTextBox.Text;
            Regex regex = new Regex(@"^\d+(\.\d+)?$");

            if (regex.IsMatch(input) && float.TryParse(input, out float bandwidthLimit))
            {
                _settings.BandwidthLimit = bandwidthLimit;
            }

            _settings.CheckForUpdates = toggleCheckForUpdates.Checked;
            _settings.EnableMessageBoxes = toggleMessageBoxes.Checked;
            _settings.DeleteAllAfterInstall = toggleDeleteAfterInstall.Checked;
            _settings.UserJsonOnGameInstall = toggleUserJson.Checked;
            _settings.NodeviceMode = toggleNoDeviceMode.Checked;
            _settings.BMBFChecked = toggleBMBF.Checked;
            _settings.AutoReinstall = toggleAutoReinstall.Checked;
            _settings.SingleThreadMode = toggleSingleThread.Checked;
            _settings.VirtualFilesystemCompatibility = toggleVirtualFilesystem.Checked;
            _settings.UseDownloadedFiles = toggleUseDownloadedFiles.Checked;
            _settings.TrailersEnabled = toggleTrailers.Checked;
            _settings.ShowAdultContent = toggleShowAdultContent.Checked;
            _settings.useProxy = toggleProxy.Checked;

            if (Program.form != null)
            {
                Program.form.SetTrailerVisibility(toggleTrailers.Checked);
                Program.form.UpdateSideloadingUI();
                Program.form.RefreshAdultContentFilter();
            }

            _settings.Save();
        }

        public void btnUploadDebug_click(object sender, EventArgs e)
        {
            if (File.Exists($"{_settings.CurrentLogPath}"))
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
            if (File.Exists($"{_settings.CurrentLogPath}"))
            {
                File.Delete($"{_settings.CurrentLogPath}");
            }

            if (File.Exists($"{Environment.CurrentDirectory}\\debuglog.txt"))
            {
                File.Delete($"{Environment.CurrentDirectory}\\debuglog.txt");
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Parse bandwidth value
            var bandwidthInput = bandwidthLimitTextBox.Text;
            Regex regex = new Regex(@"^\d+(\.\d+)?$");

            if (regex.IsMatch(bandwidthInput) && float.TryParse(bandwidthInput, out float bandwidthLimit))
            {
                _settings.BandwidthLimit = bandwidthLimit;
            }
            else
            {
                MessageBox.Show("Please enter a valid number for the bandwidth limit.");
                return;
            }

            // Parse proxy values if proxy is enabled
            if (toggleProxy.Checked)
            {
                // Parse proxy address
                var proxyAddressInput = proxyAddressTextBox.Text;

                if (proxyAddressInput.StartsWith("http://"))
                {
                    proxyAddressInput = proxyAddressInput.Substring("http://".Length);
                }
                else if (proxyAddressInput.StartsWith("https://"))
                {
                    proxyAddressInput = proxyAddressInput.Substring("https://".Length);
                }

                if (proxyAddressInput.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                    IPAddress.TryParse(proxyAddressInput, out _))
                {
                    _settings.ProxyAddress = proxyAddressInput;
                }
                else
                {
                    MessageBox.Show("Please enter a valid address for the proxy.");
                }

                // Parse proxy port
                var proxyPortInput = proxyPortTextBox.Text;

                if (ushort.TryParse(proxyPortInput, out _))
                {
                    _settings.ProxyPort = proxyPortInput;
                }
                else
                {
                    MessageBox.Show("Please enter a valid port for the proxy.");
                }
            }

            SaveAllSettings();
            this.Close();
        }

        private void resetSettingsButton_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void toggleNoDeviceMode_CheckedChanged(object sender, EventArgs e)
        {
            // Update UI state only - settings saved on form close
            if (!toggleNoDeviceMode.Checked)
            {
                toggleDeleteAfterInstall.Checked = true;
                toggleDeleteAfterInstall.Enabled = true;
                lblDeleteAfterInstall.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                toggleDeleteAfterInstall.SetCheckedSilent(false);
                toggleDeleteAfterInstall.Enabled = false;
                lblDeleteAfterInstall.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            }
        }

        private void toggleAutoReinstall_Click(object sender, EventArgs e)
        {
            if (toggleAutoReinstall.Checked)
            {
                DialogResult dialogResult = FlexibleMessageBox.Show(this,
                    "WARNING: This enables automatic reinstall when installs fail.\n\n" +
                    "Some games (less than 5%) don't allow access to their save data, " +
                    "which can lead to losing your progress.\n\n" +
                    "However, with this option enabled, you won't have to confirm reinstalls manually " +
                    "(ideal for queue installations).\n\n" +
                    "NOTE: If your USB/wireless ADB connection is slow, this may cause " +
                    "larger APK installations to fail.\n\nEnable anyway?",
                    "WARNING", MessageBoxButtons.OKCancel);

                if (dialogResult == DialogResult.Cancel)
                {
                    toggleAutoReinstall.SetCheckedSilent(false);
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

        private void setDownloadDirectory_Click(object sender, EventArgs e)
        {
            var dialog = new FolderSelectDialog
            {
                Title = "Select Download Folder",
                InitialDirectory = _settings.CustomDownloadDir && Directory.Exists(_settings.DownloadDir)
                    ? _settings.DownloadDir
                    : Environment.CurrentDirectory
            };

            if (dialog.Show(this.Handle))
            {
                _settings.CustomDownloadDir = true;
                _settings.DownloadDir = dialog.FileName;

                if (MainForm.isOffline)
                    Program.form.RescanLocalLibrary();
                else
                    Program.form.RefreshDownloadedState();
            }
        }

        private void setBackupDirectory_Click(object sender, EventArgs e)
        {
            var dialog = new FolderSelectDialog
            {
                Title = "Select Backup Folder",
                InitialDirectory = _settings.GetEffectiveBackupDir()
            };

            if (dialog.Show(this.Handle))
            {
                _settings.CustomBackupDir = true;
                _settings.BackupDir = dialog.FileName;
            }
        }

        private void openDownloadDirectory_Click(object sender, EventArgs e)
        {
            string pathToOpen = _settings.CustomDownloadDir ? _settings.DownloadDir : Environment.CurrentDirectory;
            MainForm.OpenDirectory(pathToOpen);
        }

        private void openBackupDirectory_Click(object sender, EventArgs e)
        {
            string pathToOpen = _settings.GetEffectiveBackupDir();
            MainForm.OpenDirectory(pathToOpen);
        }

        private void bandwidthLimitTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}