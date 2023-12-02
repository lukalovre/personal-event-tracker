using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MusicViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;

    public List<MusicGridItem> Music { get; set; }

    public MusicViewModel()
    {
        _datasource = new TsvDatasource();
        Music = GetData<Music, MusicGridItem>();
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
        var i = item as Music;
        return new MusicGridItem(i.Artist, i.Title, i.Year, e.Bookmakred, eventList.Count()) as T2;
    }
}
