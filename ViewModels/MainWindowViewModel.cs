using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title => "Data";

    public List<MovieGridItem> Movies { get; set; }

    public MainWindowViewModel(IDatasource datasource)
    {
        var movieList = datasource.GetList<Movie>();
        var eventList = datasource.GetEventList<Movie>();

        Movies = movieList
            .Select(o => Convert(o, eventList))
            .Where(o => o.Date.HasValue && o.Date.Value.Year == DateTime.Now.Year)
            .OrderBy(o => o.Date)
            .ToList();
    }

    private MovieGridItem Convert(Movie o, List<Event> eventList)
    {
        var even = eventList.LastOrDefault(obj => obj.ItemID == o.Imdb);

        return new MovieGridItem(o.Title, o.Director, o.Year, even.Date);
    }
}
