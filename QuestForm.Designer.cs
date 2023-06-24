using System.Windows.Forms;

namespace AndroidSideloader
{
    partial class QuestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        /// 

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.QUon = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.DeleteShots = new System.Windows.Forms.CheckBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.label10 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.CPUComboBox = new System.Windows.Forms.ComboBox();
            this.GPUComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ResolutionLabel = new System.Windows.Forms.Label();
            this.GlobalUsername = new System.Windows.Forms.TextBox();
            this.FOVy = new System.Windows.Forms.TextBox();
            this.FOVx = new System.Windows.Forms.TextBox();
            this.UsrBox = new System.Windows.Forms.TextBox();
            this.ResBox = new System.Windows.Forms.TextBox();
            this.TextureResTextBox = new System.Windows.Forms.TextBox();
            this.QURfrRt = new System.Windows.Forms.ComboBox();
            this.RefreshRateComboBox = new System.Windows.Forms.ComboBox();
            this.button1 = new AndroidSideloader.RoundButton();
            this.WifiWake = new AndroidSideloader.RoundButton();
            this.questVids = new AndroidSideloader.RoundButton();
            this.questPics = new AndroidSideloader.RoundButton();
            this.QUEnable = new AndroidSideloader.RoundButton();
            this.deleteButton = new AndroidSideloader.RoundButton();
            this.ResetQU = new AndroidSideloader.RoundButton();
            this.button3 = new AndroidSideloader.RoundButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.LightCyan;
            this.label1.Location = new System.Drawing.Point(31, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Temporary settings for all Quest apps";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.label2.Location = new System.Drawing.Point(70, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 32);
            this.label2.TabIndex = 6;
            this.label2.Text = "Reboot to reset. - Turn screen off, \r\nthen back on with hold button to apply.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.LightCyan;
            this.label3.Location = new System.Drawing.Point(57, 339);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(256, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "QU Settings (for -QU releases)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.label4.Location = new System.Drawing.Point(16, 363);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(337, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Persists on device reboot. Enter 0 to reset any category.\r\n";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.LightCyan;
            this.label11.Location = new System.Drawing.Point(26, 217);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(319, 20);
            this.label11.TabIndex = 6;
            this.label11.Text = "Transfer screen/videoshots to Desktop";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // QUon
            // 
            this.QUon.AutoSize = true;
            this.QUon.BackColor = System.Drawing.Color.Transparent;
            this.QUon.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.QUon.Cursor = System.Windows.Forms.Cursors.Default;
            this.QUon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QUon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.QUon.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.QUon.Location = new System.Drawing.Point(111, 383);
            this.QUon.Name = "QUon";
            this.QUon.Size = new System.Drawing.Size(148, 21);
            this.QUon.TabIndex = 8;
            this.QUon.Text = "Enable QU Settings";
            this.QUon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.QUon.UseVisualStyleBackColor = false;
            this.QUon.CheckedChanged += new System.EventHandler(this.QUon_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.LightCyan;
            this.label12.Location = new System.Drawing.Point(121, 656);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 20);
            this.label12.TabIndex = 6;
            this.label12.Text = "Set username*";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.label13.Location = new System.Drawing.Point(83, 678);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(203, 16);
            this.label13.TabIndex = 6;
            this.label13.Text = "*For games not released by -QU.";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DeleteShots
            // 
            this.DeleteShots.AutoSize = true;
            this.DeleteShots.BackColor = System.Drawing.Color.Transparent;
            this.DeleteShots.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DeleteShots.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteShots.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteShots.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.DeleteShots.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.DeleteShots.Location = new System.Drawing.Point(64, 296);
            this.DeleteShots.Name = "DeleteShots";
            this.DeleteShots.Size = new System.Drawing.Size(242, 21);
            this.DeleteShots.TabIndex = 7;
            this.DeleteShots.Text = "Delete files on Quest after transfer";
            this.DeleteShots.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DeleteShots.UseVisualStyleBackColor = false;
            this.DeleteShots.CheckedChanged += new System.EventHandler(this.DeleteShots_CheckedChanged);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 737);
            this.splitter1.TabIndex = 14;
            this.splitter1.TabStop = false;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Location = new System.Drawing.Point(-4, 327);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(394, 3);
            this.label10.TabIndex = 15;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label14.Location = new System.Drawing.Point(-15, 211);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(394, 3);
            this.label14.TabIndex = 17;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label15.Location = new System.Drawing.Point(-12, 646);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(394, 3);
            this.label15.TabIndex = 18;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.label16.Location = new System.Drawing.Point(14, 241);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(342, 16);
            this.label16.TabIndex = 6;
            this.label16.Text = "Desktop\\Quest Screenshots  Desktop\\Quest Videoshots";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // CPUComboBox
            // 
            this.CPUComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CPUComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CPUComboBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CPUComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CPUComboBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.CPUComboBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.CPUComboBox.FormattingEnabled = true;
            this.CPUComboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4"});
            this.CPUComboBox.Location = new System.Drawing.Point(13, 123);
            this.CPUComboBox.Name = "CPUComboBox";
            this.CPUComboBox.Size = new System.Drawing.Size(346, 26);
            this.CPUComboBox.TabIndex = 2;
            this.CPUComboBox.Text = "Select CPU level (0 for default)";
            // 
            // GPUComboBox
            // 
            this.GPUComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.GPUComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.GPUComboBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.GPUComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GPUComboBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.GPUComboBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.GPUComboBox.FormattingEnabled = true;
            this.GPUComboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4"});
            this.GPUComboBox.Location = new System.Drawing.Point(13, 94);
            this.GPUComboBox.Name = "GPUComboBox";
            this.GPUComboBox.Size = new System.Drawing.Size(346, 26);
            this.GPUComboBox.TabIndex = 1;
            this.GPUComboBox.Text = "Select GPU level (0 for default)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label7.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.label7.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.label7.Location = new System.Drawing.Point(13, 532);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 18);
            this.label7.TabIndex = 3;
            this.label7.Text = "Fov - X";
            this.label7.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label8.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label8.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.label8.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.label8.Location = new System.Drawing.Point(191, 532);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 18);
            this.label8.TabIndex = 3;
            this.label8.Text = "Fov - Y";
            this.label8.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label9.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.label9.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.label9.Location = new System.Drawing.Point(12, 583);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 18);
            this.label9.TabIndex = 3;
            this.label9.Text = "Refresh Rate";
            this.label9.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label6.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.label6.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.label6.Location = new System.Drawing.Point(93, 484);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "Enter Custom Username";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label5.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.label5.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.label5.Location = new System.Drawing.Point(16, 434);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(337, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "Custom Resolution Width (Height auto calculated)";
            this.label5.Visible = false;
            // 
            // ResolutionLabel
            // 
            this.ResolutionLabel.AutoSize = true;
            this.ResolutionLabel.BackColor = System.Drawing.Color.Transparent;
            this.ResolutionLabel.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ResolutionLabel.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ResolutionLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResolutionLabel.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.ResolutionLabel.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.ResolutionLabel.Location = new System.Drawing.Point(12, 183);
            this.ResolutionLabel.Name = "ResolutionLabel";
            this.ResolutionLabel.Size = new System.Drawing.Size(153, 18);
            this.ResolutionLabel.TabIndex = 3;
            this.ResolutionLabel.Text = "Resolution (0=default)";
            // 
            // GlobalUsername
            // 
            this.GlobalUsername.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.GlobalUsername.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.GlobalUsername.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.GlobalUsername.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.GlobalUsername.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.GlobalUsername.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.GlobalUsername.Location = new System.Drawing.Point(12, 701);
            this.GlobalUsername.Name = "GlobalUsername";
            this.GlobalUsername.Size = new System.Drawing.Size(262, 24);
            this.GlobalUsername.TabIndex = 12;
            this.GlobalUsername.TextChanged += new System.EventHandler(this.GlobalUsername_TextChanged);
            // 
            // FOVy
            // 
            this.FOVy.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.FOVy.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FOVy.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FOVy.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FOVy.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.FOVy.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.FOVy.Location = new System.Drawing.Point(189, 507);
            this.FOVy.Name = "FOVy";
            this.FOVy.Size = new System.Drawing.Size(169, 24);
            this.FOVy.TabIndex = 12;
            this.FOVy.Text = "0";
            this.FOVy.Visible = false;
            this.FOVy.TextChanged += new System.EventHandler(this.FOVy_TextChanged);
            // 
            // FOVx
            // 
            this.FOVx.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.FOVx.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FOVx.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FOVx.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FOVx.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.FOVx.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.FOVx.Location = new System.Drawing.Point(13, 507);
            this.FOVx.Name = "FOVx";
            this.FOVx.Size = new System.Drawing.Size(169, 24);
            this.FOVx.TabIndex = 11;
            this.FOVx.Text = "0";
            this.FOVx.Visible = false;
            this.FOVx.TextChanged += new System.EventHandler(this.FOVx_TextChanged);
            // 
            // UsrBox
            // 
            this.UsrBox.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.UsrBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UsrBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UsrBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UsrBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.UsrBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.UsrBox.Location = new System.Drawing.Point(13, 459);
            this.UsrBox.Name = "UsrBox";
            this.UsrBox.Size = new System.Drawing.Size(345, 24);
            this.UsrBox.TabIndex = 10;
            this.UsrBox.Text = "0";
            this.UsrBox.Visible = false;
            this.UsrBox.TextChanged += new System.EventHandler(this.UsrBox_TextChanged);
            // 
            // ResBox
            // 
            this.ResBox.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.ResBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ResBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ResBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ResBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.ResBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.ResBox.Location = new System.Drawing.Point(13, 409);
            this.ResBox.Name = "ResBox";
            this.ResBox.Size = new System.Drawing.Size(345, 24);
            this.ResBox.TabIndex = 9;
            this.ResBox.Text = "0";
            this.ResBox.Visible = false;
            this.ResBox.TextChanged += new System.EventHandler(this.ResBox_TextChanged);
            // 
            // TextureResTextBox
            // 
            this.TextureResTextBox.BackColor = global::AndroidSideloader.Properties.Settings.Default.TextBoxColor;
            this.TextureResTextBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "TextBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TextureResTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TextureResTextBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TextureResTextBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.TextureResTextBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.TextureResTextBox.Location = new System.Drawing.Point(13, 152);
            this.TextureResTextBox.Name = "TextureResTextBox";
            this.TextureResTextBox.Size = new System.Drawing.Size(167, 24);
            this.TextureResTextBox.TabIndex = 3;
            this.TextureResTextBox.Text = "0";
            // 
            // QURfrRt
            // 
            this.QURfrRt.BackColor = global::AndroidSideloader.Properties.Settings.Default.ComboBoxColor;
            this.QURfrRt.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QURfrRt.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QURfrRt.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "ComboBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QURfrRt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QURfrRt.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.QURfrRt.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.QURfrRt.FormattingEnabled = true;
            this.QURfrRt.Items.AddRange(new object[] {
            "0",
            "72",
            "90",
            "120"});
            this.QURfrRt.Location = new System.Drawing.Point(13, 555);
            this.QURfrRt.Name = "QURfrRt";
            this.QURfrRt.Size = new System.Drawing.Size(170, 26);
            this.QURfrRt.TabIndex = 13;
            this.QURfrRt.Text = "0";
            this.QURfrRt.Visible = false;
            this.QURfrRt.SelectedIndexChanged += new System.EventHandler(this.QURfrRt_SelectedIndexChanged);
            // 
            // RefreshRateComboBox
            // 
            this.RefreshRateComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.RefreshRateComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AndroidSideloader.Properties.Settings.Default, "FontStyle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RefreshRateComboBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AndroidSideloader.Properties.Settings.Default, "FontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RefreshRateComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RefreshRateComboBox.Font = global::AndroidSideloader.Properties.Settings.Default.FontStyle;
            this.RefreshRateComboBox.ForeColor = global::AndroidSideloader.Properties.Settings.Default.FontColor;
            this.RefreshRateComboBox.FormattingEnabled = true;
            this.RefreshRateComboBox.Items.AddRange(new object[] {
            "72",
            "90",
            "120"});
            this.RefreshRateComboBox.Location = new System.Drawing.Point(13, 65);
            this.RefreshRateComboBox.Name = "RefreshRateComboBox";
            this.RefreshRateComboBox.Size = new System.Drawing.Size(346, 26);
            this.RefreshRateComboBox.TabIndex = 0;
            this.RefreshRateComboBox.Text = "Select refresh rate";
            // 
            // button1
            // 
            this.button1.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.button1.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.button1.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.button1.Location = new System.Drawing.Point(189, 152);
            this.button1.Name = "button1";
            this.button1.Radius = 5;
            this.button1.Size = new System.Drawing.Size(170, 25);
            this.button1.Stroke = true;
            this.button1.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.button1.TabIndex = 19;
            this.button1.Text = "APPLY";
            this.button1.Transparency = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // WifiWake
            // 
            this.WifiWake.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.WifiWake.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.WifiWake.BackColor = System.Drawing.Color.Transparent;
            this.WifiWake.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.WifiWake.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.WifiWake.ForeColor = System.Drawing.Color.White;
            this.WifiWake.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.WifiWake.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.WifiWake.Location = new System.Drawing.Point(189, 180);
            this.WifiWake.Name = "WifiWake";
            this.WifiWake.Radius = 5;
            this.WifiWake.Size = new System.Drawing.Size(170, 25);
            this.WifiWake.Stroke = true;
            this.WifiWake.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.WifiWake.TabIndex = 20;
            this.WifiWake.Text = "ENABLE WIFI WAKE";
            this.WifiWake.Transparency = false;
            this.WifiWake.Click += new System.EventHandler(this.WifiWake_Click);
            // 
            // questVids
            // 
            this.questVids.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.questVids.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.questVids.BackColor = System.Drawing.Color.Transparent;
            this.questVids.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.questVids.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.questVids.ForeColor = System.Drawing.Color.White;
            this.questVids.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.questVids.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.questVids.Location = new System.Drawing.Point(189, 265);
            this.questVids.Name = "questVids";
            this.questVids.Radius = 5;
            this.questVids.Size = new System.Drawing.Size(167, 25);
            this.questVids.Stroke = true;
            this.questVids.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.questVids.TabIndex = 21;
            this.questVids.Text = "VIDEOSHOTS";
            this.questVids.Transparency = false;
            this.questVids.Click += new System.EventHandler(this.questVids_Click);
            // 
            // questPics
            // 
            this.questPics.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.questPics.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.questPics.BackColor = System.Drawing.Color.Transparent;
            this.questPics.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.questPics.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.questPics.ForeColor = System.Drawing.Color.White;
            this.questPics.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.questPics.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.questPics.Location = new System.Drawing.Point(15, 265);
            this.questPics.Name = "questPics";
            this.questPics.Radius = 5;
            this.questPics.Size = new System.Drawing.Size(165, 25);
            this.questPics.Stroke = true;
            this.questPics.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.questPics.TabIndex = 22;
            this.questPics.Text = "SCREENSHOTS";
            this.questPics.Transparency = false;
            this.questPics.Click += new System.EventHandler(this.questPics_Click);
            // 
            // QUEnable
            // 
            this.QUEnable.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.QUEnable.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.QUEnable.BackColor = System.Drawing.Color.Transparent;
            this.QUEnable.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.QUEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.QUEnable.ForeColor = System.Drawing.Color.White;
            this.QUEnable.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.QUEnable.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.QUEnable.Location = new System.Drawing.Point(189, 606);
            this.QUEnable.Name = "QUEnable";
            this.QUEnable.Radius = 5;
            this.QUEnable.Size = new System.Drawing.Size(170, 25);
            this.QUEnable.Stroke = true;
            this.QUEnable.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.QUEnable.TabIndex = 24;
            this.QUEnable.Text = "APPLY";
            this.QUEnable.Transparency = false;
            this.QUEnable.Click += new System.EventHandler(this.QUEnable_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.deleteButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.deleteButton.BackColor = System.Drawing.Color.Transparent;
            this.deleteButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.deleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.deleteButton.ForeColor = System.Drawing.Color.White;
            this.deleteButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.deleteButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.deleteButton.Location = new System.Drawing.Point(13, 606);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Radius = 5;
            this.deleteButton.Size = new System.Drawing.Size(167, 25);
            this.deleteButton.Stroke = true;
            this.deleteButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.deleteButton.TabIndex = 25;
            this.deleteButton.Text = "DELETE SAVED";
            this.deleteButton.Transparency = false;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ResetQU
            // 
            this.ResetQU.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ResetQU.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ResetQU.BackColor = System.Drawing.Color.Transparent;
            this.ResetQU.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ResetQU.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.ResetQU.ForeColor = System.Drawing.Color.White;
            this.ResetQU.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ResetQU.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ResetQU.Location = new System.Drawing.Point(189, 555);
            this.ResetQU.Name = "ResetQU";
            this.ResetQU.Radius = 5;
            this.ResetQU.Size = new System.Drawing.Size(168, 28);
            this.ResetQU.Stroke = true;
            this.ResetQU.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.ResetQU.TabIndex = 26;
            this.ResetQU.Text = "RESET ALL FIELDS";
            this.ResetQU.Transparency = false;
            this.ResetQU.Click += new System.EventHandler(this.ResetQU_click);
            // 
            // button3
            // 
            this.button3.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.button3.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.button3.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.button3.Location = new System.Drawing.Point(278, 700);
            this.button3.Name = "button3";
            this.button3.Radius = 5;
            this.button3.Size = new System.Drawing.Size(81, 25);
            this.button3.Stroke = true;
            this.button3.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.button3.TabIndex = 27;
            this.button3.Text = "APPLY";
            this.button3.Transparency = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // QuestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_cubes_1_1_1_0_0_0_1__000000_212121;
            this.ClientSize = new System.Drawing.Size(370, 737);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.ResetQU);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.QUEnable);
            this.Controls.Add(this.questPics);
            this.Controls.Add(this.questVids);
            this.Controls.Add(this.WifiWake);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.DeleteShots);
            this.Controls.Add(this.QUon);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CPUComboBox);
            this.Controls.Add(this.GPUComboBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ResolutionLabel);
            this.Controls.Add(this.GlobalUsername);
            this.Controls.Add(this.FOVy);
            this.Controls.Add(this.FOVx);
            this.Controls.Add(this.UsrBox);
            this.Controls.Add(this.ResBox);
            this.Controls.Add(this.TextureResTextBox);
            this.Controls.Add(this.QURfrRt);
            this.Controls.Add(this.RefreshRateComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(386, 776);
            this.MinimumSize = new System.Drawing.Size(386, 776);
            this.Name = "QuestForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quest settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.QuestForm_FormClosed);
            this.Load += new System.EventHandler(this.QuestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox RefreshRateComboBox;
        private System.Windows.Forms.TextBox TextureResTextBox;
        private System.Windows.Forms.Label ResolutionLabel;
        private System.Windows.Forms.ComboBox GPUComboBox;
        private System.Windows.Forms.ComboBox CPUComboBox;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox QURfrRt;
        private TextBox ResBox;
        private TextBox UsrBox;
        private TextBox FOVx;
        private TextBox FOVy;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label11;
        private CheckBox QUon;
        private Label label12;
        private Label label13;
        private CheckBox DeleteShots;
        private Splitter splitter1;
        private Label label10;
        private Label label14;
        private Label label15;
        private Label label16;
        private TextBox GlobalUsername;
        private RoundButton button1;
        private RoundButton WifiWake;
        private RoundButton questVids;
        private RoundButton questPics;
        private RoundButton QUEnable;
        private RoundButton deleteButton;
        private RoundButton ResetQU;
        private RoundButton button3;
    }

}
