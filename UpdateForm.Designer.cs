
namespace AndroidSideloader
{
    partial class UpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.YesUpdate = new AndroidSideloader.RoundButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.UpdateTextBox = new System.Windows.Forms.RichTextBox();
            this.UpdateVerLabel = new System.Windows.Forms.Label();
            this.CurVerLabel = new System.Windows.Forms.Label();
            this.SkipUpdate = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = global::AndroidSideloader.Properties.Settings.Default.BackColor;
            this.panel1.BackgroundImage = global::AndroidSideloader.Properties.Resources.pattern_cubes_1_1_1_0_0_0_1__000000_212121;
            this.panel1.Controls.Add(this.YesUpdate);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.UpdateVerLabel);
            this.panel1.Controls.Add(this.CurVerLabel);
            this.panel1.Controls.Add(this.SkipUpdate);
            this.panel1.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "BackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.panel1.Location = new System.Drawing.Point(-6, -6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(474, 305);
            this.panel1.TabIndex = 5;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseUp);
            // 
            // YesUpdate
            // 
            this.YesUpdate.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.YesUpdate.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.YesUpdate.BackColor = System.Drawing.Color.Transparent;
            this.YesUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.YesUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.YesUpdate.ForeColor = System.Drawing.Color.White;
            this.YesUpdate.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.YesUpdate.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.YesUpdate.Location = new System.Drawing.Point(339, 245);
            this.YesUpdate.Name = "YesUpdate";
            this.YesUpdate.Radius = 5;
            this.YesUpdate.Size = new System.Drawing.Size(111, 31);
            this.YesUpdate.Stroke = true;
            this.YesUpdate.StrokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.YesUpdate.TabIndex = 2;
            this.YesUpdate.Text = "Update Now";
            this.YesUpdate.Transparency = false;
            this.YesUpdate.Click += new System.EventHandler(this.YesUpdate_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = global::AndroidSideloader.Properties.Settings.Default.SubButtonColor;
            this.panel3.Controls.Add(this.UpdateTextBox);
            this.panel3.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "SubButtonColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.panel3.Location = new System.Drawing.Point(21, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(432, 218);
            this.panel3.TabIndex = 0;
            // 
            // UpdateTextBox
            // 
            this.UpdateTextBox.BackColor = global::AndroidSideloader.Properties.Settings.Default.ComboBoxColor;
            this.UpdateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.UpdateTextBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AndroidSideloader.Properties.Settings.Default, "ComboBoxColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UpdateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.UpdateTextBox.ForeColor = System.Drawing.Color.White;
            this.UpdateTextBox.Location = new System.Drawing.Point(12, 8);
            this.UpdateTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.UpdateTextBox.Name = "UpdateTextBox";
            this.UpdateTextBox.ReadOnly = true;
            this.UpdateTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.UpdateTextBox.Size = new System.Drawing.Size(408, 200);
            this.UpdateTextBox.TabIndex = 1;
            this.UpdateTextBox.Text = "";
            this.UpdateTextBox.TextChanged += new System.EventHandler(this.UpdateTextBox_TextChanged);
            this.UpdateTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseDown);
            this.UpdateTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseMove);
            this.UpdateTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseUp);
            // 
            // UpdateVerLabel
            // 
            this.UpdateVerLabel.AutoSize = true;
            this.UpdateVerLabel.BackColor = System.Drawing.Color.Transparent;
            this.UpdateVerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.UpdateVerLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.UpdateVerLabel.Location = new System.Drawing.Point(21, 261);
            this.UpdateVerLabel.Name = "UpdateVerLabel";
            this.UpdateVerLabel.Size = new System.Drawing.Size(94, 15);
            this.UpdateVerLabel.TabIndex = 3;
            this.UpdateVerLabel.Text = "Update Version:";
            // 
            // CurVerLabel
            // 
            this.CurVerLabel.AutoSize = true;
            this.CurVerLabel.BackColor = System.Drawing.Color.Transparent;
            this.CurVerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.CurVerLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.CurVerLabel.Location = new System.Drawing.Point(21, 245);
            this.CurVerLabel.Name = "CurVerLabel";
            this.CurVerLabel.Size = new System.Drawing.Size(94, 15);
            this.CurVerLabel.TabIndex = 2;
            this.CurVerLabel.Text = "Current Version:";
            // 
            // SkipUpdate
            // 
            this.SkipUpdate.AutoSize = true;
            this.SkipUpdate.BackColor = System.Drawing.Color.Transparent;
            this.SkipUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SkipUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SkipUpdate.ForeColor = System.Drawing.Color.Silver;
            this.SkipUpdate.Location = new System.Drawing.Point(374, 279);
            this.SkipUpdate.Name = "SkipUpdate";
            this.SkipUpdate.Size = new System.Drawing.Size(76, 13);
            this.SkipUpdate.TabIndex = 4;
            this.SkipUpdate.Text = "𝖲𝖪𝖨𝖯 𝖥𝖮𝖱 𝖭𝖮𝖶";
            this.SkipUpdate.Click += new System.EventHandler(this.SkipUpdate_Click);
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(462, 291);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpdateForm_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SkipUpdate;
        private System.Windows.Forms.Label CurVerLabel;
        private System.Windows.Forms.Label UpdateVerLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RichTextBox UpdateTextBox;
        private System.Windows.Forms.Panel panel1;
        private RoundButton YesUpdate;
    }
}