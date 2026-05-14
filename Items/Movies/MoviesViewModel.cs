using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using EventTracker.Models;
using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public partial class MoviesViewModel(IDatasource datasource, IExternal<Movie> external) : ItemViewModel<Movie, MovieGridItem>(datasource, external)
{
    public ObservableCollection<DiretorGridItem> DirectorGridList { get; set; } = [];

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

    protected override async void ReloadData()
    {
        base.ReloadData();

        DirectorGridList.Clear();
        DirectorGridList.AddRange(await LoadDataByDirector());
    }

    private async Task<List<DiretorGridItem>> LoadDataByDirector()
    {
        var resultGrid = new List<DiretorGridItem>();

        var type = Helpers.GetClassName<Movie>();
        var itemList = _datasource.GetList<Movie>(type);
        var eventList = _datasource.GetEventList(type);

        var directorList = itemList.DistinctBy(o => o.Director).Select(o => o.Director);

        foreach (var director in directorList)
        {
            var movieList = itemList.Where(o => o.Director == director);
            var minutesAuthor = 0;

            foreach (var movie in movieList)
            {
                var minutesMovie = eventList.Where(o => o.ItemID == movie.ID).Sum(o => o.Amount);
                minutesAuthor += minutesMovie;
            }

            var gridItem = new DiretorGridItem(1, director, minutesAuthor, movieList.Count());
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Minutes).ToList();
        return resultGrid;
    }

}
