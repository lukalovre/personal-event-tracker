using Avalonia.Controls;

namespace EventTracker.Views;

public class ViewHelper
{
    public static void AddConverters(IResourceDictionary resources)
    {
        resources.Add("TimeToStringConverter", new TimeToStringConverter());
        resources.Add("DaysAgoToStringConverter", new DaysAgoToStringConverter());
        resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
    }
}
