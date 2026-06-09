using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AndroidSideloader.Desktop.Views
{
    public partial class UpdateDialog : Window
    {
        public bool ShouldDownload { get; private set; }
        public string UpdateMessage { get; set; } = "A new version is available.";

        public UpdateDialog(string? message = null)
        {
            InitializeComponent();
            DataContext = this;
            if (!string.IsNullOrEmpty(message))
                UpdateMessage = message;
        }

        private void Download_Click(object? sender, RoutedEventArgs e)
        {
            ShouldDownload = true;
            Close();
        }

        private void Later_Click(object? sender, RoutedEventArgs e)
        {
            ShouldDownload = false;
            Close();
        }
    }
}
