using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JR.Utils.GUI.Forms
{
    public class FlexibleMessageBox
    {
        #region Public statics

        public static double MAX_WIDTH_FACTOR = 0.7;
        public static double MAX_HEIGHT_FACTOR = 0.9;
        public static Font FONT = SystemFonts.MessageBoxFont;

        #endregion

        #region Public show functions

        public static DialogResult Show(string text)
        {
            return FlexibleMessageBoxForm.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            return FlexibleMessageBoxForm.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption)
        {
            return FlexibleMessageBoxForm.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return FlexibleMessageBoxForm.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return FlexibleMessageBoxForm.Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return FlexibleMessageBoxForm.Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return FlexibleMessageBoxForm.Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return FlexibleMessageBoxForm.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return FlexibleMessageBoxForm.Show(null, text, caption, buttons, icon, defaultButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return FlexibleMessageBoxForm.Show(owner, text, caption, buttons, icon, defaultButton);
        }

        #endregion

        #region Internal form class

        private class FlexibleMessageBoxForm : Form
        {
            #region Constants and P/Invoke

            private const int CS_DROPSHADOW = 0x00020000;
            private const int WM_NCLBUTTONDOWN = 0xA1;
            private const int HT_CAPTION = 0x2;
            private const uint FLASHW_TRAY = 0x00000002;
            private const uint FLASHW_TIMERNOFG = 0x0000000C;

            [StructLayout(LayoutKind.Sequential)]
            private struct FLASHWINFO
            {
                public uint cbSize;
                public IntPtr hwnd;
                public uint dwFlags;
                public uint uCount;
                public uint dwTimeout;
            }

            [DllImport("user32.dll")]
            private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ClassStyle |= CS_DROPSHADOW;
                    return cp;
                }
            }

            #endregion

            #region Form-Designer generated code

            private System.ComponentModel.IContainer components = null;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void CloseButton_Click(object sender, EventArgs e)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            private void TitlePanel_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
                }
            }

            private void InitializeComponent()
            {
                components = new System.ComponentModel.Container();
                button1 = new System.Windows.Forms.Button();
                richTextBoxMessage = new System.Windows.Forms.RichTextBox();
                FlexibleMessageBoxFormBindingSource = new System.Windows.Forms.BindingSource(components);
                panel1 = new System.Windows.Forms.Panel();
                pictureBoxForIcon = new System.Windows.Forms.PictureBox();
                button2 = new System.Windows.Forms.Button();
                button3 = new System.Windows.Forms.Button();
                titlePanel = new System.Windows.Forms.Panel();
                titleLabel = new System.Windows.Forms.Label();
                closeButton = new System.Windows.Forms.Button();
                ((System.ComponentModel.ISupportInitialize)FlexibleMessageBoxFormBindingSource).BeginInit();
                panel1.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)pictureBoxForIcon).BeginInit();
                titlePanel.SuspendLayout();
                SuspendLayout();
                //
                // titlePanel
                //
                titlePanel.BackColor = System.Drawing.Color.FromArgb(20, 24, 29);
                titlePanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
                titlePanel.Location = new System.Drawing.Point(6, 6);
                titlePanel.Name = "titlePanel";
                titlePanel.Size = new System.Drawing.Size(248, 28);
                titlePanel.TabIndex = 10;
                titlePanel.Controls.Add(closeButton);
                titlePanel.Controls.Add(titleLabel);
                titlePanel.MouseDown += TitlePanel_MouseDown;
                //
                // titleLabel
                //
                titleLabel.AutoSize = false;
                titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
                titleLabel.ForeColor = System.Drawing.Color.White;
                titleLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                titleLabel.Location = new System.Drawing.Point(0, 0);
                titleLabel.Name = "titleLabel";
                titleLabel.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
                titleLabel.Size = new System.Drawing.Size(218, 28);
                titleLabel.TabIndex = 0;
                titleLabel.Text = "<Caption>";
                titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                titleLabel.MouseDown += TitlePanel_MouseDown;
                //
                // closeButton
                //
                closeButton.Dock = System.Windows.Forms.DockStyle.Right;
                closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                closeButton.FlatAppearance.BorderSize = 0;
                closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(200, 60, 60);
                closeButton.BackColor = System.Drawing.Color.FromArgb(20, 24, 29);
                closeButton.ForeColor = System.Drawing.Color.White;
                closeButton.Font = new System.Drawing.Font("Segoe UI", 9F);
                closeButton.Location = new System.Drawing.Point(218, 0);
                closeButton.Name = "closeButton";
                closeButton.Size = new System.Drawing.Size(30, 28);
                closeButton.TabIndex = 1;
                closeButton.TabStop = false;
                closeButton.Text = "✕";
                closeButton.Click += CloseButton_Click;
                //
                // button1
                //
                button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
                button1.DialogResult = System.Windows.Forms.DialogResult.OK;
                button1.Location = new System.Drawing.Point(26, 80);
                button1.Name = "button1";
                button1.Size = new System.Drawing.Size(75, 28);
                button1.TabIndex = 2;
                button1.Text = "OK";
                button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button1.FlatAppearance.BorderSize = 0;
                button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(60, 65, 80);
                button1.BackColor = System.Drawing.Color.FromArgb(42, 45, 58);
                button1.ForeColor = System.Drawing.Color.White;
                button1.Font = new System.Drawing.Font("Segoe UI", 9F);
                button1.Cursor = System.Windows.Forms.Cursors.Hand;
                button1.Visible = false;
                //
                // richTextBoxMessage
                //
                richTextBoxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
                richTextBoxMessage.BackColor = System.Drawing.Color.FromArgb(20, 24, 29);
                richTextBoxMessage.ForeColor = System.Drawing.Color.White;
                richTextBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
                richTextBoxMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", FlexibleMessageBoxFormBindingSource, "MessageText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                richTextBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
                richTextBoxMessage.Location = new System.Drawing.Point(46, 6);
                richTextBoxMessage.Margin = new System.Windows.Forms.Padding(0);
                richTextBoxMessage.Name = "richTextBoxMessage";
                richTextBoxMessage.ReadOnly = true;
                richTextBoxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
                richTextBoxMessage.Size = new System.Drawing.Size(190, 20);
                richTextBoxMessage.TabIndex = 0;
                richTextBoxMessage.TabStop = false;
                richTextBoxMessage.Text = "<Message>";
                richTextBoxMessage.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(richTextBoxMessage_LinkClicked);
                //
                // panel1
                //
                panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
                panel1.BackColor = System.Drawing.Color.FromArgb(20, 24, 29);
                panel1.Controls.Add(pictureBoxForIcon);
                panel1.Controls.Add(richTextBoxMessage);
                panel1.Location = new System.Drawing.Point(6, 34);
                panel1.Name = "panel1";
                panel1.Size = new System.Drawing.Size(248, 59);
                panel1.TabIndex = 1;
                //
                // pictureBoxForIcon
                //
                pictureBoxForIcon.BackColor = System.Drawing.Color.Transparent;
                pictureBoxForIcon.Location = new System.Drawing.Point(15, 15);
                pictureBoxForIcon.Name = "pictureBoxForIcon";
                pictureBoxForIcon.Size = new System.Drawing.Size(32, 32);
                pictureBoxForIcon.TabIndex = 8;
                pictureBoxForIcon.TabStop = false;
                //
                // button2
                //
                button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
                button2.DialogResult = System.Windows.Forms.DialogResult.OK;
                button2.Location = new System.Drawing.Point(107, 80);
                button2.Name = "button2";
                button2.Size = new System.Drawing.Size(75, 28);
                button2.TabIndex = 3;
                button2.Text = "OK";
                button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button2.FlatAppearance.BorderSize = 0;
                button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(60, 65, 80);
                button2.BackColor = System.Drawing.Color.FromArgb(42, 45, 58);
                button2.ForeColor = System.Drawing.Color.White;
                button2.Font = new System.Drawing.Font("Segoe UI", 9F);
                button2.Cursor = System.Windows.Forms.Cursors.Hand;
                button2.Visible = false;
                //
                // button3
                //
                button3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
                button3.DialogResult = System.Windows.Forms.DialogResult.OK;
                button3.Location = new System.Drawing.Point(188, 80);
                button3.Name = "button3";
                button3.Size = new System.Drawing.Size(75, 28);
                button3.TabIndex = 0;
                button3.Text = "OK";
                button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button3.FlatAppearance.BorderSize = 0;
                button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(60, 65, 80);
                button3.BackColor = System.Drawing.Color.FromArgb(42, 45, 58);
                button3.ForeColor = System.Drawing.Color.White;
                button3.Font = new System.Drawing.Font("Segoe UI", 9F);
                button3.Cursor = System.Windows.Forms.Cursors.Hand;
                button3.Visible = false;
                //
                // FlexibleMessageBoxForm
                //
                AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                BackColor = System.Drawing.Color.FromArgb(25, 25, 30);
                ForeColor = System.Drawing.Color.White;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                ClientSize = new System.Drawing.Size(260, 115);
                Padding = new System.Windows.Forms.Padding(5);
                Controls.Add(titlePanel);
                Controls.Add(button3);
                Controls.Add(button2);
                Controls.Add(button1);
                Controls.Add(panel1);
                MaximizeBox = false;
                MinimizeBox = false;
                MinimumSize = new System.Drawing.Size(276, 120);
                Name = "FlexibleMessageBoxForm";
                ShowIcon = false;
                SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                Text = "<Caption>";
                Shown += new System.EventHandler(FlexibleMessageBoxForm_Shown);
                ((System.ComponentModel.ISupportInitialize)FlexibleMessageBoxFormBindingSource).EndInit();
                panel1.ResumeLayout(false);
                ((System.ComponentModel.ISupportInitialize)pictureBoxForIcon).EndInit();
                titlePanel.ResumeLayout(false);
                ResumeLayout(false);

                // Apply rounded corners and custom painting
                this.Paint += FlexibleMessageBoxForm_Paint;
                button1.Paint += RoundedButton_Paint;
                button2.Paint += RoundedButton_Paint;
                button3.Paint += RoundedButton_Paint;

                // Setup hover effects for buttons
                SetupButtonHover(button1);
                SetupButtonHover(button2);
                SetupButtonHover(button3);

                Activate();
            }

            private Button button1;
            private BindingSource FlexibleMessageBoxFormBindingSource;
            private RichTextBox richTextBoxMessage;
            private Panel panel1;
            private PictureBox pictureBoxForIcon;
            private Button button2;
            private Button button3;
            private Panel titlePanel;
            private Label titleLabel;
            private Button closeButton;

            #endregion

            #region Custom Painting

            private Dictionary<Button, bool> _buttonHoverState = new Dictionary<Button, bool>();
            private const int SHADOW_SIZE = 2;
            private const int CONTENT_RADIUS = 10;

            private void FlexibleMessageBoxForm_Paint(object sender, PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                int w = this.Width;
                int h = this.Height;

                // Draw shadow gradient layers around the content area
                for (int i = SHADOW_SIZE; i >= 1; i--)
                {
                    int alpha = (SHADOW_SIZE - i + 1) * 12; // 12, 24, 36, 48, 60
                    Rectangle shadowRect = new Rectangle(
                        SHADOW_SIZE - i,
                        SHADOW_SIZE - i,
                        w - (SHADOW_SIZE - i) * 2 - 1,
                        h - (SHADOW_SIZE - i) * 2 - 1);

                    using (Pen shadowPen = new Pen(Color.FromArgb(alpha, 0, 0, 0), 1))
                    using (GraphicsPath shadowPath = GetRoundedRectPath(shadowRect, CONTENT_RADIUS + i))
                    {
                        e.Graphics.DrawPath(shadowPen, shadowPath);
                    }
                }

                // Draw content background
                Rectangle contentRect = new Rectangle(SHADOW_SIZE, SHADOW_SIZE, w - SHADOW_SIZE * 2, h - SHADOW_SIZE * 2);
                using (GraphicsPath contentPath = GetRoundedRectPath(contentRect, CONTENT_RADIUS))
                {
                    using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(20, 24, 29)))
                    {
                        e.Graphics.FillPath(bgBrush, contentPath);
                    }

                    // Draw thin border
                    using (Pen borderPen = new Pen(Color.FromArgb(70, 80, 100), 1f))
                    {
                        e.Graphics.DrawPath(borderPen, contentPath);
                    }
                }

                // Apply rounded region to form (with shadow area)
                using (GraphicsPath regionPath = GetRoundedRectPath(new Rectangle(0, 0, w, h), CONTENT_RADIUS + SHADOW_SIZE))
                {
                    this.Region = new Region(regionPath);
                }
            }

            private void SetupButtonHover(Button btn)
            {
                _buttonHoverState[btn] = false;

                btn.MouseEnter += (s, e) =>
                {
                    _buttonHoverState[btn] = true;
                    btn.Invalidate();
                };

                btn.MouseLeave += (s, e) =>
                {
                    _buttonHoverState[btn] = false;
                    btn.Invalidate();
                };
            }

            private void RoundedButton_Paint(object sender, PaintEventArgs e)
            {
                Button btn = sender as Button;
                if (btn == null) return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                bool isHovered = _buttonHoverState.ContainsKey(btn) && _buttonHoverState[btn];

                int radius = 4;

                // Use a rect that's 1 pixel smaller to avoid edge clipping
                Rectangle drawRect = new Rectangle(1, 1, btn.Width - 2, btn.Height - 2);

                // Fill entire button area with parent background first to clear previous state
                using (SolidBrush clearBrush = new SolidBrush(Color.FromArgb(20, 24, 29)))
                {
                    e.Graphics.FillRectangle(clearBrush, 0, 0, btn.Width, btn.Height);
                }

                using (GraphicsPath path = GetRoundedRectPath(drawRect, radius))
                {
                    // Determine colors based on hover state
                    Color bgColor = isHovered
                        ? Color.FromArgb(93, 203, 173)  // Accent color on hover
                        : btn.BackColor;

                    Color textColor = isHovered
                        ? Color.FromArgb(20, 20, 20)    // Dark text on accent
                        : btn.ForeColor;

                    // Draw background
                    using (SolidBrush brush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Draw subtle border on normal state
                    if (!isHovered)
                    {
                        using (Pen borderPen = new Pen(Color.FromArgb(70, 75, 90), 1))
                        {
                            e.Graphics.DrawPath(borderPen, path);
                        }
                    }

                    // Draw text centered in original button bounds
                    TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font,
                        new Rectangle(0, 0, btn.Width, btn.Height), textColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // Set region to full button size (not the draw rect)
                using (GraphicsPath regionPath = GetRoundedRectPath(new Rectangle(0, 0, btn.Width, btn.Height), radius))
                {
                    btn.Region = new Region(regionPath);
                }
            }

            private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();

                if (radius <= 0)
                {
                    path.AddRectangle(rect);
                    return path;
                }

                int diameter = radius * 2;

                // Ensure diameter doesn't exceed rect dimensions
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

            #endregion

            #region Private constants

            private static readonly string STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";
            private static readonly string STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

            private enum ButtonID { OK = 0, CANCEL, YES, NO, ABORT, RETRY, IGNORE };

            private enum TwoLetterISOLanguageID { en, de, es, it };
            private static readonly string[] BUTTON_TEXTS_ENGLISH_EN = { "OK", "Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore" };
            private static readonly string[] BUTTON_TEXTS_GERMAN_DE = { "OK", "Abbrechen", "&Ja", "&Nein", "&Abbrechen", "&Wiederholen", "&Ignorieren" };
            private static readonly string[] BUTTON_TEXTS_SPANISH_ES = { "Aceptar", "Cancelar", "&Sí", "&No", "&Abortar", "&Reintentar", "&Ignorar" };
            private static readonly string[] BUTTON_TEXTS_ITALIAN_IT = { "OK", "Annulla", "&Sì", "&No", "&Interrompi", "&Riprova", "&Ignora" };

            #endregion

            #region Private members

            private MessageBoxDefaultButton defaultButton;
            private int visibleButtonsCount;
            private readonly TwoLetterISOLanguageID languageID = TwoLetterISOLanguageID.en;

            #endregion

            #region Private constructor

            private FlexibleMessageBoxForm()
            {
                InitializeComponent();
                _ = Enum.TryParse<TwoLetterISOLanguageID>(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out languageID);
                KeyPreview = true;
                KeyUp += FlexibleMessageBoxForm_KeyUp;
            }

            #endregion

            #region Private helper functions

            private static string[] GetStringRows(string message)
            {
                if (string.IsNullOrEmpty(message))
                {
                    return null;
                }
                return message.Split(new char[] { '\n' }, StringSplitOptions.None);
            }

            private string GetButtonText(ButtonID buttonID)
            {
                int buttonTextArrayIndex = Convert.ToInt32(buttonID);
                switch (languageID)
                {
                    case TwoLetterISOLanguageID.de: return BUTTON_TEXTS_GERMAN_DE[buttonTextArrayIndex];
                    case TwoLetterISOLanguageID.es: return BUTTON_TEXTS_SPANISH_ES[buttonTextArrayIndex];
                    case TwoLetterISOLanguageID.it: return BUTTON_TEXTS_ITALIAN_IT[buttonTextArrayIndex];
                    default: return BUTTON_TEXTS_ENGLISH_EN[buttonTextArrayIndex];
                }
            }

            private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor)
            {
                const double MIN_FACTOR = 0.2;
                const double MAX_FACTOR = 1.0;
                return workingAreaFactor < MIN_FACTOR ? MIN_FACTOR : workingAreaFactor > MAX_FACTOR ? MAX_FACTOR : workingAreaFactor;
            }

            private static void SetDialogStartPosition(FlexibleMessageBoxForm flexibleMessageBoxForm, IWin32Window owner)
            {
                flexibleMessageBoxForm.StartPosition = FormStartPosition.Manual;

                // Try to get owner form, fallback to active form if owner is null
                Form ownerForm = null;
                if (owner != null)
                {
                    ownerForm = owner as Form;
                    if (ownerForm == null)
                    {
                        Control ownerControl = Control.FromHandle(owner.Handle);
                        ownerForm = ownerControl?.FindForm();
                    }
                }

                // Fallback to active form if no owner specified
                if (ownerForm == null)
                {
                    ownerForm = Form.ActiveForm;
                }

                if (ownerForm != null && ownerForm.Visible && ownerForm.WindowState != FormWindowState.Minimized)
                {
                    // Center relative to owner window
                    int x = ownerForm.Left + (ownerForm.Width - flexibleMessageBoxForm.Width) / 2;
                    int y = ownerForm.Top + (ownerForm.Height - flexibleMessageBoxForm.Height) / 2;

                    // Ensure the dialog stays within screen bounds
                    Screen screen = Screen.FromControl(ownerForm);
                    x = Math.Max(screen.WorkingArea.Left, Math.Min(x, screen.WorkingArea.Right - flexibleMessageBoxForm.Width));
                    y = Math.Max(screen.WorkingArea.Top, Math.Min(y, screen.WorkingArea.Bottom - flexibleMessageBoxForm.Height));

                    flexibleMessageBoxForm.Left = x;
                    flexibleMessageBoxForm.Top = y;
                }
                else
                {
                    // No owner found or minimized: center on current screen
                    CenterOnScreen(flexibleMessageBoxForm);
                }
            }

            private static void CenterOnScreen(FlexibleMessageBoxForm form)
            {
                Screen screen = Screen.FromPoint(Cursor.Position);
                form.Left = screen.WorkingArea.Left + (screen.WorkingArea.Width - form.Width) / 2;
                form.Top = screen.WorkingArea.Top + (screen.WorkingArea.Height - form.Height) / 2;
            }

            private static void SetDialogSizes(FlexibleMessageBoxForm flexibleMessageBoxForm, string text, string caption)
            {
                flexibleMessageBoxForm.MaximumSize = new Size(Convert.ToInt32(SystemInformation.WorkingArea.Width * FlexibleMessageBoxForm.GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
                                                              Convert.ToInt32(SystemInformation.WorkingArea.Height * FlexibleMessageBoxForm.GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));

                string[] stringRows = GetStringRows(text);
                if (stringRows == null) return;

                int textHeight = TextRenderer.MeasureText(text, FONT).Height;
                const int SCROLLBAR_WIDTH_OFFSET = 15;
                int longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, FONT).Width);
                int captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
                int textWidth = Math.Max(longestTextRowWidth + SCROLLBAR_WIDTH_OFFSET, captionWidth);

                int marginWidth = flexibleMessageBoxForm.Width - flexibleMessageBoxForm.richTextBoxMessage.Width;
                int marginHeight = flexibleMessageBoxForm.Height - flexibleMessageBoxForm.richTextBoxMessage.Height;

                flexibleMessageBoxForm.Size = new Size(textWidth + marginWidth, textHeight + marginHeight);
            }

            private static void SetDialogIcon(FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxIcon icon)
            {
                switch (icon)
                {
                    case MessageBoxIcon.Information:
                        flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Error:
                        flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
                        break;
                    case MessageBoxIcon.Question:
                        flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
                        break;
                    default:
                        flexibleMessageBoxForm.pictureBoxForIcon.Visible = false;
                        flexibleMessageBoxForm.richTextBoxMessage.Left -= flexibleMessageBoxForm.pictureBoxForIcon.Width;
                        flexibleMessageBoxForm.richTextBoxMessage.Width += flexibleMessageBoxForm.pictureBoxForIcon.Width;
                        break;
                }
            }

            private static void SetDialogButtons(FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
            {
                switch (buttons)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                        flexibleMessageBoxForm.visibleButtonsCount = 3;
                        flexibleMessageBoxForm.button1.Visible = true;
                        flexibleMessageBoxForm.button1.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.ABORT);
                        flexibleMessageBoxForm.button1.DialogResult = DialogResult.Abort;
                        flexibleMessageBoxForm.button2.Visible = true;
                        flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.RETRY);
                        flexibleMessageBoxForm.button2.DialogResult = DialogResult.Retry;
                        flexibleMessageBoxForm.button3.Visible = true;
                        flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.IGNORE);
                        flexibleMessageBoxForm.button3.DialogResult = DialogResult.Ignore;
                        flexibleMessageBoxForm.ControlBox = false;
                        break;

                    case MessageBoxButtons.OKCancel:
                        flexibleMessageBoxForm.visibleButtonsCount = 2;
                        flexibleMessageBoxForm.button2.Visible = true;
                        flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.OK);
                        flexibleMessageBoxForm.button2.DialogResult = DialogResult.OK;
                        flexibleMessageBoxForm.button3.Visible = true;
                        flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.CANCEL);
                        flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;
                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
                        break;

                    case MessageBoxButtons.RetryCancel:
                        flexibleMessageBoxForm.visibleButtonsCount = 2;
                        flexibleMessageBoxForm.button2.Visible = true;
                        flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.RETRY);
                        flexibleMessageBoxForm.button2.DialogResult = DialogResult.Retry;
                        flexibleMessageBoxForm.button3.Visible = true;
                        flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.CANCEL);
                        flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;
                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
                        break;

                    case MessageBoxButtons.YesNo:
                        flexibleMessageBoxForm.visibleButtonsCount = 2;
                        flexibleMessageBoxForm.button2.Visible = true;
                        flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.YES);
                        flexibleMessageBoxForm.button2.DialogResult = DialogResult.Yes;
                        flexibleMessageBoxForm.button3.Visible = true;
                        flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.NO);
                        flexibleMessageBoxForm.button3.DialogResult = DialogResult.No;
                        flexibleMessageBoxForm.ControlBox = false;
                        break;

                    case MessageBoxButtons.YesNoCancel:
                        flexibleMessageBoxForm.visibleButtonsCount = 3;
                        flexibleMessageBoxForm.button1.Visible = true;
                        flexibleMessageBoxForm.button1.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.YES);
                        flexibleMessageBoxForm.button1.DialogResult = DialogResult.Yes;
                        flexibleMessageBoxForm.button2.Visible = true;
                        flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.NO);
                        flexibleMessageBoxForm.button2.DialogResult = DialogResult.No;
                        flexibleMessageBoxForm.button3.Visible = true;
                        flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.CANCEL);
                        flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;
                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
                        break;

                    case MessageBoxButtons.OK:
                    default:
                        flexibleMessageBoxForm.visibleButtonsCount = 1;
                        flexibleMessageBoxForm.button3.Visible = true;
                        flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.OK);
                        flexibleMessageBoxForm.button3.DialogResult = DialogResult.OK;
                        flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
                        break;
                }
                flexibleMessageBoxForm.defaultButton = defaultButton;
            }

            #endregion

            #region Private event handlers

            private void FlexibleMessageBoxForm_Shown(object sender, EventArgs e)
            {
                int buttonIndexToFocus;
                switch (defaultButton)
                {
                    case MessageBoxDefaultButton.Button1:
                    default:
                        buttonIndexToFocus = 1;
                        break;
                    case MessageBoxDefaultButton.Button2:
                        buttonIndexToFocus = 2;
                        break;
                    case MessageBoxDefaultButton.Button3:
                        buttonIndexToFocus = 3;
                        break;
                }

                if (buttonIndexToFocus > visibleButtonsCount)
                    buttonIndexToFocus = visibleButtonsCount;

                Button buttonToFocus = buttonIndexToFocus == 3 ? button3 : buttonIndexToFocus == 2 ? button2 : button1;
                _ = buttonToFocus.Focus();
            }

            private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _ = Process.Start(e.LinkText);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }

            private void FlexibleMessageBoxForm_KeyUp(object sender, KeyEventArgs e)
            {
                if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
                {
                    string buttonsTextLine = (button1.Visible ? button1.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                        + (button2.Visible ? button2.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                        + (button3.Visible ? button3.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty);

                    string textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                         + Text + Environment.NewLine
                                         + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                         + richTextBoxMessage.Text + Environment.NewLine
                                         + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                         + buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
                                         + STANDARD_MESSAGEBOX_SEPARATOR_LINES;

                    Clipboard.SetText(textForClipboard);
                }
            }

            #endregion

            #region Properties

            public string CaptionText { get; set; }
            public string MessageText { get; set; }

            #endregion

            #region Public show function

            public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
            {
                FlexibleMessageBoxForm flexibleMessageBoxForm = new FlexibleMessageBoxForm
                {
                    // Show in the taskbar and keep the prompt above other windows so it can
                    // never hide behind another app (e.g. a browser/IDE) and leave the disabled
                    // main window looking "frozen" while it silently waits for an answer.
                    ShowInTaskbar = true,
                    TopMost = true,
                    CaptionText = caption,
                    MessageText = text
                };
                flexibleMessageBoxForm.Shown += (s, e) =>
                {
                    try
                    {
                        flexibleMessageBoxForm.BringToFront();
                        flexibleMessageBoxForm.Activate();
                    }
                    catch { }
                };
                flexibleMessageBoxForm.FlexibleMessageBoxFormBindingSource.DataSource = flexibleMessageBoxForm;
                flexibleMessageBoxForm.titleLabel.Text = caption;

                SetDialogButtons(flexibleMessageBoxForm, buttons, defaultButton);
                SetDialogIcon(flexibleMessageBoxForm, icon);

                flexibleMessageBoxForm.Font = FONT;
                flexibleMessageBoxForm.richTextBoxMessage.Font = FONT;

                SetDialogSizes(flexibleMessageBoxForm, text, caption);

                // Force panel resize to fix closebutton position
                int contentWidth = flexibleMessageBoxForm.ClientSize.Width - 16; // 8px padding
                flexibleMessageBoxForm.titlePanel.Width = contentWidth;

                // Get owner form
                Form ownerForm = owner as Form ?? Form.ActiveForm;
                bool ownerWasMinimized = ownerForm != null && ownerForm.WindowState == FormWindowState.Minimized;

                SetDialogStartPosition(flexibleMessageBoxForm, owner);

                // If owner was minimized, reposition dialog when owner is restored
                if (ownerWasMinimized && ownerForm != null)
                {
                    EventHandler resizeHandler = null;
                    resizeHandler = (s, e) =>
                    {
                        if (ownerForm.WindowState != FormWindowState.Minimized && flexibleMessageBoxForm.Visible && !flexibleMessageBoxForm.IsDisposed)
                        {
                            SetDialogStartPosition(flexibleMessageBoxForm, owner);
                            ownerForm.Resize -= resizeHandler;
                        }
                    };
                    ownerForm.Resize += resizeHandler;
                }

                // Flash taskbar if application is inactive or minimized
                if (Form.ActiveForm == null || ownerWasMinimized)
                {
                    Form targetForm = ownerForm ?? Application.OpenForms.Cast<Form>().FirstOrDefault(f => f != null && !f.IsDisposed && f != flexibleMessageBoxForm && f.Visible);
                    if (targetForm != null && !targetForm.IsDisposed)
                    {
                        FLASHWINFO info = new FLASHWINFO
                        {
                            cbSize = (uint)Marshal.SizeOf(typeof(FLASHWINFO)),
                            hwnd = targetForm.Handle,
                            dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG,
                            uCount = 5,
                            dwTimeout = 0
                        };
                        _ = FlashWindowEx(ref info);
                    }
                }

                return flexibleMessageBoxForm.ShowDialog(owner);
            }

            #endregion
        }

        #endregion
    }
}