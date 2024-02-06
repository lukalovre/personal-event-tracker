using System.Threading.Tasks;
using AvaloniaApplication1.Repositories.External;

namespace AvaloniaApplication1.Repositories;

public class SongExternal : IExternal<Song>
{
    private readonly IExternal<Song> _bandcamp;
    private readonly IExternal<Song> _soundcloud;
    private readonly IExternal<Song> _youtube;

    public SongExternal()
    {
        _bandcamp = new Bandcamp();
        _youtube = new YouTube();
        _soundcloud = new Soundcloud();
    }

    public async Task<Song> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            return await _youtube.GetItem(url);
        }

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return await _bandcamp.GetItem(url);
        }

        if (url.Contains(Soundcloud.UrlIdentifier))
        {
            return await _soundcloud.GetItem(url);
        }

        return new Song();
    }
}
