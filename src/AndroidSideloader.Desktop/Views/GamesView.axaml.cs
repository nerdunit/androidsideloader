using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AndroidSideloader.Desktop.ViewModels;

namespace AndroidSideloader.Desktop.Views
{
    public partial class GamesView : UserControl
    {
        private TextBox? _searchTextBox;

        public GamesView()
        {
            InitializeComponent();
            _searchTextBox = this.FindControl<TextBox>("SearchTextBox");
            KeyDown += GamesView_KeyDown;
        }

        private void GamesView_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && e.KeyModifiers == KeyModifiers.Control)
            {
                _searchTextBox?.Focus();
                e.Handled = true;
            }
        }

        private void GalleryItem_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control ctrl && ctrl.DataContext is GameItemViewModel game)
            {
                if (DataContext is GamesViewModel vm)
                {
                    vm.SelectedGame = game;
                }
            }
        }
    }
}
