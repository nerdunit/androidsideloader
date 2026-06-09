using AndroidSideloader.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class GamesViewModel : ObservableObject
    {
        private readonly MainWindowViewModel _mainWindow;
        private readonly SettingsManager _settings = SettingsManager.Instance;
        private List<GameItemViewModel> _allGames = new List<GameItemViewModel>();

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

        [ObservableProperty]
        private bool _showFavoritesOnly;

        [ObservableProperty]
        private bool _isGalleryView;

        public GamesViewModel(MainWindowViewModel mainWindow)
        {
            _mainWindow = mainWindow;
            IsGalleryView = _settings.UseGalleryView;
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterGames();
        }

        partial void OnShowFavoritesOnlyChanged(bool value)
        {
            FilterGames();
        }

        private void FilterGames()
        {
            var query = _allGames.AsEnumerable();

            if (ShowFavoritesOnly)
            {
                query = query.Where(g => g.IsFavorite);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(g =>
                    g.GameName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    g.ReleaseName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            Games = new ObservableCollection<GameItemViewModel>(query);
        }

        [RelayCommand]
        private async Task InitListViewAsync()
        {
            IsRefreshing = true;
            RefreshStatus = "Loading games...";
            _allGames.Clear();

            try
            {
                var items = await Task.Run(BuildGameList);
                _allGames = items;
                FilterGames();
                RefreshStatus = $"Loaded {items.Count} games";
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
        private async Task RefreshGamesAsync()
        {
            await InitListViewAsync();
        }

        [RelayCommand]
        private async Task ScanLocalGamesAsync()
        {
            IsRefreshing = true;
            RefreshStatus = "Scanning local downloads...";

            try
            {
                string downloadDir = _settings.DownloadDir;
                if (string.IsNullOrEmpty(downloadDir) || !Directory.Exists(downloadDir))
                {
                    downloadDir = Path.Combine(AppContext.BaseDirectory, "downloads");
                }

                await Task.Run(() => SideloaderRCLONE.ScanLocalGames(downloadDir));
                await InitListViewAsync();
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

        private List<GameItemViewModel> BuildGameList()
        {
            var result = new List<GameItemViewModel>();
            if (SideloaderRCLONE.games.Count == 0) return result;

            var installedApps = _settings.InstalledApps
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Parse installed versions from dumpsys if connected
            var installedVersions = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var dump = ADB.RunAdbCommandToString("shell dumpsys package", suppressLogging: true).Output;
                string? currentPkg = null;
                foreach (var rawLine in dump.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var line = rawLine.TrimStart();
                    if (line.StartsWith("Package [", StringComparison.Ordinal))
                    {
                        var start = line.IndexOf('[');
                        var end = line.IndexOf(']');
                        if (start >= 0 && end > start)
                            currentPkg = line.Substring(start + 1, end - start - 1);
                        else
                            currentPkg = null;
                        continue;
                    }
                    if (currentPkg != null && line.StartsWith("versionCode=", StringComparison.Ordinal))
                    {
                        var after = line.Substring(12);
                        int spaceIdx = after.IndexOf(' ');
                        var digits = spaceIdx > 0 ? after.Substring(0, spaceIdx) : after;
                        if (ulong.TryParse(digits, out var v))
                        {
                            if (installedApps.Contains(currentPkg))
                                installedVersions[currentPkg] = v;
                        }
                        currentPkg = null;
                    }
                }
            }
            catch { }

            // Precompute cloud max version per package
            var cloudMaxVersion = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
            foreach (var release in SideloaderRCLONE.games)
            {
                string pkg = release[SideloaderRCLONE.PackageNameIndex];
                ulong v = 0;
                try { v = ulong.Parse(StringUtilities.KeepOnlyNumbers(release[SideloaderRCLONE.VersionCodeIndex])); }
                catch { v = 0; }
                if (cloudMaxVersion.TryGetValue(pkg, out var existing))
                { if (v > existing) cloudMaxVersion[pkg] = v; }
                else
                { cloudMaxVersion[pkg] = v; }
            }

            foreach (var release in SideloaderRCLONE.games)
            {
                var item = CreateGameItem(release, installedVersions, cloudMaxVersion);
                if (item != null)
                    result.Add(item);
            }

            return result;
        }

        private GameItemViewModel? CreateGameItem(string[] game,
            Dictionary<string, ulong> installedVersions,
            Dictionary<string, ulong> cloudMaxVersion)
        {
            string gameName = game.Length > SideloaderRCLONE.GameNameIndex ? game[SideloaderRCLONE.GameNameIndex] : "";
            string releaseName = game.Length > SideloaderRCLONE.ReleaseNameIndex ? game[SideloaderRCLONE.ReleaseNameIndex] : "";
            string packageName = game.Length > SideloaderRCLONE.PackageNameIndex ? game[SideloaderRCLONE.PackageNameIndex] : "";
            string versionCode = game.Length > SideloaderRCLONE.VersionCodeIndex ? game[SideloaderRCLONE.VersionCodeIndex] : "";
            string versionName = game.Length > SideloaderRCLONE.VersionNameIndex ? game[SideloaderRCLONE.VersionNameIndex] : "";
            string downloads = game.Length > SideloaderRCLONE.DownloadsIndex ? game[SideloaderRCLONE.DownloadsIndex] : "";
            string lastUpdated = game.Length > 4 ? game[4] : "";
            string sizeRaw = game.Length > 5 ? game[5] : "";

            double sizeMB = 0;
            if (StringUtilities.TryParseDouble(sizeRaw, out sizeMB)) { }

            string sizeText = sizeMB >= 1024 ? $"{sizeMB / 1024:F2} GB" : $"{sizeMB:F0} MB";
            lastUpdated = lastUpdated.Replace(" UTC", "");

            var item = new GameItemViewModel
            {
                GameName = gameName,
                ReleaseName = releaseName,
                PackageName = packageName,
                VersionCode = versionCode,
                VersionName = versionName,
                Downloads = downloads,
                Size = sizeText,
                LastUpdated = lastUpdated,
                IsFavorite = _settings.FavoritedGames.Contains(releaseName)
            };

            // Determine status
            if (installedVersions.TryGetValue(packageName, out ulong installedVersion))
            {
                item.IsInstalled = true;
                item.InstalledVersion = $"v{installedVersion}";

                cloudMaxVersion.TryGetValue(packageName, out ulong cloudVersion);
                if (installedVersion == cloudVersion)
                {
                    item.StatusColor = "#3c91e6"; // installed, up to date
                }
                else if (installedVersion < cloudVersion)
                {
                    item.UpdateAvailable = true;
                    item.StatusColor = "#4daa57"; // update available
                }
                else
                {
                    item.IsDonateGame = true;
                    item.StatusColor = "#cb9cf2"; // newer than list
                }
            }
            else
            {
                item.StatusColor = "#ffffff";
            }

            // Mark downloaded if in download dir
            string downloadDir = _settings.CustomDownloadDir ? _settings.DownloadDir : AppContext.BaseDirectory;
            string gameDir = Path.Combine(downloadDir, releaseName);
            if (Directory.Exists(gameDir))
            {
                item.IsDownloaded = true;
                if (!item.IsInstalled)
                    item.StatusColor = "#67c7b1";
            }

            return item;
        }

        [RelayCommand]
        private void ToggleFavorite()
        {
            if (SelectedGame == null) return;
            SelectedGame.IsFavorite = !SelectedGame.IsFavorite;

            if (SelectedGame.IsFavorite)
            {
                if (!_settings.FavoritedGames.Contains(SelectedGame.ReleaseName))
                {
                    var list = _settings.FavoritedGames.ToList();
                    list.Add(SelectedGame.ReleaseName);
                    _settings.FavoritedGames = list.ToArray();
                }
            }
            else
            {
                var list = _settings.FavoritedGames.ToList();
                list.Remove(SelectedGame.ReleaseName);
                _settings.FavoritedGames = list.ToArray();
            }
            _settings.Save();
        }

        [RelayCommand]
        private void AddToQueue()
        {
            if (SelectedGame == null) return;
            double sizeMB = 0;
            if (double.TryParse(SelectedGame.Size.Replace(" GB", "").Replace(" MB", ""), out var parsed))
            {
                if (SelectedGame.Size.Contains("GB")) sizeMB = parsed * 1024;
                else sizeMB = parsed;
            }
            _mainWindow.QueueViewModel.AddToQueue(SelectedGame.ReleaseName, SelectedGame.GameName, SelectedGame.PackageName, sizeMB);
            _mainWindow.StatusText = $"Added {SelectedGame.GameName} to queue";
        }

        [RelayCommand]
        private void ToggleView()
        {
            IsGalleryView = !IsGalleryView;
            _settings.UseGalleryView = IsGalleryView;
            _settings.Save();
        }
    }
}
