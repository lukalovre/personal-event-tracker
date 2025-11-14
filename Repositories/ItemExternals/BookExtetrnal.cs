using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories.External;
using Repositories;

namespace EventTracker.Repositories;

public class BookExtetrnal : IExternal<Book>
{
    public async Task<Book> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Goodreads.UrlIdentifier))
        {
            var item = await Goodreads.GetGoodredsItem<Book>(url);

            return new Book
            {
                Title = item.Title,
                Author = item.Writer,
                Year = item.Year,
                ExternalID = item.GoodreadsID,
                is1001 = false
            };
        }

        return new Book();
    }
}
