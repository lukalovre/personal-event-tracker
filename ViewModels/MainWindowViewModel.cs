using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MoviesViewModel MoviesViewModel { get; }
    public StandupViewModel StandupViewModel { get; }
    public MusicViewModel MusicViewModel { get; }
    public WorkViewModel WorkViewModel { get; }
    public BooksViewModel BooksViewModel { get; }
    public ComicsViewModel ComicsViewModel { get; }
    public GamesViewModel GamesViewModel { get; }
    public TVShowsViewModel TVShowsViewModel { get; }
    public SongsViewModel SongsViewModel { get; }
    public List<ZooGridItem> Zoo { get; set; }

    public MainWindowViewModel(IDatasource datasource)
    {
        MoviesViewModel = new MoviesViewModel(datasource, new MovieExternal());
        StandupViewModel = new StandupViewModel(datasource, new StandupExternal());
        MusicViewModel = new MusicViewModel(datasource, new MusicExternal());
        WorkViewModel = new WorkViewModel(datasource);
        BooksViewModel = new BooksViewModel(datasource, new BookExtetrnal());
        ComicsViewModel = new ComicsViewModel(datasource, new ComicExtetrnal());
        GamesViewModel = new GamesViewModel(datasource, new GameExtetrnal());
        TVShowsViewModel = new TVShowsViewModel(datasource, new TVShowExternal());
        SongsViewModel = new SongsViewModel(datasource, new SongExternal());

        // _datasource.GetEventListConvert<MyWork>();
        Zoo = GetData<Zoo, ZooGridItem>(datasource);
    }

    private List<T2> GetData<T1, T2>(IDatasource datasource)
        where T1 : IItem
        where T2 : class
    {
        var itemList = datasource.GetList<T1>();
        var eventList = datasource.GetEventList<T1>();

        var dateFilter = DateTime.MinValue;

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
            return new ZooGridItem(1, i.Name, i.City, i.Country, e.DateEnd.Value.Year) as T2;
        }

        return null;
    }
}
