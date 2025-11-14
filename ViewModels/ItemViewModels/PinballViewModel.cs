using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class PinballViewModel(IDatasource datasource) : ItemViewModel<Pinball, PinballGridItem>(datasource, null!)
{
    protected override PinballGridItem Convert(Event e, Pinball i, IEnumerable<Event> eventList)
    {
        return new PinballGridItem(
            i.ID,
            i.Title,
            i.Year ?? 0,
            eventList.LastEventDate());
    }
}