using AndroidSideloader.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class GamesViewModel : ObservableObject
    {
        private readonly MainWindowViewModel _mainWindow;

        [ObservableProperty]
        private string _searchText = "";

        [ObservableProperty]
        private ObservableCollection<GameItemViewModel> _games = new ObservableCollection<GameItemViewModel>();

        [ObservableProperty]
        private GameItemViewModel? _selectedGame;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _refreshStatus = "";

        public GamesViewModel(MainWindowViewModel mainWindow)
        {
            _mainWindow = mainWindow;
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterGames();
        }

        private void FilterGames()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadGamesFromCache();
                return;
            }

            var filtered = SideloaderRCLONE.games
                .Where(g => g.Length > SideloaderRCLONE.GameNameIndex)
                .Where(g => g[SideloaderRCLONE.GameNameIndex].Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                         || g[SideloaderRCLONE.ReleaseNameIndex].Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .Select(CreateGameItem);

            Games = new ObservableCollection<GameItemViewModel>(filtered);
        }

        [RelayCommand]
        private async Task RefreshGamesAsync()
        {
            IsRefreshing = true;
            RefreshStatus = "Fetching game list...";

            try
            {
                await Task.Run(() =>
                {
                    // Try loading from local metadata first
                    string metaPath = Path.Combine(AppContext.BaseDirectory, "meta.7z");
                    if (File.Exists(metaPath))
                    {
                        SideloaderRCLONE.ProcessMetadataFromPublic();
                    }
                });

                LoadGamesFromCache();
                RefreshStatus = $"Loaded {Games.Count} games";
            }
            catch (Exception ex)
            {
                RefreshStatus = $"Error: {ex.Message}";
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task ScanLocalGamesAsync()
        {
            IsRefreshing = true;
            RefreshStatus = "Scanning local downloads...";

            try
            {
                string downloadDir = SettingsManager.Instance.DownloadDir;
                if (string.IsNullOrEmpty(downloadDir) || !Directory.Exists(downloadDir))
                {
                    downloadDir = Path.Combine(AppContext.BaseDirectory, "downloads");
                }

                await Task.Run(() => SideloaderRCLONE.ScanLocalGames(downloadDir));
                LoadGamesFromCache();
                RefreshStatus = $"Scanned {Games.Count} games locally";
            }
            catch (Exception ex)
            {
                RefreshStatus = $"Error: {ex.Message}";
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void LoadGamesFromCache()
        {
            var items = SideloaderRCLONE.games
                .Where(g => g.Length > SideloaderRCLONE.GameNameIndex)
                .Select(CreateGameItem);

            Games = new ObservableCollection<GameItemViewModel>(items);
        }

        private GameItemViewModel CreateGameItem(string[] game)
        {
            string name = game.Length > SideloaderRCLONE.GameNameIndex ? game[SideloaderRCLONE.GameNameIndex] : "Unknown";
            string release = game.Length > SideloaderRCLONE.ReleaseNameIndex ? game[SideloaderRCLONE.ReleaseNameIndex] : "";
            string package = game.Length > SideloaderRCLONE.PackageNameIndex ? game[SideloaderRCLONE.PackageNameIndex] : "";
            string version = game.Length > SideloaderRCLONE.VersionNameIndex ? game[SideloaderRCLONE.VersionNameIndex] : "";

            return new GameItemViewModel
            {
                GameName = name,
                ReleaseName = release,
                PackageName = package,
                Version = version
            };
        }

        [RelayCommand]
        private async Task InstallSelectedGameAsync()
        {
            if (SelectedGame == null) return;

            _mainWindow.StatusText = $"Installing {SelectedGame.GameName}...";

            try
            {
                // TODO: implement full install flow (download + sideload)
                await Task.Delay(500);
                _mainWindow.StatusText = $"Installed {SelectedGame.GameName}";
            }
            catch (Exception ex)
            {
                _mainWindow.StatusText = $"Install failed: {ex.Message}";
            }
        }
    }

    public partial class GameItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _gameName = "";

        [ObservableProperty]
        private string _releaseName = "";

        [ObservableProperty]
        private string _packageName = "";

        [ObservableProperty]
        private string _version = "";
    }
}
