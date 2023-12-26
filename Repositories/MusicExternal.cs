using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class MusicExternal : IExternal<Music>, IExternal<Song>
{
    private readonly IExternal<Music> _bandcampMusic;
    private readonly IExternal<Song> _bandcampSong;
    private readonly Spotify _spotify;
    private readonly Soundcloud _soundcloud;
    private readonly IExternal<Song> _youtubeSong;

    public MusicExternal()
    {
        var bandcamp = new Bandcamp();
        _bandcampMusic = bandcamp;
        _bandcampSong = bandcamp;

        _youtubeSong = new YouTube();
        _spotify = new Spotify();
        _soundcloud = new Soundcloud();
    }

    public Music GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return _bandcampMusic.GetItem(url);
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
            return _youtubeSong.GetItem(url);
        }

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return _bandcampSong.GetItem(url);
        }

        if (url.Contains(Soundcloud.UrlIdentifier))
        {
            return _soundcloud.GetItem(url);
        }

        return new Song();
    }
}
