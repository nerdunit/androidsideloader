using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AndroidSideloader.Desktop.ViewModels;

namespace AndroidSideloader.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
