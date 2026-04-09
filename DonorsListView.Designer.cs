
namespace AndroidSideloader
{
    partial class DonorsListViewForm
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
            this.DonationTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.skip_forever = new AndroidSideloader.RoundButton();
            this.SkipButton = new AndroidSideloader.RoundButton();
            this.DonateButton = new AndroidSideloader.RoundButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DonorsListView = new System.Windows.Forms.ListView();
            this.GameNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PackageNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VersionCodeIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UpdateOrNew = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bothdet = new System.Windows.Forms.Label();
            this.newdet = new System.Windows.Forms.Label();
            this.upddet = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TimerDesc = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.skip_forever);
            this.panel1.Controls.Add(this.SkipButton);
            this.panel1.Controls.Add(this.DonateButton);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.bothdet);
            this.panel1.Controls.Add(this.newdet);
            this.panel1.Controls.Add(this.upddet);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.TimerDesc);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(16);
            this.panel1.Size = new System.Drawing.Size(460, 420);
            this.panel1.TabIndex = 1;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // skip_forever
            // 
            this.skip_forever.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.skip_forever.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.skip_forever.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skip_forever.BackColor = System.Drawing.Color.Transparent;
            this.skip_forever.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.skip_forever.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.skip_forever.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.skip_forever.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.skip_forever.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.skip_forever.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this.skip_forever.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this.skip_forever.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this.skip_forever.Location = new System.Drawing.Point(20, 380);
            this.skip_forever.Margin = new System.Windows.Forms.Padding(0);
            this.skip_forever.Name = "skip_forever";
            this.skip_forever.Radius = 4;
            this.skip_forever.Size = new System.Drawing.Size(420, 32);
            this.skip_forever.Stroke = true;
            this.skip_forever.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.skip_forever.TabIndex = 97;
            this.skip_forever.Text = "Add to blacklist / Never ask for the selected apps again";
            this.skip_forever.Transparency = false;
            this.skip_forever.Click += new System.EventHandler(this.skip_forever_Click);
            // 
            // SkipButton
            // 
            this.SkipButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.SkipButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.SkipButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SkipButton.BackColor = System.Drawing.Color.Transparent;
            this.SkipButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SkipButton.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.SkipButton.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.SkipButton.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.SkipButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkipButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this.SkipButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this.SkipButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this.SkipButton.Location = new System.Drawing.Point(20, 326);
            this.SkipButton.Margin = new System.Windows.Forms.Padding(0);
            this.SkipButton.Name = "SkipButton";
            this.SkipButton.Radius = 4;
            this.SkipButton.Size = new System.Drawing.Size(100, 32);
            this.SkipButton.Stroke = true;
            this.SkipButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.SkipButton.TabIndex = 96;
            this.SkipButton.Text = "Skip";
            this.SkipButton.Transparency = false;
            this.SkipButton.Click += new System.EventHandler(this.SkipButton_Click);
            // 
            // DonateButton
            // 
            this.DonateButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(140)))), ((int)(((byte)(115)))));
            this.DonateButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(125)))), ((int)(((byte)(105)))));
            this.DonateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DonateButton.BackColor = System.Drawing.Color.Transparent;
            this.DonateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DonateButton.Disabled1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.DonateButton.Disabled2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(28)))), ((int)(((byte)(35)))));
            this.DonateButton.DisabledStrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.DonateButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.DonateButton.ForeColor = System.Drawing.Color.White;
            this.DonateButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(120)))), ((int)(((byte)(100)))));
            this.DonateButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(100)))), ((int)(((byte)(85)))));
            this.DonateButton.Location = new System.Drawing.Point(128, 326);
            this.DonateButton.Margin = new System.Windows.Forms.Padding(0);
            this.DonateButton.Name = "DonateButton";
            this.DonateButton.Radius = 4;
            this.DonateButton.Size = new System.Drawing.Size(312, 32);
            this.DonateButton.Stroke = true;
            this.DonateButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(150)))), ((int)(((byte)(125)))));
            this.DonateButton.TabIndex = 95;
            this.DonateButton.Text = "Share Selected Apps";
            this.DonateButton.Transparency = false;
            this.DonateButton.Click += new System.EventHandler(this.DonateButton_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.panel2.Controls.Add(this.DonorsListView);
            this.panel2.Location = new System.Drawing.Point(20, 70);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(1);
            this.panel2.Size = new System.Drawing.Size(420, 250);
            this.panel2.TabIndex = 2;
            // 
            // DonorsListView
            // 
            this.DonorsListView.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.DonorsListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.DonorsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DonorsListView.CausesValidation = false;
            this.DonorsListView.CheckBoxes = true;
            this.DonorsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GameNameIndex,
            this.PackageNameIndex,
            this.VersionCodeIndex,
            this.UpdateOrNew});
            this.DonorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DonorsListView.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.DonorsListView.ForeColor = System.Drawing.Color.White;
            this.DonorsListView.FullRowSelect = true;
            this.DonorsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.DonorsListView.HideSelection = false;
            this.DonorsListView.Location = new System.Drawing.Point(1, 1);
            this.DonorsListView.MinimumSize = new System.Drawing.Size(100, 100);
            this.DonorsListView.Name = "DonorsListView";
            this.DonorsListView.Size = new System.Drawing.Size(418, 248);
            this.DonorsListView.TabIndex = 0;
            this.DonorsListView.UseCompatibleStateImageBehavior = false;
            this.DonorsListView.View = System.Windows.Forms.View.Details;
            this.DonorsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.DonorsListView_ItemChecked);
            this.DonorsListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.DonorsListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.DonorsListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // GameNameIndex
            // 
            this.GameNameIndex.Text = "App Name";
            this.GameNameIndex.Width = 220;
            // 
            // PackageNameIndex
            // 
            this.PackageNameIndex.DisplayIndex = 2;
            this.PackageNameIndex.Text = "Package";
            this.PackageNameIndex.Width = 0;
            // 
            // VersionCodeIndex
            // 
            this.VersionCodeIndex.DisplayIndex = 3;
            this.VersionCodeIndex.Text = "Version";
            this.VersionCodeIndex.Width = 100;
            // 
            // UpdateOrNew
            // 
            this.UpdateOrNew.DisplayIndex = 1;
            this.UpdateOrNew.Text = "Type";
            this.UpdateOrNew.Width = 80;
            // 
            // bothdet
            // 
            this.bothdet.AutoSize = true;
            this.bothdet.BackColor = System.Drawing.Color.Transparent;
            this.bothdet.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.bothdet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.bothdet.Location = new System.Drawing.Point(20, 15);
            this.bothdet.Name = "bothdet";
            this.bothdet.Size = new System.Drawing.Size(228, 20);
            this.bothdet.TabIndex = 3;
            this.bothdet.Text = "Updates && New Apps Available";
            this.bothdet.Visible = false;
            this.bothdet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.bothdet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.bothdet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // newdet
            // 
            this.newdet.AutoSize = true;
            this.newdet.BackColor = System.Drawing.Color.Transparent;
            this.newdet.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.newdet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.newdet.Location = new System.Drawing.Point(20, 15);
            this.newdet.Name = "newdet";
            this.newdet.Size = new System.Drawing.Size(149, 20);
            this.newdet.TabIndex = 3;
            this.newdet.Text = "New Apps Available";
            this.newdet.Visible = false;
            this.newdet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.newdet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.newdet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // upddet
            // 
            this.upddet.AutoSize = true;
            this.upddet.BackColor = System.Drawing.Color.Transparent;
            this.upddet.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.upddet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this.upddet.Location = new System.Drawing.Point(20, 15);
            this.upddet.Name = "upddet";
            this.upddet.Size = new System.Drawing.Size(135, 20);
            this.upddet.TabIndex = 3;
            this.upddet.Text = "Updates";
            this.upddet.Visible = false;
            this.upddet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.upddet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.upddet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this.label2.Location = new System.Drawing.Point(20, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(338, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "All apps are donated by users! Help the community by sharing.";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // TimerDesc
            // 
            this.TimerDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TimerDesc.AutoSize = true;
            this.TimerDesc.BackColor = System.Drawing.Color.Transparent;
            this.TimerDesc.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.TimerDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(125)))), ((int)(((byte)(135)))));
            this.TimerDesc.Location = new System.Drawing.Point(79, 362);
            this.TimerDesc.Name = "TimerDesc";
            this.TimerDesc.Size = new System.Drawing.Size(292, 13);
            this.TimerDesc.TabIndex = 3;
            this.TimerDesc.Text = "Don\'t share free apps. Upload happens in background.";
            this.TimerDesc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.TimerDesc.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.TimerDesc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            // 
            // DonorsListViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(460, 420);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DonorsListViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.DonorsListViewForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DonorsListViewForm_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView DonorsListView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label TimerDesc;
        private System.Windows.Forms.ColumnHeader GameNameIndex;
        private System.Windows.Forms.ColumnHeader PackageNameIndex;
        private System.Windows.Forms.ColumnHeader VersionCodeIndex;
        private System.Windows.Forms.ColumnHeader UpdateOrNew;
        public System.Windows.Forms.Timer DonationTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label bothdet;
        private System.Windows.Forms.Label newdet;
        private System.Windows.Forms.Label upddet;
        private System.Windows.Forms.Panel panel2;
        private RoundButton DonateButton;
        private RoundButton SkipButton;
        private RoundButton skip_forever;
    }
}