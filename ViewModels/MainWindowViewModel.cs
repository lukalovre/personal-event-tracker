using System.Collections.Generic;
using System.Linq;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title => "Data";

    public List<MovieGridItem> Movies { get; set; }

    public MainWindowViewModel()
    {
        var movieList = new TsvDatasource().GetList<Movie>();
        var movieEventList = new TsvDatasource().GetList<MovieEvent>();

        Movies = movieList.Select(o => Convert(o, movieEventList)).OrderBy(o => o.Date).ToList();
    }

    private MovieGridItem Convert(Movie o, List<MovieEvent> movieEventList)
    {
        var even = movieEventList.LastOrDefault(obj => obj.Imdb == o.Imdb);

        return new MovieGridItem(o.Title, o.Director, o.Year, even.Date);
    }
}
