using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public partial class ClipsViewModel(IDatasource datasource, IExternal<Clip> external) : ItemViewModel<Clip, ClipGridItem>(datasource, external)
{
    protected override ClipGridItem Convert(Event e, Clip i, IEnumerable<Event> eventList)
    {
        return new ClipGridItem(i.ID, i.Author, i.Title, e?.Rating ?? 1);
    }
}
