using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class StandupViewModel(IDatasource datasource, IExternal<Standup> external)
: ItemViewModel<Standup, StandupGridItem>(datasource, external)
{
    protected override DateTime? DateTimeFilter => new DateTime(DateTime.Now.Year, 1, 1);

    protected override int? DefaultNewItemChapter => null;
    protected override StandupGridItem Convert(int index, Event e, Standup i, IEnumerable<Event> eventList)
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new StandupGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Director,
            i.Year,
            e.Rating.Value
        );
    }

}
