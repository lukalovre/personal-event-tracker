using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class PaintingsViewModel(IDatasource datasource) : ItemViewModel<Painting, PaintingsGridItem>(datasource, null!)
{
    protected override PaintingsGridItem Convert(Event e, Painting i, IEnumerable<Event> eventList)
    {
        return new PaintingsGridItem(
            i.ID,
            i.Title,
            i.Author,
            i.Year,
            eventList.LastEventDate());
    }
}