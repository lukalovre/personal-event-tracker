using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.ViewModels.Extensions;

public static class EventsExtension
{
    public static DateTime LastEventDate(this IEnumerable<Event> eventList)
    {
        return eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
    }
}
