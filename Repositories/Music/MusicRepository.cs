namespace Repositories;

public class MusicRepository
{
    public static Music GetAlbumInfo(string url)
    {
        if (url.Contains("bandcamp.com"))
        {
            return BandcampRepository.GetAlbumInfoBandcamp(url);
        }

        if (url.Contains("bspotify.com"))
        {
            return SpotifyRepository.GetAlbumInfoSpotify(url);
        }

        return null;
    }
}
