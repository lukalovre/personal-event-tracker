using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

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
