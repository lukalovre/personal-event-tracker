using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class MovieExternal : IExternal<Movie>
{
    private readonly IExternal<Movie> _imdb;

    public MovieExternal()
    {
        _imdb = new Imdb();
    }

    public Movie GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            return _imdb.GetItem(url);
        }

        return new Movie();
    }

}
