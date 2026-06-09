using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AndroidSideloader.Desktop.Converters
{
    public class HexToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrEmpty(hex))
            {
                try
                {
                    var color = Color.Parse(hex);
                    return new SolidColorBrush(color);
                }
                catch
                {
                    return Brushes.White;
                }
            }
            return Brushes.White;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
