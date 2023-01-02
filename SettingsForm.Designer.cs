﻿
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
            this.checkForUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.enableMessageBoxesCheckBox = new System.Windows.Forms.CheckBox();
            this.deleteAfterInstallCheckBox = new System.Windows.Forms.CheckBox();
            this.updateConfigCheckBox = new System.Windows.Forms.CheckBox();
            this.userJsonOnGameInstall = new System.Windows.Forms.CheckBox();
            this.txtBandwidth = new System.Windows.Forms.TextBox();
            this.lblDownloadSpeedLimiter = new System.Windows.Forms.Label();
            this.crashlogID = new System.Windows.Forms.Label();
            this.nodevicemodeBox = new System.Windows.Forms.CheckBox();
            this.bmbfBox = new System.Windows.Forms.CheckBox();
            this.AutoReinstBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new AndroidSideloader.RoundButton();
            this.resetSettingsButton = new AndroidSideloader.RoundButton();
            this.btnResetDebug = new AndroidSideloader.RoundButton();
            this.btnUploadDebug = new AndroidSideloader.RoundButton();
            this.btnOpenDebug = new AndroidSideloader.RoundButton();
            this.lblMibs = new System.Windows.Forms.Label();
            this.setDownloadDirectory = new AndroidSideloader.RoundButton();
            this.downloadDirectorySetter = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // checkForUpdatesCheckBox
            // 
            this.checkForUpdatesCheckBox.AutoSize = true;
            this.checkForUpdatesCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.checkForUpdatesCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkForUpdatesCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.checkForUpdatesCheckBox.Location = new System.Drawing.Point(12, 12);
            this.checkForUpdatesCheckBox.Name = "checkForUpdatesCheckBox";
            this.checkForUpdatesCheckBox.Size = new System.Drawing.Size(148, 22);
            this.checkForUpdatesCheckBox.TabIndex = 0;
            this.checkForUpdatesCheckBox.Text = "Check for updates";
            this.checkForUpdatesCheckBox.UseVisualStyleBackColor = false;
            this.checkForUpdatesCheckBox.CheckedChanged += new System.EventHandler(this.checkForUpdatesCheckBox_CheckedChanged);
            // 
            // enableMessageBoxesCheckBox
            // 
            this.enableMessageBoxesCheckBox.AutoSize = true;
            this.enableMessageBoxesCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.enableMessageBoxesCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.enableMessageBoxesCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.enableMessageBoxesCheckBox.Location = new System.Drawing.Point(12, 38);
            this.enableMessageBoxesCheckBox.Name = "enableMessageBoxesCheckBox";
            this.enableMessageBoxesCheckBox.Size = new System.Drawing.Size(309, 22);
            this.enableMessageBoxesCheckBox.TabIndex = 1;
            this.enableMessageBoxesCheckBox.Text = "Enable Message Boxes on task completed";
            this.enableMessageBoxesCheckBox.UseVisualStyleBackColor = false;
            this.enableMessageBoxesCheckBox.CheckedChanged += new System.EventHandler(this.enableMessageBoxesCheckBox_CheckedChanged);
            // 
            // deleteAfterInstallCheckBox
            // 
            this.deleteAfterInstallCheckBox.AutoSize = true;
            this.deleteAfterInstallCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.deleteAfterInstallCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.deleteAfterInstallCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.deleteAfterInstallCheckBox.Location = new System.Drawing.Point(12, 64);
            this.deleteAfterInstallCheckBox.Name = "deleteAfterInstallCheckBox";
            this.deleteAfterInstallCheckBox.Size = new System.Drawing.Size(288, 22);
            this.deleteAfterInstallCheckBox.TabIndex = 3;
            this.deleteAfterInstallCheckBox.Text = "Delete games after download and install";
            this.deleteAfterInstallCheckBox.UseVisualStyleBackColor = false;
            this.deleteAfterInstallCheckBox.CheckedChanged += new System.EventHandler(this.deleteAfterInstallCheckBox_CheckedChanged);
            // 
            // updateConfigCheckBox
            // 
            this.updateConfigCheckBox.AutoSize = true;
            this.updateConfigCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.updateConfigCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.updateConfigCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.updateConfigCheckBox.Location = new System.Drawing.Point(12, 90);
            this.updateConfigCheckBox.Name = "updateConfigCheckBox";
            this.updateConfigCheckBox.Size = new System.Drawing.Size(208, 22);
            this.updateConfigCheckBox.TabIndex = 6;
            this.updateConfigCheckBox.Text = "Update config automatically";
            this.updateConfigCheckBox.UseVisualStyleBackColor = false;
            this.updateConfigCheckBox.CheckedChanged += new System.EventHandler(this.updateConfigCheckBox_CheckedChanged);
            // 
            // userJsonOnGameInstall
            // 
            this.userJsonOnGameInstall.AutoSize = true;
            this.userJsonOnGameInstall.BackColor = System.Drawing.Color.Transparent;
            this.userJsonOnGameInstall.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.userJsonOnGameInstall.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.userJsonOnGameInstall.Location = new System.Drawing.Point(12, 116);
            this.userJsonOnGameInstall.Name = "userJsonOnGameInstall";
            this.userJsonOnGameInstall.Size = new System.Drawing.Size(243, 22);
            this.userJsonOnGameInstall.TabIndex = 9;
            this.userJsonOnGameInstall.Text = "Push random user.json on install";
            this.userJsonOnGameInstall.UseVisualStyleBackColor = false;
            this.userJsonOnGameInstall.CheckedChanged += new System.EventHandler(this.userJsonOnGameInstall_CheckedChanged);
            // 
            // txtBandwidth
            // 
            this.txtBandwidth.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.txtBandwidth.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtBandwidth.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtBandwidth.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtBandwidth.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.txtBandwidth.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.txtBandwidth.Location = new System.Drawing.Point(49, 254);
            this.txtBandwidth.Name = "txtBandwidth";
            this.txtBandwidth.Size = new System.Drawing.Size(103, 24);
            this.txtBandwidth.TabIndex = 11;
            // 
            // lblDownloadSpeedLimiter
            // 
            this.lblDownloadSpeedLimiter.AutoSize = true;
            this.lblDownloadSpeedLimiter.BackColor = System.Drawing.Color.Transparent;
            this.lblDownloadSpeedLimiter.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblDownloadSpeedLimiter.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.lblDownloadSpeedLimiter.Location = new System.Drawing.Point(87, 230);
            this.lblDownloadSpeedLimiter.Name = "lblDownloadSpeedLimiter";
            this.lblDownloadSpeedLimiter.Size = new System.Drawing.Size(169, 18);
            this.lblDownloadSpeedLimiter.TabIndex = 12;
            this.lblDownloadSpeedLimiter.Text = "Download Speed Limiter";
            // 
            // crashlogID
            // 
            this.crashlogID.AutoSize = true;
            this.crashlogID.Location = new System.Drawing.Point(13, 439);
            this.crashlogID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.crashlogID.Name = "crashlogID";
            this.crashlogID.Size = new System.Drawing.Size(0, 13);
            this.crashlogID.TabIndex = 15;
            // 
            // nodevicemodeBox
            // 
            this.nodevicemodeBox.AutoSize = true;
            this.nodevicemodeBox.BackColor = System.Drawing.Color.Transparent;
            this.nodevicemodeBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nodevicemodeBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.nodevicemodeBox.Location = new System.Drawing.Point(12, 168);
            this.nodevicemodeBox.Name = "nodevicemodeBox";
            this.nodevicemodeBox.Size = new System.Drawing.Size(181, 22);
            this.nodevicemodeBox.TabIndex = 9;
            this.nodevicemodeBox.Text = "Enable no device mode";
            this.nodevicemodeBox.UseVisualStyleBackColor = false;
            this.nodevicemodeBox.CheckedChanged += new System.EventHandler(this.nodevicemodeBox_CheckedChanged);
            // 
            // bmbfBox
            // 
            this.bmbfBox.AutoSize = true;
            this.bmbfBox.BackColor = System.Drawing.Color.Transparent;
            this.bmbfBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.bmbfBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.bmbfBox.Location = new System.Drawing.Point(12, 142);
            this.bmbfBox.Name = "bmbfBox";
            this.bmbfBox.Size = new System.Drawing.Size(281, 22);
            this.bmbfBox.TabIndex = 9;
            this.bmbfBox.Text = "Enable BMBF song zips drag and drop";
            this.bmbfBox.UseVisualStyleBackColor = false;
            this.bmbfBox.CheckedChanged += new System.EventHandler(this.bmbfBox_CheckedChanged);
            // 
            // AutoReinstBox
            // 
            this.AutoReinstBox.AutoSize = true;
            this.AutoReinstBox.BackColor = System.Drawing.Color.Transparent;
            this.AutoReinstBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AutoReinstBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.AutoReinstBox.Location = new System.Drawing.Point(12, 194);
            this.AutoReinstBox.Name = "AutoReinstBox";
            this.AutoReinstBox.Size = new System.Drawing.Size(280, 22);
            this.AutoReinstBox.TabIndex = 9;
            this.AutoReinstBox.Text = "Enable auto reinstall upon install failure";
            this.AutoReinstBox.UseVisualStyleBackColor = false;
            this.AutoReinstBox.CheckedChanged += new System.EventHandler(this.AutoReinstBox_CheckedChanged);
            this.AutoReinstBox.Click += new System.EventHandler(this.AutoReinstBox_Click);
            // 
            // applyButton
            // 
            this.applyButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.applyButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.applyButton.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.applyButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.applyButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.applyButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.applyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.applyButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.applyButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.applyButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.applyButton.Location = new System.Drawing.Point(29, 301);
            this.applyButton.Name = "applyButton";
            this.applyButton.Radius = 5;
            this.applyButton.Size = new System.Drawing.Size(133, 31);
            this.applyButton.Stroke = true;
            this.applyButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.applyButton.TabIndex = 17;
            this.applyButton.Text = "Apply Settings";
            this.applyButton.Transparency = false;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // resetSettingsButton
            // 
            this.resetSettingsButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.resetSettingsButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.resetSettingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.resetSettingsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.resetSettingsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.resetSettingsButton.ForeColor = System.Drawing.Color.White;
            this.resetSettingsButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.resetSettingsButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.resetSettingsButton.Location = new System.Drawing.Point(181, 301);
            this.resetSettingsButton.Name = "resetSettingsButton";
            this.resetSettingsButton.Radius = 5;
            this.resetSettingsButton.Size = new System.Drawing.Size(133, 31);
            this.resetSettingsButton.Stroke = true;
            this.resetSettingsButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.resetSettingsButton.TabIndex = 18;
            this.resetSettingsButton.Text = "Reset Settings";
            this.resetSettingsButton.Transparency = false;
            this.resetSettingsButton.Click += new System.EventHandler(this.resetSettingsButton_Click);
            // 
            // btnResetDebug
            // 
            this.btnResetDebug.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnResetDebug.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnResetDebug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnResetDebug.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnResetDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.btnResetDebug.ForeColor = System.Drawing.Color.White;
            this.btnResetDebug.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnResetDebug.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnResetDebug.Location = new System.Drawing.Point(29, 385);
            this.btnResetDebug.Name = "btnResetDebug";
            this.btnResetDebug.Radius = 5;
            this.btnResetDebug.Size = new System.Drawing.Size(285, 31);
            this.btnResetDebug.Stroke = true;
            this.btnResetDebug.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnResetDebug.TabIndex = 20;
            this.btnResetDebug.Text = "Reset Debug Log";
            this.btnResetDebug.Transparency = false;
            this.btnResetDebug.Click += new System.EventHandler(this.btnResetDebug_click);
            // 
            // btnUploadDebug
            // 
            this.btnUploadDebug.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnUploadDebug.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnUploadDebug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnUploadDebug.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUploadDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.btnUploadDebug.ForeColor = System.Drawing.Color.White;
            this.btnUploadDebug.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnUploadDebug.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnUploadDebug.Location = new System.Drawing.Point(29, 422);
            this.btnUploadDebug.Name = "btnUploadDebug";
            this.btnUploadDebug.Radius = 5;
            this.btnUploadDebug.Size = new System.Drawing.Size(285, 31);
            this.btnUploadDebug.Stroke = true;
            this.btnUploadDebug.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnUploadDebug.TabIndex = 19;
            this.btnUploadDebug.Text = "Upload Debug Log";
            this.btnUploadDebug.Transparency = false;
            this.btnUploadDebug.Click += new System.EventHandler(this.btnUploadDebug_click);
            // 
            // btnOpenDebug
            // 
            this.btnOpenDebug.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnOpenDebug.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnOpenDebug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnOpenDebug.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOpenDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.btnOpenDebug.ForeColor = System.Drawing.Color.White;
            this.btnOpenDebug.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnOpenDebug.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnOpenDebug.Location = new System.Drawing.Point(29, 348);
            this.btnOpenDebug.Name = "btnOpenDebug";
            this.btnOpenDebug.Radius = 5;
            this.btnOpenDebug.Size = new System.Drawing.Size(285, 31);
            this.btnOpenDebug.Stroke = true;
            this.btnOpenDebug.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnOpenDebug.TabIndex = 21;
            this.btnOpenDebug.Text = "Open Debug Log";
            this.btnOpenDebug.Transparency = false;
            this.btnOpenDebug.Click += new System.EventHandler(this.btnOpenDebug_Click);
            // 
            // lblMibs
            // 
            this.lblMibs.AutoSize = true;
            this.lblMibs.BackColor = System.Drawing.Color.Transparent;
            this.lblMibs.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblMibs.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.lblMibs.Location = new System.Drawing.Point(159, 257);
            this.lblMibs.Name = "lblMibs";
            this.lblMibs.Size = new System.Drawing.Size(135, 18);
            this.lblMibs.TabIndex = 22;
            this.lblMibs.Text = "MiB/s (0 to disable)";
            // 
            // setDownloadDirectory
            // 
            this.setDownloadDirectory.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.setDownloadDirectory.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.setDownloadDirectory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.setDownloadDirectory.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.setDownloadDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.setDownloadDirectory.ForeColor = System.Drawing.Color.White;
            this.setDownloadDirectory.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.setDownloadDirectory.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.setDownloadDirectory.Location = new System.Drawing.Point(29, 482);
            this.setDownloadDirectory.Name = "setDownloadDirectory";
            this.setDownloadDirectory.Radius = 5;
            this.setDownloadDirectory.Size = new System.Drawing.Size(285, 31);
            this.setDownloadDirectory.Stroke = true;
            this.setDownloadDirectory.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.setDownloadDirectory.TabIndex = 23;
            this.setDownloadDirectory.Text = "Set Download Directory";
            this.setDownloadDirectory.Transparency = false;
            this.setDownloadDirectory.Click += new System.EventHandler(this.setDownloadDirectory_Click);
            // 
            // downloadDirectorySetter
            // 
            this.downloadDirectorySetter.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = global::AndroidSideloader.Properties.Settings.Default.BackColor;
            this.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_cubes_1_1_1_0_0_0_1__000000_212121;
            this.ClientSize = new System.Drawing.Size(342, 547);
            this.Controls.Add(this.setDownloadDirectory);
            this.Controls.Add(this.lblMibs);
            this.Controls.Add(this.btnOpenDebug);
            this.Controls.Add(this.btnResetDebug);
            this.Controls.Add(this.btnUploadDebug);
            this.Controls.Add(this.resetSettingsButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.crashlogID);
            this.Controls.Add(this.lblDownloadSpeedLimiter);
            this.Controls.Add(this.txtBandwidth);
            this.Controls.Add(this.bmbfBox);
            this.Controls.Add(this.AutoReinstBox);
            this.Controls.Add(this.nodevicemodeBox);
            this.Controls.Add(this.userJsonOnGameInstall);
            this.Controls.Add(this.updateConfigCheckBox);
            this.Controls.Add(this.deleteAfterInstallCheckBox);
            this.Controls.Add(this.enableMessageBoxesCheckBox);
            this.Controls.Add(this.checkForUpdatesCheckBox);
            this.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "BackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SettingsForm_KeyPress);
            this.Leave += new System.EventHandler(this.SettingsForm_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkForUpdatesCheckBox;
        private System.Windows.Forms.CheckBox enableMessageBoxesCheckBox;
        private System.Windows.Forms.CheckBox deleteAfterInstallCheckBox;
        private System.Windows.Forms.CheckBox updateConfigCheckBox;
        private System.Windows.Forms.CheckBox userJsonOnGameInstall;
        private System.Windows.Forms.TextBox txtBandwidth;
        private System.Windows.Forms.Label lblDownloadSpeedLimiter;
        private System.Windows.Forms.Label crashlogID;
        private System.Windows.Forms.CheckBox nodevicemodeBox;
        private System.Windows.Forms.CheckBox bmbfBox;
        private System.Windows.Forms.CheckBox AutoReinstBox;
        private RoundButton applyButton;
        private RoundButton resetSettingsButton;
        private RoundButton btnResetDebug;
        private RoundButton btnUploadDebug;
        private RoundButton btnOpenDebug;
        private System.Windows.Forms.Label lblMibs;
        private RoundButton setDownloadDirectory;
        private System.Windows.Forms.FolderBrowserDialog downloadDirectorySetter;
    }
}