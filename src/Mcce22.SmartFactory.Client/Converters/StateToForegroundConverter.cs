using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Mcce22.SmartFactory.Client.Converters
{
    public class StateToForegroundConverter : IValueConverter
    {
        private static readonly Brush ActiveBrush = Brushes.Orange;
        private static readonly Brush InactiveBrush = Brushes.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? ActiveBrush : InactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
