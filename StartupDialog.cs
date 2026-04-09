using AndroidSideloader.Utilities;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
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
        public enum StartupChoice { None, Online, Offline }
        public StartupChoice Choice { get; private set; } = StartupChoice.None;
        // State
        private bool _hasExistingConfig;
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

            // Check for an existing valid download.config before asking for a URL
            _hasExistingConfig = TryValidateExistingConfig();

            if (_hasExistingConfig)
            {
                _existingConfigLabel.Text = "\u2714 Valid config found \u2014 click HERE to reconnect";
                _existingConfigLabel.ForeColor = AccentColor;
                _existingConfigLabel.Visible = true;
                _existingConfigLabel.Cursor = Cursors.Hand;
            }
            else if (_configFailReason != null)
            {
                // Config file exists but failed validation — tell the user why
                ShowError(_configFailReason);
            }

            // If the caller passed an explicit error (e.g. all mirrors exhausted),
            // show it — this overrides the existing-config label if needed.
            if (!string.IsNullOrEmpty(initialError))
                ShowError(initialError);

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

            // Enter URL button
            _btnEnterUrl.Paint += RoundedButton_Paint;
            _btnEnterUrl.Click += (s, e) => OnEnterUrlClicked();
            _btnEnterUrl.MouseEnter += (s, e) => _btnEnterUrl.BackColor = ButtonActive1;
            _btnEnterUrl.MouseLeave += (s, e) => _btnEnterUrl.BackColor = ButtonInactive1;

            // Browse File button
            _btnBrowseFile.Paint += RoundedButton_Paint;
            _btnBrowseFile.Click += (s, e) => OnBrowseJsonClicked();
            _btnBrowseFile.MouseEnter += (s, e) => _btnBrowseFile.BackColor = ButtonActive1;
            _btnBrowseFile.MouseLeave += (s, e) => _btnBrowseFile.BackColor = ButtonInactive1;

            // Paste Config button
            _btnPasteCode.Paint += RoundedButton_Paint;
            _btnPasteCode.Click += (s, e) => OnPasteJsonClicked();
            _btnPasteCode.MouseEnter += (s, e) => _btnPasteCode.BackColor = ButtonActive1;
            _btnPasteCode.MouseLeave += (s, e) => _btnPasteCode.BackColor = ButtonInactive1;

            // Existing config label click
            _existingConfigLabel.Click += (s, e) => OnExistingConfigClicked();

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
                if (ctrl is Label && ctrl != _btnEnterUrl && ctrl != _btnBrowseFile
                    && ctrl != _btnPasteCode && ctrl != _offlineButton
                    && ctrl != _existingConfigLabel)
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

        // Enter URL logic
        private void OnEnterUrlClicked()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Enter Config URL";
                dialog.Size = new Size(440, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = "Enter URL to a config file:",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var textBox = new TextBox
                {
                    Location = new Point(15, 38),
                    Size = new Size(395, 23),
                    BackColor = Color.FromArgb(28, 32, 38),
                    ForeColor = Color.FromArgb(200, 205, 215),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 9.5F)
                };

                string savedUrl = SettingsManager.Instance.ConfigUrl;
                if (!string.IsNullOrWhiteSpace(savedUrl))
                    textBox.Text = savedUrl;

                var errorLbl = new Label
                {
                    ForeColor = ErrorColor,
                    Font = new Font("Segoe UI", 8F),
                    Location = new Point(15, 65),
                    Size = new Size(395, 16),
                    Visible = false
                };

                var connectBtn = new Button
                {
                    Text = "CONNECT",
                    Location = new Point(245, 105),
                    Size = new Size(85, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AccentColor,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                connectBtn.FlatAppearance.BorderSize = 0;

                var cancelBtn = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(340, 105),
                    Size = new Size(75, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand
                };
                cancelBtn.FlatAppearance.BorderSize = 0;

                bool busy = false;
                connectBtn.Click += async (s, ev) =>
                {
                    string url = textBox.Text.Trim();
                    if (string.IsNullOrWhiteSpace(url) || busy) return;

                    busy = true;
                    errorLbl.Visible = false;
                    connectBtn.Text = "...";
                    connectBtn.Cursor = Cursors.WaitCursor;
                    textBox.Enabled = false;

                    try
                    {
                        string err = await System.Threading.Tasks.Task.Run(() => TryFetchConfig(url));
                        if (err == null)
                        {
                            SettingsManager.Instance.ConfigUrl = url;
                            SettingsManager.Instance.Save();
                            dialog.DialogResult = DialogResult.OK;
                            dialog.Close();
                        }
                        else
                        {
                            errorLbl.Text = err;
                            errorLbl.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLbl.Text = $"Unexpected error: {ex.Message}";
                        errorLbl.Visible = true;
                    }
                    finally
                    {
                        busy = false;
                        connectBtn.Text = "CONNECT";
                        connectBtn.Cursor = Cursors.Hand;
                        textBox.Enabled = true;
                    }
                };

                dialog.Controls.AddRange(new Control[] { label, textBox, errorLbl, connectBtn, cancelBtn });
                dialog.CancelButton = cancelBtn;

                textBox.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter)
                    {
                        ev.SuppressKeyPress = true;
                        connectBtn.PerformClick();
                    }
                };

                dialog.Shown += (s, ev) => textBox.Focus();

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
            }

            Choice = StartupChoice.Online;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Use existing config
        private void OnExistingConfigClicked()
        {
            if (!_hasExistingConfig) return;
            Choice = StartupChoice.Online;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Attempts to download config JSON from the given URL, parse it,
        /// and write it to public.json.  Returns null on success or
        /// an error message string on failure.
        /// </summary>
        private string TryFetchConfig(string url)
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

                string json;
                try
                {
                    var request = DnsHelper.CreateWebRequest(url);
                    request.Timeout = 15000;
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                catch (System.Net.WebException wex)
                {
                    Logger.Log($"Config fetch failed: {wex.Message}", LogLevel.ERROR);
                    if (wex.Response is System.Net.HttpWebResponse httpResp)
                        return $"Server returned {(int)httpResp.StatusCode} {httpResp.StatusDescription}";
                    return $"Could not reach the server. Check URL and your connection";
                }

                if (string.IsNullOrWhiteSpace(json))
                    return "Server returned an empty response";

                Models.PublicConfig config;
                try
                {
                    config = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PublicConfig>(json);
                }
                catch
                {
                    return "Response does not include a valid config";
                }

                if (config == null)
                    return "Response does not include a valid config";

                if (string.IsNullOrWhiteSpace(config.BaseUri))
                    return "Config is missing the 'baseUri' field";

                if (string.IsNullOrWhiteSpace(config.Password))
                    return "Config is missing the 'password' field";

                string configFilePath = System.IO.Path.Combine(
                    Environment.CurrentDirectory, "public.json");
                System.IO.File.WriteAllText(configFilePath, json);
                Logger.Log($"Config saved from: {url}");

                return null;
            }
            catch (Exception ex)
            {
                Logger.Log($"Config fetch error: {ex.Message}", LogLevel.ERROR);
                return $"Failed to retrieve config: {ex.Message}";
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
        }

        private void ClearError()
        {
            if (_errorLabel != null)
            {
                _errorLabel.Text = "";
                _errorLabel.Visible = false;
            }
            if (_existingConfigLabel != null && _hasExistingConfig)
                _existingConfigLabel.Visible = true;
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

        // Browse JSON file
        private void OnBrowseJsonClicked()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select a config JSON file";
                dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                dialog.InitialDirectory = Environment.CurrentDirectory;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    string json = File.ReadAllText(dialog.FileName);
                    string error = TryLoadJsonContent(json);

                    if (error == null)
                    {
                        Choice = StartupChoice.Online;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        ShowError(error);
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Failed to read file: {ex.Message}");
                }
            }
        }

        // Paste JSON code
        private void OnPasteJsonClicked()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Paste Config JSON";
                dialog.Size = new Size(440, 280);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(20, 24, 29);
                dialog.ForeColor = Color.White;

                var label = new Label
                {
                    Text = "Paste JSON config content below:",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                var textBox = new TextBox
                {
                    Location = new Point(15, 38),
                    Size = new Size(395, 155),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    BackColor = Color.FromArgb(28, 32, 38),
                    ForeColor = Color.FromArgb(200, 205, 215),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Consolas", 9F),
                    AcceptsReturn = true
                };

                var okButton = new Button
                {
                    Text = "Apply",
                    DialogResult = DialogResult.OK,
                    Location = new Point(255, 205),
                    Size = new Size(75, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AccentColor,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand
                };
                okButton.FlatAppearance.BorderSize = 0;

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(340, 205),
                    Size = new Size(75, 28),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(42, 45, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand
                };
                cancelButton.FlatAppearance.BorderSize = 0;

                dialog.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                dialog.Shown += (s, ev) => textBox.Focus();

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                string json = textBox.Text.Trim();
                string error = TryLoadJsonContent(json);

                if (error == null)
                {
                    Choice = StartupChoice.Online;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError(error);
                }
            }
        }
    }
}