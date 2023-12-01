using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title => "Data";

    private readonly IDatasource _datasource;

    public List<MovieGridItem> Movies { get; set; }
    public List<BookGridItem> Books { get; set; }
    public List<MusicGridItem> Music { get; set; }
    public List<TVShowGridItem> TVShows { get; set; }
    public List<ComicGridItem> Comics { get; set; }
    public List<SongGridItem> Songs { get; set; }

    public MainWindowViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        // _datasource.GetEventListConvert<Song>();

        Movies = GetData<Movie, MovieGridItem>();
        Books = GetData<Book, BookGridItem>();
        Music = GetData<Music, MusicGridItem>();
        TVShows = GetData<TVShow, TVShowGridItem>();
        Comics = GetData<Comic, ComicGridItem>();
        Songs = GetData<Song, SongGridItem>();
    }

    private List<T2> GetData<T1, T2>()
        where T1 : IItem
        where T2 : class
    {
        var itemList = _datasource.GetList<T1>();
        var eventList = _datasource.GetEventList<T1>();

        return eventList
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == DateTime.Now.Year)
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
        if (typeof(T1) == typeof(Movie))
        {
            var i = item as Movie;
            return new MovieGridItem(i.Title, i.Director, i.Year, e.DateEnd) as T2;
        }

        if (typeof(T1) == typeof(Book))
        {
            var i = item as Book;
            return new BookGridItem(i.Title, i.Author, i.Year, e.Rating, e.DateEnd) as T2;
        }

        if (typeof(T1) == typeof(Music))
        {
            var i = item as Music;
            return new MusicGridItem(i.Artist, i.Title, i.Year, e.Bookmakred, eventList.Count())
                as T2;
        }

        if (typeof(T1) == typeof(TVShow))
        {
            var i = item as TVShow;
            return new TVShowGridItem(
                    i.Title,
                    e.Chapter ?? 1,
                    eventList.Count(o => o.Chapter == e.Chapter)
                ) as T2;
        }

        if (typeof(T1) == typeof(Comic))
        {
            var i = item as Comic;
            return new ComicGridItem(
                    i.Title,
                    i.Writer,
                    e.Chapter,
                    eventList.Where(o => o.Chapter == e.Chapter).Sum(o => o.Amount),
                    e.Rating
                ) as T2;
        }

        if (typeof(T1) == typeof(Song))
        {
            var i = item as Song;
            return new SongGridItem(i.Artist, i.Title, i.Year, eventList.Count(), e.Bookmakred)
                as T2;
        }

        return null;
    }
}
