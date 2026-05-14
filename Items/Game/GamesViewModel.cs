using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using EventTracker.Models;
using EventTracker.Repositories;
using EventTracker.ViewModels.Extensions;
using ReactiveUI;
using Repositories;

namespace EventTracker.ViewModels;

public partial class GamesViewModel(IDatasource datasource, IExternal<Game> external) : ItemViewModel<Game, GameGridItem>(datasource, external)
{
    public ObservableCollection<GameGridItem> GameTimeList { get; set; } = [];

    private int _gridCountGameTimeList;

    public int GridCountGameTimeList
    {
        get => _gridCountGameTimeList;
        private set => this.RaiseAndSetIfChanged(ref _gridCountGameTimeList, value);
    }

    protected override GameGridItem Convert(Event e, Game i, IEnumerable<Event> eventList)
    {
        return new GameGridItem(
            i.ID,
            i.Title,
            i.Developer,
            i.Year,
            i.Platform,
            eventList.Sum(o => o.Amount),
            e.Completed,
            e?.Rating ?? 0,
            eventList.LastEventDate());
    }

    protected override async void ReloadData()
    {
        base.ReloadData();

        GameTimeList.Clear();
        var list = await LoadData(skipFilters: true);
        list = list.OrderByDescending(o => o.Time).ToList();

        GameTimeList.AddRange(list);
        GridCountGameTimeList = GameTimeList.Count;
    }
}
