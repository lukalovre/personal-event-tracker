using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AvaloniaApplication1.Repositories.External;

public class Imdb
{
    private const string API_KEY_FILE_NAME = "omdbapi_key.txt";

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

    // public static ImdbData GetDataFromAPI(string imdb, bool savePoster = true)
    // {
    //     var client = new HttpClient { BaseAddress = new Uri("http://www.omdbapi.com/") };

    //     client.DefaultRequestHeaders.Accept.Add(
    //         new MediaTypeWithQualityHeaderValue("application/json")
    //     );

    //     var apiKey = File.ReadAllText(@"..\..\..\Keys\");

    //     var response = client.GetAsync($"?i={imdb}&apikey={apiKey}").Result;
    //     var imdbData = response.Content.ReadAsAsync<ImdbData>().Result;

    //     client.Dispose();

    //     if (savePoster)
    //     {
    //         SavePoster(imdbData);
    //     }

    //     return imdbData;
    // }

    // public static TVShow GetTVShow(string url)
    // 	{
    // 		string inputImdb = Imdb.GetImdbIDFromUrl(url);

    // 		if (!string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(inputImdb))
    // 		{
    // 			return GetYoutubeChannel(url);
    // 		}

    // 		var imdbData = Imdb.GetDataFromAPI(inputImdb);

    // 		var runtime = 0;

    // 		try
    // 		{
    // 			runtime = imdbData.Runtime == @"\N" || imdbData.Runtime == @"N/A" ? 0 : int.Parse(imdbData.Runtime.TrimEnd(" min".ToArray()));
    // 		}
    // 		catch
    // 		{
    // 		}

    // 		return new TVShow
    // 		{
    // 			Title = imdbData.Title,
    // 			Runtime = runtime,
    // 			Year = int.Parse(imdbData.Year.Split('–').FirstOrDefault()),
    // 			Imdb = imdbData.imdbID,
    // 			Actors = imdbData.Actors,
    // 			Country = imdbData.Country,
    // 			Director = imdbData.Director,
    // 			Genre = imdbData.Genre,
    // 			Language = imdbData.Language,
    // 			Plot = imdbData.Plot,
    // 			Type = imdbData.Type,
    // 			Writer = imdbData.Writer
    // 		};
    // 	}

    // private static TVShow GetYoutubeChannel(string url)
    // {
    // 	var youtubeData = Links.GetYouTubeChannelNameData(url);

    // 	return new TVShow
    // 	{
    // 		Title = youtubeData.Title,
    // 		Imdb = youtubeData.ID,
    // 		Year = DateTime.Now.Year,
    // 		Runtime = 10
    // 	};
    // }

    private static void SavePoster(ImdbData imdbData)
    {
        // var destinationFile = Path.Combine(Paths.Posters, $"{imdbData.imdbID}");
        // Web.DownloadPNG(imdbData.Poster, destinationFile);
    }

    // public static void OpenLink(IImdb imdb)
    // {
    //     var hyperlink = $"https://www.imdb.com/title/{imdb.Imdb}/";
    //     Web.OpenLink(hyperlink);
    // }

    public static string GetImdbIDFromUrl(string url)
    {
        return url.Split('/').FirstOrDefault(i => i.StartsWith("tt"));
    }

    // 	public static void OpenHyperlink(Movie movie)
    // {
    // 	var hyperlink = $"https://www.imdb.com/title/{movie.Imdb}";
    // 	Web.OpenLink(hyperlink);
    // }
}
