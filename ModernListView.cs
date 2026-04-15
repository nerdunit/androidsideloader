using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public class ModernListView : NativeWindow
    {
        private const int WM_PAINT = 0x000F;
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SETCURSOR = 0x0020;
        private const int WM_SETFONT = 0x0030;
        private const int WM_PRINTCLIENT = 0x0318;

        private const int LVM_GETHEADER = 0x101F;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1036;
        private const int LVS_EX_DOUBLEBUFFER = 0x00010000;

        private const int HDM_HITTEST = 0x1206;
        private const int HDM_LAYOUT = 0x1205;
        private const int HHT_ONDIVIDER = 0x00000004;
        private const int HHT_ONDIVOPEN = 0x00000008;

        private const int CellPaddingX = 10;
        private const int CellPaddingTotalX = CellPaddingX * 2;
        private const int CUSTOM_HEADER_HEIGHT = 32;

        private static readonly Color HeaderBg = Color.FromArgb(28, 32, 38);
        private static readonly Color HeaderBorder = Color.FromArgb(50, 55, 65);
        private static readonly Color HeaderText = Color.FromArgb(140, 145, 150);
        private static readonly Color HeaderTextSorted = Color.FromArgb(93, 203, 173);

        private static readonly Color RowAlt = Color.FromArgb(30, 32, 38);
        private static readonly Color RowNormal = Color.FromArgb(24, 26, 30);
        private static readonly Color RowHover = Color.FromArgb(42, 48, 58);

        private static readonly Color RowDownloadedNormal = Color.FromArgb(27, 43, 42);
        private static readonly Color RowDownloadedAlt = Color.FromArgb(32, 49, 48);
        private static readonly Color RowDownloadedHover = Color.FromArgb(45, 67, 65);

        private static readonly Color RowSelectedActive = Color.FromArgb(50, 65, 85);
        private static readonly Color RowSelectedActiveBorder = Color.FromArgb(93, 203, 173);

        private static readonly Color RowSelectedInactive = Color.FromArgb(64, 66, 80);
        private static readonly Color RowSelectedInactiveBorder = Color.FromArgb(180, 185, 190);

        private static readonly Font HeaderFont = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font ItemFont = new Font("Segoe UI", 9.75f, FontStyle.Regular);
        private static readonly Font ItemFontBold = new Font("Segoe UI", 9.75f, FontStyle.Bold);

        private static readonly SolidBrush HeaderBgBrush = new SolidBrush(HeaderBg);
        private static readonly SolidBrush RowAltBrush = new SolidBrush(RowAlt);
        private static readonly SolidBrush RowNormalBrush = new SolidBrush(RowNormal);
        private static readonly SolidBrush RowHoverBrush = new SolidBrush(RowHover);
        private static readonly SolidBrush RowDownloadedNormalBrush = new SolidBrush(RowDownloadedNormal);
        private static readonly SolidBrush RowDownloadedAltBrush = new SolidBrush(RowDownloadedAlt);
        private static readonly SolidBrush RowDownloadedHoverBrush = new SolidBrush(RowDownloadedHover);
        private static readonly SolidBrush RowSelectedActiveBrush = new SolidBrush(RowSelectedActive);
        private static readonly SolidBrush RowSelectedInactiveBrush = new SolidBrush(RowSelectedInactive);

        private static readonly Pen HeaderBorderPen = new Pen(HeaderBorder, 1);
        private static readonly Pen HeaderSeparatorPen = new Pen(Color.FromArgb(55, 60, 70), 1);
        private static readonly Pen SelectedActiveBorderPen = new Pen(RowSelectedActiveBorder, 4);
        private static readonly Pen SelectedInactiveBorderPen = new Pen(RowSelectedInactiveBorder, 4);
        private static readonly SolidBrush SortArrowBrush = new SolidBrush(HeaderTextSorted);

        private readonly ListView _listView;
        private readonly ListViewColumnSorter _columnSorter;
        private readonly HeaderCursorWindow _headerCursor;
        private readonly Timer _columnResizeDebounce = new Timer { Interval = 150 };

        private int DefaultSortColumn = 0;
        private SortOrder DefaultSortOrder = SortOrder.Ascending;

        private enum ColumnFillMode { StretchLastColumn, Proportional }
        private ColumnFillMode _fillMode = ColumnFillMode.Proportional;

        private int MarqueeStartDelayMs = 250;
        private int MarqueePauseMs = 500;
        private float MarqueeSpeedPxPerSecond = 30f;
        private int MarqueeFadeWidthPx = 8;
        private int MinOverflowForMarqueePx = 2;
        private float MarqueeMinProgressPerSecond = 0.15f;

        private readonly Timer _marqueeTimer = new Timer { Interval = 8 }; // ~120 FPS
        private readonly Stopwatch _marqueeSw = Stopwatch.StartNew();

        private float[] _marqueeOffsets = new float[0];
        private float[] _marqueeMax = new float[0];
        private float[] _marqueeProgress = new float[0];
        private int[] _marqueeDirs = new int[0];
        private int[] _marqueeHoldMs = new int[0];

        private int _marqueeSelectedIndex = -1;
        private long _marqueeLastTickMs;
        private long _marqueeStartAtMs;

        private int _hoveredItemIndex = -1;
        private bool _inPostPaintOverlay;
        private bool _inAutoFit;
        private bool _userIsResizingColumns;
        private float[] _columnRatios = new float[0];

        private IntPtr _headerHfont = IntPtr.Zero;

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private bool _suppressHeader = true;

        public bool SuppressHeader
        {
            get => _suppressHeader;
            set
            {
                if (_suppressHeader == value) return;
                _suppressHeader = value;

                // Invalidate ListView and header control
                _listView.Invalidate();
                if (_headerCursor.Handle != IntPtr.Zero)
                {
                    InvalidateRect(_headerCursor.Handle, IntPtr.Zero, true);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HDHITTESTINFO
        {
            public Point pt;
            public uint flags;
            public int iItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HDLAYOUT
        {
            public IntPtr prc;
            public IntPtr pwpos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private sealed class HeaderCursorWindow : NativeWindow
        {
            private readonly ModernListView _owner;

            public HeaderCursorWindow(ModernListView owner)
            {
                _owner = owner;
            }

            public void Attach(IntPtr headerHwnd)
            {
                if (headerHwnd != IntPtr.Zero)
                    AssignHandle(headerHwnd);
            }

            protected override void WndProc(ref Message m)
            {
                const int WM_LBUTTONDOWN = 0x0201;
                const int WM_LBUTTONUP = 0x0202;
                const int WM_LBUTTONDBLCLK = 0x0203;
                const int WM_RBUTTONDOWN = 0x0204;
                const int WM_RBUTTONUP = 0x0205;

                // Block mouse interaction when header is suppressed, but still handle layout
                if (_owner._suppressHeader)
                {
                    // Block mouse clicks
                    if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_LBUTTONUP || m.Msg == WM_LBUTTONDBLCLK ||
                        m.Msg == WM_RBUTTONDOWN || m.Msg == WM_RBUTTONUP)
                    {
                        m.Result = IntPtr.Zero;
                        return;
                    }

                    if (m.Msg == WM_SETCURSOR)
                    {
                        Cursor.Current = Cursors.Default;
                        m.Result = (IntPtr)1;
                        return;
                    }

                    // Still handle HDM_LAYOUT to maintain custom header height
                    if (m.Msg == HDM_LAYOUT)
                    {
                        base.WndProc(ref m);

                        try
                        {
                            HDLAYOUT hdl = Marshal.PtrToStructure<HDLAYOUT>(m.LParam);
                            WINDOWPOS wpos = Marshal.PtrToStructure<WINDOWPOS>(hdl.pwpos);
                            RECT rc = Marshal.PtrToStructure<RECT>(hdl.prc);

                            wpos.cy = CUSTOM_HEADER_HEIGHT;
                            rc.top = CUSTOM_HEADER_HEIGHT;

                            Marshal.StructureToPtr(wpos, hdl.pwpos, false);
                            Marshal.StructureToPtr(rc, hdl.prc, false);
                        }
                        catch { }

                        m.Result = (IntPtr)1;
                        return;
                    }

                    base.WndProc(ref m);
                    return;
                }

                if (m.Msg == WM_ERASEBKGND)
                {
                    m.Result = (IntPtr)1;
                    return;
                }

                if (m.Msg == WM_SETCURSOR)
                {
                    if (IsOnDivider())
                    {
                        base.WndProc(ref m);
                        return;
                    }

                    Cursor.Current = Cursors.Hand;
                    m.Result = (IntPtr)1;
                    return;
                }

                if (m.Msg == HDM_LAYOUT)
                {
                    base.WndProc(ref m);

                    try
                    {
                        HDLAYOUT hdl = Marshal.PtrToStructure<HDLAYOUT>(m.LParam);
                        WINDOWPOS wpos = Marshal.PtrToStructure<WINDOWPOS>(hdl.pwpos);
                        RECT rc = Marshal.PtrToStructure<RECT>(hdl.prc);

                        wpos.cy = CUSTOM_HEADER_HEIGHT;
                        rc.top = CUSTOM_HEADER_HEIGHT;

                        Marshal.StructureToPtr(wpos, hdl.pwpos, false);
                        Marshal.StructureToPtr(rc, hdl.prc, false);
                    }
                    catch { }

                    m.Result = (IntPtr)1;
                    return;
                }

                base.WndProc(ref m);

                if (m.Msg == WM_PAINT || m.Msg == WM_PRINTCLIENT)
                    _owner.PaintHeaderRightGap(Handle);
            }

            private bool IsOnDivider()
            {
                if (Handle == IntPtr.Zero || !GetCursorPos(out var screenPt)) return false;

                var pt = screenPt;
                if (!ScreenToClient(Handle, ref pt)) return false;

                var hti = new HDHITTESTINFO { pt = pt };
                IntPtr mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(HDHITTESTINFO)));
                try
                {
                    Marshal.StructureToPtr(hti, mem, false);
                    SendMessage(Handle, HDM_HITTEST, IntPtr.Zero, mem);
                    hti = Marshal.PtrToStructure<HDHITTESTINFO>(mem);
                    return (hti.flags & (HHT_ONDIVIDER | HHT_ONDIVOPEN)) != 0;
                }
                finally
                {
                    Marshal.FreeHGlobal(mem);
                }
            }
        }

        public ModernListView(ListView listView, ListViewColumnSorter columnSorter)
        {
            _listView = listView;
            _columnSorter = columnSorter;
            _headerCursor = new HeaderCursorWindow(this);

            _listView.OwnerDraw = true;
            _listView.FullRowSelect = true;
            _listView.View = View.Details;
            _listView.HotTracking = false;
            _listView.HoverSelection = false;
            _listView.GridLines = false;

            EnableManagedDoubleBuffering();

            if (_listView.IsHandleCreated)
                AssignHandle(_listView.Handle);

            _listView.DrawColumnHeader += DrawColumnHeader;
            _listView.DrawItem += DrawItem;
            _listView.DrawSubItem += DrawSubItem;
            _listView.MouseMove += OnMouseMove;
            _listView.MouseLeave += OnMouseLeave;
            _listView.Paint += OnPaint;
            _listView.Resize += OnResize;
            _listView.ColumnWidthChanging += OnColumnWidthChanging;
            _listView.ColumnWidthChanged += OnColumnWidthChanged;
            _listView.HandleCreated += OnHandleCreated;
            _listView.HandleDestroyed += OnHandleDestroyed;

            _listView.SelectedIndexChanged += (s, e) => RecalcMarqueeForSelection();
            _listView.ItemSelectionChanged += (s, e) => RecalcMarqueeForSelection();
            _listView.GotFocus += (s, e) => UpdateMarqueeTimerState();
            _listView.LostFocus += (s, e) => UpdateMarqueeTimerState();

            _columnResizeDebounce.Tick += (s, e) =>
            {
                _columnResizeDebounce.Stop();
                _userIsResizingColumns = false;
                CaptureColumnRatios();
                AutoFitColumnsToWidth();
                RecalcMarqueeForSelection();
                _listView.Invalidate();
            };

            _marqueeTimer.Tick += (s, e) => UpdateMarquee();

            InitializeColumnSizing();
            RecalcMarqueeForSelection();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                m.Result = (IntPtr)1;
                return;
            }

            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && !_inPostPaintOverlay)
            {
                try
                {
                    _inPostPaintOverlay = true;
                    PaintEmptyAreaOverlay();
                }
                finally
                {
                    _inPostPaintOverlay = false;
                }
            }

            switch (m.Msg)
            {
                case WM_VSCROLL:
                case WM_HSCROLL:
                case WM_MOUSEWHEEL:
                    OnScrollDetected();
                    break;
                case WM_KEYDOWN:
                    int key = m.WParam.ToInt32();
                    if (key == 0x21 || key == 0x22 || key == 0x26 || key == 0x28)
                        OnScrollDetected();
                    break;
            }
        }

        private void OnScrollDetected()
        {
            if (!_listView.IsHandleCreated) 
                return;

            // Keep hover state in sync after scroll without forcing a full repaint
            UpdateHoverFromCursor();
        }

        private void UpdateHoverFromCursor()
        {
            if (!_listView.IsHandleCreated)
                return;

            Point clientPt = _listView.PointToClient(Control.MousePosition);
            int newHoveredIndex = -1;

            if (_listView.ClientRectangle.Contains(clientPt) && !IsPointInHeader(clientPt))
            {
                var hit = _listView.HitTest(clientPt);
                newHoveredIndex = hit.Item != null ? hit.Item.Index : -1;
            }

            if (newHoveredIndex == _hoveredItemIndex)
                return;

            int oldIndex = _hoveredItemIndex;
            _hoveredItemIndex = newHoveredIndex;

            if (oldIndex >= 0 && oldIndex < _listView.Items.Count)
                _listView.RedrawItems(oldIndex, oldIndex, true);

            if (newHoveredIndex >= 0 && newHoveredIndex < _listView.Items.Count)
                _listView.RedrawItems(newHoveredIndex, newHoveredIndex, true);
        }

        private void OnHandleCreated(object sender, EventArgs e)
        {
            AssignHandle(_listView.Handle);
            ApplyModernScrollbars();
            EnableNativeDoubleBuffering();

            _listView.BeginInvoke(new Action(() =>
            {
                if (!_listView.IsHandleCreated) return;

                IntPtr header = SendMessage(_listView.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                _headerCursor.Attach(header);

                ApplyHeaderFont(header);
                PaintHeaderRightGap(header);
            }));

            InitializeColumnSizing();
            RecalcMarqueeForSelection();
        }

        private void OnHandleDestroyed(object sender, EventArgs e)
        {
            _columnResizeDebounce.Stop();
            _marqueeTimer.Stop();
            _headerCursor.ReleaseHandle();
            ReleaseHandle();

            if (_headerHfont != IntPtr.Zero)
            {
                DeleteObject(_headerHfont);
                _headerHfont = IntPtr.Zero;
            }
        }

        private void ApplyHeaderFont(IntPtr headerHandle)
        {
            if (headerHandle == IntPtr.Zero) return;

            if (_headerHfont == IntPtr.Zero)
                _headerHfont = HeaderFont.ToHfont();

            SendMessage(headerHandle, WM_SETFONT, _headerHfont, (IntPtr)1);
            _listView.Invalidate();
        }

        private void EnableManagedDoubleBuffering()
        {
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_listView, true, null);
        }

        private void EnableNativeDoubleBuffering()
        {
            if (!_listView.IsHandleCreated) return;
            SendMessage(_listView.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE,
                (IntPtr)LVS_EX_DOUBLEBUFFER, (IntPtr)LVS_EX_DOUBLEBUFFER);
        }

        private void ApplyModernScrollbars()
        {
            if (!_listView.IsHandleCreated) return;

            int dark = 1;
            int hr = DwmSetWindowAttribute(_listView.Handle, 20, ref dark, sizeof(int));
            if (hr != 0)
                DwmSetWindowAttribute(_listView.Handle, 19, ref dark, sizeof(int));

            if (SetWindowTheme(_listView.Handle, "DarkMode_Explorer", null) != 0)
                SetWindowTheme(_listView.Handle, "Explorer", null);
        }

        private void InitializeColumnSizing()
        {
            if (!_listView.IsHandleCreated || _listView.Columns.Count == 0) return;
            CaptureColumnRatios();
            AutoFitColumnsToWidth();
        }

        private void OnResize(object sender, EventArgs e)
        {
            AutoFitColumnsToWidth();
            RecalcMarqueeForSelection();
            _listView.Invalidate();
        }

        private void OnColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            _userIsResizingColumns = true;
            _columnResizeDebounce.Stop();
        }

        private void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (_inAutoFit) return;

            if (_userIsResizingColumns)
            {
                _columnResizeDebounce.Stop();
                _columnResizeDebounce.Start();
                return;
            }

            RecalcMarqueeForSelection();
            _listView.Invalidate();
        }

        private void CaptureColumnRatios()
        {
            int count = _listView.Columns.Count;
            if (count == 0) { _columnRatios = new float[0]; return; }

            float total = 0f;
            for (int i = 0; i < count; i++)
                total += Math.Max(1, _listView.Columns[i].Width);

            if (total <= 0.01f) total = 1f;

            _columnRatios = new float[count];
            for (int i = 0; i < count; i++)
                _columnRatios[i] = Math.Max(1, _listView.Columns[i].Width) / total;
        }

        private void AutoFitColumnsToWidth()
        {
            if (_inAutoFit || _userIsResizingColumns || !_listView.IsHandleCreated || _listView.Columns.Count == 0)
                return;

            try
            {
                _inAutoFit = true;
                _listView.BeginUpdate();

                int clientWidth = Math.Max(0, _listView.ClientSize.Width);
                if (clientWidth <= 0) return;

                if (_fillMode == ColumnFillMode.StretchLastColumn)
                    StretchLastColumn(clientWidth);
                else
                    ProportionalFill(clientWidth);
            }
            finally
            {
                _listView.EndUpdate();
                _inAutoFit = false;
            }
        }

        private void StretchLastColumn(int clientWidth)
        {
            int count = _listView.Columns.Count;
            if (count == 1)
            {
                _listView.Columns[0].Width = clientWidth;
                return;
            }

            int otherTotal = 0;
            for (int i = 0; i < count - 1; i++)
                otherTotal += _listView.Columns[i].Width;

            _listView.Columns[count - 1].Width = Math.Max(60, clientWidth - otherTotal);
        }

        private void ProportionalFill(int clientWidth)
        {
            int count = _listView.Columns.Count;
            if (_columnRatios.Length != count)
                CaptureColumnRatios();

            int used = 0;
            for (int i = 0; i < count - 1; i++)
            {
                int w = Math.Max(30, (int)Math.Round(clientWidth * _columnRatios[i]));
                _listView.Columns[i].Width = w;
                used += w;
            }

            _listView.Columns[count - 1].Width = Math.Max(60, clientWidth - used);
        }

        private int GetHeaderHeight()
        {
            if (_listView.Items.Count > 0 && _listView.Items[0].Bounds.Top > 0)
                return _listView.Items[0].Bounds.Top;

            return CUSTOM_HEADER_HEIGHT;
        }

        private bool IsPointInHeader(Point pt) => pt.Y >= 0 && pt.Y < GetHeaderHeight();

        private static bool IsDownloadedItem(ListViewItem item)
        {
            if (item == null || item.SubItems.Count <= 1) return false;
            return MainForm.DownloadedReleaseNames.Contains(item.SubItems[1].Text);
        }

        private SolidBrush GetRowBrush(int itemIndex, bool isSelected, bool isHovered, bool isDownloaded = false)
        {
            if (isSelected)
                return _listView.Focused ? RowSelectedActiveBrush : RowSelectedInactiveBrush;
            if (isDownloaded)
            {
                if (isHovered) return RowDownloadedHoverBrush;
                return (itemIndex % 2 == 1) ? RowDownloadedAltBrush : RowDownloadedNormalBrush;
            }
            if (isHovered)
                return RowHoverBrush;
            return (itemIndex % 2 == 1) ? RowAltBrush : RowNormalBrush;
        }

        private Color GetRowColor(int itemIndex, bool isSelected, bool isHovered, bool isDownloaded = false)
        {
            if (isSelected) return _listView.Focused ? RowSelectedActive : RowSelectedInactive;
            if (isDownloaded)
            {
                if (isHovered) return RowDownloadedHover;
                return (itemIndex % 2 == 1) ? RowDownloadedAlt : RowDownloadedNormal;
            }
            if (isHovered) return RowHover;
            return (itemIndex % 2 == 1) ? RowAlt : RowNormal;
        }

        private void PaintHeaderRightGap(IntPtr headerHandle)
        {
            if (headerHandle == IntPtr.Zero || !_listView.IsHandleCreated) return;

            RECT rc;
            if (!GetClientRect(headerHandle, out rc)) return;

            int width = rc.right - rc.left;
            int height = rc.bottom - rc.top;
            if (width <= 0 || height <= 0) return;

            int total = 0;
            foreach (ColumnHeader col in _listView.Columns)
                total += col.Width;

            if (total >= width) return;

            using (var g = Graphics.FromHwnd(headerHandle))
            {
                g.FillRectangle(HeaderBgBrush, new Rectangle(total, 0, width - total + 1, height));
                g.DrawLine(HeaderBorderPen, total, height - 1, width, height - 1);
            }
        }

        private void DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            var g = e.Graphics;
            int listViewWidth = _listView.ClientSize.Width;

            // During loading, just fill with background color
            if (_suppressHeader)
            {
                g.FillRectangle(RowNormalBrush, new Rectangle(0, e.Bounds.Y, listViewWidth, e.Bounds.Height));
                e.DrawDefault = false;
                return;
            }

            g.FillRectangle(HeaderBgBrush, e.Bounds);

            if (e.ColumnIndex == _listView.Columns.Count - 1)
            {
                int rightEdge = e.Bounds.Right;
                if (rightEdge < listViewWidth)
                {
                    var extended = new Rectangle(rightEdge, e.Bounds.Top, listViewWidth - rightEdge + 1, e.Bounds.Height);
                    g.FillRectangle(HeaderBgBrush, extended);
                    g.DrawLine(HeaderBorderPen, rightEdge, e.Bounds.Bottom - 1, listViewWidth, e.Bounds.Bottom - 1);
                }
            }

            g.DrawLine(HeaderBorderPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            if (e.ColumnIndex < _listView.Columns.Count - 1)
            {
                int separatorX = e.Bounds.Right - 1;
                g.DrawLine(HeaderSeparatorPen, separatorX, e.Bounds.Top + 4, separatorX, e.Bounds.Bottom - 4);
            }

            var textBounds = new Rectangle(e.Bounds.X + CellPaddingX, e.Bounds.Y, e.Bounds.Width - CellPaddingTotalX, e.Bounds.Height);

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;
            if (e.Header.TextAlign == HorizontalAlignment.Center)
                flags |= TextFormatFlags.HorizontalCenter;
            else if (e.Header.TextAlign == HorizontalAlignment.Right)
                flags |= TextFormatFlags.Right;
            else
                flags |= TextFormatFlags.Left;

            bool hasActiveSort = _columnSorter != null && _columnSorter.Order != SortOrder.None;
            bool isActiveSortedColumn = hasActiveSort && _columnSorter.SortColumn == e.ColumnIndex;

            bool showDefaultSort = !hasActiveSort && DefaultSortOrder != SortOrder.None && e.ColumnIndex == DefaultSortColumn;
            bool tint = isActiveSortedColumn || showDefaultSort;

            Color headerTextColor = tint ? HeaderTextSorted : HeaderText;
            TextRenderer.DrawText(g, e.Header.Text, HeaderFont, textBounds, headerTextColor, flags);

            SortOrder drawOrder = SortOrder.None;
            if (isActiveSortedColumn) drawOrder = _columnSorter.Order;
            else if (showDefaultSort) drawOrder = DefaultSortOrder;

            // Invert sort arrow for "Popularity" column
            if (e.ColumnIndex == 6 && drawOrder != SortOrder.None)
                drawOrder = (drawOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;

            if (drawOrder != SortOrder.None)
                DrawSortArrow(g, e.Bounds, drawOrder);

            e.DrawDefault = false;
        }

        private static void DrawSortArrow(Graphics g, Rectangle bounds, SortOrder order)
        {
            int centerX = bounds.Right - 12;
            int centerY = bounds.Top + (bounds.Height / 2) - 1;

            int halfWidth = 4;
            int halfHeight = 3;

            Point[] arrow = (order == SortOrder.Ascending)
                ? new[]
                {
                    new Point(centerX,             centerY - halfHeight),
                    new Point(centerX - halfWidth, centerY + halfHeight),
                    new Point(centerX + halfWidth, centerY + halfHeight),
                }
                : new[]
                {
                    new Point(centerX,             centerY + halfHeight),
                    new Point(centerX - halfWidth, centerY - halfHeight),
                    new Point(centerX + halfWidth, centerY - halfHeight),
                };

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPolygon(SortArrowBrush, arrow);
            g.SmoothingMode = SmoothingMode.None;
        }

        private void DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = false;
        }

        private void DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var g = e.Graphics;

            bool isSelected = e.Item.Selected;
            bool isHovered = (e.ItemIndex == _hoveredItemIndex) && !isSelected;

            if (e.ColumnIndex == 0)
                DrawRowBackground(g, e.Item, e.ItemIndex, isSelected, isHovered);

            string text = e.SubItem?.Text ?? "";
            var font = isSelected ? ItemFontBold : ItemFont;

            var textBounds = new Rectangle(e.Bounds.X + CellPaddingX, e.Bounds.Y, e.Bounds.Width - CellPaddingTotalX, e.Bounds.Height);
            Color textColor = GetTextColor(e.Item.ForeColor, isSelected, isHovered);

            if (ShouldDrawMarquee(e.ItemIndex, e.ColumnIndex, isSelected, textBounds, text))
            {
                DrawMarqueeText(g, textBounds, e.Bounds, text, font, textColor, _marqueeOffsets[e.ColumnIndex]);
                DrawFadeEdgesOverlay(g, textBounds, GetRowColor(e.ItemIndex, isSelected, isHovered, IsDownloadedItem(e.Item)), MarqueeFadeWidthPx);

                e.DrawDefault = false;
                return;
            }

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;
            flags |= (e.ColumnIndex > 2) ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;
            TextRenderer.DrawText(g, text, font, textBounds, textColor, flags);

            e.DrawDefault = false;
        }

        private void DrawRowBackground(Graphics g, ListViewItem item, int itemIndex, bool isSelected, bool isHovered)
        {
            bool isDl = IsDownloadedItem(item);
            int listViewWidth = _listView.ClientSize.Width;
            var bgBrush = GetRowBrush(itemIndex, isSelected, isHovered, isDl);
            var fullRowRect = new Rectangle(0, item.Bounds.Top, listViewWidth, item.Bounds.Height);
            g.FillRectangle(bgBrush, fullRowRect);

            if (isSelected)
            {
                Pen pen = _listView.Focused ? SelectedActiveBorderPen : SelectedInactiveBorderPen;
                g.DrawLine(pen, 1, item.Bounds.Top + 1, 1, item.Bounds.Bottom - 1);
            }
        }

        private static Color GetTextColor(Color baseColor, bool isSelected, bool isHovered)
        {
            Color c = baseColor;
            if (c == SystemColors.WindowText || c == Color.Empty || c == Color.Black)
                c = Color.White;

            if (isSelected || isHovered)
            {
                int boost = isSelected ? 40 : 20;
                c = Color.FromArgb(
                    Math.Min(255, c.R + boost),
                    Math.Min(255, c.G + boost),
                    Math.Min(255, c.B + boost));
            }

            return c;
        }

        private bool ShouldDrawMarquee(int itemIndex, int columnIndex, bool isSelected, Rectangle textBounds, string text)
        {
            if (!isSelected) return false;

            if (itemIndex != _marqueeSelectedIndex) return false;
            if (string.IsNullOrEmpty(text)) return false;
            if (textBounds.Width <= 8) return false;

            if (columnIndex < 0 || columnIndex >= _marqueeMax.Length) return false;
            return _marqueeMax[columnIndex] > 1f;
        }

        private static void DrawMarqueeText(Graphics g, Rectangle clipBounds, Rectangle itemBounds, string text, Font font, Color textColor, float offset)
        {
            float x = clipBounds.X - offset;
            float y = itemBounds.Y + (itemBounds.Height - font.Height) / 2f;

            var state = g.Save();
            try
            {
                g.SetClip(clipBounds);
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                using (var b = new SolidBrush(textColor))
                using (var sf = StringFormat.GenericTypographic)
                {
                    sf.FormatFlags |= StringFormatFlags.NoWrap;
                    sf.Trimming = StringTrimming.None;
                    g.DrawString(text, font, b, new PointF(x, y), sf);
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        private static void DrawFadeEdgesOverlay(Graphics g, Rectangle bounds, Color bg, int fadeWidthPx)
        {
            int w = Math.Max(0, Math.Min(fadeWidthPx, bounds.Width / 2));
            if (w <= 0) return;

            const int overlap = 2;

            var leftRect = new Rectangle(bounds.Left - overlap, bounds.Top, w + overlap, bounds.Height);
            using (var lb = new LinearGradientBrush(leftRect, Color.FromArgb(255, bg), Color.FromArgb(0, bg), LinearGradientMode.Horizontal))
                g.FillRectangle(lb, leftRect);

            var rightRect = new Rectangle(bounds.Right - w, bounds.Top, w + overlap, bounds.Height);
            using (var rb = new LinearGradientBrush(rightRect, Color.FromArgb(0, bg), Color.FromArgb(255, bg), LinearGradientMode.Horizontal))
                g.FillRectangle(rb, rightRect);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int newHoveredIndex = -1;

            if (!IsPointInHeader(e.Location))
            {
                var hitTest = _listView.HitTest(e.Location);
                newHoveredIndex = hitTest.Item?.Index ?? -1;
            }

            if (newHoveredIndex == _hoveredItemIndex)
                return;

            int oldIndex = _hoveredItemIndex;
            _hoveredItemIndex = newHoveredIndex;

            if (oldIndex >= 0 && oldIndex < _listView.Items.Count)
                _listView.RedrawItems(oldIndex, oldIndex, true);

            if (newHoveredIndex >= 0 && newHoveredIndex < _listView.Items.Count)
                _listView.RedrawItems(newHoveredIndex, newHoveredIndex, true);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (_hoveredItemIndex < 0 || _hoveredItemIndex >= _listView.Items.Count)
                return;

            int oldIndex = _hoveredItemIndex;
            _hoveredItemIndex = -1;
            _listView.RedrawItems(oldIndex, oldIndex, true);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            int listViewWidth = _listView.ClientSize.Width;

            int totalColumnWidth = 0;
            foreach (ColumnHeader col in _listView.Columns)
                totalColumnWidth += col.Width;

            int headerHeight = GetHeaderHeight();

            if (totalColumnWidth < listViewWidth)
            {
                var emptyHeaderRect = new Rectangle(totalColumnWidth, 0, listViewWidth - totalColumnWidth + 1, headerHeight);
                g.FillRectangle(HeaderBgBrush, emptyHeaderRect);
                g.DrawLine(HeaderBorderPen, totalColumnWidth, headerHeight - 1, listViewWidth, headerHeight - 1);
            }

            FillEmptyBottomArea(g, headerHeight, listViewWidth);
        }

        private void FillEmptyBottomArea(Graphics g, int headerHeight, int listViewWidth)
        {
            if (_listView.Items.Count == 0)
            {
                var emptyRect = new Rectangle(0, headerHeight, listViewWidth, _listView.ClientSize.Height - headerHeight);
                g.FillRectangle(RowNormalBrush, emptyRect);
                return;
            }

            int firstVisibleIndex = _listView.TopItem != null ? _listView.TopItem.Index : 0;
            int itemHeight = _listView.Items[0].Bounds.Height;
            int visibleCount = (_listView.ClientSize.Height - headerHeight) / Math.Max(1, itemHeight) + 2;
            int lastVisibleIndex = Math.Min(firstVisibleIndex + visibleCount, _listView.Items.Count - 1);

            var lastItem = _listView.Items[lastVisibleIndex];
            int bottomY = lastItem.Bounds.Bottom;

            if (bottomY < _listView.ClientSize.Height)
            {
                var emptyBottomRect = new Rectangle(0, bottomY, listViewWidth, _listView.ClientSize.Height - bottomY);
                g.FillRectangle(RowNormalBrush, emptyBottomRect);
            }
        }

        private void PaintEmptyAreaOverlay()
        {
            int w = _listView.ClientSize.Width;
            int h = _listView.ClientSize.Height;
            if (w <= 0 || h <= 0) return;

            int headerHeight = GetHeaderHeight();
            int fillTop = headerHeight;

            if (_listView.Items.Count > 0)
            {
                int firstVisibleIndex = _listView.TopItem != null ? _listView.TopItem.Index : 0;
                int itemHeight = _listView.Items[0].Bounds.Height;
                int visibleCount = (h - headerHeight) / Math.Max(1, itemHeight) + 2;
                int lastVisibleIndex = Math.Min(firstVisibleIndex + visibleCount, _listView.Items.Count - 1);

                fillTop = Math.Max(fillTop, _listView.Items[lastVisibleIndex].Bounds.Bottom);
            }

            if (fillTop < h)
            {
                int overlayTop = (fillTop == headerHeight) ? Math.Max(0, fillTop - 10) : fillTop;
                using (var g = Graphics.FromHwnd(_listView.Handle))
                {
                    g.FillRectangle(RowNormalBrush, new Rectangle(0, overlayTop, w, h - overlayTop));
                }
            }
        }

        private ListViewItem GetActiveSelectedItem()
        {
            var focused = _listView.FocusedItem;
            if (focused != null && focused.Selected)
                return focused;

            if (_listView.SelectedItems.Count > 0)
                return _listView.SelectedItems[0];

            return null;
        }

        private void EnsureMarqueeArrays()
        {
            int count = _listView.Columns.Count;
            if (count < 0) count = 0;

            if (_marqueeOffsets.Length == count &&
                _marqueeMax.Length == count &&
                _marqueeProgress.Length == count &&
                _marqueeDirs.Length == count &&
                _marqueeHoldMs.Length == count)
                return;

            _marqueeOffsets = new float[count];
            _marqueeMax = new float[count];
            _marqueeProgress = new float[count];
            _marqueeDirs = new int[count];
            _marqueeHoldMs = new int[count];

            for (int i = 0; i < count; i++)
                _marqueeDirs[i] = 1;
        }

        private void RecalcMarqueeForSelection()
        {
            if (!_listView.IsHandleCreated) return;

            var item = GetActiveSelectedItem();
            int idx = item != null ? item.Index : -1;

            if (idx != _marqueeSelectedIndex)
            {
                _marqueeSelectedIndex = idx;
                ResetMarqueeState();
                return;
            }

            ResetMarqueeExtentsOnly();
        }

        private void ResetMarqueeState()
        {
            EnsureMarqueeArrays();

            Array.Clear(_marqueeOffsets, 0, _marqueeOffsets.Length);
            Array.Clear(_marqueeMax, 0, _marqueeMax.Length);
            Array.Clear(_marqueeProgress, 0, _marqueeProgress.Length);
            Array.Clear(_marqueeHoldMs, 0, _marqueeHoldMs.Length);

            for (int i = 0; i < _marqueeDirs.Length; i++)
                _marqueeDirs[i] = 1;

            _marqueeLastTickMs = _marqueeSw.ElapsedMilliseconds;
            _marqueeStartAtMs = _marqueeLastTickMs + Math.Max(0, MarqueeStartDelayMs);

            ComputeMarqueeMaxForSelectedRow();
            UpdateMarqueeTimerState();

            if (_marqueeSelectedIndex >= 0 && _marqueeSelectedIndex < _listView.Items.Count)
                _listView.RedrawItems(_marqueeSelectedIndex, _marqueeSelectedIndex, true);
            else
                _listView.Invalidate();
        }

        private void ResetMarqueeExtentsOnly()
        {
            EnsureMarqueeArrays();

            float[] oldMax = (float[])_marqueeMax.Clone();
            ComputeMarqueeMaxForSelectedRow();

            for (int i = 0; i < _marqueeOffsets.Length; i++)
            {
                if (_marqueeMax[i] <= 0f)
                {
                    _marqueeOffsets[i] = 0f;
                    _marqueeProgress[i] = 0f;
                    _marqueeHoldMs[i] = 0;
                    _marqueeDirs[i] = 1;
                    continue;
                }

                float overflow = _marqueeMax[i];
                float travel = overflow + (2f * MarqueeFadeWidthPx);
                float p = Clamp01(_marqueeProgress[i]);
                _marqueeProgress[i] = p;
                _marqueeOffsets[i] = (Ease(p) * travel) - MarqueeFadeWidthPx;
            }

            UpdateMarqueeTimerState();

            bool changed = false;
            for (int i = 0; i < oldMax.Length && i < _marqueeMax.Length; i++)
            {
                if (Math.Abs(oldMax[i] - _marqueeMax[i]) > 0.5f) { changed = true; break; }
            }

            if (changed && _marqueeSelectedIndex >= 0 && _marqueeSelectedIndex < _listView.Items.Count)
                _listView.RedrawItems(_marqueeSelectedIndex, _marqueeSelectedIndex, true);
        }

        private void ComputeMarqueeMaxForSelectedRow()
        {
            if (_marqueeSelectedIndex < 0 || _marqueeSelectedIndex >= _listView.Items.Count)
            {
                for (int i = 0; i < _marqueeMax.Length; i++) _marqueeMax[i] = 0f;
                return;
            }

            var item = _listView.Items[_marqueeSelectedIndex];
            int colCount = _listView.Columns.Count;
            int minOverflow = Math.Max(0, MinOverflowForMarqueePx);

            for (int col = 0; col < colCount; col++)
            {
                string text = (col == 0)
                    ? (item.Text ?? "")
                    : (col < item.SubItems.Count ? (item.SubItems[col]?.Text ?? "") : "");

                if (string.IsNullOrEmpty(text))
                {
                    _marqueeMax[col] = 0f;
                    continue;
                }

                int available = Math.Max(0, _listView.Columns[col].Width - CellPaddingTotalX);
                if (available <= 6)
                {
                    _marqueeMax[col] = 0f;
                    continue;
                }

                Size measured = TextRenderer.MeasureText(
                    text,
                    ItemFontBold,
                    new Size(int.MaxValue, ItemFontBold.Height),
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

                float overflow = measured.Width - available;
                _marqueeMax[col] = (overflow > minOverflow) ? overflow : 0f;
            }
        }

        private void UpdateMarqueeTimerState()
        {
            bool any = false;
            for (int i = 0; i < _marqueeMax.Length; i++)
            {
                if (_marqueeMax[i] > 1f) { any = true; break; }
            }

            if (any && _marqueeSelectedIndex >= 0)
            {
                if (!_marqueeTimer.Enabled)
                {
                    _marqueeLastTickMs = _marqueeSw.ElapsedMilliseconds;
                    _marqueeTimer.Start();
                }
            }
            else
            {
                if (_marqueeTimer.Enabled)
                    _marqueeTimer.Stop();
            }
        }

        private void UpdateMarquee()
        {
            var active = GetActiveSelectedItem();
            int idx = active != null ? active.Index : -1;
            if (idx != _marqueeSelectedIndex)
            {
                _marqueeSelectedIndex = idx;
                ResetMarqueeState();
                return;
            }

            if (_marqueeSelectedIndex < 0 || _marqueeSelectedIndex >= _listView.Items.Count)
            {
                _marqueeTimer.Stop();
                return;
            }

            long nowMs = _marqueeSw.ElapsedMilliseconds;
            float dt = (nowMs - _marqueeLastTickMs) / 1000f;
            _marqueeLastTickMs = nowMs;

            if (dt <= 0) return;
            if (dt > 0.08f) dt = 0.08f;

            if (nowMs < _marqueeStartAtMs) return;

            bool changed = false;
            int pauseMs = Math.Max(0, MarqueePauseMs);
            float speed = Math.Max(1f, MarqueeSpeedPxPerSecond);

            for (int col = 0; col < _marqueeOffsets.Length; col++)
            {
                float overflow = _marqueeMax[col];
                if (overflow <= 1f)
                {
                    if (_marqueeOffsets[col] != 0f || _marqueeProgress[col] != 0f)
                    {
                        _marqueeOffsets[col] = 0f;
                        _marqueeProgress[col] = 0f;
                        _marqueeHoldMs[col] = 0;
                        _marqueeDirs[col] = 1;
                        changed = true;
                    }
                    continue;
                }

                if (_marqueeHoldMs[col] > 0)
                {
                    _marqueeHoldMs[col] -= (int)Math.Round(dt * 1000);
                    if (_marqueeHoldMs[col] < 0) _marqueeHoldMs[col] = 0;
                    continue;
                }

                int dir = _marqueeDirs[col];
                if (dir == 0) dir = 1;

                float travel = overflow + (2f * MarqueeFadeWidthPx);

                float pSpeed = speed / (1.5f * travel);
                pSpeed = Math.Max(pSpeed, MarqueeMinProgressPerSecond);

                float p = _marqueeProgress[col] + dir * dt * pSpeed;

                if (p <= 0f)
                {
                    p = 0f;
                    _marqueeDirs[col] = 1;
                    _marqueeHoldMs[col] = pauseMs;
                }
                else if (p >= 1f)
                {
                    p = 1f;
                    _marqueeDirs[col] = -1;
                    _marqueeHoldMs[col] = pauseMs;
                }

                float newOffset = (Ease(p) * travel) - MarqueeFadeWidthPx;

                if (Math.Abs(newOffset - _marqueeOffsets[col]) > 0.02f || Math.Abs(p - _marqueeProgress[col]) > 0.0005f)
                {
                    _marqueeProgress[col] = p;
                    _marqueeOffsets[col] = newOffset;
                    changed = true;
                }
            }

            if (changed)
                _listView.RedrawItems(_marqueeSelectedIndex, _marqueeSelectedIndex, true);
        }

        public void ApplySort(int columnIndex, SortOrder order)
        {
            if (_columnSorter == null) return;

            _columnSorter.SortColumn = columnIndex;
            _columnSorter.Order = order;

            _listView.BeginUpdate();
            try
            {
                _listView.Sort();
            }
            finally
            {
                _listView.EndUpdate();
            }

            // Invalidate header to update sort indicators
            _listView.Invalidate(new Rectangle(0, 0, _listView.ClientSize.Width,
                _listView.Font.Height + 8));
        }

        private static float Ease(float p)
        {
            p = Clamp01(p);
            float t = 0.5f - 0.5f * (float)Math.Cos(Math.PI * p);
            const float gamma = 0.65f;
            return (float)Math.Pow(t, gamma);
        }

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}