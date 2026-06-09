using AndroidSideloader.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class QueueViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<QueueItemViewModel> _items = new ObservableCollection<QueueItemViewModel>();

        [ObservableProperty]
        private QueueItemViewModel? _currentItem;

        [ObservableProperty]
        private float _overallProgress;

        [ObservableProperty]
        private string _statusText = "";

        [ObservableProperty]
        private bool _isRunning;

        private CancellationTokenSource? _cts;
        private readonly SettingsManager _settings = SettingsManager.Instance;

        public void AddToQueue(string releaseName, string gameName, string packageName, double sizeMB)
        {
            if (Items.Any(i => i.ReleaseName.Equals(releaseName, StringComparison.OrdinalIgnoreCase)))
                return;

            Items.Add(new QueueItemViewModel
            {
                ReleaseName = releaseName,
                GameName = gameName,
                PackageName = packageName,
                SizeMB = sizeMB,
                Status = QueueStatus.Pending
            });
        }

        [RelayCommand]
        private void ClearCompleted()
        {
            var completed = Items.Where(i => i.Status == QueueStatus.Completed || i.Status == QueueStatus.Failed).ToList();
            foreach (var item in completed)
                Items.Remove(item);
        }

        [RelayCommand]
        private async Task StartQueueAsync()
        {
            if (IsRunning || Items.Count == 0) return;

            IsRunning = true;
            _cts = new CancellationTokenSource();

            try
            {
                while (Items.Count > 0 && !_cts.Token.IsCancellationRequested)
                {
                    var item = Items.FirstOrDefault(i => i.Status == QueueStatus.Pending);
                    if (item == null) break;

                    CurrentItem = item;
                    await ProcessItemAsync(item, _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                StatusText = "Queue paused";
            }
            finally
            {
                IsRunning = false;
                CurrentItem = null;
            }
        }

        [RelayCommand]
        private void CancelQueue()
        {
            _cts?.Cancel();
        }

        private async Task ProcessItemAsync(QueueItemViewModel item, CancellationToken ct)
        {
            item.Status = QueueStatus.Downloading;
            StatusText = $"Downloading {item.GameName}...";

            try
            {
                // Simulate download progress for now
                for (int i = 0; i <= 100; i += 10)
                {
                    ct.ThrowIfCancellationRequested();
                    item.Progress = i;
                    await Task.Delay(200, ct);
                }

                item.Status = QueueStatus.Installing;
                StatusText = $"Installing {item.GameName}...";

                string downloadDir = _settings.CustomDownloadDir ? _settings.DownloadDir : AppContext.BaseDirectory;
                string gameDir = Path.Combine(downloadDir, item.ReleaseName);

                // Find APK
                string? apkPath = Directory.GetFiles(gameDir, "*.apk", SearchOption.AllDirectories).FirstOrDefault();
                if (apkPath == null)
                {
                    throw new FileNotFoundException($"No APK found in {gameDir}");
                }

                // Install APK with progress
                var installResult = await ADB.SideloadWithProgressAsync(
                    apkPath,
                    (progress, eta) => item.Progress = progress,
                    status => StatusText = status,
                    item.PackageName,
                    item.GameName);

                if (!string.IsNullOrEmpty(installResult.Error))
                {
                    throw new Exception(installResult.Error);
                }

                // Copy OBB if exists
                string obbDir = Path.Combine(gameDir, item.PackageName);
                if (!Directory.Exists(obbDir))
                {
                    // Try to find any folder with dots (package name format)
                    var candidate = Directory.GetDirectories(gameDir)
                        .FirstOrDefault(d => Path.GetFileName(d).Contains("."));
                    if (candidate != null) obbDir = candidate;
                }

                if (Directory.Exists(obbDir) && SideloaderUtilities.CheckFolderIsObb(obbDir))
                {
                    StatusText = $"Copying OBB for {item.GameName}...";
                    var obbResult = await ADB.CopyOBBWithProgressAsync(
                        item.GameName,
                        obbDir,
                        item.PackageName,
                        (progress, eta) => item.Progress = 100 + progress, // show OBB as 100-200 range
                        status => StatusText = status);

                    if (!string.IsNullOrEmpty(obbResult.Error))
                    {
                        Logger.Log($"OBB copy failed: {obbResult.Error}", LogLevel.WARNING);
                    }
                }

                item.Status = QueueStatus.Completed;
                item.Progress = 100;
                StatusText = $"Completed {item.GameName}";
            }
            catch (OperationCanceledException)
            {
                item.Status = QueueStatus.Pending;
                throw;
            }
            catch (Exception ex)
            {
                item.Status = QueueStatus.Failed;
                item.ErrorMessage = ex.Message;
                StatusText = $"Failed: {ex.Message}";
            }
        }
    }

    public enum QueueStatus { Pending, Downloading, Installing, Completed, Failed, Paused }

    public partial class QueueItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _releaseName = "";

        [ObservableProperty]
        private string _gameName = "";

        [ObservableProperty]
        private string _packageName = "";

        [ObservableProperty]
        private double _sizeMB;

        [ObservableProperty]
        private float _progress;

        [ObservableProperty]
        private QueueStatus _status;

        [ObservableProperty]
        private string? _errorMessage;
    }
}
