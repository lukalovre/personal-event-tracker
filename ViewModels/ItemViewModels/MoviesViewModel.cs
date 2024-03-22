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
    protected override int? DefaultNewItemChapter => null;

    protected override int? NewItemAmountOverride => NewItem.Runtime;

    protected override MovieGridItem Convert(
         int index,
         Event e,
         Movie i,
         IEnumerable<Event> eventList
     )
    {
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
