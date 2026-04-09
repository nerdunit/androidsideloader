namespace AndroidSideloader
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.downloadDirectorySetter = new System.Windows.Forms.FolderBrowserDialog();
            this.backupDirectorySetter = new System.Windows.Forms.FolderBrowserDialog();
            this.crashlogID = new System.Windows.Forms.Label();
            this.lblGeneralSection = new System.Windows.Forms.Label();
            this.lblDownloadSection = new System.Windows.Forms.Label();
            this.lblAdvancedSection = new System.Windows.Forms.Label();
            this.lblDirectoriesSection = new System.Windows.Forms.Label();
            this.lblDebugSection = new System.Windows.Forms.Label();
            this.lblCheckForUpdates = new System.Windows.Forms.Label();
            this.lblNoDeviceMode = new System.Windows.Forms.Label();
            this.lblDeleteAfterInstall = new System.Windows.Forms.Label();
            this.lblSingleThread = new System.Windows.Forms.Label();
            this.lblUseDownloadedFiles = new System.Windows.Forms.Label();
            this.lblAutoReinstall = new System.Windows.Forms.Label();
            this.lblMessageBoxes = new System.Windows.Forms.Label();
            this.lblUserJson = new System.Windows.Forms.Label();
            this.lblBMBF = new System.Windows.Forms.Label();
            this.lblVirtualFilesystem = new System.Windows.Forms.Label();
            this.lblTrailers = new System.Windows.Forms.Label();
            this.bandwidthLabel = new System.Windows.Forms.Label();
            this.bandwidthLimitTextBox = new System.Windows.Forms.TextBox();
            this.lblBandwidthUnit = new System.Windows.Forms.Label();
            this.separator1 = new System.Windows.Forms.Panel();
            this.separator2 = new System.Windows.Forms.Panel();
            this.separator3 = new System.Windows.Forms.Panel();
            this.separator4 = new System.Windows.Forms.Panel();
            this.toggleCheckForUpdates = new AndroidSideloader.ToggleSwitch();
            this.toggleMessageBoxes = new AndroidSideloader.ToggleSwitch();
            this.toggleTrailers = new AndroidSideloader.ToggleSwitch();
            this.toggleNoDeviceMode = new AndroidSideloader.ToggleSwitch();
            this.toggleDeleteAfterInstall = new AndroidSideloader.ToggleSwitch();
            this.toggleUseDownloadedFiles = new AndroidSideloader.ToggleSwitch();
            this.toggleAutoReinstall = new AndroidSideloader.ToggleSwitch();
            this.toggleSingleThread = new AndroidSideloader.ToggleSwitch();
            this.toggleUserJson = new AndroidSideloader.ToggleSwitch();
            this.toggleBMBF = new AndroidSideloader.ToggleSwitch();
            this.toggleVirtualFilesystem = new AndroidSideloader.ToggleSwitch();
            this.setDownloadDirectory = new AndroidSideloader.RoundButton();
            this.openDownloadDirectory = new AndroidSideloader.RoundButton();
            this.setBackupDirectory = new AndroidSideloader.RoundButton();
            this.openBackupDirectory = new AndroidSideloader.RoundButton();
            this.btnOpenDebug = new AndroidSideloader.RoundButton();
            this.btnResetDebug = new AndroidSideloader.RoundButton();
            this.btnUploadDebug = new AndroidSideloader.RoundButton();
            this.applyButton = new AndroidSideloader.RoundButton();
            this.resetSettingsButton = new AndroidSideloader.RoundButton();
            this.toggleProxy = new AndroidSideloader.ToggleSwitch();
            this.lblProxy = new System.Windows.Forms.Label();
            this.lblProxyAddress = new System.Windows.Forms.Label();
            this.proxyAddressTextBox = new System.Windows.Forms.TextBox();
            this.lblProxyPort = new System.Windows.Forms.Label();
            this.proxyPortTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // downloadDirectorySetter
            // 
            this.downloadDirectorySetter.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // backupDirectorySetter
            // 
            this.backupDirectorySetter.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // crashlogID
            // 
            this.crashlogID.AutoSize = true;
            this.crashlogID.Location = new System.Drawing.Point(24, 530);
            this.crashlogID.Name = "crashlogID";
            this.crashlogID.Size = new System.Drawing.Size(0, 13);
            this.crashlogID.TabIndex = 36;
            this.crashlogID.Visible = false;
            // 
            // lblGeneralSection
            // 
            this.lblGeneralSection.AutoSize = true;
            this.lblGeneralSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblGeneralSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblGeneralSection.Location = new System.Drawing.Point(20, 15);
            this.lblGeneralSection.Name = "lblGeneralSection";
            this.lblGeneralSection.Size = new System.Drawing.Size(63, 20);
            this.lblGeneralSection.TabIndex = 0;
            this.lblGeneralSection.Text = "General";
            // 
            // lblDownloadSection
            // 
            this.lblDownloadSection.AutoSize = true;
            this.lblDownloadSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDownloadSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblDownloadSection.Location = new System.Drawing.Point(20, 120);
            this.lblDownloadSection.Name = "lblDownloadSection";
            this.lblDownloadSection.Size = new System.Drawing.Size(144, 20);
            this.lblDownloadSection.TabIndex = 10;
            this.lblDownloadSection.Text = "Download && Install";
            // 
            // lblAdvancedSection
            // 
            this.lblAdvancedSection.AutoSize = true;
            this.lblAdvancedSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAdvancedSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblAdvancedSection.Location = new System.Drawing.Point(20, 260);
            this.lblAdvancedSection.Name = "lblAdvancedSection";
            this.lblAdvancedSection.Size = new System.Drawing.Size(78, 20);
            this.lblAdvancedSection.TabIndex = 23;
            this.lblAdvancedSection.Text = "Advanced";
            // 
            // lblDirectoriesSection
            // 
            this.lblDirectoriesSection.AutoSize = true;
            this.lblDirectoriesSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDirectoriesSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblDirectoriesSection.Location = new System.Drawing.Point(20, 457);
            this.lblDirectoriesSection.Name = "lblDirectoriesSection";
            this.lblDirectoriesSection.Size = new System.Drawing.Size(85, 20);
            this.lblDirectoriesSection.TabIndex = 33;
            this.lblDirectoriesSection.Text = "Directories";
            // 
            // lblDebugSection
            // 
            this.lblDebugSection.AutoSize = true;
            this.lblDebugSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDebugSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblDebugSection.Location = new System.Drawing.Point(20, 532);
            this.lblDebugSection.Name = "lblDebugSection";
            this.lblDebugSection.Size = new System.Drawing.Size(55, 20);
            this.lblDebugSection.TabIndex = 35;
            this.lblDebugSection.Text = "Debug";
            // 
            // lblCheckForUpdates
            // 
            this.lblCheckForUpdates.AutoSize = true;
            this.lblCheckForUpdates.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCheckForUpdates.ForeColor = System.Drawing.Color.White;
            this.lblCheckForUpdates.Location = new System.Drawing.Point(70, 49);
            this.lblCheckForUpdates.Name = "lblCheckForUpdates";
            this.lblCheckForUpdates.Size = new System.Drawing.Size(168, 15);
            this.lblCheckForUpdates.TabIndex = 2;
            this.lblCheckForUpdates.Text = "Check for Application Updates";
            // 
            // lblNoDeviceMode
            // 
            this.lblNoDeviceMode.AutoSize = true;
            this.lblNoDeviceMode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNoDeviceMode.ForeColor = System.Drawing.Color.White;
            this.lblNoDeviceMode.Location = new System.Drawing.Point(70, 154);
            this.lblNoDeviceMode.Name = "lblNoDeviceMode";
            this.lblNoDeviceMode.Size = new System.Drawing.Size(169, 15);
            this.lblNoDeviceMode.TabIndex = 12;
            this.lblNoDeviceMode.Text = "Disable Sideloading (Installing)";
            // 
            // lblDeleteAfterInstall
            // 
            this.lblDeleteAfterInstall.AutoSize = true;
            this.lblDeleteAfterInstall.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDeleteAfterInstall.ForeColor = System.Drawing.Color.White;
            this.lblDeleteAfterInstall.Location = new System.Drawing.Point(348, 154);
            this.lblDeleteAfterInstall.Name = "lblDeleteAfterInstall";
            this.lblDeleteAfterInstall.Size = new System.Drawing.Size(142, 15);
            this.lblDeleteAfterInstall.TabIndex = 14;
            this.lblDeleteAfterInstall.Text = "Delete Games After Install";
            // 
            // lblSingleThread
            // 
            this.lblSingleThread.AutoSize = true;
            this.lblSingleThread.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSingleThread.ForeColor = System.Drawing.Color.White;
            this.lblSingleThread.Location = new System.Drawing.Point(70, 294);
            this.lblSingleThread.Name = "lblSingleThread";
            this.lblSingleThread.Size = new System.Drawing.Size(128, 15);
            this.lblSingleThread.TabIndex = 25;
            this.lblSingleThread.Text = "Single-Threaded Mode";
            // 
            // lblUseDownloadedFiles
            // 
            this.lblUseDownloadedFiles.AutoSize = true;
            this.lblUseDownloadedFiles.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUseDownloadedFiles.ForeColor = System.Drawing.Color.White;
            this.lblUseDownloadedFiles.Location = new System.Drawing.Point(70, 184);
            this.lblUseDownloadedFiles.Name = "lblUseDownloadedFiles";
            this.lblUseDownloadedFiles.Size = new System.Drawing.Size(168, 15);
            this.lblUseDownloadedFiles.TabIndex = 16;
            this.lblUseDownloadedFiles.Text = "Don\'t Prompt to Re-Download";
            // 
            // lblAutoReinstall
            // 
            this.lblAutoReinstall.AutoSize = true;
            this.lblAutoReinstall.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAutoReinstall.ForeColor = System.Drawing.Color.White;
            this.lblAutoReinstall.Location = new System.Drawing.Point(348, 184);
            this.lblAutoReinstall.Name = "lblAutoReinstall";
            this.lblAutoReinstall.Size = new System.Drawing.Size(171, 15);
            this.lblAutoReinstall.TabIndex = 18;
            this.lblAutoReinstall.Text = "Auto-Reinstall on Install Failure";
            // 
            // lblMessageBoxes
            // 
            this.lblMessageBoxes.AutoSize = true;
            this.lblMessageBoxes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMessageBoxes.ForeColor = System.Drawing.Color.White;
            this.lblMessageBoxes.Location = new System.Drawing.Point(70, 79);
            this.lblMessageBoxes.Name = "lblMessageBoxes";
            this.lblMessageBoxes.Size = new System.Drawing.Size(201, 15);
            this.lblMessageBoxes.TabIndex = 6;
            this.lblMessageBoxes.Text = "Show Message Boxes on Completion";
            // 
            // lblUserJson
            // 
            this.lblUserJson.AutoSize = true;
            this.lblUserJson.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUserJson.ForeColor = System.Drawing.Color.White;
            this.lblUserJson.Location = new System.Drawing.Point(348, 294);
            this.lblUserJson.Name = "lblUserJson";
            this.lblUserJson.Size = new System.Drawing.Size(179, 15);
            this.lblUserJson.TabIndex = 27;
            this.lblUserJson.Text = "Push random user.json on Install";
            // 
            // lblBMBF
            // 
            this.lblBMBF.AutoSize = true;
            this.lblBMBF.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBMBF.ForeColor = System.Drawing.Color.White;
            this.lblBMBF.Location = new System.Drawing.Point(70, 324);
            this.lblBMBF.Name = "lblBMBF";
            this.lblBMBF.Size = new System.Drawing.Size(173, 15);
            this.lblBMBF.TabIndex = 29;
            this.lblBMBF.Text = "BMBF Song Zips Drag and Drop";
            // 
            // lblVirtualFilesystem
            // 
            this.lblVirtualFilesystem.AutoSize = true;
            this.lblVirtualFilesystem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblVirtualFilesystem.ForeColor = System.Drawing.Color.White;
            this.lblVirtualFilesystem.Location = new System.Drawing.Point(348, 324);
            this.lblVirtualFilesystem.Name = "lblVirtualFilesystem";
            this.lblVirtualFilesystem.Size = new System.Drawing.Size(174, 15);
            this.lblVirtualFilesystem.TabIndex = 31;
            this.lblVirtualFilesystem.Text = "Virtual Filesystem Compatibility";
            // 
            // lblTrailers
            // 
            this.lblTrailers.AutoSize = true;
            this.lblTrailers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTrailers.ForeColor = System.Drawing.Color.White;
            this.lblTrailers.Location = new System.Drawing.Point(348, 79);
            this.lblTrailers.Name = "lblTrailers";
            this.lblTrailers.Size = new System.Drawing.Size(110, 15);
            this.lblTrailers.TabIndex = 8;
            this.lblTrailers.Text = "Show Game Trailers";
            // 
            // bandwidthLabel
            // 
            this.bandwidthLabel.AutoSize = true;
            this.bandwidthLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bandwidthLabel.ForeColor = System.Drawing.Color.White;
            this.bandwidthLabel.Location = new System.Drawing.Point(24, 218);
            this.bandwidthLabel.Name = "bandwidthLabel";
            this.bandwidthLabel.Size = new System.Drawing.Size(97, 15);
            this.bandwidthLabel.TabIndex = 19;
            this.bandwidthLabel.Text = "Bandwidth Limit:";
            // 
            // bandwidthLimitTextBox
            // 
            this.bandwidthLimitTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.bandwidthLimitTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bandwidthLimitTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bandwidthLimitTextBox.ForeColor = System.Drawing.Color.White;
            this.bandwidthLimitTextBox.Location = new System.Drawing.Point(127, 215);
            this.bandwidthLimitTextBox.Name = "bandwidthLimitTextBox";
            this.bandwidthLimitTextBox.Size = new System.Drawing.Size(60, 23);
            this.bandwidthLimitTextBox.TabIndex = 20;
            this.bandwidthLimitTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.bandwidthLimitTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.bandwidthLimitTextBox_KeyPress);
            // 
            // lblBandwidthUnit
            // 
            this.lblBandwidthUnit.AutoSize = true;
            this.lblBandwidthUnit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBandwidthUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.lblBandwidthUnit.Location = new System.Drawing.Point(193, 218);
            this.lblBandwidthUnit.Name = "lblBandwidthUnit";
            this.lblBandwidthUnit.Size = new System.Drawing.Size(35, 15);
            this.lblBandwidthUnit.TabIndex = 21;
            this.lblBandwidthUnit.Text = "MB/s";
            // 
            // separator1
            // 
            this.separator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.separator1.Location = new System.Drawing.Point(20, 108);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(510, 1);
            this.separator1.TabIndex = 9;
            // 
            // separator2
            // 
            this.separator2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.separator2.Location = new System.Drawing.Point(20, 248);
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(510, 1);
            this.separator2.TabIndex = 22;
            // 
            // separator3
            // 
            this.separator3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.separator3.Location = new System.Drawing.Point(20, 445);
            this.separator3.Name = "separator3";
            this.separator3.Size = new System.Drawing.Size(510, 1);
            this.separator3.TabIndex = 32;
            // 
            // separator4
            // 
            this.separator4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.separator4.Location = new System.Drawing.Point(20, 520);
            this.separator4.Name = "separator4";
            this.separator4.Size = new System.Drawing.Size(510, 1);
            this.separator4.TabIndex = 34;
            // 
            // toggleCheckForUpdates
            // 
            this.toggleCheckForUpdates.BackColor = System.Drawing.Color.Transparent;
            this.toggleCheckForUpdates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleCheckForUpdates.Location = new System.Drawing.Point(28, 48);
            this.toggleCheckForUpdates.Name = "toggleCheckForUpdates";
            this.toggleCheckForUpdates.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleCheckForUpdates.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleCheckForUpdates.Size = new System.Drawing.Size(36, 18);
            this.toggleCheckForUpdates.TabIndex = 1;
            this.toggleCheckForUpdates.ThumbColor = System.Drawing.Color.White;
            this.toggleCheckForUpdates.CheckedChanged += new System.EventHandler(this.toggleCheckForUpdates_CheckedChanged);
            // 
            // toggleMessageBoxes
            // 
            this.toggleMessageBoxes.BackColor = System.Drawing.Color.Transparent;
            this.toggleMessageBoxes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleMessageBoxes.Location = new System.Drawing.Point(28, 78);
            this.toggleMessageBoxes.Name = "toggleMessageBoxes";
            this.toggleMessageBoxes.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleMessageBoxes.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleMessageBoxes.Size = new System.Drawing.Size(36, 18);
            this.toggleMessageBoxes.TabIndex = 5;
            this.toggleMessageBoxes.ThumbColor = System.Drawing.Color.White;
            this.toggleMessageBoxes.CheckedChanged += new System.EventHandler(this.toggleMessageBoxes_CheckedChanged);
            // 
            // toggleTrailers
            // 
            this.toggleTrailers.BackColor = System.Drawing.Color.Transparent;
            this.toggleTrailers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleTrailers.Location = new System.Drawing.Point(306, 78);
            this.toggleTrailers.Name = "toggleTrailers";
            this.toggleTrailers.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleTrailers.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleTrailers.Size = new System.Drawing.Size(36, 18);
            this.toggleTrailers.TabIndex = 7;
            this.toggleTrailers.ThumbColor = System.Drawing.Color.White;
            this.toggleTrailers.CheckedChanged += new System.EventHandler(this.toggleTrailers_CheckedChanged);
            // 
            // toggleNoDeviceMode
            // 
            this.toggleNoDeviceMode.BackColor = System.Drawing.Color.Transparent;
            this.toggleNoDeviceMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleNoDeviceMode.Location = new System.Drawing.Point(28, 153);
            this.toggleNoDeviceMode.Name = "toggleNoDeviceMode";
            this.toggleNoDeviceMode.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleNoDeviceMode.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleNoDeviceMode.Size = new System.Drawing.Size(36, 18);
            this.toggleNoDeviceMode.TabIndex = 11;
            this.toggleNoDeviceMode.ThumbColor = System.Drawing.Color.White;
            this.toggleNoDeviceMode.CheckedChanged += new System.EventHandler(this.toggleNoDeviceMode_CheckedChanged);
            // 
            // toggleDeleteAfterInstall
            // 
            this.toggleDeleteAfterInstall.BackColor = System.Drawing.Color.Transparent;
            this.toggleDeleteAfterInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleDeleteAfterInstall.Location = new System.Drawing.Point(306, 153);
            this.toggleDeleteAfterInstall.Name = "toggleDeleteAfterInstall";
            this.toggleDeleteAfterInstall.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleDeleteAfterInstall.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleDeleteAfterInstall.Size = new System.Drawing.Size(36, 18);
            this.toggleDeleteAfterInstall.TabIndex = 13;
            this.toggleDeleteAfterInstall.ThumbColor = System.Drawing.Color.White;
            this.toggleDeleteAfterInstall.CheckedChanged += new System.EventHandler(this.toggleDeleteAfterInstall_CheckedChanged);
            // 
            // toggleUseDownloadedFiles
            // 
            this.toggleUseDownloadedFiles.BackColor = System.Drawing.Color.Transparent;
            this.toggleUseDownloadedFiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleUseDownloadedFiles.Location = new System.Drawing.Point(28, 183);
            this.toggleUseDownloadedFiles.Name = "toggleUseDownloadedFiles";
            this.toggleUseDownloadedFiles.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleUseDownloadedFiles.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleUseDownloadedFiles.Size = new System.Drawing.Size(36, 18);
            this.toggleUseDownloadedFiles.TabIndex = 15;
            this.toggleUseDownloadedFiles.ThumbColor = System.Drawing.Color.White;
            this.toggleUseDownloadedFiles.CheckedChanged += new System.EventHandler(this.toggleUseDownloadedFiles_CheckedChanged);
            // 
            // toggleAutoReinstall
            // 
            this.toggleAutoReinstall.BackColor = System.Drawing.Color.Transparent;
            this.toggleAutoReinstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleAutoReinstall.Location = new System.Drawing.Point(306, 183);
            this.toggleAutoReinstall.Name = "toggleAutoReinstall";
            this.toggleAutoReinstall.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleAutoReinstall.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleAutoReinstall.Size = new System.Drawing.Size(36, 18);
            this.toggleAutoReinstall.TabIndex = 17;
            this.toggleAutoReinstall.ThumbColor = System.Drawing.Color.White;
            this.toggleAutoReinstall.CheckedChanged += new System.EventHandler(this.toggleAutoReinstall_CheckedChanged);
            this.toggleAutoReinstall.Click += new System.EventHandler(this.toggleAutoReinstall_Click);
            // 
            // toggleSingleThread
            // 
            this.toggleSingleThread.BackColor = System.Drawing.Color.Transparent;
            this.toggleSingleThread.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleSingleThread.Location = new System.Drawing.Point(28, 293);
            this.toggleSingleThread.Name = "toggleSingleThread";
            this.toggleSingleThread.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleSingleThread.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleSingleThread.Size = new System.Drawing.Size(36, 18);
            this.toggleSingleThread.TabIndex = 24;
            this.toggleSingleThread.ThumbColor = System.Drawing.Color.White;
            this.toggleSingleThread.CheckedChanged += new System.EventHandler(this.toggleSingleThread_CheckedChanged);
            // 
            // toggleUserJson
            // 
            this.toggleUserJson.BackColor = System.Drawing.Color.Transparent;
            this.toggleUserJson.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleUserJson.Location = new System.Drawing.Point(306, 293);
            this.toggleUserJson.Name = "toggleUserJson";
            this.toggleUserJson.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleUserJson.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleUserJson.Size = new System.Drawing.Size(36, 18);
            this.toggleUserJson.TabIndex = 26;
            this.toggleUserJson.ThumbColor = System.Drawing.Color.White;
            this.toggleUserJson.CheckedChanged += new System.EventHandler(this.toggleUserJson_CheckedChanged);
            // 
            // toggleBMBF
            // 
            this.toggleBMBF.BackColor = System.Drawing.Color.Transparent;
            this.toggleBMBF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleBMBF.Location = new System.Drawing.Point(28, 323);
            this.toggleBMBF.Name = "toggleBMBF";
            this.toggleBMBF.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleBMBF.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleBMBF.Size = new System.Drawing.Size(36, 18);
            this.toggleBMBF.TabIndex = 28;
            this.toggleBMBF.ThumbColor = System.Drawing.Color.White;
            this.toggleBMBF.CheckedChanged += new System.EventHandler(this.toggleBMBF_CheckedChanged);
            // 
            // toggleVirtualFilesystem
            // 
            this.toggleVirtualFilesystem.BackColor = System.Drawing.Color.Transparent;
            this.toggleVirtualFilesystem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleVirtualFilesystem.Location = new System.Drawing.Point(306, 323);
            this.toggleVirtualFilesystem.Name = "toggleVirtualFilesystem";
            this.toggleVirtualFilesystem.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleVirtualFilesystem.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleVirtualFilesystem.Size = new System.Drawing.Size(36, 18);
            this.toggleVirtualFilesystem.TabIndex = 30;
            this.toggleVirtualFilesystem.ThumbColor = System.Drawing.Color.White;
            this.toggleVirtualFilesystem.CheckedChanged += new System.EventHandler(this.toggleVirtualFilesystem_CheckedChanged);
            // 
            // setDownloadDirectory
            // 
            this.setDownloadDirectory.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.setDownloadDirectory.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.setDownloadDirectory.BackColor = System.Drawing.Color.Transparent;
            this.setDownloadDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.setDownloadDirectory.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.setDownloadDirectory.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.setDownloadDirectory.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.setDownloadDirectory.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.setDownloadDirectory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.setDownloadDirectory.ForeColor = System.Drawing.Color.White;
            this.setDownloadDirectory.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.setDownloadDirectory.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.setDownloadDirectory.Location = new System.Drawing.Point(24, 485);
            this.setDownloadDirectory.Name = "setDownloadDirectory";
            this.setDownloadDirectory.Radius = 5;
            this.setDownloadDirectory.Size = new System.Drawing.Size(125, 28);
            this.setDownloadDirectory.Stroke = true;
            this.setDownloadDirectory.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.setDownloadDirectory.TabIndex = 23;
            this.setDownloadDirectory.Text = "Set Download Dir";
            this.setDownloadDirectory.TextXOffset = 0;
            this.setDownloadDirectory.Transparency = false;
            this.setDownloadDirectory.Click += new System.EventHandler(this.setDownloadDirectory_Click);
            // 
            // openDownloadDirectory
            // 
            this.openDownloadDirectory.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.openDownloadDirectory.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.openDownloadDirectory.BackColor = System.Drawing.Color.Transparent;
            this.openDownloadDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.openDownloadDirectory.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.openDownloadDirectory.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.openDownloadDirectory.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.openDownloadDirectory.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.openDownloadDirectory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.openDownloadDirectory.ForeColor = System.Drawing.Color.White;
            this.openDownloadDirectory.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.openDownloadDirectory.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.openDownloadDirectory.Location = new System.Drawing.Point(153, 485);
            this.openDownloadDirectory.Name = "openDownloadDirectory";
            this.openDownloadDirectory.Radius = 5;
            this.openDownloadDirectory.Size = new System.Drawing.Size(125, 28);
            this.openDownloadDirectory.Stroke = true;
            this.openDownloadDirectory.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.openDownloadDirectory.TabIndex = 27;
            this.openDownloadDirectory.Text = "Open Download Dir";
            this.openDownloadDirectory.TextXOffset = 0;
            this.openDownloadDirectory.Transparency = false;
            this.openDownloadDirectory.Click += new System.EventHandler(this.openDownloadDirectory_Click);
            // 
            // setBackupDirectory
            // 
            this.setBackupDirectory.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.setBackupDirectory.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.setBackupDirectory.BackColor = System.Drawing.Color.Transparent;
            this.setBackupDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.setBackupDirectory.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.setBackupDirectory.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.setBackupDirectory.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.setBackupDirectory.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.setBackupDirectory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.setBackupDirectory.ForeColor = System.Drawing.Color.White;
            this.setBackupDirectory.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.setBackupDirectory.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.setBackupDirectory.Location = new System.Drawing.Point(282, 485);
            this.setBackupDirectory.Name = "setBackupDirectory";
            this.setBackupDirectory.Radius = 5;
            this.setBackupDirectory.Size = new System.Drawing.Size(120, 28);
            this.setBackupDirectory.Stroke = true;
            this.setBackupDirectory.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.setBackupDirectory.TabIndex = 24;
            this.setBackupDirectory.Text = "Set Backup Dir";
            this.setBackupDirectory.TextXOffset = 0;
            this.setBackupDirectory.Transparency = false;
            this.setBackupDirectory.Click += new System.EventHandler(this.setBackupDirectory_Click);
            // 
            // openBackupDirectory
            // 
            this.openBackupDirectory.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.openBackupDirectory.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.openBackupDirectory.BackColor = System.Drawing.Color.Transparent;
            this.openBackupDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.openBackupDirectory.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.openBackupDirectory.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.openBackupDirectory.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.openBackupDirectory.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.openBackupDirectory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.openBackupDirectory.ForeColor = System.Drawing.Color.White;
            this.openBackupDirectory.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.openBackupDirectory.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.openBackupDirectory.Location = new System.Drawing.Point(406, 485);
            this.openBackupDirectory.Name = "openBackupDirectory";
            this.openBackupDirectory.Radius = 5;
            this.openBackupDirectory.Size = new System.Drawing.Size(120, 28);
            this.openBackupDirectory.Stroke = true;
            this.openBackupDirectory.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.openBackupDirectory.TabIndex = 28;
            this.openBackupDirectory.Text = "Open Backup Dir";
            this.openBackupDirectory.TextXOffset = 0;
            this.openBackupDirectory.Transparency = false;
            this.openBackupDirectory.Click += new System.EventHandler(this.openBackupDirectory_Click);
            // 
            // btnOpenDebug
            // 
            this.btnOpenDebug.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnOpenDebug.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnOpenDebug.BackColor = System.Drawing.Color.Transparent;
            this.btnOpenDebug.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenDebug.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOpenDebug.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnOpenDebug.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnOpenDebug.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnOpenDebug.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenDebug.ForeColor = System.Drawing.Color.White;
            this.btnOpenDebug.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.btnOpenDebug.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.btnOpenDebug.Location = new System.Drawing.Point(24, 560);
            this.btnOpenDebug.Name = "btnOpenDebug";
            this.btnOpenDebug.Radius = 5;
            this.btnOpenDebug.Size = new System.Drawing.Size(90, 28);
            this.btnOpenDebug.Stroke = true;
            this.btnOpenDebug.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.btnOpenDebug.TabIndex = 21;
            this.btnOpenDebug.Text = "Open Log";
            this.btnOpenDebug.TextXOffset = 0;
            this.btnOpenDebug.Transparency = false;
            this.btnOpenDebug.Click += new System.EventHandler(this.btnOpenDebug_Click);
            // 
            // btnResetDebug
            // 
            this.btnResetDebug.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnResetDebug.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnResetDebug.BackColor = System.Drawing.Color.Transparent;
            this.btnResetDebug.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetDebug.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnResetDebug.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnResetDebug.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnResetDebug.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnResetDebug.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnResetDebug.ForeColor = System.Drawing.Color.White;
            this.btnResetDebug.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.btnResetDebug.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.btnResetDebug.Location = new System.Drawing.Point(120, 560);
            this.btnResetDebug.Name = "btnResetDebug";
            this.btnResetDebug.Radius = 5;
            this.btnResetDebug.Size = new System.Drawing.Size(90, 28);
            this.btnResetDebug.Stroke = true;
            this.btnResetDebug.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.btnResetDebug.TabIndex = 20;
            this.btnResetDebug.Text = "Reset Log";
            this.btnResetDebug.TextXOffset = 0;
            this.btnResetDebug.Transparency = false;
            this.btnResetDebug.Click += new System.EventHandler(this.btnResetDebug_click);
            // 
            // btnUploadDebug
            // 
            this.btnUploadDebug.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnUploadDebug.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnUploadDebug.BackColor = System.Drawing.Color.Transparent;
            this.btnUploadDebug.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUploadDebug.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUploadDebug.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnUploadDebug.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnUploadDebug.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnUploadDebug.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnUploadDebug.ForeColor = System.Drawing.Color.White;
            this.btnUploadDebug.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.btnUploadDebug.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.btnUploadDebug.Location = new System.Drawing.Point(216, 560);
            this.btnUploadDebug.Name = "btnUploadDebug";
            this.btnUploadDebug.Radius = 5;
            this.btnUploadDebug.Size = new System.Drawing.Size(90, 28);
            this.btnUploadDebug.Stroke = true;
            this.btnUploadDebug.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.btnUploadDebug.TabIndex = 19;
            this.btnUploadDebug.Text = "Upload Log";
            this.btnUploadDebug.TextXOffset = 0;
            this.btnUploadDebug.Transparency = false;
            this.btnUploadDebug.Click += new System.EventHandler(this.btnUploadDebug_click);
            // 
            // applyButton
            // 
            this.applyButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.applyButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.applyButton.BackColor = System.Drawing.Color.Transparent;
            this.applyButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.applyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyButton.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.applyButton.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.applyButton.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.applyButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.applyButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.applyButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.applyButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.applyButton.Location = new System.Drawing.Point(24, 607);
            this.applyButton.Name = "applyButton";
            this.applyButton.Radius = 5;
            this.applyButton.Size = new System.Drawing.Size(245, 36);
            this.applyButton.Stroke = false;
            this.applyButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.applyButton.TabIndex = 17;
            this.applyButton.Text = "SAVE & CLOSE";
            this.applyButton.TextXOffset = 0;
            this.applyButton.Transparency = false;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // resetSettingsButton
            // 
            this.resetSettingsButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.resetSettingsButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.resetSettingsButton.BackColor = System.Drawing.Color.Transparent;
            this.resetSettingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.resetSettingsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.resetSettingsButton.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.resetSettingsButton.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.resetSettingsButton.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.resetSettingsButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resetSettingsButton.ForeColor = System.Drawing.Color.White;
            this.resetSettingsButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.resetSettingsButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.resetSettingsButton.Location = new System.Drawing.Point(281, 607);
            this.resetSettingsButton.Name = "resetSettingsButton";
            this.resetSettingsButton.Radius = 5;
            this.resetSettingsButton.Size = new System.Drawing.Size(245, 36);
            this.resetSettingsButton.Stroke = true;
            this.resetSettingsButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.resetSettingsButton.TabIndex = 18;
            this.resetSettingsButton.Text = "CANCEL";
            this.resetSettingsButton.TextXOffset = 0;
            this.resetSettingsButton.Transparency = false;
            this.resetSettingsButton.Click += new System.EventHandler(this.resetSettingsButton_Click);
            // 
            // toggleProxy
            // 
            this.toggleProxy.BackColor = System.Drawing.Color.Transparent;
            this.toggleProxy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleProxy.Location = new System.Drawing.Point(28, 353);
            this.toggleProxy.Name = "toggleProxy";
            this.toggleProxy.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleProxy.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleProxy.Size = new System.Drawing.Size(36, 18);
            this.toggleProxy.TabIndex = 37;
            this.toggleProxy.ThumbColor = System.Drawing.Color.White;
            this.toggleProxy.CheckedChanged += new System.EventHandler(this.toggleProxy_CheckedChanged);
            // 
            // lblProxy
            // 
            this.lblProxy.AutoSize = true;
            this.lblProxy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProxy.ForeColor = System.Drawing.Color.White;
            this.lblProxy.Location = new System.Drawing.Point(70, 354);
            this.lblProxy.Name = "lblProxy";
            this.lblProxy.Size = new System.Drawing.Size(148, 15);
            this.lblProxy.TabIndex = 38;
            this.lblProxy.Text = "Use HTTP Proxy for Rclone";
            // 
            // lblProxyAddress
            // 
            this.lblProxyAddress.AutoSize = true;
            this.lblProxyAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProxyAddress.ForeColor = System.Drawing.Color.White;
            this.lblProxyAddress.Location = new System.Drawing.Point(25, 386);
            this.lblProxyAddress.Name = "lblProxyAddress";
            this.lblProxyAddress.Size = new System.Drawing.Size(84, 15);
            this.lblProxyAddress.TabIndex = 39;
            this.lblProxyAddress.Text = "Proxy Address:";
            // 
            // proxyAddressTextBox
            // 
            this.proxyAddressTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.proxyAddressTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.proxyAddressTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.proxyAddressTextBox.ForeColor = System.Drawing.Color.White;
            this.proxyAddressTextBox.Location = new System.Drawing.Point(115, 384);
            this.proxyAddressTextBox.Name = "proxyAddressTextBox";
            this.proxyAddressTextBox.Size = new System.Drawing.Size(95, 23);
            this.proxyAddressTextBox.TabIndex = 40;
            // 
            // lblProxyPort
            // 
            this.lblProxyPort.AutoSize = true;
            this.lblProxyPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProxyPort.ForeColor = System.Drawing.Color.White;
            this.lblProxyPort.Location = new System.Drawing.Point(25, 415);
            this.lblProxyPort.Name = "lblProxyPort";
            this.lblProxyPort.Size = new System.Drawing.Size(64, 15);
            this.lblProxyPort.TabIndex = 41;
            this.lblProxyPort.Text = "Proxy Port:";
            // 
            // proxyPortTextBox
            // 
            this.proxyPortTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.proxyPortTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.proxyPortTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.proxyPortTextBox.ForeColor = System.Drawing.Color.White;
            this.proxyPortTextBox.Location = new System.Drawing.Point(115, 413);
            this.proxyPortTextBox.Name = "proxyPortTextBox";
            this.proxyPortTextBox.Size = new System.Drawing.Size(49, 23);
            this.proxyPortTextBox.TabIndex = 42;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(550, 661);
            this.Controls.Add(this.lblProxyPort);
            this.Controls.Add(this.proxyPortTextBox);
            this.Controls.Add(this.lblProxyAddress);
            this.Controls.Add(this.proxyAddressTextBox);
            this.Controls.Add(this.toggleProxy);
            this.Controls.Add(this.lblProxy);
            this.Controls.Add(this.lblGeneralSection);
            this.Controls.Add(this.toggleCheckForUpdates);
            this.Controls.Add(this.lblCheckForUpdates);
            this.Controls.Add(this.toggleMessageBoxes);
            this.Controls.Add(this.lblMessageBoxes);
            this.Controls.Add(this.toggleTrailers);
            this.Controls.Add(this.lblTrailers);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.lblDownloadSection);
            this.Controls.Add(this.toggleNoDeviceMode);
            this.Controls.Add(this.lblNoDeviceMode);
            this.Controls.Add(this.toggleDeleteAfterInstall);
            this.Controls.Add(this.lblDeleteAfterInstall);
            this.Controls.Add(this.toggleUseDownloadedFiles);
            this.Controls.Add(this.lblUseDownloadedFiles);
            this.Controls.Add(this.toggleAutoReinstall);
            this.Controls.Add(this.lblAutoReinstall);
            this.Controls.Add(this.bandwidthLabel);
            this.Controls.Add(this.bandwidthLimitTextBox);
            this.Controls.Add(this.lblBandwidthUnit);
            this.Controls.Add(this.separator2);
            this.Controls.Add(this.lblAdvancedSection);
            this.Controls.Add(this.toggleSingleThread);
            this.Controls.Add(this.lblSingleThread);
            this.Controls.Add(this.toggleUserJson);
            this.Controls.Add(this.lblUserJson);
            this.Controls.Add(this.toggleBMBF);
            this.Controls.Add(this.lblBMBF);
            this.Controls.Add(this.toggleVirtualFilesystem);
            this.Controls.Add(this.lblVirtualFilesystem);
            this.Controls.Add(this.separator3);
            this.Controls.Add(this.lblDirectoriesSection);
            this.Controls.Add(this.setDownloadDirectory);
            this.Controls.Add(this.openDownloadDirectory);
            this.Controls.Add(this.setBackupDirectory);
            this.Controls.Add(this.openBackupDirectory);
            this.Controls.Add(this.separator4);
            this.Controls.Add(this.lblDebugSection);
            this.Controls.Add(this.btnOpenDebug);
            this.Controls.Add(this.btnResetDebug);
            this.Controls.Add(this.btnUploadDebug);
            this.Controls.Add(this.crashlogID);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.resetSettingsButton);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SettingsForm_KeyPress);
            this.Leave += new System.EventHandler(this.SettingsForm_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Section Labels
        private System.Windows.Forms.Label lblGeneralSection;
        private System.Windows.Forms.Label lblDownloadSection;
        private System.Windows.Forms.Label lblAdvancedSection;
        private System.Windows.Forms.Label lblDirectoriesSection;
        private System.Windows.Forms.Label lblDebugSection;

        // Toggle Switches
        private ToggleSwitch toggleCheckForUpdates;
        private ToggleSwitch toggleNoDeviceMode;
        private ToggleSwitch toggleDeleteAfterInstall;
        private ToggleSwitch toggleSingleThread;
        private ToggleSwitch toggleUseDownloadedFiles;
        private ToggleSwitch toggleAutoReinstall;
        private ToggleSwitch toggleMessageBoxes;
        private ToggleSwitch toggleUserJson;
        private ToggleSwitch toggleBMBF;
        private ToggleSwitch toggleVirtualFilesystem;
        private ToggleSwitch toggleTrailers;

        // Toggle Labels
        private System.Windows.Forms.Label lblCheckForUpdates;
        private System.Windows.Forms.Label lblNoDeviceMode;
        private System.Windows.Forms.Label lblDeleteAfterInstall;
        private System.Windows.Forms.Label lblSingleThread;
        private System.Windows.Forms.Label lblUseDownloadedFiles;
        private System.Windows.Forms.Label lblAutoReinstall;
        private System.Windows.Forms.Label lblMessageBoxes;
        private System.Windows.Forms.Label lblUserJson;
        private System.Windows.Forms.Label lblBMBF;
        private System.Windows.Forms.Label lblVirtualFilesystem;
        private System.Windows.Forms.Label lblTrailers;

        // Bandwidth
        private System.Windows.Forms.Label bandwidthLabel;
        private System.Windows.Forms.TextBox bandwidthLimitTextBox;
        private System.Windows.Forms.Label lblBandwidthUnit;

        // Buttons
        private RoundButton setDownloadDirectory;
        private RoundButton setBackupDirectory;
        private RoundButton openDownloadDirectory;
        private RoundButton openBackupDirectory;
        private RoundButton btnOpenDebug;
        private RoundButton btnResetDebug;
        private RoundButton btnUploadDebug;
        private RoundButton applyButton;
        private RoundButton resetSettingsButton;

        // Dialogs
        private System.Windows.Forms.FolderBrowserDialog downloadDirectorySetter;
        private System.Windows.Forms.FolderBrowserDialog backupDirectorySetter;

        // Other
        private System.Windows.Forms.Label crashlogID;

        // Separators
        private System.Windows.Forms.Panel separator1;
        private System.Windows.Forms.Panel separator2;
        private System.Windows.Forms.Panel separator3;
        private System.Windows.Forms.Panel separator4;
        private ToggleSwitch toggleProxy;
        private System.Windows.Forms.Label lblProxy;
        private System.Windows.Forms.Label lblProxyAddress;
        private System.Windows.Forms.TextBox proxyAddressTextBox;
        private System.Windows.Forms.Label lblProxyPort;
        private System.Windows.Forms.TextBox proxyPortTextBox;
    }
}