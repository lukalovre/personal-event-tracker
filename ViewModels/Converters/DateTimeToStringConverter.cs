using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString(parameter?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            return DateTime.ParseExact(str, parameter?.ToString() ?? string.Empty, CultureInfo.InvariantCulture);
        }

        return DateTime.MinValue;
    }
}
