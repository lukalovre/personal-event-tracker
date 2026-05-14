using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    public ObservableCollection<DeveloperGridItem> GameDeveloperList { get; set; } = [];

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

        GameDeveloperList.Clear();
        GameDeveloperList.AddRange(await LoadDataByDeveloper());
    }

    private async Task<List<DeveloperGridItem>> LoadDataByDeveloper()
    {
        var resultGrid = new List<DeveloperGridItem>();

        var type = Helpers.GetClassName<Game>();
        var itemList = _datasource.GetList<Game>(type);
        var eventList = _datasource.GetEventList(type);

        var developerList = itemList.DistinctBy(o => o.Developer).Select(o => o.Developer);

        foreach (var developer in developerList)
        {
            var gamesList = itemList.Where(o => o.Developer == developer);
            var minutesDeveloper = 0;

            foreach (var game in gamesList)
            {
                var minutesGame = eventList.Where(o => o.ItemID == game.ID).Sum(o => o.Amount);
                minutesDeveloper += minutesGame;
            }

            var gridItem = new DeveloperGridItem(1, developer, minutesDeveloper, gamesList.Count());
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Minutes).ToList();
        return resultGrid;
    }
}
