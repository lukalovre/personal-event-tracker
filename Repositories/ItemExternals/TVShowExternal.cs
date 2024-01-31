using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class TVShowExternal : IExternal<TVShow>
{
    private readonly Imdb _imdb;
    private readonly YouTube _youtube;

    public TVShowExternal()
    {
        _imdb = new Imdb();
        _youtube = new YouTube();
    }

    public TVShow GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            return _imdb.GetItem(url);
        }

        if (url.Contains(YouTube.UrlIdentifier))
        {
            return _youtube.GetItem(url);
        }

        return new TVShow();
    }

    // public static TVShow GetTVShow(string url)
    // {
    //     string inputImdb = Imdb.GetImdbIDFromUrl(url);

    //     if (!string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(inputImdb))
    //     {
    //         return GetYoutubeChannel(url);
    //     }

    //     var imdbData = Imdb.GetDataFromAPI(inputImdb);

    //     var runtime = 0;

    //     try
    //     {
    //         runtime =
    //             imdbData.Runtime == @"\N" || imdbData.Runtime == @"N/A"
    //                 ? 0
    //                 : int.Parse(imdbData.Runtime.TrimEnd(" min".ToArray()));
    //     }
    //     catch { }

    //     return new TVShow
    //     {
    //         Title = imdbData.Title,
    //         Runtime = runtime,
    //         Year = int.Parse(imdbData.Year.Split('–').FirstOrDefault()),
    //         Imdb = imdbData.imdbID,
    //         Actors = imdbData.Actors,
    //         Country = imdbData.Country,
    //         Director = imdbData.Director,
    //         Genre = imdbData.Genre,
    //         Language = imdbData.Language,
    //         Plot = imdbData.Plot,
    //         Type = imdbData.Type,
    //         Writer = imdbData.Writer
    //     };
    // }
}
