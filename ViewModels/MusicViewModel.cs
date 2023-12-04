using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MusicViewModel : ViewModelBase
{
    public EventViewModel EventViewModel { get; }
    private readonly IDatasource _datasource;
    private MusicGridItem selectedItem;
    private List<Music> _itemList;
    private List<Event> _eventList;
    private string inputUrl;
    private string url;

    public ObservableCollection<MusicGridItem> Music { get; set; }
    public ObservableCollection<InfoModel> Info { get; set; }
    public ObservableCollection<Event> Events { get; set; }

    public string InputUrl
    {
        get => inputUrl;
        set
        {
            inputUrl = value;
            InputUrlChanged();
        }
    }

    public string Url
    {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

    private void InputUrlChanged()
    {
        Url = "great success!:  " + InputUrl;
    }

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
        _itemList = _datasource.GetList<Music>();
        _eventList = _datasource.GetEventList<Music>();

        Music = new ObservableCollection<MusicGridItem>(GetData());
        Info = new ObservableCollection<InfoModel>();

        Events = new ObservableCollection<Event>();
        EventViewModel = new EventViewModel(Events);
    }

    private List<InfoModel> GetSelectedItemInfo<T>()
    {
        var result = new List<InfoModel>();

        var properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var e = _eventList.First(o => o.ItemID == selectedItem.ID);
            var i = _itemList.First(o => o.ID == e.ItemID);

            var value = property.GetValue(i);
            result.Add(new InfoModel(property.Name, value));
        }

        return result;
    }

    private List<MusicGridItem> GetData()
    {
        return _eventList
            .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == DateTime.Now.Year)
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

    private MusicGridItem Convert(Event e, Music i, IEnumerable<Event> eventList)
    {
        return new MusicGridItem(i.ID, i.Artist, i.Title, i.Year, e.Bookmakred, eventList.Count());
    }

    public void SelectedItemChanged()
    {
        Info.Clear();
        Info.AddRange(GetSelectedItemInfo<Music>());

        Events.Clear();
        Events.AddRange(_eventList.Where(o => o.ItemID == selectedItem.ID));
    }
}
