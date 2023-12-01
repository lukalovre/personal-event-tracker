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

    public MainWindowViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        Movies = GetData<Movie, MovieGridItem>();
        Books = GetData<Book, BookGridItem>();
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
            .Select(o => Convert<T1, T2>(o, itemList.First(m => m.ID == o.ItemID)))
            .ToList();
    }

    private T2 Convert<T1, T2>(Event e, T1 item)
        where T1 : IItem
        where T2 : class
    {
        if (typeof(T1) == typeof(Movie))
        {
            var movie = item as Movie;
            return new MovieGridItem(movie.Title, movie.Director, movie.Year, e.DateEnd) as T2;
        }

        if (typeof(T1) == typeof(Book))
        {
            var book = item as Book;
            return new BookGridItem(book.Title, book.Author, book.Year, e.Rating, e.DateEnd) as T2;
        }

        return null;
    }
}
