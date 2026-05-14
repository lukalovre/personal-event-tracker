using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories.External;
using Repositories;

namespace EventTracker.Repositories;

public class MovieExternal : IExternal<Movie>
{

    public async Task<Movie> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            var item = await Imdb.GetImdbItem<Movie>(url);

            return new Movie
            {
                Title = item.Title,
                Runtime = item.Runtime,
                Year = item.Year,
                ExternalID = item.ExternalID,
                Actors = item.Actors,
                Country = item.Country,
                Director = item.Director,
                Ganre = item.Genre,
                Language = item.Language,
                Plot = item.Plot,
                Type = item.Type,
                Writer = item.Writer
            };
        }

        return new Movie();
    }

}
