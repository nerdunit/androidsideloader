using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.Views
{
    public partial class AdbCommandDialog : Window
    {
        public AdbCommandDialog()
        {
            InitializeComponent();
            DataContext = new AdbCommandDialogViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public partial class AdbCommandDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _commandText = "";

        [ObservableProperty]
        private string _outputText = "";

        [RelayCommand]
        private async Task ExecuteCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(CommandText)) return;

            OutputText += $"> adb {CommandText}\n";
            try
            {
                var result = await Task.Run(() => ADB.RunAdbCommandToString(CommandText));
                OutputText += result.Output + "\n";
                if (!string.IsNullOrEmpty(result.Error))
                {
                    OutputText += $"[ERROR] {result.Error}\n";
                }
            }
            catch (System.Exception ex)
            {
                OutputText += $"[EXCEPTION] {ex.Message}\n";
            }
        }
    }
}
