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

public partial class BooksViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private BookGridItem _selectedGridItem;
    private List<Book> _itemList;
    private List<Event> _eventList;
    private Book _newItem;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;

    private bool _useNewDate;
    private Book _selectedItem;
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
            Enum.GetValues(typeof(eMusicPlatformType))
                .Cast<eMusicPlatformType>()
                .Select(v => v.ToString())
        );

    public static ObservableCollection<PersonComboBoxItem> PeopleList =>
        new(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem SelectedPerson { get; set; }

    public ObservableCollection<BookGridItem> GridItems { get; set; }
    public ObservableCollection<BookGridItem> GridItemsBookmarked { get; set; }
    public ObservableCollection<InfoModel> Info { get; set; }

    public Book SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }

    public Book NewItem
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
    public BookGridItem SelectedGridItem
    {
        get => _selectedGridItem;
        set
        {
            _selectedGridItem = value;
            SelectedItemChanged();
        }
    }

    public BooksViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        GridItems = new ObservableCollection<BookGridItem>();
        GridItemsBookmarked = new ObservableCollection<BookGridItem>();
        ReloadData();

        Info = new ObservableCollection<InfoModel>();

        Events = new ObservableCollection<Event>();
        EventViewModel = new EventViewModel(Events, MusicPlatformTypes);

        AddItemClick = ReactiveCommand.Create(AddItemClickAction);
        AddEventClick = ReactiveCommand.Create(AddEventClickAction);

        SelectedGridItem = GridItems.LastOrDefault();
    }

    private int SetAmount(int value)
    {
        var events = _eventList.Where(o => o.ItemID == SelectedItem.ID);
        var currentAmount = GetBookPages(events);
        var newAmount = value - currentAmount;

        _newAmount = newAmount;
        AddAmountString = $"    Adding {newAmount} pages";
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
                : lastEvent.DateEnd.Value.AddMinutes(-_newAmount * 2);

        lastEvent.Platform = EventViewModel.SelectedPlatformType;
        lastEvent.Amount = _newAmount;

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

    private List<BookGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Book>();
        _eventList = _datasource.GetEventList<Book>();

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

    private List<BookGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<Book>();
        _eventList = _datasource.GetEventList<Book>();

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

    private static int GetBookPages(IEnumerable<Event> eventList)
    {
        // This is for the case that the book is already completed by you are rereading it.
        var lastCompletedDate =
            eventList.Where(o => o.Completed)?.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;

        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;

        var dateFilter = lastCompletedDate;

        if (lastCompletedDate == lastDate)
        {
            return eventList.Sum(o => o.Amount);
        }

        return eventList.Where(o => o.DateEnd > dateFilter).Sum(o => o.Amount);
    }

    private static BookGridItem Convert(int index, Event e, Book i, IEnumerable<Event> eventList)
    {
        return new BookGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Author,
            i.Year,
            e.Rating,
            GetBookPages(eventList),
            e.DateEnd
        );
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
        Info.AddRange(GetSelectedItemInfo<Book>());
        Events.AddRange(_eventList.Where(o => o.ItemID == SelectedItem.ID).OrderBy(o => o.DateEnd));

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepsitory.GetImage<Book>(item.ID);
    }
}
