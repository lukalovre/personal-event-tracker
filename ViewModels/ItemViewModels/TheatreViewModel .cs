using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class TheatreViewModel(IDatasource datasource) : ItemViewModel<Theatre, TheatreGridItem>(datasource, null!)
{
    protected override TheatreGridItem Convert(Event e, Theatre i, IEnumerable<Event> eventList)
    {
        return new TheatreGridItem(
            i.ID,
            i.Title,
            i.Writer,
            i.Director,
            e?.Rating ?? 0,
            eventList.LastEventDate()
        );
    }
}
