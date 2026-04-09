using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AndroidSideloader
{
    [Description("Rounded Button")]
    public class RoundButton : Control, IButtonControl
    {
        #region Variables
        private int radius;
        private MouseState state;
        private RoundedRectangleF roundedRect;
        private Color inactive1, inactive2, active1, active2;
        private Color strokeColor;
        private bool stroke;
        private int textXOffset;

        public bool Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                Invalidate();
            }
        }

        public Color StrokeColor
        {
            get => strokeColor;
            set
            {
                strokeColor = value;
                Invalidate();
            }
        }

        private Color disabled1, disabled2;
        private Color disabledStrokeColor;

        public Color Disabled1
        {
            get => disabled1;
            set
            {
                disabled1 = value;
                Invalidate();
            }
        }

        public Color Disabled2
        {
            get => disabled2;
            set
            {
                disabled2 = value;
                Invalidate();
            }
        }

        public Color DisabledStrokeColor
        {
            get => disabledStrokeColor;
            set
            {
                disabledStrokeColor = value;
                Invalidate();
            }
        }
        #endregion
        #region RoundButton
        public RoundButton()
        {
            Width = 65;
            Height = 30;
            stroke = false;
            strokeColor = Color.Gray;
            inactive1 = Color.FromArgb(44, 188, 210);
            inactive2 = Color.FromArgb(33, 167, 188);
            active1 = Color.FromArgb(64, 168, 183);
            active2 = Color.FromArgb(36, 164, 183);
            disabled1 = Color.FromArgb(32, 35, 45);
            disabled2 = Color.FromArgb(25, 28, 35);
            disabledStrokeColor = Color.FromArgb(50, 55, 65);

            radius = 10;
            roundedRect = new RoundedRectangleF(Width, Height, radius);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            Font = new System.Drawing.Font("Comic Sans MS", 10, FontStyle.Bold);
            state = MouseState.Leave;
            Transparency = false;
        }
        #endregion
        #region Events
        protected override void OnPaint(PaintEventArgs e)
        {
            #region Transparency
            if (Transparency)
            {
                Transparenter.MakeTransparent(this, e.Graphics);
            }
            #endregion

            #region Drawing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            roundedRect = new RoundedRectangleF(Width, Height, radius);
            e.Graphics.FillRectangle(Brushes.Transparent, ClientRectangle);

            int R1 = (active1.R + inactive1.R) / 2;
            int G1 = (active1.G + inactive1.G) / 2;
            int B1 = (active1.B + inactive1.B) / 2;

            int R2 = (active2.R + inactive2.R) / 2;
            int G2 = (active2.G + inactive2.G) / 2;
            int B2 = (active2.B + inactive2.B) / 2;

            Rectangle rect = new Rectangle(0, 0, Width, Height);

            if (Enabled)
            {
                if (state == MouseState.Leave)
                {
                    using (LinearGradientBrush inactiveGB = new LinearGradientBrush(rect, inactive1, inactive2, 90f))
                    {
                        e.Graphics.FillPath(inactiveGB, roundedRect.Path);
                    }
                }
                else if (state == MouseState.Enter)
                {
                    using (LinearGradientBrush activeGB = new LinearGradientBrush(rect, active1, active2, 90f))
                    {
                        e.Graphics.FillPath(activeGB, roundedRect.Path);
                    }
                }
                else if (state == MouseState.Down)
                {
                    using (LinearGradientBrush downGB = new LinearGradientBrush(rect, Color.FromArgb(R1, G1, B1), Color.FromArgb(R2, G2, B2), 90f))
                    {
                        e.Graphics.FillPath(downGB, roundedRect.Path);
                    }
                }

                if (stroke)
                {
                    using (Pen pen = new Pen(strokeColor, 1))
                    using (GraphicsPath path = new RoundedRectangleF(Width - (radius > 0 ? 0 : 1), Height - (radius > 0 ? 0 : 1), radius).Path)
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush disabledGB = new LinearGradientBrush(rect, disabled1, disabled2, 90f))
                {
                    e.Graphics.FillPath(disabledGB, roundedRect.Path);
                }

                if (stroke)
                {
                    using (Pen pen = new Pen(disabledStrokeColor, 1))
                    using (GraphicsPath path = new RoundedRectangleF(Width - (radius > 0 ? 0 : 1), Height - (radius > 0 ? 0 : 1), radius).Path)
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }


            #endregion

            #region Text Drawing
            using (StringFormat sf = new StringFormat()
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center,
            })
            using (Brush brush = new SolidBrush(ForeColor))
            {
                Rectangle textRect = new Rectangle(textXOffset, 0, Width - textXOffset, Height);
                e.Graphics.DrawString(Text, Font, brush, textRect, sf);
            }
            #endregion
            base.OnPaint(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            base.OnClick(e);
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            state = MouseState.Enter;
            base.OnMouseEnter(e);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            state = MouseState.Leave;
            base.OnMouseLeave(e);
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Capture = false;
            state = MouseState.Down;
            base.OnMouseDown(e);
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (state != MouseState.Leave)
            {
                state = MouseState.Enter;
            }

            base.OnMouseUp(e);
            Invalidate();
        }
        #endregion
        #region Properties


        public int Radius
        {
            get => radius;
            set
            {
                radius = value;
                Invalidate();
            }
        }
        public Color Inactive1
        {
            get => inactive1;
            set
            {
                inactive1 = value;
                Invalidate();
            }
        }
        public Color Inactive2
        {
            get => inactive2;
            set
            {
                inactive2 = value;
                Invalidate();
            }
        }
        public Color Active1
        {
            get => active1;
            set
            {
                active1 = value;
                Invalidate();
            }
        }
        public Color Active2
        {
            get => active2;
            set
            {
                active2 = value;
                Invalidate();
            }
        }
        public bool Transparency { get; set; }
        public int TextXOffset
        {
            get => textXOffset;
            set
            {
                textXOffset = value;
                Invalidate();
            }
        }
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Invalidate();
            }
        }
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                Invalidate();
            }
        }

        public DialogResult DialogResult
        {
            get => System.Windows.Forms.DialogResult.OK;
            set
            {
            }
        }

        public void NotifyDefault(bool value)
        {
        }

        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }
        #endregion
    }
    public enum MouseState
    {
        Enter,
        Leave,
        Down,
        Up,
    }

}