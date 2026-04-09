using AndroidSideloader.Utilities;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AndroidSideloader
{
    /// <summary>
    /// Modern startup dialog that lets the user choose between
    /// online mode (by providing a config URL) or offline/local library mode.
    /// </summary>
    public partial class StartupDialog : Form
    {
        // Modern theme colors (matching DonorsListView)
        private static readonly Color BackgroundColor = Color.FromArgb(20, 24, 29);
        private static readonly Color BorderColor = Color.FromArgb(70, 80, 100);
        private static readonly Color AccentColor = Color.FromArgb(82, 203, 173);
        private static readonly Color ErrorColor = Color.FromArgb(245, 47, 87);         // #f52f57
        private static readonly Color ButtonInactive1 = Color.FromArgb(30, 35, 42);
        private static readonly Color ButtonActive1 = Color.FromArgb(45, 52, 62);
        private static readonly Color FieldBorderColor = Color.FromArgb(50, 55, 65);

        // Shadow and corner settings (matching DonorsListView)
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private const int SHADOW_SIZE = 2;
        private const int CONTENT_RADIUS = 10;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        // Result
        public enum StartupChoice { None, Online, Offline, RcloneConfig }
        public StartupChoice Choice { get; private set; } = StartupChoice.None;
        // State
        private bool _hasExistingConfig;
        private bool _hasExistingRcloneConfig;
        private string _configFailReason;

        public StartupDialog(string initialError = null)
        {
            InitializeComponent();

            // Use same icon as the executable
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            ApplyModernTheme();

            // Show version in title
            _titleLabel.Text = $"Rookie Sideloader {Updater.LocalVersion}";

            // Check for existing valid configs before asking for a URL
            _hasExistingConfig = TryValidateExistingConfig();
            _hasExistingRcloneConfig = TryValidateExistingRcloneConfig();

            // Position existing config labels and resize the form to fit exactly
            int TITLE_BOTTOM_Y = 30;    // first usable Y below title/close button
            const int LABEL_ROW_HEIGHT = 16;  // height per label row (13px font + 3px gap)
            const int AFTER_LABEL_GAP = 6;    // gap between last label and first button
            const int DESIGNER_BUTTON_Y = 50; // _btnProvidePublic.Top in designer

            int labelCount = 0;
            int labelY = TITLE_BOTTOM_Y;

            if (_hasExistingConfig)
            {
                _existingConfigLabel.Text = "\u2714 Public config found \u2014 click here to reconnect";
                _existingConfigLabel.ForeColor = AccentColor;
                _existingConfigLabel.Location = new Point(3, labelY);
                _existingConfigLabel.Visible = true;
                _existingConfigLabel.Cursor = Cursors.Hand;
                labelCount++;
                labelY += LABEL_ROW_HEIGHT;
            }

            if (_hasExistingRcloneConfig)
            {
                _existingRcloneLabel.Text = "\u2714 Rclone config found \u2014 click here to reconnect";
                _existingRcloneLabel.ForeColor = AccentColor;
                _existingRcloneLabel.Location = new Point(3, labelY);
                _existingRcloneLabel.Visible = true;
                _existingRcloneLabel.Cursor = Cursors.Hand;
                labelCount++;
            }

            // Keep error label aligned with the label area
            _errorLabel.Location = new Point(2, TITLE_BOTTOM_Y);

            if (!_hasExistingConfig && !_hasExistingRcloneConfig && _configFailReason != null)
            {
                // Config file exists but failed validation — tell the user why
                ShowError(_configFailReason);
                labelCount = 1;
            }

            // If the caller passed an explicit error (e.g. all mirrors exhausted),
            // show it — this overrides the existing-config labels if needed.
            if (!string.IsNullOrEmpty(initialError))
            {
                ShowError(initialError);
                if (labelCount == 0) labelCount = 1;
            }

            // Move the buttons down a little if there are no labels
            if (labelCount == 0)
            {
                TITLE_BOTTOM_Y += 8;
            }

            // Shift all controls below the label area and resize the form to match
            int buttonTopY = TITLE_BOTTOM_Y + labelCount * LABEL_ROW_HEIGHT + (labelCount > 0 ? AFTER_LABEL_GAP : 0);
            int shift = buttonTopY - DESIGNER_BUTTON_Y;
            _btnProvidePublic.Top += shift;
            _btnProvideRclone.Top += shift;
            _separator.Top += shift;
            _orLabel.Top += shift;
            _offlineButton.Top += shift;
            this.Height += shift;

            this.ActiveControl = _titleLabel;
        }

        private void ApplyModernTheme()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(25, 25, 30);
            this.Padding = new Padding(5);

            panel1.BackColor = BackgroundColor;

            // Click on empty area unfocuses the URL field
            panel1.Click += (s, ev) => { this.ActiveControl = _titleLabel; };

            this.Paint += Form_Paint;

            // Close button
            var closeButton = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = BackgroundColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(30, 28),
                Location = new Point(panel1.Width - 35, 5),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 60, 60);
            closeButton.Click += (s, e) => { Choice = StartupChoice.None; this.Close(); };
            panel1.Controls.Add(closeButton);
            closeButton.BringToFront();

            // Provide Public Config button
            _btnProvidePublic.Paint += RoundedButton_Paint;
            _btnProvidePublic.Click += (s, e) => OnProvidePublicClicked();
            _btnProvidePublic.MouseEnter += (s, e) => _btnProvidePublic.BackColor = ButtonActive1;
            _btnProvidePublic.MouseLeave += (s, e) => _btnProvidePublic.BackColor = ButtonInactive1;

            // Provide Rclone Config button
            _btnProvideRclone.Paint += RoundedButton_Paint;
            _btnProvideRclone.Click += (s, e) => OnProvideRcloneClicked();
            _btnProvideRclone.MouseEnter += (s, e) => _btnProvideRclone.BackColor = ButtonActive1;
            _btnProvideRclone.MouseLeave += (s, e) => _btnProvideRclone.BackColor = ButtonInactive1;

            // Existing config label clicks
            _existingConfigLabel.Click += (s, e) =>
            {
                if (!_hasExistingConfig) return;
                Choice = StartupChoice.Online;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            _existingRcloneLabel.Click += (s, e) =>
            {
                if (!_hasExistingRcloneConfig) return;
                Choice = StartupChoice.RcloneConfig;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            // Offline button paint + events
            _offlineButton.Paint += RoundedButton_Paint;
            _offlineButton.Click += (s, e) =>
            {
                Choice = StartupChoice.Offline;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            _offlineButton.MouseEnter += (s, e) => _offlineButton.BackColor = ButtonActive1;
            _offlineButton.MouseLeave += (s, e) => _offlineButton.BackColor = ButtonInactive1;

            // Enable dragging on labels (but not the clickable buttons/links)
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Label && ctrl != _btnProvidePublic && ctrl != _btnProvideRclone
                    && ctrl != _offlineButton
                    && ctrl != _existingConfigLabel && ctrl != _existingRcloneLabel)
                {
                    ctrl.MouseDown += StartupDialog_MouseDown;
                }
            }
        }

        // Drag support
        private void StartupDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
            }
        }

        private void StartupDialog_MouseMove(object sender, MouseEventArgs e) { }
        private void StartupDialog_MouseUp(object sender, MouseEventArgs e) { }

        // Provide Public Config
        private void OnProvidePublicClicked()
        {
            if (ShowProvideConfigDialog(true))
            {
                Choice = StartupChoice.Online;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // Provide Rclone Config
        private void OnProvideRcloneClicked()
        {
            if (ShowProvideConfigDialog(false))
            {
                Choice = StartupChoice.RcloneConfig;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Shows a unified dialog that lets the user provide config content by
        /// pasting it, browsing for a file, or fetching from a URL.
        /// Returns true if the config was successfully validated and saved.
        /// </summary>
        private bool ShowProvideConfigDialog(bool isPublic)
        {
            string title = isPublic ? "Provide Public Config" : "Provide Rclone Config";
            string hint = isPublic
                ? "Paste JSON config, browse for a .json file, or fetch from a URL:"
                : "Paste Rclone config, browse for a file, or fetch from a URL:";
            string browseFilter = isPublic
                ? "JSON files (*.json)|*.json|All files (*.*)|*.*"
                : "Config files (*.config;*.conf)|*.config;*.conf|All files (*.*)|*.*";
            string browseTitle = isPublic
                ? "Select a public config JSON file"
                : "Select an Rclone config file";

            using (Form dialog = new Form())
            {
                dialog.Text = title;
                dialog.Size = new Size(460, 340);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = hint,
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var textBox = new TextBox
                {
                    Location = new Point(15, 38),
                    Size = new Size(415, 185),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    BackColor = Color.FromArgb(28, 32, 38),
                    ForeColor = Color.FromArgb(200, 205, 215),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Consolas", 9F),
                    AcceptsReturn = true
                };

                var errorLbl = new Label
                {
                    ForeColor = ErrorColor,
                    Font = new Font("Segoe UI", 8F),
                    Location = new Point(15, 228),
                    Size = new Size(415, 16),
                    Visible = false
                };

                // Browse button — loads file content into the textbox
                var browseBtn = new Button
                {
                    Text = "FROM FILE",
                    Location = new Point(15, 250),
                    Size = new Size(85, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand
                };
                browseBtn.FlatAppearance.BorderSize = 0;
                browseBtn.Click += (s, ev) =>
                {
                    using (var ofd = new OpenFileDialog())
                    {
                        ofd.Title = browseTitle;
                        ofd.Filter = browseFilter;
                        if (!isPublic)
                        {
                            string rcloneDir = Path.Combine(Environment.CurrentDirectory, "rclone");
                            ofd.InitialDirectory = Directory.Exists(rcloneDir) ? rcloneDir : Environment.CurrentDirectory;
                        }
                        else
                        {
                            ofd.InitialDirectory = Environment.CurrentDirectory;
                        }
                        if (ofd.ShowDialog(dialog) == DialogResult.OK)
                        {
                            try { textBox.Text = File.ReadAllText(ofd.FileName); errorLbl.Visible = false; }
                            catch (Exception ex) { errorLbl.Text = $"Failed to read file: {ex.Message}"; errorLbl.Visible = true; }
                        }
                    }
                };

                // From URL button — fetches content from a URL into the textbox
                var urlBtn = new Button
                {
                    Text = "FROM URL",
                    Location = new Point(108, 250),
                    Size = new Size(95, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand
                };
                urlBtn.FlatAppearance.BorderSize = 0;
                urlBtn.Click += (s, ev) =>
                {
                    using (Form urlDialog = new Form())
                    {
                        urlDialog.Text = "Fetch from URL";
                        urlDialog.Size = new Size(440, 155);
                        urlDialog.StartPosition = FormStartPosition.CenterParent;
                        urlDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                        urlDialog.MaximizeBox = false;
                        urlDialog.MinimizeBox = false;
                        urlDialog.BackColor = Color.FromArgb(20, 24, 29);
                        urlDialog.ForeColor = Color.White;

                        var urlLabel = new Label
                        {
                            Text = "Enter URL:",
                            ForeColor = Color.White,
                            AutoSize = true,
                            Location = new Point(15, 15)
                        };

                        var urlTextBox = new TextBox
                        {
                            Location = new Point(15, 38),
                            Size = new Size(395, 23),
                            BackColor = Color.FromArgb(28, 32, 38),
                            ForeColor = Color.FromArgb(200, 205, 215),
                            BorderStyle = BorderStyle.FixedSingle,
                            Font = new Font("Segoe UI", 9.5F)
                        };

                        if (isPublic)
                        {
                            string savedUrl = SettingsManager.Instance.ConfigUrl;
                            if (!string.IsNullOrWhiteSpace(savedUrl))
                                urlTextBox.Text = savedUrl;
                        }

                        var urlErrorLbl = new Label
                        {
                            ForeColor = ErrorColor,
                            Font = new Font("Segoe UI", 8F),
                            Location = new Point(15, 65),
                            Size = new Size(395, 16),
                            Visible = false
                        };

                        var fetchBtn = new Button
                        {
                            Text = "FETCH",
                            Location = new Point(255, 85),
                            Size = new Size(75, 28),
                            FlatStyle = FlatStyle.Flat,
                            BackColor = AccentColor,
                            ForeColor = Color.Black,
                            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                            Cursor = Cursors.Hand
                        };
                        fetchBtn.FlatAppearance.BorderSize = 0;

                        var urlCancelBtn = new Button
                        {
                            Text = "Cancel",
                            DialogResult = DialogResult.Cancel,
                            Location = new Point(340, 85),
                            Size = new Size(75, 28),
                            FlatStyle = FlatStyle.Flat,
                            BackColor = Color.FromArgb(42, 45, 58),
                            ForeColor = Color.White,
                            Font = new Font("Segoe UI", 9F),
                            Cursor = Cursors.Hand
                        };
                        urlCancelBtn.FlatAppearance.BorderSize = 0;

                        bool busy = false;
                        fetchBtn.Click += async (s2, ev2) =>
                        {
                            string url = urlTextBox.Text.Trim();
                            if (string.IsNullOrWhiteSpace(url) || busy) return;

                            busy = true;
                            urlErrorLbl.Visible = false;
                            fetchBtn.Text = "...";
                            fetchBtn.Cursor = Cursors.WaitCursor;
                            urlTextBox.Enabled = false;

                            try
                            {
                                var result = await System.Threading.Tasks.Task.Run(() => FetchUrlContent(url));
                                if (result.Item1 != null)
                                {
                                    if (isPublic)
                                    {
                                        SettingsManager.Instance.ConfigUrl = url;
                                        SettingsManager.Instance.Save();
                                    }
                                    textBox.Text = result.Item1;
                                    urlDialog.DialogResult = DialogResult.OK;
                                    urlDialog.Close();
                                }
                                else
                                {
                                    urlErrorLbl.Text = result.Item2;
                                    urlErrorLbl.Visible = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                urlErrorLbl.Text = $"Unexpected error: {ex.Message}";
                                urlErrorLbl.Visible = true;
                            }
                            finally
                            {
                                busy = false;
                                fetchBtn.Text = "FETCH";
                                fetchBtn.Cursor = Cursors.Hand;
                                urlTextBox.Enabled = true;
                            }
                        };

                        urlTextBox.KeyDown += (s2, ev2) =>
                        {
                            if (ev2.KeyCode == Keys.Enter)
                            {
                                ev2.SuppressKeyPress = true;
                                fetchBtn.PerformClick();
                            }
                        };

                        urlDialog.Controls.AddRange(new Control[] { urlLabel, urlTextBox, urlErrorLbl, fetchBtn, urlCancelBtn });
                        urlDialog.CancelButton = urlCancelBtn;
                        urlDialog.Shown += (s2, ev2) => urlTextBox.Focus();
                        urlDialog.ShowDialog(dialog);
                    }
                };

                // Apply button — validates content and saves the config file
                var applyBtn = new Button
                {
                    Text = "APPLY",
                    Location = new Point(275, 250),
                    Size = new Size(75, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AccentColor,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                applyBtn.FlatAppearance.BorderSize = 0;

                var cancelBtn = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(360, 250),
                    Size = new Size(75, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand
                };
                cancelBtn.FlatAppearance.BorderSize = 0;

                applyBtn.Click += (s, ev) =>
                {
                    errorLbl.Visible = false;
                    string content = textBox.Text.Trim();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        errorLbl.Text = "Provide config content first";
                        errorLbl.Visible = true;
                        return;
                    }

                    string error = isPublic ? TryLoadJsonContent(content) : TryLoadRcloneContent(content);
                    if (error == null)
                    {
                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                    else
                    {
                        errorLbl.Text = error;
                        errorLbl.Visible = true;
                    }
                };

                dialog.Controls.AddRange(new Control[] { label, textBox, errorLbl, browseBtn, urlBtn, applyBtn, cancelBtn });
                dialog.CancelButton = cancelBtn;
                dialog.Shown += (s, ev) => textBox.Focus();

                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }

        /// <summary>
        /// Downloads text content from a URL.
        /// Returns Tuple(content, null) on success, or Tuple(null, errorMessage) on failure.
        /// </summary>
        private Tuple<string, string> FetchUrlContent(string url)
        {
            try
            {
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://" + url;
                }

                System.Net.ServicePointManager.SecurityProtocol =
                    System.Net.SecurityProtocolType.Tls |
                    System.Net.SecurityProtocolType.Tls11 |
                    System.Net.SecurityProtocolType.Tls12 |
                    System.Net.SecurityProtocolType.Ssl3;

                var request = DnsHelper.CreateWebRequest(url);
                request.Timeout = 15000;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                        return Tuple.Create((string)null, "Server returned an empty response");
                    return Tuple.Create(content, (string)null);
                }
            }
            catch (System.Net.WebException wex)
            {
                Logger.Log($"URL fetch failed: {wex.Message}", LogLevel.ERROR);
                if (wex.Response is System.Net.HttpWebResponse httpResp)
                    return Tuple.Create((string)null, $"Server returned {(int)httpResp.StatusCode} {httpResp.StatusDescription}");
                return Tuple.Create((string)null, "Could not reach the server. Check URL and your connection");
            }
            catch (Exception ex)
            {
                Logger.Log($"URL fetch error: {ex.Message}", LogLevel.ERROR);
                return Tuple.Create((string)null, $"Failed to fetch URL: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates rclone config content and writes it to rclone/download.config.
        /// Returns null on success, or an error message on failure.
        /// </summary>
        private string TryLoadRcloneContent(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                    return "Config content is empty";

                string destPath = Path.Combine(Environment.CurrentDirectory, "rclone", "download.config");
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                File.WriteAllText(destPath, content);
                Logger.Log("Rclone download.config written from user-provided content");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Log($"Rclone config save error: {ex.Message}", LogLevel.ERROR);
                return $"Failed to save config: {ex.Message}";
            }
        }

        // UI helpers
        private void ShowError(string message)
        {
            if (_errorLabel != null)
            {
                _errorLabel.Text = "✖ " + message;
                _errorLabel.Visible = true;
            }
            if (_existingConfigLabel != null)
                _existingConfigLabel.Visible = false;
            if (_existingRcloneLabel != null)
                _existingRcloneLabel.Visible = false;
        }

        // Custom painting
        private void Form_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int w = this.Width;
            int h = this.Height;

            // Draw shadow layers
            for (int i = SHADOW_SIZE; i >= 1; i--)
            {
                int alpha = (SHADOW_SIZE - i + 1) * 12;
                Rectangle shadowRect = new Rectangle(
                    SHADOW_SIZE - i,
                    SHADOW_SIZE - i,
                    w - (SHADOW_SIZE - i) * 2 - 1,
                    h - (SHADOW_SIZE - i) * 2 - 1);

                using (Pen shadowPen = new Pen(Color.FromArgb(alpha, 0, 0, 0), 1))
                using (GraphicsPath shadowPath = CreateRoundedRectPath(shadowRect, CONTENT_RADIUS + i))
                {
                    e.Graphics.DrawPath(shadowPen, shadowPath);
                }
            }

            // Draw content background
            Rectangle contentRect = new Rectangle(SHADOW_SIZE, SHADOW_SIZE, w - SHADOW_SIZE * 2, h - SHADOW_SIZE * 2);
            using (GraphicsPath contentPath = CreateRoundedRectPath(contentRect, CONTENT_RADIUS))
            {
                using (SolidBrush bgBrush = new SolidBrush(BackgroundColor))
                {
                    e.Graphics.FillPath(bgBrush, contentPath);
                }

                using (Pen borderPen = new Pen(BorderColor, 1f))
                {
                    e.Graphics.DrawPath(borderPen, contentPath);
                }
            }

            // Apply rounded region
            using (GraphicsPath regionPath = CreateRoundedRectPath(new Rectangle(0, 0, w, h), CONTENT_RADIUS + SHADOW_SIZE))
            {
                this.Region = new Region(regionPath);
            }
        }

        private void RoundedButton_Paint(object sender, PaintEventArgs e)
        {
            var lbl = sender as Label;
            if (lbl == null) return;

            var rect = new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1);

            using (var brush = new SolidBrush(lbl.BackColor))
                e.Graphics.FillRectangle(brush, rect);

            Color strokeColor = FieldBorderColor;
            using (var pen = new Pen(strokeColor, 1f))
                e.Graphics.DrawRectangle(pen, rect);

            TextRenderer.DrawText(e.Graphics, lbl.Text, lbl.Font, lbl.ClientRectangle,
                lbl.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            radius = diameter / 2;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Existing config validation
        /// <summary>
        /// Checks if public.json exists locally, contains valid config data,
        /// and the server is reachable.  Returns true only when the config is
        /// structurally correct and a quick connectivity test succeeds.
        /// </summary>
        private bool TryValidateExistingConfig()
        {
            _configFailReason = null;

            try
            {
                string configFilePath = Path.Combine(Environment.CurrentDirectory, "public.json");
                if (!File.Exists(configFilePath))
                    return false;

                string json = File.ReadAllText(configFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _configFailReason = "Existing config file is empty";
                    return false;
                }

                Models.PublicConfig config;
                try
                {
                    config = JsonConvert.DeserializeObject<Models.PublicConfig>(json);
                }
                catch
                {
                    _configFailReason = "Existing config file is invalid";
                    return false;
                }

                if (config == null)
                {
                    _configFailReason = "Existing config file is invalid";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.BaseUri) || string.IsNullOrWhiteSpace(config.Password))
                {
                    _configFailReason = "Existing config is missing required fields";
                    return false;
                }

                // Validate that BaseUri is a well-formed URL
                if (!Uri.TryCreate(config.BaseUri, UriKind.Absolute, out Uri baseUri) ||
                    (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
                {
                    Logger.Log($"Existing config has an invalid BaseUri: {config.BaseUri}", LogLevel.WARNING);
                    _configFailReason = "Existing config has an invalid server URL";
                    return false;
                }

                // Quick connectivity test — try to reach the server
                string reachError = TestServerReachability(config.BaseUri);
                if (reachError != null)
                {
                    _configFailReason = $"Config found but {reachError}";
                    return false;
                }

                Logger.Log("Existing public.json is valid and server is reachable");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Existing public.json validation failed: {ex.Message}", LogLevel.WARNING);
                _configFailReason = "Existing config could not be validated";
                return false;
            }
        }

        /// <summary>
        /// Sends a quick HTTP HEAD request to the given URL to verify the server
        /// is reachable.  Returns null on success or an error message on failure.
        /// </summary>
        private static string TestServerReachability(string url)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol =
                    System.Net.SecurityProtocolType.Tls |
                    System.Net.SecurityProtocolType.Tls11 |
                    System.Net.SecurityProtocolType.Tls12 |
                    System.Net.SecurityProtocolType.Ssl3;

                var request = DnsHelper.CreateWebRequest(url);
                request.Method = "HEAD";
                request.Timeout = 5000;
                using (var response = request.GetResponse())
                {
                    // Server responded — reachable
                }
                return null;
            }
            catch (System.Net.WebException wex)
            {
                // A valid HTTP error response (e.g. 403, 404) still means the
                // server is reachable, so the config host is valid.
                if (wex.Response is System.Net.HttpWebResponse)
                {
                    Logger.Log("Config server returned an HTTP error, but is reachable");
                    return null;
                }

                Logger.Log($"Config server unreachable: {wex.Message}", LogLevel.WARNING);
                return "server is unreachable";
            }
            catch (Exception ex)
            {
                Logger.Log($"Config server connectivity test failed: {ex.Message}", LogLevel.WARNING);
                return "server could not be reached";
            }
        }

        /// <summary>
        /// Validates raw JSON content as a valid config and writes it to public.json.
        /// Returns null on success, or an error message on failure.
        /// </summary>
        private string TryLoadJsonContent(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return "JSON content is empty";

                Models.PublicConfig config;
                try
                {
                    config = JsonConvert.DeserializeObject<Models.PublicConfig>(json);
                }
                catch
                {
                    return "Invalid JSON format";
                }

                if (config == null)
                    return "JSON does not contain a valid config";

                if (string.IsNullOrWhiteSpace(config.BaseUri))
                    return "Config is missing the 'baseUri' field";

                if (string.IsNullOrWhiteSpace(config.Password))
                    return "Config is missing the 'password' field";

                // Validate that BaseUri is a well-formed URL
                if (!Uri.TryCreate(config.BaseUri, UriKind.Absolute, out Uri baseUri) ||
                    (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
                    return "Config has an invalid server URL";

                // Note: we intentionally skip a connectivity test here.
                // The user may be providing a new config precisely because the
                // current server is unreachable (e.g. quota exceeded). Actual
                // connectivity is verified when MainForm tries to use the config.

                string configFilePath = Path.Combine(Environment.CurrentDirectory, "public.json");
                File.WriteAllText(configFilePath, json);
                Logger.Log("Config saved from user-provided JSON");

                return null;
            }
            catch (Exception ex)
            {
                Logger.Log($"JSON config load error: {ex.Message}", LogLevel.ERROR);
                return $"Failed to process config: {ex.Message}";
            }
        }

        /// <summary>
        /// Checks if rclone/download.config exists and contains at least one
        /// remote section.
        /// </summary>
        private bool TryValidateExistingRcloneConfig()
        {
            try
            {
                string configPath = Path.Combine(Environment.CurrentDirectory, "rclone", "download.config");
                if (!File.Exists(configPath))
                    return false;

                string text = File.ReadAllText(configPath);
                if (string.IsNullOrWhiteSpace(text))
                    return false;

                return Regex.IsMatch(text, @"^\[.+\]", RegexOptions.Multiline);
            }
            catch
            {
                return false;
            }
        }

            }
        }