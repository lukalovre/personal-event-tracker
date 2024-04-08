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

public class ItemViewModel<TItem, TGridItem> : ViewModelBase, IDataGrid where TItem : IItem where TGridItem : IGridItem
{
    public ItemViewModel(IDatasource datasource, IExternal<TItem> external)
    {
        _datasource = datasource;
        _external = external;

        GridFilterViewModel = new GridFilterViewModel(this);
        People = new PeopleSelectionViewModel();

        GridItems = [];
        GridItemsBookmarked = [];
        ReloadData();

        Events = [];
        EventViewModel = new EventViewModel(Events, PlatformTypes);

        AddItemClick = ReactiveCommand.Create(AddItemClickAction);
        AddEventClick = ReactiveCommand.Create(AddEventClickAction);

        OpenLink = ReactiveCommand.Create(OpenLinkAction);
        OpenImage = ReactiveCommand.Create(OpenImageAction);

        SelectedGridItem = GridItems.LastOrDefault()!;
        NewEvent = new Event();
        NewItem = (TItem)Activator.CreateInstance(typeof(TItem))!;

        IsFullAmount = IsFullAmountDefaultValue;
    }

    protected virtual bool IsFullAmountDefaultValue => true;

    protected virtual float AmountToMinutesModifier => 1f;
    private readonly IDatasource _datasource;
    private readonly IExternal<TItem> _external;
    private TGridItem _selectedGridItem = default!;
    private List<TItem> _itemList = [];
    private List<Event> _eventList = [];
    private TItem _newItem = default!;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;
    private bool _useNewDate;
    private TItem _selectedItem;

    private int _gridCountItemsBookmarked;
    private int _addAmount;
    private string _addAmountString;

    public EventViewModel EventViewModel { get; }
    public GridFilterViewModel GridFilterViewModel { get; }

    public int AddAmount
    {
        get => _addAmount;
        set
        {
            this.RaiseAndSetIfChanged(ref _addAmount, value);
            _addAmount = SetAmount(value);
        }
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

    public PeopleSelectionViewModel People { get; set; }

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

    private async void InputUrlChanged()
    {
        NewItem = await _external.GetItem(InputUrl);

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
        var people = People.GetPeople();

        NewEvent ??= new Event();

        var newEvent = new Event
        {
            ID = 0,
            ItemID = 0,
            ExternalID = string.Empty,
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

        var people = EventViewModel.People.GetPeople();

        var newEvent = new Event
        {
            ID = 0,
            ItemID = 0,
            ExternalID = string.Empty,
            DateEnd = dateEnd,
            Rating = lastEvent?.Rating ?? 0,
            Bookmakred = lastEvent?.Bookmakred ?? false,
            Chapter = DefaultNewItemChapter,
            Amount = amount,
            AmountType = lastEvent?.AmountType ?? 0,
            Completed = lastEvent?.Completed ?? false,
            Comment = lastEvent?.Comment ?? string.Empty,
            People = people,
            Platform = EventViewModel.SelectedPlatformType,
            LocationID = lastEvent?.LocationID
        };

        _datasource.Add(SelectedItem, newEvent);

        ReloadData();
        ClearNewItemControls();
    }

    protected virtual void ReloadData()
    {
        GridItems.Clear();
        GridItems.AddRange(LoadData());
        GridFilterViewModel.GridCountItems = GridItems.Count;

        GridItemsBookmarked.Clear();
        GridItemsBookmarked.AddRange(LoadDataBookmarked());
        // GridCountItemsBookmarked = GridItemsBookmarked.Count;
    }

    private void ClearNewItemControls()
    {
        NewItem = default;
        NewEvent = default;
        NewImage = default;
    }

    protected virtual DateTime? DateTimeFilter
    {
        get
        {
            return new DateTime(GridFilterViewModel.YearFilter, 1, 1);
        }
    }

    internal List<TGridItem> LoadData()
    {
        _itemList = _datasource.GetList<TItem>();
        _eventList = _datasource.GetEventList<TItem>();

        var result = _eventList
                    .OrderByDescending(o => o.DateEnd)
                    .DistinctBy(o => o.ItemID)
                    .OrderBy(o => o.DateEnd)
                    .ToList();

        if (DateTimeFilter.HasValue && string.IsNullOrWhiteSpace(GridFilterViewModel.SearchText))
        {
            result = result
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value >= DateTimeFilter.Value && o.DateEnd.Value < new DateTime(DateTimeFilter.Value.Year + 1, 1, 1))
            .ToList();
        }

        var resultGrid = result.Select(o =>
                        Convert(
                            o,
                            _itemList.First(m => m.ID == o.ItemID),
                            _eventList.Where(e => e.ItemID == o.ItemID)
                        )
                        ).ToList();

        if (!string.IsNullOrWhiteSpace(GridFilterViewModel.SearchText))
        {
            resultGrid = resultGrid
            .Where(o =>
                JsonConvert.SerializeObject(o)
                .Contains(GridFilterViewModel.SearchText, StringComparison.InvariantCultureIgnoreCase))
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
                o =>
                    Convert(
                        o,
                        _itemList.First(m => m.ID == o.ItemID),
                        _eventList.Where(e => e.ItemID == o.ItemID)
                    )
            )
            .ToList();
    }

    protected virtual TGridItem Convert(Event e, TItem i, IEnumerable<Event> events)
    {
        return default!;
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

    protected virtual int DefaultAddAmount => 0;

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

        AddAmount = DefaultAddAmount;
    }

    private ObservableCollection<TGridItem> GetSelectedGrid()
    {
        return GridItems;
    }

    int IDataGrid.ReloadData()
    {
        var selectedGrid = GetSelectedGrid();
        selectedGrid.Clear();
        selectedGrid.AddRange(LoadData());
        return selectedGrid.Count;
    }
}
