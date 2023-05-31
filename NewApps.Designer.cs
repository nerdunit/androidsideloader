
namespace AndroidSideloader
{
    partial class NewApps
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewApps));
            this.NewAppsListView = new System.Windows.Forms.ListView();
            this.GameNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PackageNameIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.NewAppsButton = new AndroidSideloader.RoundButton();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // NewAppsListView
            // 
            this.NewAppsListView.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.NewAppsListView.BackColor = global::AndroidSideloader.Properties.Settings.Default.BackColor;
            this.NewAppsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NewAppsListView.CausesValidation = false;
            this.NewAppsListView.CheckBoxes = true;
            this.NewAppsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GameNameIndex,
            this.PackageNameIndex});
            this.NewAppsListView.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "BackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NewAppsListView.ForeColor = System.Drawing.Color.White;
            this.NewAppsListView.FullRowSelect = true;
            this.NewAppsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.NewAppsListView.HideSelection = false;
            this.NewAppsListView.Location = new System.Drawing.Point(3, 5);
            this.NewAppsListView.Name = "NewAppsListView";
            this.NewAppsListView.RightToLeftLayout = true;
            this.NewAppsListView.Size = new System.Drawing.Size(288, 167);
            this.NewAppsListView.TabIndex = 1;
            this.NewAppsListView.UseCompatibleStateImageBehavior = false;
            this.NewAppsListView.View = System.Windows.Forms.View.Details;
            this.NewAppsListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label2_MouseDown);
            this.NewAppsListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label2_MouseMove);
            this.NewAppsListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label2_MouseUp);
            // 
            // GameNameIndex
            // 
            this.GameNameIndex.Text = "Game Name";
            this.GameNameIndex.Width = 284;
            // 
            // PackageNameIndex
            // 
            this.PackageNameIndex.Width = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.panel2.Controls.Add(this.NewAppsListView);
            this.panel2.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.panel2.Location = new System.Drawing.Point(9, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(295, 175);
            this.panel2.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(28, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(256, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Check box of all free/non-VR apps";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label2_MouseDown);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label2_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label2_MouseUp);
            // 
            // NewAppsButton
            // 
            this.NewAppsButton.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.NewAppsButton.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.NewAppsButton.BackColor = System.Drawing.Color.Transparent;
            this.NewAppsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.NewAppsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.NewAppsButton.ForeColor = System.Drawing.Color.White;
            this.NewAppsButton.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.NewAppsButton.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.NewAppsButton.Location = new System.Drawing.Point(12, 212);
            this.NewAppsButton.Name = "NewAppsButton";
            this.NewAppsButton.Radius = 5;
            this.NewAppsButton.Size = new System.Drawing.Size(288, 29);
            this.NewAppsButton.Stroke = true;
            this.NewAppsButton.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.NewAppsButton.TabIndex = 2;
            this.NewAppsButton.Text = "Accept";
            this.NewAppsButton.Transparency = false;
            this.NewAppsButton.Click += new System.EventHandler(this.DonateButton_Click);
            // 
            // NewApps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = global::AndroidSideloader.Properties.Settings.Default.BackColor;
            this.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_cubes_1_1_1_0_0_0_1__000000_212121;
            this.ClientSize = new System.Drawing.Size(313, 248);
            this.ControlBox = false;
            this.Controls.Add(this.NewAppsButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "BackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewApps";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.NewApps_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label2_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label2_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label2_MouseUp);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView NewAppsListView;
        private System.Windows.Forms.ColumnHeader GameNameIndex;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader PackageNameIndex;
        private RoundButton NewAppsButton;
    }
}