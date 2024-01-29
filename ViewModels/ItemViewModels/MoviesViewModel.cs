using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MoviesViewModel(IDatasource datasource, IExternal<Movie> external)
: ItemViewModel<Movie, MovieGridItem>(datasource, external)
{
    public override MovieGridItem Convert(
         int index,
         Event e,
         Movie i,
         IEnumerable<Event> eventList
     )
    {
        var lastDate = eventList.MaxBy(o => o.DateEnd)?.DateEnd ?? DateTime.MinValue;
        var daysAgo = (int)(DateTime.Now - lastDate).TotalDays;

        return new MovieGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Director,
            i.Year,
            e.Rating.Value
        );
    }

}
