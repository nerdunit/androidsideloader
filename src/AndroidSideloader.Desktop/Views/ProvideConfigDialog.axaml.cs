using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using AndroidSideloader.Models;
using AndroidSideloader.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.Views
{
    public partial class ProvideConfigDialog : Window
    {
        public bool Result { get; private set; }
        public string ConfigText { get; set; } = "";
        public string? ErrorText { get; private set; }
        public bool HasError => !string.IsNullOrEmpty(ErrorText);
        public string HintText { get; }

        private readonly bool _isPublic;

        public ProvideConfigDialog(bool isPublic)
        {
            InitializeComponent();
            DataContext = this;
            _isPublic = isPublic;
            HintText = isPublic
                ? "Paste JSON config, browse for a .json file, or fetch from a URL:"
                : "Paste Rclone config, browse for a file, or fetch from a URL:";
        }

        private async void Browse_Click(object? sender, RoutedEventArgs e)
        {
            var options = new FilePickerOpenOptions
            {
                Title = _isPublic ? "Select a public config JSON file" : "Select an Rclone config file",
                AllowMultiple = false
            };

            var files = await StorageProvider.OpenFilePickerAsync(options);
            if (files.Count > 0)
            {
                var path = files[0].TryGetLocalPath();
                if (!string.IsNullOrEmpty(path))
                {
                    ConfigText = File.ReadAllText(path);
                }
            }
        }

        private void Ok_Click(object? sender, RoutedEventArgs e)
        {
            if (ValidateAndSave())
            {
                Result = true;
                Close();
            }
        }

        private void Cancel_Click(object? sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private bool ValidateAndSave()
        {
            ErrorText = null;
            string input = ConfigText.Trim();

            if (string.IsNullOrEmpty(input))
            {
                ErrorText = "Please paste a config, enter a URL, or browse for a file.";
                return false;
            }

            // If it looks like a URL, fetch it
            if (input.StartsWith("http://") || input.StartsWith("https://"))
            {
                try
                {
                    using var client = new WebClient();
                    input = client.DownloadString(input);
                }
                catch (Exception ex)
                {
                    ErrorText = $"Failed to fetch URL: {ex.Message}";
                    return false;
                }
            }

            if (_isPublic)
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<PublicConfig>(input);
                    if (config == null || string.IsNullOrWhiteSpace(config.BaseUri) || string.IsNullOrWhiteSpace(config.Password))
                    {
                        ErrorText = "Invalid public config: BaseUri and Password are required.";
                        return false;
                    }

                    string publicJsonPath = Path.Combine(AppContext.BaseDirectory, "public.json");
                    File.WriteAllText(publicJsonPath, input);
                    AppState.PublicConfigFile = config;
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorText = $"Invalid JSON: {ex.Message}";
                    return false;
                }
            }
            else
            {
                string rcloneDir = Path.Combine(AppContext.BaseDirectory, "rclone");
                Directory.CreateDirectory(rcloneDir);
                string downloadConfigPath = Path.Combine(rcloneDir, "download.config");
                File.WriteAllText(downloadConfigPath, input);
                return true;
            }
        }
    }
}
