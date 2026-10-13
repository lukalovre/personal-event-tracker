using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EventTracker.Models;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class WorkViewModel(IDatasource datasource) : ItemViewModel<Work, WorkGridItem>(datasource, null!)
{
    public ObservableCollection<WorkGridItem> GridItemsCompleted { get; set; } = [];

    protected override WorkGridItem Convert(Event e, Work i, IEnumerable<Event> eventList)
    {
        return new WorkGridItem(
            i.ID,
            i.Title,
            i.Type,
            eventList.Sum(o => o.Amount),
            eventList.LastEventDate());
    }

    protected override async void ReloadData()
    {
        base.ReloadData();

        var allItems = await LoadData(skipFilters: true);
        LoadItemsAndEvents(out _, out var eventList);

        var completedItemIDs = eventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .Where(o => o.Completed)
            .Select(o => o.ItemID)
            .ToHashSet();

        GridItemsCompleted.Clear();
        foreach (var item in allItems.Where(o => completedItemIDs.Contains(o.ID)))
        {
            GridItemsCompleted.Add(item);
        }
    }
}
