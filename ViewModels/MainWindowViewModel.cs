using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title => "Data";

    public List<MovieGridItem> Movies { get; set; }
    public List<MovieGridItem> Books { get; set; }

    public MainWindowViewModel(IDatasource datasource)
    {
        // var movieList = datasource.GetList<Movie>();
        // var movieEventList = datasource.GetEventList<Movie>();

        // Movies = movieEventList
        //     .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == DateTime.Now.Year)
        //     .OrderBy(o => o.DateEnd)
        //     .Select(o => Convert(o, movieList.First(m => m.ID == o.ItemID)))
        //     .ToList();

        var bookList = datasource.GetList<Book>();
        var bookEventList = datasource.GetEventList<Book>();

        Books = bookEventList
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == DateTime.Now.Year)
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Select(o => Convert(o, bookList.First(m => m.ID == o.ItemID)))
            .ToList();
    }

    private MovieGridItem Convert(Event e, Movie m)
    {
        return new MovieGridItem(m.Title, m.Director, m.Year, e.DateEnd);
    }

    private MovieGridItem Convert(Event e, Book m)
    {
        return new MovieGridItem(m.Title, m.Author, m.Year, e.DateEnd);
    }
}
