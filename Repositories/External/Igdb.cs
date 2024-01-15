using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IGDB;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Igdb : IExternal<Game>
{
    private IGDBClient _client;

    private const string API_KEY_FILE_NAME = "igdb_keys.txt";
    public static string UrlIdentifier => "igdb.com";

    public Game GetItem(string url)
    {
        return GetDataFromAPIAsync(url).Result;
    }

    private IGDBClient GetClient()
    {
        if (_client != null)
        {
            return _client;
        }

        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var lines = File.ReadAllLines(keyFilePath);

        if (lines.Length == 0)
        {
            // Api keys missing.
            return null;
        }

        var clientId = lines[0];
        var clientSecret = lines[1];

        return new IGDBClient(clientId, clientSecret);
    }

    public async Task<Game> GetDataFromAPIAsync(string igdbUrl)
    {
        _client = GetClient();

        if (_client == null)
        {
            return new Game();
        }

        var games = await _client.QueryAsync<IGDB.Models.Game>(IGDBClient.Endpoints.Games,
         $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where url = \"{igdbUrl.Trim()}\";");
        var game = games.FirstOrDefault();

        var imageId = game.Cover.Value.ImageId;
        var coverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";

        var destinationFile = Paths.GetTempPath<Game>();
        HtmlHelper.DownloadPNG(coverUrl, destinationFile);

        return new Game
        {
            Igdb = (int)game.Id.Value,
            Title = game.Name,
            Year = game.FirstReleaseDate.Value.Year
        };
    }

    public async Task<string> GetUrlFromAPIAsync(int igdbID)
    {
        _client = GetClient();

        var games = await _client.QueryAsync<IGDB.Models.Game>(
            IGDBClient.Endpoints.Games,
            $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where id = {igdbID};"
        );
        var game = games.FirstOrDefault();

        return game.Url;
    }
}
