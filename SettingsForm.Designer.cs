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
            this.applyButton = new System.Windows.Forms.Button();
            this.enableMessageBoxesCheckBox = new System.Windows.Forms.CheckBox();
            this.resetSettingsButton = new System.Windows.Forms.Button();
            this.deleteAfterInstallCheckBox = new System.Windows.Forms.CheckBox();
            this.userJsonOnGameInstall = new System.Windows.Forms.CheckBox();
            this.BandwithTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BandwithComboBox = new System.Windows.Forms.ComboBox();
            this.DebugLogCopy = new System.Windows.Forms.Button();
            this.crashlogID = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.debuglogID = new System.Windows.Forms.Label();
            this.DebugID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.CblindBox = new System.Windows.Forms.CheckBox();
            this.nodevicemodeBox = new System.Windows.Forms.CheckBox();
            this.bmbfBox = new System.Windows.Forms.CheckBox();
            this.AutoReinstBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkForUpdatesCheckBox
            // 
            this.checkForUpdatesCheckBox.AutoSize = true;
            this.checkForUpdatesCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkForUpdatesCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.checkForUpdatesCheckBox.Location = new System.Drawing.Point(12, 12);
            this.checkForUpdatesCheckBox.Name = "checkForUpdatesCheckBox";
            this.checkForUpdatesCheckBox.Size = new System.Drawing.Size(148, 22);
            this.checkForUpdatesCheckBox.TabIndex = 0;
            this.checkForUpdatesCheckBox.Text = "Check for updates";
            this.checkForUpdatesCheckBox.UseVisualStyleBackColor = true;
            this.checkForUpdatesCheckBox.CheckedChanged += new System.EventHandler(this.checkForUpdatesCheckBox_CheckedChanged);
            // 
            // applyButton
            // 
            this.applyButton.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.applyButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.applyButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.applyButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.applyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.applyButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.applyButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.applyButton.Location = new System.Drawing.Point(67, 328);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(101, 31);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = false;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // enableMessageBoxesCheckBox
            // 
            this.enableMessageBoxesCheckBox.AutoSize = true;
            this.enableMessageBoxesCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.enableMessageBoxesCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.enableMessageBoxesCheckBox.Location = new System.Drawing.Point(12, 39);
            this.enableMessageBoxesCheckBox.Name = "enableMessageBoxesCheckBox";
            this.enableMessageBoxesCheckBox.Size = new System.Drawing.Size(309, 22);
            this.enableMessageBoxesCheckBox.TabIndex = 1;
            this.enableMessageBoxesCheckBox.Text = "Enable Message Boxes on task completed";
            this.enableMessageBoxesCheckBox.UseVisualStyleBackColor = true;
            this.enableMessageBoxesCheckBox.CheckedChanged += new System.EventHandler(this.enableMessageBoxesCheckBox_CheckedChanged);
            // 
            // resetSettingsButton
            // 
            this.resetSettingsButton.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.resetSettingsButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.resetSettingsButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.resetSettingsButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.resetSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetSettingsButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.resetSettingsButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.resetSettingsButton.Location = new System.Drawing.Point(174, 328);
            this.resetSettingsButton.Name = "resetSettingsButton";
            this.resetSettingsButton.Size = new System.Drawing.Size(101, 31);
            this.resetSettingsButton.TabIndex = 4;
            this.resetSettingsButton.Text = "Reset Settings";
            this.resetSettingsButton.UseVisualStyleBackColor = false;
            this.resetSettingsButton.Click += new System.EventHandler(this.resetSettingsButton_Click);
            // 
            // deleteAfterInstallCheckBox
            // 
            this.deleteAfterInstallCheckBox.AutoSize = true;
            this.deleteAfterInstallCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.deleteAfterInstallCheckBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.deleteAfterInstallCheckBox.Location = new System.Drawing.Point(12, 65);
            this.deleteAfterInstallCheckBox.Name = "deleteAfterInstallCheckBox";
            this.deleteAfterInstallCheckBox.Size = new System.Drawing.Size(288, 22);
            this.deleteAfterInstallCheckBox.TabIndex = 3;
            this.deleteAfterInstallCheckBox.Text = "Delete games after download and install";
            this.deleteAfterInstallCheckBox.UseVisualStyleBackColor = true;
            this.deleteAfterInstallCheckBox.CheckedChanged += new System.EventHandler(this.deleteAfterInstallCheckBox_CheckedChanged);
            // 
            // userJsonOnGameInstall
            // 
            this.userJsonOnGameInstall.AutoSize = true;
            this.userJsonOnGameInstall.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.userJsonOnGameInstall.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.userJsonOnGameInstall.Location = new System.Drawing.Point(12, 93);
            this.userJsonOnGameInstall.Name = "userJsonOnGameInstall";
            this.userJsonOnGameInstall.Size = new System.Drawing.Size(243, 22);
            this.userJsonOnGameInstall.TabIndex = 9;
            this.userJsonOnGameInstall.Text = "Push random user.json on install";
            this.userJsonOnGameInstall.UseVisualStyleBackColor = true;
            this.userJsonOnGameInstall.CheckedChanged += new System.EventHandler(this.userJsonOnGameInstall_CheckedChanged);
            // 
            // BandwithTextbox
            // 
            this.BandwithTextbox.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.BandwithTextbox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.BandwithTextbox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.BandwithTextbox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.BandwithTextbox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.BandwithTextbox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.BandwithTextbox.Location = new System.Drawing.Point(52, 296);
            this.BandwithTextbox.Name = "BandwithTextbox";
            this.BandwithTextbox.Size = new System.Drawing.Size(177, 24);
            this.BandwithTextbox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label1.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.label1.Location = new System.Drawing.Point(40, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Download speed limiter, 0 to disable";
            // 
            // BandwithComboBox
            // 
            this.BandwithComboBox.BackColor = global::AndroidSideloader.Properties.Settings.Default.ComboBoxColor;
            this.BandwithComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.BandwithComboBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.BandwithComboBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "ComboBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.BandwithComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BandwithComboBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.BandwithComboBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.BandwithComboBox.FormattingEnabled = true;
            this.BandwithComboBox.Items.AddRange(new object[] {
            "B",
            "K",
            "M",
            "G"});
            this.BandwithComboBox.Location = new System.Drawing.Point(235, 296);
            this.BandwithComboBox.Name = "BandwithComboBox";
            this.BandwithComboBox.Size = new System.Drawing.Size(55, 26);
            this.BandwithComboBox.TabIndex = 13;
            // 
            // DebugLogCopy
            // 
            this.DebugLogCopy.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.DebugLogCopy.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DebugLogCopy.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DebugLogCopy.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DebugLogCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DebugLogCopy.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.DebugLogCopy.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.DebugLogCopy.Location = new System.Drawing.Point(29, 373);
            this.DebugLogCopy.Name = "DebugLogCopy";
            this.DebugLogCopy.Size = new System.Drawing.Size(285, 31);
            this.DebugLogCopy.TabIndex = 5;
            this.DebugLogCopy.Text = "Send DebugLog to server.";
            this.DebugLogCopy.UseVisualStyleBackColor = false;
            this.DebugLogCopy.Click += new System.EventHandler(this.DebugLogCopy_click);
            // 
            // crashlogID
            // 
            this.crashlogID.AutoSize = true;
            this.crashlogID.Location = new System.Drawing.Point(13, 489);
            this.crashlogID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.crashlogID.Name = "crashlogID";
            this.crashlogID.Size = new System.Drawing.Size(0, 13);
            this.crashlogID.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.button1.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.button1.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.button1.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.button1.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.button1.Location = new System.Drawing.Point(29, 408);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(285, 31);
            this.button1.TabIndex = 5;
            this.button1.Text = "Reset Debug Log";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_click);
            // 
            // debuglogID
            // 
            this.debuglogID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.debuglogID.Location = new System.Drawing.Point(29, 484);
            this.debuglogID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.debuglogID.Name = "debuglogID";
            this.debuglogID.Size = new System.Drawing.Size(285, 48);
            this.debuglogID.TabIndex = 14;
            this.debuglogID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DebugID
            // 
            this.DebugID.AccessibleRole = System.Windows.Forms.AccessibleRole.Row;
            this.DebugID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.DebugID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DebugID.Cursor = System.Windows.Forms.Cursors.Default;
            this.DebugID.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.DebugID.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.DebugID.Location = new System.Drawing.Point(29, 455);
            this.DebugID.Margin = new System.Windows.Forms.Padding(2);
            this.DebugID.Name = "DebugID";
            this.DebugID.ReadOnly = true;
            this.DebugID.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.DebugID.Size = new System.Drawing.Size(285, 21);
            this.DebugID.TabIndex = 16;
            this.DebugID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DebugID.Click += new System.EventHandler(this.DebugID_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label2.Location = new System.Drawing.Point(29, 555);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(284, 86);
            this.label2.TabIndex = 14;
            this.label2.Text = "This is your most recent CrashLogID. Click on the CrashLogID to copy it to your c" +
    "lipboard.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Row;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.Location = new System.Drawing.Point(29, 538);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBox1.Size = new System.Drawing.Size(285, 21);
            this.textBox1.TabIndex = 16;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.Click += new System.EventHandler(this.textBox1_Click);
            // 
            // CblindBox
            // 
            this.CblindBox.AutoSize = true;
            this.CblindBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CblindBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.CblindBox.Location = new System.Drawing.Point(12, 119);
            this.CblindBox.Name = "CblindBox";
            this.CblindBox.Size = new System.Drawing.Size(182, 22);
            this.CblindBox.TabIndex = 9;
            this.CblindBox.Text = "Enable colorblind mode";
            this.CblindBox.UseVisualStyleBackColor = true;
            this.CblindBox.CheckedChanged += new System.EventHandler(this.CblindBox_CheckedChanged);
            this.CblindBox.Click += new System.EventHandler(this.CblindBox_Click);
            // 
            // nodevicemodeBox
            // 
            this.nodevicemodeBox.AutoSize = true;
            this.nodevicemodeBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nodevicemodeBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.nodevicemodeBox.Location = new System.Drawing.Point(12, 170);
            this.nodevicemodeBox.Name = "nodevicemodeBox";
            this.nodevicemodeBox.Size = new System.Drawing.Size(181, 22);
            this.nodevicemodeBox.TabIndex = 9;
            this.nodevicemodeBox.Text = "Enable no device mode";
            this.nodevicemodeBox.UseVisualStyleBackColor = true;
            this.nodevicemodeBox.CheckedChanged += new System.EventHandler(this.nodevicemodeBox_CheckedChanged);
            // 
            // bmbfBox
            // 
            this.bmbfBox.AutoSize = true;
            this.bmbfBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.bmbfBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.bmbfBox.Location = new System.Drawing.Point(12, 144);
            this.bmbfBox.Name = "bmbfBox";
            this.bmbfBox.Size = new System.Drawing.Size(281, 22);
            this.bmbfBox.TabIndex = 9;
            this.bmbfBox.Text = "Enable BMBF song zips drag and drop";
            this.bmbfBox.UseVisualStyleBackColor = true;
            this.bmbfBox.CheckedChanged += new System.EventHandler(this.bmbfBox_CheckedChanged);
            // 
            // AutoReinstBox
            // 
            this.AutoReinstBox.AutoSize = true;
            this.AutoReinstBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AutoReinstBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.AutoReinstBox.Location = new System.Drawing.Point(12, 196);
            this.AutoReinstBox.Name = "AutoReinstBox";
            this.AutoReinstBox.Size = new System.Drawing.Size(280, 22);
            this.AutoReinstBox.TabIndex = 9;
            this.AutoReinstBox.Text = "Enable auto reinstall upon install failure";
            this.AutoReinstBox.UseVisualStyleBackColor = true;
            this.AutoReinstBox.CheckedChanged += new System.EventHandler(this.AutoReinstBox_CheckedChanged);
            this.AutoReinstBox.Click += new System.EventHandler(this.AutoReinstBox_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = global::AndroidSideloader.Properties.Settings.Default.BackColor;
            this.ClientSize = new System.Drawing.Size(342, 634);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.DebugID);
            this.Controls.Add(this.crashlogID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.debuglogID);
            this.Controls.Add(this.BandwithComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BandwithTextbox);
            this.Controls.Add(this.bmbfBox);
            this.Controls.Add(this.AutoReinstBox);
            this.Controls.Add(this.nodevicemodeBox);
            this.Controls.Add(this.CblindBox);
            this.Controls.Add(this.userJsonOnGameInstall);
            this.Controls.Add(this.deleteAfterInstallCheckBox);
            this.Controls.Add(this.enableMessageBoxesCheckBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.DebugLogCopy);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.checkForUpdatesCheckBox);
            this.Controls.Add(this.resetSettingsButton);
            this.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "BackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SettingsForm_KeyPress);
            this.Leave += new System.EventHandler(this.SettingsForm_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkForUpdatesCheckBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.CheckBox enableMessageBoxesCheckBox;
        private System.Windows.Forms.Button resetSettingsButton;
        private System.Windows.Forms.CheckBox deleteAfterInstallCheckBox;
        private System.Windows.Forms.CheckBox userJsonOnGameInstall;
        private System.Windows.Forms.TextBox BandwithTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox BandwithComboBox;
        private System.Windows.Forms.Button DebugLogCopy;
        private System.Windows.Forms.Label crashlogID;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label debuglogID;
        private System.Windows.Forms.TextBox DebugID;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox CblindBox;
        private System.Windows.Forms.CheckBox nodevicemodeBox;
        private System.Windows.Forms.CheckBox bmbfBox;
        private System.Windows.Forms.CheckBox AutoReinstBox;
    }
}