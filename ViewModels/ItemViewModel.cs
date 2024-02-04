using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using DynamicData;
using Newtonsoft.Json;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class ItemViewModel<TItem, TGridItem> : ViewModelBase
where TItem : IItem
where TGridItem : IGridItem
{
    public ItemViewModel(IDatasource datasource, IExternal<TItem> external)
    {
        _datasource = datasource;
        _external = external;

        GridItems = [];
        GridItemsBookmarked = [];
        ReloadData();

        Events = [];
        EventViewModel = new EventViewModel(Events, PlatformTypes);

        AddItemClick = ReactiveCommand.Create(AddItemClickAction);
        AddEventClick = ReactiveCommand.Create(AddEventClickAction);
        Search = ReactiveCommand.Create(SearchAction);
        OpenLink = ReactiveCommand.Create(OpenLinkAction);
        OpenImage = ReactiveCommand.Create(OpenImageAction);

        SelectedGridItem = GridItems.LastOrDefault();
        NewEvent = new Event();
        NewItem = (TItem)Activator.CreateInstance(typeof(TItem));

        IsFullAmount = IsFullAmountDefaultValue;
    }

    protected virtual bool IsFullAmountDefaultValue => true;

    protected virtual float AmountToMinutesModifier => 1f;
    private readonly IDatasource _datasource;
    private readonly IExternal<TItem> _external;
    private TGridItem _selectedGridItem;
    private List<TItem> _itemList;
    private List<Event> _eventList;
    private TItem _newItem;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;
    private bool _useNewDate;
    private TItem _selectedItem;
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
    private bool _isFullAmount;
    private int _newItemAmount;

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

    public bool IsFullAmount
    {
        get => _isFullAmount;
        set => this.RaiseAndSetIfChanged(ref _isFullAmount, value);
    }

    public virtual ObservableCollection<string> PlatformTypes => [];

    public static ObservableCollection<PersonComboBoxItem> PeopleList => new(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem SelectedPerson { get; set; }

    public ObservableCollection<TGridItem> GridItems { get; set; }
    public ObservableCollection<TGridItem> GridItemsBookmarked { get; set; }

    public TItem SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }
    public ReactiveCommand<Unit, Unit> Search { get; }
    public ReactiveCommand<Unit, Unit> OpenLink { get; }
    public ReactiveCommand<Unit, Unit> OpenImage { get; }

    public TItem NewItem
    {
        get => _newItem;
        set => this.RaiseAndSetIfChanged(ref _newItem, value);
    }

    public DateTime NewDate { get; set; } = DateTime.Now;

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

    public int NewItemAmount
    {
        get => _newItemAmount;
        set => this.RaiseAndSetIfChanged(ref _newItemAmount, value);
    }

    public TGridItem SelectedGridItem
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

    protected virtual string AmountVerb => "minutes";

    public string SearchText { get; set; }

    private void SearchAction()
    {
        SearchText = SearchText.Trim();

        GridItems.Clear();
        GridItems.AddRange(LoadData());
        SelectedGridItem = GridItems.LastOrDefault();
    }

    private int SetAmount(int value)
    {
        var events = _eventList.Where(o => o.ItemID == SelectedItem.ID);
        var currentAmount = IsFullAmount ? 0 : GetItemAmount(events);
        var newAmount = value - currentAmount;

        _newAmount = newAmount;
        AddAmountString = $"    Adding {newAmount} {AmountVerb}";
        return value;
    }

    protected virtual int DefaultNewItemRating => 1;
    protected virtual int? DefaultNewItemChapter => EventViewModel.NewEventChapter;
    protected virtual string DefaultNewItemPlatform => string.Empty;
    protected virtual bool DefaultNewItemCompleted => false;
    protected virtual bool DefaultNewItemBookmakred => false;

    private void InputUrlChanged()
    {
        NewItem = _external.GetItem(InputUrl);

        NewImage = FileRepsitory.GetImageTemp<TItem>();
        NewEvent = new Event
        {
            Rating = DefaultNewItemRating,
            Platform = DefaultNewItemPlatform,
            Completed = DefaultNewItemCompleted,
            Bookmakred = DefaultNewItemBookmakred
        };

        _inputUrl = string.Empty;
    }

    private void OpenImageAction()
    {
        throw new NotImplementedException();
    }

    protected virtual string OpenLinkUrl => string.Empty;
    protected virtual List<string> GetAlternativeOpenLinkSearchParams() => [];

    private void OpenLinkAction()
    {
        HtmlHelper.OpenLink(OpenLinkUrl, [.. GetAlternativeOpenLinkSearchParams()]);
    }

    protected virtual int? NewItemAmountOverride => null;

    private void AddItemClickAction()
    {
        var amount = NewItemAmountOverride ?? NewItemAmount;
        var dateEnd = UseNewDate ? NewDate : DateTime.Now;
        var dateStart = CalculateDateStart(dateEnd, amount);
        var people = SelectedPerson?.ID.ToString() ?? string.Empty;

        NewEvent ??= new Event();

        var newEvent = new Event
        {
            ID = 0,
            ItemID = 0,
            ExternalID = string.Empty,
            DateStart = dateStart,
            DateEnd = dateEnd,
            Rating = NewEvent.Rating,
            Bookmakred = NewEvent.Bookmakred,
            Chapter = NewEvent.Chapter,
            Amount = amount,
            AmountType = NewEvent.AmountType,
            Completed = NewEvent.Completed,
            Comment = NewEvent.Comment,
            People = people,
            Platform = NewEvent.Platform,
            LocationID = NewEvent.LocationID
        };

        _datasource.Add(NewItem, newEvent);

        ReloadData();
        ClearNewItemControls();
    }

    private void AddEventClickAction()
    {
        var lastEvent = Events.MaxBy(o => o.DateEnd) ?? Events.LastOrDefault();

        var amount = _newAmount == 0
        ? lastEvent.Amount
        : _newAmount;

        var dateEnd = !EventViewModel.IsEditDate
        ? DateTime.Now
        : EventViewModel.SelectedEvent.DateEnd.Value;
        var dateStart = CalculateDateStart(dateEnd, amount);

        var newEvent = new Event
        {
            ID = 0,
            ItemID = 0,
            ExternalID = string.Empty,
            DateStart = dateStart,
            DateEnd = dateEnd,
            Rating = lastEvent.Rating,
            Bookmakred = lastEvent.Bookmakred,
            Chapter = DefaultNewItemChapter,
            Amount = amount,
            AmountType = lastEvent.AmountType,
            Completed = lastEvent.Completed,
            Comment = lastEvent.Comment,
            People = lastEvent.People,
            Platform = EventViewModel.SelectedPlatformType,
            LocationID = lastEvent.LocationID
        };

        _datasource.Add(SelectedItem, newEvent);

        ReloadData();
        ClearNewItemControls();
    }

    protected virtual void ReloadData()
    {
        GridItems.Clear();
        GridItems.AddRange(LoadData());
        GridCountItems = GridItems.Count;

        GridItemsBookmarked.Clear();
        GridItemsBookmarked.AddRange(LoadDataBookmarked());
        GridCountItemsBookmarked = GridItemsBookmarked.Count;
    }

    private DateTime CalculateDateStart(DateTime dateEnd, int amount)
    {
        return dateEnd.TimeOfDay.Ticks == 0
             ? dateEnd
             : dateEnd.AddMinutes(-amount * AmountToMinutesModifier);
    }
    private void ClearNewItemControls()
    {
        NewItem = default;
        NewEvent = default;
        NewImage = default;
        SelectedPerson = default;
    }

    protected virtual DateTime? DateTimeFilter => new DateTime(DateTime.Now.Year, 1, 1);

    private List<TGridItem> LoadData()
    {
        _itemList = _datasource.GetList<TItem>();
        _eventList = _datasource.GetEventList<TItem>();

        var result = _eventList
                    .OrderByDescending(o => o.DateEnd)
                    .DistinctBy(o => o.ItemID)
                    .OrderBy(o => o.DateEnd)
                    .ToList();

        if (DateTimeFilter.HasValue && string.IsNullOrWhiteSpace(SearchText))
        {
            result = result
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value >= DateTimeFilter.Value)
            .ToList();
        }

        var resultGrid = result.Select((o, i) =>
                        Convert(
                            i,
                            o,
                            _itemList.First(m => m.ID == o.ItemID),
                            _eventList.Where(e => e.ItemID == o.ItemID)
                        )
                        ).ToList();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            resultGrid = resultGrid
            .Where(o =>
                JsonConvert.SerializeObject(o)
                .Contains(SearchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        }

        return resultGrid;
    }

    protected List<TGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<TItem>();
        _eventList = _datasource.GetEventList<TItem>();

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

    protected virtual TGridItem Convert(int index, Event e, TItem i, IEnumerable<Event> events)
    {
        return default;
    }

    protected int GetItemAmount(IEnumerable<Event> eventList)
    {
        // This is for the case that the Item is already completed by you are rereading it.
        var lastCompletedDate = eventList.Where(o => o.Completed)?.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var dateFilter = lastCompletedDate;

        var lastChapter = eventList.LastOrDefault()?.Chapter;

        if (EventViewModel is not null
            && lastChapter.HasValue
            && lastChapter < EventViewModel.NewEventChapter)
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

        if (!Events.Any())
        {
            Events.AddRange(_eventList.Where(o => o.ItemID == SelectedItem.ID));
        }

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepsitory.GetImage<TItem>(item.ID);
    }
}