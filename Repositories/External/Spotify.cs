using System;
using System.IO;
using System.Linq;
using Repositories;
using SpotifyAPI.Web;

namespace AvaloniaApplication1.Repositories.External;

public class Spotify : IExternal<Music>
{
    private const string API_KEY_FILE_NAME = "spotify_key.txt";
    public static string UrlIdentifier => "spotify.com";

    public Music GetItem(string url)
    {
        var albumID = url.Split('/').LastOrDefault();

        var spotifyClient = GetClient();

        if (spotifyClient == null)
        {
            return new Music();
        }

        var album = spotifyClient.Albums.Get(albumID).Result;

        var destinationFile = Paths.GetTempPath<Music>();
        HtmlHelper.DownloadPNG(album.Images.FirstOrDefault()?.Url, destinationFile);

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
            SpotifyID = url
        };
    }

    private static SpotifyClient GetClient()
    {
        var config = SpotifyClientConfig.CreateDefault();

        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var lines = File.ReadAllLines(keyFilePath);

        if (!lines.Any())
        {
            // Api keys missing.
            return null;
        }

        var clientId = lines[0];
        var clientSecret = lines[1];

        var request = new ClientCredentialsRequest(clientId, clientSecret);
        var response = new OAuthClient(config).RequestToken(request).Result;

        return new SpotifyClient(config.WithToken(response.AccessToken));
    }

    #region FindAlbum
    //public static void FindAlbum(Music music)
    //{
    //    var destinationFile = Path.Combine(Paths.Albums, $"{music.ItemID}.png");

    //    if (File.Exists(destinationFile) && music.SpotifyID != null)
    //    {
    //        return;
    //    }

    //    var spotify = GetSpotifyClient();

    //    var albumSearchList = spotify.Search.Item(new SearchRequest(SearchRequest.Types.Album, music.Title)).Result;

    //    SimpleAlbum foundAlbum = null;

    //    foreach (var albumInfo in albumSearchList.Albums.Items)
    //    {
    //        DateTime.TryParse(albumInfo.ReleaseDate, out var date);

    //        if (albumInfo.Artists.Any(o => o.Name == music.Artist)
    //            &&
    //            (albumInfo.ReleaseDate == music.Year.ToString()
    //            || date.Year == music.Year))
    //        {
    //            foundAlbum = albumInfo;
    //            break;
    //        }
    //    }

    //    if (foundAlbum != null)
    //    {
    //        Web.Download(foundAlbum.Images.FirstOrDefault().Url, destinationFile);

    //        music.SpotifyID = foundAlbum.Id;

    //        using (var sqlConnection = new SqlConnection(Resources.MainConnectionString))
    //        {
    //            sqlConnection.Open();
    //            sqlConnection.Update(music);
    //        }
    //    }
    //    else
    //    {
    //    }
    //}
    #endregion
}
