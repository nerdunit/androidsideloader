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
            this.speedLabel = new System.Windows.Forms.Label();
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
            this.DownloadsIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gamesQueueLabel = new System.Windows.Forms.Label();
            this.notesRichTextBox = new System.Windows.Forms.RichTextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.gamesPictureBox = new System.Windows.Forms.PictureBox();
            this.startsideloadbutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.startsideloadbutton = new System.Windows.Forms.Button();
            this.devicesbutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.devicesbutton = new System.Windows.Forms.Button();
            this.obbcopybutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.obbcopybutton = new System.Windows.Forms.Button();
            this.backupadbbutton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.backupadbbutton = new System.Windows.Forms.Button();
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
            this.QuestOptionsButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.QuestOptionsButton = new System.Windows.Forms.Button();
            this.btnOpenDownloads_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnOpenDownloads = new System.Windows.Forms.Button();
            this.btnRunAdbCmd_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnRunAdbCmd = new System.Windows.Forms.Button();
            this.ADBWirelessToggle_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ADBWirelessToggle = new System.Windows.Forms.Button();
            this.UpdateGamesButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.UpdateGamesButton = new System.Windows.Forms.Button();
            this.listApkButton_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.listApkButton = new System.Windows.Forms.Button();
            this.speedLabel_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.etaLabel_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.progressDLbtnContainer = new System.Windows.Forms.Panel();
            this.diskLabel = new System.Windows.Forms.Label();
            this.questStorageProgressBar = new System.Windows.Forms.Panel();
            this.batteryLevImg = new System.Windows.Forms.PictureBox();
            this.deviceDrop = new System.Windows.Forms.Button();
            this.deviceDropContainer = new System.Windows.Forms.Panel();
            this.mountDeviceButton = new System.Windows.Forms.Button();
            this.selectDeviceButton = new System.Windows.Forms.Button();
            this.sideloadDrop = new System.Windows.Forms.Button();
            this.sideloadContainer = new System.Windows.Forms.Panel();
            this.btnNoDevice = new System.Windows.Forms.Button();
            this.installedAppsMenu = new System.Windows.Forms.Button();
            this.installedAppsMenuContainer = new System.Windows.Forms.Panel();
            this.backupDrop = new System.Windows.Forms.Button();
            this.backupContainer = new System.Windows.Forms.Panel();
            this.otherDrop = new System.Windows.Forms.Button();
            this.otherContainer = new System.Windows.Forms.Panel();
            this.selectMirrorButton = new System.Windows.Forms.Button();
            this.questInfoPanel = new System.Windows.Forms.Panel();
            this.batteryLabel = new System.Windows.Forms.Label();
            this.questInfoLabel = new System.Windows.Forms.Label();
            this.ULLabel = new System.Windows.Forms.Label();
            this.leftNavContainer = new System.Windows.Forms.Panel();
            this.statusInfoPanel = new System.Windows.Forms.Panel();
            this.sideloadingStatusLabel = new System.Windows.Forms.Label();
            this.activeMirrorLabel = new System.Windows.Forms.Label();
            this.deviceIdLabel = new System.Windows.Forms.Label();
            this.rookieStatusLabel = new System.Windows.Forms.Label();
            this.sidebarMediaPanel = new System.Windows.Forms.Panel();
            this.selectedGameLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.favoriteGame = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.favoriteButton = new System.Windows.Forms.ToolStripMenuItem();
            this.gamesGalleryView = new System.Windows.Forms.FlowLayoutPanel();
            this.btnViewToggle_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.webViewPlaceholderPanel = new System.Windows.Forms.Panel();
            this.searchPanel = new AndroidSideloader.RoundButton();
            this.searchIconPictureBox = new System.Windows.Forms.PictureBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.btnViewToggle = new AndroidSideloader.RoundButton();
            this.favoriteSwitcher = new AndroidSideloader.RoundButton();
            this.btnInstalled = new AndroidSideloader.RoundButton();
            this.btnUpdateAvailable = new AndroidSideloader.RoundButton();
            this.btnNewerThanList = new AndroidSideloader.RoundButton();
            this.btnDownloaded = new AndroidSideloader.RoundButton();
            this.progressBar = new AndroidSideloader.ModernProgressBar();
            this.downloadInstallGameButton = new AndroidSideloader.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.gamesPictureBox)).BeginInit();
            this.gamesPictureBox.SuspendLayout();
            this.progressDLbtnContainer.SuspendLayout();
            this.questStorageProgressBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.batteryLevImg)).BeginInit();
            this.deviceDropContainer.SuspendLayout();
            this.sideloadContainer.SuspendLayout();
            this.installedAppsMenuContainer.SuspendLayout();
            this.backupContainer.SuspendLayout();
            this.otherContainer.SuspendLayout();
            this.questInfoPanel.SuspendLayout();
            this.leftNavContainer.SuspendLayout();
            this.statusInfoPanel.SuspendLayout();
            this.sidebarMediaPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.favoriteGame.SuspendLayout();
            this.searchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchIconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // m_combo
            // 
            this.m_combo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.m_combo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_combo.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.m_combo.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.m_combo.Location = new System.Drawing.Point(253, 9);
            this.m_combo.Name = "m_combo";
            this.m_combo.Size = new System.Drawing.Size(374, 24);
            this.m_combo.TabIndex = 0;
            this.m_combo.Text = "Select an Installed App...";
            this.m_combo.Visible = false;
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.BackColor = System.Drawing.Color.Transparent;
            this.speedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.speedLabel.ForeColor = System.Drawing.Color.White;
            this.speedLabel.Location = new System.Drawing.Point(-1, 3);
            this.speedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(152, 16);
            this.speedLabel.TabIndex = 76;
            this.speedLabel.Text = "DLS: Speed in MBPS";
            this.speedLabel_Tooltip.SetToolTip(this.speedLabel, "Current download speed, updates every second, in mbps");
            // 
            // freeDisclaimer
            // 
            this.freeDisclaimer.BackColor = System.Drawing.Color.Transparent;
            this.freeDisclaimer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.freeDisclaimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.freeDisclaimer.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.freeDisclaimer.ForeColor = System.Drawing.Color.White;
            this.freeDisclaimer.Location = new System.Drawing.Point(0, 0);
            this.freeDisclaimer.Name = "freeDisclaimer";
            this.freeDisclaimer.Size = new System.Drawing.Size(238, 136);
            this.freeDisclaimer.TabIndex = 0;
            this.freeDisclaimer.Text = "THIS APP IS FREE!\r\nClick here to go to GitHub";
            this.freeDisclaimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.freeDisclaimer.Click += new System.EventHandler(this.freeDisclaimer_Click);
            // 
            // gamesQueListBox
            // 
            this.gamesQueListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gamesQueListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.gamesQueListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gamesQueListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.gamesQueListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.gamesQueListBox.ForeColor = System.Drawing.Color.White;
            this.gamesQueListBox.FormattingEnabled = true;
            this.gamesQueListBox.ItemHeight = 24;
            this.gamesQueListBox.Location = new System.Drawing.Point(654, 496);
            this.gamesQueListBox.Margin = new System.Windows.Forms.Padding(2);
            this.gamesQueListBox.Name = "gamesQueListBox";
            this.gamesQueListBox.Size = new System.Drawing.Size(266, 192);
            this.gamesQueListBox.TabIndex = 9;
            // 
            // devicesComboBox
            // 
            this.devicesComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.devicesComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.devicesComboBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.devicesComboBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.devicesComboBox.FormattingEnabled = true;
            this.devicesComboBox.Location = new System.Drawing.Point(253, 39);
            this.devicesComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.devicesComboBox.Name = "devicesComboBox";
            this.devicesComboBox.Size = new System.Drawing.Size(164, 24);
            this.devicesComboBox.TabIndex = 1;
            this.devicesComboBox.Text = "Select your device";
            this.devicesComboBox.Visible = false;
            this.devicesComboBox.SelectedIndexChanged += new System.EventHandler(this.devicesComboBox_SelectedIndexChanged);
            // 
            // remotesList
            // 
            this.remotesList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.remotesList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.remotesList.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.remotesList.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.remotesList.FormattingEnabled = true;
            this.remotesList.Location = new System.Drawing.Point(567, 40);
            this.remotesList.Margin = new System.Windows.Forms.Padding(2);
            this.remotesList.Name = "remotesList";
            this.remotesList.Size = new System.Drawing.Size(67, 24);
            this.remotesList.TabIndex = 3;
            this.remotesList.Visible = false;
            this.remotesList.SelectedIndexChanged += new System.EventHandler(this.remotesList_SelectedIndexChanged);
            // 
            // gamesListView
            // 
            this.gamesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamesListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.gamesListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gamesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GameNameIndex,
            this.ReleaseNameIndex,
            this.PackageNameIndex,
            this.VersionCodeIndex,
            this.ReleaseAPKPathIndex,
            this.VersionNameIndex,
            this.DownloadsIndex});
            this.gamesListView.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gamesListView.ForeColor = System.Drawing.Color.White;
            this.gamesListView.FullRowSelect = true;
            this.gamesListView.HideSelection = false;
            this.gamesListView.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.gamesListView.Location = new System.Drawing.Point(258, 44);
            this.gamesListView.Name = "gamesListView";
            this.gamesListView.OwnerDraw = true;
            this.gamesListView.ShowGroups = false;
            this.gamesListView.Size = new System.Drawing.Size(984, 409);
            this.gamesListView.TabIndex = 6;
            this.gamesListView.UseCompatibleStateImageBehavior = false;
            this.gamesListView.View = System.Windows.Forms.View.Details;
            this.gamesListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.gamesListView.SelectedIndexChanged += new System.EventHandler(this.gamesListView_SelectedIndexChanged);
            this.gamesListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.gamesListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.gamesListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gamesListView_MouseClick);
            this.gamesListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gamesListView_MouseDoubleClick);
            // 
            // GameNameIndex
            // 
            this.GameNameIndex.Text = "Game Name";
            this.GameNameIndex.Width = 160;
            // 
            // ReleaseNameIndex
            // 
            this.ReleaseNameIndex.Text = "Release Name";
            this.ReleaseNameIndex.Width = 220;
            // 
            // PackageNameIndex
            // 
            this.PackageNameIndex.Text = "Package Name";
            this.PackageNameIndex.Width = 120;
            // 
            // VersionCodeIndex
            // 
            this.VersionCodeIndex.Text = "Version (Rookie/Local)";
            this.VersionCodeIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VersionCodeIndex.Width = 164;
            // 
            // ReleaseAPKPathIndex
            // 
            this.ReleaseAPKPathIndex.Text = "Last Updated";
            this.ReleaseAPKPathIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ReleaseAPKPathIndex.Width = 135;
            // 
            // VersionNameIndex
            // 
            this.VersionNameIndex.Text = "Size";
            this.VersionNameIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VersionNameIndex.Width = 85;
            // 
            // DownloadsIndex
            // 
            this.DownloadsIndex.Text = "Popularity";
            this.DownloadsIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DownloadsIndex.Width = 100;
            // 
            // gamesQueueLabel
            // 
            this.gamesQueueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gamesQueueLabel.AutoSize = true;
            this.gamesQueueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.gamesQueueLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gamesQueueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(150)))));
            this.gamesQueueLabel.Location = new System.Drawing.Point(669, 689);
            this.gamesQueueLabel.Name = "gamesQueueLabel";
            this.gamesQueueLabel.Size = new System.Drawing.Size(103, 15);
            this.gamesQueueLabel.TabIndex = 86;
            this.gamesQueueLabel.Text = "Download Queue";
            // 
            // notesRichTextBox
            // 
            this.notesRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notesRichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.notesRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.notesRichTextBox.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.notesRichTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.notesRichTextBox.HideSelection = false;
            this.notesRichTextBox.Location = new System.Drawing.Point(954, 496);
            this.notesRichTextBox.Name = "notesRichTextBox";
            this.notesRichTextBox.ReadOnly = true;
            this.notesRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.notesRichTextBox.ShowSelectionMargin = true;
            this.notesRichTextBox.Size = new System.Drawing.Size(265, 192);
            this.notesRichTextBox.TabIndex = 10;
            this.notesRichTextBox.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.notesRichTextBox.Text = "\n\n\n\n\nTip: Press F1 to see all shortcuts\n\nDrag and drop APKs or folders to install" +
    "";
            this.notesRichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.notesRichTextBox_LinkClicked);
            // 
            // lblNotes
            // 
            this.lblNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNotes.AutoSize = true;
            this.lblNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.lblNotes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNotes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(150)))));
            this.lblNotes.Location = new System.Drawing.Point(966, 689);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(86, 15);
            this.lblNotes.TabIndex = 86;
            this.lblNotes.Text = "Release Notes";
            // 
            // gamesPictureBox
            // 
            this.gamesPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.gamesPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.gamesPictureBox.Controls.Add(this.freeDisclaimer);
            this.gamesPictureBox.Location = new System.Drawing.Point(6, 34);
            this.gamesPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.gamesPictureBox.Name = "gamesPictureBox";
            this.gamesPictureBox.Size = new System.Drawing.Size(238, 136);
            this.gamesPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.gamesPictureBox.TabIndex = 84;
            this.gamesPictureBox.TabStop = false;
            this.gamesPictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.gamesPictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.gamesPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gamesPictureBox_Paint);
            // 
            // startsideloadbutton
            // 
            this.startsideloadbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.startsideloadbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startsideloadbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.startsideloadbutton.FlatAppearance.BorderSize = 0;
            this.startsideloadbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startsideloadbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.startsideloadbutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.startsideloadbutton.Location = new System.Drawing.Point(0, 84);
            this.startsideloadbutton.Name = "startsideloadbutton";
            this.startsideloadbutton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.startsideloadbutton.Size = new System.Drawing.Size(233, 28);
            this.startsideloadbutton.TabIndex = 5;
            this.startsideloadbutton.Text = "SIDELOAD APK";
            this.startsideloadbutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startsideloadbutton_Tooltip.SetToolTip(this.startsideloadbutton, "Sideload an APK onto your device");
            this.startsideloadbutton.UseVisualStyleBackColor = false;
            this.startsideloadbutton.Click += new System.EventHandler(this.startsideloadbutton_Click);
            // 
            // devicesbutton
            // 
            this.devicesbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.devicesbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.devicesbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.devicesbutton.FlatAppearance.BorderSize = 0;
            this.devicesbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.devicesbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.devicesbutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.devicesbutton.Location = new System.Drawing.Point(0, 56);
            this.devicesbutton.Name = "devicesbutton";
            this.devicesbutton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.devicesbutton.Size = new System.Drawing.Size(233, 28);
            this.devicesbutton.TabIndex = 0;
            this.devicesbutton.Text = "RECONNECT DEVICE";
            this.devicesbutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.devicesbutton_Tooltip.SetToolTip(this.devicesbutton, "Rookie will attempt to reconnect to your device");
            this.devicesbutton.UseVisualStyleBackColor = false;
            this.devicesbutton.Click += new System.EventHandler(this.devicesbutton_Click);
            // 
            // obbcopybutton
            // 
            this.obbcopybutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.obbcopybutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.obbcopybutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.obbcopybutton.FlatAppearance.BorderSize = 0;
            this.obbcopybutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.obbcopybutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.obbcopybutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.obbcopybutton.Location = new System.Drawing.Point(0, 28);
            this.obbcopybutton.Name = "obbcopybutton";
            this.obbcopybutton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.obbcopybutton.Size = new System.Drawing.Size(233, 28);
            this.obbcopybutton.TabIndex = 0;
            this.obbcopybutton.Text = "COPY OBB";
            this.obbcopybutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.obbcopybutton_Tooltip.SetToolTip(this.obbcopybutton, "Copies an OBB folder to the Android/obb folder from the device (Not all games use" +
        " OBB files)");
            this.obbcopybutton.UseVisualStyleBackColor = false;
            this.obbcopybutton.Click += new System.EventHandler(this.obbcopybutton_Click);
            // 
            // backupadbbutton
            // 
            this.backupadbbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.backupadbbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.backupadbbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupadbbutton.FlatAppearance.BorderSize = 0;
            this.backupadbbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backupadbbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.backupadbbutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.backupadbbutton.Location = new System.Drawing.Point(0, 28);
            this.backupadbbutton.Name = "backupadbbutton";
            this.backupadbbutton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.backupadbbutton.Size = new System.Drawing.Size(233, 28);
            this.backupadbbutton.TabIndex = 1;
            this.backupadbbutton.Text = "BACKUP GAMESAVE WITH ADB";
            this.backupadbbutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.backupadbbutton_Tooltip.SetToolTip(this.backupadbbutton, "Save game data via ADB-Backup");
            this.backupadbbutton.UseVisualStyleBackColor = false;
            this.backupadbbutton.Click += new System.EventHandler(this.backupadbbutton_Click);
            // 
            // backupbutton
            // 
            this.backupbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.backupbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.backupbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupbutton.FlatAppearance.BorderSize = 0;
            this.backupbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backupbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.backupbutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.backupbutton.Location = new System.Drawing.Point(0, 0);
            this.backupbutton.Name = "backupbutton";
            this.backupbutton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.backupbutton.Size = new System.Drawing.Size(233, 28);
            this.backupbutton.TabIndex = 1;
            this.backupbutton.Text = "BACKUP ALL GAMESAVES";
            this.backupbutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.backupbutton_Tooltip.SetToolTip(this.backupbutton, "Save game and apps data to the backup folder (Does not save APKs or OBBs)");
            this.backupbutton.UseVisualStyleBackColor = false;
            this.backupbutton.Click += new System.EventHandler(this.backupbutton_Click);
            // 
            // restorebutton
            // 
            this.restorebutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.restorebutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.restorebutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.restorebutton.FlatAppearance.BorderSize = 0;
            this.restorebutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restorebutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.restorebutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.restorebutton.Location = new System.Drawing.Point(0, 56);
            this.restorebutton.Name = "restorebutton";
            this.restorebutton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.restorebutton.Size = new System.Drawing.Size(233, 28);
            this.restorebutton.TabIndex = 0;
            this.restorebutton.Text = "RESTORE GAMESAVES";
            this.restorebutton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.restorebutton_Tooltip.SetToolTip(this.restorebutton, "Restore game and apps data to the device");
            this.restorebutton.UseVisualStyleBackColor = false;
            this.restorebutton.Click += new System.EventHandler(this.restorebutton_Click);
            // 
            // getApkButton
            // 
            this.getApkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.getApkButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.getApkButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.getApkButton.FlatAppearance.BorderSize = 0;
            this.getApkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.getApkButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.getApkButton.Location = new System.Drawing.Point(0, 0);
            this.getApkButton.Name = "getApkButton";
            this.getApkButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.getApkButton.Size = new System.Drawing.Size(233, 28);
            this.getApkButton.TabIndex = 2;
            this.getApkButton.Text = "SHARE APP";
            this.getApkButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.getApkButton_Tooltip.SetToolTip(this.getApkButton, "Upload an app to our servers");
            this.getApkButton.UseVisualStyleBackColor = false;
            this.getApkButton.Click += new System.EventHandler(this.getApkButton_Click);
            // 
            // uninstallAppButton
            // 
            this.uninstallAppButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.uninstallAppButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uninstallAppButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.uninstallAppButton.FlatAppearance.BorderSize = 0;
            this.uninstallAppButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uninstallAppButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.uninstallAppButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.uninstallAppButton.Location = new System.Drawing.Point(0, 28);
            this.uninstallAppButton.Name = "uninstallAppButton";
            this.uninstallAppButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.uninstallAppButton.Size = new System.Drawing.Size(233, 28);
            this.uninstallAppButton.TabIndex = 3;
            this.uninstallAppButton.Text = "UNINSTALL APP";
            this.uninstallAppButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uninstallAppButton_Tooltip.SetToolTip(this.uninstallAppButton, "Uninstall an app");
            this.uninstallAppButton.UseVisualStyleBackColor = false;
            this.uninstallAppButton.Click += new System.EventHandler(this.uninstallAppButton_Click);
            // 
            // pullAppToDesktopBtn
            // 
            this.pullAppToDesktopBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.pullAppToDesktopBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pullAppToDesktopBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.pullAppToDesktopBtn.FlatAppearance.BorderSize = 0;
            this.pullAppToDesktopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pullAppToDesktopBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.pullAppToDesktopBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.pullAppToDesktopBtn.Location = new System.Drawing.Point(0, 56);
            this.pullAppToDesktopBtn.Name = "pullAppToDesktopBtn";
            this.pullAppToDesktopBtn.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.pullAppToDesktopBtn.Size = new System.Drawing.Size(233, 28);
            this.pullAppToDesktopBtn.TabIndex = 4;
            this.pullAppToDesktopBtn.Text = "PULL APP TO DESKTOP";
            this.pullAppToDesktopBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pullAppToDesktopBtn_Tooltip.SetToolTip(this.pullAppToDesktopBtn, "Extract APK and OBB to your desktop");
            this.pullAppToDesktopBtn.UseVisualStyleBackColor = false;
            this.pullAppToDesktopBtn.Click += new System.EventHandler(this.pullAppToDesktopBtn_Click);
            // 
            // copyBulkObbButton
            // 
            this.copyBulkObbButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.copyBulkObbButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.copyBulkObbButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.copyBulkObbButton.FlatAppearance.BorderSize = 0;
            this.copyBulkObbButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.copyBulkObbButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.copyBulkObbButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.copyBulkObbButton.Location = new System.Drawing.Point(0, 56);
            this.copyBulkObbButton.Name = "copyBulkObbButton";
            this.copyBulkObbButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.copyBulkObbButton.Size = new System.Drawing.Size(233, 28);
            this.copyBulkObbButton.TabIndex = 1;
            this.copyBulkObbButton.Text = "RECURSIVE COPY OBB";
            this.copyBulkObbButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.copyBulkObbButton_Tooltip.SetToolTip(this.copyBulkObbButton, "Copy multiple OBB folders to your device");
            this.copyBulkObbButton.UseVisualStyleBackColor = false;
            this.copyBulkObbButton.Click += new System.EventHandler(this.copyBulkObbButton_Click);
            // 
            // aboutBtn
            // 
            this.aboutBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.aboutBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.aboutBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.aboutBtn.FlatAppearance.BorderSize = 0;
            this.aboutBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(51)))), ((int)(((byte)(65)))));
            this.aboutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.aboutBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.aboutBtn.Location = new System.Drawing.Point(0, 991);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.aboutBtn.Size = new System.Drawing.Size(233, 28);
            this.aboutBtn.TabIndex = 5;
            this.aboutBtn.Text = "ABOUT";
            this.aboutBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.aboutBtn_Tooltip.SetToolTip(this.aboutBtn, "About Rookie, it\'s amazing creators and contributors");
            this.aboutBtn.UseVisualStyleBackColor = false;
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.settingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.settingsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.settingsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.settingsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsButton.Location = new System.Drawing.Point(0, 140);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.settingsButton.Size = new System.Drawing.Size(233, 28);
            this.settingsButton.TabIndex = 8;
            this.settingsButton.Text = "ROOKIE SETTINGS";
            this.settingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsButton_Tooltip.SetToolTip(this.settingsButton, "Rookie App Settings");
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // QuestOptionsButton
            // 
            this.QuestOptionsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.QuestOptionsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.QuestOptionsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.QuestOptionsButton.FlatAppearance.BorderSize = 0;
            this.QuestOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QuestOptionsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.QuestOptionsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.QuestOptionsButton.Location = new System.Drawing.Point(0, 112);
            this.QuestOptionsButton.Name = "QuestOptionsButton";
            this.QuestOptionsButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.QuestOptionsButton.Size = new System.Drawing.Size(233, 28);
            this.QuestOptionsButton.TabIndex = 2;
            this.QuestOptionsButton.Text = "QUEST SETTINGS";
            this.QuestOptionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.QuestOptionsButton_Tooltip.SetToolTip(this.QuestOptionsButton, "Quest Settings and Utilities");
            this.QuestOptionsButton.UseVisualStyleBackColor = false;
            this.QuestOptionsButton.Click += new System.EventHandler(this.QuestOptionsButton_Click);
            // 
            // btnOpenDownloads
            // 
            this.btnOpenDownloads.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.btnOpenDownloads.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenDownloads.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpenDownloads.FlatAppearance.BorderSize = 0;
            this.btnOpenDownloads.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenDownloads.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnOpenDownloads.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnOpenDownloads.Location = new System.Drawing.Point(0, 28);
            this.btnOpenDownloads.Name = "btnOpenDownloads";
            this.btnOpenDownloads.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnOpenDownloads.Size = new System.Drawing.Size(233, 28);
            this.btnOpenDownloads.TabIndex = 7;
            this.btnOpenDownloads.Text = "OPEN DOWNLOADS FOLDER";
            this.btnOpenDownloads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenDownloads_Tooltip.SetToolTip(this.btnOpenDownloads, "Open your set Rookie Download Folder");
            this.btnOpenDownloads.UseVisualStyleBackColor = false;
            this.btnOpenDownloads.Click += new System.EventHandler(this.btnOpenDownloads_Click);
            // 
            // btnRunAdbCmd
            // 
            this.btnRunAdbCmd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.btnRunAdbCmd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRunAdbCmd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRunAdbCmd.FlatAppearance.BorderSize = 0;
            this.btnRunAdbCmd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunAdbCmd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnRunAdbCmd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnRunAdbCmd.Location = new System.Drawing.Point(0, 56);
            this.btnRunAdbCmd.Name = "btnRunAdbCmd";
            this.btnRunAdbCmd.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnRunAdbCmd.Size = new System.Drawing.Size(233, 28);
            this.btnRunAdbCmd.TabIndex = 6;
            this.btnRunAdbCmd.Text = "RUN ADB COMMAND";
            this.btnRunAdbCmd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRunAdbCmd_Tooltip.SetToolTip(this.btnRunAdbCmd, "Open the \'Run ADB Command\' prompt");
            this.btnRunAdbCmd.UseVisualStyleBackColor = false;
            this.btnRunAdbCmd.Click += new System.EventHandler(this.btnRunAdbCmd_Click);
            // 
            // ADBWirelessToggle
            // 
            this.ADBWirelessToggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.ADBWirelessToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ADBWirelessToggle.Dock = System.Windows.Forms.DockStyle.Top;
            this.ADBWirelessToggle.FlatAppearance.BorderSize = 0;
            this.ADBWirelessToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ADBWirelessToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ADBWirelessToggle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.ADBWirelessToggle.Location = new System.Drawing.Point(0, 0);
            this.ADBWirelessToggle.Name = "ADBWirelessToggle";
            this.ADBWirelessToggle.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.ADBWirelessToggle.Size = new System.Drawing.Size(233, 28);
            this.ADBWirelessToggle.TabIndex = 0;
            this.ADBWirelessToggle.Text = "TOGGLE WIRELESS ADB";
            this.ADBWirelessToggle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ADBWirelessToggle_Tooltip.SetToolTip(this.ADBWirelessToggle, "Enable or disable wireless ADB connection");
            this.ADBWirelessToggle.UseVisualStyleBackColor = false;
            this.ADBWirelessToggle.Click += new System.EventHandler(this.ADBWirelessToggle_Click);
            // 
            // UpdateGamesButton
            // 
            this.UpdateGamesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.UpdateGamesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UpdateGamesButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.UpdateGamesButton.FlatAppearance.BorderSize = 0;
            this.UpdateGamesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateGamesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.UpdateGamesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.UpdateGamesButton.Location = new System.Drawing.Point(0, 84);
            this.UpdateGamesButton.Name = "UpdateGamesButton";
            this.UpdateGamesButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.UpdateGamesButton.Size = new System.Drawing.Size(233, 29);
            this.UpdateGamesButton.TabIndex = 7;
            this.UpdateGamesButton.Text = "REFRESH APP LIST";
            this.UpdateGamesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.UpdateGamesButton_Tooltip.SetToolTip(this.UpdateGamesButton, "Refresh game list and available updates");
            this.UpdateGamesButton.UseVisualStyleBackColor = false;
            this.UpdateGamesButton.Click += new System.EventHandler(this.UpdateGamesButton_Click);
            // 
            // listApkButton
            // 
            this.listApkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.listApkButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listApkButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.listApkButton.FlatAppearance.BorderSize = 0;
            this.listApkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.listApkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.listApkButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.listApkButton.Location = new System.Drawing.Point(0, 113);
            this.listApkButton.Name = "listApkButton";
            this.listApkButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.listApkButton.Size = new System.Drawing.Size(233, 28);
            this.listApkButton.TabIndex = 6;
            this.listApkButton.Text = "REFRESH ALL";
            this.listApkButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.listApkButton_Tooltip.SetToolTip(this.listApkButton, "Refresh connected devices, installed apps and game list");
            this.listApkButton.UseVisualStyleBackColor = false;
            this.listApkButton.Click += new System.EventHandler(this.listApkButton_Click);
            // 
            // progressDLbtnContainer
            // 
            this.progressDLbtnContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressDLbtnContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.progressDLbtnContainer.BackColor = System.Drawing.Color.Transparent;
            this.progressDLbtnContainer.Controls.Add(this.progressBar);
            this.progressDLbtnContainer.Controls.Add(this.speedLabel);
            this.progressDLbtnContainer.Location = new System.Drawing.Point(258, 453);
            this.progressDLbtnContainer.MinimumSize = new System.Drawing.Size(600, 34);
            this.progressDLbtnContainer.Name = "progressDLbtnContainer";
            this.progressDLbtnContainer.Size = new System.Drawing.Size(984, 40);
            this.progressDLbtnContainer.TabIndex = 96;
            // 
            // diskLabel
            // 
            this.diskLabel.BackColor = System.Drawing.Color.Transparent;
            this.diskLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diskLabel.ForeColor = System.Drawing.Color.LightGray;
            this.diskLabel.Location = new System.Drawing.Point(8, 24);
            this.diskLabel.Name = "diskLabel";
            this.diskLabel.Size = new System.Drawing.Size(150, 12);
            this.diskLabel.TabIndex = 7;
            this.diskLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.diskLabel.Visible = false;
            // 
            // questStorageProgressBar
            // 
            this.questStorageProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.questStorageProgressBar.Controls.Add(this.batteryLevImg);
            this.questStorageProgressBar.Location = new System.Drawing.Point(4, 4);
            this.questStorageProgressBar.Margin = new System.Windows.Forms.Padding(0);
            this.questStorageProgressBar.Name = "questStorageProgressBar";
            this.questStorageProgressBar.Size = new System.Drawing.Size(243, 44);
            this.questStorageProgressBar.TabIndex = 100;
            this.questStorageProgressBar.Visible = false;
            this.questStorageProgressBar.Paint += new System.Windows.Forms.PaintEventHandler(this.questStorageProgressBar_Paint);
            // 
            // batteryLevImg
            // 
            this.batteryLevImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.batteryLevImg.BackColor = System.Drawing.Color.Transparent;
            this.batteryLevImg.BackgroundImage = global::AndroidSideloader.Properties.Resources.battery;
            this.batteryLevImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.batteryLevImg.Location = new System.Drawing.Point(184, 8);
            this.batteryLevImg.Margin = new System.Windows.Forms.Padding(2);
            this.batteryLevImg.Name = "batteryLevImg";
            this.batteryLevImg.Size = new System.Drawing.Size(55, 29);
            this.batteryLevImg.TabIndex = 85;
            this.batteryLevImg.TabStop = false;
            this.batteryLevImg.Visible = false;
            // 
            // deviceDrop
            // 
            this.deviceDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.deviceDrop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deviceDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.deviceDrop.FlatAppearance.BorderSize = 0;
            this.deviceDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(51)))), ((int)(((byte)(65)))));
            this.deviceDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deviceDrop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.deviceDrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.deviceDrop.Location = new System.Drawing.Point(0, 262);
            this.deviceDrop.Margin = new System.Windows.Forms.Padding(2);
            this.deviceDrop.Name = "deviceDrop";
            this.deviceDrop.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.deviceDrop.Size = new System.Drawing.Size(233, 28);
            this.deviceDrop.TabIndex = 1;
            this.deviceDrop.Text = "DEVICE";
            this.deviceDrop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deviceDrop.UseVisualStyleBackColor = false;
            this.deviceDrop.Click += new System.EventHandler(this.deviceDropContainer_Click);
            // 
            // deviceDropContainer
            // 
            this.deviceDropContainer.AutoSize = true;
            this.deviceDropContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deviceDropContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.deviceDropContainer.Controls.Add(this.listApkButton);
            this.deviceDropContainer.Controls.Add(this.UpdateGamesButton);
            this.deviceDropContainer.Controls.Add(this.devicesbutton);
            this.deviceDropContainer.Controls.Add(this.mountDeviceButton);
            this.deviceDropContainer.Controls.Add(this.selectDeviceButton);
            this.deviceDropContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.deviceDropContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.deviceDropContainer.Location = new System.Drawing.Point(0, 290);
            this.deviceDropContainer.Margin = new System.Windows.Forms.Padding(2);
            this.deviceDropContainer.Name = "deviceDropContainer";
            this.deviceDropContainer.Size = new System.Drawing.Size(233, 141);
            this.deviceDropContainer.TabIndex = 73;
            this.deviceDropContainer.Visible = false;
            // 
            // mountDeviceButton
            // 
            this.mountDeviceButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.mountDeviceButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mountDeviceButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.mountDeviceButton.FlatAppearance.BorderSize = 0;
            this.mountDeviceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mountDeviceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.mountDeviceButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.mountDeviceButton.Location = new System.Drawing.Point(0, 28);
            this.mountDeviceButton.Name = "mountDeviceButton";
            this.mountDeviceButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.mountDeviceButton.Size = new System.Drawing.Size(233, 28);
            this.mountDeviceButton.TabIndex = 9;
            this.mountDeviceButton.Text = "MOUNT DEVICE";
            this.mountDeviceButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mountDeviceButton.UseVisualStyleBackColor = false;
            this.mountDeviceButton.Click += new System.EventHandler(this.MountButton_Click);
            // 
            // selectDeviceButton
            // 
            this.selectDeviceButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.selectDeviceButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectDeviceButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectDeviceButton.FlatAppearance.BorderSize = 0;
            this.selectDeviceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectDeviceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.selectDeviceButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.selectDeviceButton.Location = new System.Drawing.Point(0, 0);
            this.selectDeviceButton.Name = "selectDeviceButton";
            this.selectDeviceButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.selectDeviceButton.Size = new System.Drawing.Size(233, 28);
            this.selectDeviceButton.TabIndex = 8;
            this.selectDeviceButton.Text = "SELECT DEVICE";
            this.selectDeviceButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectDeviceButton.UseVisualStyleBackColor = false;
            this.selectDeviceButton.Click += new System.EventHandler(this.selectDeviceButton_Click);
            // 
            // sideloadDrop
            // 
            this.sideloadDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.sideloadDrop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sideloadDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.sideloadDrop.FlatAppearance.BorderSize = 0;
            this.sideloadDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(51)))), ((int)(((byte)(65)))));
            this.sideloadDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sideloadDrop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.sideloadDrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.sideloadDrop.Location = new System.Drawing.Point(0, 431);
            this.sideloadDrop.Margin = new System.Windows.Forms.Padding(2);
            this.sideloadDrop.Name = "sideloadDrop";
            this.sideloadDrop.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.sideloadDrop.Size = new System.Drawing.Size(233, 28);
            this.sideloadDrop.TabIndex = 1;
            this.sideloadDrop.Text = "SIDELOAD";
            this.sideloadDrop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.sideloadDrop.UseVisualStyleBackColor = false;
            this.sideloadDrop.Click += new System.EventHandler(this.sideloadContainer_Click);
            // 
            // sideloadContainer
            // 
            this.sideloadContainer.AutoSize = true;
            this.sideloadContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sideloadContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.sideloadContainer.Controls.Add(this.startsideloadbutton);
            this.sideloadContainer.Controls.Add(this.copyBulkObbButton);
            this.sideloadContainer.Controls.Add(this.obbcopybutton);
            this.sideloadContainer.Controls.Add(this.btnNoDevice);
            this.sideloadContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sideloadContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.sideloadContainer.Location = new System.Drawing.Point(0, 459);
            this.sideloadContainer.Margin = new System.Windows.Forms.Padding(2);
            this.sideloadContainer.Name = "sideloadContainer";
            this.sideloadContainer.Size = new System.Drawing.Size(233, 112);
            this.sideloadContainer.TabIndex = 74;
            this.sideloadContainer.Visible = false;
            // 
            // btnNoDevice
            // 
            this.btnNoDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.btnNoDevice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNoDevice.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNoDevice.FlatAppearance.BorderSize = 0;
            this.btnNoDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNoDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnNoDevice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnNoDevice.Location = new System.Drawing.Point(0, 0);
            this.btnNoDevice.Name = "btnNoDevice";
            this.btnNoDevice.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnNoDevice.Size = new System.Drawing.Size(233, 28);
            this.btnNoDevice.TabIndex = 6;
            this.btnNoDevice.Text = "DISABLE SIDELOADING";
            this.btnNoDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNoDevice.UseVisualStyleBackColor = false;
            this.btnNoDevice.Click += new System.EventHandler(this.btnNoDevice_Click);
            // 
            // installedAppsMenu
            // 
            this.installedAppsMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.installedAppsMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.installedAppsMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.installedAppsMenu.FlatAppearance.BorderSize = 0;
            this.installedAppsMenu.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(51)))), ((int)(((byte)(65)))));
            this.installedAppsMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.installedAppsMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.installedAppsMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.installedAppsMenu.Location = new System.Drawing.Point(0, 571);
            this.installedAppsMenu.Margin = new System.Windows.Forms.Padding(2);
            this.installedAppsMenu.Name = "installedAppsMenu";
            this.installedAppsMenu.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.installedAppsMenu.Size = new System.Drawing.Size(233, 28);
            this.installedAppsMenu.TabIndex = 1;
            this.installedAppsMenu.Text = "INSTALLED APPS";
            this.installedAppsMenu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.installedAppsMenu.UseVisualStyleBackColor = false;
            this.installedAppsMenu.Click += new System.EventHandler(this.installedAppsMenuContainer_Click);
            // 
            // installedAppsMenuContainer
            // 
            this.installedAppsMenuContainer.AutoSize = true;
            this.installedAppsMenuContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.installedAppsMenuContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.installedAppsMenuContainer.Controls.Add(this.pullAppToDesktopBtn);
            this.installedAppsMenuContainer.Controls.Add(this.uninstallAppButton);
            this.installedAppsMenuContainer.Controls.Add(this.getApkButton);
            this.installedAppsMenuContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.installedAppsMenuContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.installedAppsMenuContainer.Location = new System.Drawing.Point(0, 599);
            this.installedAppsMenuContainer.Margin = new System.Windows.Forms.Padding(2);
            this.installedAppsMenuContainer.Name = "installedAppsMenuContainer";
            this.installedAppsMenuContainer.Size = new System.Drawing.Size(233, 84);
            this.installedAppsMenuContainer.TabIndex = 73;
            this.installedAppsMenuContainer.Visible = false;
            // 
            // backupDrop
            // 
            this.backupDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.backupDrop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.backupDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupDrop.FlatAppearance.BorderSize = 0;
            this.backupDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(51)))), ((int)(((byte)(65)))));
            this.backupDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backupDrop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.backupDrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.backupDrop.Location = new System.Drawing.Point(0, 683);
            this.backupDrop.Margin = new System.Windows.Forms.Padding(2);
            this.backupDrop.Name = "backupDrop";
            this.backupDrop.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.backupDrop.Size = new System.Drawing.Size(233, 28);
            this.backupDrop.TabIndex = 2;
            this.backupDrop.Text = "BACKUP / RESTORE";
            this.backupDrop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.backupDrop.UseVisualStyleBackColor = false;
            this.backupDrop.Click += new System.EventHandler(this.backupDrop_Click);
            // 
            // backupContainer
            // 
            this.backupContainer.AutoSize = true;
            this.backupContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.backupContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.backupContainer.Controls.Add(this.restorebutton);
            this.backupContainer.Controls.Add(this.backupadbbutton);
            this.backupContainer.Controls.Add(this.backupbutton);
            this.backupContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.backupContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.backupContainer.Location = new System.Drawing.Point(0, 711);
            this.backupContainer.Margin = new System.Windows.Forms.Padding(2);
            this.backupContainer.Name = "backupContainer";
            this.backupContainer.Size = new System.Drawing.Size(233, 84);
            this.backupContainer.TabIndex = 76;
            this.backupContainer.Visible = false;
            // 
            // otherDrop
            // 
            this.otherDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.otherDrop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.otherDrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.otherDrop.FlatAppearance.BorderSize = 0;
            this.otherDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(51)))), ((int)(((byte)(65)))));
            this.otherDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.otherDrop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.otherDrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.otherDrop.Location = new System.Drawing.Point(0, 795);
            this.otherDrop.Margin = new System.Windows.Forms.Padding(2);
            this.otherDrop.Name = "otherDrop";
            this.otherDrop.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.otherDrop.Size = new System.Drawing.Size(233, 28);
            this.otherDrop.TabIndex = 3;
            this.otherDrop.Text = "OTHER";
            this.otherDrop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.otherDrop.UseVisualStyleBackColor = false;
            this.otherDrop.Click += new System.EventHandler(this.otherDrop_Click);
            // 
            // otherContainer
            // 
            this.otherContainer.AutoSize = true;
            this.otherContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.otherContainer.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.otherContainer.Controls.Add(this.settingsButton);
            this.otherContainer.Controls.Add(this.QuestOptionsButton);
            this.otherContainer.Controls.Add(this.selectMirrorButton);
            this.otherContainer.Controls.Add(this.btnRunAdbCmd);
            this.otherContainer.Controls.Add(this.btnOpenDownloads);
            this.otherContainer.Controls.Add(this.ADBWirelessToggle);
            this.otherContainer.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.otherContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.otherContainer.Location = new System.Drawing.Point(0, 823);
            this.otherContainer.Margin = new System.Windows.Forms.Padding(2);
            this.otherContainer.Name = "otherContainer";
            this.otherContainer.Size = new System.Drawing.Size(233, 168);
            this.otherContainer.TabIndex = 80;
            this.otherContainer.Visible = false;
            // 
            // selectMirrorButton
            // 
            this.selectMirrorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.selectMirrorButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectMirrorButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectMirrorButton.FlatAppearance.BorderSize = 0;
            this.selectMirrorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectMirrorButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.selectMirrorButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.selectMirrorButton.Location = new System.Drawing.Point(0, 84);
            this.selectMirrorButton.Name = "selectMirrorButton";
            this.selectMirrorButton.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.selectMirrorButton.Size = new System.Drawing.Size(233, 28);
            this.selectMirrorButton.TabIndex = 9;
            this.selectMirrorButton.Text = "SELECT MIRROR";
            this.selectMirrorButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectMirrorButton.UseVisualStyleBackColor = false;
            this.selectMirrorButton.Click += new System.EventHandler(this.selectMirrorButton_Click);
            // 
            // questInfoPanel
            // 
            this.questInfoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.questInfoPanel.Controls.Add(this.batteryLabel);
            this.questInfoPanel.Controls.Add(this.diskLabel);
            this.questInfoPanel.Controls.Add(this.questInfoLabel);
            this.questInfoPanel.Controls.Add(this.questStorageProgressBar);
            this.questInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.questInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.questInfoPanel.Margin = new System.Windows.Forms.Padding(0);
            this.questInfoPanel.Name = "questInfoPanel";
            this.questInfoPanel.Size = new System.Drawing.Size(233, 48);
            this.questInfoPanel.TabIndex = 0;
            // 
            // batteryLabel
            // 
            this.batteryLabel.BackColor = System.Drawing.Color.Transparent;
            this.batteryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.batteryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.batteryLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.batteryLabel.Location = new System.Drawing.Point(0, 0);
            this.batteryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.batteryLabel.Name = "batteryLabel";
            this.batteryLabel.Padding = new System.Windows.Forms.Padding(0, 0, 8, 4);
            this.batteryLabel.Size = new System.Drawing.Size(233, 48);
            this.batteryLabel.TabIndex = 84;
            this.batteryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.batteryLabel.Visible = false;
            // 
            // questInfoLabel
            // 
            this.questInfoLabel.BackColor = System.Drawing.Color.Transparent;
            this.questInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.questInfoLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.questInfoLabel.Location = new System.Drawing.Point(8, 4);
            this.questInfoLabel.Name = "questInfoLabel";
            this.questInfoLabel.Size = new System.Drawing.Size(150, 20);
            this.questInfoLabel.TabIndex = 1;
            this.questInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.questInfoLabel.Visible = false;
            // 
            // ULLabel
            // 
            this.ULLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ULLabel.AutoSize = true;
            this.ULLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ULLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.ULLabel.Location = new System.Drawing.Point(1109, 701);
            this.ULLabel.Name = "ULLabel";
            this.ULLabel.Size = new System.Drawing.Size(130, 13);
            this.ULLabel.TabIndex = 87;
            this.ULLabel.Text = "Uploading to server...";
            this.ULLabel.Visible = false;
            // 
            // leftNavContainer
            // 
            this.leftNavContainer.AutoScroll = true;
            this.leftNavContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.leftNavContainer.Controls.Add(this.statusInfoPanel);
            this.leftNavContainer.Controls.Add(this.aboutBtn);
            this.leftNavContainer.Controls.Add(this.otherContainer);
            this.leftNavContainer.Controls.Add(this.otherDrop);
            this.leftNavContainer.Controls.Add(this.backupContainer);
            this.leftNavContainer.Controls.Add(this.backupDrop);
            this.leftNavContainer.Controls.Add(this.installedAppsMenuContainer);
            this.leftNavContainer.Controls.Add(this.installedAppsMenu);
            this.leftNavContainer.Controls.Add(this.sideloadContainer);
            this.leftNavContainer.Controls.Add(this.sideloadDrop);
            this.leftNavContainer.Controls.Add(this.deviceDropContainer);
            this.leftNavContainer.Controls.Add(this.deviceDrop);
            this.leftNavContainer.Controls.Add(this.sidebarMediaPanel);
            this.leftNavContainer.Controls.Add(this.questInfoPanel);
            this.leftNavContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftNavContainer.Location = new System.Drawing.Point(0, 0);
            this.leftNavContainer.Margin = new System.Windows.Forms.Padding(2);
            this.leftNavContainer.Name = "leftNavContainer";
            this.leftNavContainer.Size = new System.Drawing.Size(250, 721);
            this.leftNavContainer.TabIndex = 73;
            // 
            // statusInfoPanel
            // 
            this.statusInfoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.statusInfoPanel.Controls.Add(this.sideloadingStatusLabel);
            this.statusInfoPanel.Controls.Add(this.activeMirrorLabel);
            this.statusInfoPanel.Controls.Add(this.deviceIdLabel);
            this.statusInfoPanel.Controls.Add(this.rookieStatusLabel);
            this.statusInfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusInfoPanel.Location = new System.Drawing.Point(0, 1019);
            this.statusInfoPanel.Name = "statusInfoPanel";
            this.statusInfoPanel.Padding = new System.Windows.Forms.Padding(8, 4, 8, 8);
            this.statusInfoPanel.Size = new System.Drawing.Size(233, 81);
            this.statusInfoPanel.TabIndex = 102;
            // 
            // sideloadingStatusLabel
            // 
            this.sideloadingStatusLabel.AutoEllipsis = true;
            this.sideloadingStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.sideloadingStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.sideloadingStatusLabel.Location = new System.Drawing.Point(8, 55);
            this.sideloadingStatusLabel.Name = "sideloadingStatusLabel";
            this.sideloadingStatusLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.sideloadingStatusLabel.Size = new System.Drawing.Size(225, 17);
            this.sideloadingStatusLabel.TabIndex = 3;
            this.sideloadingStatusLabel.Text = "Sideloading: Enabled";
            // 
            // activeMirrorLabel
            // 
            this.activeMirrorLabel.AutoEllipsis = true;
            this.activeMirrorLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.activeMirrorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(150)))));
            this.activeMirrorLabel.Location = new System.Drawing.Point(8, 38);
            this.activeMirrorLabel.Name = "activeMirrorLabel";
            this.activeMirrorLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.activeMirrorLabel.Size = new System.Drawing.Size(225, 17);
            this.activeMirrorLabel.TabIndex = 2;
            this.activeMirrorLabel.Text = "Mirror: None";
            // 
            // deviceIdLabel
            // 
            this.deviceIdLabel.AutoEllipsis = true;
            this.deviceIdLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.deviceIdLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(150)))));
            this.deviceIdLabel.Location = new System.Drawing.Point(8, 21);
            this.deviceIdLabel.Name = "deviceIdLabel";
            this.deviceIdLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.deviceIdLabel.Size = new System.Drawing.Size(225, 17);
            this.deviceIdLabel.TabIndex = 1;
            this.deviceIdLabel.Text = "Device: Not connected";
            // 
            // rookieStatusLabel
            // 
            this.rookieStatusLabel.AutoEllipsis = true;
            this.rookieStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.rookieStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.rookieStatusLabel.Location = new System.Drawing.Point(8, 4);
            this.rookieStatusLabel.Name = "rookieStatusLabel";
            this.rookieStatusLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.rookieStatusLabel.Size = new System.Drawing.Size(225, 17);
            this.rookieStatusLabel.TabIndex = 0;
            this.rookieStatusLabel.Text = "Status";
            this.rookieStatusLabel.UseMnemonic = false;
            // 
            // sidebarMediaPanel
            // 
            this.sidebarMediaPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.sidebarMediaPanel.Controls.Add(this.gamesPictureBox);
            this.sidebarMediaPanel.Controls.Add(this.downloadInstallGameButton);
            this.sidebarMediaPanel.Controls.Add(this.selectedGameLabel);
            this.sidebarMediaPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sidebarMediaPanel.Location = new System.Drawing.Point(0, 48);
            this.sidebarMediaPanel.Margin = new System.Windows.Forms.Padding(0);
            this.sidebarMediaPanel.Name = "sidebarMediaPanel";
            this.sidebarMediaPanel.Size = new System.Drawing.Size(233, 214);
            this.sidebarMediaPanel.TabIndex = 101;
            // 
            // selectedGameLabel
            // 
            this.selectedGameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.selectedGameLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.selectedGameLabel.ForeColor = System.Drawing.Color.White;
            this.selectedGameLabel.Location = new System.Drawing.Point(8, 6);
            this.selectedGameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.selectedGameLabel.Name = "selectedGameLabel";
            this.selectedGameLabel.Size = new System.Drawing.Size(217, 20);
            this.selectedGameLabel.TabIndex = 99;
            this.selectedGameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectedGameLabel.UseMnemonic = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.searchPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnViewToggle, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.favoriteSwitcher, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDownloaded, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnInstalled, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUpdateAvailable, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnNewerThanList, 6, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(258, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(984, 34);
            this.tableLayoutPanel1.TabIndex = 97;
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.webView21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.webView21.Location = new System.Drawing.Point(259, 496);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(384, 216);
            this.webView21.TabIndex = 98;
            this.webView21.ZoomFactor = 1D;
            // 
            // favoriteGame
            // 
            this.favoriteGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(48)))));
            this.favoriteGame.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.favoriteButton});
            this.favoriteGame.Name = "favoriteGame";
            this.favoriteGame.ShowImageMargin = false;
            this.favoriteGame.Size = new System.Drawing.Size(149, 26);
            // 
            // favoriteButton
            // 
            this.favoriteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(48)))));
            this.favoriteButton.ForeColor = System.Drawing.Color.White;
            this.favoriteButton.Name = "favoriteButton";
            this.favoriteButton.Size = new System.Drawing.Size(148, 22);
            this.favoriteButton.Text = "★ Add to Favorites";
            this.favoriteButton.Click += new System.EventHandler(this.favoriteButton_Click);
            // 
            // gamesGalleryView
            // 
            this.gamesGalleryView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamesGalleryView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.gamesGalleryView.Location = new System.Drawing.Point(258, 44);
            this.gamesGalleryView.Name = "gamesGalleryView";
            this.gamesGalleryView.Size = new System.Drawing.Size(984, 409);
            this.gamesGalleryView.TabIndex = 102;
            // 
            // webViewPlaceholderPanel
            // 
            this.webViewPlaceholderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.webViewPlaceholderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(26)))), ((int)(((byte)(30)))));
            this.webViewPlaceholderPanel.Location = new System.Drawing.Point(259, 496);
            this.webViewPlaceholderPanel.Name = "webViewPlaceholderPanel";
            this.webViewPlaceholderPanel.Size = new System.Drawing.Size(384, 217);
            this.webViewPlaceholderPanel.TabIndex = 103;
            this.webViewPlaceholderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.webViewPlaceholderPanel_Paint);
            // 
            // searchPanel
            // 
            this.searchPanel.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchPanel.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.searchPanel.BackColor = System.Drawing.Color.Transparent;
            this.searchPanel.Controls.Add(this.searchIconPictureBox);
            this.searchPanel.Controls.Add(this.searchTextBox);
            this.searchPanel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.searchPanel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.searchPanel.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchPanel.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchPanel.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.searchPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.searchPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.searchPanel.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchPanel.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchPanel.Location = new System.Drawing.Point(0, 3);
            this.searchPanel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.searchPanel.MinimumSize = new System.Drawing.Size(110, 28);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Radius = 5;
            this.searchPanel.Size = new System.Drawing.Size(340, 28);
            this.searchPanel.Stroke = false;
            this.searchPanel.StrokeColor = System.Drawing.Color.Gray;
            this.searchPanel.TabIndex = 104;
            this.searchPanel.Transparency = false;
            this.searchPanel.Click += new System.EventHandler(this.searchTextBox_Click);
            // 
            // searchIconPictureBox
            // 
            this.searchIconPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.searchIconPictureBox.Image = global::AndroidSideloader.Properties.Resources.SearchGlass;
            this.searchIconPictureBox.Location = new System.Drawing.Point(9, 6);
            this.searchIconPictureBox.Name = "searchIconPictureBox";
            this.searchIconPictureBox.Size = new System.Drawing.Size(16, 16);
            this.searchIconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.searchIconPictureBox.TabIndex = 0;
            this.searchIconPictureBox.TabStop = false;
            this.searchIconPictureBox.Click += new System.EventHandler(this.searchTextBox_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.searchTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.searchTextBox.Location = new System.Drawing.Point(32, 7);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(300, 16);
            this.searchTextBox.TabIndex = 5;
            this.searchTextBox.Text = "Search...";
            this.searchTextBox.Click += new System.EventHandler(this.searchTextBox_Click);
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);
            this.searchTextBox.Leave += new System.EventHandler(this.searchTextBox_Leave);
            // 
            // btnViewToggle
            // 
            this.btnViewToggle.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnViewToggle.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnViewToggle.BackColor = System.Drawing.Color.Transparent;
            this.btnViewToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnViewToggle.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnViewToggle.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnViewToggle.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnViewToggle.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnViewToggle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnViewToggle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnViewToggle.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnViewToggle.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnViewToggle.Location = new System.Drawing.Point(346, 3);
            this.btnViewToggle.Name = "btnViewToggle";
            this.btnViewToggle.Radius = 5;
            this.btnViewToggle.Size = new System.Drawing.Size(75, 28);
            this.btnViewToggle.Stroke = true;
            this.btnViewToggle.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnViewToggle.TabIndex = 103;
            this.btnViewToggle.Text = "LIST";
            this.btnViewToggle.Transparency = false;
            this.btnViewToggle.Click += new System.EventHandler(this.btnViewToggle_Click);
            // 
            // favoriteSwitcher
            // 
            this.favoriteSwitcher.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.favoriteSwitcher.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.favoriteSwitcher.BackColor = System.Drawing.Color.Transparent;
            this.favoriteSwitcher.Cursor = System.Windows.Forms.Cursors.Hand;
            this.favoriteSwitcher.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.favoriteSwitcher.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.favoriteSwitcher.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.favoriteSwitcher.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.favoriteSwitcher.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.favoriteSwitcher.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.favoriteSwitcher.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.favoriteSwitcher.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.favoriteSwitcher.Location = new System.Drawing.Point(427, 3);
            this.favoriteSwitcher.Name = "favoriteSwitcher";
            this.favoriteSwitcher.Radius = 5;
            this.favoriteSwitcher.Size = new System.Drawing.Size(88, 28);
            this.favoriteSwitcher.Stroke = true;
            this.favoriteSwitcher.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.favoriteSwitcher.TabIndex = 101;
            this.favoriteSwitcher.Text = "FAVORITES";
            this.favoriteSwitcher.Transparency = false;
            this.favoriteSwitcher.Click += new System.EventHandler(this.favoriteSwitcher_Click);
            // 
            // btnInstalled
            // 
            this.btnInstalled.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnInstalled.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnInstalled.BackColor = System.Drawing.Color.Transparent;
            this.btnInstalled.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInstalled.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnInstalled.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnInstalled.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnInstalled.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnInstalled.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnInstalled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnInstalled.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnInstalled.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnInstalled.Location = new System.Drawing.Point(521, 3);
            this.btnInstalled.Name = "btnInstalled";
            this.btnInstalled.Radius = 5;
            this.btnInstalled.Size = new System.Drawing.Size(150, 28);
            this.btnInstalled.Stroke = true;
            this.btnInstalled.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnInstalled.TabIndex = 90;
            this.btnInstalled.Text = "INSTALLED";
            this.btnInstalled.Transparency = false;
            this.btnInstalled.Click += new System.EventHandler(this.btnInstalled_Click);
            // 
            // btnUpdateAvailable
            // 
            this.btnUpdateAvailable.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnUpdateAvailable.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnUpdateAvailable.BackColor = System.Drawing.Color.Transparent;
            this.btnUpdateAvailable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdateAvailable.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUpdateAvailable.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnUpdateAvailable.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnUpdateAvailable.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnUpdateAvailable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUpdateAvailable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnUpdateAvailable.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnUpdateAvailable.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnUpdateAvailable.Location = new System.Drawing.Point(677, 3);
            this.btnUpdateAvailable.Name = "btnUpdateAvailable";
            this.btnUpdateAvailable.Radius = 5;
            this.btnUpdateAvailable.Size = new System.Drawing.Size(150, 28);
            this.btnUpdateAvailable.Stroke = true;
            this.btnUpdateAvailable.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnUpdateAvailable.TabIndex = 91;
            this.btnUpdateAvailable.Text = "UPDATE AVAILABLE";
            this.btnUpdateAvailable.Transparency = false;
            this.btnUpdateAvailable.Click += new System.EventHandler(this.btnUpdateAvailable_Click);
            // 
            // btnNewerThanList
            // 
            this.btnNewerThanList.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnNewerThanList.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnNewerThanList.BackColor = System.Drawing.Color.Transparent;
            this.btnNewerThanList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewerThanList.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNewerThanList.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnNewerThanList.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnNewerThanList.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnNewerThanList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNewerThanList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnNewerThanList.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnNewerThanList.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnNewerThanList.Location = new System.Drawing.Point(833, 3);
            this.btnNewerThanList.Name = "btnNewerThanList";
            this.btnNewerThanList.Radius = 5;
            this.btnNewerThanList.Size = new System.Drawing.Size(148, 28);
            this.btnNewerThanList.Stroke = true;
            this.btnNewerThanList.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnNewerThanList.TabIndex = 92;
            this.btnNewerThanList.Text = "NEWER THAN LIST";
            this.btnNewerThanList.Transparency = false;
            this.btnNewerThanList.Click += new System.EventHandler(this.btnNewerThanList_Click);
            // 
            // btnDownloaded
            // 
            this.btnDownloaded.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnDownloaded.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.btnDownloaded.BackColor = System.Drawing.Color.Transparent;
            this.btnDownloaded.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownloaded.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDownloaded.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnDownloaded.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.btnDownloaded.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.btnDownloaded.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDownloaded.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.btnDownloaded.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnDownloaded.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.btnDownloaded.Location = new System.Drawing.Point(521, 3);
            this.btnDownloaded.Name = "btnDownloaded";
            this.btnDownloaded.Radius = 5;
            this.btnDownloaded.Size = new System.Drawing.Size(148, 28);
            this.btnDownloaded.Stroke = true;
            this.btnDownloaded.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.btnDownloaded.TabIndex = 105;
            this.btnDownloaded.Text = "DOWNLOADED";
            this.btnDownloaded.Transparency = false;
            this.btnDownloaded.Click += new System.EventHandler(this.btnDownloaded_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.progressBar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.progressBar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.progressBar.IndeterminateColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.progressBar.IsIndeterminate = false;
            this.progressBar.Location = new System.Drawing.Point(1, 23);
            this.progressBar.Maximum = 100F;
            this.progressBar.Minimum = 0F;
            this.progressBar.MinimumSize = new System.Drawing.Size(200, 13);
            this.progressBar.Name = "progressBar";
            this.progressBar.OperationType = "";
            this.progressBar.ProgressEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(160)))), ((int)(((byte)(130)))));
            this.progressBar.ProgressStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(220)))), ((int)(((byte)(190)))));
            this.progressBar.Radius = 6;
            this.progressBar.Size = new System.Drawing.Size(983, 13);
            this.progressBar.StatusText = "";
            this.progressBar.TabIndex = 7;
            this.progressBar.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.progressBar.Value = 0F;
            // 
            // downloadInstallGameButton
            // 
            this.downloadInstallGameButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(215)))), ((int)(((byte)(190)))));
            this.downloadInstallGameButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(215)))), ((int)(((byte)(190)))));
            this.downloadInstallGameButton.BackColor = System.Drawing.Color.Transparent;
            this.downloadInstallGameButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.downloadInstallGameButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.downloadInstallGameButton.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(18)))), ((int)(((byte)(22)))));
            this.downloadInstallGameButton.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(18)))), ((int)(((byte)(22)))));
            this.downloadInstallGameButton.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.downloadInstallGameButton.Enabled = false;
            this.downloadInstallGameButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.downloadInstallGameButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(67)))), ((int)(((byte)(82)))));
            this.downloadInstallGameButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.downloadInstallGameButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.downloadInstallGameButton.Location = new System.Drawing.Point(6, 177);
            this.downloadInstallGameButton.Margin = new System.Windows.Forms.Padding(0);
            this.downloadInstallGameButton.Name = "downloadInstallGameButton";
            this.downloadInstallGameButton.Radius = 4;
            this.downloadInstallGameButton.Size = new System.Drawing.Size(238, 30);
            this.downloadInstallGameButton.Stroke = true;
            this.downloadInstallGameButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.downloadInstallGameButton.TabIndex = 94;
            this.downloadInstallGameButton.Text = "DOWNLOAD";
            this.downloadInstallGameButton.Transparency = false;
            this.downloadInstallGameButton.Click += new System.EventHandler(this.downloadInstallGameButton_Click);
            this.downloadInstallGameButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.downloadInstallGameButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1254, 721);
            this.Controls.Add(this.ULLabel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.progressDLbtnContainer);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.gamesQueueLabel);
            this.Controls.Add(this.gamesQueListBox);
            this.Controls.Add(this.leftNavContainer);
            this.Controls.Add(this.notesRichTextBox);
            this.Controls.Add(this.gamesListView);
            this.Controls.Add(this.gamesGalleryView);
            this.Controls.Add(this.webViewPlaceholderPanel);
            this.Controls.Add(this.webView21);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.HelpButton = true;
            this.MinimumSize = new System.Drawing.Size(1048, 760);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rookie Sideloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.DragLeave += new System.EventHandler(this.Form1_DragLeave);
            ((System.ComponentModel.ISupportInitialize)(this.gamesPictureBox)).EndInit();
            this.gamesPictureBox.ResumeLayout(false);
            this.progressDLbtnContainer.ResumeLayout(false);
            this.progressDLbtnContainer.PerformLayout();
            this.questStorageProgressBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.batteryLevImg)).EndInit();
            this.deviceDropContainer.ResumeLayout(false);
            this.sideloadContainer.ResumeLayout(false);
            this.installedAppsMenuContainer.ResumeLayout(false);
            this.backupContainer.ResumeLayout(false);
            this.otherContainer.ResumeLayout(false);
            this.questInfoPanel.ResumeLayout(false);
            this.leftNavContainer.ResumeLayout(false);
            this.leftNavContainer.PerformLayout();
            this.statusInfoPanel.ResumeLayout(false);
            this.sidebarMediaPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.favoriteGame.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchIconPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private SergeUtils.EasyCompletionComboBox m_combo;
        private ModernProgressBar progressBar;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Label freeDisclaimer;
        private System.Windows.Forms.ComboBox devicesComboBox;
        private System.Windows.Forms.ListBox gamesQueListBox;
        private System.Windows.Forms.ListView gamesListView;
        private System.Windows.Forms.FlowLayoutPanel gamesGalleryView;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.PictureBox gamesPictureBox;
        private System.Windows.Forms.Label gamesQueueLabel;
        private System.Windows.Forms.RichTextBox notesRichTextBox;
        private System.Windows.Forms.Label lblNotes;
        public System.Windows.Forms.ComboBox remotesList;
        public System.Windows.Forms.ColumnHeader GameNameIndex;
        public System.Windows.Forms.ColumnHeader ReleaseNameIndex;
        private System.Windows.Forms.ColumnHeader PackageNameIndex;
        private System.Windows.Forms.ColumnHeader VersionCodeIndex;
        private System.Windows.Forms.ColumnHeader ReleaseAPKPathIndex;
        public System.Windows.Forms.ColumnHeader VersionNameIndex;
        public System.Windows.Forms.ColumnHeader DownloadsIndex;
        private RoundButton downloadInstallGameButton;
        private RoundButton btnViewToggle;
        private ToolTip startsideloadbutton_Tooltip;
        private ToolTip devicesbutton_Tooltip;
        private ToolTip obbcopybutton_Tooltip;
        private ToolTip backupadbbutton_Tooltip;
        private ToolTip backupbutton_Tooltip;
        private ToolTip restorebutton_Tooltip;
        private ToolTip getApkButton_Tooltip;
        private ToolTip uninstallAppButton_Tooltip;
        private ToolTip pullAppToDesktopBtn_Tooltip;
        private ToolTip copyBulkObbButton_Tooltip;
        private ToolTip aboutBtn_Tooltip;
        private ToolTip settingsButton_Tooltip;
        private ToolTip QuestOptionsButton_Tooltip;
        private ToolTip btnOpenDownloads_Tooltip;
        private ToolTip btnRunAdbCmd_Tooltip;
        private ToolTip ADBWirelessToggle_Tooltip;
        private ToolTip UpdateGamesButton_Tooltip;
        private ToolTip listApkButton_Tooltip;
        private ToolTip speedLabel_Tooltip;
        private ToolTip etaLabel_Tooltip;
        private ToolTip btnViewToggle_Tooltip;
        private Panel progressDLbtnContainer;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Button devicesbutton;
        private Button deviceDrop;
        private Panel deviceDropContainer;
        private Button sideloadDrop;
        private Panel sideloadContainer;
        private Button installedAppsMenu;
        private Panel installedAppsMenuContainer;
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
        private Button backupadbbutton;
        private Button backupbutton;
        private Button restorebutton;
        private Button otherDrop;
        private Panel otherContainer;
        private Button QuestOptionsButton;
        private Button ADBWirelessToggle;
        private Button settingsButton;
        private Button aboutBtn;
        private PictureBox batteryLevImg;
        private Label batteryLabel;
        private Label ULLabel;
        private Panel leftNavContainer;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnOpenDownloads;
        private Button btnRunAdbCmd;
        private Button btnNoDevice;
        private ContextMenuStrip favoriteGame;
        private ToolStripMenuItem favoriteButton;
        private RoundButton favoriteSwitcher;
        private Panel questInfoPanel;
        private Panel questStorageProgressBar;
        private Label questInfoLabel;
        private Label diskLabel;
        private PictureBox searchIconPictureBox;
        private Panel sidebarMediaPanel;
        private Label selectedGameLabel;
        private Button selectDeviceButton;
        private Button mountDeviceButton;
        private Button selectMirrorButton;
        private RoundButton btnInstalled;
        private RoundButton btnUpdateAvailable;
        private RoundButton btnNewerThanList;
        private RoundButton btnDownloaded;
        private RoundButton searchPanel;
        private Panel notesPanel;
        private Panel queuePanel;
        private Panel webViewPlaceholderPanel;
        private Panel statusInfoPanel;
        private Label deviceIdLabel;
        private Label activeMirrorLabel;
        private Label sideloadingStatusLabel;
        private Label rookieStatusLabel;
        private ModernListView _listViewRenderer;
    }
}