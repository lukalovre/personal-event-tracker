using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaApplication1.ViewModels.Extensions;

public class TimeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int minutes)
        {
            var hours = minutes / 60;
            var min = minutes % 60;

            var hText = hours == 0 ? string.Empty : $"{hours}h";
            var mText = min == 0 ? string.Empty : $"{min}m";

            var space = hours == 0 || min == 0 ? string.Empty : " ";

            return hText + space + mText;
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            var split = str.Split(" ");

            if (split.Length == 1)
            {
                if (str.Contains('h'))
                {
                    return int.Parse(str.TrimEnd("h")) * 60;
                }

                return int.Parse(str.TrimEnd("m"));
            }
            else
            {
                int hours = int.Parse(split[0].TrimEnd("h"));
                int minutes = int.Parse(split[1].TrimEnd("m"));

                return (hours * 60) + minutes;
            }
        }

        return 0;
    }
}
