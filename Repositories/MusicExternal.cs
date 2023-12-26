using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class MusicExternal : IExternal<Music>, IExternal<Song>
{
    private readonly Bandcamp _bandcamp;
    private readonly Spotify _spotify;
    private readonly YouTube _youtube;

    public MusicExternal()
    {
        _bandcamp = new Bandcamp();
        _spotify = new Spotify();
        _youtube = new YouTube();
    }

    public Music GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return _bandcamp.GetItem(url);
        }

        if (url.Contains(Spotify.UrlIdentifier))
        {
            return _spotify.GetItem(url);
        }

        return new Music();
    }

    Song IExternal<Song>.GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            return _youtube.GetSongItem(url);
        }

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return _bandcamp.GetSongItem(url);
        }

        return new Song();
    }
}
