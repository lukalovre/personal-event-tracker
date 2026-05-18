using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class ConcertsViewModel(IDatasource datasource) : ItemViewModel<Concert, ConcertsGridItem>(datasource, null!)
{
    protected override ConcertsGridItem Convert(Event e, Concert i, IEnumerable<Event> eventList)
    {
        return new ConcertsGridItem(
            i.ID,
            i.Artist,
            i.Festival,
            i.Venue,
            i.City,
            i.Country,
            i.Price ?? 0,
            eventList.LastEventDate().Year,
            eventList.LastEventDate());
    }
}