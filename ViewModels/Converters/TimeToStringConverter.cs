using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class TimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int time)
        {
            return GetFormatedTime(time);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            // return DateTime.ParseExact(str, (string)parameter, CultureInfo.InvariantCulture);
        }

        return null;
    }

    public static string GetFormatedTime(int minutes)
    {
        var hours = minutes / 60;
        var min = minutes % 60;

        var hText = hours == 0 ? string.Empty : $"{hours}h";
        var mText = min == 0 ? string.Empty : $"{min}min";

        var space = hours == 0 || min == 0 ? string.Empty : " ";

        return hText + space + mText;
    }
}
