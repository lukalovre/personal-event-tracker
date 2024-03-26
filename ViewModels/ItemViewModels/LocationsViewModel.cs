using System;
using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class LocationsViewModel(IDatasource datasource) : ItemViewModel<Location, LocationGridItem>(datasource, null!)
{
    protected override DateTime? DateTimeFilter => DateTime.MinValue;

    protected override LocationGridItem Convert(Event e, Location i, IEnumerable<Event> eventList)
    {
        return new LocationGridItem(
            i.ID,
            i.Name,
            i.City,
            i.Country,
            eventList.LastEventDate().Date.Year);
    }
}
