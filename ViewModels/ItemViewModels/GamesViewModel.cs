using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.Extensions;
using DynamicData;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class GamesViewModel(IDatasource datasource, IExternal<Game> external)
: ItemViewModel<Game, GameGridItem>(datasource, external)
{
    public override ObservableCollection<string> PlatformTypes =>
        new(
            Enum.GetValues(typeof(eGamePlatformTypes))
                .Cast<eGamePlatformTypes>()
                .Select(v => v.ToString())
        );

    protected override int? DefaultNewItemChapter => null;
    protected override GameGridItem Convert(Event e, Game i, IEnumerable<Event> eventList)
    {
        return new GameGridItem(
            i.ID,
            i.Title,
            i.Year,
            i.Platform,
            eventList.Sum(o => o.Amount),
            e.Completed,
            eventList.LastEventDate());
    }
}
