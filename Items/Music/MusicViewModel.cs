using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace EventTracker.ViewModels;

public partial class MusicViewModel(IDatasource datasource, IExternal<Music> external) : ItemViewModel<Music, MusicGridItem>(datasource, external)
{
    protected override string DefaultGridTabHeader => "Info";

    private int _gridCountMusicTodo1;
    private int _gridCountMusicTodo2;

    public ObservableCollection<MusicGridItem> MusicTodo1 { get; set; } = [];
    public ObservableCollection<MusicGridItem> MusicTodo2 { get; set; } = [];
    public ObservableCollection<ArtistGridItem> GridItemsArtists { get; set; } = [];
    public ObservableCollection<AlbumGridItem> GridItemsAlbums { get; set; } = [];

    protected override List<string> GetAlternativeOpenLinkSearchParams()
    {
        var openLinkParams = SelectedItem.Artist.Split(' ').ToList();
        openLinkParams.AddRange(SelectedItem.Title.Split(' '));
        openLinkParams.AddRange([SelectedItem.Year.ToString()]);

        return openLinkParams;
    }

    public int GridCountMusicTodo1
    {
        get => _gridCountMusicTodo1;
        private set => this.RaiseAndSetIfChanged(ref _gridCountMusicTodo1, value);
    }

    public int GridCountMusicTodo2
    {
        get => _gridCountMusicTodo2;
        private set => this.RaiseAndSetIfChanged(ref _gridCountMusicTodo2, value);
    }

    protected override async void ReloadData()
    {
        base.ReloadData();

        MusicTodo1.Clear();
        MusicTodo1.AddRange(LoadDataBookmarked(1));
        GridCountMusicTodo1 = MusicTodo1.Count;

        MusicTodo2.Clear();
        MusicTodo2.AddRange(LoadDataBookmarked(2));
        GridCountMusicTodo2 = MusicTodo2.Count;

        GridItemsArtists.Clear();
        GridItemsArtists.AddRange(LoadDataByArtist());

        GridItemsAlbums.Clear();
        GridItemsAlbums.AddRange(LoadDataByAlbum());
    }

    protected override async Task LoadGridTabData(string? tabHeader)
    {
        switch (tabHeader)
        {
            case "Todo 1":
                LoadItemsAndEvents(out _, out _, realLoad: true);
                MusicTodo1.AddRange(LoadDataBookmarked(1));
                GridCountMusicTodo1 = MusicTodo1.Count;
                return;
            case "Todo 2":
                LoadItemsAndEvents(out _, out _, realLoad: true);
                MusicTodo2.AddRange(LoadDataBookmarked(2));
                GridCountMusicTodo2 = MusicTodo2.Count;
                return;
            case "Artists":
                LoadItemsAndEvents(out _, out _, realLoad: true);
                GridItemsArtists.AddRange(LoadDataByArtist());
                return;
            case "Albums":
                LoadItemsAndEvents(out _, out _, realLoad: true);
                GridItemsAlbums.AddRange(LoadDataByAlbum());
                return;
            default:
                await base.LoadGridTabData(tabHeader);
                return;
        }
    }

    protected override int GetLoadedGridCount(string? tabHeader)
    {
        return tabHeader switch
        {
            "Todo 1" => MusicTodo1.Count,
            "Todo 2" => MusicTodo2.Count,
            "Artists" => GridItemsArtists.Count,
            "Albums" => GridItemsAlbums.Count,
            _ => base.GetLoadedGridCount(tabHeader)
        };
    }

    private List<ArtistGridItem> LoadDataByArtist()
    {
        var resultGrid = new List<ArtistGridItem>();

        LoadItemsAndEvents(out List<Music> itemList, out List<Event> eventList);

        var artistList = itemList.DistinctBy(o => o.Artist).Select(o => o.Artist);

        foreach (var artist in artistList)
        {
            var albumsList = itemList.Where(o => o.Artist == artist);
            var minutesArtist = 0;

            foreach (var album in albumsList)
            {
                var minutesAlbum = eventList.Where(o => o.ItemID == album.ID).Sum(o => o.Amount);
                minutesArtist += minutesAlbum;
            }

            var gridItem = new ArtistGridItem(1, artist, minutesArtist, albumsList.Count());
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Minutes).ToList();
        return resultGrid;
    }

    private List<AlbumGridItem> LoadDataByAlbum()
    {
        var resultGrid = new List<AlbumGridItem>();

        LoadItemsAndEvents(out List<Music> itemList, out List<Event> eventList);

        foreach (var album in itemList)
        {
            var events = eventList.Where(o => o.ItemID == album.ID);
            var minutesAlbum = events.Sum(o => o.Amount);

            var gridItem = new AlbumGridItem(1, album.Artist, album.Title, minutesAlbum, events.Count());
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Minutes).ToList();
        return resultGrid;
    }

    protected override MusicGridItem Convert(Event e, Music i, IEnumerable<Event> eventList)
    {
        return new MusicGridItem(
            i.ID,
            i.Artist,
            i.Title,
            i.Year,
            i.Runtime,
            e.Bookmarked,
            eventList.Count(),
            e.Platform
        );
    }

    protected override string GetFilterByOnNewItem()
    {
        return NewItem.Artist;
    }
}
