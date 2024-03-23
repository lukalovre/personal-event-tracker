using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.ViewModels.Extensions;

public static class DateTimeExtensions
{
    public static int DaysAgo(this DateTime dateTime)
    {
        return (DateTime.Now - dateTime).Days;
    }

    public static string DaysAgoString(this DateTime dateTime)
    {
        int totalDays = (DateTime.Now - dateTime).Days;
        int years = totalDays / 365;
        int days = totalDays - years * 365;

        var yearString = years == 0 ? string.Empty : $"{years}y";
        var daysString = $"{days}";

        var list = new List<string> { yearString, daysString };
        list.RemoveAll(string.IsNullOrWhiteSpace);
        return string.Join(" ", list);
    }
}
