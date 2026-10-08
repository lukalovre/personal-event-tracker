using System;
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
    public ObservableCollection<GameGridItem> DeveloperGameList { get; set; } = [];

    private int _selectedGameTabIndex;
    public int SelectedGameTabIndex
    {
        get => _selectedGameTabIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedGameTabIndex, value);
            if (value != 3 && SelectedRightTabIndex == 3)
            {
                SelectedRightTabIndex = 0;
            }

            IsDeveloperGameTabVisible = value == 3;
        }
    }

    private bool _isDeveloperGameTabVisible;
    public bool IsDeveloperGameTabVisible
    {
        get => _isDeveloperGameTabVisible;
        private set => this.RaiseAndSetIfChanged(ref _isDeveloperGameTabVisible, value);
    }

    private DeveloperGridItem? _selectedDeveloperGridItem;
    public DeveloperGridItem? SelectedDeveloperGridItem
    {
        get => _selectedDeveloperGridItem;
        set
        {
            if (value == _selectedDeveloperGridItem)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _selectedDeveloperGridItem, value);
            SelectedDeveloperGame = null;
            SelectedRightTabIndex = value is null ? 0 : 3;
            DeveloperGameList.Clear();

            if (value is null)
            {
                return;
            }

            LoadItemsAndEvents(out List<Game> itemList, out List<Event> eventList);
            foreach (var game in itemList.Where(o => GetDevelopers(o).Contains(value.Developer)).OrderByDescending(o => o.Year).ThenBy(o => o.Title))
            {
                var minutes = eventList.Where(o => o.ItemID == game.ID).Sum(o => o.Amount);
                DeveloperGameList.Add(new GameGridItem(game.ID, game.Title, game.Developer, game.Year, game.Platform, minutes, false, 0, null));
            }
        }
    }

    private GameGridItem? _selectedDeveloperGame;
    public GameGridItem? SelectedDeveloperGame
    {
        get => _selectedDeveloperGame;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDeveloperGame, value);
            SelectedGridItem = value!;
        }
    }

    private int _selectedRightTabIndex;
    public int SelectedRightTabIndex
    {
        get => _selectedRightTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedRightTabIndex, value);
    }

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

        LoadItemsAndEvents(out List<Game> itemList, out List<Event> eventList);

        var gamesByDeveloper = itemList
            .SelectMany(game => GetDevelopers(game).Select(developer => (Developer: developer, Game: game)))
            .GroupBy(o => o.Developer);

        foreach (var developerGames in gamesByDeveloper)
        {
            var gamesList = developerGames.Select(o => o.Game).ToList();
            var minutesDeveloper = 0;

            foreach (var game in gamesList)
            {
                var minutesGame = eventList.Where(o => o.ItemID == game.ID).Sum(o => o.Amount);
                minutesDeveloper += minutesGame;
            }

            var gridItem = new DeveloperGridItem(1, developerGames.Key, minutesDeveloper, gamesList.Count);
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Minutes).ToList();
        return resultGrid;
    }

    private static IEnumerable<string> GetDevelopers(Game game)
    {
        return game.Developer
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct();
    }
}
