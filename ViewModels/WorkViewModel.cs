using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.GridItems;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class WorkViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private WorkGridItem _selectedItem;
    private List<Work> _itemList;
    private List<Event> _eventList;
    private string _inputUrl;
    private Work _newWork;
    private Bitmap? _cover;
    private Bitmap? _newMusicCover;
    private Event _newEvent;

    private bool _useNewDate;
    private Work _selectedWork;
    private int _gridCountMusic;

    private int _gridCountMusicBookmarked;

    public EventViewModel EventViewModel { get; }

    public int AddMinutes { get; set; }

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

    public ObservableCollection<WorkGridItem> Work { get; set; }
    public ObservableCollection<WorkGridItem> WorkBookmarked { get; set; }

    public Work SelectedWork
    {
        get => _selectedWork;
        set => this.RaiseAndSetIfChanged(ref _selectedWork, value);
    }
    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddClick { get; }
    public ReactiveCommand<Unit, Unit> ListenAgain { get; }

    public Work NewWork
    {
        get => _newWork;
        set => this.RaiseAndSetIfChanged(ref _newWork, value);
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
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    public Bitmap? NewImage
    {
        get => _newMusicCover;
        private set => this.RaiseAndSetIfChanged(ref _newMusicCover, value);
    }

    public int GridCountWork
    {
        get => _gridCountMusic;
        private set => this.RaiseAndSetIfChanged(ref _gridCountMusic, value);
    }

    public int GridCountWorkBookmarked
    {
        get => _gridCountMusicBookmarked;
        private set => this.RaiseAndSetIfChanged(ref _gridCountMusicBookmarked, value);
    }

    public WorkViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        Work = [];
        WorkBookmarked = [];
        ReloadData();

        Events = [];
        EventViewModel = new EventViewModel(Events, MusicPlatformTypes);

        AddClick = ReactiveCommand.Create(AddClickAction);
        ListenAgain = ReactiveCommand.Create(ListenAgainAction);

        SelectedItem = Work.LastOrDefault();
    }

    public WorkGridItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            SelectedItemChanged();
        }
    }

    private void OpenImageAction() { }

    private void AddClickAction()
    {
        NewEvent.DateEnd = UseNewDate ? NewDate + NewTime : DateTime.Now;
        NewEvent.DateStart =
            NewEvent.DateEnd.Value.TimeOfDay.Ticks == 0
                ? NewEvent.DateEnd.Value
                : NewEvent.DateEnd.Value.AddMinutes(-NewEvent.Amount);
        NewEvent.People = SelectedPerson?.ID.ToString() ?? null;

        _datasource.Add(NewWork, NewEvent);

        ReloadData();
        ClearNewItemControls();
    }

    private void ListenAgainAction()
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
                : lastEvent.DateEnd.Value.AddMinutes(-AddMinutes);

        lastEvent.Platform = EventViewModel.SelectedPlatformType;
        lastEvent.Amount = AddMinutes;

        _datasource.Add(SelectedWork, lastEvent);

        ReloadData();
        ClearNewItemControls();
    }

    private void ReloadData()
    {
        Work.Clear();
        Work.AddRange(LoadData());
        GridCountWork = Work.Count;

        WorkBookmarked.Clear();
        WorkBookmarked.AddRange(LoadDataBookmarked());
        GridCountWorkBookmarked = WorkBookmarked.Count;
    }

    private void ClearNewItemControls()
    {
        NewWork = default;
        NewEvent = default;
        NewImage = default;
        SelectedPerson = default;
    }
    private List<WorkGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Work>();
        _eventList = _datasource.GetEventList<Work>();

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

    private List<WorkGridItem> LoadDataBookmarked(int? yearsAgo = null)
    {
        _itemList = _datasource.GetList<Work>();
        _eventList = _datasource.GetEventList<Work>();

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

    private static WorkGridItem Convert(int index, Event e, Work i, IEnumerable<Event> eventList)
    {
        return new WorkGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Type,
            eventList.Sum(o => o.Amount),
            e.DateEnd.Value
        );
    }

    public void SelectedItemChanged()
    {
        Events.Clear();
        Image = null;

        if (SelectedItem == null)
        {
            return;
        }

        SelectedWork = _itemList.First(o => o.ID == SelectedItem.ID);
        Events.AddRange(_eventList.Where(o => o.ItemID == SelectedItem.ID).OrderBy(o => o.DateEnd));

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Image = FileRepsitory.GetImage<Work>(item.ID);
    }
}
