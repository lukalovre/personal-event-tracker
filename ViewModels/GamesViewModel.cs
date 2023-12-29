using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class GamesViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private GameGridItem _selectedGridItem;
    private List<Game> _itemList;
    private List<Event> _eventList;
    private Game _newItem;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;

    private bool _useNewDate;
    private Game _selectedItem;
    private int _gridCountItems;

    private int _gridCountItemsBookmarked;
    private int _addAmount;
    private string _addAmountString;

    public EventViewModel EventViewModel { get; }

    public int AddAmount
    {
        get => _addAmount;
        set { _addAmount = SetAmount(value); }
    }

    public string AddAmountString
    {
        get => _addAmountString;
        set => this.RaiseAndSetIfChanged(ref _addAmountString, value);
    }

    public bool UseNewDate
    {
        get => _useNewDate;
        set => this.RaiseAndSetIfChanged(ref _useNewDate, value);
    }

    public static ObservableCollection<string> MusicPlatformTypes =>
        new(
            Enum.GetValues(typeof(eGamePlatformTypes))
                .Cast<eGamePlatformTypes>()
                .Select(v => v.ToString())
        );

    public static ObservableCollection<PersonComboBoxItem> PeopleList =>
        new(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem SelectedPerson { get; set; }

    public ObservableCollection<GameGridItem> GridItems { get; set; }
    public ObservableCollection<GameGridItem> GridItemsBookmarked { get; set; }

    public Game SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }

    public Game NewItem
    {
        get => _newItem;
        set => this.RaiseAndSetIfChanged(ref _newItem, value);
    }

    public DateTime NewDate { get; set; } =
        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

    public TimeSpan NewTime { get; set; } = new TimeSpan();
    public Event NewEvent
    {
        get => _newEvent;
        set => this.RaiseAndSetIfChanged(ref _newEvent, value);
    }

    public Bitmap? Image
    {
        get => _itemImage;
        private set => this.RaiseAndSetIfChanged(ref _itemImage, value);
    }

    public Bitmap? NewImage
    {
        get => _newItemImage;
        private set => this.RaiseAndSetIfChanged(ref _newItemImage, value);
    }

    public int GridCountItems
    {
        get => _gridCountItems;
        private set => this.RaiseAndSetIfChanged(ref _gridCountItems, value);
    }

    public int GridCountItemsBookmarked
    {
        get => _gridCountItemsBookmarked;
        private set => this.RaiseAndSetIfChanged(ref _gridCountItemsBookmarked, value);
    }
    public GameGridItem SelectedGridItem
    {
        get => _selectedGridItem;
        set
        {
            _selectedGridItem = value;
            SelectedItemChanged();
        }
    }

    public GamesViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        GridItems = [];
        GridItemsBookmarked = [];
        ReloadData();

        Events = [];
        EventViewModel = new EventViewModel(Events, MusicPlatformTypes);

        AddItemClick = ReactiveCommand.Create(AddItemClickAction);
        AddEventClick = ReactiveCommand.Create(AddEventClickAction);

        SelectedGridItem = GridItems.LastOrDefault();
    }

    private int SetAmount(int value)
    {
        _addAmount = value;
        AddAmountString = $"    Adding {_addAmount} minutes";
        return value;
    }

    private void AddItemClickAction()
    {
        NewEvent.DateEnd = UseNewDate ? NewDate + NewTime : DateTime.Now;
        NewEvent.DateStart =
            NewEvent.DateEnd.Value.TimeOfDay.Ticks == 0
                ? NewEvent.DateEnd.Value
                : NewEvent.DateEnd.Value.AddMinutes(-NewEvent.Amount);
        NewEvent.People = SelectedPerson?.ID.ToString() ?? null;

        _datasource.Add(NewItem, NewEvent);

        ReloadData();
        ClearNewItemControls();
    }

    private void AddEventClickAction()
    {
        var lastEvent = Events.MaxBy(o => o.DateEnd);

        lastEvent.ID = 0;

        if (!EventViewModel.IsEditDate)
        {
            lastEvent.DateEnd = DateTime.Now;
        }

        lastEvent.DateStart =
            lastEvent.DateEnd.Value.TimeOfDay.Ticks == 0
                ? lastEvent.DateEnd.Value
                : lastEvent.DateEnd.Value.AddMinutes(-_addAmount);

        lastEvent.Platform = EventViewModel.SelectedPlatformType;
        lastEvent.Amount = _addAmount;

        // For now
        lastEvent.Comment = null;

        _datasource.Add(SelectedItem, lastEvent);

        ReloadData();
        ClearNewItemControls();
    }

    private void ReloadData()
    {
        GridItems.Clear();
        GridItems.AddRange(LoadData());
        GridCountItems = GridItems.Count;

        GridItemsBookmarked.Clear();
        GridItemsBookmarked.AddRange(LoadDataBookmarked());
        GridCountItemsBookmarked = GridItemsBookmarked.Count;
    }

    private void ClearNewItemControls()
    {
        NewItem = default;
        NewEvent = default;
        NewImage = default;
        SelectedPerson = default;
    }

    private List<GameGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Game>();
        _eventList = _datasource.GetEventList<Game>();

        return _eventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Select(
                (o, i) =>
                    Convert(
                        i,
                        o,
                        _itemList.First(m => m.ID == o.ItemID),
                        _eventList.Where(e => e.ItemID == o.ItemID)
                    )
            )
            .ToList();
    }

    private List<GameGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<Game>();
        _eventList = _datasource.GetEventList<Game>();

        var dateFilter = yearsAgo.HasValue
            ? DateTime.Now.AddYears(-yearsAgo.Value)
            : DateTime.MaxValue;

        return _eventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value <= dateFilter)
            .Where(o => o.Bookmakred)
            .Select(
                (o, i) =>
                    Convert(
                        i,
                        o,
                        _itemList.First(m => m.ID == o.ItemID),
                        _eventList.Where(e => e.ItemID == o.ItemID)
                    )
            )
            .ToList();
    }

    private static GameGridItem Convert(int index, Event e, Game i, IEnumerable<Event> eventList)
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

    public void SelectedItemChanged()
    {
        Events.Clear();
        Image = null;

        if (SelectedGridItem == null)
        {
            return;
        }

        SelectedItem = _itemList.First(o => o.ID == SelectedGridItem.ID);
        Events.AddRange(
            _eventList
                .Where(o => o.ItemID == SelectedItem.ID && o.DateEnd.HasValue)
                .OrderBy(o => o.DateEnd)
        );

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepsitory.GetImage<Game>(item.ID);
    }
}
