namespace Repositories;

public class MusicRepository
{
    public static Music GetAlbumInfo(string url)
    {
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
