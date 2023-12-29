using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Imdb : IExternal<Movie>, IExternal<TVShow>
{
    private const string API_KEY_FILE_NAME = "omdbapi_key.txt";

    public static string UrlIdentifier => "imdb.com";

    Movie IExternal<Movie>.GetItem(string url)
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
            Imdb = imdbData.imdbID,
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

    public TVShow GetItem(string url)
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
            Imdb = imdbData.imdbID,
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

        return runtimeString == @"\N" || runtimeString == @"N/A"
            ? 0
            : int.Parse(runtimeString.TrimEnd(" min".ToArray()));
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

    // 	public static void OpenHyperlink(Movie movie)
    // {
    // 	var hyperlink = $"https://www.imdb.com/title/{movie.Imdb}";
    // 	Web.OpenLink(hyperlink);
    // }

    public class ImdbData
    {
        public string Actors { get; set; }
        public string Awards { get; set; }
        public string BoxOffice { get; set; }
        public string Country { get; set; }
        public string Director { get; set; }
        public string DVD { get; set; }
        public string Genre { get; set; }
        public string imdbID { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string Language { get; set; }
        public string Metascore { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }
        public string Production { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Response { get; set; }
        public string Runtime { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Website { get; set; }
        public string Writer { get; set; }
        public string Year { get; set; }
    }
}
