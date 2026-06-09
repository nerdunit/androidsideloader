using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AndroidSideloader.Desktop.ViewModels;
using AndroidSideloader.Desktop.Views;
using AndroidSideloader.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await RunStartupAsync(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ApplyThemeSettings()
        {
            try
            {
                var settings = SettingsManager.Instance;
                var fontFamily = FontFamily.Parse(settings.FontFamily);
                Resources["FontFamilyKey"] = fontFamily;
                Resources["FontSizeKey"] = settings.FontSize;
            }
            catch { }
        }

        private async Task RunStartupAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                // Init logging early
                Logger.Initialize();

                // Load settings
                var settings = SettingsManager.Instance;
                settings.MainDir = AppContext.BaseDirectory;
                settings.Save();

                // Check command line args
                string[] args = Environment.GetCommandLineArgs();
                foreach (string arg in args)
                {
                    if (arg == "--offline")
                        AppState.IsOffline = true;
                    if (arg == "--no-rclone-update")
                        AppState.NoRcloneUpdating = true;
                    if (arg == "--disable-app-check")
                        AppState.DebugMode = true; // reusing for now
                }

                // Show startup dialog if not offline
                if (!AppState.IsOffline)
                {
                    var startup = new StartupDialog();
                    startup.Show();
                    var tcs = new TaskCompletionSource<StartupDialog.StartupChoice>();
                    startup.Closed += (s, e) =>
                    {
                        if (s is StartupDialog dlg)
                            tcs.TrySetResult(dlg.Choice);
                    };
                    var choice = await tcs.Task;

                    if (choice == StartupDialog.StartupChoice.None)
                    {
                        desktop.Shutdown();
                        return;
                    }

                    if (startup.Choice == StartupDialog.StartupChoice.Offline)
                    {
                        AppState.IsOffline = true;
                        Logger.Log("User chose offline mode from startup dialog");
                    }
                    else if (startup.Choice == StartupDialog.StartupChoice.RcloneConfig)
                    {
                        // Rclone config mode - validate existing or new config
                        string rcloneDir = Path.Combine(AppContext.BaseDirectory, "rclone");
                        string downloadConfig = Path.Combine(rcloneDir, "download.config");
                        if (!File.Exists(downloadConfig))
                        {
                            AppState.IsOffline = true;
                        }
                    }
                }

                // Download dependencies
                AppState.UpdateStatus("Downloading Dependencies...");
                await Task.Run(() => GetDependencies.DownloadFiles());

                // Init rclone in online mode
                if (!AppState.IsOffline)
                {
                    AppState.UpdateStatus("Initializing RCLONE...");
                    await Task.Run(() => RCLONE.Init());
                }

                // Update check
                if (!AppState.DebugMode && settings.CheckForUpdates && !AppState.IsOffline)
                {
                    try
                    {
                        bool hasUpdate = await Updater.CheckUpdateAsync();
                        if (hasUpdate)
                        {
                            var updateDlg = new UpdateDialog("A new version of Android Sideloader is available.");
                            updateDlg.Show();
                            var updateTcs = new TaskCompletionSource<bool>();
                            updateDlg.Closed += (s, e) =>
                            {
                                if (s is UpdateDialog dlg)
                                    updateTcs.TrySetResult(dlg.ShouldDownload);
                            };
                            bool shouldDownload = await updateTcs.Task;
                            if (shouldDownload)
                            {
                                // TODO: trigger update download
                            }
                        }
                    }
                    catch { }
                }

                // Init mirrors if online
                if (!AppState.IsOffline)
                {
                    AppState.UpdateStatus("Initializing Servers...");
                    // TODO: init mirrors
                }
                else
                {
                    AppState.UpdateStatus("Offline mode - scanning local library...");
                    string downloadDir = settings.DownloadDir;
                    if (string.IsNullOrEmpty(downloadDir) || !Directory.Exists(downloadDir))
                    {
                        downloadDir = Path.Combine(AppContext.BaseDirectory, "downloads");
                    }
                    await Task.Run(() => SideloaderRCLONE.ScanLocalGames(downloadDir));
                }

                // Show main window
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
                mainWindow.Show();

                // Trigger game list load
                if (mainWindow.DataContext is MainWindowViewModel vm)
                {
                    vm.GamesViewModel.RefreshGamesCommand.Execute(null);
                }

                // Apply theme settings
                ApplyThemeSettings();
            }
            catch (Exception ex)
            {
                Logger.Log($"Fatal startup error: {ex}", LogLevel.ERROR);
                // Show error and shutdown
                var errorWindow = new Window
                {
                    Title = "Startup Error",
                    Content = new TextBlock { Text = $"Failed to start: {ex.Message}" }
                };
                errorWindow.Show();
            }
        }
    }
}
