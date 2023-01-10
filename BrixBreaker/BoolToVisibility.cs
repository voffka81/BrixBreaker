using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrackOut
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
                return isVisible ? Visibility.Visible : Visibility.Collapsed;

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
