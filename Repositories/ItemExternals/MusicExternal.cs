using System.Threading.Tasks;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class MusicExternal : IExternal<Music>
{
    private readonly IExternal<Music> _bandcamp;
    private readonly IExternal<Music> _spotify;
    private readonly IExternal<Music> _youtube;

    public MusicExternal()
    {
        _bandcamp = new Bandcamp();
        _spotify = new Spotify();
        _youtube = new YouTube();
    }

    public async Task<Music> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return await _bandcamp.GetItem(url);
        }

        if (url.Contains(Spotify.UrlIdentifier))
        {
            return await _spotify.GetItem(url);
        }

        if (url.Contains(YouTube.UrlIdentifier))
        {
            return await _youtube.GetItem(url);
        }

        return new Music();
    }
}
