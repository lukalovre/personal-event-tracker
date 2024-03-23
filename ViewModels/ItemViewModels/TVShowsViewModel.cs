using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class TVShowsViewModel(IDatasource datasource, IExternal<TVShow> external)
: ItemViewModel<TVShow, TVShowGridItem>(datasource, external)
{
    protected override int DefaultAddAmount => SelectedItem.Runtime;

    protected override TVShowGridItem Convert(Event e, TVShow i, IEnumerable<Event> eventList)
    {
        return new TVShowGridItem(
            i.ID,
            i.Title,
            e.Chapter.Value,
            eventList.Count(o => o.Chapter == e.Chapter),
            eventList.LastEventDate()
        );
    }
}
