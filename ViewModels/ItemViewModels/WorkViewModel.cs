using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.GridItems;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class WorkViewModel(IDatasource datasource)
: ItemViewModel<Work, WorkGridItem>(datasource, null)
{
    protected override int? DefaultNewItemChapter => null;
    protected override WorkGridItem Convert(int index, Event e, Work i, IEnumerable<Event> eventList)
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new WorkGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Type,
            eventList.Sum(o => o.Amount),
            daysAgo);
    }
}
