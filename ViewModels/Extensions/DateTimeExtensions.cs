using System;

namespace AvaloniaApplication1.ViewModels.Extensions;

public static class DateTimeExtensions
{
    public static int DaysAgo(this DateTime dateTime)
    {
        return (int)(DateTime.Now - dateTime).TotalDays;
    }
}
