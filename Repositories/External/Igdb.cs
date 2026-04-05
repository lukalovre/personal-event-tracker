using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventTracker.Models;
using IGDB;
using Repositories;

namespace EventTracker.Repositories.External;

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

        var companyName = await GetDeveloperName(client, game);

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
             companyName,
             year
        );
    }

    public static async Task<IgdbItem> GetItemById(string igdbUrl)
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
             $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where id = {igdbUrl.Trim()};");

        var game = games.SingleOrDefault() ?? new IGDB.Models.Game();

        if (game is null)
        {
            return null;
        }

        string companyName = await GetDeveloperName(client, game);

        var externalID = (int)(game?.Id ?? 0);
        var title = game?.Name ?? string.Empty;
        var year = game?.FirstReleaseDate?.Year ?? 0;

        return new IgdbItem
        (
             externalID,
             title.Trim(),
             companyName,
             year
        );
    }

    private static async Task<string> GetDeveloperName(IGDBClient client, IGDB.Models.Game game)
    {
        var developerList = new List<string>();

        if (game.InvolvedCompanies is null)
        {
            return string.Empty;
        }

        foreach (var involvedCompanieId in game.InvolvedCompanies.Ids)
        {
            var involvedCompany = await client.QueryAsync<IGDB.Models.InvolvedCompany>(
                        IGDBClient.Endpoints.InvolvedCompanies,
                         $"fields company, developer; where id = {involvedCompanieId};");

            var developerCompanyIdList = involvedCompany
                   .Where(o => o.Developer.HasValue && o.Developer.Value)
                   .Select(o => o.Company.Id);

            if (!developerCompanyIdList.Any())
            {
                continue;
            }

            var company = await client.QueryAsync<IGDB.Models.Company>(
                IGDBClient.Endpoints.Companies,
                 $"fields name; where id = {developerCompanyIdList.First()};");

            var companyName = company[0].Name;

            developerList.Add(companyName);
        }

        developerList = developerList.Distinct().ToList();
        return string.Join("; ", developerList);
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
