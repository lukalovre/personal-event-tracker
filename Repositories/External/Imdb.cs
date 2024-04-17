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

public class Imdb : IExternal<Movie>, IExternal<TVShow>, IExternal<Standup>
{
    private const string API_KEY_FILE_NAME = "omdbapi_key.txt";

    public static string UrlIdentifier => "imdb.com";

    async Task<Movie> IExternal<Movie>.GetItem(string url)
    {
        var item = await GetImdbItem<Movie>(url);

        return new Movie
        {
            Title = item.Title,
            Runtime = item.Runtime,
            Year = item.Year,
            ExternalID = item.ExternalID,
            Actors = item.Actors,
            Country = item.Country,
            Director = item.Director,
            Ganre = item.Genre,
            Language = item.Language,
            Plot = item.Plot,
            Type = item.Type,
            Writer = item.Writer
        };
    }

    public async Task<TVShow> GetItem(string url)
    {
        var item = await GetImdbItem<TVShow>(url);

        return new TVShow
        {
            Title = item.Title,
            Runtime = item.Runtime,
            Year = item.Runtime,
            ExternalID = item.ExternalID,
            Actors = item.Actors,
            Country = item.Country,
            Director = item.Director,
            Genre = item.Genre,
            Language = item.Language,
            Plot = item.Plot,
            Type = item.Type,
            Writer = item.Writer
        };
    }

    async Task<Standup> IExternal<Standup>.GetItem(string url)
    {
        var imdbData = await GetDataFromAPI<Standup>(url);

        var runtime = GetRuntime(imdbData.Runtime);
        int year = GetYear(imdbData.Year);

        var split = imdbData.Title.Split(':');

        string performer;
        string title;

        if (split.Count() > 1)
        {
            performer = split[0].Trim();
            title = split[1].Trim();
        }
        else
        {
            performer = imdbData.Writer;
            title = imdbData.Title;
        }

        return new Standup
        {
            Performer = performer,
            Title = title,
            Link = url,
            Country = imdbData.Country,
            Director = imdbData.Director,
            Writer = imdbData.Writer,
            Plot = imdbData.Plot,
            Runtime = runtime,
            Year = year
        };
    }

    private static async Task<ImdbItem> GetImdbItem<T>(string url) where T : IItem
    {
        var imdbData = await GetDataFromAPI<T>(url);

        var imdbItem = new ImdbItem(
        imdbData.Title,
        GetRuntime(imdbData.Runtime),
        GetYear(imdbData.Year),
        imdbData.imdbID,
        imdbData.Actors,
        imdbData.Country,
        imdbData.Director,
        imdbData.Genre,
        imdbData.Language,
        imdbData.Plot,
        imdbData.Type,
        imdbData.Writer);
        return imdbItem;
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
