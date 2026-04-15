using AndroidSideloader;
using AndroidSideloader.Utilities;
using JR.Utils.GUI.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public enum SortField { Name, LastUpdated, Size, Popularity }
public enum SortDirection { Ascending, Descending }

public class FastGalleryPanel : Control
{
    // Data
    private List<ListViewItem> _items;
    private List<ListViewItem> _originalItems; // Keep original for re-sorting
    private int _tileWidth;
    private int _tileHeight;
    private readonly int _spacing;
    private readonly int _baseTileWidth;
    private readonly int _baseTileHeight;

    // Grouping
    private Dictionary<string, List<ListViewItem>> _groupedByPackage;
    private List<GroupedTile> _displayTiles;
    private int _expandedTileIndex = -1;
    private float _expandOverlayOpacity = 0f;
    private float _targetExpandOverlayOpacity = 0f;
    private int _overlayHoveredVersion = -1;
    private Rectangle _overlayRect;
    private List<Rectangle> _versionRects;
    private int _overlayScrollOffset = 0;
    private int _overlayMaxScroll = 0;

    private class GroupedTile
    {
        public string PackageName;
        public string GameName;
        public string BaseGameName; // Common name across all versions
        public List<ListViewItem> Versions;
        public ListViewItem Primary => Versions[0];
    }

    // Sorting
    private SortField _currentSortField = SortField.Name;
    private SortDirection _currentSortDirection = SortDirection.Ascending;
    public SortField CurrentSortField => _currentSortField;
    public SortDirection CurrentSortDirection => _currentSortDirection;
    private readonly Panel _sortPanel;
    private readonly List<Button> _sortButtons;
    private Label _sortStatusLabel;
    private ModernSlider _sizeSlider;
    private Label _sizeLabel;
    private const int SORT_PANEL_HEIGHT = 36;
    private const int SLIDER_MIN = 100;
    private const int SLIDER_MAX = 150;
    private const int SLIDER_DEFAULT = 100;

    // Layout
    private int _columns;
    private int _rows;
    private int _contentHeight;
    private int _leftPadding;

    // Smooth scrolling
    private float _scrollY;
    private float _targetScrollY;
    private bool _isScrolling;
    private readonly VScrollBar _scrollBar;

    // Animation
    private readonly System.Windows.Forms.Timer _animationTimer;
    private readonly Dictionary<int, TileAnimationState> _tileStates;

    // Image cache (LRU)
    private readonly Dictionary<string, Image> _imageCache;
    private readonly Queue<string> _cacheOrder;
    private const int MAX_CACHE_SIZE = 200;

    // Interaction
    private int _hoveredIndex = -1;
    public int _selectedIndex = -1;
    private ListViewItem _selectedItem = null;
    private bool _isHoveringDeleteButton = false;

    // Context Menu & Favorites
    private ContextMenuStrip _contextMenu;
    private int _rightClickedIndex = -1;
    private int _rightClickedVersionIndex = -1;
    private HashSet<string> _favoritesCache;

    // Rendering
    private Bitmap _backBuffer;

    // Visual constants
    private const int CORNER_RADIUS = 10;
    private const int THUMB_CORNER_RADIUS = 8;
    private const float HOVER_SCALE = 1.08f;
    private const float ANIMATION_SPEED = 0.33f;
    private const float SCROLL_SMOOTHING = 0.3f;
    private const int DELETE_BUTTON_SIZE = 26;
    private const int DELETE_BUTTON_MARGIN = 6;
    private const int VERSION_ROW_HEIGHT = 44;
    private const int OVERLAY_PADDING = 10;
    private const int OVERLAY_MAX_HEIGHT = 320;

    // Theme colors
    private static readonly Color TileBorderHover = Color.FromArgb(93, 203, 173);
    private static readonly Color TileBorderSelected = Color.FromArgb(200, 200, 200);
    private static readonly Color TileBorderFavorite = Color.FromArgb(255, 215, 0);
    private static readonly Color BadgeFavoriteBg = Color.FromArgb(200, 255, 180, 0);
    private static readonly Color TextColor = Color.FromArgb(245, 255, 255, 255);
    private static readonly Color BadgeInstalledBg = Color.FromArgb(180, 60, 145, 230);
    private static readonly Color BadgeDownloadedBg = Color.FromArgb(180, 80, 175, 150);
    private static readonly Color DeleteButtonBg = Color.FromArgb(200, 180, 50, 50);
    private static readonly Color DeleteButtonHoverBg = Color.FromArgb(255, 220, 70, 70);
    private static readonly Color SortButtonBg = Color.FromArgb(40, 42, 48);
    private static readonly Color SortButtonActiveBg = Color.FromArgb(93, 203, 173);
    private static readonly Color SortButtonHoverBg = Color.FromArgb(55, 58, 65);
    private static readonly Color OverlayBgColor = Color.FromArgb(250, 28, 30, 36);
    private static readonly Color VersionRowHoverBg = Color.FromArgb(255, 45, 48, 56);

    public event EventHandler<int> TileClicked;
    public event EventHandler<int> TileDoubleClicked;
    public event EventHandler<int> TileDeleteClicked;
    public event EventHandler<int> TileRightClicked;
    public event EventHandler<string> TileHovered; // Update release notes for hovered grouped sub-item
    public event EventHandler<SortField> SortChanged;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

    private void ApplyModernScrollbars()
    {
        if (_scrollBar == null || !_scrollBar.IsHandleCreated) return;
        int dark = 1;
        int hr = DwmSetWindowAttribute(_scrollBar.Handle, 20, ref dark, sizeof(int));
        if (hr != 0) DwmSetWindowAttribute(_scrollBar.Handle, 19, ref dark, sizeof(int));
        if (SetWindowTheme(_scrollBar.Handle, "DarkMode_Explorer", null) != 0)
            SetWindowTheme(_scrollBar.Handle, "Explorer", null);
    }

    private class TileAnimationState
    {
        public float Scale = 1.0f;
        public float TargetScale = 1.0f;
        public float BorderOpacity = 0f;
        public float TargetBorderOpacity = 0f;
        public float BackgroundBrightness = 30f;
        public float TargetBackgroundBrightness = 30f;
        public float SelectionOpacity = 0f;
        public float TargetSelectionOpacity = 0f;
        public float TooltipOpacity = 0f;
        public float TargetTooltipOpacity = 0f;
        public float DeleteButtonOpacity = 0f;
        public float TargetDeleteButtonOpacity = 0f;
        public float FavoriteOpacity = 0f;
        public float TargetFavoriteOpacity = 0f;
        public float GroupBadgeOpacity = 0f;
        public float TargetGroupBadgeOpacity = 0f;
    }

    private class ModernSlider : Control
    {
        private int _minimum = 100;
        private int _maximum = 150;
        private int _value = 100;
        private bool _dragging;
        private bool _hovering;
        private float _hoverOpacity;
        private readonly System.Windows.Forms.Timer _fadeTimer;

        private static readonly Color TrackBg = Color.FromArgb(50, 52, 58);
        private static readonly Color TrackFill = Color.FromArgb(93, 203, 173);
        private static readonly Color ThumbColor = Color.FromArgb(93, 203, 173);
        private static readonly Color ThumbHoverColor = Color.FromArgb(130, 220, 195);
        private static readonly Color ThumbBorderColor = Color.FromArgb(255, 255, 255);

        private const int TRACK_HEIGHT = 4;
        private const int THUMB_RADIUS = 5;
        private const int THUMB_HOVER_RADIUS = 7;

        public event EventHandler ValueChanged;

        public int Minimum { get { return _minimum; } set { _minimum = value; Invalidate(); } }
        public int Maximum { get { return _maximum; } set { _maximum = value; Invalidate(); } }
        public int Value
        {
            get { return _value; }
            set
            {
                int clamped = Math.Max(_minimum, Math.Min(_maximum, value));
                if (clamped != _value) { _value = clamped; ValueChanged?.Invoke(this, EventArgs.Empty); Invalidate(); }
            }
        }

        public ModernSlider()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Height = 26;
            Width = 100;
            Cursor = Cursors.Hand;

            _fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _fadeTimer.Tick += (s, e) =>
            {
                float target = (_hovering || _dragging) ? 1.0f : 0f;
                float diff = target - _hoverOpacity;
                if (Math.Abs(diff) > 0.02f) { _hoverOpacity += diff * 0.3f; Invalidate(); }
                else { _hoverOpacity = target; _fadeTimer.Stop(); Invalidate(); }
            };
        }

        private int GetTrackLeft() { return THUMB_HOVER_RADIUS + 1; }
        private int GetTrackRight() { return Width - THUMB_HOVER_RADIUS - 1; }

        private int ValueToX()
        {
            if (_maximum <= _minimum) return GetTrackLeft();
            float ratio = (float)(_value - _minimum) / (_maximum - _minimum);
            return GetTrackLeft() + (int)(ratio * (GetTrackRight() - GetTrackLeft()));
        }

        private int XToValue(int x)
        {
            float ratio = (float)(x - GetTrackLeft()) / (GetTrackRight() - GetTrackLeft());
            ratio = Math.Max(0f, Math.Min(1f, ratio));
            return _minimum + (int)(ratio * (_maximum - _minimum));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cy = Height / 2;
            int trackLeft = GetTrackLeft();
            int trackRight = GetTrackRight();
            int thumbX = ValueToX();

            // Track background
            using (var trackPath = CreatePillPath(trackLeft, cy - TRACK_HEIGHT / 2, trackRight - trackLeft, TRACK_HEIGHT))
            using (var trackBrush = new SolidBrush(TrackBg))
                g.FillPath(trackBrush, trackPath);

            // Track fill (left of thumb)
            if (thumbX > trackLeft)
            {
                int fillWidth = thumbX - trackLeft;
                using (var fillPath = CreatePillPath(trackLeft, cy - TRACK_HEIGHT / 2, fillWidth, TRACK_HEIGHT))
                using (var fillBrush = new SolidBrush(TrackFill))
                    g.FillPath(fillBrush, fillPath);
            }

            // Thumb hover glow
            if (_hoverOpacity > 0.01f)
            {
                int glowRadius = THUMB_HOVER_RADIUS + 4;
                using (var glowBrush = new SolidBrush(Color.FromArgb((int)(30 * _hoverOpacity), TrackFill)))
                    g.FillEllipse(glowBrush, thumbX - glowRadius, cy - glowRadius, glowRadius * 2, glowRadius * 2);
            }

            // Thumb
            int thumbR = THUMB_RADIUS + (int)((THUMB_HOVER_RADIUS - THUMB_RADIUS) * _hoverOpacity);
            Color currentThumb = BlendColor(ThumbColor, ThumbHoverColor, _hoverOpacity);
            using (var thumbBrush = new SolidBrush(currentThumb))
                g.FillEllipse(thumbBrush, thumbX - thumbR, cy - thumbR, thumbR * 2, thumbR * 2);

            // Thumb border
            using (var borderPen = new Pen(Color.FromArgb((int)(60 * (1f - _hoverOpacity * 0.5f)), ThumbBorderColor), 1.5f))
                g.DrawEllipse(borderPen, thumbX - thumbR, cy - thumbR, thumbR * 2, thumbR * 2);
        }

        private static Color BlendColor(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private static GraphicsPath CreatePillPath(int x, int y, int width, int height)
        {
            var path = new GraphicsPath();
            if (width <= 0) { path.AddEllipse(x, y, height, height); return path; }
            int r = height;
            path.AddArc(x, y, r, r, 90, 180);
            path.AddArc(x + width - r, y, r, r, 270, 180);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) { _dragging = true; Value = XToValue(e.X); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging) Value = XToValue(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovering = true;
            _fadeTimer.Start();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovering = false;
            if (!_dragging) _fadeTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _fadeTimer?.Stop(); _fadeTimer?.Dispose(); }
            base.Dispose(disposing);
        }
    }

    public FastGalleryPanel(List<ListViewItem> items, int tileWidth, int tileHeight, int spacing, int initialWidth, int initialHeight)
    {
        _originalItems = items ?? new List<ListViewItem>();
        _items = new List<ListViewItem>(_originalItems);
        _displayTiles = new List<GroupedTile>();
        _groupedByPackage = new Dictionary<string, List<ListViewItem>>(StringComparer.OrdinalIgnoreCase);
        _versionRects = new List<Rectangle>();
        _baseTileWidth = tileWidth;
        _baseTileHeight = tileHeight;
        int savedSize = Math.Max(SLIDER_MIN, Math.Min(SLIDER_MAX, SettingsManager.Instance.GalleryTileSize));
        float initScale = savedSize / 100f;
        _tileWidth = Math.Max(80, (int)(tileWidth * initScale));
        _tileHeight = Math.Max(55, (int)(tileHeight * initScale));
        _spacing = spacing;
        _imageCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        _cacheOrder = new Queue<string>();
        _tileStates = new Dictionary<int, TileAnimationState>();
        _sortButtons = new List<Button>();
        RefreshFavoritesCache();

        // Avoid any implicit padding from the control container
        Padding = Padding.Empty;
        Margin = Padding.Empty;

        Size = new Size(initialWidth, initialHeight);

        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.Selectable |
                 ControlStyles.ResizeRedraw, true);

        BackColor = Color.FromArgb(24, 26, 30);

        // Create context menu
        CreateContextMenu();

        // Create sort panel
        _sortPanel = CreateSortPanel();
        Controls.Add(_sortPanel);
        LayoutSliderControls();

        // Scrollbar - direct interaction jumps immediately (no smooth scroll)
        _scrollBar = new VScrollBar { Minimum = 0, SmallChange = _tileHeight / 2, LargeChange = _tileHeight * 2 };
        _scrollBar.Scroll += (s, e) =>
        {
            _scrollY = _scrollBar.Value;
            _targetScrollY = _scrollBar.Value;
            _isScrolling = false;
            Invalidate();
        };
        _scrollBar.HandleCreated += (s, e) => ApplyModernScrollbars();
        Controls.Add(_scrollBar);

        // Animation timer (~120fps)
        _animationTimer = new System.Windows.Forms.Timer { Interval = 8 };
        _animationTimer.Tick += AnimationTimer_Tick;
        _animationTimer.Start();

        // Apply initial sort
        ApplySort();
        RecalculateLayout();
    }

    private string GetBaseGameName(List<ListViewItem> versions)
    {
        if (versions == null || versions.Count == 0) return "";

        // If only one version, use actual name
        if (versions.Count == 1) return versions[0].Text;

        // Strip parentheses (...) from all names - except (MR-Fix), then use the shortest result
        var strippedNames = versions.Select(v =>
        {
            string name = v.Text;
            bool hasMrFix = name.IndexOf("(MR-Fix)", StringComparison.OrdinalIgnoreCase) >= 0;
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\s*\([^)]*\)", "").Trim();
            return hasMrFix ? name + " (MR-Fix)" : name;
        });

        return strippedNames.OrderBy(n => n.Length).First();
    }

    private void BuildGroupedTiles()
    {
        _groupedByPackage.Clear();
        _displayTiles.Clear();

        foreach (var item in _items)
        {
            string packageName = item.SubItems.Count > 2 ? item.SubItems[2].Text : "";
            if (string.IsNullOrEmpty(packageName)) packageName = item.Text;

            if (!_groupedByPackage.ContainsKey(packageName))
                _groupedByPackage[packageName] = new List<ListViewItem>();
            _groupedByPackage[packageName].Add(item);
        }

        foreach (var kvp in _groupedByPackage)
        {
            kvp.Value.Sort((a, b) =>
            {
                var dateA = ParseDate(a.SubItems.Count > 4 ? a.SubItems[4].Text : "");
                var dateB = ParseDate(b.SubItems.Count > 4 ? b.SubItems[4].Text : "");
                return dateB.CompareTo(dateA);
            });

            _displayTiles.Add(new GroupedTile
            {
                PackageName = kvp.Key,
                GameName = kvp.Value[0].Text,
                BaseGameName = GetBaseGameName(kvp.Value),
                Versions = kvp.Value
            });
        }

        SortDisplayTiles();

        _tileStates.Clear();
        for (int i = 0; i < _displayTiles.Count; i++)
            _tileStates[i] = new TileAnimationState();
    }

    private void SortDisplayTiles()
    {
        switch (_currentSortField)
        {
            case SortField.Name:
                _displayTiles = _currentSortDirection == SortDirection.Ascending
                    ? _displayTiles.OrderBy(t => t.BaseGameName, new GameNameComparer()).ToList()
                    : _displayTiles.OrderByDescending(t => t.BaseGameName, new GameNameComparer()).ToList();
                break;
            case SortField.LastUpdated:
                _displayTiles = _currentSortDirection == SortDirection.Ascending
                    ? _displayTiles.OrderBy(t => t.Versions.Max(v => ParseDate(v.SubItems.Count > 4 ? v.SubItems[4].Text : ""))).ToList()
                    : _displayTiles.OrderByDescending(t => t.Versions.Max(v => ParseDate(v.SubItems.Count > 4 ? v.SubItems[4].Text : ""))).ToList();
                break;
            case SortField.Size:
                _displayTiles = _currentSortDirection == SortDirection.Ascending
                    ? _displayTiles.OrderBy(t => t.Versions.Max(v => ParseSize(v.SubItems.Count > 5 ? v.SubItems[5].Text : "0"))).ToList()
                    : _displayTiles.OrderByDescending(t => t.Versions.Max(v => ParseSize(v.SubItems.Count > 5 ? v.SubItems[5].Text : "0"))).ToList();
                break;
            case SortField.Popularity:
                if (_currentSortDirection == SortDirection.Ascending)
                    _displayTiles = _displayTiles.OrderByDescending(t => ParsePopularity(t.Primary.SubItems.Count > 6 ? t.Primary.SubItems[6].Text : "-"))
                                   .ThenBy(t => t.BaseGameName, new GameNameComparer()).ToList();
                else
                    _displayTiles = _displayTiles.OrderBy(t => ParsePopularity(t.Primary.SubItems.Count > 6 ? t.Primary.SubItems[6].Text : "-"))
                                   .ThenBy(t => t.BaseGameName, new GameNameComparer()).ToList();
                break;
        }
    }

    private Panel CreateSortPanel()
    {
        var panel = new Panel
        {
            Height = SORT_PANEL_HEIGHT,
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(28, 30, 34),
            Padding = new Padding(8, 4, 8, 4)
        };

        var label = new Label
        {
            Text = "Sort by:",
            ForeColor = Color.FromArgb(180, 180, 180),
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            Location = new Point(10, 9)
        };
        panel.Controls.Add(label);

        int buttonX = 70;
        SortField[] fields = { SortField.Name, SortField.LastUpdated, SortField.Size, SortField.Popularity };
        string[] texts = { "Name", "Updated", "Size", "Popularity" };

        for (int i = 0; i < fields.Length; i++)
        {
            var btn = CreateSortButton(texts[i], fields[i], buttonX);
            panel.Controls.Add(btn);
            _sortButtons.Add(btn);
            buttonX += btn.Width + 6;
        }

        _sortStatusLabel = new Label
        {
            Text = GetSortStatusText(),
            ForeColor = Color.FromArgb(140, 140, 140),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
            AutoSize = true,
            Location = new Point(buttonX + 10, 9)
        };
        panel.Controls.Add(_sortStatusLabel);

        // Tile size slider (right-aligned)
        _sizeLabel = new Label
        {
            Text = "Tile Size",
            ForeColor = Color.FromArgb(140, 140, 140),
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        panel.Controls.Add(_sizeLabel);

        _sizeSlider = new ModernSlider
        {
            Minimum = SLIDER_MIN,
            Maximum = SLIDER_MAX,
            Value = Math.Max(SLIDER_MIN, Math.Min(SLIDER_MAX, SettingsManager.Instance.GalleryTileSize)),
            Height = 22,
            Width = 100,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        _sizeSlider.ValueChanged += SizeSlider_ValueChanged;
        panel.Controls.Add(_sizeSlider);

        // Position slider controls on the right side
        panel.Resize += (s, ev) => LayoutSliderControls();

        UpdateSortButtonStyles();
        return panel;
    }

    private string GetSortStatusText()
    {
        switch (_currentSortField)
        {
            case SortField.Name: return _currentSortDirection == SortDirection.Ascending ? "A → Z" : "Z → A";
            case SortField.LastUpdated: return _currentSortDirection == SortDirection.Ascending ? "Oldest → Newest" : "Newest → Oldest";
            case SortField.Size: return _currentSortDirection == SortDirection.Ascending ? "Smallest → Largest" : "Largest → Smallest";
            case SortField.Popularity: return _currentSortDirection == SortDirection.Ascending ? "Least → Most Popular" : "Most → Least Popular";
            default: return "";
        }
    }

    private Button CreateSortButton(string text, SortField field, int x)
    {
        var btn = new Button
        {
            Text = field == _currentSortField ? GetSortButtonText(text) : text,
            Tag = field,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.White,
            BackColor = SortButtonBg,
            Size = new Size(text == "Popularity" ? 90 : 75, 26),
            Location = new Point(x, 5),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = SortButtonHoverBg;
        btn.FlatAppearance.MouseDownBackColor = SortButtonActiveBg;
        btn.Click += (s, e) => OnSortButtonClick(field);
        return btn;
    }

    private string GetSortButtonText(string baseText)
    {
        return baseText + (_currentSortDirection == SortDirection.Ascending ? " ▲" : " ▼");
    }

    private void OnSortButtonClick(SortField field)
    {
        if (_currentSortField == field)
            // Toggle direction
            _currentSortDirection = _currentSortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
        else
        {
            _currentSortField = field;
            // Popularity, LastUpdated, Size default to descending (most popular/newest/largest first)
            // Name defaults to ascending (A-Z)
            _currentSortDirection = field == SortField.Name ? SortDirection.Ascending : SortDirection.Descending;
        }
        UpdateSortButtonStyles();
        ApplySort();
        SortChanged?.Invoke(this, field);
    }

    private void UpdateSortButtonStyles()
    {
        foreach (var btn in _sortButtons)
        {
            var field = (SortField)btn.Tag;
            bool isActive = field == _currentSortField;
            string baseText = field == SortField.Name ? "Name" : field == SortField.LastUpdated ? "Updated" : field == SortField.Size ? "Size" : "Popularity";

            btn.Text = isActive ? GetSortButtonText(baseText) : baseText;
            // Set appropriate hover color based on active state
            btn.BackColor = isActive ? SortButtonActiveBg : SortButtonBg;
            btn.ForeColor = isActive ? Color.FromArgb(24, 26, 30) : Color.White;
            btn.FlatAppearance.MouseOverBackColor = isActive ? Color.FromArgb(110, 215, 190) : SortButtonHoverBg;
            btn.FlatAppearance.MouseDownBackColor = isActive ? Color.FromArgb(80, 180, 155) : SortButtonActiveBg;
        }

        // Update the sort status label
        if (_sortStatusLabel != null) _sortStatusLabel.Text = GetSortStatusText();
    }

    private void LayoutSliderControls()
    {
        if (_sortPanel == null || _sizeSlider == null || _sizeLabel == null) return;
        int panelWidth = _sortPanel.ClientSize.Width;
        _sizeSlider.Location = new Point(panelWidth - _sizeSlider.Width - 8, 5);
        _sizeLabel.Location = new Point(_sizeSlider.Left - _sizeLabel.Width - 4, 9);
    }

    private void SizeSlider_ValueChanged(object sender, EventArgs e)
    {
        float scale = _sizeSlider.Value / 100f;
        _tileWidth = Math.Max(80, (int)(_baseTileWidth * scale));
        _tileHeight = Math.Max(55, (int)(_baseTileHeight * scale));

        SettingsManager.Instance.GalleryTileSize = _sizeSlider.Value;
        SettingsManager.Instance.Save();

        CloseOverlay();
        RecalculateLayout();
        Invalidate();
    }

    private void ApplySort()
    {
        // Reset original order
        _items = new List<ListViewItem>(_originalItems);

        // Reset selection and hover
        _hoveredIndex = -1;
        _selectedIndex = -1;
        _selectedItem = null;

        CloseOverlay();
        BuildGroupedTiles();

        // Reset scroll position
        _scrollY = 0;
        _targetScrollY = 0;

        RecalculateLayout();
        Invalidate();
    }

    private void CloseOverlay()
    {
        _expandedTileIndex = -1;
        _targetExpandOverlayOpacity = 0f;
        _overlayHoveredVersion = -1;
        _overlayScrollOffset = 0;
        _rightClickedVersionIndex = -1;
    }

    public void SetSortState(SortField field, SortDirection direction)
    {
        _currentSortField = field;
        _currentSortDirection = direction;
        UpdateSortButtonStyles();
        ApplySort();
    }

    private int ParsePopularity(string popStr)
    {
        if (string.IsNullOrEmpty(popStr)) return int.MaxValue; // Unranked goes to end
        popStr = popStr.Trim();
        if (popStr == "-") return int.MaxValue; // Unranked goes to end
        if (popStr.StartsWith("#") && int.TryParse(popStr.Substring(1), out int rank)) return rank;
        if (int.TryParse(popStr, out int rawNum)) return rawNum; // Fallback: try parsing as raw number
        return int.MaxValue; // Unparseable goes to end
    }

    // Custom sort to match list sort behaviour: '_' before digits, digits before letters (case-insensitive)
    private class GameNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int minLen = Math.Min(x.Length, y.Length);
            for (int i = 0; i < minLen; i++)
            {
                int orderX = GetCharOrder(x[i]);
                int orderY = GetCharOrder(y[i]);
                if (orderX != orderY) return orderX.CompareTo(orderY);

                // Same category, compare case-insensitively
                int cmp = char.ToLowerInvariant(x[i]).CompareTo(char.ToLowerInvariant(y[i]));
                if (cmp != 0) return cmp;
            }

            return x.Length.CompareTo(y.Length); // Shorter string comes first
        }

        private static int GetCharOrder(char c)
        {
            // Order: underscore (0), digits (1), letters (2), everything else (3)
            if (c == '_') return 0;
            if (char.IsDigit(c)) return 1;
            if (char.IsLetter(c)) return 2;
            return 3;
        }
    }

    private DateTime ParseDate(string dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return DateTime.MinValue;
        string[] formats = { "yyyy-MM-dd HH:mm 'UTC'", "yyyy-MM-dd HH:mm" };
        return DateTime.TryParseExact(dateStr, formats, System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
            out DateTime date) ? date : DateTime.MinValue;
    }

    private double ParseSize(string sizeStr)
    {
        if (string.IsNullOrEmpty(sizeStr)) return 0;
        sizeStr = sizeStr.Trim(); // Remove whitespace

        // Handle new format: "1.23 GB" or "123 MB"
        if (sizeStr.EndsWith(" GB", StringComparison.OrdinalIgnoreCase))
        {
            if (double.TryParse(sizeStr.Substring(0, sizeStr.Length - 3).Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double gb)) return gb * 1024.0; // Convert GB to MB for consistent sorting
        }
        else if (sizeStr.EndsWith(" MB", StringComparison.OrdinalIgnoreCase))
        {
            if (double.TryParse(sizeStr.Substring(0, sizeStr.Length - 3).Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double mb)) return mb;
        }

        // Fallback: try parsing as raw number
        if (double.TryParse(sizeStr, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out double raw)) return raw;

        return 0;
    }

    public void UpdateItems(List<ListViewItem> newItems)
    {
        if (newItems == null) newItems = new List<ListViewItem>();
        _originalItems = new List<ListViewItem>(newItems);
        _items = new List<ListViewItem>(newItems);

        // Reset selection and hover states
        _hoveredIndex = -1;
        _selectedIndex = -1;
        _selectedItem = null;
        _isHoveringDeleteButton = false;

        CloseOverlay();

        // Reset scroll position for new results
        _scrollY = 0;
        _targetScrollY = 0;
        _isScrolling = false;

        // Refresh favorites cache and re-apply sort
        RefreshFavoritesCache();
        ApplySort();
    }

    public ListViewItem GetItemAtIndex(int index)
    {
        if (_selectedItem != null) return _selectedItem;
        if (index >= 0 && index < _displayTiles.Count)
            return _displayTiles[index].Primary;
        return null;
    }

    private bool IsItemInstalled(ListViewItem item)
    {
        if (item == null) return false;
        return item.ForeColor.ToArgb() == MainForm.ColorInstalled.ToArgb() ||
               item.ForeColor.ToArgb() == MainForm.ColorUpdateAvailable.ToArgb() ||
               item.ForeColor.ToArgb() == MainForm.ColorDonateGame.ToArgb();
    }

    private bool IsAnyVersionInstalled(GroupedTile tile)
    {
        return tile.Versions.Any(v => IsItemInstalled(v));
    }

    private Rectangle GetDeleteButtonRect(int index, int row, int col, int scrollY)
    {
        if (!_tileStates.TryGetValue(index, out var state)) state = new TileAnimationState();

        int baseX = _leftPadding + col * (_tileWidth + _spacing);
        int baseY = _spacing + SORT_PANEL_HEIGHT + row * (_tileHeight + _spacing) - scrollY;

        float scale = state.Scale;
        int scaledW = (int)(_tileWidth * scale);
        int scaledH = (int)(_tileHeight * scale);
        int x = baseX - (scaledW - _tileWidth) / 2;
        int y = baseY - (scaledH - _tileHeight) / 2;

        // Position delete button in bottom-right corner of thumbnail
        int btnX = x + scaledW - DELETE_BUTTON_SIZE - 2 - DELETE_BUTTON_MARGIN;
        int btnY = y + 2 + scaledH - DELETE_BUTTON_SIZE - DELETE_BUTTON_MARGIN - 20;
        return new Rectangle(btnX, btnY, DELETE_BUTTON_SIZE, DELETE_BUTTON_SIZE);
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        bool needsRedraw = false;

        // Smooth scrolling
        if (_isScrolling)
        {
            float diff = _targetScrollY - _scrollY;
            if (Math.Abs(diff) > 0.5f)
            {
                _scrollY += diff * SCROLL_SMOOTHING;
                _scrollY = Math.Max(0, Math.Min(_scrollY, Math.Max(0, _contentHeight - (Height - SORT_PANEL_HEIGHT))));
                if (_scrollBar.Visible && _scrollBar.Value != (int)_scrollY)
                    _scrollBar.Value = Math.Max(_scrollBar.Minimum, Math.Min(_scrollBar.Maximum - _scrollBar.LargeChange + 1, (int)_scrollY));
                needsRedraw = true;
            }
            else { _scrollY = _targetScrollY; _isScrolling = false; }
        }

        if (Math.Abs(_expandOverlayOpacity - _targetExpandOverlayOpacity) > 0.01f)
        {
            _expandOverlayOpacity += (_targetExpandOverlayOpacity - _expandOverlayOpacity) * 0.4f;
            needsRedraw = true;
        }
        else _expandOverlayOpacity = _targetExpandOverlayOpacity;

        // Update overlay hover state based on current mouse position
        if (_expandedTileIndex >= 0 && _expandOverlayOpacity > 0.5f && _versionRects.Count > 0)
        {
            var mousePos = PointToClient(Cursor.Position);
            int newHover = GetOverlayVersionAtPoint(mousePos.X, mousePos.Y);
            if (newHover != _overlayHoveredVersion)
            {
                _overlayHoveredVersion = newHover;
                needsRedraw = true;

                // Update release notes when hovering over a version
                if (newHover >= 0 && newHover < _displayTiles[_expandedTileIndex].Versions.Count)
                {
                    var hoveredVersion = _displayTiles[_expandedTileIndex].Versions[newHover];
                    string releaseName = hoveredVersion.SubItems.Count > 1 ? hoveredVersion.SubItems[1].Text : "";
                    TileHovered?.Invoke(this, releaseName);
                }
            }
        }

        // Tile animations - only process visible tiles for performance
        int scrollYInt = (int)_scrollY;
        int startRow = Math.Max(0, (scrollYInt - _spacing - _tileHeight) / (_tileHeight + _spacing));
        int endRow = Math.Min(_rows - 1, (scrollYInt + Height + _tileHeight) / (_tileHeight + _spacing));

        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                int index = row * _columns + col;
                if (index >= _displayTiles.Count) break;

                if (!_tileStates.TryGetValue(index, out var state))
                {
                    state = new TileAnimationState();
                    _tileStates[index] = state;
                }

                var tile = _displayTiles[index];
                bool isHovered = index == _hoveredIndex && _expandedTileIndex < 0;
                bool isSelected = index == _selectedIndex;
                bool isInstalled = IsAnyVersionInstalled(tile);
                string pkgName = tile.Primary.SubItems.Count > 1 ? tile.Primary.SubItems[1].Text : "";
                bool isFavorite = _favoritesCache.Contains(pkgName);

                state.TargetFavoriteOpacity = isFavorite ? 1.0f : 0f;
                state.TargetScale = isHovered ? HOVER_SCALE : 1.0f;
                state.TargetBorderOpacity = isHovered ? 1.0f : 0f;
                state.TargetBackgroundBrightness = isHovered ? 45f : (isSelected ? 38f : 30f);
                state.TargetSelectionOpacity = isSelected ? 1.0f : 0f;
                state.TargetTooltipOpacity = isHovered ? 1.0f : 0f;
                state.TargetDeleteButtonOpacity = (isHovered && isInstalled) ? 1.0f : 0f;
                state.TargetGroupBadgeOpacity = tile.Versions.Count > 1 ? 1.0f : 0f;

                needsRedraw |= AnimateValue(ref state.Scale, state.TargetScale, ANIMATION_SPEED, 0.001f);
                needsRedraw |= AnimateValue(ref state.BorderOpacity, state.TargetBorderOpacity, ANIMATION_SPEED, 0.01f);
                needsRedraw |= AnimateValue(ref state.BackgroundBrightness, state.TargetBackgroundBrightness, ANIMATION_SPEED, 0.5f);
                needsRedraw |= AnimateValue(ref state.SelectionOpacity, state.TargetSelectionOpacity, ANIMATION_SPEED, 0.01f);
                needsRedraw |= AnimateValue(ref state.TooltipOpacity, state.TargetTooltipOpacity, 0.35f, 0.01f);
                needsRedraw |= AnimateValue(ref state.DeleteButtonOpacity, state.TargetDeleteButtonOpacity, 0.35f, 0.01f);
                needsRedraw |= AnimateValue(ref state.FavoriteOpacity, state.TargetFavoriteOpacity, 0.35f, 0.01f);
                needsRedraw |= AnimateValue(ref state.GroupBadgeOpacity, state.TargetGroupBadgeOpacity, 0.35f, 0.01f);
            }
        }

        if (needsRedraw) Invalidate();
    }

    private bool AnimateValue(ref float current, float target, float speed, float threshold)
    {
        if (Math.Abs(current - target) > threshold)
        {
            current += (target - current) * speed;
            return true;
        }
        current = target;
        return false;
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (height <= 0 && Height > 0) height = Height;
        if (width <= 0 && Width > 0) width = Width;
        base.SetBoundsCore(x, y, width, height, specified);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (Width > 0 && Height > 0 && _scrollBar != null) { RecalculateLayout(); Refresh(); }
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (Parent != null && !IsDisposed && !Disposing) RecalculateLayout();
    }

    private void RecalculateLayout()
    {
        if (IsDisposed || Disposing || _scrollBar == null || Width <= 0 || Height <= 0) return;

        int availableHeight = Height - SORT_PANEL_HEIGHT;
        _scrollBar.SetBounds(Width - _scrollBar.Width, SORT_PANEL_HEIGHT, _scrollBar.Width, availableHeight);

        int availableWidth = Width - _scrollBar.Width - _spacing * 2;
        _columns = Math.Max(1, (availableWidth + _spacing) / (_tileWidth + _spacing));
        _rows = (int)Math.Ceiling((double)_displayTiles.Count / _columns);
        _contentHeight = _rows * (_tileHeight + _spacing) + _spacing + 20;

        int usedWidth = _columns * (_tileWidth + _spacing) - _spacing;
        _leftPadding = Math.Max(_spacing, (availableWidth - usedWidth) / 2 + _spacing);

        _scrollBar.Maximum = Math.Max(0, _contentHeight);
        _scrollBar.LargeChange = Math.Max(1, availableHeight);
        _scrollBar.Visible = _contentHeight > availableHeight;

        _scrollY = Math.Max(0, Math.Min(_scrollY, Math.Max(0, _contentHeight - availableHeight)));
        _targetScrollY = _scrollY;
        if (_scrollBar.Visible) _scrollBar.Value = (int)_scrollY;

        if (_backBuffer == null || _backBuffer.Width != Width || _backBuffer.Height != Height)
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(Math.Max(1, Width), Math.Max(1, Height));
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_backBuffer == null) return;

        using (var g = Graphics.FromImage(_backBuffer))
        {
            g.Clear(BackColor);

            // Fill sort panel area
            g.FillRectangle(new SolidBrush(Color.FromArgb(28, 30, 34)), 0, 0, Width, SORT_PANEL_HEIGHT);

            g.SmoothingMode = _isScrolling ? SmoothingMode.HighSpeed : SmoothingMode.AntiAlias;
            g.InterpolationMode = _isScrolling ? InterpolationMode.Low : InterpolationMode.Bilinear;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clip to tile area (below sort panel)
            g.SetClip(new Rectangle(0, SORT_PANEL_HEIGHT, Width, Height - SORT_PANEL_HEIGHT));

            int scrollYInt = (int)_scrollY;
            int startRow = Math.Max(0, (scrollYInt - _spacing - _tileHeight) / (_tileHeight + _spacing));
            int endRow = Math.Min(_rows - 1, (scrollYInt + Height + _tileHeight) / (_tileHeight + _spacing));

            // Draw non-hovered, non-selected tiles first
            for (int row = startRow; row <= endRow; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    int index = row * _columns + col;
                    if (index >= _displayTiles.Count) break;
                    if (index != _hoveredIndex && index != _selectedIndex)
                        DrawTile(g, index, row, col, scrollYInt);
                }
            }

            // Draw selected tile
            if (_selectedIndex >= 0 && _selectedIndex < _displayTiles.Count && _selectedIndex != _hoveredIndex)
            {
                int selectedRow = _selectedIndex / _columns;
                int selectedCol = _selectedIndex % _columns;
                if (selectedRow >= startRow && selectedRow <= endRow)
                    DrawTile(g, _selectedIndex, selectedRow, selectedCol, scrollYInt);
            }

            // Draw hovered tile last (on top)
            if (_hoveredIndex >= 0 && _hoveredIndex < _displayTiles.Count)
            {
                int hoveredRow = _hoveredIndex / _columns;
                int hoveredCol = _hoveredIndex % _columns;
                if (hoveredRow >= startRow && hoveredRow <= endRow)
                    DrawTile(g, _hoveredIndex, hoveredRow, hoveredCol, scrollYInt);
            }

            if (_expandedTileIndex >= 0 && _expandOverlayOpacity > 0.01f)
                DrawGroupOverlay(g, scrollYInt);

            g.ResetClip();
        }
        e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
    }

    private ulong ParseCloudVersionCode(ListViewItem item)
    {
        if (item == null || item.SubItems.Count <= 3) return 0;
        string versionText = item.SubItems[3].Text;

        int startIdx = versionText.IndexOf('v');
        if (startIdx < 0) return 0;

        int endIdx = versionText.IndexOf(' ', startIdx);
        string cloudPart = endIdx > 0
            ? versionText.Substring(startIdx + 1, endIdx - startIdx - 1)
            : versionText.Substring(startIdx + 1);

        ulong.TryParse(StringUtilities.KeepOnlyNumbers(cloudPart), out ulong result);
        return result;
    }

    private ulong ParseInstalledVersionCode(ListViewItem item)
    {
        if (item == null || item.SubItems.Count <= 3) return 0;
        string versionText = item.SubItems[3].Text;

        int dividerIdx = versionText.IndexOf(" / v");
        if (dividerIdx < 0) return 0;

        string installedPart = versionText.Substring(dividerIdx + 4);
        ulong.TryParse(StringUtilities.KeepOnlyNumbers(installedPart), out ulong result);
        return result;
    }

    private void DrawGroupOverlay(Graphics g, int scrollY)
    {
        if (_expandedTileIndex < 0 || _expandedTileIndex >= _displayTiles.Count) return;

        var tile = _displayTiles[_expandedTileIndex];
        if (tile.Versions.Count <= 1) return;

        // Find installed version code from any version of this package
        ulong installedVersionCode = 0;
        foreach (var v in tile.Versions)
        {
            ulong installed = ParseInstalledVersionCode(v);
            if (installed > 0) { installedVersionCode = installed; break; }
        }

        int row = _expandedTileIndex / _columns;
        int col = _expandedTileIndex % _columns;
        int baseX = _leftPadding + col * (_tileWidth + _spacing);
        int baseY = _spacing + SORT_PANEL_HEIGHT + row * (_tileHeight + _spacing) - scrollY;
        int tileCenterX = baseX + _tileWidth / 2;
        int tileCenterY = baseY + _tileHeight / 2;

        int overlayWidth = Math.Max(300, _tileWidth + 60);
        int headerHeight = 28;
        int contentHeight = tile.Versions.Count * VERSION_ROW_HEIGHT;

        int maxContentArea = OVERLAY_MAX_HEIGHT - headerHeight - OVERLAY_PADDING;
        bool needsScroll = contentHeight > maxContentArea;
        int visibleContentHeight = needsScroll ? maxContentArea : contentHeight;
        int overlayHeight = headerHeight + visibleContentHeight + OVERLAY_PADDING;

        _overlayMaxScroll = needsScroll ? contentHeight - visibleContentHeight : 0;
        _overlayScrollOffset = Math.Max(0, Math.Min(_overlayScrollOffset, _overlayMaxScroll));

        int overlayX = tileCenterX - overlayWidth / 2;
        int overlayY = tileCenterY - overlayHeight / 2;

        overlayX = Math.Max(10, Math.Min(overlayX, Width - overlayWidth - _scrollBar.Width - 10));
        overlayY = Math.Max(SORT_PANEL_HEIGHT + 10, Math.Min(overlayY, Height - overlayHeight - 10));

        _overlayRect = new Rectangle(overlayX, overlayY, overlayWidth, overlayHeight);

        int alpha = (int)(255 * _expandOverlayOpacity);

        // Dim background
        using (var dimBrush = new SolidBrush(Color.FromArgb((int)(120 * _expandOverlayOpacity), 0, 0, 0)))
            g.FillRectangle(dimBrush, 0, SORT_PANEL_HEIGHT, Width, Height - SORT_PANEL_HEIGHT);

        // Shadow
        using (var shadowPath = CreateRoundedRectangle(new Rectangle(overlayX + 4, overlayY + 4, overlayWidth, overlayHeight), 12))
        using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(80 * _expandOverlayOpacity), 0, 0, 0)))
            g.FillPath(shadowBrush, shadowPath);

        // Main overlay
        using (var overlayPath = CreateRoundedRectangle(_overlayRect, 12))
        using (var bgBrush = new SolidBrush(Color.FromArgb(alpha, OverlayBgColor.R, OverlayBgColor.G, OverlayBgColor.B)))
        using (var borderPen = new Pen(Color.FromArgb(alpha, TileBorderHover), 2f))
        {
            g.FillPath(bgBrush, overlayPath);
            g.DrawPath(borderPen, overlayPath);
        }

        // Header
        using (var headerFont = new Font("Segoe UI", 9f))
        using (var headerBrush = new SolidBrush(Color.FromArgb((int)(alpha * 0.7), 200, 200, 200)))
        {
            g.DrawString($"Select a version ({tile.Versions.Count} available)", headerFont, headerBrush,
                overlayX + OVERLAY_PADDING, overlayY + OVERLAY_PADDING);
        }

        // Clip for scrollable content
        var contentRect = new Rectangle(overlayX, overlayY + headerHeight, overlayWidth, visibleContentHeight);
        var oldClip = g.Clip;
        g.SetClip(contentRect, CombineMode.Intersect);

        // Version rows
        _versionRects.Clear();
        int yOffset = overlayY + headerHeight - _overlayScrollOffset;

        using (var nameFont = new Font("Segoe UI Semibold", 9f))
        using (var nameFontBold = new Font("Segoe UI", 9f, FontStyle.Bold))
        using (var detailFont = new Font("Segoe UI", 8.5f))
        using (var detailFontBold = new Font("Segoe UI", 8.5f, FontStyle.Bold))
        {
            for (int i = 0; i < tile.Versions.Count; i++)
            {
                var version = tile.Versions[i];
                int rowWidth = needsScroll ? overlayWidth - 20 : overlayWidth - 12;
                var rowRect = new Rectangle(overlayX + 6, yOffset, rowWidth, VERSION_ROW_HEIGHT - 4);

                var clickRect = new Rectangle(rowRect.X, Math.Max(rowRect.Y, contentRect.Y),
                    rowRect.Width, Math.Min(rowRect.Bottom, contentRect.Bottom) - Math.Max(rowRect.Y, contentRect.Y));
                _versionRects.Add(clickRect.Height > 0 ? clickRect : Rectangle.Empty);

                bool isHovered = i == _overlayHoveredVersion;

                // Determine version status by comparing version codes
                ulong thisVersionCode = ParseCloudVersionCode(version);
                bool isExactInstalled = installedVersionCode > 0 && thisVersionCode == installedVersionCode;
                bool isNewerThanInstalled = installedVersionCode > 0 && thisVersionCode > installedVersionCode;
                bool isOlderThanInstalled = installedVersionCode > 0 && thisVersionCode < installedVersionCode;
                bool isDownloaded = version.SubItems.Count > 1 && MainForm.DownloadedReleaseNames.Contains(version.SubItems[1].Text);

                if (isDownloaded && rowRect.IntersectsWith(contentRect))
                {
                    using (var dlPath = CreateRoundedRectangle(rowRect, 6))
                    using (var dlBrush = new SolidBrush(Color.FromArgb(alpha, 27, 41, 44)))
                        g.FillPath(dlBrush, dlPath);
                }

                if (isHovered && rowRect.IntersectsWith(contentRect))
                {
                    using (var hoverPath = CreateRoundedRectangle(rowRect, 6))
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(alpha, isDownloaded ? Color.FromArgb(45, 64, 68) : VersionRowHoverBg)))
                        g.FillPath(hoverBrush, hoverPath);
                }

                int textX = rowRect.X + 8;
                int textY = rowRect.Y + 4;

                // Check if this version is a favorite
                string versionPkgName = version.SubItems.Count > 1 ? version.SubItems[1].Text : "";
                bool isFavorite = !string.IsNullOrEmpty(versionPkgName) && _favoritesCache.Contains(versionPkgName);

                // Draw favorite star badge on the left
                if (isFavorite)
                {
                    int starSize = 8;
                    int starX = textX - 5 + starSize / 2;
                    int starY = rowRect.Y + rowRect.Height / 2;

                    using (var starPath = CreateStarPath(starX, starY, starSize / 2, starSize / 4, 5))
                    using (var starBrush = new SolidBrush(Color.FromArgb(alpha, TileBorderFavorite)))
                        g.FillPath(starBrush, starPath);

                    textX += 6; // Shift text right to accommodate star
                }

                // Determine badge width for name rect
                int badgeWidth = isExactInstalled ? 68 : (isNewerThanInstalled ? 58 : (isOlderThanInstalled ? 52 : 8));

                // Game name - bold font when hovered
                using (var nameBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                {
                    int extraPadding = isFavorite ? 3 : 0;
                    var nameRect = new Rectangle(textX, textY, rowRect.Width - badgeWidth - 8 - extraPadding, 18);
                    var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
                    g.DrawString(version.Text, isHovered ? nameFontBold : nameFont, nameBrush, nameRect, sf);
                }

                // Size, date, version - bold font when hovered
                string size = version.SubItems.Count > 5 ? version.SubItems[5].Text : "";
                string date = FormatLastUpdated(version.SubItems.Count > 4 ? version.SubItems[4].Text : "");
                ulong versionNum = ParseCloudVersionCode(version);
                string versionCode = versionNum > 0 ? "v" + versionNum : "";
                string details = string.Join(" • ", new[] { size, date, versionCode }.Where(s => !string.IsNullOrEmpty(s)));

                Color detailColor = Color.FromArgb(180, 180, 180);
                using (var detailBrush = new SolidBrush(Color.FromArgb((int)(alpha * (isHovered ? 1.0 : 0.6)), detailColor)))
                    g.DrawString(details, isHovered ? detailFontBold : detailFont, detailBrush, textX, textY + 18);

                // Status badge
                if (isExactInstalled)
                {
                    var badgeRect = new Rectangle(rowRect.Right - 68, rowRect.Y + (rowRect.Height - 18) / 2, 60, 18);
                    using (var badgePath = CreateRoundedRectangle(badgeRect, 4))
                    using (var badgeBrush = new SolidBrush(Color.FromArgb(alpha, BadgeInstalledBg)))
                    {
                        g.FillPath(badgeBrush, badgePath);
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        using (var textBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                            g.DrawString("INSTALLED", new Font("Segoe UI", 6.5f, FontStyle.Bold), textBrush, badgeRect, sf);
                    }
                }
                else if (isNewerThanInstalled)
                {
                    var badgeRect = new Rectangle(rowRect.Right - 58, rowRect.Y + (rowRect.Height - 18) / 2, 50, 18);
                    using (var badgePath = CreateRoundedRectangle(badgeRect, 4))
                    using (var badgeBrush = new SolidBrush(Color.FromArgb(alpha, MainForm.ColorUpdateAvailable)))
                    {
                        g.FillPath(badgeBrush, badgePath);
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        using (var textBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                            g.DrawString("NEWER", new Font("Segoe UI", 6.5f, FontStyle.Bold), textBrush, badgeRect, sf);
                    }
                }
                else if (isOlderThanInstalled)
                {
                    var badgeRect = new Rectangle(rowRect.Right - 52, rowRect.Y + (rowRect.Height - 18) / 2, 44, 18);
                    using (var badgePath = CreateRoundedRectangle(badgeRect, 4))
                    using (var badgeBrush = new SolidBrush(Color.FromArgb((int)(alpha * 0.5), 100, 100, 100)))
                    {
                        g.FillPath(badgeBrush, badgePath);
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        using (var textBrush = new SolidBrush(Color.FromArgb((int)(alpha * 0.7), 180, 180, 180)))
                            g.DrawString("OLDER", new Font("Segoe UI", 6.5f, FontStyle.Bold), textBrush, badgeRect, sf);
                    }
                }

                yOffset += VERSION_ROW_HEIGHT;
            }
        }

        g.Clip = oldClip;

        // Scroll indicator
        if (needsScroll)
        {
            int scrollBarX = overlayX + overlayWidth - 10;
            int scrollTrackY = overlayY + headerHeight + 4;
            int scrollTrackHeight = visibleContentHeight - 8;

            using (var trackBrush = new SolidBrush(Color.FromArgb((int)(40 * _expandOverlayOpacity), 255, 255, 255)))
                g.FillRectangle(trackBrush, scrollBarX, scrollTrackY, 4, scrollTrackHeight);

            float scrollRatio = (float)_overlayScrollOffset / _overlayMaxScroll;
            float thumbRatio = (float)visibleContentHeight / contentHeight;
            int thumbHeight = Math.Max(20, (int)(scrollTrackHeight * thumbRatio));
            int thumbY = scrollTrackY + (int)((scrollTrackHeight - thumbHeight) * scrollRatio);

            using (var thumbPath = CreateRoundedRectangle(new Rectangle(scrollBarX, thumbY, 4, thumbHeight), 2))
            using (var thumbBrush = new SolidBrush(Color.FromArgb((int)(120 * _expandOverlayOpacity), TileBorderHover)))
                g.FillPath(thumbBrush, thumbPath);
        }
    }

    private GraphicsPath CreateStarPath(float cx, float cy, float outerRadius, float innerRadius, int points)
    {
        var path = new GraphicsPath();
        var starPoints = new PointF[points * 2];
        double angleStep = Math.PI / points;
        double startAngle = -Math.PI / 2; // Start from top

        for (int i = 0; i < points * 2; i++)
        {
            double angle = startAngle + i * angleStep;
            float radius = (i % 2 == 0) ? outerRadius : innerRadius;
            starPoints[i] = new PointF(
                cx + (float)(radius * Math.Cos(angle)),
                cy + (float)(radius * Math.Sin(angle))
            );
        }

        path.AddPolygon(starPoints);
        return path;
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void DrawTile(Graphics g, int index, int row, int col, int scrollY)
    {
        var tile = _displayTiles[index];
        var item = tile.Primary;
        var state = _tileStates.ContainsKey(index) ? _tileStates[index] : new TileAnimationState();
        bool isHovered = index == _hoveredIndex && _expandedTileIndex < 0;

        int baseX = _leftPadding + col * (_tileWidth + _spacing);
        int baseY = _spacing + SORT_PANEL_HEIGHT + row * (_tileHeight + _spacing) - scrollY;

        float scale = state.Scale;
        int scaledW = (int)(_tileWidth * scale);
        int scaledH = (int)(_tileHeight * scale);
        int x = baseX - (scaledW - _tileWidth) / 2;
        int y = baseY - (scaledH - _tileHeight) / 2;

        var tileRect = new Rectangle(x, y, scaledW, scaledH);
        var thumbnail = GetCachedImage(tile.PackageName);

        using (var tilePath = CreateRoundedRectangle(tileRect, THUMB_CORNER_RADIUS))
        {
            var oldClip = g.Clip;
            g.SetClip(tilePath, CombineMode.Replace);

            if (thumbnail != null)
            {
                if (isHovered) g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                float imgRatio = (float)thumbnail.Width / thumbnail.Height;
                float rectRatio = (float)tileRect.Width / tileRect.Height;
                Rectangle drawRect = imgRatio > rectRatio
                    ? new Rectangle(x - ((int)(scaledH * imgRatio) - scaledW) / 2, y, (int)(scaledH * imgRatio), scaledH)
                    : new Rectangle(x, y - ((int)(scaledW / imgRatio) - scaledH) / 2, scaledW, (int)(scaledW / imgRatio));
                g.DrawImage(thumbnail, drawRect);
                if (isHovered) g.InterpolationMode = InterpolationMode.Bilinear;
            }
            else
            {
                using (var brush = new SolidBrush(Color.FromArgb(35, 35, 40)))
                    g.FillPath(brush, tilePath);
                using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
                using (var text = new SolidBrush(Color.FromArgb(110, 110, 120)))
                {
                    var sfName = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(tile.BaseGameName, font, text, new Rectangle(x + 10, y, scaledW - 20, scaledH), sfName);
                }
            }
            g.Clip = oldClip;
        }

        // Left-side badges
        int badgeY = y + 4;
        if (state.FavoriteOpacity > 0.5f) { DrawBadge(g, "★", x + 4, badgeY, BadgeFavoriteBg); badgeY += 15; }

        bool hasUpdate = tile.Versions.Any(v => v.ForeColor.ToArgb() == MainForm.ColorUpdateAvailable.ToArgb());
        bool installed = IsAnyVersionInstalled(tile);
        bool canDonate = tile.Versions.Any(v => v.ForeColor.ToArgb() == MainForm.ColorDonateGame.ToArgb());
        bool downloaded = tile.Versions.Any(v => v.SubItems.Count > 1 && MainForm.DownloadedReleaseNames.Contains(v.SubItems[1].Text));

        if (hasUpdate) { DrawBadge(g, "UPDATE", x + 4, badgeY, Color.FromArgb(180, MainForm.ColorUpdateAvailable)); badgeY += 15; }
        if (canDonate) { DrawBadge(g, "AHEAD", x + 4, badgeY, Color.FromArgb(180, MainForm.ColorDonateGame)); badgeY += 15; }
        if (installed) { DrawBadge(g, "INSTALLED", x + 4, badgeY, BadgeInstalledBg); badgeY += 15; }
        if (downloaded) DrawBadge(g, "DOWNLOADED", x + 4, badgeY, BadgeDownloadedBg);

        // Right-side badges (top)
        int rightBadgeY = y + 4;

        // Size badge - show largest size across all versions for grouped tiles
        string sizeText = "";
        if (tile.Versions.Count > 1)
        {
            double maxSize = 0;
            foreach (var v in tile.Versions)
            {
                string vSize = v.SubItems.Count > 5 ? v.SubItems[5].Text : "";
                double parsed = ParseSize(vSize);
                if (parsed > maxSize) { maxSize = parsed; sizeText = vSize; }
            }
        }
        else
        {
            sizeText = item.SubItems.Count > 5 ? item.SubItems[5].Text : "";
        }
        if (!string.IsNullOrEmpty(sizeText))
        {
            DrawRightAlignedBadge(g, sizeText, x + scaledW - 4, rightBadgeY, 1.0f);
            rightBadgeY += 15;
        }

        // Date badge
        if (item.SubItems.Count > 4)
        {
            string formattedDate = FormatLastUpdated(item.SubItems[4].Text);
            if (!string.IsNullOrEmpty(formattedDate))
                DrawRightAlignedBadge(g, formattedDate, x + scaledW - 4, rightBadgeY, 1.0f);
        }

        // Delete button
        if (state.DeleteButtonOpacity > 0.01f)
            DrawDeleteButton(g, x, y, scaledW, scaledH, state.DeleteButtonOpacity, _isHoveringDeleteButton && index == _hoveredIndex);

        // Game name overlay - use BaseGameName
        if (state.TooltipOpacity > 0.01f)
        {
            int overlayH = 20;
            var overlayRect = new Rectangle(x, y + scaledH - overlayH, scaledW, overlayH);
            using (var clipPath = CreateRoundedRectangle(tileRect, THUMB_CORNER_RADIUS))
            {
                Region oldClip = g.Clip;
                g.SetClip(clipPath, CombineMode.Intersect);
                using (var overlayBrush = new SolidBrush(Color.FromArgb((int)(180 * state.TooltipOpacity), 0, 0, 0)))
                    g.FillRectangle(overlayBrush, overlayRect.X - 1, overlayRect.Y, overlayRect.Width + 2, overlayRect.Height + 1);
                g.Clip = oldClip;
            }

            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb((int)(TextColor.A * state.TooltipOpacity), TextColor)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
                g.DrawString(tile.BaseGameName, font, brush, new Rectangle(overlayRect.X, overlayRect.Y + 1, overlayRect.Width, overlayRect.Height), sf);
            }
        }

        // Tile borders
        using (var tilePath = CreateRoundedRectangle(tileRect, CORNER_RADIUS))
        {
            if (state.SelectionOpacity > 0.01f) // Selected border
                using (var selectionPen = new Pen(Color.FromArgb((int)(255 * state.SelectionOpacity), TileBorderSelected), 3f))
                    g.DrawPath(selectionPen, tilePath);

            if (state.BorderOpacity > 0.01f) // Hover border
                using (var borderPen = new Pen(Color.FromArgb((int)(200 * state.BorderOpacity), TileBorderHover), 2f))
                    g.DrawPath(borderPen, tilePath);

            if (state.FavoriteOpacity > 0.5f) // Favorite border
                using (var favPen = new Pen(Color.FromArgb((int)(180 * state.FavoriteOpacity), TileBorderFavorite), 1f))
                    g.DrawPath(favPen, tilePath);
        }

        // Version count badge (top-center, straddling the top edge) - drawn last so it's on top
        if (tile.Versions.Count > 1 && state.GroupBadgeOpacity > 0.01f)
        {
            string countText = tile.Versions.Count + " VERSIONS";
            using (var badgeFont = new Font("Segoe UI", 6f, FontStyle.Bold))
            {
                var sz = g.MeasureString(countText, badgeFont);
                int badgeW = (int)sz.Width + 6;
                int badgeH = 12;
                int vbX = x + (scaledW - badgeW) / 2;
                int vbY = y - badgeH / 2 + 1;
                var badgeRect = new Rectangle(vbX, vbY, badgeW, badgeH);
                int alpha = (int)(255 * state.GroupBadgeOpacity);
                using (var path = CreateRoundedRectangle(badgeRect, 3))
                using (var bgBrush = new SolidBrush(Color.FromArgb(alpha * 220 / 255, 24, 24, 28)))
                {
                    g.FillPath(bgBrush, path);
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    using (var textBrush = new SolidBrush(Color.FromArgb(alpha, 230, 230, 230)))
                        g.DrawString(countText, badgeFont, textBrush, badgeRect, sf);
                }
            }
        }
    }

    private void DrawDeleteButton(Graphics g, int tileX, int tileY, int tileWidth, int tileHeight, float opacity, bool isHovering)
    {
        // Position in bottom-right corner of thumbnail
        int btnX = tileX + tileWidth - DELETE_BUTTON_SIZE - 2 - DELETE_BUTTON_MARGIN;
        int btnY = tileY + 2 + tileHeight - DELETE_BUTTON_SIZE - DELETE_BUTTON_MARGIN - 20;
        var btnRect = new Rectangle(btnX, btnY, DELETE_BUTTON_SIZE, DELETE_BUTTON_SIZE);

        Color bgColor = isHovering ? DeleteButtonHoverBg : DeleteButtonBg;
        using (var path = CreateRoundedRectangle(btnRect, 6))
        using (var bgBrush = new SolidBrush(Color.FromArgb((int)(opacity * 255), bgColor)))
            g.FillPath(bgBrush, path);

        // Draw trash icon
        int iconPadding = 5;
        int iconX = btnX + iconPadding;
        int iconY = btnY + iconPadding;
        int iconSize = DELETE_BUTTON_SIZE - iconPadding * 2;

        using (var pen = new Pen(Color.FromArgb((int)(opacity * 255), Color.White), 1.5f))
        {
            // Trash can body
            int bodyTop = iconY + 4;
            int bodyBottom = iconY + iconSize;
            int bodyLeft = iconX + 2;
            int bodyRight = iconX + iconSize - 2;

            // Draw body outline (trapezoid-ish shape)
            g.DrawLine(pen, bodyLeft, bodyTop, bodyLeft + 1, bodyBottom);
            g.DrawLine(pen, bodyLeft + 1, bodyBottom, bodyRight - 1, bodyBottom);
            g.DrawLine(pen, bodyRight - 1, bodyBottom, bodyRight, bodyTop);

            // Draw lid
            g.DrawLine(pen, iconX, bodyTop, iconX + iconSize, bodyTop);

            // Draw handle on lid
            int handleLeft = iconX + iconSize / 2 - 3;
            int handleRight = iconX + iconSize / 2 + 3;
            int handleTop = iconY + 1;
            g.DrawLine(pen, handleLeft, bodyTop, handleLeft, handleTop);
            g.DrawLine(pen, handleLeft, handleTop, handleRight, handleTop);
            g.DrawLine(pen, handleRight, handleTop, handleRight, bodyTop);

            // Draw vertical lines inside trash
            int lineY1 = bodyTop + 3;
            int lineY2 = bodyBottom - 3;
            g.DrawLine(pen, iconX + iconSize / 2, lineY1, iconX + iconSize / 2, lineY2);
            if (iconSize > 10)
            {
                g.DrawLine(pen, iconX + iconSize / 2 - 4, lineY1, iconX + iconSize / 2 - 4, lineY2);
                g.DrawLine(pen, iconX + iconSize / 2 + 4, lineY1, iconX + iconSize / 2 + 4, lineY2);
            }
        }
    }

    private void DrawRightAlignedBadge(Graphics g, string text, int rightX, int y, float opacity)
    {
        using (var font = new Font("Segoe UI", 6.5f, FontStyle.Bold))
        {
            var sz = g.MeasureString(text, font);
            var rect = new Rectangle(rightX - (int)sz.Width - 6, y, (int)sz.Width + 6, 13);
            using (var path = CreateRoundedRectangle(rect, 3))
            using (var bgBrush = new SolidBrush(Color.FromArgb((int)(180 * opacity), 0, 0, 0)))
            {
                g.FillPath(bgBrush, path);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using (var textBrush = new SolidBrush(Color.FromArgb((int)(255 * opacity), 255, 255, 255)))
                    g.DrawString(text, font, textBrush, rect, sf);
            }
        }
    }

    private string FormatLastUpdated(string dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return "";

        // Extract just the date part before space
        if (DateTime.TryParse(dateStr.Split(' ')[0], out DateTime date))
            return date.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToUpperInvariant();

        return dateStr; // Fallback: return original if parsing fails
    }

    private void DrawBadge(Graphics g, string text, int x, int y, Color bgColor)
    {
        using (var font = new Font("Segoe UI", 6.5f, FontStyle.Bold))
        {
            var sz = g.MeasureString(text, font);
            var rect = new Rectangle(x, y, (int)sz.Width + 6, 13);
            using (var path = CreateRoundedRectangle(rect, 3))
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, font, Brushes.White, rect, sf);
            }
        }
    }

    private Image GetCachedImage(string packageName)
    {
        if (string.IsNullOrEmpty(packageName)) return null;
        if (_imageCache.TryGetValue(packageName, out var cached)) return cached;

        string basePath = SideloaderRCLONE.ThumbnailsFolder;
        string path = new[] { ".jpg", ".png" }.Select(ext => Path.Combine(basePath, packageName + ext)).FirstOrDefault(File.Exists);
        if (path == null) return null;

        try
        {
            while (_imageCache.Count >= MAX_CACHE_SIZE && _cacheOrder.Count > 0)
            {
                string oldKey = _cacheOrder.Dequeue();
                if (_imageCache.TryGetValue(oldKey, out var oldImg)) { oldImg.Dispose(); _imageCache.Remove(oldKey); }
            }
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var img = Image.FromStream(stream);
                _imageCache[packageName] = img;
                _cacheOrder.Enqueue(packageName);
                return img;
            }
        }
        catch { return null; }
    }

    private int GetTileIndexAtPoint(int x, int y)
    {
        // Account for sort panel offset
        if (y < SORT_PANEL_HEIGHT) return -1;
        int adjustedY = (y - SORT_PANEL_HEIGHT) + (int)_scrollY;
        int col = (x - _leftPadding) / (_tileWidth + _spacing);
        int row = (adjustedY - _spacing) / (_tileHeight + _spacing);
        if (col < 0 || col >= _columns || row < 0) return -1;

        int tileX = _leftPadding + col * (_tileWidth + _spacing);
        int tileY = _spacing + row * (_tileHeight + _spacing);
        if (x >= tileX && x < tileX + _tileWidth && adjustedY >= tileY && adjustedY < tileY + _tileHeight)
        {
            int index = row * _columns + col;
            return index < _displayTiles.Count ? index : -1;
        }
        return -1;
    }

    private bool IsPointOnDeleteButton(int x, int y, int index)
    {
        if (index < 0 || index >= _displayTiles.Count) return false;
        if (!IsAnyVersionInstalled(_displayTiles[index])) return false;
        int row = index / _columns;
        int col = index % _columns;
        return GetDeleteButtonRect(index, row, col, (int)_scrollY).Contains(x, y);
    }

    private int GetOverlayVersionAtPoint(int x, int y)
    {
        if (_expandOverlayOpacity < 0.5f || _expandedTileIndex < 0) return -1;
        for (int i = 0; i < _versionRects.Count; i++)
            if (!_versionRects[i].IsEmpty && _versionRects[i].Contains(x, y)) return i;
        return -1;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_expandedTileIndex >= 0 && _expandOverlayOpacity > 0.5f)
        {
            int versionIdx = GetOverlayVersionAtPoint(e.X, e.Y);
            if (versionIdx != _overlayHoveredVersion)
            {
                _overlayHoveredVersion = versionIdx;

                // Update release notes when hovering over a version
                if (versionIdx >= 0 && versionIdx < _displayTiles[_expandedTileIndex].Versions.Count)
                {
                    var hoveredVersion = _displayTiles[_expandedTileIndex].Versions[versionIdx];
                    string releaseName = hoveredVersion.SubItems.Count > 1 ? hoveredVersion.SubItems[1].Text : "";
                    TileHovered?.Invoke(this, releaseName);
                }

                Invalidate();
            }
            Cursor = versionIdx >= 0 ? Cursors.Hand : Cursors.Default;
            return;
        }

        int newHover = GetTileIndexAtPoint(e.X, e.Y);
        bool wasHoveringDelete = _isHoveringDeleteButton;

        if (newHover != _hoveredIndex)
        {
            _hoveredIndex = newHover;
            _isHoveringDeleteButton = false;
        }

        if (_hoveredIndex >= 0)
            _isHoveringDeleteButton = IsPointOnDeleteButton(e.X, e.Y, _hoveredIndex);
        else
            _isHoveringDeleteButton = false;

        // Update cursor
        Cursor = _isHoveringDeleteButton || _hoveredIndex >= 0 ? Cursors.Hand : Cursors.Default;
        if (wasHoveringDelete != _isHoveringDeleteButton) Invalidate(); // Redraw if delete button hover state changed
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoveredIndex = -1;
        _isHoveringDeleteButton = false;
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        // Take focus to unfocus any other control (like search text box)
        if (!Focused) Focus();

        if (e.Button == MouseButtons.Left)
        {
            if (_expandedTileIndex >= 0 && _expandOverlayOpacity > 0.5f)
            {
                int versionIdx = GetOverlayVersionAtPoint(e.X, e.Y);
                if (versionIdx >= 0)
                {
                    var tile = _displayTiles[_expandedTileIndex];
                    var selectedVersion = tile.Versions[versionIdx];
                    _selectedItem = selectedVersion;
                    int actualIndex = _items.IndexOf(selectedVersion);
                    CloseOverlay();
                    TileClicked?.Invoke(this, actualIndex);
                    TileDoubleClicked?.Invoke(this, actualIndex);
                    Invalidate();
                    return;
                }

                if (!_overlayRect.Contains(e.X, e.Y))
                {
                    CloseOverlay();
                    Invalidate();
                    return;
                }
                return;
            }

            int i = GetTileIndexAtPoint(e.X, e.Y);
            if (i >= 0)
            {
                var tile = _displayTiles[i];

                // Check if clicking on delete button
                if (IsPointOnDeleteButton(e.X, e.Y, i))
                {
                    // Select this item so the uninstall knows which app to remove
                    _selectedIndex = i;
                    _selectedItem = tile.Primary;
                    int actualIndex = _items.IndexOf(tile.Primary);
                    TileClicked?.Invoke(this, actualIndex);
                    Invalidate();

                    // Then trigger delete
                    TileDeleteClicked?.Invoke(this, actualIndex);
                    return;
                }

                if (tile.Versions.Count > 1)
                {
                    _expandedTileIndex = i;
                    _targetExpandOverlayOpacity = 1.0f;
                    _overlayHoveredVersion = -1;
                    _overlayScrollOffset = 0;

                    // Pre-select the shortest-named version (base game) to show thumbnail/trailer
                    _selectedIndex = i;
                    var baseVersion = tile.Versions.OrderBy(v => v.Text.Length).First();
                    _selectedItem = baseVersion;
                    int baseIdx = _items.IndexOf(baseVersion);
                    TileClicked?.Invoke(this, baseIdx);

                    Invalidate();
                    return;
                }

                _selectedIndex = i;
                _selectedItem = tile.Primary;
                int idx = _items.IndexOf(tile.Primary);
                Invalidate();
                TileClicked?.Invoke(this, idx);
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            // Right-click in overlay - context menu for specific version
            if (_expandedTileIndex >= 0 && _expandOverlayOpacity > 0.5f)
            {
                int versionIdx = GetOverlayVersionAtPoint(e.X, e.Y);
                if (versionIdx >= 0)
                {
                    var tile = _displayTiles[_expandedTileIndex];
                    var version = tile.Versions[versionIdx];
                    _selectedItem = version;
                    _rightClickedIndex = _expandedTileIndex;
                    _rightClickedVersionIndex = versionIdx;
                    int actualIdx = _items.IndexOf(version);
                    TileClicked?.Invoke(this, actualIdx);
                    TileRightClicked?.Invoke(this, actualIdx);
                    _contextMenu.Show(this, e.Location);
                    Invalidate();
                    return;
                }

                if (!_overlayRect.Contains(e.X, e.Y))
                {
                    CloseOverlay();
                    Invalidate();
                }
                return;
            }

            int i = GetTileIndexAtPoint(e.X, e.Y);
            if (i >= 0)
            {
                _rightClickedIndex = i;
                _rightClickedVersionIndex = -1;
                _selectedIndex = i;
                _selectedItem = _displayTiles[i].Primary;
                int actualIdx = _items.IndexOf(_displayTiles[i].Primary);
                Invalidate();
                TileClicked?.Invoke(this, actualIdx);
                TileRightClicked?.Invoke(this, actualIdx);
                _contextMenu.Show(this, e.Location);
            }
        }
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        if (e.Button != MouseButtons.Left || _expandedTileIndex >= 0) return;

        int i = GetTileIndexAtPoint(e.X, e.Y);

        // Don't trigger double-click if on delete button
        if (i >= 0 && !IsPointOnDeleteButton(e.X, e.Y, i))
        {
            var tile = _displayTiles[i];
            if (tile.Versions.Count == 1)
            {
                int idx = _items.IndexOf(tile.Primary);
                TileDoubleClicked?.Invoke(this, idx);
            }
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        // Scroll overlay if open and has overflow
        if (_expandedTileIndex >= 0 && _overlayMaxScroll > 0 && _overlayRect.Contains(e.X, e.Y))
        {
            _overlayScrollOffset -= e.Delta / 3;
            _overlayScrollOffset = Math.Max(0, Math.Min(_overlayScrollOffset, _overlayMaxScroll));
            Invalidate();
            return;
        }

        if (_expandedTileIndex >= 0) return;

        float scrollAmount = e.Delta * 1.2f;
        int maxScroll = Math.Max(0, _contentHeight - (Height - SORT_PANEL_HEIGHT));
        _targetScrollY = Math.Max(0, Math.Min(maxScroll, _targetScrollY - scrollAmount));
        _isScrolling = true;
    }

    private void CreateContextMenu()
    {
        _contextMenu = new ContextMenuStrip();
        _contextMenu.BackColor = Color.FromArgb(40, 42, 48);
        _contextMenu.ForeColor = Color.White;
        _contextMenu.ShowImageMargin = false;
        _contextMenu.Renderer = new MainForm.CenteredMenuRenderer();

        var favoriteItem = new ToolStripMenuItem("★ Add to Favorites");
        favoriteItem.Click += ContextMenu_FavoriteClick;
        _contextMenu.Items.Add(favoriteItem);

        var openFolderItem = new ToolStripMenuItem("📂 Open Folder");
        openFolderItem.Click += ContextMenu_OpenFolderClick;
        _contextMenu.Items.Add(openFolderItem);

        var deleteFolderItem = new ToolStripMenuItem("🗑 Delete from PC");
        deleteFolderItem.Click += ContextMenu_DeleteFolderClick;
        _contextMenu.Items.Add(deleteFolderItem);

        _contextMenu.Opening += ContextMenu_Opening;
    }

    private void ContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_rightClickedIndex < 0 || _rightClickedIndex >= _displayTiles.Count) { e.Cancel = true; return; }

        var tile = _displayTiles[_rightClickedIndex];
        ListViewItem targetItem;

        // If right-clicked on a specific version in overlay, use that
        if (_rightClickedVersionIndex >= 0 && _rightClickedVersionIndex < tile.Versions.Count)
            targetItem = tile.Versions[_rightClickedVersionIndex];
        else
            targetItem = tile.Primary;

        string packageName = targetItem.SubItems.Count > 1 ? targetItem.SubItems[1].Text : "";
        if (string.IsNullOrEmpty(packageName)) { e.Cancel = true; return; }

        bool isFavorite = _favoritesCache.Contains(packageName);
        ((ToolStripMenuItem)_contextMenu.Items[0]).Text = isFavorite ? "Remove from Favorites" : "★ Add to Favorites";

        // Show "Open Folder" / "Delete from PC" if the game folder exists locally
        if (_contextMenu.Items.Count > 2)
        {
            string releaseName = targetItem.SubItems.Count > SideloaderRCLONE.ReleaseNameIndex
                ? targetItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text : "";
            var settings = SettingsManager.Instance;
            string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
            string folderPath = Path.Combine(dlDir, releaseName);
            bool folderExists = !string.IsNullOrEmpty(releaseName) && Directory.Exists(folderPath);
            _contextMenu.Items[1].Visible = folderExists;
            _contextMenu.Items[2].Visible = folderExists;
        }
    }

    private void ContextMenu_FavoriteClick(object sender, EventArgs e)
    {
        if (_rightClickedIndex < 0 || _rightClickedIndex >= _displayTiles.Count) return;

        var tile = _displayTiles[_rightClickedIndex];
        ListViewItem targetItem;

        // If right-clicked on a specific version in overlay, use that
        if (_rightClickedVersionIndex >= 0 && _rightClickedVersionIndex < tile.Versions.Count)
            targetItem = tile.Versions[_rightClickedVersionIndex];
        else
            targetItem = tile.Primary;

        string packageName = targetItem.SubItems.Count > 1 ? targetItem.SubItems[1].Text : "";
        if (string.IsNullOrEmpty(packageName)) return;

        var settings = SettingsManager.Instance;
        if (_favoritesCache.Contains(packageName))
        {
            settings.RemoveFavoriteGame(packageName);
            _favoritesCache.Remove(packageName);

            // Check if in favorites-only view (= this item is now the only non-favorite)
            bool isFavoritesView = _items.All(item =>
                item == targetItem || _favoritesCache.Contains(item.SubItems.Count > 1 ? item.SubItems[1].Text : ""));

            if (isFavoritesView)
                RemoveVersionFromDisplay(tile, targetItem, _rightClickedIndex);
        }
        else
        {
            settings.AddFavoriteGame(packageName);
            _favoritesCache.Add(packageName);
        }

        Invalidate();
    }

    private void ContextMenu_OpenFolderClick(object sender, EventArgs e)
    {
        if (_rightClickedIndex < 0 || _rightClickedIndex >= _displayTiles.Count) return;

        var tile = _displayTiles[_rightClickedIndex];
        ListViewItem targetItem;

        if (_rightClickedVersionIndex >= 0 && _rightClickedVersionIndex < tile.Versions.Count)
            targetItem = tile.Versions[_rightClickedVersionIndex];
        else
            targetItem = tile.Primary;

        if (targetItem.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex) return;

        string releaseName = targetItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
        var settings = SettingsManager.Instance;
        string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
        string folderPath = Path.Combine(dlDir, releaseName);
        MainForm.OpenDirectory(folderPath);
    }

    private void ContextMenu_DeleteFolderClick(object sender, EventArgs e)
    {
        if (_rightClickedIndex < 0 || _rightClickedIndex >= _displayTiles.Count) return;

        var tile = _displayTiles[_rightClickedIndex];
        ListViewItem targetItem = (_rightClickedVersionIndex >= 0 && _rightClickedVersionIndex < tile.Versions.Count)
            ? tile.Versions[_rightClickedVersionIndex] : tile.Primary;

        if (targetItem.SubItems.Count <= SideloaderRCLONE.ReleaseNameIndex) return;

        string releaseName = targetItem.SubItems[SideloaderRCLONE.ReleaseNameIndex].Text;
        var settings = SettingsManager.Instance;
        string dlDir = settings.CustomDownloadDir ? settings.DownloadDir : Environment.CurrentDirectory;
        string folderPath = Path.Combine(dlDir, releaseName);

        if (!Directory.Exists(folderPath)) return;

        DialogResult confirm = FlexibleMessageBox.Show(FindForm(),
            $"Delete downloaded files for {releaseName}?\n\nFiles will be moved to the Recycle Bin.",
            "Delete Downloaded Files?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        if (!FileSystemUtilities.MoveToRecycleBin(folderPath))
        {
            MessageBox.Show("Failed to move folder to Recycle Bin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var mainForm = FindForm() as MainForm;
        mainForm?.RefreshDownloadedState();
    }

    private void RemoveVersionFromDisplay(GroupedTile tile, ListViewItem item, int tileIndex)
    {
        _items.Remove(item);
        _originalItems.Remove(item);
        tile.Versions.Remove(item);

        if (tile.Versions.Count == 0)
        {
            _displayTiles.RemoveAt(tileIndex);
            CloseOverlay();
            if (_selectedIndex == tileIndex) { _selectedIndex = -1; _selectedItem = null; }
            else if (_selectedIndex > tileIndex) _selectedIndex--;
        }
        else
        {
            tile.BaseGameName = GetBaseGameName(tile.Versions);
            tile.GameName = tile.Versions[0].Text;

            if (tile.Versions.Count == 1)
                CloseOverlay();
            else if (_overlayHoveredVersion >= tile.Versions.Count)
                _overlayHoveredVersion = tile.Versions.Count - 1;
        }

        // Rebuild tile states
        _tileStates.Clear();
        for (int i = 0; i < _displayTiles.Count; i++)
            _tileStates[i] = new TileAnimationState();

        RecalculateLayout();
    }

    public void RefreshFavoritesCache()
    {
        _favoritesCache = new HashSet<string>(SettingsManager.Instance.FavoritedGames, StringComparer.OrdinalIgnoreCase);
    }

    public void ScrollToPackage(string releaseName)
    {
        if (string.IsNullOrEmpty(releaseName) || _displayTiles == null || _displayTiles.Count == 0) return;

        // Find the index of the item with the matching release name
        for (int i = 0; i < _displayTiles.Count; i++)
        {
            var tile = _displayTiles[i];
            if (tile.Primary.SubItems.Count > 1 && tile.Primary.SubItems[1].Text.Equals(releaseName, StringComparison.OrdinalIgnoreCase))
            {
                // Calculate the row this item is in
                int row = i / _columns;

                // Calculate the Y position to scroll to (center the row in view if possible)
                int targetY = _spacing + SORT_PANEL_HEIGHT + row * (_tileHeight + _spacing);
                int viewportHeight = Height - SORT_PANEL_HEIGHT;
                int centeredY = targetY - (viewportHeight / 2) + (_tileHeight / 2);

                // Clamp to valid scroll range
                int maxScroll = Math.Max(0, _contentHeight - viewportHeight);
                _scrollY = Math.Max(0, Math.Min(centeredY, maxScroll));
                _targetScrollY = _scrollY;

                // Update scrollbar and redraw
                if (_scrollBar.Visible)
                    _scrollBar.Value = Math.Max(_scrollBar.Minimum, Math.Min(_scrollBar.Maximum - _scrollBar.LargeChange + 1, (int)_scrollY));

                // Also select this item visually
                _selectedIndex = i;

                Invalidate();
                break;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _animationTimer?.Stop();
            _animationTimer?.Dispose();
            _contextMenu?.Dispose();
            foreach (var img in _imageCache.Values) { try { img?.Dispose(); } catch { } }
            _imageCache.Clear();
            _cacheOrder.Clear();
            _tileStates.Clear();
            _backBuffer?.Dispose();
            _backBuffer = null;
        }
        base.Dispose(disposing);
    }
}