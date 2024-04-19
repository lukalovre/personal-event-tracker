using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Repositories;
using AvaloniaApplication1.ViewModels.Extensions;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Repositories.External;

public class Imdb
{
    private const string API_KEY_FILE_NAME = "omdbapi_key.txt";

    public static string UrlIdentifier => "imdb.com";

    internal static async Task<ImdbItem> GetImdbItem<T>(string url) where T : IItem
    {
        var imdbData = await GetDataFromAPI<T>(url);

        var split = imdbData.Title.Split(':');

        string standupPerformer;
        string standupTitle;

        if (split.Length > 1)
        {
            standupPerformer = split[0].Trim();
            standupTitle = split[1].Trim();
        }
        else
        {
            standupPerformer = imdbData.Writer;
            standupTitle = imdbData.Title;
        }

        return new ImdbItem(
         imdbData.Title.Trim(),
         GetRuntime(imdbData.Runtime),
         GetYear(imdbData.Year),
         imdbData.imdbID.Trim(),
         imdbData.Actors.Trim(),
         imdbData.Country.Trim(),
         imdbData.Director.Trim(),
         imdbData.Genre.Trim(),
         imdbData.Language.Trim(),
         imdbData.Plot.Trim(),
         imdbData.Type.Trim(),
         imdbData.Writer.Trim(),
         standupPerformer.Trim(),
         standupTitle.Trim());
    }

    private static int GetRuntime(string runtimeString)
    {
        if (string.IsNullOrWhiteSpace(runtimeString))
        {
            return 0;
        }

        var resultString = runtimeString == @"\N" || runtimeString == @"N/A"
                    ? "0"
                    : runtimeString.TrimEnd(" min");

        return int.TryParse(resultString, out var result) ? result : 0;
    }

    private static int GetYear(string yearString)
    {
        if (string.IsNullOrWhiteSpace(yearString))
        {
            return 0;
        }

        if (int.TryParse(yearString.Split('–').FirstOrDefault(), out var year))
        {
            return year;
        }

        return 0;
    }

    public static async Task<ImdbData> GetDataFromAPI<T>(string url) where T : IItem
    {
        var imdbID = GetImdbID(url);

        using var client = new HttpClient { BaseAddress = new Uri("http://www.omdbapi.com/") };

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var apiKey = File.ReadAllText(keyFilePath);

        var response = await client.GetAsync($"?i={imdbID}&apikey={apiKey}");
        var imdbData = await response.Content.ReadFromJsonAsync<ImdbData>();

        if (imdbData is null)
        {
            return default!;
        }

        var destinationFile = Paths.GetTempPath<T>();
        await HtmlHelper.DownloadPNG(imdbData.Poster, destinationFile);

        return imdbData;
    }

    // public static void OpenLink(IImdb imdb)
    // {
    //     var hyperlink = $"https://www.imdb.com/title/{imdb.Imdb}/";
    //     Web.OpenLink(hyperlink);
    // }

    private static string GetImdbID(string url)
    {
        return url
        ?.Split('/')
        ?.FirstOrDefault(i => i.StartsWith("tt"))
        ?? string.Empty;
    }

    // 	public static void OpenHyperlink(Movie movie)
    // {
    // 	var hyperlink = $"https://www.imdb.com/title/{movie.Imdb}";
    // 	Web.OpenLink(hyperlink);
    // }
}
