using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class BookExtetrnal : IExternal<Book>
{
    private readonly Goodreads _goodreads;

    public BookExtetrnal()
    {
        _goodreads = new Goodreads();
    }

    public Book GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Goodreads.UrlIdentifier))
        {
            return _goodreads.GetItem(url);
        }

        return new Book();
    }
}
