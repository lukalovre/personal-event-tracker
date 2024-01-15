using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
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

        using var client = new WebClient();
        client.Headers.Add("user-agent", "...");
        var content = client.DownloadData(url);
        using var stream = new MemoryStream(content);
        string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(result);

        var title = GetTitle(htmlDocument);
        var id = GetID(htmlDocument);
        var year = GetYear(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        var destinationFile = Paths.GetTempPath<Game>();
        HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new Game
        {
            Igdb = id,
            Title = title,
            Year = year
        };

        // return GetDataFromAPIAsync(url).Result;
    }

    private int GetYear(HtmlDocument htmlDocument)
    {
        var node = htmlDocument.DocumentNode.SelectSingleNode(
          "//title");

        var titleAndYear = node.InnerText.Trim();

        var split1 = titleAndYear.Split('(').Last();
        var split2 = split1.Split(')').First();

        return HtmlHelper.GetYear(split2);
    }

    private int GetID(HtmlDocument htmlDocument)
    {
        var node = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@id, 'pageid')]");

        return int.Parse(node.GetAttributeValue("data-game-id", string.Empty).Trim());
    }

    private string GetImageUrl(HtmlDocument htmlDocument)
    {
        var node = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@property, 'og:image')]");

        return node.GetAttributeValue("content", string.Empty).Trim();
    }

    private string GetTitle(HtmlDocument htmlDocument)
    {
        var node = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@property, 'og:title')]");

        return node.GetAttributeValue("content", string.Empty).Trim();
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
