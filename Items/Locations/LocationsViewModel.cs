using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class LocationsViewModel(IDatasource datasource) : ItemViewModel<Location, LocationGridItem>(datasource, null!)
{
    protected override LocationGridItem Convert(Event e, Location i, IEnumerable<Event> eventList)
    {
        return new LocationGridItem(
            i.ID,
            i.Title,
            i.City,
            i.Country,
            eventList.LastEventDate().Date.Year);
    }
}
