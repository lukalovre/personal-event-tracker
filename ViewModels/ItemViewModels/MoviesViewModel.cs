using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public partial class MoviesViewModel(IDatasource datasource, IExternal<Movie> external) : ItemViewModel<Movie, MovieGridItem>(datasource, external)
{
    protected override MovieGridItem Convert(Event e, Movie i, IEnumerable<Event> eventList)
    {
        return new MovieGridItem(
            i.ID,
            i.Title,
            i.Director,
            i.Year,
            e?.Rating ?? 0
        );
    }

}
