using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories.External;
using Repositories;

namespace EventTracker.Repositories;

public class ComicExtetrnal : IExternal<Comic>
{
    public async Task<Comic> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Goodreads.UrlIdentifier))
        {
            var item = await Goodreads.GetGoodredsItem<Comic>(url);

            return new Comic
            {
                Title = item.Title,
                Writer = item.Writer,
                Illustrator = item.Illustrator,
                Year = item.Year,
                ExternalID = item.GoodreadsID,
                _1001 = false
            };
        }

        return new Comic();
    }
}
