using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories.External;
using Repositories;

namespace EventTracker.Repositories;

public class GameExtetrnal : IExternal<Game>
{
    public async Task<Game> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Igdb.UrlIdentifier))
        {
            var item = await Igdb.GetItem(url);

            return new Game
            {
                ExternalID = item.ExternalID,
                Title = item.Title,
                Year = item.Year
            };
        }

        return new Game();
    }
}
