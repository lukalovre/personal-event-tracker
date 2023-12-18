using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class SongsViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private readonly IExternal<Song> _external;
    private SongGridItem _selectedGridItem;
    private List<Song> _itemList;
    private List<Event> _eventList;
    private Song _newItem;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;
    private bool _useNewDate;
    private Song _selectedItem;
    private int _gridCountItems;
    private int _gridCountItemsBookmarked;
    private int _addAmount;
    private string _addAmountString;
    private string _inputUrl;

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

    public static ObservableCollection<string> MusicPlatformTypes => new();

    public static ObservableCollection<PersonComboBoxItem> PeopleList =>
        new(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem SelectedPerson { get; set; }

    public ObservableCollection<SongGridItem> GridItems { get; set; }
    public ObservableCollection<SongGridItem> GridItemsBookmarked { get; set; }
    public ObservableCollection<InfoModel> Info { get; set; }

    public Song SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }

    public Song NewItem
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
    public SongGridItem SelectedGridItem
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

    public SongsViewModel(IDatasource datasource, IExternal<Song> external)
    {
        _datasource = datasource;
        _external = external;

        GridItems = new ObservableCollection<SongGridItem>();
        GridItemsBookmarked = new ObservableCollection<SongGridItem>();
        ReloadData();

        Info = new ObservableCollection<InfoModel>();

        Events = new ObservableCollection<Event>();
        EventViewModel = new EventViewModel(Events, MusicPlatformTypes);

        AddItemClick = ReactiveCommand.Create(AddItemClickAction);
        AddEventClick = ReactiveCommand.Create(AddEventClickAction);

        SelectedGridItem = GridItems.LastOrDefault();
    }

    private void InputUrlChanged()
    {
        NewItem = _external.GetItem(InputUrl);
        NewImage = FileRepsitory.GetImageTemp<Song>();
        NewEvent = new Event { Amount = NewItem.Runtime, Rating = 1 };

        _inputUrl = string.Empty;
    }

    private int SetAmount(int value)
    {
        _addAmount = value;
        AddAmountString = $"    Adding {_addAmount} minutes";
        return value;
    }

    private void AddItemClickAction()
    {
        NewEvent.Amount = NewItem.Runtime;
        NewEvent.DateEnd = UseNewDate ? NewDate + NewTime : DateTime.Now;
        NewEvent.DateStart =
            NewEvent.DateEnd.Value.TimeOfDay.Ticks == 0
                ? NewEvent.DateEnd.Value
                : NewEvent.DateEnd.Value.AddMinutes(-NewEvent.Amount);
        NewEvent.People = SelectedPerson?.ID.ToString() ?? null;
        NewEvent.Chapter = 1;

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

    private List<InfoModel> GetSelectedItemInfo<T>()
    {
        var result = new List<InfoModel>();

        if (SelectedItem == null)
        {
            return result;
        }

        var properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var e = _eventList?.First(o => o.ItemID == SelectedItem.ID);
            var i = _itemList.First(o => o.ID == e.ItemID);

            var value = property.GetValue(i);
            result.Add(new InfoModel(property.Name, value));
        }

        return result;
    }

    private List<SongGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Song>();
        _eventList = _datasource.GetEventList<Song>();

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

    private List<SongGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<Song>();
        _eventList = _datasource.GetEventList<Song>();

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

    private static SongGridItem Convert(
        int index,
        Event e,
        Song i,
        IEnumerable<Event> eventList
    )
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new SongGridItem(
            i.ID,
            index + 1,
            i.Artist,
            i.Title,
            i.Year,
            eventList.Count(),
            e.Bookmakred);
    }

    public void SelectedItemChanged()
    {
        Info.Clear();
        Events.Clear();
        Image = null;

        if (SelectedGridItem == null)
        {
            return;
        }

        SelectedItem = _itemList.First(o => o.ID == SelectedGridItem.ID);
        Info.AddRange(GetSelectedItemInfo<Song>());
        Events.AddRange(
            _eventList
                .Where(o => o.ItemID == SelectedItem.ID && o.DateEnd.HasValue)
                .OrderBy(o => o.DateEnd)
        );

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepsitory.GetImage<Song>(item.ID);
    }
}
