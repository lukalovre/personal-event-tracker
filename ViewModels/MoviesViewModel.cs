using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MoviesViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;

    public List<MovieGridItem> Movies { get; set; }

    public MoviesViewModel()
    {
        _datasource = new TsvDatasource();
        Movies = GetData<Movie, MovieGridItem>();
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
        var i = item as Movie;
        return new MovieGridItem(i.Title, i.Director, i.Year, e.DateEnd) as T2;
    }
}
