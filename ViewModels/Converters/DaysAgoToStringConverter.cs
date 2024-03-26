using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaApplication1.ViewModels.Extensions;

public class DaysAgoToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.DaysAgoString();
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            var split = str.Split(" ");

            int daysAgo;

            if (split.Length == 1)
            {
                daysAgo = int.Parse(str);
            }
            else
            {
                int years = int.Parse(split[0].TrimEnd("y"));
                int days = int.Parse(split[1]);

                daysAgo = (years * 365) + days;
            }

            return DateTime.Now.AddDays(-daysAgo);
        }

        return DateTime.MinValue;
    }
}
