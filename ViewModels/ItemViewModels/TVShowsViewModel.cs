using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class TVShowsViewModel(IDatasource datasource, IExternal<TVShow> external)
: ItemViewModel<TVShow, TVShowGridItem>(datasource, external)
{
    protected override int DefaultAddAmount => SelectedItem.Runtime;

    protected override TVShowGridItem Convert(
        int index,
        Event e,
        TVShow i,
        IEnumerable<Event> eventList
    )
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new TVShowGridItem(
            i.ID,
            index + 1,
            i.Title,
            e.Chapter.Value,
            eventList.Count(o => o.Chapter == e.Chapter),
            lastDate,
            daysAgo
        );
    }
}
