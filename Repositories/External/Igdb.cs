using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IGDB;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Igdb : IExternal<Game>
{
    private const string API_KEY_FILE_NAME = "igdb_keys.txt";
    public static string UrlIdentifier => "igdb.com";

    public async Task<Game> GetItem(string igdbUrl)
    {
        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var lines = File.ReadAllLines(keyFilePath);

        if (lines.Length == 0)
        {
            // Api keys missing.
            // return;
        }

        var clientId = lines[0];
        var clientSecret = lines[1];

        var client = new IGDBClient(clientId, clientSecret);

        var games = await client.QueryAsync<IGDB.Models.Game>(
            IGDBClient.Endpoints.Games,
             $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where url = \"{igdbUrl.Trim()}\";");

        var game = games.SingleOrDefault();

        var imageId = game.Cover.Value.ImageId;
        var coverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";

        var destinationFile = Paths.GetTempPath<Game>();
        HtmlHelper.DownloadPNG(coverUrl, destinationFile);

        return new Game
        {
            ExternalID = (int)game.Id.Value,
            Title = game.Name,
            Year = game.FirstReleaseDate.Value.Year
        };
    }

    // public async Task<string> GetUrlFromAPIAsync(int igdbID)
    // {
    //     var client = GetClient();

    //     var games = await client.QueryAsync<IGDB.Models.Game>(
    //         IGDBClient.Endpoints.Games,
    //         $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where id = {igdbID};"
    //     );
    //     var game = games.FirstOrDefault();

    //     return game.Url;
    // }
}
