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
        string inputImdb = GetImdbID(url);

        var imdbData = GetDataFromAPI<Movie>(inputImdb);

        var runtime = GetRuntime(imdbData.Runtime);
        int year = GetYear(imdbData.Year);

        return new Movie
        {
            Title = imdbData.Title,
            Runtime = runtime,
            Year = year,
            ExternalID = imdbData.imdbID,
            Actors = imdbData.Actors,
            Country = imdbData.Country,
            Director = imdbData.Director,
            Ganre = imdbData.Genre,
            Language = imdbData.Language,
            Plot = imdbData.Plot,
            Type = imdbData.Type,
            Writer = imdbData.Writer
        };
    }

    public async Task<TVShow> GetItem(string url)
    {
        string inputImdb = GetImdbID(url);

        var imdbData = GetDataFromAPI<TVShow>(inputImdb);

        var runtime = GetRuntime(imdbData.Runtime);
        int year = GetYear(imdbData.Year);

        return new TVShow
        {
            Title = imdbData.Title,
            Runtime = runtime,
            Year = year,
            ExternalID = imdbData.imdbID,
            Actors = imdbData.Actors,
            Country = imdbData.Country,
            Director = imdbData.Director,
            Genre = imdbData.Genre,
            Language = imdbData.Language,
            Plot = imdbData.Plot,
            Type = imdbData.Type,
            Writer = imdbData.Writer
        };
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

    public static ImdbData GetDataFromAPI<T>(string imdbID)
        where T : IItem
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://www.omdbapi.com/") };

        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        var keyFilePath = Paths.GetAPIKeyFilePath(API_KEY_FILE_NAME);
        var apiKey = File.ReadAllText(keyFilePath);

        var response = client.GetAsync($"?i={imdbID}&apikey={apiKey}").Result;
        var imdbData = response.Content.ReadFromJsonAsync<ImdbData>().Result;

        var destinationFile = Paths.GetTempPath<T>();
        HtmlHelper.DownloadPNG(imdbData.Poster, destinationFile);

        return imdbData;
    }

    // public static void OpenLink(IImdb imdb)
    // {
    //     var hyperlink = $"https://www.imdb.com/title/{imdb.Imdb}/";
    //     Web.OpenLink(hyperlink);
    // }

    private static string GetImdbID(string url)
    {
        return url.Split('/').FirstOrDefault(i => i.StartsWith("tt"));
    }

    async Task<Standup> IExternal<Standup>.GetItem(string url)
    {
        string inputImdb = GetImdbID(url);

        var imdbData = GetDataFromAPI<Standup>(inputImdb);

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

    // 	public static void OpenHyperlink(Movie movie)
    // {
    // 	var hyperlink = $"https://www.imdb.com/title/{movie.Imdb}";
    // 	Web.OpenLink(hyperlink);
    // }

    public class ImdbData
    {
        public string Actors { get; set; } = string.Empty;
        public string Awards { get; set; } = string.Empty;
        public string BoxOffice { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string DVD { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string imdbID { get; set; } = string.Empty;
        public string imdbRating { get; set; } = string.Empty;
        public string imdbVotes { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Metascore { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Production { get; set; } = string.Empty;
        public string Rated { get; set; } = string.Empty;
        public string Released { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Runtime { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Writer { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }
}
