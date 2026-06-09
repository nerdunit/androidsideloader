using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AndroidSideloader.Models;
using AndroidSideloader.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.Views
{
    public partial class StartupDialog : Window
    {
        public enum StartupChoice { None, Online, Offline, RcloneConfig }
        public StartupChoice Choice { get; private set; } = StartupChoice.None;

        public bool HasExistingConfig { get; private set; }
        public bool HasExistingRcloneConfig { get; private set; }
        public string? ErrorText { get; private set; }
        public bool HasError => !string.IsNullOrEmpty(ErrorText);
        public string VersionText => $"Version {Updater.LocalVersion}";

        public StartupDialog(string? initialError = null)
        {
            InitializeComponent();
            DataContext = this;
            ErrorText = initialError;

            HasExistingConfig = TryValidateExistingConfig();
            HasExistingRcloneConfig = TryValidateExistingRcloneConfig();
        }

        private bool TryValidateExistingConfig()
        {
            try
            {
                string publicJson = Path.Combine(AppContext.BaseDirectory, "public.json");
                if (!File.Exists(publicJson)) return false;
                string content = File.ReadAllText(publicJson);
                var config = JsonConvert.DeserializeObject<PublicConfig>(content);
                return config != null && !string.IsNullOrWhiteSpace(config.BaseUri);
            }
            catch { return false; }
        }

        private bool TryValidateExistingRcloneConfig()
        {
            try
            {
                string rcloneDir = Path.Combine(AppContext.BaseDirectory, "rclone");
                string downloadConfig = Path.Combine(rcloneDir, "download.config");
                return File.Exists(downloadConfig);
            }
            catch { return false; }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Choice = StartupChoice.None;
            Close();
        }

        private void ExistingConfig_Click(object? sender, PointerPressedEventArgs e)
        {
            if (!HasExistingConfig) return;
            Choice = StartupChoice.Online;
            Close();
        }

        private void ExistingRclone_Click(object? sender, PointerPressedEventArgs e)
        {
            if (!HasExistingRcloneConfig) return;
            Choice = StartupChoice.RcloneConfig;
            Close();
        }

        private async void ProvidePublic_Click(object? sender, PointerPressedEventArgs e)
        {
            if (await ShowProvideConfigDialog(true))
            {
                Choice = StartupChoice.Online;
                Close();
            }
        }

        private async void ProvideRclone_Click(object? sender, PointerPressedEventArgs e)
        {
            if (await ShowProvideConfigDialog(false))
            {
                Choice = StartupChoice.RcloneConfig;
                Close();
            }
        }

        private void Offline_Click(object? sender, PointerPressedEventArgs e)
        {
            Choice = StartupChoice.Offline;
            Close();
        }

        private void Button_PointerEntered(object? sender, PointerEventArgs e)
        {
            if (sender is Border b) b.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#2d3540"));
        }

        private void Button_PointerExited(object? sender, PointerEventArgs e)
        {
            if (sender is Border b) b.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1e232a"));
        }

        private async Task<bool> ShowProvideConfigDialog(bool isPublic)
        {
            var dialog = new ProvideConfigDialog(isPublic);
            return await dialog.ShowDialog<bool>(this);
        }
    }
}
