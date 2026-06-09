using AndroidSideloader.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private string _deviceStatus = "No device connected";

        [ObservableProperty]
        private bool _isDeviceConnected;

        [ObservableProperty]
        private object? _currentView;

        public GamesViewModel GamesViewModel { get; }
        public DeviceViewModel DeviceViewModel { get; }

        public MainWindowViewModel()
        {
            GamesViewModel = new GamesViewModel(this);
            DeviceViewModel = new DeviceViewModel(this);
            CurrentView = GamesViewModel;

            // Hook up backend events
            AppState.OnShowMessageBox += (title, message) =>
            {
                // TODO: show dialog via Avalonia window
                System.Diagnostics.Debug.WriteLine($"[MSG] {title}: {message}");
            };

            AppState.OnChangeTitle += title =>
            {
                StatusText = title;
            };

            _ = RefreshDeviceStatusAsync();
        }

        [RelayCommand]
        private void ShowGames()
        {
            CurrentView = GamesViewModel;
        }

        [RelayCommand]
        private void ShowDevice()
        {
            CurrentView = DeviceViewModel;
        }

        [RelayCommand]
        private async Task RefreshDeviceStatusAsync()
        {
            try
            {
                var output = await Task.Run(() => ADB.RunAdbCommandToString("devices"));
                if (output.Output.Contains("device\n") || output.Output.Contains("device\r\n"))
                {
                    IsDeviceConnected = true;
                    DeviceStatus = "Device connected";

                    var deviceList = await Task.Run(() => ADB.RunAdbCommandToString("devices -l"));
                    if (!string.IsNullOrWhiteSpace(deviceList.Output))
                    {
                        var lines = deviceList.Output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (line.Contains("device ") && !line.StartsWith("List"))
                            {
                                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length > 0)
                                {
                                    ADB.DeviceID = parts[0];
                                    DeviceStatus = $"Connected: {parts[0]}";
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    IsDeviceConnected = false;
                    DeviceStatus = "No device connected";
                }
            }
            catch (Exception ex)
            {
                DeviceStatus = $"Error: {ex.Message}";
                IsDeviceConnected = false;
            }
        }

        [RelayCommand]
        private void OpenSettings()
        {
            // TODO: implement settings view
            StatusText = "Settings (not yet implemented)";
        }
    }
}
