using System.Collections.Generic;
using System.Linq;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class MagazinesViewModel(IDatasource datasource) : ItemViewModel<Magazine, MagazinesGridItem>(datasource, null!)
{
    protected override MagazinesGridItem Convert(Event e, Magazine i, IEnumerable<Event> eventList)
    {
        return new MagazinesGridItem(
            i.ID,
            i.Title,
            eventList.Last().Chapter ?? 0,
            eventList.LastEventDate());
    }
}