using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString((string)parameter);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return DateTime.ParseExact(str, (string)parameter, CultureInfo.InvariantCulture);
        }

        return null;
    }
}
