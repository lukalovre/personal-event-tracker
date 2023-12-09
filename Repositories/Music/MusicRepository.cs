using System.Linq;

namespace Repositories;

public class MusicRepository
{
    public static Music GetAlbumInfo(string url)
    {
        url = url.Split('?').FirstOrDefault();
        url = url.Trim();

        if (url.Contains(BandcampRepository.UrlIdentifier))
        {
            return BandcampRepository.GetAlbumInfoBandcamp(url);
        }

        if (url.Contains(SpotifyRepository.UrlIdentifier))
        {
            return SpotifyRepository.GetAlbumInfoSpotify(url);
        }

        return null;
    }
}
