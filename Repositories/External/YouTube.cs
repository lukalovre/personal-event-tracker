using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using AvaloniaApplication1.ViewModels.Extensions;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class YouTube : IExternal<TVShow>, IExternal<Song>
{
    public static string UrlIdentifier => "youtube.com";

    public TVShow GetItem(string url)
    {
        using var client = new WebClient();
        var content = client.DownloadData(url);
        using var stream = new MemoryStream(content);
        string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(result);

        var title = GetTitle(htmlDocument);

        var handle = url.TrimStart("https://www.youtube.com/");

        var posterNode = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@property, 'og:image')]"
        );
        var imageLink = posterNode.GetAttributeValue("content", string.Empty).Trim();

        // var destinationFile = Path.Combine(Paths.Posters, $"{handle}");
        // Web.DownloadPNG(imageLink, destinationFile);

        return new TVShow
        {
            Title = title,
            Imdb = handle,
            Year = DateTime.Now.Year,
            Runtime = 10
        };
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var node = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@property, 'og:title')]"
        );

        return node.GetAttributeValue("content", string.Empty).Trim();
    }

    Song IExternal<Song>.GetItem(string url)
    {
        using var client = new WebClient();
        var content = client.DownloadData(url);
        using var stream = new MemoryStream(content);
        string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(result);
        var node = htmlDocument.DocumentNode.SelectSingleNode("//title");
        var videoTitle = node.InnerHtml.Trim();

        var artist = GetArtist(videoTitle);
        var title = GetTitle(videoTitle);

        if (title == "YouTube")
        {
            title = artist;

            var channelNameNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[contains(@property, 'og:video:tag')]");
            artist = channelNameNode.GetAttributeValue("content", string.Empty).Trim();
        }

        var link = GetUrl(url);
        var year = GetYear(htmlDocument);
        int runtime = GetRuntime(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        var destinationFile = Paths.GetTempPath<Song>();
        HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new Song
        {
            Artist = WebUtility.HtmlDecode(artist),
            Title = WebUtility.HtmlDecode(title),
            Link = link,
            Year = year,
            Runtime = runtime
        };
    }

    private static int GetRuntime(HtmlDocument htmlDocument)
    {
        var runtimeNode = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@itemprop, 'duration')]"
        );
        var runtimeText = runtimeNode.GetAttributeValue("content", string.Empty).Trim();
        var runtimeSplit = runtimeText.TrimStart("PT").TrimEnd("S").Split('M');
        var runtimeMinutes = int.Parse(runtimeSplit.FirstOrDefault());
        var runtimeSeconds = int.Parse(runtimeSplit.LastOrDefault());
        var runtime = runtimeMinutes + (runtimeSeconds > 30 ? 1 : 0);
        return runtime;
    }

    private static string GetUrl(string url)
    {
        url = url.Split(new string[] { "&list=" }, StringSplitOptions.None).FirstOrDefault();
        return url;
    }

    private static int GetYear(HtmlDocument htmlDocument)
    {
        var yearNode = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@itemprop, 'datePublished')]"
        );
        var yearText = yearNode.GetAttributeValue("content", string.Empty).Trim();
        var year = int.Parse(yearText.Split('-').FirstOrDefault());
        return year;
    }

    private static string GetImageUrl(HtmlDocument htmlDocument)
    {
        var posterNode = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@property, 'og:image')]"
        );
        return posterNode.GetAttributeValue("content", string.Empty).Trim();
    }

    private static string GetTitle(string videoTitle)
    {
        var toRemoveList = new List<string>{
            "(Original Mix)",
            "(Music video)",
            "(Official Music Video)",
            "(Official Audio)",
            "(Official Video)",
            "[HD]",
            "[Official Video]",
            "|HQ|",
            "(Audio)"};

        videoTitle = videoTitle.Split(" - ")[1];

        foreach (var item in toRemoveList)
        {
            videoTitle = videoTitle.Replace(item, string.Empty, ignoreCase: true, CultureInfo.InvariantCulture);
        }

        return videoTitle.Replace("  ", " ").Trim();
    }

    private static string GetArtist(string videoTitle)
    {
        return videoTitle.Split(" - ")[0].Trim();
    }

}
