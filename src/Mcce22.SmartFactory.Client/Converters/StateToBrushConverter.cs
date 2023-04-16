using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Mcce22.SmartFactory.Client.Converters
{
    public class StateToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush _defaultBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#333");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultBrush = parameter as Brush ?? _defaultBrush;
            return (bool)value == true ? Brushes.Green : defaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
