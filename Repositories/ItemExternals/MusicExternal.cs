using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class MusicExternal : IExternal<Music>
{
    private readonly IExternal<Music> _bandcamp;
    private readonly IExternal<Music> _spotify;
    private readonly IExternal<Music> _youtube;
    private readonly IExternal<Music> _soundcloud;

    public MusicExternal()
    {
        _bandcamp = new Bandcamp();
        _spotify = new Spotify();
        _youtube = new YouTube();
        _soundcloud = new Soundcloud();
    }

    public async Task<Music> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            return await _youtube.GetItem(url);
        }

        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return await _bandcamp.GetItem(url);
        }

        if (url.Contains(Spotify.UrlIdentifier))
        {
            return await _spotify.GetItem(url);
        }

        if (url.Contains(Soundcloud.UrlIdentifier))
        {
            return await _soundcloud.GetItem(url);
        }

        return new Music();
    }
}
