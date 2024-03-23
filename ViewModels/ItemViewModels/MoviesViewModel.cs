using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MoviesViewModel(IDatasource datasource, IExternal<Movie> external)
: ItemViewModel<Movie, MovieGridItem>(datasource, external)
{
    protected override int? DefaultNewItemChapter => null;

    protected override int? NewItemAmountOverride => NewItem.Runtime;

    protected override MovieGridItem Convert(Event e, Movie i, IEnumerable<Event> eventList)
    {
        return new MovieGridItem(
            i.ID,
            i.Title,
            i.Director,
            i.Year,
            e.Rating.Value
        );
    }

}
