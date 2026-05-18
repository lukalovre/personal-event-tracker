using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class DnDViewModel(IDatasource datasource) : ItemViewModel<DnD, DnDGridItem>(datasource, null!)
{
    protected override DnDGridItem Convert(Event e, DnD i, IEnumerable<Event> eventList)
    {
        return new DnDGridItem(
            i.ID,
            i.Title,
            eventList.LastEventDate());
    }
}