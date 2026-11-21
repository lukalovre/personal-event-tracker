using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using EventTracker.Models;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;
using DynamicData;
using Newtonsoft.Json;
using ReactiveUI;
using Repositories;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using EventTracker.Views;

namespace EventTracker.ViewModels;

public class ItemViewModel<TItem, TGridItem> : ViewModelBase, IDataGrid, IImagePickerViewModel, ILoadGridOnSelection
where TItem : IItem
where TGridItem : IGridItem
{
    public ItemViewModel(IDatasource datasource, IExternal<TItem> external)
    {
        _settings = Settings.Instance.GetItemSettigns<TItem>();

        _datasource = datasource;
        _external = external;

        GridFilterViewModel = new GridFilterViewModel(this)
        {
            ShowYearFilter = _settings.ShowYearFilter
        };
        People = new PeopleSelectionViewModel();

        GridItems = [];
        GridItemsBookmarked = [];

        Events = [];
        EventViewModel = new EventViewModel(Events, _settings.PlatformTypes, _settings);

        AddItemClick = ReactiveCommand.CreateFromTask(AddItemClickAction);
        AddEventClick = ReactiveCommand.Create(AddEventClickAction);
        UpdateItemClick = ReactiveCommand.Create(UpdateItemClickAction);

        OpenLink = ReactiveCommand.Create(OpenLinkAction);
        Search = ReactiveCommand.Create(SearchAction);
        OpenImage = ReactiveCommand.CreateFromTask(() => PickImageAsync(GetMainWindow()));
        ReplaceImage = ReactiveCommand.CreateFromTask(() => ReplaceSelectedImageAsync(GetMainWindow()));
        CropImage = ReactiveCommand.CreateFromTask(() => CropSelectedImageAsync(GetMainWindow()));
        ToggleDateTimePicker = ReactiveCommand.Create(ToggleDateTimePickerAction);
        ConfirmDateTime = ReactiveCommand.Create(ConfirmDateTimeAction);

        NewEvent = new Event();
        NewItem = (TItem)Activator.CreateInstance(typeof(TItem))!;
        SelectedItem = (TItem)Activator.CreateInstance(typeof(TItem))!;

        IsFullAmount = _settings.IsFullAmountDefaultValue;
    }

    protected DateTime? DateTimeFilter
    {
        get
        {
            return _settings.DateTimeFilter ?? new DateTime(GridFilterViewModel.YearFilter, 1, 1);
        }
    }

    protected virtual string DefaultGridTabHeader => "★";

    public ObservableCollection<string> PlatformTypes => _settings.PlatformTypes;
    public ReactiveCommand<Unit, Unit> ToggleDateTimePicker { get; }
    public ReactiveCommand<Unit, Unit> ConfirmDateTime { get; }
    public bool ShowChapterInNewEvent => _settings.ShowChapterInNewEvent;
    public bool ShowRatingInNewEvent => _settings.ShowRatingInNewEvent;
    public bool ShowDateInNewEvent => _settings.ShowDateInNewEvent;
    public bool ShowAmountInNewEvent => _settings.ShowAmountInNewEvent;
    public bool ShowBookmarkedInNewEvent => _settings.ShowBookmarkedInNewEvent;
    public bool ShowCompletedInNewEvent => _settings.ShowCompletedInNewEvent;
    public bool ShowPlatformInNewEvent => _settings.ShowPlatformInNewEvent;
    public bool ShowCommentInNewEvent => _settings.ShowCommentInNewEvent;
    public bool ShowPeopleInNewEvent => _settings.ShowPeopleInNewEvent;
    private readonly ItemSettings _settings = new();
    protected readonly IDatasource _datasource;
    private readonly IExternal<TItem> _external;
    private TGridItem _selectedGridItem = default!;
    private List<TItem> _itemList = [];
    protected List<Event> _eventList = [];
    private bool _hasLoadedItemsAndEvents;
    private bool _hasLoadedGridData;
    private List<(TGridItem Item, DateTime? DateEnd, string SearchableText)> _gridData = [];
    private TItem _newItem = default!;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private readonly ObservableCollection<ItemEditField> _editFields = [];
    private readonly ObservableCollection<ItemEditField> _newItemFields = [];
    private Event _newEvent = new();
    private bool _useNewDate;
    private bool _isDateTimePickerOpen;
    private DateTimeOffset? _pendingEventDate;
    private TimeSpan? _pendingEventTime;
    private DateTime _newDate = DateTime.Now;
    private readonly HashSet<string> _loadedGridTabs = new(StringComparer.OrdinalIgnoreCase);
    private TItem _selectedItem = default!;

    private int _gridCountItemsBookmarked;
    private int _addAmount;
    private string _addAmountString = string.Empty;
    private int _selectedRightTabIndex;

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
    private string _inputUrl = string.Empty;
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

    public bool IsDateTimePickerOpen
    {
        get => _isDateTimePickerOpen;
        set
        {
            this.RaiseAndSetIfChanged(ref _isDateTimePickerOpen, value);
            if (value)
            {
                PendingEventDate = new DateTimeOffset(NewDate);
                PendingEventTime = NewDate.TimeOfDay;
            }
        }
    }

    public DateTimeOffset? PendingEventDate
    {
        get => _pendingEventDate;
        set => this.RaiseAndSetIfChanged(ref _pendingEventDate, value);
    }

    public TimeSpan? PendingEventTime
    {
        get => _pendingEventTime;
        set => this.RaiseAndSetIfChanged(ref _pendingEventTime, value);
    }

    public bool IsFullAmount
    {
        get => _isFullAmount;
        set => this.RaiseAndSetIfChanged(ref _isFullAmount, value);
    }

    public PeopleSelectionViewModel People { get; set; }

    public ObservableCollection<TGridItem> GridItems { get; set; }
    public ObservableCollection<TGridItem> GridItemsBookmarked { get; set; }

    public int SelectedRightTabIndex
    {
        get => _selectedRightTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedRightTabIndex, value);
    }

    public TItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            var normalizedValue = value ?? (TItem)Activator.CreateInstance(typeof(TItem))!;
            this.RaiseAndSetIfChanged(ref _selectedItem, normalizedValue);
            RefreshEditFields();
            this.RaisePropertyChanged(nameof(OpenLinkToolTip));
            OnSelectedItemChanged();
        }
    }

    protected virtual void OnSelectedItemChanged()
    {
    }

    public ObservableCollection<ItemEditField> EditFields => _editFields;

    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> UpdateItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }
    public ReactiveCommand<Unit, Unit> OpenLink { get; }
    public ReactiveCommand<Unit, Unit> Search { get; }
    public ReactiveCommand<Unit, Unit> OpenImage { get; }
    public ReactiveCommand<Unit, Unit> ReplaceImage { get; }
    public ReactiveCommand<Unit, Unit> CropImage { get; }
    public string OpenLinkToolTip => GetLinkToOpen();

    public TItem NewItem
    {
        get => _newItem;
        set
        {
            this.RaiseAndSetIfChanged(ref _newItem, value);
            RefreshNewItemFields();
        }
    }

    public ObservableCollection<ItemEditField> NewItemFields => _newItemFields;

    public DateTime NewDate
    {
        get => _newDate;
        set => this.RaiseAndSetIfChanged(ref _newDate, value);
    }

    public Event NewEvent
    {
        get => _newEvent;
        set
        {
            this.RaiseAndSetIfChanged(ref _newEvent, value);
            this.RaisePropertyChanged(nameof(NewEventPlatform));
            this.RaisePropertyChanged(nameof(IsCinemaPlatform));
        }
    }

    public string NewEventPlatform
    {
        get => NewEvent.Platform;
        set
        {
            var platform = value ?? string.Empty;
            if (NewEvent.Platform == platform)
            {
                return;
            }

            NewEvent.Platform = platform;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(IsCinemaPlatform));
        }
    }

    public bool IsCinemaPlatform
    {
        get => string.Equals(NewEvent.Platform, "Cinema", StringComparison.OrdinalIgnoreCase);
        set
        {
            if (value)
            {
                NewEventPlatform = "Cinema";
            }
            else if (IsCinemaPlatform)
            {
                NewEventPlatform = string.Empty;
            }
        }
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

    private int SetAmount(int value)
    {
        if (SelectedItem is null)
        {
            return 0;
        }

        var events = _eventList.Where(o => o.ItemID == SelectedItem.ID);
        var currentAmount = IsFullAmount ? 0 : GetItemAmount(events);
        var newAmount = value - currentAmount;

        _newAmount = newAmount;
        AddAmountString = $"    Adding {newAmount} {_settings.AmountVerb}";
        return value;
    }

    private async void InputUrlChanged()
    {
        NewItem = await _external.GetItem(InputUrl);

        NewImage = FileRepository.GetImageTemp<TItem>();
        NewEvent = new Event
        {
            Rating = _settings.DefaultNewItemRating,
            Platform = _settings.DefaultNewItemPlatform,
            Completed = _settings.DefaultNewItemCompleted,
            Bookmarked = _settings.DefaultNewItemBookmarked
        };

        FilterByNewItem();
    }

    private void FilterByNewItem()
    {
        var filterByOnNewItem = GetFilterByOnNewItem();

        if (string.IsNullOrWhiteSpace(filterByOnNewItem))
        {
            return;
        }

        GridFilterViewModel.SearchText = filterByOnNewItem;
        ReloadData();
    }

    protected virtual string GetFilterByOnNewItem()
    {
        return string.Empty;
    }

    public async Task PickImageAsync(Window window, bool isNewImage = false)
    {
        if (isNewImage)
        {
            if (NewImage is not null)
            {
                return;
            }

            NewImage = await ImagePicker.PickAndSaveAsync(
                window,
                $"{Paths.GetTempPath<TItem>()}.png",
                ImageNormalizer.AppliesTo<TItem>());
            return;
        }

        if (Image is not null || SelectedItem is null)
        {
            return;
        }

        Image = await ImagePicker.PickAndSaveAsync(
            window,
            Paths.GetImagePath<TItem>(SelectedItem.ID),
            ImageNormalizer.AppliesTo<TItem>());
    }

    private static Window GetMainWindow()
    {
        return ((ClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
    }

    protected virtual List<string> GetAlternativeOpenLinkSearchParams()
    {
        if (SelectedItem is null || string.IsNullOrWhiteSpace(SelectedItem.Title))
        {
            return [];
        }

        return SelectedItem.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    protected virtual string GetOpenLink()
    {
        return (SelectedItem as IExternal)?.ExternalID ?? string.Empty;
    }

    private string GetLinkToOpen()
    {
        if (SelectedItem is null)
        {
            return string.Empty;
        }

        var link = GetOpenLink();
        return HtmlHelper.GetLinkUrl(link, [.. GetAlternativeOpenLinkSearchParams()]);
    }

    private void OpenLinkAction()
    {
        HtmlHelper.OpenLink(GetLinkToOpen());
    }

    private void SearchAction()
    {
        if (SelectedItem is null)
        {
            return;
        }

        HtmlHelper.OpenLink(string.Empty, [.. GetAlternativeOpenLinkSearchParams()]);
    }

    private async Task AddItemClickAction()
    {
        if (NewItemFields.Any(field => !field.TryApply()))
        {
            return;
        }

        var overrideAmount = _settings.NewItemAmountOverride;

        if (overrideAmount == -1)
        {
            overrideAmount = (NewItem as IRuntime)?.Runtime ?? 0;
        }

        var amount = overrideAmount ?? NewItemAmount;
        var dateEnd = UseNewDate ? NewDate : DateTime.Now;
        var people = People.GetPeople();

        var amountType = _settings.AmountType ?? NewEvent.AmountType;

        NewEvent ??= new Event();
        var chapter = NewEvent.Chapter ?? _settings.DefaultNewItemChapter;

        var newEvent = new Event
        {
            ID = 0,
            ItemID = 0,
            ExternalID = string.Empty,
            DateEnd = dateEnd,
            Rating = NewEvent.Rating,
            Bookmarked = NewEvent.Bookmarked,
            Chapter = chapter,
            Amount = amount,
            AmountType = amountType,
            Completed = NewEvent.Completed,
            Comment = NewEvent.Comment,
            People = people,
            Platform = NewEvent.Platform,
            LocationID = NewEvent.LocationID
        };

        _datasource.Add(NewItem, newEvent);
        var newItemId = NewItem.ID;

        await ReloadDataAsync();

        var newGridItem = GridItems.FirstOrDefault(item => item.ID == newItemId);
        if (newGridItem is not null)
        {
            SelectedGridItem = newGridItem;
        }

        SelectedRightTabIndex = 0;
        ClearNewItemControls();
    }

    private void ToggleDateTimePickerAction()
    {
        IsDateTimePickerOpen = !IsDateTimePickerOpen;
    }

    private void ConfirmDateTimeAction()
    {
        if (PendingEventDate is null || PendingEventTime is null)
        {
            return;
        }

        NewDate = PendingEventDate.Value.Date.Add(PendingEventTime.Value);
        UseNewDate = true;
        IsDateTimePickerOpen = false;
    }

    private void UpdateItemClickAction()
    {
        if (SelectedItem is null)
        {
            return;
        }

        if (EditFields.Any(field => !field.TryApply()))
        {
            return;
        }

        _datasource.Update(SelectedItem);
        ReloadData();
        ClearNewItemControls();
    }

    private void RefreshEditFields()
    {
        EditFields.Clear();

        if (SelectedItem is null)
        {
            return;
        }

        foreach (var field in CreateItemFields(SelectedItem))
        {
            EditFields.Add(field);
        }
    }

    private void RefreshNewItemFields()
    {
        NewItemFields.Clear();

        if (NewItem is null)
        {
            return;
        }

        foreach (var field in CreateItemFields(NewItem))
        {
            NewItemFields.Add(field);
        }
    }

    private static IEnumerable<ItemEditField> CreateItemFields(TItem item)
    {
        return typeof(TItem).GetProperties()
            .Where(property => property.Name != nameof(IItem.ID)
                && property.CanRead
                && property.CanWrite
                && property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.MetadataToken)
            .Select(property => ItemEditField.Create(item, property));
    }

    private async Task ReplaceSelectedImageAsync(Window window)
    {
        if (SelectedItem is null)
        {
            return;
        }

        var image = await ImagePicker.PickAndSaveAsync(
            window,
            Paths.GetImagePath<TItem>(SelectedItem.ID),
            ImageNormalizer.AppliesTo<TItem>());
        if (image is not null)
        {
            Image = image;
        }
    }

    private async Task CropSelectedImageAsync(Window window)
    {
        if (SelectedItem is null)
        {
            return;
        }

        var imagePath = Paths.GetImagePath<TItem>(SelectedItem.ID);
        if (!File.Exists(imagePath))
        {
            return;
        }

        var cropWindow = new SquareCropWindow(imagePath);
        var result = await cropWindow.ShowDialog<bool?>(window);
        if (result == true)
        {
            if (ImageNormalizer.AppliesTo<TItem>())
            {
                ImageNormalizer.Normalize(imagePath);
            }
            Image = FileRepository.GetImage<TItem>(SelectedItem.ID);
        }
    }

    private void AddEventClickAction()
    {
        var lastEvent = Events.MaxBy(o => o.DateEnd) ?? Events.LastOrDefault();

        var amount = _newAmount == 0
        ? EventViewModel.NewItemAmount
        : _newAmount;

        var dateEnd = !EventViewModel.IsEditDate
        ? DateTime.Now
        : EventViewModel?.SelectedEvent?.DateEnd ?? DateTime.Now;

        var people = EventViewModel?.People.GetPeople() ?? string.Empty;

        int? chapter = null;

        if (_settings.DefaultNewItemChapter is not null)
        {
            chapter = EventViewModel?.NewEventChapter ?? _settings.DefaultNewItemChapter;
        }

        var eventForm = EventViewModel?.NewEvent
            ?? throw new InvalidOperationException("The event form is unavailable.");
        var newEvent = new Event
        {
            ID = 0,
            ItemID = 0,
            ExternalID = string.Empty,
            DateEnd = dateEnd,
            Rating = eventForm.Rating,
            Bookmarked = eventForm.Bookmarked,
            Chapter = eventForm.Chapter ?? chapter,
            Amount = amount,
            AmountType = lastEvent?.AmountType ?? 0,
            Completed = eventForm.Completed,
            Comment = eventForm.Comment,
            People = people,
            Platform = eventForm.Platform,
            LocationID = eventForm.LocationID ?? lastEvent?.LocationID
        };

        _datasource.Add(SelectedItem, newEvent);

        ReloadData();
        ClearNewItemControls();
    }

    protected async void ReloadData()
    {
        await ReloadDataAsync();
    }

    protected virtual async Task ReloadDataAsync()
    {
        LoadItemsAndEvents(out _, out _, realLoad: true);

        GridItems.Clear();
        GridItems.AddRange(await LoadData());
        GridFilterViewModel.GridCountItems = GridItems.Count;

        GridItemsBookmarked.Clear();
        GridItemsBookmarked.AddRange(LoadDataBookmarked());
    }

    private void ClearNewItemControls()
    {
        this.RaiseAndSetIfChanged(ref _inputUrl, string.Empty, nameof(InputUrl));
        NewImage = default;
        NewEvent = new Event();
        NewItem = (TItem)Activator.CreateInstance(typeof(TItem))!;
    }

    internal Task<List<TGridItem>> LoadData(string searchText = null!, bool skipFilters = false)
    {
        LoadItemsAndEvents(out _, out _);

        searchText ??= GridFilterViewModel.SearchText;
        searchText = searchText.Trim();
        if (searchText.Length < 3)
        {
            searchText = string.Empty;
        }
        var result = GetGridData();

        if (!skipFilters
            && GridFilterViewModel.IsYearFilterEnabled
            && DateTimeFilter.HasValue
            && DateTimeFilter != DateTime.MinValue)
        {
            if (_settings.IsRelativeDateTimeFilter)
            {
                var startOfToday = DateTime.Today;
                result = result
                    .Where(o => o.DateEnd.HasValue && o.DateEnd.Value >= startOfToday)
                    .ToList();
            }
            else
            {
                result = result
                    .Where(o => o.DateEnd.HasValue
                        && o.DateEnd.Value >= DateTimeFilter.Value
                        && o.DateEnd.Value < new DateTime(DateTimeFilter.Value.Year + 1, 1, 1))
                    .ToList();
            }
        }

        if (!skipFilters && !string.IsNullOrWhiteSpace(searchText))
        {
            result = result
            .Where(o => o.SearchableText.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        }

        return Task.FromResult(result.Select(o => o.Item).ToList());
    }

    private List<(TGridItem Item, DateTime? DateEnd, string SearchableText)> GetGridData()
    {
        if (_hasLoadedGridData)
        {
            return _gridData;
        }

        _gridData = _eventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .Select(e =>
            {
                var gridItem = Convert(
                    e,
                    _itemList.First(item => item.ID == e.ItemID),
                    _eventList.Where(itemEvent => itemEvent.ItemID == e.ItemID));

                return (gridItem, e.DateEnd, JsonConvert.SerializeObject(gridItem));
            })
            .ToList();
        _hasLoadedGridData = true;
        return _gridData;
    }

    protected void LoadItemsAndEvents(out List<TItem> itemList, out List<Event> eventList, bool realLoad = false)
    {
        if (realLoad || !_hasLoadedItemsAndEvents)
        {
            var type = Helpers.GetClassName<TItem>();
            _itemList = _datasource.GetList<TItem>(type);
            _eventList = _datasource.GetEventList(type);
            _hasLoadedItemsAndEvents = true;
            _hasLoadedGridData = false;
            _gridData.Clear();
        }

        itemList = _itemList;
        eventList = _eventList;
    }

    protected List<TGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        LoadItemsAndEvents(out _, out _);

        var dateFilter = yearsAgo.HasValue
            ? DateTime.Now.AddYears(-yearsAgo.Value)
            : DateTime.MaxValue;

        return _eventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value <= dateFilter)
            .Where(o => o.Bookmarked)
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

    public void SelectedItemChanged()
    {
        EventViewModel.SelectedEvent = null!;
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

        EventViewModel.SelectedEvent = Events.LastOrDefault() ?? new Event();

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepository.GetImage<TItem>(item.ID);

        if (_settings.DefaultAddAmount == -1)
        {
            AddAmount = Events.LastOrDefault()?.Amount ?? 0;
        }
        else
        {
            AddAmount = _settings.DefaultAddAmount;
        }

        EventViewModel.FillStats<TItem>(Events);
    }

    public virtual void SelectFirstItem()
    {
        if (GridItems.Count > 0)
        {
            SelectedGridItem = GridItems[0];
        }
    }

    public void LoadOnSelection()
    {
        LoadGridOnSelection(null);
    }

    public async void LoadGridOnSelection(string? tabHeader)
    {
        var tabKey = tabHeader ?? DefaultGridTabHeader;
        if (!_loadedGridTabs.Add(tabKey))
        {
            return;
        }

        People.LoadData();
        EventViewModel.People.LoadData();
        await LoadGridTabData(tabKey);
        GridFilterViewModel.GridCountItems = GetLoadedGridCount(tabKey);
    }

    public void LoadDefaultGridOnSelection()
    {
        LoadGridOnSelection(DefaultGridTabHeader);
    }

    protected virtual async Task LoadGridTabData(string? tabHeader)
    {
        if (string.Equals(tabHeader, "★", StringComparison.Ordinal))
        {
            LoadItemsAndEvents(out _, out _);
            GridItemsBookmarked.AddRange(LoadDataBookmarked());

            if (GridItemsBookmarked.Count > 0)
            {
                SelectedGridItem = GridItemsBookmarked[0];
            }

            return;
        }

        var gridItems = await LoadData();
        GridItems.AddRange(gridItems);
        GridFilterViewModel.GridCountItems = GridItems.Count;

        if (GridItems.Count > 0)
        {
            SelectedGridItem = GridItems[0];
        }
    }

    protected virtual int GetLoadedGridCount(string? tabHeader)
    {
        return string.Equals(tabHeader, "★", StringComparison.Ordinal)
            ? GridItemsBookmarked.Count
            : GridItems.Count;
    }

    private ObservableCollection<TGridItem> GetSelectedGrid()
    {
        return GridItems;
    }

    int IDataGrid.ReloadData()
    {
        var selectedGrid = GetSelectedGrid();
        selectedGrid.Clear();
        selectedGrid.AddRange(LoadData().Result);
        return selectedGrid.Count;
    }
}
