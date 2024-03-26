using System.Threading.Tasks;
using AvaloniaApplication1.Models;
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

    public async Task<Movie> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            return await _imdb.GetItem(url);
        }

        return new Movie();
    }

}
