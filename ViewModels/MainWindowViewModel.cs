using System.Collections.Generic;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title => "Data";

    public List<Movie> Movies { get; set; }

    public MainWindowViewModel()
    {
        Movies = new List<Movie>();

        for (int i = 0; i < 500; i++)
        {
            Movies.Add(new() { Director = $"Directore {i}" });
        }
    }
}
