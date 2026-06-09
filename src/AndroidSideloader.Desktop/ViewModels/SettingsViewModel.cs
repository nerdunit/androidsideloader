using AndroidSideloader.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsManager _settings = SettingsManager.Instance;

        [ObservableProperty]
        private string _downloadDir = "";

        [ObservableProperty]
        private bool _customDownloadDir;

        [ObservableProperty]
        private bool _deleteAfterInstall;

        [ObservableProperty]
        private bool _nodeviceMode;

        [ObservableProperty]
        private bool _autoReinstall;

        [ObservableProperty]
        private bool _checkForUpdates;

        [ObservableProperty]
        private bool _useGalleryView;

        [ObservableProperty]
        private string _backupDir = "";

        [ObservableProperty]
        private bool _useProxy;

        [ObservableProperty]
        private string _proxyAddress = "";

        [ObservableProperty]
        private string _proxyPort = "";

        [ObservableProperty]
        private string _fontFamily = "Segoe UI";

        [ObservableProperty]
        private float _fontSize = 11.25f;

        [ObservableProperty]
        private string _fontColor = "#ffffff";

        public SettingsViewModel()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            DownloadDir = _settings.DownloadDir;
            CustomDownloadDir = _settings.CustomDownloadDir;
            DeleteAfterInstall = _settings.DeleteAllAfterInstall;
            NodeviceMode = _settings.NodeviceMode;
            AutoReinstall = _settings.AutoReinstall;
            CheckForUpdates = _settings.CheckForUpdates;
            UseGalleryView = _settings.UseGalleryView;
            BackupDir = _settings.BackupDir;
            UseProxy = _settings.useProxy;
            ProxyAddress = _settings.ProxyAddress;
            ProxyPort = _settings.ProxyPort;
            FontFamily = _settings.FontFamily;
            FontSize = _settings.FontSize;
            FontColor = _settings.FontColorHex;
        }

        [RelayCommand]
        private void SaveSettings()
        {
            _settings.DownloadDir = DownloadDir;
            _settings.CustomDownloadDir = CustomDownloadDir;
            _settings.DeleteAllAfterInstall = DeleteAfterInstall;
            _settings.NodeviceMode = NodeviceMode;
            _settings.AutoReinstall = AutoReinstall;
            _settings.CheckForUpdates = CheckForUpdates;
            _settings.UseGalleryView = UseGalleryView;
            _settings.BackupDir = BackupDir;
            _settings.useProxy = UseProxy;
            _settings.ProxyAddress = ProxyAddress;
            _settings.ProxyPort = ProxyPort;
            _settings.FontFamily = FontFamily;
            _settings.FontSize = FontSize;
            _settings.FontColorHex = FontColor;
            _settings.Save();
        }

        [RelayCommand]
        private async Task BrowseDownloadDirAsync()
        {
            // TODO: open folder picker
        }

        [RelayCommand]
        private async Task BrowseBackupDirAsync()
        {
            // TODO: open folder picker
        }
    }
}
