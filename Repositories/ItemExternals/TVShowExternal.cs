using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class TVShowExternal : IExternal<TVShow>
{
    private readonly Imdb _imdb;
    private readonly YouTube _youtube;

    public TVShowExternal()
    {
        _imdb = new Imdb();
        _youtube = new YouTube();
    }

    public async Task<TVShow> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            return await _youtube.GetItem(url);
        }

        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            return await _imdb.GetItem(url);
        }

        return new TVShow();
    }
}
