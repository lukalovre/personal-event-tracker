using System;
using System.IO;
using System.Linq;
using System.Net;
using AvaloniaApplication1.ViewModels.Extensions;
using HtmlAgilityPack;

public class YouTubeRepository
{
    public class YoutubeChannelData
    {
        public string Title { get; set; }
        public string ID { get; set; }
    }

    public class YoutubeSongData
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int Year { get; set; }
        public int Runtime { get; set; }
    }

    public static TVShow GetTVShow(string url)
    {
        string inputImdb = Imdb.GetImdbIDFromUrl(url);

        if (!string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(inputImdb))
        {
            return GetYoutubeChannel(url);
        }

        var imdbData = Imdb.GetDataFromAPI(inputImdb);

        var runtime = 0;

        try
        {
            runtime =
                imdbData.Runtime == @"\N" || imdbData.Runtime == @"N/A"
                    ? 0
                    : int.Parse(imdbData.Runtime.TrimEnd(" min".ToArray()));
        }
        catch { }

        return new TVShow
        {
            Title = imdbData.Title,
            Runtime = runtime,
            Year = int.Parse(imdbData.Year.Split('–').FirstOrDefault()),
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

    private static TVShow GetYoutubeChannel(string url)
    {
        var youtubeData = Links.GetYouTubeChannelNameData(url);

        return new TVShow
        {
            Title = youtubeData.Title,
            Imdb = youtubeData.ID,
            Year = DateTime.Now.Year,
            Runtime = 10
        };
    }

    public static YoutubeChannelData GetYouTubeChannelNameData(string url)
    {
        using (var client = new WebClient())
        {
            var content = client.DownloadData(url);
            using (var stream = new MemoryStream(content))
            {
                string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(result);
                var node = htmlDocument.DocumentNode.SelectSingleNode(
                    "//meta[contains(@property, 'og:title')]"
                );
                var title = node.GetAttributeValue("content", string.Empty).Trim();

                if (node == null || string.IsNullOrWhiteSpace(title))
                {
                    return null;
                }

                var handle = url.TrimStart("https://www.youtube.com/");

                var posterNode = htmlDocument.DocumentNode.SelectSingleNode(
                    "//meta[contains(@property, 'og:image')]"
                );
                var imageLink = posterNode.GetAttributeValue("content", string.Empty).Trim();

                // var destinationFile = Path.Combine(Paths.Posters, $"{handle}");
                // Web.DownloadPNG(imageLink, destinationFile);

                return new YoutubeChannelData { Title = title, ID = handle };
            }
        }
    }

    public static YoutubeSongData GetYouTubeSongData(string url)
    {
        using (var client = new WebClient())
        {
            var content = client.DownloadData(url);
            using (var stream = new MemoryStream(content))
            {
                string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(result);
                var node = htmlDocument.DocumentNode.SelectSingleNode("//title");

                var videoTitle = node.InnerHtml.Trim();

                var artist = videoTitle.Split('-')[0].Trim();
                var title = videoTitle.Split('-')[1]
                    .Trim()
                    .TrimEnd("(Original Mix)")
                    .TrimEnd("(Music video)")
                    .TrimEnd("(Official Music Video)")
                    .Trim();

                if (node == null || string.IsNullOrWhiteSpace(title))
                {
                    return null;
                }

                var handle = url.TrimStart("https://www.youtube.com/watch?v=")
                    .Split(new string[] { "&list=" }, StringSplitOptions.None)
                    .FirstOrDefault();

                var posterNode = htmlDocument.DocumentNode.SelectSingleNode(
                    "//meta[contains(@property, 'og:image')]"
                );
                var imageLink = posterNode.GetAttributeValue("content", string.Empty).Trim();

                // var destinationFile = Path.Combine(Paths.SongCovers, $"{handle}");
                // Web.DownloadPNG(imageLink, destinationFile);

                var yearNode = htmlDocument.DocumentNode.SelectSingleNode(
                    "//meta[contains(@itemprop, 'datePublished')]"
                );
                var yearText = yearNode.GetAttributeValue("content", string.Empty).Trim();
                var year = int.Parse(yearText.Split('-').FirstOrDefault());

                var runtimeNode = htmlDocument.DocumentNode.SelectSingleNode(
                    "//meta[contains(@itemprop, 'duration')]"
                );
                var runtimeText = runtimeNode.GetAttributeValue("content", string.Empty).Trim();
                var runtimeSplit = runtimeText.TrimStart("PT").TrimEnd("S").Split('M');
                var runtimeMinutes = int.Parse(runtimeSplit.FirstOrDefault());
                var runtimeSeconds = int.Parse(runtimeSplit.LastOrDefault());
                var runtime = runtimeMinutes + (runtimeSeconds > 30 ? 1 : 0);

                return new YoutubeSongData
                {
                    Artist = WebUtility.HtmlDecode(artist),
                    Title = WebUtility.HtmlDecode(title),
                    Link = url,
                    Year = year,
                    Runtime = runtime
                };
            }
        }
    }
}
