using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public partial class UpdateForm : Form
    {
        private bool mouseDown;
        private Point lastLocation;

        // Modern theme colors
        private static readonly Color BackgroundColor = Color.FromArgb(20, 24, 29);
        private static readonly Color PanelColor = Color.FromArgb(28, 32, 38);
        private static readonly Color TextColor = Color.White;
        private static readonly Color SecondaryTextColor = Color.FromArgb(160, 165, 175);
        private static readonly Color BorderColor = Color.FromArgb(60, 65, 75);

        public UpdateForm()
        {
            InitializeComponent();

            // Use same icon as the executable
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            ApplyModernTheme();
            CenterToScreen();
            CurVerLabel.Text = $"Current Version: {Updater.LocalVersion}";
            UpdateVerLabel.Text = $"Update Version: {Updater.currentVersion}";
            UpdateTextBox.Text = Updater.changelog;
        }

        private void ApplyModernTheme()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = BackgroundColor;
            this.DoubleBuffered = true;

            // Enable double buffering on panels for smooth rounded corners
            EnableDoubleBuffering(panel1);
            EnableDoubleBuffering(panel3);

            // Add custom paint handler for rounded panel1 (main container)
            panel1.Paint += Panel1_Paint;
            panel1.BackColor = Color.Transparent;

            // Add custom paint handler for rounded panel3 (changelog container)
            panel3.Paint += Panel3_Paint;
            panel3.BackColor = Color.Transparent;

            // Update textbox to have matching background
            UpdateTextBox.BackColor = PanelColor;

            // Add title label
            var titleLabel = new Label
            {
                Text = "Update",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = TextColor,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            panel1.Controls.Add(titleLabel);
            titleLabel.BringToFront();

            // Add close button
            var closeButton = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 10F),
                ForeColor = SecondaryTextColor,
                BackColor = Color.Transparent,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(this.ClientSize.Width - 30, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.Click += (s, e) => Close();
            closeButton.MouseEnter += (s, e) => closeButton.ForeColor = Color.FromArgb(220, 80, 80);
            closeButton.MouseLeave += (s, e) => closeButton.ForeColor = SecondaryTextColor;
            panel1.Controls.Add(closeButton);
            closeButton.BringToFront();

            // Apply custom painting for form rounded corners and border
            this.Paint += UpdateForm_Paint;
        }

        private void EnableDoubleBuffering(Panel panel)
        {
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panel, new object[] { true });
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int radius = 12;
            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);

            using (var path = CreateRoundedRectPath(rect, radius))
            {
                // Fill background
                using (var brush = new SolidBrush(BackgroundColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Draw border
                using (var pen = new Pen(BorderColor, 1f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // Apply rounded region to clip children
            using (var regionPath = CreateRoundedRectPath(new Rectangle(0, 0, panel.Width, panel.Height), radius))
            {
                panel.Region = new Region(regionPath);
            }
        }

        private void Panel3_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int radius = 10;
            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);

            using (var path = CreateRoundedRectPath(rect, radius))
            {
                // Fill background
                using (var brush = new SolidBrush(PanelColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Draw border
                using (var pen = new Pen(BorderColor, 1f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // Apply rounded region to clip children
            using (var regionPath = CreateRoundedRectPath(new Rectangle(0, 0, panel.Width, panel.Height), radius))
            {
                panel.Region = new Region(regionPath);
            }
        }

        private void UpdateForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int radius = 12;

            // Draw border
            using (var borderPen = new Pen(BorderColor, 1f))
            using (var path = CreateRoundedRectPath(new Rectangle(0, 0, w - 1, h - 1), radius))
            {
                e.Graphics.DrawPath(borderPen, path);
            }

            // Apply rounded region
            using (var regionPath = CreateRoundedRectPath(new Rectangle(0, 0, w, h), radius))
            {
                this.Region = new Region(regionPath);
            }
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            diameter = Math.Min(diameter, Math.Min(rect.Width, rect.Height));
            radius = diameter / 2;

            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arcRect, 180, 90);
            // Top right arc
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            // Bottom right arc
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            // Bottom left arc
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
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