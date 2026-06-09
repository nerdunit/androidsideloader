using CommunityToolkit.Mvvm.ComponentModel;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class GameItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _gameName = "";

        [ObservableProperty]
        private string _releaseName = "";

        [ObservableProperty]
        private string _packageName = "";

        [ObservableProperty]
        private string _versionCode = "";

        [ObservableProperty]
        private string _versionName = "";

        [ObservableProperty]
        private string _downloads = "";

        [ObservableProperty]
        private string _installedVersion = "";

        [ObservableProperty]
        private string _size = "";

        [ObservableProperty]
        private string _lastUpdated = "";

        [ObservableProperty]
        private string _statusColor = "#ffffff";

        [ObservableProperty]
        private bool _isInstalled;

        [ObservableProperty]
        private bool _updateAvailable;

        [ObservableProperty]
        private bool _isDownloaded;

        [ObservableProperty]
        private bool _isDonateGame;

        [ObservableProperty]
        private int _popularityRank;

        [ObservableProperty]
        private bool _isFavorite;

        [ObservableProperty]
        private string? _thumbnailPath;
    }
}
