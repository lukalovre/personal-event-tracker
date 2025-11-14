using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EventTracker.Models;
using EventTracker.Repositories;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class GamesViewModel(IDatasource datasource, IExternal<Game> external) : ItemViewModel<Game, GameGridItem>(datasource, external)
{
    protected override GameGridItem Convert(Event e, Game i, IEnumerable<Event> eventList)
    {
        return new GameGridItem(
            i.ID,
            i.Title,
            i.Year,
            i.Platform,
            eventList.Sum(o => o.Amount),
            e.Completed,
            e?.Rating ?? 0,
            eventList.LastEventDate());
    }
}
