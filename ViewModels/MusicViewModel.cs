using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MusicViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private MusicGridItem selectedItem;

    public ObservableCollection<MusicGridItem> Music { get; set; }
    public ObservableCollection<InfoModel> Info { get; set; }

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

        Music = new ObservableCollection<MusicGridItem>(GetData<Music, MusicGridItem>());
        Info = new ObservableCollection<InfoModel>();
    }

    private List<InfoModel> GetSelectedItemInfo()
    {
        return new List<InfoModel>
        {
            new("Artist", selectedItem.Artist),
            new("Title", selectedItem.Title)
        };
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

    public void SelectedItemChanged()
    {
        Info.Clear();
        Info.AddRange(GetSelectedItemInfo());
    }
}
