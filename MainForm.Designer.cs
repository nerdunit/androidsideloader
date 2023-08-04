
using AndroidSideloader.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace AndroidSideloader
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.m_combo = new SergeUtils.EasyCompletionComboBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.speedLabel = new System.Windows.Forms.Label();
            this.etaLabel = new System.Windows.Forms.Label();
            this.freeDisclaimer = new System.Windows.Forms.Label();
            this.gamesQueListBox = new System.Windows.Forms.ListBox();
            this.devicesComboBox = new System.Windows.Forms.ComboBox();
            this.remotesList = new System.Windows.Forms.ComboBox();
            this.gamesListView = new System.Windows.Forms.ListView();
            this.GameNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReleaseNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PackageNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VersionCodeIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReleaseAPKPathIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VersionNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.gamesQueueLabel = new System.Windows.Forms.Label();
            this.ProgressText = new System.Windows.Forms.Label();
            this.notesRichTextBox = new System.Windows.Forms.RichTextBox();
            this.DragDropLbl = new System.Windows.Forms.Label();
            this.lblNotes = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSearchHelp = new System.Windows.Forms.Label();
            this.lblShortcutsF2 = new System.Windows.Forms.Label();
            this.lblQuickSearch = new System.Windows.Forms.Label();
            this.lblUpdateAvailable = new System.Windows.Forms.Label();
            this.lblUpToDate = new System.Windows.Forms.Label();
            this.lblMirror = new System.Windows.Forms.Label();
            this.EnterInstallBox = new System.Windows.Forms.CheckBox();
            this.ADBcommandbox = new System.Windows.Forms.TextBox();
            this.lblShortcutCtrlR = new System.Windows.Forms.Label();
            this.lblAdbCommand = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.lblNeedsDonate = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.PictureBox();
            this.gamesPictureBox = new System.Windows.Forms.PictureBox();
            this.startsideloadbutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.startsideloadbutton = new System.Windows.Forms.Button();
            this.devicesbutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.devicesbutton = new System.Windows.Forms.Button();
            this.obbcopybutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.obbcopybutton = new System.Windows.Forms.Button();
            this.backupbutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.backupbutton = new System.Windows.Forms.Button();
            this.restorebutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.restorebutton = new System.Windows.Forms.Button();
            this.getApkButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.getApkButton = new System.Windows.Forms.Button();
            this.uninstallAppButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.uninstallAppButton = new System.Windows.Forms.Button();
            this.pullAppToDesktopBtn_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.pullAppToDesktopBtn = new System.Windows.Forms.Button();
            this.copyBulkObbButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.copyBulkObbButton = new System.Windows.Forms.Button();
            this.aboutBtn_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.aboutBtn = new System.Windows.Forms.Button();
            this.settingsButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.settingsButton = new System.Windows.Forms.Button();
            this.InstallQUset_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.InstallQUset = new System.Windows.Forms.Button();
            this.removeQUSetting_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.removeQUSetting = new System.Windows.Forms.Button();
            this.QuestOptionsButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.QuestOptionsButton = new System.Windows.Forms.Button();
            this.ADBWirelessDisable_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ADBWirelessDisable = new System.Windows.Forms.Button();
            this.ADBWirelessEnable_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ADBWirelessEnable = new System.Windows.Forms.Button();
            this.UpdateGamesButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.UpdateGamesButton = new System.Windows.Forms.Button();
            this.listApkButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.listApkButton = new System.Windows.Forms.Button();
            this.speedLabel_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.etaLabel_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.EnterInstallBox_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.diskLabel = new System.Windows.Forms.Label();
            this.sideloadDrop = new System.Windows.Forms.Button();
            this.sideloadContainer = new System.Windows.Forms.Panel();
            this.backupDrop = new System.Windows.Forms.Button();
            this.backupContainer = new System.Windows.Forms.Panel();
            this.otherDrop = new System.Windows.Forms.Button();
            this.otherContainer = new System.Windows.Forms.Panel();
            this.downloadingLabel = new System.Windows.Forms.Label();
            this.imgBattery = new System.Windows.Forms.PictureBox();
            this.BatteryLbl = new System.Windows.Forms.Label();
            this.ULGif = new System.Windows.Forms.PictureBox();
            this.ULLabel = new System.Windows.Forms.Label();
            this.verLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.downloadInstallGameButton = new AndroidSideloader.RoundButton();
            this.MountButton = new AndroidSideloader.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesPictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.sideloadContainer.SuspendLayout();
            this.backupContainer.SuspendLayout();
            this.otherContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBattery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ULGif)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_combo
            // 
            this.m_combo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.m_combo.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_combo.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_combo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_combo.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.m_combo.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.m_combo.Location = new System.Drawing.Point(224, 9);
            this.m_combo.Name = "m_combo";
            this.m_combo.Size = new System.Drawing.Size(374, 26);
            this.m_combo.TabIndex = 0;
            this.m_combo.Text = "Select an Installed App to Uninstall or Share...";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.progressBar.ForeColor = System.Drawing.Color.Purple;
            this.progressBar.Location = new System.Drawing.Point(2, 0);
            this.progressBar.MinimumSize = new System.Drawing.Size(200, 13);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(418, 13);
            this.progressBar.TabIndex = 7;
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.BackColor = System.Drawing.Color.Transparent;
            this.speedLabel.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.speedLabel.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.speedLabel.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.speedLabel.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.speedLabel.Location = new System.Drawing.Point(2, 14);
            this.speedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(149, 18);
            this.speedLabel.TabIndex = 76;
            this.speedLabel.Text = "DLS: Speed in MBPS";
            this.speedLabel_Tooltip.SetToolTip(this.speedLabel, "Current download speed, updates every second, in mbps");
            // 
            // etaLabel
            // 
            this.etaLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.etaLabel.BackColor = System.Drawing.Color.Transparent;
            this.etaLabel.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.etaLabel.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.etaLabel.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.etaLabel.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.etaLabel.Location = new System.Drawing.Point(224, 14);
            this.etaLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.etaLabel.Name = "etaLabel";
            this.etaLabel.Size = new System.Drawing.Size(196, 18);
            this.etaLabel.TabIndex = 75;
            this.etaLabel.Text = "ETA: HH:MM:SS Left";
            this.etaLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.etaLabel_Tooltip.SetToolTip(this.etaLabel, "Estimated time when game will finish download, updates every 5 seconds, format is" +
        " HH:MM:SS");
            // 
            // freeDisclaimer
            // 
            this.freeDisclaimer.AutoSize = true;
            this.freeDisclaimer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.freeDisclaimer.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.freeDisclaimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.freeDisclaimer.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.freeDisclaimer.Location = new System.Drawing.Point(289, 556);
            this.freeDisclaimer.Name = "freeDisclaimer";
            this.freeDisclaimer.Size = new System.Drawing.Size(246, 40);
            this.freeDisclaimer.TabIndex = 79;
            this.freeDisclaimer.Text = "This app is FREE!! \r\nClick here to go to the github.";
            this.freeDisclaimer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.freeDisclaimer.Click += new System.EventHandler(this.freeDisclaimer_Click);
            // 
            // gamesQueListBox
            // 
            this.gamesQueListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamesQueListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.gamesQueListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gamesQueListBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.gamesQueListBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.gamesQueListBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.gamesQueListBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.gamesQueListBox.FormattingEnabled = true;
            this.gamesQueListBox.ItemHeight = 18;
            this.gamesQueListBox.Location = new System.Drawing.Point(601, 467);
            this.gamesQueListBox.Margin = new System.Windows.Forms.Padding(2);
            this.gamesQueListBox.Name = "gamesQueListBox";
            this.gamesQueListBox.Size = new System.Drawing.Size(421, 128);
            this.gamesQueListBox.TabIndex = 9;
            this.gamesQueListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gamesQueListBox_MouseClick);
            this.gamesQueListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.gamesQueListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            // 
            // devicesComboBox
            // 
            this.devicesComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.devicesComboBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.devicesComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.devicesComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.devicesComboBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.devicesComboBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.devicesComboBox.FormattingEnabled = true;
            this.devicesComboBox.Location = new System.Drawing.Point(224, 39);
            this.devicesComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.devicesComboBox.Name = "devicesComboBox";
            this.devicesComboBox.Size = new System.Drawing.Size(164, 26);
            this.devicesComboBox.TabIndex = 1;
            this.devicesComboBox.Text = "Select your device";
            this.devicesComboBox.SelectedIndexChanged += new System.EventHandler(this.devicesComboBox_SelectedIndexChanged);
            // 
            // remotesList
            // 
            this.remotesList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.remotesList.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.remotesList.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.remotesList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.remotesList.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.remotesList.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.remotesList.FormattingEnabled = true;
            this.remotesList.Location = new System.Drawing.Point(545, 40);
            this.remotesList.Margin = new System.Windows.Forms.Padding(2);
            this.remotesList.Name = "remotesList";
            this.remotesList.Size = new System.Drawing.Size(53, 26);
            this.remotesList.TabIndex = 3;
            this.remotesList.SelectedIndexChanged += new System.EventHandler(this.remotesList_SelectedIndexChanged);
            // 
            // gamesListView
            // 
            this.gamesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamesListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.gamesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GameNameIndex,
            this.ReleaseNameIndex,
            this.PackageNameIndex,
            this.VersionCodeIndex,
            this.ReleaseAPKPathIndex,
            this.VersionNameIndex});
            this.gamesListView.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.gamesListView.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gamesListView.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.gamesListView.HideSelection = false;
            this.gamesListView.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.gamesListView.Location = new System.Drawing.Point(224, 70);
            this.gamesListView.Name = "gamesListView";
            this.gamesListView.ShowGroups = false;
            this.gamesListView.Size = new System.Drawing.Size(798, 352);
            this.gamesListView.TabIndex = 6;
            this.gamesListView.UseCompatibleStateImageBehavior = false;
            this.gamesListView.View = System.Windows.Forms.View.Details;
            this.gamesListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.gamesListView.SelectedIndexChanged += new System.EventHandler(this.gamesListView_SelectedIndexChanged);
            this.gamesListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.gamesListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.gamesListView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gamesListView_KeyPress);
            this.gamesListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gamesListView_MouseDoubleClick);
            // 
            // GameNameIndex
            // 
            this.GameNameIndex.Text = "Game Name";
            this.GameNameIndex.Width = 158;
            // 
            // ReleaseNameIndex
            // 
            this.ReleaseNameIndex.Text = "Release Name";
            this.ReleaseNameIndex.Width = 244;
            // 
            // PackageNameIndex
            // 
            this.PackageNameIndex.Text = "Package Name";
            this.PackageNameIndex.Width = 87;
            // 
            // VersionCodeIndex
            // 
            this.VersionCodeIndex.Text = "Version";
            this.VersionCodeIndex.Width = 75;
            // 
            // ReleaseAPKPathIndex
            // 
            this.ReleaseAPKPathIndex.Text = "Last Updated";
            this.ReleaseAPKPathIndex.Width = 145;
            // 
            // VersionNameIndex
            // 
            this.VersionNameIndex.Text = "Size (MB)";
            this.VersionNameIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.VersionNameIndex.Width = 66;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTextBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.searchTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.searchTextBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.searchTextBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.searchTextBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.searchTextBox.Location = new System.Drawing.Point(484, 250);
            this.searchTextBox.MaximumSize = new System.Drawing.Size(231, 26);
            this.searchTextBox.MinimumSize = new System.Drawing.Size(231, 26);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(231, 24);
            this.searchTextBox.TabIndex = 5;
            this.searchTextBox.Text = "Search";
            this.searchTextBox.Visible = false;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            this.searchTextBox.Leave += new System.EventHandler(this.searchTextBox_Leave);
            // 
            // gamesQueueLabel
            // 
            this.gamesQueueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gamesQueueLabel.AutoSize = true;
            this.gamesQueueLabel.BackColor = System.Drawing.Color.Transparent;
            this.gamesQueueLabel.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.gamesQueueLabel.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.gamesQueueLabel.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.gamesQueueLabel.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.gamesQueueLabel.Location = new System.Drawing.Point(599, 578);
            this.gamesQueueLabel.Name = "gamesQueueLabel";
            this.gamesQueueLabel.Size = new System.Drawing.Size(52, 18);
            this.gamesQueueLabel.TabIndex = 86;
            this.gamesQueueLabel.Text = "Queue";
            // 
            // ProgressText
            // 
            this.ProgressText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressText.AutoSize = true;
            this.ProgressText.BackColor = System.Drawing.Color.Transparent;
            this.ProgressText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgressText.ForeColor = System.Drawing.Color.White;
            this.ProgressText.Location = new System.Drawing.Point(225, 684);
            this.ProgressText.Name = "ProgressText";
            this.ProgressText.Size = new System.Drawing.Size(0, 18);
            this.ProgressText.TabIndex = 88;
            // 
            // notesRichTextBox
            // 
            this.notesRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notesRichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.notesRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notesRichTextBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.notesRichTextBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.notesRichTextBox.HideSelection = false;
            this.notesRichTextBox.Location = new System.Drawing.Point(601, 614);
            this.notesRichTextBox.Name = "notesRichTextBox";
            this.notesRichTextBox.ReadOnly = true;
            this.notesRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.notesRichTextBox.ShowSelectionMargin = true;
            this.notesRichTextBox.Size = new System.Drawing.Size(421, 66);
            this.notesRichTextBox.TabIndex = 10;
            this.notesRichTextBox.Text = "\n\n\n                                     TIP: PRESS F1 TO SEE A LIST OF SHORTCUTS";
            // 
            // DragDropLbl
            // 
            this.DragDropLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DragDropLbl.AutoSize = true;
            this.DragDropLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.DragDropLbl.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DragDropLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DragDropLbl.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.DragDropLbl.Location = new System.Drawing.Point(602, 501);
            this.DragDropLbl.Name = "DragDropLbl";
            this.DragDropLbl.Size = new System.Drawing.Size(320, 55);
            this.DragDropLbl.TabIndex = 25;
            this.DragDropLbl.Text = "DragDropLBL";
            this.DragDropLbl.Visible = false;
            // 
            // lblNotes
            // 
            this.lblNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNotes.AutoSize = true;
            this.lblNotes.BackColor = System.Drawing.Color.Transparent;
            this.lblNotes.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblNotes.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblNotes.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.lblNotes.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.lblNotes.Location = new System.Drawing.Point(597, 666);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(48, 18);
            this.lblNotes.TabIndex = 86;
            this.lblNotes.Text = "Notes";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.DataBindings.Add(new System.Windows.Forms.Binding("ImageLocation", global::AndroidSideloader.Properties.Settings.Default, "BackPicturePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.ImageLocation = global::AndroidSideloader.Properties.Settings.Default.BackPicturePath;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(218, -15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(819, 743);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 74;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(464, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(271, 120);
            this.label2.TabIndex = 89;
            this.label2.Visible = false;
            // 
            // lblSearchHelp
            // 
            this.lblSearchHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchHelp.AutoSize = true;
            this.lblSearchHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSearchHelp.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSearchHelp.Location = new System.Drawing.Point(516, 209);
            this.lblSearchHelp.Name = "lblSearchHelp";
            this.lblSearchHelp.Size = new System.Drawing.Size(170, 51);
            this.lblSearchHelp.TabIndex = 90;
            this.lblSearchHelp.Text = "Start typing to search.\r\nPress ESC to close.\r\n\r\n";
            this.lblSearchHelp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSearchHelp.Visible = false;
            // 
            // lblShortcutsF2
            // 
            this.lblShortcutsF2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShortcutsF2.AutoSize = true;
            this.lblShortcutsF2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.lblShortcutsF2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblShortcutsF2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblShortcutsF2.Location = new System.Drawing.Point(509, 284);
            this.lblShortcutsF2.Name = "lblShortcutsF2";
            this.lblShortcutsF2.Size = new System.Drawing.Size(188, 17);
            this.lblShortcutsF2.TabIndex = 90;
            this.lblShortcutsF2.Text = "Shortcuts: CTRL+F or F2";
            this.lblShortcutsF2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblShortcutsF2.Visible = false;
            // 
            // lblQuickSearch
            // 
            this.lblQuickSearch.AutoSize = true;
            this.lblQuickSearch.BackColor = System.Drawing.Color.Transparent;
            this.lblQuickSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblQuickSearch.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblQuickSearch.Location = new System.Drawing.Point(738, 11);
            this.lblQuickSearch.Name = "lblQuickSearch";
            this.lblQuickSearch.Size = new System.Drawing.Size(105, 17);
            this.lblQuickSearch.TabIndex = 90;
            this.lblQuickSearch.Text = "Quick Search";
            this.lblQuickSearch.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUpdateAvailable
            // 
            this.lblUpdateAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUpdateAvailable.AutoSize = true;
            this.lblUpdateAvailable.BackColor = System.Drawing.Color.Transparent;
            this.lblUpdateAvailable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUpdateAvailable.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdateAvailable.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblUpdateAvailable.Location = new System.Drawing.Point(878, 24);
            this.lblUpdateAvailable.Name = "lblUpdateAvailable";
            this.lblUpdateAvailable.Size = new System.Drawing.Size(151, 20);
            this.lblUpdateAvailable.TabIndex = 90;
            this.lblUpdateAvailable.Text = "𝖴𝖯𝖣𝖠𝖳𝖤 𝖠𝖵𝖠𝖨𝖫𝖠𝖡𝖫𝖤";
            this.lblUpdateAvailable.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblUpdateAvailable.Click += new System.EventHandler(this.updateAvailable_Click);
            // 
            // lblUpToDate
            // 
            this.lblUpToDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUpToDate.AutoSize = true;
            this.lblUpToDate.BackColor = System.Drawing.Color.Transparent;
            this.lblUpToDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUpToDate.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpToDate.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblUpToDate.Location = new System.Drawing.Point(878, 6);
            this.lblUpToDate.Name = "lblUpToDate";
            this.lblUpToDate.Size = new System.Drawing.Size(96, 20);
            this.lblUpToDate.TabIndex = 90;
            this.lblUpToDate.Text = "𝖴𝖯 𝖳𝖮 𝖣𝖠𝖳𝖤";
            this.lblUpToDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblUpToDate.Click += new System.EventHandler(this.lblUpToDate_Click);
            // 
            // lblMirror
            // 
            this.lblMirror.AutoSize = true;
            this.lblMirror.BackColor = System.Drawing.Color.Transparent;
            this.lblMirror.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblMirror.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMirror.Location = new System.Drawing.Point(475, 44);
            this.lblMirror.Name = "lblMirror";
            this.lblMirror.Size = new System.Drawing.Size(65, 17);
            this.lblMirror.TabIndex = 90;
            this.lblMirror.Text = "Mirror #";
            this.lblMirror.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // EnterInstallBox
            // 
            this.EnterInstallBox.AutoSize = true;
            this.EnterInstallBox.BackColor = System.Drawing.Color.Transparent;
            this.EnterInstallBox.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.EnterInstallBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.EnterInstallBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.EnterInstallBox.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.EnterInstallBox.Location = new System.Drawing.Point(625, 10);
            this.EnterInstallBox.Name = "EnterInstallBox";
            this.EnterInstallBox.Size = new System.Drawing.Size(102, 54);
            this.EnterInstallBox.TabIndex = 93;
            this.EnterInstallBox.Text = "Download w/ \r\n Enter Key";
            this.EnterInstallBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnterInstallBox_Tooltip.SetToolTip(this.EnterInstallBox, "Use the ENTER key on your keyboard to start installation of games from the Game L" +
        "ist");
            this.EnterInstallBox.UseVisualStyleBackColor = true;
            this.EnterInstallBox.CheckedChanged += new System.EventHandler(this.EnterInstallBox_CheckedChanged);
            // 
            // ADBcommandbox
            // 
            this.ADBcommandbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ADBcommandbox.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.ADBcommandbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ADBcommandbox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBcommandbox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBcommandbox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBcommandbox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.ADBcommandbox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.ADBcommandbox.Location = new System.Drawing.Point(484, 250);
            this.ADBcommandbox.MaximumSize = new System.Drawing.Size(231, 26);
            this.ADBcommandbox.MinimumSize = new System.Drawing.Size(231, 26);
            this.ADBcommandbox.Name = "ADBcommandbox";
            this.ADBcommandbox.Size = new System.Drawing.Size(231, 24);
            this.ADBcommandbox.TabIndex = 5;
            this.ADBcommandbox.Visible = false;
            this.ADBcommandbox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            this.ADBcommandbox.Enter += new System.EventHandler(this.ADBcommandbox_Enter);
            this.ADBcommandbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ADBcommandbox_KeyPress);
            this.ADBcommandbox.Leave += new System.EventHandler(this.ADBcommandbox_Leave);
            // 
            // lblShortcutCtrlR
            // 
            this.lblShortcutCtrlR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShortcutCtrlR.AutoSize = true;
            this.lblShortcutCtrlR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.lblShortcutCtrlR.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblShortcutCtrlR.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblShortcutCtrlR.Location = new System.Drawing.Point(528, 285);
            this.lblShortcutCtrlR.Name = "lblShortcutCtrlR";
            this.lblShortcutCtrlR.Size = new System.Drawing.Size(148, 17);
            this.lblShortcutCtrlR.TabIndex = 90;
            this.lblShortcutCtrlR.Text = "Shortcut = CTRL+R";
            this.lblShortcutCtrlR.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblShortcutCtrlR.Visible = false;
            // 
            // lblAdbCommand
            // 
            this.lblAdbCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAdbCommand.AutoSize = true;
            this.lblAdbCommand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.lblAdbCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblAdbCommand.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblAdbCommand.Location = new System.Drawing.Point(514, 206);
            this.lblAdbCommand.Name = "lblAdbCommand";
            this.lblAdbCommand.Size = new System.Drawing.Size(173, 68);
            this.lblAdbCommand.TabIndex = 90;
            this.lblAdbCommand.Text = "Type command without\r\n\"adb\" prefix.\r\n\r\n\r\n";
            this.lblAdbCommand.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAdbCommand.Visible = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.DataBindings.Add(new System.Windows.Forms.Binding("ImageLocation", global::AndroidSideloader.Properties.Settings.Default, "BackPicturePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.pictureBox7.ErrorImage = null;
            this.pictureBox7.ImageLocation = global::AndroidSideloader.Properties.Settings.Default.BackPicturePath;
            this.pictureBox7.InitialImage = null;
            this.pictureBox7.Location = new System.Drawing.Point(217, 18);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(819, 710);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 74;
            this.pictureBox7.TabStop = false;
            // 
            // lblNeedsDonate
            // 
            this.lblNeedsDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNeedsDonate.AutoSize = true;
            this.lblNeedsDonate.BackColor = System.Drawing.Color.Transparent;
            this.lblNeedsDonate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblNeedsDonate.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNeedsDonate.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblNeedsDonate.Location = new System.Drawing.Point(878, 43);
            this.lblNeedsDonate.Name = "lblNeedsDonate";
            this.lblNeedsDonate.Size = new System.Drawing.Size(142, 20);
            this.lblNeedsDonate.TabIndex = 90;
            this.lblNeedsDonate.Text = "𝖭𝖤𝖶𝖤𝖱 𝖳𝖧𝖠𝖭 𝖫𝖨𝖲𝖳";
            this.lblNeedsDonate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblNeedsDonate.Click += new System.EventHandler(this.lblNeedsDonate_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.BackgroundImage = global::AndroidSideloader.Properties.Resources.SearchGlass;
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSearch.Location = new System.Drawing.Point(773, 37);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(28, 24);
            this.btnSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.btnSearch.TabIndex = 91;
            this.btnSearch.TabStop = false;
            this.btnSearch.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // gamesPictureBox
            // 
            this.gamesPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gamesPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.gamesPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.gamesPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gamesPictureBox.Location = new System.Drawing.Point(224, 467);
            this.gamesPictureBox.Name = "gamesPictureBox";
            this.gamesPictureBox.Size = new System.Drawing.Size(374, 214);
            this.gamesPictureBox.TabIndex = 84;
            this.gamesPictureBox.TabStop = false;
            this.gamesPictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.gamesPictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            // 
            // startsideloadbutton
            // 
            this.startsideloadbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.startsideloadbutton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.startsideloadbutton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.startsideloadbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.startsideloadbutton.FlatAppearance.BorderSize = 0;
            this.startsideloadbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startsideloadbutton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.startsideloadbutton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.startsideloadbutton.Location = new System.Drawing.Point(0, 140);
            this.startsideloadbutton.Name = "startsideloadbutton";
            this.startsideloadbutton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.startsideloadbutton.Size = new System.Drawing.Size(221, 28);
            this.startsideloadbutton.TabIndex = 5;
            this.startsideloadbutton.Text = "Sideload APK";
            this.startsideloadbutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startsideloadbutton_Tooltip.SetToolTip(this.startsideloadbutton, "Sideload an APK onto your device");
            this.startsideloadbutton.UseVisualStyleBackColor = false;
            this.startsideloadbutton.Click += new System.EventHandler(this.startsideloadbutton_Click);
            // 
            // devicesbutton
            // 
            this.devicesbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.devicesbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.devicesbutton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.devicesbutton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.devicesbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.devicesbutton.FlatAppearance.BorderSize = 0;
            this.devicesbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.devicesbutton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.devicesbutton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.devicesbutton.Location = new System.Drawing.Point(0, 0);
            this.devicesbutton.Name = "devicesbutton";
            this.devicesbutton.Size = new System.Drawing.Size(221, 28);
            this.devicesbutton.TabIndex = 0;
            this.devicesbutton.Text = "RECONNECT DEVICE";
            this.devicesbutton_Tooltip.SetToolTip(this.devicesbutton, "Rookie will attempt to reconnect to your Device");
            this.devicesbutton.UseVisualStyleBackColor = false;
            this.devicesbutton.Click += new System.EventHandler(this.devicesbutton_Click);
            // 
            // obbcopybutton
            // 
            this.obbcopybutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.obbcopybutton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.obbcopybutton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.obbcopybutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.obbcopybutton.FlatAppearance.BorderSize = 0;
            this.obbcopybutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.obbcopybutton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.obbcopybutton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.obbcopybutton.Location = new System.Drawing.Point(0, 0);
            this.obbcopybutton.Name = "obbcopybutton";
            this.obbcopybutton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.obbcopybutton.Size = new System.Drawing.Size(221, 28);
            this.obbcopybutton.TabIndex = 0;
            this.obbcopybutton.Text = "Copy OBB";
            this.obbcopybutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.obbcopybutton_Tooltip.SetToolTip(this.obbcopybutton, "Copies an obb folder to the Android/obb folder from the device (Not all games use" +
        " obb files)");
            this.obbcopybutton.UseVisualStyleBackColor = false;
            this.obbcopybutton.Click += new System.EventHandler(this.obbcopybutton_Click);
            // 
            // backupbutton
            // 
            this.backupbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.backupbutton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupbutton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupbutton.FlatAppearance.BorderSize = 0;
            this.backupbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backupbutton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.backupbutton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.backupbutton.Location = new System.Drawing.Point(0, 28);
            this.backupbutton.Name = "backupbutton";
            this.backupbutton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.backupbutton.Size = new System.Drawing.Size(221, 28);
            this.backupbutton.TabIndex = 1;
            this.backupbutton.Text = "Backup Gamedata";
            this.backupbutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.backupbutton_Tooltip.SetToolTip(this.backupbutton, "Saves the game and apps data to the sideloader folder (Does not save APKs or OBBs" +
        ")");
            this.backupbutton.UseVisualStyleBackColor = false;
            this.backupbutton.Click += new System.EventHandler(this.backupbutton_Click);
            // 
            // restorebutton
            // 
            this.restorebutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.restorebutton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.restorebutton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.restorebutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.restorebutton.FlatAppearance.BorderSize = 0;
            this.restorebutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restorebutton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.restorebutton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.restorebutton.Location = new System.Drawing.Point(0, 0);
            this.restorebutton.Name = "restorebutton";
            this.restorebutton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.restorebutton.Size = new System.Drawing.Size(221, 28);
            this.restorebutton.TabIndex = 0;
            this.restorebutton.Text = "Restore Gamedata";
            this.restorebutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.restorebutton_Tooltip.SetToolTip(this.restorebutton, "Restores the game and apps data to the device (Use the Backup Game Data button fi" +
        "rst!)");
            this.restorebutton.UseVisualStyleBackColor = false;
            this.restorebutton.Click += new System.EventHandler(this.restorebutton_Click);
            // 
            // getApkButton
            // 
            this.getApkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.getApkButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.getApkButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.getApkButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.getApkButton.FlatAppearance.BorderSize = 0;
            this.getApkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.getApkButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.getApkButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.getApkButton.Location = new System.Drawing.Point(0, 56);
            this.getApkButton.Name = "getApkButton";
            this.getApkButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.getApkButton.Size = new System.Drawing.Size(221, 28);
            this.getApkButton.TabIndex = 2;
            this.getApkButton.Text = "Share Selected App";
            this.getApkButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.getApkButton_Tooltip.SetToolTip(this.getApkButton, "Uploads the selected app to our Servers (Pick app within the Dropdown)");
            this.getApkButton.UseVisualStyleBackColor = false;
            this.getApkButton.Click += new System.EventHandler(this.getApkButton_Click);
            // 
            // uninstallAppButton
            // 
            this.uninstallAppButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.uninstallAppButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.uninstallAppButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.uninstallAppButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.uninstallAppButton.FlatAppearance.BorderSize = 0;
            this.uninstallAppButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uninstallAppButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.uninstallAppButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.uninstallAppButton.Location = new System.Drawing.Point(0, 84);
            this.uninstallAppButton.Name = "uninstallAppButton";
            this.uninstallAppButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.uninstallAppButton.Size = new System.Drawing.Size(221, 28);
            this.uninstallAppButton.TabIndex = 3;
            this.uninstallAppButton.Text = "Uninstall Selected App";
            this.uninstallAppButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uninstallAppButton_Tooltip.SetToolTip(this.uninstallAppButton, "Uninstalls the selected app (Select within the Dropdown)");
            this.uninstallAppButton.UseVisualStyleBackColor = false;
            this.uninstallAppButton.Click += new System.EventHandler(this.uninstallAppButton_Click);
            // 
            // pullAppToDesktopBtn
            // 
            this.pullAppToDesktopBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.pullAppToDesktopBtn.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.pullAppToDesktopBtn.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.pullAppToDesktopBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.pullAppToDesktopBtn.FlatAppearance.BorderSize = 0;
            this.pullAppToDesktopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pullAppToDesktopBtn.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.pullAppToDesktopBtn.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.pullAppToDesktopBtn.Location = new System.Drawing.Point(0, 112);
            this.pullAppToDesktopBtn.Name = "pullAppToDesktopBtn";
            this.pullAppToDesktopBtn.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pullAppToDesktopBtn.Size = new System.Drawing.Size(221, 28);
            this.pullAppToDesktopBtn.TabIndex = 4;
            this.pullAppToDesktopBtn.Text = "Pull App To Desktop";
            this.pullAppToDesktopBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pullAppToDesktopBtn_Tooltip.SetToolTip(this.pullAppToDesktopBtn, "Extracts APK and OBB to your Desktop (Select within the Dropdown)");
            this.pullAppToDesktopBtn.UseVisualStyleBackColor = false;
            this.pullAppToDesktopBtn.Click += new System.EventHandler(this.pullAppToDesktopBtn_Click);
            // 
            // copyBulkObbButton
            // 
            this.copyBulkObbButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.copyBulkObbButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.copyBulkObbButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.copyBulkObbButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.copyBulkObbButton.FlatAppearance.BorderSize = 0;
            this.copyBulkObbButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.copyBulkObbButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.copyBulkObbButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.copyBulkObbButton.Location = new System.Drawing.Point(0, 28);
            this.copyBulkObbButton.Name = "copyBulkObbButton";
            this.copyBulkObbButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.copyBulkObbButton.Size = new System.Drawing.Size(221, 28);
            this.copyBulkObbButton.TabIndex = 1;
            this.copyBulkObbButton.Text = "Recursive Copy OBB";
            this.copyBulkObbButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.copyBulkObbButton_Tooltip.SetToolTip(this.copyBulkObbButton, "Copies an multiple OBB folders to your device.");
            this.copyBulkObbButton.UseVisualStyleBackColor = false;
            this.copyBulkObbButton.Click += new System.EventHandler(this.copyBulkObbButton_Click);
            // 
            // aboutBtn
            // 
            this.aboutBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.aboutBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.aboutBtn.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.aboutBtn.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.aboutBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.aboutBtn.FlatAppearance.BorderSize = 0;
            this.aboutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutBtn.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.aboutBtn.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.aboutBtn.Location = new System.Drawing.Point(0, 563);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(221, 28);
            this.aboutBtn.TabIndex = 5;
            this.aboutBtn.Text = " ?   ABOUT";
            this.aboutBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.aboutBtn_Tooltip.SetToolTip(this.aboutBtn, "About the Rookie App and it\'s amazing creators and contributors");
            this.aboutBtn.UseVisualStyleBackColor = false;
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.settingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.settingsButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.settingsButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.settingsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.settingsButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.settingsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsButton.Location = new System.Drawing.Point(0, 535);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(221, 28);
            this.settingsButton.TabIndex = 4;
            this.settingsButton.Text = "⚙ SETTINGS";
            this.settingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsButton_Tooltip.SetToolTip(this.settingsButton, "Rookie App Settings");
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // InstallQUset
            // 
            this.InstallQUset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.InstallQUset.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.InstallQUset.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.InstallQUset.Dock = System.Windows.Forms.DockStyle.Top;
            this.InstallQUset.FlatAppearance.BorderSize = 0;
            this.InstallQUset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InstallQUset.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.InstallQUset.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.InstallQUset.Location = new System.Drawing.Point(0, 112);
            this.InstallQUset.Name = "InstallQUset";
            this.InstallQUset.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.InstallQUset.Size = new System.Drawing.Size(221, 28);
            this.InstallQUset.TabIndex = 5;
            this.InstallQUset.Text = "Install QU Setting";
            this.InstallQUset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.InstallQUset_Tooltip.SetToolTip(this.InstallQUset, "After you enter your username it will create an user.json file needed for some ga" +
        "mes");
            this.InstallQUset.UseVisualStyleBackColor = false;
            this.InstallQUset.Click += new System.EventHandler(this.InstallQUset_Click);
            // 
            // removeQUSetting
            // 
            this.removeQUSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.removeQUSetting.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.removeQUSetting.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.removeQUSetting.Dock = System.Windows.Forms.DockStyle.Top;
            this.removeQUSetting.FlatAppearance.BorderSize = 0;
            this.removeQUSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeQUSetting.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.removeQUSetting.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.removeQUSetting.Location = new System.Drawing.Point(0, 84);
            this.removeQUSetting.Name = "removeQUSetting";
            this.removeQUSetting.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.removeQUSetting.Size = new System.Drawing.Size(221, 28);
            this.removeQUSetting.TabIndex = 4;
            this.removeQUSetting.Text = "Remove QU Setting";
            this.removeQUSetting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeQUSetting_Tooltip.SetToolTip(this.removeQUSetting, "Remove QU Settings");
            this.removeQUSetting.UseVisualStyleBackColor = false;
            this.removeQUSetting.Click += new System.EventHandler(this.removeQUSetting_Click);
            // 
            // QuestOptionsButton
            // 
            this.QuestOptionsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.QuestOptionsButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QuestOptionsButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QuestOptionsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.QuestOptionsButton.FlatAppearance.BorderSize = 0;
            this.QuestOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QuestOptionsButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.QuestOptionsButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.QuestOptionsButton.Location = new System.Drawing.Point(0, 56);
            this.QuestOptionsButton.Name = "QuestOptionsButton";
            this.QuestOptionsButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.QuestOptionsButton.Size = new System.Drawing.Size(221, 28);
            this.QuestOptionsButton.TabIndex = 2;
            this.QuestOptionsButton.Text = "Quest Options";
            this.QuestOptionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.QuestOptionsButton_Tooltip.SetToolTip(this.QuestOptionsButton, "Additional Quest Settings and Utilities");
            this.QuestOptionsButton.UseVisualStyleBackColor = false;
            this.QuestOptionsButton.Click += new System.EventHandler(this.QuestOptionsButton_Click);
            // 
            // ADBWirelessDisable
            // 
            this.ADBWirelessDisable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ADBWirelessDisable.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBWirelessDisable.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBWirelessDisable.Dock = System.Windows.Forms.DockStyle.Top;
            this.ADBWirelessDisable.FlatAppearance.BorderSize = 0;
            this.ADBWirelessDisable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ADBWirelessDisable.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.ADBWirelessDisable.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.ADBWirelessDisable.Location = new System.Drawing.Point(0, 28);
            this.ADBWirelessDisable.Name = "ADBWirelessDisable";
            this.ADBWirelessDisable.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ADBWirelessDisable.Size = new System.Drawing.Size(221, 28);
            this.ADBWirelessDisable.TabIndex = 1;
            this.ADBWirelessDisable.Text = "Disable Wireless ADB";
            this.ADBWirelessDisable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ADBWirelessDisable_Tooltip.SetToolTip(this.ADBWirelessDisable, "Removes Wireless ADB settings and disconnects any Wireless devices");
            this.ADBWirelessDisable.UseVisualStyleBackColor = false;
            this.ADBWirelessDisable.Click += new System.EventHandler(this.ADBWirelessDisable_Click);
            // 
            // ADBWirelessEnable
            // 
            this.ADBWirelessEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ADBWirelessEnable.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBWirelessEnable.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ADBWirelessEnable.Dock = System.Windows.Forms.DockStyle.Top;
            this.ADBWirelessEnable.FlatAppearance.BorderSize = 0;
            this.ADBWirelessEnable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ADBWirelessEnable.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.ADBWirelessEnable.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.ADBWirelessEnable.Location = new System.Drawing.Point(0, 0);
            this.ADBWirelessEnable.Name = "ADBWirelessEnable";
            this.ADBWirelessEnable.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ADBWirelessEnable.Size = new System.Drawing.Size(221, 28);
            this.ADBWirelessEnable.TabIndex = 0;
            this.ADBWirelessEnable.Text = "Enable  Wireless ADB";
            this.ADBWirelessEnable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ADBWirelessEnable_Tooltip.SetToolTip(this.ADBWirelessEnable, "Enables Wireless sideloading. Requires a connected device to activate!");
            this.ADBWirelessEnable.UseVisualStyleBackColor = false;
            this.ADBWirelessEnable.Click += new System.EventHandler(this.ADBWirelessEnable_Click);
            // 
            // UpdateGamesButton
            // 
            this.UpdateGamesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.UpdateGamesButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UpdateGamesButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UpdateGamesButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.UpdateGamesButton.FlatAppearance.BorderSize = 0;
            this.UpdateGamesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateGamesButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.UpdateGamesButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.UpdateGamesButton.Location = new System.Drawing.Point(0, 196);
            this.UpdateGamesButton.Name = "UpdateGamesButton";
            this.UpdateGamesButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.UpdateGamesButton.Size = new System.Drawing.Size(221, 29);
            this.UpdateGamesButton.TabIndex = 7;
            this.UpdateGamesButton.Text = "Refresh Update List";
            this.UpdateGamesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.UpdateGamesButton_Tooltip.SetToolTip(this.UpdateGamesButton, "Refresh Game List and available updates");
            this.UpdateGamesButton.UseVisualStyleBackColor = false;
            this.UpdateGamesButton.Click += new System.EventHandler(this.UpdateGamesButton_Click);
            // 
            // listApkButton
            // 
            this.listApkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.listApkButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.listApkButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.listApkButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.listApkButton.FlatAppearance.BorderSize = 0;
            this.listApkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.listApkButton.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.listApkButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.listApkButton.Location = new System.Drawing.Point(0, 168);
            this.listApkButton.Name = "listApkButton";
            this.listApkButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.listApkButton.Size = new System.Drawing.Size(221, 28);
            this.listApkButton.TabIndex = 6;
            this.listApkButton.Text = "Refresh All";
            this.listApkButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.listApkButton_Tooltip.SetToolTip(this.listApkButton, "Refresh connected devices, installed apps, and update game list");
            this.listApkButton.UseVisualStyleBackColor = false;
            this.listApkButton.Click += new System.EventHandler(this.listApkButton_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.downloadInstallGameButton);
            this.panel2.Controls.Add(this.progressBar);
            this.panel2.Controls.Add(this.etaLabel);
            this.panel2.Controls.Add(this.speedLabel);
            this.panel2.Location = new System.Drawing.Point(224, 428);
            this.panel2.MinimumSize = new System.Drawing.Size(600, 34);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(798, 34);
            this.panel2.TabIndex = 96;
            // 
            // webView21
            // 
            this.webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(224, 467);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(374, 214);
            this.webView21.TabIndex = 97;
            this.webView21.ZoomFactor = 1D;
            // 
            // diskLabel
            // 
            this.diskLabel.AutoSize = true;
            this.diskLabel.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.diskLabel.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.diskLabel.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.diskLabel.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.diskLabel.Location = new System.Drawing.Point(2, 594);
            this.diskLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.diskLabel.Name = "diskLabel";
            this.diskLabel.Size = new System.Drawing.Size(77, 18);
            this.diskLabel.TabIndex = 7;
            this.diskLabel.Text = "Disk Label";
            // 
            // sideloadDrop
            // 
            this.sideloadDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.sideloadDrop.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_herringbone_2_1_3_0_0_90_1__000000_1c1c1c;
            this.sideloadDrop.Cursor = System.Windows.Forms.Cursors.Default;
            this.sideloadDrop.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sideloadDrop.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sideloadDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.sideloadDrop.FlatAppearance.BorderSize = 0;
            this.sideloadDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sideloadDrop.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.sideloadDrop.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.sideloadDrop.Location = new System.Drawing.Point(0, 28);
            this.sideloadDrop.Margin = new System.Windows.Forms.Padding(2);
            this.sideloadDrop.Name = "sideloadDrop";
            this.sideloadDrop.Size = new System.Drawing.Size(221, 28);
            this.sideloadDrop.TabIndex = 1;
            this.sideloadDrop.Text = "▶ SIDELOAD ◀";
            this.sideloadDrop.UseVisualStyleBackColor = false;
            this.sideloadDrop.Click += new System.EventHandler(this.sideloadContainer_Click);
            // 
            // sideloadContainer
            // 
            this.sideloadContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.sideloadContainer.Controls.Add(this.UpdateGamesButton);
            this.sideloadContainer.Controls.Add(this.listApkButton);
            this.sideloadContainer.Controls.Add(this.startsideloadbutton);
            this.sideloadContainer.Controls.Add(this.pullAppToDesktopBtn);
            this.sideloadContainer.Controls.Add(this.uninstallAppButton);
            this.sideloadContainer.Controls.Add(this.getApkButton);
            this.sideloadContainer.Controls.Add(this.copyBulkObbButton);
            this.sideloadContainer.Controls.Add(this.obbcopybutton);
            this.sideloadContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sideloadContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.sideloadContainer.Location = new System.Drawing.Point(0, 56);
            this.sideloadContainer.Margin = new System.Windows.Forms.Padding(2);
            this.sideloadContainer.Name = "sideloadContainer";
            this.sideloadContainer.Size = new System.Drawing.Size(221, 226);
            this.sideloadContainer.TabIndex = 74;
            // 
            // backupDrop
            // 
            this.backupDrop.BackColor = global::AndroidSideloader.Properties.Settings.Default.ButtonColor;
            this.backupDrop.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_herringbone_2_1_3_0_0_90_1__000000_1c1c1c;
            this.backupDrop.Cursor = System.Windows.Forms.Cursors.Default;
            this.backupDrop.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupDrop.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupDrop.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "ButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupDrop.FlatAppearance.BorderSize = 0;
            this.backupDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backupDrop.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.backupDrop.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.backupDrop.Location = new System.Drawing.Point(0, 282);
            this.backupDrop.Margin = new System.Windows.Forms.Padding(2);
            this.backupDrop.Name = "backupDrop";
            this.backupDrop.Size = new System.Drawing.Size(221, 28);
            this.backupDrop.TabIndex = 2;
            this.backupDrop.Text = "▶ BACKUP / RESTORE ◀";
            this.backupDrop.UseVisualStyleBackColor = false;
            this.backupDrop.Click += new System.EventHandler(this.backupDrop_Click);
            // 
            // backupContainer
            // 
            this.backupContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.backupContainer.Controls.Add(this.backupbutton);
            this.backupContainer.Controls.Add(this.restorebutton);
            this.backupContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupContainer.Location = new System.Drawing.Point(0, 310);
            this.backupContainer.Margin = new System.Windows.Forms.Padding(2);
            this.backupContainer.Name = "backupContainer";
            this.backupContainer.Size = new System.Drawing.Size(221, 56);
            this.backupContainer.TabIndex = 76;
            // 
            // otherDrop
            // 
            this.otherDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.otherDrop.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_herringbone_2_1_3_0_0_90_1__000000_1c1c1c;
            this.otherDrop.Cursor = System.Windows.Forms.Cursors.Default;
            this.otherDrop.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.otherDrop.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.otherDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.otherDrop.FlatAppearance.BorderSize = 0;
            this.otherDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.otherDrop.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.otherDrop.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.otherDrop.Location = new System.Drawing.Point(0, 366);
            this.otherDrop.Margin = new System.Windows.Forms.Padding(2);
            this.otherDrop.Name = "otherDrop";
            this.otherDrop.Size = new System.Drawing.Size(221, 28);
            this.otherDrop.TabIndex = 3;
            this.otherDrop.Text = "▶ OTHER ◀";
            this.otherDrop.UseVisualStyleBackColor = false;
            this.otherDrop.Click += new System.EventHandler(this.otherDrop_Click);
            // 
            // otherContainer
            // 
            this.otherContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.otherContainer.Controls.Add(this.InstallQUset);
            this.otherContainer.Controls.Add(this.removeQUSetting);
            this.otherContainer.Controls.Add(this.QuestOptionsButton);
            this.otherContainer.Controls.Add(this.ADBWirelessDisable);
            this.otherContainer.Controls.Add(this.ADBWirelessEnable);
            this.otherContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.otherContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.otherContainer.Location = new System.Drawing.Point(0, 394);
            this.otherContainer.Margin = new System.Windows.Forms.Padding(2);
            this.otherContainer.Name = "otherContainer";
            this.otherContainer.Size = new System.Drawing.Size(221, 141);
            this.otherContainer.TabIndex = 80;
            // 
            // downloadingLabel
            // 
            this.downloadingLabel.AutoSize = true;
            this.downloadingLabel.BackColor = System.Drawing.SystemColors.WindowText;
            this.downloadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.downloadingLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.downloadingLabel.Location = new System.Drawing.Point(-3, 688);
            this.downloadingLabel.Name = "downloadingLabel";
            this.downloadingLabel.Size = new System.Drawing.Size(0, 18);
            this.downloadingLabel.TabIndex = 83;
            // 
            // imgBattery
            // 
            this.imgBattery.BackColor = System.Drawing.Color.Transparent;
            this.imgBattery.BackgroundImage = global::AndroidSideloader.Properties.Resources.battery11;
            this.imgBattery.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.imgBattery.Location = new System.Drawing.Point(163, 595);
            this.imgBattery.Margin = new System.Windows.Forms.Padding(2);
            this.imgBattery.Name = "imgBattery";
            this.imgBattery.Size = new System.Drawing.Size(55, 29);
            this.imgBattery.TabIndex = 85;
            this.imgBattery.TabStop = false;
            // 
            // BatteryLbl
            // 
            this.BatteryLbl.AutoSize = true;
            this.BatteryLbl.BackColor = System.Drawing.Color.Transparent;
            this.BatteryLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BatteryLbl.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BatteryLbl.Location = new System.Drawing.Point(167, 601);
            this.BatteryLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BatteryLbl.Name = "BatteryLbl";
            this.BatteryLbl.Size = new System.Drawing.Size(0, 13);
            this.BatteryLbl.TabIndex = 84;
            // 
            // ULGif
            // 
            this.ULGif.Enabled = false;
            this.ULGif.Location = new System.Drawing.Point(178, 699);
            this.ULGif.Name = "ULGif";
            this.ULGif.Size = new System.Drawing.Size(34, 17);
            this.ULGif.TabIndex = 86;
            this.ULGif.TabStop = false;
            this.ULGif.Visible = false;
            // 
            // ULLabel
            // 
            this.ULLabel.AutoSize = true;
            this.ULLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ULLabel.ForeColor = System.Drawing.Color.Snow;
            this.ULLabel.Location = new System.Drawing.Point(154, 683);
            this.ULLabel.Name = "ULLabel";
            this.ULLabel.Size = new System.Drawing.Size(64, 13);
            this.ULLabel.TabIndex = 87;
            this.ULLabel.Text = "Uploading";
            this.ULLabel.Visible = false;
            // 
            // verLabel
            // 
            this.verLabel.AutoSize = true;
            this.verLabel.BackColor = System.Drawing.Color.Transparent;
            this.verLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.verLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.verLabel.Location = new System.Drawing.Point(3, 699);
            this.verLabel.Name = "verLabel";
            this.verLabel.Size = new System.Drawing.Size(0, 20);
            this.verLabel.TabIndex = 88;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panel1.Controls.Add(this.verLabel);
            this.panel1.Controls.Add(this.ULLabel);
            this.panel1.Controls.Add(this.ULGif);
            this.panel1.Controls.Add(this.BatteryLbl);
            this.panel1.Controls.Add(this.imgBattery);
            this.panel1.Controls.Add(this.downloadingLabel);
            this.panel1.Controls.Add(this.aboutBtn);
            this.panel1.Controls.Add(this.settingsButton);
            this.panel1.Controls.Add(this.otherContainer);
            this.panel1.Controls.Add(this.otherDrop);
            this.panel1.Controls.Add(this.backupContainer);
            this.panel1.Controls.Add(this.backupDrop);
            this.panel1.Controls.Add(this.sideloadContainer);
            this.panel1.Controls.Add(this.sideloadDrop);
            this.panel1.Controls.Add(this.devicesbutton);
            this.panel1.Controls.Add(this.diskLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 721);
            this.panel1.TabIndex = 73;
            // 
            // downloadInstallGameButton
            // 
            this.downloadInstallGameButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.downloadInstallGameButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.downloadInstallGameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadInstallGameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.downloadInstallGameButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.downloadInstallGameButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.downloadInstallGameButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.downloadInstallGameButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.downloadInstallGameButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.downloadInstallGameButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.downloadInstallGameButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.downloadInstallGameButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.downloadInstallGameButton.Location = new System.Drawing.Point(434, 0);
            this.downloadInstallGameButton.Margin = new System.Windows.Forms.Padding(0);
            this.downloadInstallGameButton.Name = "downloadInstallGameButton";
            this.downloadInstallGameButton.Radius = 5;
            this.downloadInstallGameButton.Size = new System.Drawing.Size(364, 34);
            this.downloadInstallGameButton.Stroke = true;
            this.downloadInstallGameButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.downloadInstallGameButton.TabIndex = 94;
            this.downloadInstallGameButton.Text = "Download and Install Game/Add To Queue ⮩ ";
            this.downloadInstallGameButton.Transparency = false;
            this.downloadInstallGameButton.Click += new System.EventHandler(this.downloadInstallGameButton_Click);
            this.downloadInstallGameButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.downloadInstallGameButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            // 
            // MountButton
            // 
            this.MountButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.MountButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.MountButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MountButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MountButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.MountButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MountButton.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MountButton.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MountButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.MountButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.MountButton.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.MountButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MountButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MountButton.Location = new System.Drawing.Point(393, 39);
            this.MountButton.Name = "MountButton";
            this.MountButton.Radius = 5;
            this.MountButton.Size = new System.Drawing.Size(76, 26);
            this.MountButton.Stroke = true;
            this.MountButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.MountButton.TabIndex = 95;
            this.MountButton.Text = "MOUNT";
            this.MountButton.Transparency = false;
            this.MountButton.Click += new System.EventHandler(this.MountButton_Click);
            this.MountButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = global::AndroidSideloader.Properties.Settings.Default.BackColor;
            this.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_cubes_1_1_1_0_0_0_1__000000_212121;
            this.ClientSize = new System.Drawing.Size(1040, 721);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.MountButton);
            this.Controls.Add(this.lblShortcutsF2);
            this.Controls.Add(this.EnterInstallBox);
            this.Controls.Add(this.ProgressText);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblNeedsDonate);
            this.Controls.Add(this.lblUpToDate);
            this.Controls.Add(this.lblUpdateAvailable);
            this.Controls.Add(this.lblMirror);
            this.Controls.Add(this.lblQuickSearch);
            this.Controls.Add(this.lblShortcutCtrlR);
            this.Controls.Add(this.ADBcommandbox);
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.lblAdbCommand);
            this.Controls.Add(this.lblSearchHelp);
            this.Controls.Add(this.freeDisclaimer);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.DragDropLbl);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.gamesQueueLabel);
            this.Controls.Add(this.gamesPictureBox);
            this.Controls.Add(this.remotesList);
            this.Controls.Add(this.devicesComboBox);
            this.Controls.Add(this.gamesQueListBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_combo);
            this.Controls.Add(this.notesRichTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gamesListView);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox7);
            this.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "BackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1048, 760);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rookie\'s Sideloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.DragLeave += new System.EventHandler(this.Form1_DragLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesPictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.sideloadContainer.ResumeLayout(false);
            this.backupContainer.ResumeLayout(false);
            this.otherContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgBattery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ULGif)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private SergeUtils.EasyCompletionComboBox m_combo;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label etaLabel;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Label freeDisclaimer;
        private System.Windows.Forms.ComboBox devicesComboBox;
        private System.Windows.Forms.ListBox gamesQueListBox;
        private System.Windows.Forms.ListView gamesListView;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.PictureBox gamesPictureBox;
        private System.Windows.Forms.Label gamesQueueLabel;
        private System.Windows.Forms.Label ProgressText;
        private System.Windows.Forms.RichTextBox notesRichTextBox;
        private System.Windows.Forms.Label DragDropLbl;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblSearchHelp;
        private System.Windows.Forms.Label lblShortcutsF2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblQuickSearch;
        private System.Windows.Forms.PictureBox btnSearch;
        private System.Windows.Forms.Label lblUpdateAvailable;
        private System.Windows.Forms.Label lblUpToDate;
        private System.Windows.Forms.Label lblMirror;
        private System.Windows.Forms.CheckBox EnterInstallBox;
        private System.Windows.Forms.TextBox ADBcommandbox;
        private System.Windows.Forms.Label lblShortcutCtrlR;
        private System.Windows.Forms.Label lblAdbCommand;
        public System.Windows.Forms.ComboBox remotesList;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.Label lblNeedsDonate;
        public System.Windows.Forms.ColumnHeader GameNameIndex;
        public System.Windows.Forms.ColumnHeader ReleaseNameIndex;
        private System.Windows.Forms.ColumnHeader PackageNameIndex;
        private System.Windows.Forms.ColumnHeader VersionCodeIndex;
        private System.Windows.Forms.ColumnHeader ReleaseAPKPathIndex;
        public System.Windows.Forms.ColumnHeader VersionNameIndex;
        private RoundButton downloadInstallGameButton;
        private RoundButton MountButton;
        private ToolTip startsideloadbutton_Tooltip;
        private ToolTip devicesbutton_Tooltip;
        private ToolTip obbcopybutton_Tooltip;
        private ToolTip backupbutton_Tooltip;
        private ToolTip restorebutton_Tooltip;
        private ToolTip getApkButton_Tooltip;
        private ToolTip uninstallAppButton_Tooltip;
        private ToolTip pullAppToDesktopBtn_Tooltip;
        private ToolTip copyBulkObbButton_Tooltip;
        private ToolTip aboutBtn_Tooltip;
        private ToolTip settingsButton_Tooltip;
        private ToolTip InstallQUset_Tooltip;
        private ToolTip removeQUSetting_Tooltip;
        private ToolTip QuestOptionsButton_Tooltip;
        private ToolTip ADBWirelessDisable_Tooltip;
        private ToolTip ADBWirelessEnable_Tooltip;
        private ToolTip UpdateGamesButton_Tooltip;
        private ToolTip listApkButton_Tooltip;
        private ToolTip speedLabel_Tooltip;
        private ToolTip etaLabel_Tooltip;
        private ToolTip EnterInstallBox_Tooltip;
        private Panel panel2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Label diskLabel;
        private Button devicesbutton;
        private Button sideloadDrop;
        private Panel sideloadContainer;
        private Button UpdateGamesButton;
        private Button listApkButton;
        private Button startsideloadbutton;
        private Button pullAppToDesktopBtn;
        private Button uninstallAppButton;
        private Button getApkButton;
        private Button copyBulkObbButton;
        private Button obbcopybutton;
        private Button backupDrop;
        private Panel backupContainer;
        private Button backupbutton;
        private Button restorebutton;
        private Button otherDrop;
        private Panel otherContainer;
        private Button InstallQUset;
        private Button removeQUSetting;
        private Button QuestOptionsButton;
        private Button ADBWirelessDisable;
        private Button ADBWirelessEnable;
        private Button settingsButton;
        private Button aboutBtn;
        private Label downloadingLabel;
        private PictureBox imgBattery;
        private Label BatteryLbl;
        private PictureBox ULGif;
        private Label ULLabel;
        private Label verLabel;
        private Panel panel1;
    }
}
