using System.Linq;
using AvaloniaApplication1.Repositories.External;

namespace Repositories;

public class MusicRepository
{
    public static Music GetAlbumInfo(string url)
    {
        url = url.Split('?').FirstOrDefault();
        url = url.Trim();

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            return Bandcamp.GetAlbumInfoBandcamp(url);
        }

        if (url.Contains(Spotify.UrlIdentifier))
        {
            return Spotify.GetAlbumInfoSpotify(url);
        }

        return null;
    }
}
