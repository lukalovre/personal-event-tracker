using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
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

    private void AddItemClickAction()
    {
        NewItem.Platform = NewEvent.Platform;
    }

    protected override GameGridItem Convert(int index, Event e, Game i, IEnumerable<Event> eventList)
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new GameGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Year,
            i.Platform,
            eventList.Sum(o => o.Amount),
            e.Completed,
            lastDate,
            daysAgo
        );
    }
}
