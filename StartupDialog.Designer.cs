namespace AndroidSideloader
{
    partial class StartupDialog
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
            this._offlineButton = new System.Windows.Forms.Label();
            this._orLabel = new System.Windows.Forms.Label();
            this._separator = new System.Windows.Forms.Panel();
            this._errorLabel = new System.Windows.Forms.Label();
            this._btnProvidePublic = new System.Windows.Forms.Label();
            this._btnProvideRclone = new System.Windows.Forms.Label();
            this._titleLabel = new System.Windows.Forms.Label();
            this._existingConfigLabel = new System.Windows.Forms.Label();
            this._existingRcloneLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _offlineButton
            // 
            this._offlineButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this._offlineButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._offlineButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this._offlineButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this._offlineButton.Location = new System.Drawing.Point(2, 161);
            this._offlineButton.Name = "_offlineButton";
            this._offlineButton.Size = new System.Drawing.Size(378, 32);
            this._offlineButton.TabIndex = 9;
            this._offlineButton.Text = "USE LOCAL LIBRARY";
            this._offlineButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _orLabel
            // 
            this._orLabel.AutoSize = true;
            this._orLabel.BackColor = System.Drawing.Color.Transparent;
            this._orLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this._orLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(125)))), ((int)(((byte)(135)))));
            this._orLabel.Location = new System.Drawing.Point(3, 139);
            this._orLabel.Name = "_orLabel";
            this._orLabel.Size = new System.Drawing.Size(224, 13);
            this._orLabel.TabIndex = 8;
            this._orLabel.Text = "or continue offline using your local library";
            // 
            // _separator
            // 
            this._separator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this._separator.Location = new System.Drawing.Point(1, 131);
            this._separator.Name = "_separator";
            this._separator.Size = new System.Drawing.Size(380, 1);
            this._separator.TabIndex = 7;
            // 
            // _errorLabel
            // 
            this._errorLabel.BackColor = System.Drawing.Color.Transparent;
            this._errorLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this._errorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(47)))), ((int)(((byte)(87)))));
            this._errorLabel.Location = new System.Drawing.Point(2, 30);
            this._errorLabel.Name = "_errorLabel";
            this._errorLabel.Size = new System.Drawing.Size(380, 16);
            this._errorLabel.TabIndex = 6;
            this._errorLabel.Visible = false;
            // 
            // _btnProvidePublic
            // 
            this._btnProvidePublic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this._btnProvidePublic.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnProvidePublic.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this._btnProvidePublic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this._btnProvidePublic.Location = new System.Drawing.Point(2, 50);
            this._btnProvidePublic.Name = "_btnProvidePublic";
            this._btnProvidePublic.Size = new System.Drawing.Size(378, 34);
            this._btnProvidePublic.TabIndex = 2;
            this._btnProvidePublic.Text = "PROVIDE PUBLIC CONFIG";
            this._btnProvidePublic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _btnProvideRclone
            // 
            this._btnProvideRclone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(42)))));
            this._btnProvideRclone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnProvideRclone.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this._btnProvideRclone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this._btnProvideRclone.Location = new System.Drawing.Point(2, 90);
            this._btnProvideRclone.Name = "_btnProvideRclone";
            this._btnProvideRclone.Size = new System.Drawing.Size(378, 30);
            this._btnProvideRclone.TabIndex = 3;
            this._btnProvideRclone.Text = "PROVIDE RCLONE CONFIG";
            this._btnProvideRclone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.BackColor = System.Drawing.Color.Transparent;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this._titleLabel.Location = new System.Drawing.Point(1, 7);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(134, 20);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Rookie Sideloader";
            this._titleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseDown);
            this._titleLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseMove);
            this._titleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseUp);
            // 
            // _existingConfigLabel
            // 
            this._existingConfigLabel.AutoSize = true;
            this._existingConfigLabel.BackColor = System.Drawing.Color.Transparent;
            this._existingConfigLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this._existingConfigLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this._existingConfigLabel.Location = new System.Drawing.Point(3, 30);
            this._existingConfigLabel.Name = "_existingConfigLabel";
            this._existingConfigLabel.Size = new System.Drawing.Size(0, 13);
            this._existingConfigLabel.TabIndex = 12;
            this._existingConfigLabel.Visible = false;
            // 
            // _existingRcloneLabel
            // 
            this._existingRcloneLabel.AutoSize = true;
            this._existingRcloneLabel.BackColor = System.Drawing.Color.Transparent;
            this._existingRcloneLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this._existingRcloneLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(203)))), ((int)(((byte)(173)))));
            this._existingRcloneLabel.Location = new System.Drawing.Point(3, 30);
            this._existingRcloneLabel.Name = "_existingRcloneLabel";
            this._existingRcloneLabel.Size = new System.Drawing.Size(0, 13);
            this._existingRcloneLabel.TabIndex = 14;
            this._existingRcloneLabel.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.panel1.Controls.Add(this._titleLabel);
            this.panel1.Controls.Add(this._existingConfigLabel);
            this.panel1.Controls.Add(this._existingRcloneLabel);
            this.panel1.Controls.Add(this._btnProvidePublic);
            this.panel1.Controls.Add(this._btnProvideRclone);
            this.panel1.Controls.Add(this._errorLabel);
            this.panel1.Controls.Add(this._separator);
            this.panel1.Controls.Add(this._orLabel);
            this.panel1.Controls.Add(this._offlineButton);
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(165)))), ((int)(((byte)(175)))));
            this.panel1.Location = new System.Drawing.Point(8, 4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(16);
            this.panel1.Size = new System.Drawing.Size(391, 210);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseUp);
            // 
            // StartupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(407, 218);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StartupDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rookie Sideloader";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StartupDialog_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _offlineButton;
        private System.Windows.Forms.Label _orLabel;
        private System.Windows.Forms.Panel _separator;
        private System.Windows.Forms.Label _errorLabel;
        private System.Windows.Forms.Label _btnProvidePublic;
        private System.Windows.Forms.Label _btnProvideRclone;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _existingConfigLabel;
        private System.Windows.Forms.Label _existingRcloneLabel;
        private System.Windows.Forms.Panel panel1;
    }
}
