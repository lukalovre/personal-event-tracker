using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using EventTracker.Models;
using EventTracker.Repositories;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class TVShowsViewModel(IDatasource datasource, IExternal<TVShow> external) : ItemViewModel<TVShow, TVShowGridItem>(datasource, external)
{
    public ObservableCollection<TVShowsGridItem> TVShowList { get; set; } = [];

    protected override TVShowGridItem Convert(Event e, TVShow i, IEnumerable<Event> eventList)
    {
        return new TVShowGridItem(
            i.ID,
            i.Title,
            e?.Chapter ?? 0,
            eventList.Count(o => o.Chapter == e?.Chapter),
            e?.Rating ?? 1,
            eventList.LastEventDate()
        );
    }

    protected override async void ReloadData()
    {
        base.ReloadData();

        TVShowList.Clear();
        TVShowList.AddRange(await LoadDataByDirector());
    }

    private async Task<List<TVShowsGridItem>> LoadDataByDirector()
    {
        var resultGrid = new List<TVShowsGridItem>();

        var type = Helpers.GetClassName<TVShow>();
        var tvShowList = _datasource.GetList<TVShow>(type);
        var eventList = _datasource.GetEventList(type);

        foreach (var tvShow in tvShowList)
        {
            var events = eventList.Where(o => o.ItemID == tvShow.ID);
            var minutesPerTVShow = events.Sum(o => o.Amount);
            var gridItem = new TVShowsGridItem(1, tvShow.Title, events.Count(), minutesPerTVShow);
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Minutes).ToList();
        return resultGrid;
    }
}
