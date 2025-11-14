using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public class MainWindowViewModel(IDatasource datasource) : ViewModelBase
{
    public MoviesViewModel MoviesViewModel { get; } = new MoviesViewModel(datasource, new MovieExternal());
    public StandupViewModel StandupViewModel { get; } = new StandupViewModel(datasource, new StandupExternal());
    public TheatreViewModel TheatreViewModel { get; } = new TheatreViewModel(datasource);
    public MusicViewModel MusicViewModel { get; } = new MusicViewModel(datasource, new MusicExternal());
    public WorkViewModel WorkViewModel { get; } = new WorkViewModel(datasource);
    public BooksViewModel BooksViewModel { get; } = new BooksViewModel(datasource, new BookExtetrnal());
    public ComicsViewModel ComicsViewModel { get; } = new ComicsViewModel(datasource, new ComicExtetrnal());
    public GamesViewModel GamesViewModel { get; } = new GamesViewModel(datasource, new GameExtetrnal());
    public TVShowsViewModel TVShowsViewModel { get; } = new TVShowsViewModel(datasource, new TVShowExternal());
    public ClipsViewModel ClipsViewModel { get; } = new ClipsViewModel(datasource, new ClipsExternal());
    public SongsViewModel SongsViewModel { get; } = new SongsViewModel(datasource, new SongExternal());
    public ZooViewModel ZooViewModel { get; } = new ZooViewModel(datasource);
    public MagazinesViewModel MagazinesViewModel { get; } = new MagazinesViewModel(datasource);
    public BoardgamesViewModel BoardgamesViewModel { get; } = new BoardgamesViewModel(datasource);
    public DnDViewModel DnDViewModel { get; } = new DnDViewModel(datasource);
    public ClassicalViewModel ClassicalViewModel { get; } = new ClassicalViewModel(datasource);
    public PaintingsViewModel PaintingsViewModel { get; } = new PaintingsViewModel(datasource);
    public PinballViewModel PinballViewModel { get; } = new PinballViewModel(datasource);
    public PeopleViewModel PeopleViewModel { get; } = new PeopleViewModel(datasource);
    public ConcertsViewModel ConcertsViewModel { get; } = new ConcertsViewModel(datasource);
    public LocationsViewModel LocationsViewModel { get; } = new LocationsViewModel(datasource);
    public AdventuresViewModel AdventuresViewModel { get; } = new AdventuresViewModel(datasource);
    public StatsViewModel StatsViewModel { get; } = new StatsViewModel(datasource);
    public TimelineViewModel TimelineViewModel { get; } = new TimelineViewModel(datasource);
}
