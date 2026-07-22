using AndroidSideloader.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AndroidSideloader
{
    // Shown when an SSH server is detected on the headset but none of the stored
    // credentials work. Lets the user supply a username plus a password and/or a
    // private key file, which are persisted to settings for future sessions.
    public class SftpCredentialsForm : Form
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;

        private TextBox usernameBox;
        private TextBox passwordBox;
        private TextBox keyPathBox;
        private CheckBox disableCheck;

        public SftpCredentialsForm()
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Text = "SFTP Fast Transfers - Credentials";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(460, 300);
            BackColor = settings.BackColor;
            ForeColor = settings.FontColor;
            Font = settings.FontStyle;

            Label header = new Label
            {
                Text = "An SSH server was found on your headset, but Rookie could not log in.\n" +
                       "Enter its credentials to enable fast OBB transfers over SFTP.",
                Location = new Point(12, 10),
                Size = new Size(436, 50),
                ForeColor = settings.FontColor
            };
            Controls.Add(header);

            Controls.Add(MakeLabel("Username:", 12, 70));
            usernameBox = MakeTextBox(130, 67, 200);
            usernameBox.Text = string.IsNullOrEmpty(settings.SftpUsername) ? "root" : settings.SftpUsername;
            Controls.Add(usernameBox);

            Controls.Add(MakeLabel("Password:", 12, 110));
            passwordBox = MakeTextBox(130, 107, 200);
            passwordBox.UseSystemPasswordChar = true;
            passwordBox.Text = settings.SftpPassword;
            Controls.Add(passwordBox);

            Controls.Add(MakeLabel("Key file:", 12, 150));
            keyPathBox = MakeTextBox(130, 147, 240);
            keyPathBox.Text = settings.SftpPrivateKeyPath;
            Controls.Add(keyPathBox);

            Button browseButton = new Button
            {
                Text = "...",
                Location = new Point(378, 145),
                Size = new Size(40, 28),
                BackColor = settings.ButtonColor,
                ForeColor = settings.FontColor,
                FlatStyle = FlatStyle.Flat
            };
            browseButton.Click += (s, e) =>
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Title = "Select SSH private key";
                    string sshDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
                    if (Directory.Exists(sshDir))
                    {
                        dialog.InitialDirectory = sshDir;
                    }
                    dialog.Filter = "All files (*.*)|*.*";
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        keyPathBox.Text = dialog.FileName;
                    }
                }
            };
            Controls.Add(browseButton);

            disableCheck = new CheckBox
            {
                Text = "Don't ask again (disable SFTP fast transfers)",
                Location = new Point(15, 195),
                Size = new Size(420, 24),
                ForeColor = settings.FontColor
            };
            Controls.Add(disableCheck);

            Button okButton = new Button
            {
                Text = "Save && Retry",
                Location = new Point(130, 240),
                Size = new Size(120, 34),
                BackColor = settings.ButtonColor,
                ForeColor = settings.FontColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.Click += OkClicked;
            Controls.Add(okButton);

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 240),
                Size = new Size(100, 34),
                BackColor = settings.ButtonColor,
                ForeColor = settings.FontColor,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(112, 24),
                ForeColor = settings.FontColor
            };
        }

        private TextBox MakeTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 28),
                BackColor = settings.TextBoxColor,
                ForeColor = settings.FontColor,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void OkClicked(object sender, EventArgs e)
        {
            if (disableCheck.Checked)
            {
                settings.EnableSftpTransfers = false;
                settings.Save();
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            string keyPath = keyPathBox.Text.Trim();
            if (!string.IsNullOrEmpty(keyPath) && !File.Exists(keyPath))
            {
                _ = MessageBox.Show(this, "The selected key file does not exist.", "SFTP",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(keyPath) && string.IsNullOrEmpty(passwordBox.Text))
            {
                _ = MessageBox.Show(this, "Enter a password or select a private key file.", "SFTP",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            settings.SftpUsername = usernameBox.Text.Trim();
            settings.SftpPassword = passwordBox.Text;
            settings.SftpPrivateKeyPath = keyPath;
            settings.Save();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
