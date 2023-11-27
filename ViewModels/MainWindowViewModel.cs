using System.Collections.Generic;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title => "Data";

    public List<MovieViewModel> Movies { get; set; }

    public MainWindowViewModel()
    {
        Movies = new List<MovieViewModel>();

        for (int i = 0; i < 500; i++)
        {
            Movies.Add(
                new()
                {
                    Title = "title",
                    Director = $"Director {i}",
                    Year = 2000 + i
                }
            );
        }
    }
}
