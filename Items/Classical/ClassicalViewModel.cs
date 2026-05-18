using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class ClassicalViewModel(IDatasource datasource) : ItemViewModel<Classical, ClassicalGridItem>(datasource, null!)
{
    protected override ClassicalGridItem Convert(Event e, Classical i, IEnumerable<Event> eventList)
    {
        return new ClassicalGridItem(
            i.ID,
            i.Title,
            eventList.LastEventDate());
    }
}