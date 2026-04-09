using System.Windows.Forms;

namespace AndroidSideloader
{
    partial class QuestForm
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
            this.lblUsernameSection = new System.Windows.Forms.Label();
            this.lblMediaSection = new System.Windows.Forms.Label();
            this.lblPerformanceSection = new System.Windows.Forms.Label();
            this.GlobalUsername = new System.Windows.Forms.TextBox();
            this.btnApplyUsername = new AndroidSideloader.RoundButton();
            this.questPics = new AndroidSideloader.RoundButton();
            this.questVids = new AndroidSideloader.RoundButton();
            this.lblScreenshotsPath = new System.Windows.Forms.Label();
            this.lblRecordingsPath = new System.Windows.Forms.Label();
            this.toggleDeleteAfterTransfer = new AndroidSideloader.ToggleSwitch();
            this.lblDeleteAfterTransfer = new System.Windows.Forms.Label();
            this.lblPerformanceNote = new System.Windows.Forms.Label();
            this.lblRefreshRate = new System.Windows.Forms.Label();
            this.RefreshRateComboBox = new System.Windows.Forms.ComboBox();
            this.lblGpuLevel = new System.Windows.Forms.Label();
            this.GPUComboBox = new System.Windows.Forms.ComboBox();
            this.lblCpuLevel = new System.Windows.Forms.Label();
            this.CPUComboBox = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.TextureResTextBox = new System.Windows.Forms.TextBox();
            this.btnApplyTempSettings = new AndroidSideloader.RoundButton();
            this.separator1 = new System.Windows.Forms.Panel();
            this.separator2 = new System.Windows.Forms.Panel();
            this.btnClose = new AndroidSideloader.RoundButton();
            this.SuspendLayout();
            // 
            // lblUsernameSection
            // 
            this.lblUsernameSection.AutoSize = true;
            this.lblUsernameSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblUsernameSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblUsernameSection.Location = new System.Drawing.Point(20, 15);
            this.lblUsernameSection.Name = "lblUsernameSection";
            this.lblUsernameSection.Size = new System.Drawing.Size(80, 20);
            this.lblUsernameSection.TabIndex = 0;
            this.lblUsernameSection.Text = "Username";
            // 
            // lblMediaSection
            // 
            this.lblMediaSection.AutoSize = true;
            this.lblMediaSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblMediaSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblMediaSection.Location = new System.Drawing.Point(20, 97);
            this.lblMediaSection.Name = "lblMediaSection";
            this.lblMediaSection.Size = new System.Drawing.Size(114, 20);
            this.lblMediaSection.TabIndex = 3;
            this.lblMediaSection.Text = "Media Transfer";
            // 
            // lblPerformanceSection
            // 
            this.lblPerformanceSection.AutoSize = true;
            this.lblPerformanceSection.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPerformanceSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.lblPerformanceSection.Location = new System.Drawing.Point(20, 232);
            this.lblPerformanceSection.Name = "lblPerformanceSection";
            this.lblPerformanceSection.Size = new System.Drawing.Size(147, 20);
            this.lblPerformanceSection.TabIndex = 9;
            this.lblPerformanceSection.Text = "Temporary Settings";
            // 
            // GlobalUsername
            // 
            this.GlobalUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.GlobalUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GlobalUsername.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.GlobalUsername.ForeColor = System.Drawing.Color.White;
            this.GlobalUsername.Location = new System.Drawing.Point(24, 45);
            this.GlobalUsername.Name = "GlobalUsername";
            this.GlobalUsername.Size = new System.Drawing.Size(200, 24);
            this.GlobalUsername.TabIndex = 1;
            this.GlobalUsername.TextChanged += new System.EventHandler(this.GlobalUsername_TextChanged);
            // 
            // btnApplyUsername
            // 
            this.btnApplyUsername.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.btnApplyUsername.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.btnApplyUsername.BackColor = System.Drawing.Color.Transparent;
            this.btnApplyUsername.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyUsername.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApplyUsername.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnApplyUsername.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnApplyUsername.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnApplyUsername.Enabled = false;
            this.btnApplyUsername.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnApplyUsername.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.btnApplyUsername.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.btnApplyUsername.Location = new System.Drawing.Point(234, 45);
            this.btnApplyUsername.Name = "btnApplyUsername";
            this.btnApplyUsername.Radius = 5;
            this.btnApplyUsername.Size = new System.Drawing.Size(80, 25);
            this.btnApplyUsername.Stroke = false;
            this.btnApplyUsername.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.btnApplyUsername.TabIndex = 1;
            this.btnApplyUsername.Text = "APPLY";
            this.btnApplyUsername.Transparency = false;
            this.btnApplyUsername.Click += new System.EventHandler(this.btnApplyUsername_Click);
            // 
            // questPics
            // 
            this.questPics.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.questPics.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.questPics.BackColor = System.Drawing.Color.Transparent;
            this.questPics.Cursor = System.Windows.Forms.Cursors.Hand;
            this.questPics.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.questPics.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.questPics.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.questPics.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.questPics.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.questPics.ForeColor = System.Drawing.Color.White;
            this.questPics.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.questPics.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.questPics.Location = new System.Drawing.Point(24, 127);
            this.questPics.Name = "questPics";
            this.questPics.Radius = 5;
            this.questPics.Size = new System.Drawing.Size(140, 28);
            this.questPics.Stroke = true;
            this.questPics.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.questPics.TabIndex = 2;
            this.questPics.Text = "Screenshots";
            this.questPics.Transparency = false;
            this.questPics.Click += new System.EventHandler(this.questPics_Click);
            // 
            // questVids
            // 
            this.questVids.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.questVids.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.questVids.BackColor = System.Drawing.Color.Transparent;
            this.questVids.Cursor = System.Windows.Forms.Cursors.Hand;
            this.questVids.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.questVids.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.questVids.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.questVids.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.questVids.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.questVids.ForeColor = System.Drawing.Color.White;
            this.questVids.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.questVids.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.questVids.Location = new System.Drawing.Point(174, 127);
            this.questVids.Name = "questVids";
            this.questVids.Radius = 5;
            this.questVids.Size = new System.Drawing.Size(140, 28);
            this.questVids.Stroke = true;
            this.questVids.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.questVids.TabIndex = 3;
            this.questVids.Text = "Recordings";
            this.questVids.Transparency = false;
            this.questVids.Click += new System.EventHandler(this.questVids_Click);
            // 
            // lblScreenshotsPath
            // 
            this.lblScreenshotsPath.AutoSize = true;
            this.lblScreenshotsPath.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblScreenshotsPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblScreenshotsPath.Location = new System.Drawing.Point(24, 158);
            this.lblScreenshotsPath.Name = "lblScreenshotsPath";
            this.lblScreenshotsPath.Size = new System.Drawing.Size(161, 13);
            this.lblScreenshotsPath.TabIndex = 4;
            this.lblScreenshotsPath.Text = "→ Desktop\\Quest Screenshots";
            // 
            // lblRecordingsPath
            // 
            this.lblRecordingsPath.AutoSize = true;
            this.lblRecordingsPath.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRecordingsPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblRecordingsPath.Location = new System.Drawing.Point(174, 158);
            this.lblRecordingsPath.Name = "lblRecordingsPath";
            this.lblRecordingsPath.Size = new System.Drawing.Size(157, 13);
            this.lblRecordingsPath.TabIndex = 5;
            this.lblRecordingsPath.Text = "→ Desktop\\Quest Recordings";
            // 
            // toggleDeleteAfterTransfer
            // 
            this.toggleDeleteAfterTransfer.BackColor = System.Drawing.Color.Transparent;
            this.toggleDeleteAfterTransfer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toggleDeleteAfterTransfer.Location = new System.Drawing.Point(27, 189);
            this.toggleDeleteAfterTransfer.Name = "toggleDeleteAfterTransfer";
            this.toggleDeleteAfterTransfer.OffColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.toggleDeleteAfterTransfer.OnColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.toggleDeleteAfterTransfer.Size = new System.Drawing.Size(36, 18);
            this.toggleDeleteAfterTransfer.TabIndex = 6;
            this.toggleDeleteAfterTransfer.ThumbColor = System.Drawing.Color.White;
            this.toggleDeleteAfterTransfer.CheckedChanged += new System.EventHandler(this.toggleDeleteAfterTransfer_CheckedChanged);
            // 
            // lblDeleteAfterTransfer
            // 
            this.lblDeleteAfterTransfer.AutoSize = true;
            this.lblDeleteAfterTransfer.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDeleteAfterTransfer.ForeColor = System.Drawing.Color.White;
            this.lblDeleteAfterTransfer.Location = new System.Drawing.Point(72, 188);
            this.lblDeleteAfterTransfer.Name = "lblDeleteAfterTransfer";
            this.lblDeleteAfterTransfer.Size = new System.Drawing.Size(195, 17);
            this.lblDeleteAfterTransfer.TabIndex = 7;
            this.lblDeleteAfterTransfer.Text = "Delete from Quest after transfer";
            // 
            // lblPerformanceNote
            // 
            this.lblPerformanceNote.AutoSize = true;
            this.lblPerformanceNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPerformanceNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPerformanceNote.Location = new System.Drawing.Point(21, 254);
            this.lblPerformanceNote.Name = "lblPerformanceNote";
            this.lblPerformanceNote.Size = new System.Drawing.Size(120, 13);
            this.lblPerformanceNote.TabIndex = 10;
            this.lblPerformanceNote.Text = "Reboot Quest to reset";
            // 
            // lblRefreshRate
            // 
            this.lblRefreshRate.AutoSize = true;
            this.lblRefreshRate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRefreshRate.ForeColor = System.Drawing.Color.White;
            this.lblRefreshRate.Location = new System.Drawing.Point(24, 280);
            this.lblRefreshRate.Name = "lblRefreshRate";
            this.lblRefreshRate.Size = new System.Drawing.Size(72, 15);
            this.lblRefreshRate.TabIndex = 11;
            this.lblRefreshRate.Text = "Refresh Rate";
            // 
            // RefreshRateComboBox
            // 
            this.RefreshRateComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.RefreshRateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RefreshRateComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RefreshRateComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.RefreshRateComboBox.ForeColor = System.Drawing.Color.White;
            this.RefreshRateComboBox.FormattingEnabled = true;
            this.RefreshRateComboBox.Items.AddRange(new object[] {
            "72 Hz",
            "90 Hz",
            "120 Hz"});
            this.RefreshRateComboBox.Location = new System.Drawing.Point(24, 298);
            this.RefreshRateComboBox.Name = "RefreshRateComboBox";
            this.RefreshRateComboBox.Size = new System.Drawing.Size(130, 23);
            this.RefreshRateComboBox.TabIndex = 12;
            // 
            // lblGpuLevel
            // 
            this.lblGpuLevel.AutoSize = true;
            this.lblGpuLevel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGpuLevel.ForeColor = System.Drawing.Color.White;
            this.lblGpuLevel.Location = new System.Drawing.Point(170, 280);
            this.lblGpuLevel.Name = "lblGpuLevel";
            this.lblGpuLevel.Size = new System.Drawing.Size(60, 15);
            this.lblGpuLevel.TabIndex = 13;
            this.lblGpuLevel.Text = "GPU Level";
            // 
            // GPUComboBox
            // 
            this.GPUComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.GPUComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GPUComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GPUComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.GPUComboBox.ForeColor = System.Drawing.Color.White;
            this.GPUComboBox.FormattingEnabled = true;
            this.GPUComboBox.Items.AddRange(new object[] {
            "0 (Default)",
            "1",
            "2",
            "3",
            "4"});
            this.GPUComboBox.Location = new System.Drawing.Point(170, 298);
            this.GPUComboBox.Name = "GPUComboBox";
            this.GPUComboBox.Size = new System.Drawing.Size(130, 23);
            this.GPUComboBox.TabIndex = 14;
            // 
            // lblCpuLevel
            // 
            this.lblCpuLevel.AutoSize = true;
            this.lblCpuLevel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCpuLevel.ForeColor = System.Drawing.Color.White;
            this.lblCpuLevel.Location = new System.Drawing.Point(24, 330);
            this.lblCpuLevel.Name = "lblCpuLevel";
            this.lblCpuLevel.Size = new System.Drawing.Size(60, 15);
            this.lblCpuLevel.TabIndex = 15;
            this.lblCpuLevel.Text = "CPU Level";
            // 
            // CPUComboBox
            // 
            this.CPUComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.CPUComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CPUComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CPUComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.CPUComboBox.ForeColor = System.Drawing.Color.White;
            this.CPUComboBox.FormattingEnabled = true;
            this.CPUComboBox.Items.AddRange(new object[] {
            "0 (Default)",
            "1",
            "2",
            "3",
            "4"});
            this.CPUComboBox.Location = new System.Drawing.Point(24, 348);
            this.CPUComboBox.Name = "CPUComboBox";
            this.CPUComboBox.Size = new System.Drawing.Size(130, 23);
            this.CPUComboBox.TabIndex = 16;
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblResolution.ForeColor = System.Drawing.Color.White;
            this.lblResolution.Location = new System.Drawing.Point(170, 330);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(63, 15);
            this.lblResolution.TabIndex = 17;
            this.lblResolution.Text = "Resolution";
            // 
            // TextureResTextBox
            // 
            this.TextureResTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.TextureResTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextureResTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TextureResTextBox.ForeColor = System.Drawing.Color.White;
            this.TextureResTextBox.Location = new System.Drawing.Point(170, 348);
            this.TextureResTextBox.Name = "TextureResTextBox";
            this.TextureResTextBox.Size = new System.Drawing.Size(130, 23);
            this.TextureResTextBox.TabIndex = 18;
            this.TextureResTextBox.Text = "0";
            // 
            // btnApplyTempSettings
            // 
            this.btnApplyTempSettings.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.btnApplyTempSettings.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.btnApplyTempSettings.BackColor = System.Drawing.Color.Transparent;
            this.btnApplyTempSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyTempSettings.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApplyTempSettings.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnApplyTempSettings.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnApplyTempSettings.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnApplyTempSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyTempSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnApplyTempSettings.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.btnApplyTempSettings.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.btnApplyTempSettings.Location = new System.Drawing.Point(24, 385);
            this.btnApplyTempSettings.Name = "btnApplyTempSettings";
            this.btnApplyTempSettings.Radius = 5;
            this.btnApplyTempSettings.Size = new System.Drawing.Size(130, 30);
            this.btnApplyTempSettings.Stroke = false;
            this.btnApplyTempSettings.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.btnApplyTempSettings.TabIndex = 10;
            this.btnApplyTempSettings.Text = "APPLY SETTINGS";
            this.btnApplyTempSettings.Transparency = false;
            this.btnApplyTempSettings.Click += new System.EventHandler(this.btnApplyTempSettings_Click);
            // 
            // separator1
            // 
            this.separator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.separator1.Location = new System.Drawing.Point(20, 85);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(295, 1);
            this.separator1.TabIndex = 2;
            // 
            // separator2
            // 
            this.separator2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.separator2.Location = new System.Drawing.Point(20, 220);
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(295, 1);
            this.separator2.TabIndex = 8;
            // 
            // btnClose
            // 
            this.btnClose.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.btnClose.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(75)))));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnClose.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnClose.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnClose.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnClose.Location = new System.Drawing.Point(170, 385);
            this.btnClose.Name = "btnClose";
            this.btnClose.Radius = 5;
            this.btnClose.Size = new System.Drawing.Size(130, 30);
            this.btnClose.Stroke = true;
            this.btnClose.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.Transparency = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // QuestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(340, 435);
            this.Controls.Add(this.lblUsernameSection);
            this.Controls.Add(this.GlobalUsername);
            this.Controls.Add(this.btnApplyUsername);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.lblMediaSection);
            this.Controls.Add(this.questPics);
            this.Controls.Add(this.questVids);
            this.Controls.Add(this.lblScreenshotsPath);
            this.Controls.Add(this.lblRecordingsPath);
            this.Controls.Add(this.toggleDeleteAfterTransfer);
            this.Controls.Add(this.lblDeleteAfterTransfer);
            this.Controls.Add(this.separator2);
            this.Controls.Add(this.lblPerformanceSection);
            this.Controls.Add(this.lblPerformanceNote);
            this.Controls.Add(this.lblRefreshRate);
            this.Controls.Add(this.RefreshRateComboBox);
            this.Controls.Add(this.lblGpuLevel);
            this.Controls.Add(this.GPUComboBox);
            this.Controls.Add(this.lblCpuLevel);
            this.Controls.Add(this.CPUComboBox);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.TextureResTextBox);
            this.Controls.Add(this.btnApplyTempSettings);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Device Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.QuestForm_FormClosed);
            this.Load += new System.EventHandler(this.QuestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Section Labels
        private System.Windows.Forms.Label lblUsernameSection;
        private System.Windows.Forms.Label lblMediaSection;
        private System.Windows.Forms.Label lblPerformanceSection;

        // Username controls
        private System.Windows.Forms.TextBox GlobalUsername;
        private RoundButton btnApplyUsername;

        // Media controls
        private RoundButton questPics;
        private RoundButton questVids;
        private System.Windows.Forms.Label lblScreenshotsPath;
        private System.Windows.Forms.Label lblRecordingsPath;
        private ToggleSwitch toggleDeleteAfterTransfer;
        private System.Windows.Forms.Label lblDeleteAfterTransfer;

        // Performance controls
        private System.Windows.Forms.Label lblPerformanceNote;
        private System.Windows.Forms.Label lblRefreshRate;
        private System.Windows.Forms.ComboBox RefreshRateComboBox;
        private System.Windows.Forms.Label lblGpuLevel;
        private System.Windows.Forms.ComboBox GPUComboBox;
        private System.Windows.Forms.Label lblCpuLevel;
        private System.Windows.Forms.ComboBox CPUComboBox;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.TextBox TextureResTextBox;
        private RoundButton btnApplyTempSettings;

        // Separators
        private System.Windows.Forms.Panel separator1;
        private System.Windows.Forms.Panel separator2;

        // Close button
        private RoundButton btnClose;
    }
}