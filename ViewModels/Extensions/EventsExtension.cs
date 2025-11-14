using System;
using System.Collections.Generic;
using System.Linq;
using EventTracker.Models;

namespace EventTracker.ViewModels.Extensions;

public static class EventsExtension
{
    public static DateTime LastEventDate(this IEnumerable<Event> eventList)
    {
        return eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
    }
}
