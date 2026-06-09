using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.ViewModels
{
    public partial class DeviceViewModel : ObservableObject
    {
        private readonly MainWindowViewModel _mainWindow;

        [ObservableProperty]
        private string _deviceInfo = "No device information available.";

        [ObservableProperty]
        private string _storageInfo = "";

        [ObservableProperty]
        private bool _isLoading;

        public DeviceViewModel(MainWindowViewModel mainWindow)
        {
            _mainWindow = mainWindow;
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
    }
}
