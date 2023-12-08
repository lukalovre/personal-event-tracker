using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Media.Imaging;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MusicViewModel : ViewModelBase
{
    public EventViewModel EventViewModel { get; }
    private readonly IDatasource _datasource;
    private MusicGridItem selectedItem;
    private readonly List<Music> _itemList;
    private readonly List<Event> _eventList;
    private string inputUrl;
    private Music newMusic;

    public bool UseNewDate
    {
        get => useNewDate;
        set => this.RaiseAndSetIfChanged(ref useNewDate, value);
    }

    public ObservableCollection<MusicGridItem> Music { get; set; }
    public ObservableCollection<InfoModel> Info { get; set; }
    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddClick { get; }

    public Music NewMusic
    {
        get => newMusic;
        set => this.RaiseAndSetIfChanged(ref newMusic, value);
    }

    public DateTime NewDate { get; set; } =
        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

    public TimeSpan NewTime { get; set; } = new TimeSpan();
    public Event NewEvent
    {
        get => newEvent;
        set => this.RaiseAndSetIfChanged(ref newEvent, value);
    }

    public string InputUrl
    {
        get => inputUrl;
        set
        {
            inputUrl = value;
            InputUrlChanged();
        }
    }

    private Bitmap? _cover;

    public Bitmap? Cover
    {
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    private Bitmap? _newMusicCover;
    private Event newEvent;

    private bool useNewDate;

    public Bitmap? NewMusicCover
    {
        get => _newMusicCover;
        private set => this.RaiseAndSetIfChanged(ref _newMusicCover, value);
    }

    private void InputUrlChanged()
    {
        NewMusic = MusicRepository.GetAlbumInfoBandcamp(InputUrl);
        NewMusicCover = FileRepsitory.GetImageTemp<Music>();
        NewEvent = new Event { Amount = NewMusic.Runtime, Rating = 1 };
    }

    public MusicGridItem SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            SelectedItemChanged();
        }
    }

    public MusicViewModel(IDatasource datasource)
    {
        _datasource = datasource;
        _itemList = _datasource.GetList<Music>();
        _eventList = _datasource.GetEventList<Music>();

        Music = new ObservableCollection<MusicGridItem>(GetData());
        Info = new ObservableCollection<InfoModel>();

        Events = new ObservableCollection<Event>();
        EventViewModel = new EventViewModel(Events);

        AddClick = ReactiveCommand.Create(AddClickAction);
    }

    private void AddClickAction()
    {
        NewEvent.DateEnd = UseNewDate ? NewDate + NewTime : DateTime.Now;
        NewEvent.DateStart = NewEvent.DateEnd.Value.AddMinutes(-NewEvent.Amount);

        _datasource.Add(NewMusic, NewEvent);

        FileRepsitory.MoveTempImage<Music>(NewMusic.ID);
    }

    private List<InfoModel> GetSelectedItemInfo<T>()
    {
        var result = new List<InfoModel>();

        var properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var e = _eventList.First(o => o.ItemID == selectedItem.ID);
            var i = _itemList.First(o => o.ID == e.ItemID);

            var value = property.GetValue(i);
            result.Add(new InfoModel(property.Name, value));
        }

        return result;
    }

    private List<MusicGridItem> GetData()
    {
        return _eventList
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == DateTime.Now.Year)
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
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

    private static MusicGridItem Convert(Event e, Music i, IEnumerable<Event> eventList)
    {
        return new MusicGridItem(i.ID, i.Artist, i.Title, i.Year, e.Bookmakred, eventList.Count());
    }

    public void SelectedItemChanged()
    {
        Info.Clear();
        Info.AddRange(GetSelectedItemInfo<Music>());

        Events.Clear();
        Events.AddRange(_eventList.Where(o => o.ItemID == selectedItem.ID));

        var item = _itemList.First(o => o.ID == selectedItem.ID);

        Cover = FileRepsitory.GetImage<Music>(item.ID);
    }
}
