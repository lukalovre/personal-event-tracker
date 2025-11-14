using System.Collections.Generic;
using System.Linq;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class AdventuresViewModel(IDatasource datasource) : ItemViewModel<Adventure, AdventureGridItem>(datasource, null!)
{
    protected override AdventureGridItem Convert(Event e, Adventure i, IEnumerable<Event> eventList)
    {
        return new AdventureGridItem(
            i.ID,
            i.Title,
            i.City,
            eventList.Count(),
            eventList.LastEventDate().Date.Year,
            eventList.LastEventDate());
    }
}
