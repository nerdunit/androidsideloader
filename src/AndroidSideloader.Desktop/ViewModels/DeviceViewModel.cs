using AndroidSideloader.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class DeviceViewModel : ObservableObject
    {
        private readonly MainWindowViewModel _mainWindow;
        private readonly SettingsManager _settings = SettingsManager.Instance;

        [ObservableProperty]
        private string _deviceInfo = "No device information available.";

        [ObservableProperty]
        private string _storageInfo = "";

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isWirelessAdb;

        [ObservableProperty]
        private string _ipAddress = "";

        [ObservableProperty]
        private ObservableCollection<string> _installedApps = new ObservableCollection<string>();

        public DeviceViewModel(MainWindowViewModel mainWindow)
        {
            _mainWindow = mainWindow;
            IpAddress = _settings.IPAddress;
            IsWirelessAdb = _settings.WirelessADB;
        }

        [RelayCommand]
        private async Task RefreshDeviceInfoAsync()
        {
            IsLoading = true;
            try
            {
                var devices = await Task.Run(() => ADB.RunAdbCommandToString("devices -l"));
                DeviceInfo = string.IsNullOrWhiteSpace(devices.Output) ? "No devices found" : devices.Output;

                var storage = await Task.Run(() => ADB.GetAvailableSpace());
                StorageInfo = storage;
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ResetAdbAuthAsync()
        {
            try
            {
                var result = await Task.Run(() => ADB.ResetAdbAuthorization());
                DeviceInfo = result;
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task RebootDeviceAsync()
        {
            try
            {
                await Task.Run(() => ADB.RunAdbCommandToString("reboot"));
                DeviceInfo = "Reboot command sent.";
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ListInstalledAppsAsync()
        {
            IsLoading = true;
            try
            {
                var output = await Task.Run(() => ADB.RunAdbCommandToString("shell pm list packages -3"));
                var apps = output.Output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => line.StartsWith("package:"))
                    .Select(line => line.Substring(8).Trim())
                    .OrderBy(name => name)
                    .ToList();

                InstalledApps = new ObservableCollection<string>(apps);
                DeviceInfo = $"Found {apps.Count} installed apps.";
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ToggleWirelessAdbAsync()
        {
            try
            {
                if (!IsWirelessAdb)
                {
                    await Task.Run(() => ADB.RunAdbCommandToString("tcpip 5555"));
                    if (!string.IsNullOrEmpty(IpAddress))
                    {
                        await Task.Run(() => ADB.RunAdbCommandToString($"connect {IpAddress}:5555"));
                    }
                    IsWirelessAdb = true;
                    _settings.WirelessADB = true;
                }
                else
                {
                    await Task.Run(() => ADB.RunAdbCommandToString("usb"));
                    IsWirelessAdb = false;
                    _settings.WirelessADB = false;
                }
                _settings.Save();
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task BackupDeviceAsync()
        {
            try
            {
                string backupDir = _settings.CustomBackupDir ? _settings.BackupDir : Path.Combine(AppContext.BaseDirectory, "backups");
                Directory.CreateDirectory(backupDir);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string backupFile = Path.Combine(backupDir, $"backup_{timestamp}.ab");

                DeviceInfo = "Starting backup... This may take a while.";
                await Task.Run(() => ADB.RunAdbCommandToString($"backup -apk -shared -all -f \"{backupFile}\""));
                DeviceInfo = $"Backup saved to {backupFile}";
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task DisconnectDeviceAsync()
        {
            try
            {
                await Task.Run(() => ADB.RunAdbCommandToString("disconnect"));
                DeviceInfo = "Disconnected.";
            }
            catch (Exception ex)
            {
                DeviceInfo = $"Error: {ex.Message}";
            }
        }
    }
}
