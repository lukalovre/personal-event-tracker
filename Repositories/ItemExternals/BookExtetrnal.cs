using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

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
