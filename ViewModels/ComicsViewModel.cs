using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ComicsViewModel : ViewModelBase
{
    private const float AMOUNT_TO_MINUTES_MODIFIER = 0.3f;
    private readonly IDatasource _datasource;
    private readonly IExternal<Comic> _external;
    private ComicGridItem _selectedGridItem;
    private List<Comic> _itemList;
    private List<Event> _eventList;
    private Comic _newItem;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;

    private bool _useNewDate;
    private Comic _selectedItem;
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

    private int _newAmount;
    private string _inputUrl;

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

    public static ObservableCollection<string> MusicPlatformTypes => [];

    public static ObservableCollection<PersonComboBoxItem> PeopleList =>
        new(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem SelectedPerson { get; set; }

    public ObservableCollection<ComicGridItem> GridItems { get; set; }
    public ObservableCollection<ComicGridItem> GridItemsBookmarked { get; set; }

    public Comic SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public DateTime NewDateEnd { get; set; }

    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }

    public Comic NewItem
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
    public ComicGridItem SelectedGridItem
    {
        get => _selectedGridItem;
        set
        {
            _selectedGridItem = value;
            SelectedItemChanged();
        }
    }

    public string InputUrl
    {
        get => _inputUrl;
        set
        {
            this.RaiseAndSetIfChanged(ref _inputUrl, value);
            InputUrlChanged();
        }
    }

    public ComicsViewModel(IDatasource datasource, IExternal<Comic> external)
    {
        _datasource = datasource;
        _external = external;

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
        var events = _eventList.Where(o => o.ItemID == SelectedItem.ID);
        var currentAmount = GetComicPages(events);
        var newAmount = value - currentAmount;

        _newAmount = newAmount;
        AddAmountString = $"    Adding {newAmount} pages";
        return value;
    }

    private void InputUrlChanged()
    {
        NewItem = _external.GetItem(InputUrl);

        NewImage = FileRepsitory.GetImageTemp<Comic>();
        NewEvent = new Event
        {
            Rating = 1
        };

        _inputUrl = string.Empty;
    }

    private void AddItemClickAction()
    {
        NewEvent.DateEnd = UseNewDate ? NewDateEnd : DateTime.Now;
        NewEvent.DateStart = CalculateDateStart(NewEvent, NewEvent.Amount);
        NewEvent.People = SelectedPerson?.ID.ToString() ?? null;

        _datasource.Add(NewItem, NewEvent);

        ReloadData();
        ClearNewItemControls();
    }

    private void AddEventClickAction()
    {
        var lastEvent = Events.MaxBy(o => o.DateEnd);

        lastEvent.ID = 0;

        lastEvent.DateEnd = !EventViewModel.IsEditDate
        ? DateTime.Now
        : EventViewModel.SelectedEvent.DateEnd;

        lastEvent.DateStart = CalculateDateStart(lastEvent, _newAmount);
        lastEvent.Platform = EventViewModel.SelectedPlatformType;
        lastEvent.Amount = _newAmount;
        lastEvent.Chapter = EventViewModel.NewEventChapter;

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

    private static DateTime CalculateDateStart(Event e, int amount)
    {
        return e.DateEnd.Value.TimeOfDay.Ticks == 0
             ? e.DateEnd.Value
             : e.DateEnd.Value.AddMinutes(-amount * AMOUNT_TO_MINUTES_MODIFIER);
    }
    private void ClearNewItemControls()
    {
        NewItem = default;
        NewEvent = default;
        NewImage = default;
        SelectedPerson = default;
    }

    private List<ComicGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Comic>();
        _eventList = _datasource.GetEventList<Comic>();

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

    private List<ComicGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<Comic>();
        _eventList = _datasource.GetEventList<Comic>();

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

    private int GetComicPages(IEnumerable<Event> eventList)
    {
        // This is for the case that the Comic is already completed by you are rereading it.
        var lastCompletedDate = eventList.Where(o => o.Completed)?.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var dateFilter = lastCompletedDate;

        var lastChapter = eventList.LastOrDefault()?.Chapter ?? 1;

        if (EventViewModel is not null && lastChapter < EventViewModel.NewEventChapter)
        {
            lastChapter = EventViewModel.NewEventChapter;
        }

        var eventsByChapter = eventList.Where(o => o.Chapter == lastChapter);

        if (lastCompletedDate == lastDate)
        {
            return eventsByChapter.Sum(o => o.Amount);
        }

        return eventsByChapter.Where(o => o.DateEnd > dateFilter).Sum(o => o.Amount);
    }

    private ComicGridItem Convert(int index, Event e, Comic i, IEnumerable<Event> eventList)
    {
        return new ComicGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Writer,
            e.Chapter,
            GetComicPages(eventList),
            e.Rating
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
        Image = FileRepsitory.GetImage<Comic>(item.ID);
    }
}
