using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaApplication1.ViewModels.Extensions;

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

            if (str.EndsWith("UTC"))
            {
                str = str.TrimEnd("UTC").Trim();

                DateTime convertedDate = DateTime.SpecifyKind(DateTime.Parse(str), DateTimeKind.Utc);
                var kind = convertedDate.Kind;
                return convertedDate.ToLocalTime();
            }

            if (DateTime.TryParseExact(
                str,
                parameter?.ToString() ?? string.Empty,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result))
            {
                return result;
            }
        }

        return null!;
    }
}
