using JR.Utils.GUI.Forms;
using Spoofer;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class SpoofForm : Form
    {
        public SpoofForm()
        {
            InitializeComponent();
        }

        private async void SpoofButton_Click(object sender, EventArgs e)
        {
            if (!spoofer.HasDependencies())
            {
                _ = MessageBox.Show("You are missing the dependencies... Cannot spoof games");
                return;
            }
            string NewPackageName = PackageNameTextBox.Text;
            string path;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Android apps (*.apk)|*.apk";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            progressBar1.Style = ProgressBarStyle.Marquee;

            string output = "";
            //Spawn spoofer in a new thread so the ui isn't blocked
            Thread t1 = new Thread(() =>
            {
                spoofer.Init();
                output += spoofer.SpoofApk(path, NewPackageName);
            })
            {
                IsBackground = true
            };
            t1.Start();

            while (t1.IsAlive)
            {
                await Task.Delay(100);
            }

            progressBar1.Style = ProgressBarStyle.Continuous;

            _ = output.Contains("is not recognized as an internal or external command")
                ? FlexibleMessageBox.Show(Sideloader.SpooferWarning)
                : FlexibleMessageBox.Show($"App spoofed from {spoofer.originalPackageName} to {NewPackageName}");
        }

        private void SpoofForm_Load(object sender, EventArgs e)
        {
            PackageNameTextBox.Text = Utilities.GeneralUtilities.RandomPackageName();
        }

        private void RandomizeButton_Click(object sender, EventArgs e)
        {
            PackageNameTextBox.Text = Utilities.GeneralUtilities.RandomPackageName();
        }
    }
}
