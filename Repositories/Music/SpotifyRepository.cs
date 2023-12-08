using System;
using System.IO;
using System.Linq;
using Repositories;

namespace Repositories;

public class SpotifyRepository
{
    public static Music GetAlbumInfoSpotify(string albumID)
    {
        albumID = albumID.Split('/').LastOrDefault();

        var spotify = GetSpotifyClient();

        var album = spotify.Albums.Get(albumID).Result;

        var destinationFile = Paths.TempAlbumCover;

        File.Delete($"{destinationFile}.png");

        Web.DownloadPNG(album.Images.FirstOrDefault().Url, destinationFile);

        return new Music
        {
            Artist = string.Join(" and ", album.Artists.Select(x => x.Name).ToArray()),
            Title = album.Name,
            Year =
                album.ReleaseDate.Length == "1996".Length
                    ? int.Parse(album.ReleaseDate)
                    : DateTime.Parse(album.ReleaseDate).Year,
            _1001 = false,
            Runtime = album.Tracks.Items.Sum(o => o.DurationMs) / 1000 / 60,
            SpotifyID = album.Id
        };
    }

    private static SpotifyClient GetSpotifyClient()
    {
        var config = SpotifyClientConfig.CreateDefault();

        var lines = File.ReadAllLines(@"..\..\..\Keys\spotify_key.txt");

        var clientId = lines[0];
        var clientSecret = lines[1];

        var request = new ClientCredentialsRequest(clientId, clientSecret);
        var response = new OAuthClient(config).RequestToken(request).Result;

        return new SpotifyClient(config.WithToken(response.AccessToken));
    }
}
