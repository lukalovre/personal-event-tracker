using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MoviesViewModel MoviesViewModel { get; }
    public StandupViewModel StandupViewModel { get; }
    public MusicViewModel MusicViewModel { get; }
    public WorkViewModel WorkViewModel { get; }
    public BooksViewModel BooksViewModel { get; }
    public ComicsViewModel ComicsViewModel { get; }
    public GamesViewModel GamesViewModel { get; }
    public TVShowsViewModel TVShowsViewModel { get; }
    public ClipsViewModel ClipsViewModel { get; }
    public SongsViewModel SongsViewModel { get; }
    public ZooViewModel ZooViewModel { get; }
    public LocationsViewModel LocationsViewModel { get; }

    public MainWindowViewModel(IDatasource datasource)
    {
        MoviesViewModel = new MoviesViewModel(datasource, new MovieExternal());
        StandupViewModel = new StandupViewModel(datasource, new StandupExternal());
        MusicViewModel = new MusicViewModel(datasource, new MusicExternal());
        WorkViewModel = new WorkViewModel(datasource);
        BooksViewModel = new BooksViewModel(datasource, new BookExtetrnal());
        ComicsViewModel = new ComicsViewModel(datasource, new ComicExtetrnal());
        GamesViewModel = new GamesViewModel(datasource, new GameExtetrnal());
        TVShowsViewModel = new TVShowsViewModel(datasource, new TVShowExternal());
        ClipsViewModel = new ClipsViewModel(datasource, new ClipsExternal());
        SongsViewModel = new SongsViewModel(datasource, new SongExternal());
        ZooViewModel = new ZooViewModel(datasource);
        LocationsViewModel = new LocationsViewModel(datasource);
    }

}
