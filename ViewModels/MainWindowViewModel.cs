using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.GridItems;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MoviesViewModel MoviesViewModel { get; } = new MoviesViewModel(new TsvDatasource(), new MovieExternal());
    public MusicViewModel MusicViewModel { get; } = new MusicViewModel(new TsvDatasource(), new MusicExternal());
    public WorkViewModel WorkViewModel { get; } = new WorkViewModel(new TsvDatasource());
    public BooksViewModel BooksViewModel { get; } = new BooksViewModel(new TsvDatasource(), new BookExtetrnal());
    public ComicsViewModel ComicsViewModel { get; } = new ComicsViewModel(new TsvDatasource(), new ComicExtetrnal());
    public GamesViewModel GamesViewModel { get; } = new GamesViewModel(new TsvDatasource());
    public TVShowsViewModel TVShowsViewModel { get; } = new TVShowsViewModel(new TsvDatasource(), new TVShowExternal());
    public SongsViewModel SongsViewModel { get; } = new SongsViewModel(new TsvDatasource(), new SongExternal());

    private readonly IDatasource _datasource;

    public List<MusicGridItem> Music { get; set; }
    public List<ComicGridItem> Comics { get; set; }
    public List<ZooGridItem> Zoo { get; set; }

    public MainWindowViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        // _datasource.GetEventListConvert<MyWork>();
        Zoo = GetData<Zoo, ZooGridItem>();
    }

    private List<T2> GetData<T1, T2>(bool getAllData = false)
        where T1 : IItem
        where T2 : class
    {
        var itemList = _datasource.GetList<T1>();
        var eventList = _datasource.GetEventList<T1>();

        var dateFilter = getAllData ? DateTime.MinValue : DateTime.Now.AddYears(-1);

        return eventList
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value >= dateFilter)
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Select(
                o =>
                    Convert<T1, T2>(
                        o,
                        itemList.First(m => m.ID == o.ItemID),
                        eventList.Where(e => e.ItemID == o.ItemID)
                    )
            )
            .ToList();
    }

    private T2 Convert<T1, T2>(Event e, T1 item, IEnumerable<Event> eventList)
        where T1 : IItem
        where T2 : class
    {

        if (typeof(T1) == typeof(Zoo))
        {
            var i = item as Zoo;
            return new ZooGridItem(i.Name, i.City, i.Country, e.DateEnd.Value.Year) as T2;
        }

        return null;
    }
}
