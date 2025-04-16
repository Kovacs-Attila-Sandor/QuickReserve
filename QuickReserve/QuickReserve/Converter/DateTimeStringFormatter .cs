using System;
using System.Globalization;
using Xamarin.Forms;

namespace QuickReserve.Converter
{
    public class DateTimeStringFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString && DateTime.TryParse(dateString, out DateTime date))
            {
                return date.ToString("yyyy. MMMM dd.", new CultureInfo("en-US"));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}