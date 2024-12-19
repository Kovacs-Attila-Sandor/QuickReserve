using System;
using System.Globalization;
using Xamarin.Forms;

namespace QuickReserve.Converter
{
    public class ItemsHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int itemCount && int.TryParse(parameter?.ToString(), out int itemHeight))
            {
                return itemCount * itemHeight;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
