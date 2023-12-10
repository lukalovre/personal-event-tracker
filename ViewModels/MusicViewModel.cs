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
    private MusicGridItem _selectedItem;
    private List<Music> _itemList;
    private List<Event> _eventList;
    private string _inputUrl;
    private Music _newMusic;

    public string SearchText { get; set; }

    public bool UseNewDate
    {
        get => _useNewDate;
        set => this.RaiseAndSetIfChanged(ref _useNewDate, value);
    }

    public ObservableCollection<string> MusicPlatformTypes =>
        new ObservableCollection<string>(
            Enum.GetValues(typeof(eMusicPlatformType))
                .Cast<eMusicPlatformType>()
                .Select(v => v.ToString())
        );

    public ObservableCollection<PersonComboBoxItem> PeopleList =>
        new ObservableCollection<PersonComboBoxItem>(PeopleManager.Instance.GetComboboxList());

    public PersonComboBoxItem SelectedPerson { get; set; }

    public ObservableCollection<MusicGridItem> Music { get; set; }
    public ObservableCollection<MusicGridItem> ArtistMusic { get; set; }

    public ObservableCollection<InfoModel> Info { get; set; }

    public Music SelectedMusic
    {
        get => _selectedMusic;
        set => this.RaiseAndSetIfChanged(ref _selectedMusic, value);
    }
    public ObservableCollection<Event> Events { get; set; }

    public ReactiveCommand<Unit, Unit> AddClick { get; }
    public ReactiveCommand<Unit, Unit> OpenLink { get; }
    public ReactiveCommand<Unit, Unit> OpenImage { get; }
    public ReactiveCommand<Unit, Unit> ListenAgain { get; }
    public ReactiveCommand<Unit, Unit> Search { get; }

    public Music NewMusic
    {
        get => _newMusic;
        set => this.RaiseAndSetIfChanged(ref _newMusic, value);
    }

    public DateTime NewDate { get; set; } =
        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

    public TimeSpan NewTime { get; set; } = new TimeSpan();
    public Event NewEvent
    {
        get => _newEvent;
        set => this.RaiseAndSetIfChanged(ref _newEvent, value);
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

    private Bitmap? _cover;

    public Bitmap? Cover
    {
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    private Bitmap? _newMusicCover;
    private Event _newEvent;

    private bool _useNewDate;
    private Music _selectedMusic;

    public Bitmap? NewMusicCover
    {
        get => _newMusicCover;
        private set => this.RaiseAndSetIfChanged(ref _newMusicCover, value);
    }

    private void InputUrlChanged()
    {
        NewMusic = MusicRepository.GetAlbumInfo(InputUrl);

        NewMusicCover = FileRepsitory.GetImageTemp<Music>();
        NewEvent = new Event
        {
            Amount = NewMusic.Runtime,
            Rating = 1,
            Platform = eMusicPlatformType.Streaming.ToString()
        };

        FindMusic(NewMusic);

        _inputUrl = default;
    }

    private void FindMusic(Music music)
    {
        ArtistMusic.Clear();
        ArtistMusic.AddRange(LoadArtistData(music));
    }

    public MusicGridItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            SelectedItemChanged();
        }
    }

    public MusicViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        Music = new ObservableCollection<MusicGridItem>(LoadData());
        Info = new ObservableCollection<InfoModel>();
        ArtistMusic = new ObservableCollection<MusicGridItem>();

        Events = new ObservableCollection<Event>();
        EventViewModel = new EventViewModel(Events);

        AddClick = ReactiveCommand.Create(AddClickAction);
        OpenLink = ReactiveCommand.Create(OpenLinkAction);
        OpenImage = ReactiveCommand.Create(OpenImageAction);
        ListenAgain = ReactiveCommand.Create(ListenAgainAction);
        Search = ReactiveCommand.Create(SearchAction);

        SelectedItem = Music.LastOrDefault();
    }

    private void SearchAction()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Music.Clear();
            Music.AddRange(LoadData());
        }

        var searchMusic = new Music { Artist = SearchText, Title = SearchText };

        Music.Clear();
        Music.AddRange(LoadArtistData(searchMusic));
    }

    private void OpenImageAction() { }

    private void OpenLinkAction()
    {
        var hyperlink = SelectedMusic.SpotifyID;

        if (string.IsNullOrWhiteSpace(SelectedMusic.SpotifyID))
        {
            var artist = SelectedMusic.Artist.Split(' ').ToList();
            var title = SelectedMusic.Title.Split(' ').ToList();
            var year = SelectedMusic.Year.ToString();

            var parts = new List<string>();

            parts.AddRange(artist);
            parts.AddRange(title);
            parts.Add(year);

            hyperlink = $"https://duckduckgo.com/?q={string.Join("+", parts)}";
        }

        HtmlHelper.OpenLink(hyperlink);
    }

    private void AddClickAction()
    {
        NewEvent.DateEnd = UseNewDate ? NewDate + NewTime : DateTime.Now;
        NewEvent.DateStart =
            NewEvent.DateEnd.Value.TimeOfDay.Ticks == 0
                ? NewEvent.DateEnd.Value
                : NewEvent.DateEnd.Value.AddMinutes(-NewEvent.Amount);
        NewEvent.People = SelectedPerson?.ID.ToString() ?? null;

        _datasource.Add(NewMusic, NewEvent);

        Music.Clear();
        Music.AddRange(LoadData());
        SelectedItem = Music.LastOrDefault();

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
                : lastEvent.DateEnd.Value.AddMinutes(-NewEvent.Amount);

        _datasource.Add(SelectedMusic, lastEvent);

        Music.Clear();
        Music.AddRange(LoadData());
        SelectedItem = Music.LastOrDefault();

        ClearNewItemControls();
    }

    private void ClearNewItemControls()
    {
        NewMusic = default;
        NewEvent = default;
        NewMusicCover = default;
        InputUrl = default;
        SelectedPerson = default;
        ArtistMusic.Clear();
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

    private List<MusicGridItem> LoadData()
    {
        _itemList = _datasource.GetList<Music>();
        _eventList = _datasource.GetEventList<Music>();

        return _eventList
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value >= DateTime.Now.AddDays(-5))
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

    private List<MusicGridItem> LoadArtistData(Music item)
    {
        _itemList = _datasource.GetList<Music>();
        _eventList = _datasource.GetEventList<Music>();

        return _eventList
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
            .Where(
                o =>
                    o.Artist.Contains(item.Artist, StringComparison.InvariantCultureIgnoreCase)
                    || o.Title.Contains(item.Title, StringComparison.InvariantCultureIgnoreCase)
            )
            .ToList();
    }

    private static MusicGridItem Convert(Event e, Music i, IEnumerable<Event> eventList)
    {
        return new MusicGridItem(
            i.ID,
            i.Artist,
            i.Title,
            i.Year,
            i.Runtime,
            e.Bookmakred,
            eventList.Count()
        );
    }

    public void SelectedItemChanged()
    {
        Info.Clear();
        Events.Clear();
        Cover = null;

        if (SelectedItem == null)
        {
            return;
        }

        SelectedMusic = _itemList.First(o => o.ID == SelectedItem.ID);
        Info.AddRange(GetSelectedItemInfo<Music>());
        Events.AddRange(_eventList.Where(o => o.ItemID == SelectedItem.ID));

        var item = _itemList.First(o => o.ID == SelectedItem.ID);
        Cover = FileRepsitory.GetImage<Music>(item.ID);
    }
}
