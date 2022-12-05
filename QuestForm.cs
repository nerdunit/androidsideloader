using System;
using System.IO;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class QuestForm : Form
    {
        public static int length = 0;
        public static string[] result;
        public bool settingsexist = false;
        public static bool QUSon = false;
        public bool delsh = false;
        public QuestForm()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            bool ChangesMade = false;

            //Quest 2 settings, might remove them in the future since some of them are broken
            if (RefreshRateComboBox.SelectedIndex != -1)
            {
                ADB.WakeDevice();
                _ = ADB.RunAdbCommandToString($"shell setprop debug.oculus.refreshRate {RefreshRateComboBox.SelectedItem}");
                _ = ADB.RunAdbCommandToString($"shell settings put global 90hz_global {RefreshRateComboBox.SelectedIndex}");
                _ = ADB.RunAdbCommandToString($"shell settings put global 90hzglobal {RefreshRateComboBox.SelectedIndex}");
                ChangesMade = true;
            }

            if (TextureResTextBox.Text.Length > 0)
            {
                ADB.WakeDevice();
                _ = int.TryParse(TextureResTextBox.Text, out _);
                _ = ADB.RunAdbCommandToString($"shell settings put global texture_size_Global {TextureResTextBox.Text}");
                _ = ADB.RunAdbCommandToString($"shell setprop debug.oculus.textureWidth {TextureResTextBox.Text}");
                _ = ADB.RunAdbCommandToString($"shell setprop debug.oculus.textureHeight {TextureResTextBox.Text}");
                ChangesMade = true;
            }

            if (CPUComboBox.SelectedIndex != -1)
            {
                _ = ADB.RunAdbCommandToString($"shell setprop debug.oculus.cpuLevel {CPUComboBox.SelectedItem}");
                ChangesMade = true;
            }

            if (GPUComboBox.SelectedIndex != -1)
            {
                _ = ADB.RunAdbCommandToString($"shell setprop debug.oculus.gpuLevel {GPUComboBox.SelectedItem}");
                ChangesMade = true;
            }

            if (ChangesMade)
            {
                _ = MessageBox.Show("Settings applied!");
            }
        }


        public static void setLength(int value)
        {
            result = new string[value];
        }



        public void ResetQU_click(object sender, EventArgs e)
        {
            ResBox.Text = "0";
            UsrBox.Text = "Change Me";
            FOVx.Text = "0";
            FOVy.Text = "0";
            QURfrRt.SelectedIndex = 0;
        }

        private void DeleteShots_CheckedChanged(object sender, EventArgs e)
        {
            delsh = DeleteShots.Checked;
        }
        private void QUon_CheckedChanged(object sender, EventArgs e)
        {
            if (QUon.Checked)
            {
                ResBox.Visible = true;
                UsrBox.Visible = true;
                FOVx.Visible = true;
                FOVy.Visible = true;
                QURfrRt.Visible = true;
                ResetQU.Visible = true;
                QUEnable.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                deleteButton.Visible = true;
                ResBox.Text = Properties.Settings.Default.QUres;
                UsrBox.Text = Properties.Settings.Default.QUname;
                FOVy.Text = Properties.Settings.Default.QUy;
                FOVx.Text = Properties.Settings.Default.QUx;
                QURfrRt.SelectedValue = Properties.Settings.Default.QUhz;
                Properties.Settings.Default.QUturnedon = true;
            }
            else if (!QUon.Checked)
            {
                ResBox.Visible = false;
                UsrBox.Visible = false;
                FOVx.Visible = false;
                FOVy.Visible = false;
                QURfrRt.Visible = false;
                ResetQU.Visible = false;
                QUEnable.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                deleteButton.Visible = false;
                Properties.Settings.Default.QUturnedon = false;
                _ = MessageBox.Show("Ok, Deleted your custom settings file.\nIf you would like to re-enable return here and apply settings again");
                File.Delete($"{Environment.CurrentDirectory}\\Config.Json");
                File.Delete($"{Environment.CurrentDirectory}\\delete_settings");
            }

        }
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        private void QUEnable_Click(object sender, EventArgs e)
        {

            settingsexist = true;
            _ = MessageBox.Show("OK, any -QU packages installed will have these settings applied!\nTo delete settings: goto main app window, select a game with top menu, and click \"Remove QU Setting\"");
            if (QUon.Checked)
            {
                Properties.Settings.Default.QUturnedon = true;
                Random r = new Random();

                int x = r.Next(999999999);
                int y = r.Next(9999999);

                long sum = (y * (long)1000000000) + x;

                int x2 = r.Next(999999999);
                int y2 = r.Next(9999999);

                long sum2 = (y2 * (long)1000000000) + x2;

                QUSon = true;

                string selected = QURfrRt.GetItemText(QURfrRt.SelectedItem);

                Properties.Settings.Default.QUString = $"\"refresh_rate\":{selected},\"eye_texture_width\":{ResBox.Text},\"fov_x\":{FOVx.Text},\"fov_y\":{FOVy.Text},\"username\":\"{UsrBox.Text}\"}}";
                Properties.Settings.Default.QUStringF = $"{{\"user_id\":{sum},\"app_id\":\"{sum2}\",";
                Properties.Settings.Default.Save();
                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\delete_settings", "");
                string boff = Properties.Settings.Default.QUStringF + Properties.Settings.Default.QUString;
                File.WriteAllText($"{Properties.Settings.Default.MainDir}\\config.json", boff);
            }
            else
            {
                Properties.Settings.Default.QUturnedon = false;
            }



        }



        private void QuestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (QUon.Checked)
            {
                Properties.Settings.Default.QUsett = true;
                Properties.Settings.Default.Save();
            }
            if (!QUon.Checked)
            {
                Properties.Settings.Default.QUsett = false;
                Properties.Settings.Default.Save();
            }
            if (DeleteShots.Checked)
            {
                Properties.Settings.Default.delsh = true;
                Properties.Settings.Default.Save();
            }
            if (!DeleteShots.Checked)
            {
                Properties.Settings.Default.delsh = false;
                Properties.Settings.Default.Save();
            }
        }
        private void QuestForm_Load(object sender, EventArgs e)
        {
            DeleteShots.Checked = Properties.Settings.Default.delsh;
            if (Properties.Settings.Default.QUsett)
            {
                ResBox.Text = Properties.Settings.Default.QUres;
                UsrBox.Text = Properties.Settings.Default.QUname;
                FOVy.Text = Properties.Settings.Default.QUy;
                FOVx.Text = Properties.Settings.Default.QUx;
                QURfrRt.Text = Properties.Settings.Default.QUhz;
                QUon.Checked = true;
                if (settingsexist)
                {
                    QUSon = true;
                }
            }
            GlobalUsername.Text = Properties.Settings.Default.GlobalUsername;
        }

        private void ResBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.QUres = ResBox.Text;
            Properties.Settings.Default.Save();

        }

        private void UsrBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.QUname = UsrBox.Text;
            Properties.Settings.Default.Save();
        }

        private void FOVx_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.QUx = FOVx.Text;
            Properties.Settings.Default.Save();
        }

        private void FOVy_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.QUy = FOVy.Text;
            Properties.Settings.Default.Save();
        }

        private void QURfrRt_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selected = QURfrRt.GetItemText(QURfrRt.SelectedItem);
            Properties.Settings.Default.QUhz = selected;
            Properties.Settings.Default.Save();
        }
        private void DeleteButton_Click(object sender, EventArgs e)
        {

            _ = MessageBox.Show("Ok, Deleted your custom settings file.\nIf you would like to re-enable return here and apply settings again");
            File.Delete($"{Properties.Settings.Default.MainDir}\\Config.Json");
        }
        private void questPics_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (!Directory.Exists($"{path}\\Quest ScreenShots"))
            {
                _ = Directory.CreateDirectory($"{path}\\Quest ScreenShots");
            }

            _ = MessageBox.Show("Please wait until you get the message that the transfer has finished.");
            ADB.WakeDevice();
            Program.form.ChangeTitle("Pulling files...");
            _ = ADB.RunAdbCommandToString($"pull \"/sdcard/Oculus/Screenshots\" \"{path}\\Quest ScreenShots\"");
            if (delsh)
            {
                DialogResult dialogResult = MessageBox.Show("You have chosen to delete files from headset after transferring, so be sure to move them from your desktop to somewhere safe!", "Warning!", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    _ = ADB.RunAdbCommandToString("shell rm -r /sdcard/Oculus/Screenshots");
                    _ = ADB.RunAdbCommandToString("shell mkdir /sdcard/Oculus/Screenshots");
                }
            }
            _ = MessageBox.Show("Transfer finished! ScreenShots can be found in a folder named Quest Screenshots on your desktop!");
            Program.form.ChangeTitle("Done!");
        }
        private void questVids_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (!Directory.Exists($"{path}\\Quest VideoShots"))
            {
                _ = Directory.CreateDirectory($"{path}\\Quest VideoShots");
            }

            _ = MessageBox.Show("Please wait until you get the message that the transfer has finished.");
            ADB.WakeDevice();
            Program.form.ChangeTitle("Pulling files...");
            _ = ADB.RunAdbCommandToString($"pull \"/sdcard/Oculus/Videoshots\" \"{path}\\Quest VideoShots\"");
            if (delsh)
            {
                DialogResult dialogResult = MessageBox.Show("You have chosen to delete files from headset after transferring, so be sure to move them from your desktop to somewhere safe!", "Warning!", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    _ = ADB.RunAdbCommandToString("shell rm -r /sdcard/Oculus/Videoshots");
                    _ = ADB.RunAdbCommandToString("shell mkdir /sdcard/Oculus/Videoshots");
                }
            }
            _ = MessageBox.Show("Transfer finished! VideoShots can be found in a folder named Quest VideoShots on your desktop!");
            Program.form.ChangeTitle("Done!");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (GlobalUsername.Text.Contains(" "))
            {
                _ = MessageBox.Show("Usernames with a space are not permitted.", "Detected a space in username!");
            }
            else
            {
                _ = ADB.RunAdbCommandToString($"shell settings put global username {GlobalUsername.Text}");
                _ = MessageBox.Show($"Username set as {GlobalUsername.Text}", "Success");
            }
        }


        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void WifiWake_Click(object sender, EventArgs e)
        {
            _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_available 1");
            _ = ADB.RunAdbCommandToString("shell settings put global wifi_wakeup_enabled 1");
            _ = MessageBox.Show("Wake on Wifi enabled!\n\nNOTE: This requires having wireless ADB enabled to work. (Obviously)");
        }

        private void GlobalUsername_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = GlobalUsername.TextLength > 0;
            Properties.Settings.Default.GlobalUsername = GlobalUsername.Text;
            Properties.Settings.Default.Save();
        }

        private void RefreshRateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CPUComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GPUComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
