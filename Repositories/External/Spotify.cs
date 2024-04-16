using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using Repositories;
using SpotifyAPI.Web;

namespace AvaloniaApplication1.Repositories.External;

public class Spotify : IExternal<Music>
{
    private const string API_KEY_FILE_NAME = "spotify_key.txt";

    public static string UrlIdentifier => "spotify.com";

    public async Task<Music> GetItem(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return new Music();
        }

        var albumID = url
        ?.Split('/')
        ?.LastOrDefault()
        ?? null;

        if (string.IsNullOrWhiteSpace(albumID))
        {
            return new Music();
        }

        var client = GetClient();

        if (client == null)
        {
            return new Music();
        }

        var album = await client.Albums.Get(albumID);

        if (album is null)
        {
            return new Music();
        }

        var imageUrl = album
            ?.Images
            ?.FirstOrDefault()
            ?.Url
            ?? string.Empty;
        var destinationFile = Paths.GetTempPath<Music>();
        await HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        var artistArray = album
        ?.Artists
        ?.Select(x => x.Name)
        ?.ToArray()
        ?? [];
        var artist = string.Join(" and ", artistArray);

        var title = album?.Name ?? string.Empty;

        var releaseDate = album?.ReleaseDate ?? DateTime.Now.Year.ToString();

        int year = 0;

        if (releaseDate?.Length == "1996"?.Length)
        {
            year = Convert.ToInt32(releaseDate);
        }
        else
        {
            if (DateTime.TryParse(releaseDate, out var parsedDate))
            {
                year = parsedDate.Year;
            }
        }

        var runtime = album
        ?.Tracks
        ?.Items
        ?.Sum(o => o.DurationMs) / 1000 / 60
        ?? 0;

        return new Music
        {
            Artist = artist,
            Title = title,
            Year = year,
            _1001 = false,
            Runtime = runtime,
            ExternalID = url ?? string.Empty
        };
    }

    private SpotifyClient GetClient()
    {
        var config = SpotifyClientConfig.CreateDefault();

        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var lines = File.ReadAllLines(keyFilePath);

        if (lines.Length == 0)
        {
            // Api keys missing.
            return null!;
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
