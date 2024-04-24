using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using IGDB;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Igdb
{
    private const string API_KEY_FILE_NAME = "igdb_keys.txt";
    public static string UrlIdentifier => "igdb.com";

    public static async Task<IgdbItem> GetItem(string igdbUrl)
    {
        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var lines = File.ReadAllLines(keyFilePath);

        if (lines.Length == 0)
        {
            // Api keys missing.
            return null!;
        }

        var clientId = lines[0];
        var clientSecret = lines[1];

        var client = new IGDBClient(clientId, clientSecret);

        var games = await client.QueryAsync<IGDB.Models.Game>(
            IGDBClient.Endpoints.Games,
             $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where url = \"{igdbUrl.Trim()}\";");

        var game = games.SingleOrDefault() ?? new IGDB.Models.Game();

        var imageId = game.Cover.Value.ImageId;
        var coverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";

        var destinationFile = Paths.GetTempPath<Game>();
        await HtmlHelper.DownloadPNG(coverUrl, destinationFile);

        var externalID = (int)(game?.Id ?? 0);
        var title = game?.Name ?? string.Empty;
        var year = game?.FirstReleaseDate?.Year ?? 0;

        return new IgdbItem
        (
             externalID,
             title.Trim(),
             year
        );
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
