using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class TVShowExternal : IExternal<TVShow>
{
    public async Task<TVShow> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            var item = await YouTube.GetYoutubeItem<TVShow>(url);

            return new TVShow
            {
                Title = item.Title,
                ExternalID = item.Link,
                Year = item.Year,
                Runtime = item.Runtime
            };
        }

        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            var item = await Imdb.GetImdbItem<TVShow>(url);

            return new TVShow
            {
                Title = item.Title,
                Runtime = item.Runtime,
                Year = item.Runtime,
                ExternalID = item.ExternalID,
                Actors = item.Actors,
                Country = item.Country,
                Director = item.Director,
                Genre = item.Genre,
                Language = item.Language,
                Plot = item.Plot,
                Type = item.Type,
                Writer = item.Writer
            };
        }

        return new TVShow();
    }
}
