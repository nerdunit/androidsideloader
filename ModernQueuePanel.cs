using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AndroidSideloader
{
    // Modern download queue panel with drag-reorder, cancel buttons
    // and custom scrollbar with auto-scrolling during drag
    public sealed class ModernQueuePanel : Control
    {
        // Layout constants
        private const int ItemHeight = 28, ItemMargin = 4, ItemRadius = 5;
        private const int XButtonSize = 18, PauseButtonSize = 18, DragHandleWidth = 20, TextPadding = 6;
        private const int ScrollbarWidth = 6, ScrollbarWidthHover = 8, ScrollbarMargin = 2;
        private const int ScrollbarRadius = 3, MinThumbHeight = 20;
        private const int AutoScrollZoneHeight = 30, AutoScrollSpeed = 3;

        // Color palette
        private static readonly Color BgColor = Color.FromArgb(24, 26, 30);
        private static readonly Color ItemBg = Color.FromArgb(32, 36, 44);
        private static readonly Color ItemHoverBg = Color.FromArgb(42, 46, 54);
        private static readonly Color ItemDragBg = Color.FromArgb(45, 55, 70);
        private static readonly Color TextColor = Color.FromArgb(210, 210, 210);
        private static readonly Color TextDimColor = Color.FromArgb(140, 140, 140);
        private static readonly Color AccentColor = Color.FromArgb(93, 203, 173);
        private static readonly Color XButtonBg = Color.FromArgb(55, 60, 70);
        private static readonly Color XButtonHoverBg = Color.FromArgb(200, 60, 60);
        private static readonly Color PauseButtonBg = Color.FromArgb(55, 60, 70);
        private static readonly Color PauseButtonHoverBg = Color.FromArgb(70, 140, 130);
        private static readonly Color PausedAccentColor = Color.FromArgb(220, 180, 60);
        private static readonly Color GripColor = Color.FromArgb(70, 75, 85);
        private static readonly Color ItemDragBorder = Color.FromArgb(55, 65, 80);
        private static readonly Color ScrollTrackColor = Color.FromArgb(35, 38, 45);
        private static readonly Color ScrollThumbColor = Color.FromArgb(70, 75, 85);
        private static readonly Color ScrollThumbHoverColor = Color.FromArgb(90, 95, 105);
        private static readonly Color ScrollThumbDragColor = Color.FromArgb(110, 115, 125);

        private readonly List<string> _items = new List<string>();
        private readonly Timer _autoScrollTimer;

        // State tracking
        private int _hoveredIndex = -1, _dragIndex = -1, _dropIndex = -1, _scrollOffset;
        private bool _hoveringX, _hoveringPause, _scrollbarHovered, _scrollbarDragging;
        private int _scrollDragStartY, _scrollDragStartOffset, _autoScrollDirection;
        private Rectangle _scrollThumbRect, _scrollTrackRect;

        public event EventHandler<int> ItemRemoved;
        public event EventHandler<ReorderEventArgs> ItemReordered;
        public event EventHandler<bool> ItemPauseToggled;

        public ModernQueuePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = BgColor;
            _autoScrollTimer = new Timer { Interval = 16 }; // ~60 FPS
            _autoScrollTimer.Tick += (s, e) => HandleAutoScroll();
        }

        public bool IsDownloading { get; set; }
        public bool IsPaused { get; set; }
        public int Count => _items.Count;
        private int ContentHeight => _items.Count * (ItemHeight + ItemMargin) + ItemMargin;
        private int MaxScroll => Math.Max(0, ContentHeight - Height);
        private bool ScrollbarVisible => ContentHeight > Height;

        public void SetItems(IEnumerable<string> items)
        {
            _items.Clear();
            _items.AddRange(items);
            ResetState();
        }

        private void ResetState()
        {
            _hoveredIndex = _dragIndex = -1;
            ClampScroll();
            Invalidate();
        }

        private void ClampScroll() =>
            _scrollOffset = Math.Max(0, Math.Min(MaxScroll, _scrollOffset));

        // Auto-scroll when dragging near edges
        private void HandleAutoScroll()
        {
            if (_dragIndex < 0 || _autoScrollDirection == 0)
            {
                _autoScrollTimer.Stop();
                return;
            }

            int oldOffset = _scrollOffset;
            _scrollOffset += _autoScrollDirection * AutoScrollSpeed;
            ClampScroll();

            if (_scrollOffset != oldOffset)
            {
                UpdateDropIndex(PointToClient(MousePosition).Y);
                Invalidate();
            }
        }

        private void UpdateAutoScroll(int mouseY)
        {
            if (_dragIndex < 0 || MaxScroll <= 0)
            {
                StopAutoScroll();
                return;
            }

            _autoScrollDirection = mouseY < AutoScrollZoneHeight && _scrollOffset > 0 ? -1 :
                                   mouseY > Height - AutoScrollZoneHeight && _scrollOffset < MaxScroll ? 1 : 0;

            if (_autoScrollDirection != 0 && !_autoScrollTimer.Enabled)
                _autoScrollTimer.Start();
            else if (_autoScrollDirection == 0)
                _autoScrollTimer.Stop();
        }

        private void StopAutoScroll()
        {
            _autoScrollDirection = 0;
            _autoScrollTimer.Stop();
        }

        private void UpdateDropIndex(int mouseY) =>
            _dropIndex = Math.Max(1, Math.Min(_items.Count, (mouseY + _scrollOffset + ItemHeight / 2) / (ItemHeight + ItemMargin)));

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BgColor);

            if (_items.Count == 0)
            {
                DrawEmptyState(g);
                return;
            }

            // Draw visible items
            for (int i = 0; i < _items.Count; i++)
            {
                var rect = GetItemRect(i);
                if (rect.Bottom >= 0 && rect.Top <= Height)
                    DrawItem(g, i, rect);
            }

            // Draw drop indicator and scrollbar
            if (_dragIndex >= 0 && _dropIndex >= 0 && _dropIndex != _dragIndex)
                DrawDropIndicator(g);
            if (ScrollbarVisible)
                DrawScrollbar(g);
        }

        private void DrawEmptyState(Graphics g)
        {
            using (var brush = new SolidBrush(TextDimColor))
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Italic))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Queue is empty", font, brush, ClientRectangle, sf);
            }
        }

        private void DrawItem(Graphics g, int index, Rectangle rect)
        {
            bool isFirst = index == 0;
            bool isDragging = index == _dragIndex;
            bool isHovered = !isDragging && index == _hoveredIndex;
            Color bg = isDragging ? ItemDragBg : isHovered ? ItemHoverBg : ItemBg;

            // Draw item background
            using (var path = CreateRoundedRect(rect, ItemRadius))
            using (var brush = new SolidBrush(bg))
                g.FillPath(brush, path);

            // Active download (first item) gets gradient accent and border
            if (isFirst)
                DrawFirstItemAccent(g, rect);
            // Dragged items get subtle highlight border
            else if (isDragging)
                DrawBorder(g, rect, ItemDragBorder, 0.5f);

            // Draw drag handle, text, and close button
            if (!isFirst)
                DrawDragHandle(g, rect);
            DrawItemText(g, index, rect, isFirst);
            if (isFirst && IsDownloading)
                DrawPauseButton(g, rect, isHovered && _hoveringPause);
            DrawXButton(g, rect, isHovered && _hoveringX);
        }

        // Draw gradient accent and border for active download (first item)
        private void DrawFirstItemAccent(Graphics g, Rectangle rect)
        {
            Color accent = IsPaused ? PausedAccentColor : AccentColor;
            using (var path = CreateRoundedRect(rect, ItemRadius))
            using (var gradBrush = new LinearGradientBrush(rect,
                Color.FromArgb(60, accent), Color.FromArgb(0, accent), LinearGradientMode.Horizontal))
            {
                var oldClip = g.Clip;
                g.SetClip(path);
                g.FillRectangle(gradBrush, rect);
                g.Clip = oldClip;
            }
            DrawBorder(g, rect, accent, 1.5f);
        }

        private void DrawBorder(Graphics g, Rectangle rect, Color color, float width)
        {
            using (var path = CreateRoundedRect(rect, ItemRadius))
            using (var pen = new Pen(color, width))
                g.DrawPath(pen, path);
        }

        private void DrawDragHandle(Graphics g, Rectangle rect)
        {
            int cx = rect.X + 8, cy = rect.Y + rect.Height / 2;
            using (var brush = new SolidBrush(GripColor))
            {
                for (int row = -1; row <= 1; row++)
                    for (int col = 0; col < 2; col++)
                        g.FillEllipse(brush, cx + col * 4, cy + row * 4 - 1, 2, 2);
            }
        }

        private void DrawItemText(Graphics g, int index, Rectangle rect, bool isFirst)
        {
            int textLeft = isFirst ? rect.X + TextPadding : rect.X + DragHandleWidth;
            int rightPad = ScrollbarVisible ? ScrollbarWidthHover + ScrollbarMargin * 2 : 0;
            int rightButtons = XButtonSize + 6 + (isFirst && IsDownloading ? PauseButtonSize + 4 : 0);
            var textRect = new Rectangle(textLeft, rect.Y, rect.Right - rightButtons - textLeft - rightPad, rect.Height);

            using (var brush = new SolidBrush(TextColor))
            using (var font = new Font("Segoe UI", isFirst ? 8.5f : 8f, isFirst ? FontStyle.Bold : FontStyle.Regular))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                g.DrawString(_items[index], font, brush, textRect, sf);
            }
        }

        private void DrawXButton(Graphics g, Rectangle itemRect, bool hovered)
        {
            var xRect = GetXButtonRect(itemRect);
            using (var path = CreateRoundedRect(xRect, 3))
            using (var brush = new SolidBrush(hovered ? XButtonHoverBg : XButtonBg))
                g.FillPath(brush, path);

            using (var pen = new Pen(Color.White, 1.4f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                int p = 4;
                g.DrawLine(pen, xRect.X + p, xRect.Y + p, xRect.Right - p, xRect.Bottom - p);
                g.DrawLine(pen, xRect.Right - p, xRect.Y + p, xRect.X + p, xRect.Bottom - p);
            }
        }

        private void DrawPauseButton(Graphics g, Rectangle itemRect, bool hovered)
        {
            var pRect = GetPauseButtonRect(itemRect);
            using (var path = CreateRoundedRect(pRect, 3))
            using (var brush = new SolidBrush(hovered ? PauseButtonHoverBg : PauseButtonBg))
                g.FillPath(brush, path);

            if (IsPaused)
            {
                // Draw play triangle (▶)
                int pad = 5;
                var pts = new Point[]
                {
                    new Point(pRect.X + pad + 1, pRect.Y + pad),
                    new Point(pRect.Right - pad, pRect.Y + pRect.Height / 2),
                    new Point(pRect.X + pad + 1, pRect.Bottom - pad)
                };
                using (var brush = new SolidBrush(Color.White))
                    g.FillPolygon(brush, pts);
            }
            else
            {
                // Draw pause bars (⏸)
                int pad = 5;
                int barWidth = 2;
                int gap = 3;
                int cx = pRect.X + pRect.Width / 2;
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(brush, cx - gap / 2 - barWidth, pRect.Y + pad, barWidth, pRect.Height - pad * 2);
                    g.FillRectangle(brush, cx + gap / 2, pRect.Y + pad, barWidth, pRect.Height - pad * 2);
                }
            }
        }

        private void DrawDropIndicator(Graphics g)
        {
            int y = (_dropIndex >= _items.Count ? _items.Count : _dropIndex) * (ItemHeight + ItemMargin) + ItemMargin / 2 - _scrollOffset;
            int left = ItemMargin + 2;
            int right = Width - ItemMargin - 2 - (ScrollbarVisible ? ScrollbarWidthHover + ScrollbarMargin : 0);

            using (var pen = new Pen(AccentColor, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawLine(pen, left, y, right, y);
        }

        // Draw custom scrollbar with hover expansion
        private void DrawScrollbar(Graphics g)
        {
            if (MaxScroll <= 0) return;

            bool expanded = _scrollbarHovered || _scrollbarDragging;
            int sbWidth = expanded ? ScrollbarWidthHover : ScrollbarWidth;
            int trackX = Width - ScrollbarWidth - ScrollbarMargin - (expanded ? (ScrollbarWidthHover - ScrollbarWidth) / 2 : 0);

            _scrollTrackRect = new Rectangle(trackX, ScrollbarMargin, sbWidth, Height - ScrollbarMargin * 2);

            using (var trackBrush = new SolidBrush(Color.FromArgb(40, ScrollTrackColor)))
            using (var trackPath = CreateRoundedRect(_scrollTrackRect, ScrollbarRadius))
                g.FillPath(trackBrush, trackPath);

            // Calculate thumb position and size
            int trackHeight = _scrollTrackRect.Height;
            int thumbHeight = Math.Max(MinThumbHeight, (int)(trackHeight * ((float)Height / ContentHeight)));
            float scrollRatio = MaxScroll > 0 ? (float)_scrollOffset / MaxScroll : 0;
            int thumbY = ScrollbarMargin + (int)((trackHeight - thumbHeight) * scrollRatio);

            _scrollThumbRect = new Rectangle(trackX, thumbY, sbWidth, thumbHeight);
            Color thumbColor = _scrollbarDragging ? ScrollThumbDragColor : _scrollbarHovered ? ScrollThumbHoverColor : ScrollThumbColor;

            using (var thumbBrush = new SolidBrush(thumbColor))
            using (var thumbPath = CreateRoundedRect(_scrollThumbRect, ScrollbarRadius))
                g.FillPath(thumbBrush, thumbPath);
        }

        private Rectangle GetItemRect(int index)
        {
            int y = index * (ItemHeight + ItemMargin) + ItemMargin - _scrollOffset;
            int w = Width - ItemMargin * 2 - (ScrollbarVisible ? ScrollbarWidthHover + ScrollbarMargin + 2 : 0);
            return new Rectangle(ItemMargin, y, w, ItemHeight);
        }

        private Rectangle GetXButtonRect(Rectangle itemRect) =>
            new Rectangle(itemRect.Right - XButtonSize - 3, itemRect.Y + (itemRect.Height - XButtonSize) / 2, XButtonSize, XButtonSize);

        private Rectangle GetPauseButtonRect(Rectangle itemRect) =>
            new Rectangle(itemRect.Right - XButtonSize - 3 - PauseButtonSize - 4, itemRect.Y + (itemRect.Height - PauseButtonSize) / 2, PauseButtonSize, PauseButtonSize);

        private int HitTest(Point pt)
        {
            for (int i = 0; i < _items.Count; i++)
                if (GetItemRect(i).Contains(pt)) return i;
            return -1;
        }

        private bool HitTestScrollbar(Point pt) =>
            ScrollbarVisible && new Rectangle(_scrollTrackRect.X - 4, _scrollTrackRect.Y, _scrollTrackRect.Width + 8, _scrollTrackRect.Height).Contains(pt);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_scrollbarDragging)
            {
                HandleScrollbarDrag(e.Y);
                return;
            }

            // Update scrollbar hover state
            bool wasHovered = _scrollbarHovered;
            _scrollbarHovered = HitTestScrollbar(e.Location);
            if (_scrollbarHovered != wasHovered) Invalidate();

            if (_scrollbarHovered)
            {
                Cursor = Cursors.Default;
                _hoveredIndex = -1;
                _hoveringX = false;
                return;
            }

            // Handle drag operation
            if (_dragIndex >= 0)
            {
                UpdateAutoScroll(e.Y);
                int newDrop = Math.Max(1, Math.Min(_items.Count, (e.Y + _scrollOffset + ItemHeight / 2) / (ItemHeight + ItemMargin)));
                if (newDrop != _dropIndex) { _dropIndex = newDrop; Invalidate(); }
                return;
            }

            // Update hover state
            int hit = HitTest(e.Location);
            bool overX = hit >= 0 && GetXButtonRect(GetItemRect(hit)).Contains(e.Location);
            bool overPause = hit == 0 && IsDownloading && GetPauseButtonRect(GetItemRect(hit)).Contains(e.Location);

            if (hit != _hoveredIndex || overX != _hoveringX || overPause != _hoveringPause)
            {
                _hoveredIndex = hit;
                _hoveringX = overX;
                _hoveringPause = overPause;
                Cursor = (overX || overPause) ? Cursors.Hand : hit > 0 ? Cursors.SizeNS : Cursors.Default;
                Invalidate();
            }
        }

        private void HandleScrollbarDrag(int mouseY)
        {
            int trackHeight = Height - ScrollbarMargin * 2;
            int thumbHeight = Math.Max(MinThumbHeight, (int)(trackHeight * ((float)Height / ContentHeight)));
            int scrollableHeight = trackHeight - thumbHeight;

            if (scrollableHeight > 0)
            {
                float scrollRatio = (float)(mouseY - _scrollDragStartY) / scrollableHeight;
                _scrollOffset = _scrollDragStartOffset + (int)(scrollRatio * MaxScroll);
                ClampScroll();
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            // Handle scrollbar thumb drag
            if (ScrollbarVisible && _scrollThumbRect.Contains(e.Location))
            {
                _scrollbarDragging = true;
                _scrollDragStartY = e.Y;
                _scrollDragStartOffset = _scrollOffset;
                Capture = true;
                return;
            }

            // Handle scrollbar track click
            if (ScrollbarVisible && HitTestScrollbar(e.Location))
            {
                _scrollOffset += e.Y < _scrollThumbRect.Top ? -Height : Height;
                ClampScroll();
                Invalidate();
                return;
            }

            int hit = HitTest(e.Location);
            if (hit < 0) return;

            // Handle pause button click (first item only)
            if (hit == 0 && IsDownloading && GetPauseButtonRect(GetItemRect(hit)).Contains(e.Location))
            {
                IsPaused = !IsPaused;
                ItemPauseToggled?.Invoke(this, IsPaused);
                Invalidate();
                return;
            }

            // Handle close button click
            if (GetXButtonRect(GetItemRect(hit)).Contains(e.Location))
            {
                ItemRemoved?.Invoke(this, hit);
                return;
            }

            // Start drag operation (only for non-first items)
            if (hit > 0)
            {
                _dragIndex = _dropIndex = hit;
                Capture = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            StopAutoScroll();

            if (_scrollbarDragging)
            {
                _scrollbarDragging = false;
                Capture = false;
                Invalidate();
                return;
            }

            // Complete drag reorder operation
            if (_dragIndex > 0 && _dropIndex > 0 && _dropIndex != _dragIndex)
            {
                int from = _dragIndex;
                int to = Math.Max(1, _dropIndex > _dragIndex ? _dropIndex - 1 : _dropIndex);
                var item = _items[from];
                _items.RemoveAt(from);
                _items.Insert(to, item);
                ItemReordered?.Invoke(this, new ReorderEventArgs(from, to));
            }

            _dragIndex = _dropIndex = -1;
            Capture = false;
            Cursor = Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_dragIndex < 0 && !_scrollbarDragging)
            {
                _hoveredIndex = -1;
                _hoveringX = _hoveringPause = _scrollbarHovered = false;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (MaxScroll <= 0) return;
            _scrollOffset -= e.Delta / 4;
            ClampScroll();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ClampScroll();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoScrollTimer?.Stop();
                _autoScrollTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class ReorderEventArgs : EventArgs
    {
        public int FromIndex { get; }
        public int ToIndex { get; }
        public ReorderEventArgs(int from, int to) { FromIndex = from; ToIndex = to; }
    }
}