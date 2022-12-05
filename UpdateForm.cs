using System;
using System.Drawing;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class UpdateForm : Form
    {
        private bool mouseDown;
        private Point lastLocation;

        public UpdateForm()
        {
            InitializeComponent();
            CenterToScreen();
            CurVerLabel.Text += " " + Updater.LocalVersion;
            UpdateVerLabel.Text += " " + Updater.currentVersion;
            UpdateTextBox.Text = Updater.changelog;
        }

        private void YesUpdate_Click(object sender, EventArgs e)
        {
            Updater.doUpdate();
            Close();
        }

        private void SkipUpdate_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void UpdateForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point(
                    Location.X - lastLocation.X + e.X, Location.Y - lastLocation.Y + e.Y);

                Update();
            }
        }

        private void UpdateForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
