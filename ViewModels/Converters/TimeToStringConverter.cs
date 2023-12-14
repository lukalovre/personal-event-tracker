using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class TimeToStringConverter : IValueConverter
{
    private const string TIME_FORMAT = @"hh\:mm";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int time)
        {
            return TimeSpan.FromMinutes(time).ToString(TIME_FORMAT);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return TimeSpan.ParseExact(
                str,
                TIME_FORMAT,
                CultureInfo.InvariantCulture,
                TimeSpanStyles.None
            );
        }

        return null;
    }
}
