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
        var movieList = new JsonnDatasource().GetList<Movie>();

        Movies = movieList.Select(o => Convert(o)).ToList();
    }

    private MovieGridItem Convert(Movie o)
    {
        return new MovieGridItem
        {
            Title = o.Actors,
            Director = o.Director,
            Year = 2001
        };
    }
}
