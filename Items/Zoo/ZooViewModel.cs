using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class ZooViewModel(IDatasource datasource) : ItemViewModel<Zoo, ZooGridItem>(datasource, null!)
{
    protected override ZooGridItem Convert(Event e, Zoo i, IEnumerable<Event> eventList)
    {
        return new ZooGridItem(
            i.ID,
            i.Title,
            i.City,
            i.Country,
            eventList.LastEventDate().Date.Year);
    }
}
