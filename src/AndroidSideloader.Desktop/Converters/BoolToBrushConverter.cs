using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AndroidSideloader.Desktop.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public IBrush TrueBrush { get; set; } = Brushes.Green;
        public IBrush FalseBrush { get; set; } = Brushes.Red;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? TrueBrush : FalseBrush;
            return FalseBrush;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
