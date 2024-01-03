using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using SpotifyAPI.Web;

namespace AvaloniaApplication1.ViewModels;

public partial class MoviesViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private readonly IExternal<Movie> _external;
    private MovieGridItem _selectedGridItem;
    private List<Movie> _itemList;
    private List<Event> _eventList;
    private Movie _newItem;
    private Bitmap? _itemImage;
    private Bitmap? _newItemImage;
    private Event _newEvent;
    private bool _useNewDate;
    private Movie _selectedItem;
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

    public static ObservableCollection<string> MusicPlatformTypes => [];

    public static ObservableCollection<PersonComboBoxItem> PeopleList =>
        new(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem? SelectedPerson { get; set; }

    public ObservableCollection<MovieGridItem> GridItems { get; set; }
    public ObservableCollection<MovieGridItem> GridItemsBookmarked { get; set; }

    public Movie SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public string SearchText { get; set; }

    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddItemClick { get; }
    public ReactiveCommand<Unit, Unit> AddEventClick { get; }
    public ReactiveCommand<Unit, Unit> Search { get; }

    public Movie NewItem
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
    public MovieGridItem SelectedGridItem
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

    public MoviesViewModel(IDatasource datasource, IExternal<Movie> external)
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
        Search = ReactiveCommand.Create(SearchAction);

        SelectedGridItem = GridItems.LastOrDefault();
    }

    private void InputUrlChanged()
    {
        NewItem = _external.GetItem(InputUrl);
        NewImage = FileRepsitory.GetImageTemp<Movie>();
        NewEvent = new Event { Amount = NewItem.Runtime, Rating = 1 };

        _inputUrl = string.Empty;
    }

    private void SearchAction()
    {
        SearchText = SearchText.Trim();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            GridItemsBookmarked.Clear();
            GridItemsBookmarked.AddRange(LoadData());
            return;
        }

        var searchMovie = new Movie { Director = SearchText, Title = SearchText };

        GridItemsBookmarked.Clear();
        GridItemsBookmarked.AddRange(LoadArtistData(searchMovie));
    }

    private IEnumerable<MovieGridItem> LoadArtistData(Movie searchMovie)
    {
        _itemList = _datasource.GetList<Movie>();
        _eventList = _datasource.GetEventList<Movie>();

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
             .Where(
                o =>
                    o.Director.Contains(searchMovie.Director, StringComparison.InvariantCultureIgnoreCase)
                    || o.Title.Contains(searchMovie.Title, StringComparison.InvariantCultureIgnoreCase)
            )
            .ToList();
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
        NewEvent.Completed = true;

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

        lastEvent.DateStart =
            lastEvent.DateEnd.Value.TimeOfDay.Ticks == 0
                ? lastEvent.DateEnd.Value
                : lastEvent.DateEnd.Value.AddMinutes(-SelectedItem.Runtime);

        lastEvent.Platform = EventViewModel.SelectedPlatformType;
        lastEvent.Amount = SelectedItem.Runtime;
        lastEvent.Completed = true;

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
        GridItemsBookmarked.AddRange(LoadDataBookmarked(0));
        GridCountItemsBookmarked = GridItemsBookmarked.Count;
    }

    private void ClearNewItemControls()
    {
        NewItem = new Movie();
        NewEvent = new Event();
        NewImage = default;
        SelectedPerson = default;
    }

    private List<MovieGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Movie>();
        _eventList = _datasource.GetEventList<Movie>();

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

    private List<MovieGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<Movie>();
        _eventList = _datasource.GetEventList<Movie>();

        var dateFilter = yearsAgo.HasValue
            ? DateTime.Now.AddYears(-yearsAgo.Value)
            : DateTime.MaxValue;

        return _eventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == dateFilter.Year)
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

    private static MovieGridItem Convert(
        int index,
        Event e,
        Movie i,
        IEnumerable<Event> eventList
    )
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new MovieGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Director,
            i.Year,
            e.Rating.Value
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
        var events = _eventList
                .Where(o => o.ItemID == SelectedItem.ID && o.DateEnd.HasValue)
                .OrderBy(o => o.DateEnd).ToList();

        events = events.Count != 0
        ? events
        : _eventList.Where(o => o.ItemID == SelectedItem.ID).ToList();

        Events.AddRange(events);

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepsitory.GetImage<Movie>(item.ID);
    }
}
