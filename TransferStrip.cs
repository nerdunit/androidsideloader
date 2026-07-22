using System;
using System.Drawing;
using System.Windows.Forms;

namespace AndroidSideloader
{
    // Integrated, collapsible transfer strip. It overlays the bottom of the games list
    // whenever files are moving (SFTP / push), themed to match the main window, and hides
    // itself when idle. Finished rows auto-clear a couple of seconds after they complete.
    // This is a child of the main form, not a separate window.
    public class TransferStrip : Panel
    {
        private readonly ListView _list;
        private readonly Timer _timer;
        private Control _anchor;                 // control whose bottom edge we sit on (games list)
        private const int ExpandedHeight = 150;

        private readonly Color _bg;
        private readonly Color _bgAlt;
        private readonly Color _header;
        private readonly Color _barBack;
        private readonly Color _fg;
        private readonly Color _accent = Color.FromArgb(70, 130, 220);
        private readonly Color _doneColor = Color.FromArgb(70, 165, 95);
        private readonly Color _failColor = Color.FromArgb(200, 80, 80);

        private sealed class BufferedListView : ListView
        {
            public BufferedListView() { DoubleBuffered = true; }
        }

        public TransferStrip(Color themeBack, Color themeFore)
        {
            _bg = themeBack;
            _bgAlt = Shift(themeBack, 8);
            _header = Shift(themeBack, 18);
            _barBack = Shift(themeBack, 28);
            _fg = themeFore;

            DoubleBuffered = true;
            BackColor = _header;
            Visible = false;
            Padding = new Padding(1, 0, 1, 1);

            Label title = new Label
            {
                Text = "  TRANSFERS",
                Dock = DockStyle.Top,
                Height = 22,
                BackColor = _header,
                ForeColor = _fg,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold)
            };

            _list = new BufferedListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                OwnerDraw = true,
                BackColor = _bg,
                ForeColor = _fg,
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            _list.Columns.Add("File", 300);
            _list.Columns.Add("Size", 80, HorizontalAlignment.Right);
            _list.Columns.Add("Progress", 150);
            _list.Columns.Add("Speed", 80, HorizontalAlignment.Right);
            _list.Columns.Add("Status", 70, HorizontalAlignment.Left);
            _list.DrawColumnHeader += OnDrawHeader;
            _list.DrawItem += (s, e) => { /* handled per-subitem */ };
            _list.DrawSubItem += OnDrawSubItem;

            Controls.Add(_list);
            Controls.Add(title);

            // Right-click menu: retry a failed file, remove a row, or clear finished rows.
            ContextMenuStrip menu = new ContextMenuStrip { BackColor = _header, ForeColor = _fg };
            ToolStripMenuItem retryMi = new ToolStripMenuItem("Retry");
            ToolStripMenuItem removeMi = new ToolStripMenuItem("Remove");
            ToolStripMenuItem clearMi = new ToolStripMenuItem("Clear finished");
            retryMi.Click += (s, e) => { TransferItem t = _menuTarget; if (t != null && t.Retry != null) t.Retry(); };
            removeMi.Click += (s, e) => { if (_menuTarget != null) TransferQueue.Remove(_menuTarget); };
            clearMi.Click += (s, e) => TransferQueue.ClearFinished();
            menu.Items.Add(retryMi);
            menu.Items.Add(removeMi);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(clearMi);
            menu.Opening += (s, e) =>
            {
                retryMi.Enabled = _menuTarget != null && _menuTarget.State == TransferState.Failed && _menuTarget.Retry != null;
                removeMi.Enabled = _menuTarget != null;
            };
            _list.ContextMenuStrip = menu;
            _list.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    ListViewHitTestInfo hit = _list.HitTest(e.Location);
                    _menuTarget = hit.Item != null ? hit.Item.Tag as TransferItem : null;
                }
            };

            _timer = new Timer { Interval = 250 };
            _timer.Tick += (s, e) => Tick();
        }

        private TransferItem _menuTarget;

        // Attaches the strip to a parent form and anchors it to a control's bottom edge.
        public void AttachTo(Control parent, Control anchor)
        {
            _anchor = anchor;
            if (parent != null && !parent.IsDisposed)
            {
                parent.Controls.Add(this);
                BringToFront();
            }
            _timer.Start();
        }

        private void Tick()
        {
            try
            {
                TransferQueue.PurgeCompleted(2.0);
                var items = TransferQueue.Snapshot();

                if (items.Count == 0)
                {
                    if (Visible) Visible = false;
                    return;
                }

                if (_anchor != null && !_anchor.IsDisposed)
                {
                    int h = Math.Min(ExpandedHeight, Math.Max(64, _anchor.Height - 30));
                    SetBounds(_anchor.Left, _anchor.Bottom - h, _anchor.Width, h);
                }

                if (!Visible) Visible = true;
                BringToFront();
                RefreshRows(items);
            }
            catch { }
        }

        private void RefreshRows(System.Collections.Generic.List<TransferItem> items)
        {
            _list.BeginUpdate();
            while (_list.Items.Count > items.Count)
            {
                _list.Items.RemoveAt(_list.Items.Count - 1);
            }
            for (int i = 0; i < items.Count; i++)
            {
                TransferItem t = items[i];
                ListViewItem row;
                if (i < _list.Items.Count)
                {
                    row = _list.Items[i];
                }
                else
                {
                    row = new ListViewItem();
                    row.SubItems.Add("");
                    row.SubItems.Add("");
                    row.SubItems.Add("");
                    row.SubItems.Add("");
                    _list.Items.Add(row);
                }

                row.Tag = t;
                row.SubItems[0].Text = t.Name;
                row.SubItems[1].Text = FormatSize(t.TotalBytes);
                double pct = t.TotalBytes > 0 ? (t.TransferredBytes * 100.0 / t.TotalBytes) : 0;
                row.SubItems[2].Text = pct.ToString("0.0") + "%";
                row.SubItems[3].Text = t.State == TransferState.Active ? t.SpeedMBps.ToString("0.0") + " MB/s" : "";
                row.SubItems[4].Text = StatusText(t.State);
            }
            _list.EndUpdate();
        }

        private static string StatusText(TransferState s)
        {
            switch (s)
            {
                case TransferState.Queued: return "Queued";
                case TransferState.Active: return "Active";
                case TransferState.Done: return "Done";
                case TransferState.Failed: return "Failed";
                default: return "";
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes <= 0) return "";
            double mb = bytes / 1048576.0;
            return mb >= 1024 ? (mb / 1024.0).ToString("0.00") + " GB" : mb.ToString("0.0") + " MB";
        }

        private static Color Shift(Color c, int amt)
        {
            return Color.FromArgb(
                Math.Min(255, Math.Max(0, c.R + amt)),
                Math.Min(255, Math.Max(0, c.G + amt)),
                Math.Min(255, Math.Max(0, c.B + amt)));
        }

        private void OnDrawHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(_header))
            {
                e.Graphics.FillRectangle(b, e.Bounds);
            }
            TextRenderer.DrawText(e.Graphics, e.Header.Text, Font, e.Bounds, Color.Gainsboro,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
            using (Pen p = new Pen(Shift(_bg, 40)))
            {
                e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }

        private void OnDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            TransferItem t = e.Item.Tag as TransferItem;
            Color rowBg = (e.ItemIndex % 2 == 0) ? _bg : _bgAlt;
            using (SolidBrush b = new SolidBrush(rowBg))
            {
                e.Graphics.FillRectangle(b, e.Bounds);
            }

            if (e.ColumnIndex == 2 && t != null)
            {
                Rectangle r = e.Bounds;
                r.Inflate(-4, -5);
                using (SolidBrush bb = new SolidBrush(_barBack))
                {
                    e.Graphics.FillRectangle(bb, r);
                }
                double frac = t.TotalBytes > 0 ? Math.Max(0, Math.Min(1, (double)t.TransferredBytes / t.TotalBytes)) : 0;
                Rectangle fill = r;
                fill.Width = (int)(r.Width * frac);
                Color barColor = t.State == TransferState.Failed ? _failColor
                               : t.State == TransferState.Done ? _doneColor
                               : _accent;
                using (SolidBrush fb = new SolidBrush(barColor))
                {
                    e.Graphics.FillRectangle(fb, fill);
                }
                TextRenderer.DrawText(e.Graphics, (frac * 100).ToString("0.0") + "%", Font, e.Bounds, _fg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            else
            {
                TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
                if (e.Header != null && e.Header.TextAlign == HorizontalAlignment.Right)
                {
                    flags |= TextFormatFlags.Right;
                }
                Color txt = _fg;
                if (e.ColumnIndex == 4 && t != null)
                {
                    txt = t.State == TransferState.Done ? Color.FromArgb(120, 205, 145)
                        : t.State == TransferState.Failed ? Color.FromArgb(230, 120, 120)
                        : t.State == TransferState.Active ? Color.FromArgb(120, 180, 240)
                        : Color.Gainsboro;
                }
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, Font, e.Bounds, txt, flags);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _timer?.Stop(); _timer?.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }
    }
}
