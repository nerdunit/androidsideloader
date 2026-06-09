using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AndroidSideloader.Desktop.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AndroidSideloader.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            AddHandler(DragDrop.DropEvent, OnDrop);
        }

        private async void OnDrop(object? sender, DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files)) return;

            var files = e.Data.GetFiles();
            if (files == null) return;

            var apkFiles = files.Where(f => f.Path.Scheme == "file")
                                .Select(f => f.Path.LocalPath)
                                .Where(p => !string.IsNullOrEmpty(p) && p.EndsWith(".apk"))
                                .ToList();

            if (apkFiles.Count == 0) return;

            if (DataContext is MainWindowViewModel vm)
            {
                vm.StatusText = $"Installing {apkFiles.Count} dropped APK(s)...";
            }

            foreach (var apk in apkFiles)
            {
                try
                {
                    await ADB.SideloadWithProgressAsync(
                        apk!,
                        (progress, eta) => { },
                        status => { if (DataContext is MainWindowViewModel v) v.StatusText = status; });
                }
                catch { }
            }

            if (DataContext is MainWindowViewModel vm2)
            {
                vm2.StatusText = $"Installed {apkFiles.Count} APK(s)";
            }
        }
    }
}
