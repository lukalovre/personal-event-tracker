using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class YouTube : IExternal<TVShow>, IExternal<Song>, IExternal<Music>, IExternal<Clip>
{
    public static string UrlIdentifier => "youtube.com";

    public async Task<TVShow> GetItem(string url)
    {
        var item = await GetYoutubeItem<TVShow>(url);

        return new TVShow
        {
            Title = item.Title,
            ExternalID = item.Link,
            Year = item.Year,
            Runtime = item.Runtime
        };
    }

    async Task<Clip> IExternal<Clip>.GetItem(string url)
    {
        var item = await GetYoutubeItem<Clip>(url);

        return new Clip
        {
            Title = item.Title,
            ExternalID = item.Link,
            Year = item.Year,
            Runtime = item.Runtime,
            Author = item.Author
        };
    }

    async Task<Song> IExternal<Song>.GetItem(string url)
    {
        var item = await GetYoutubeItem<Song>(url);

        return new Song
        {
            Artist = item.Artist,
            Title = item.MusicTitle,
            Link = item.Link,
            Year = item.Year,
            Runtime = item.Runtime
        };
    }

    async Task<Music> IExternal<Music>.GetItem(string url)
    {
        var item = await GetYoutubeItem<Music>(url);

        return new Music
        {
            Title = item.MusicTitle,
            Artist = item.Artist,
            Year = item.Year,
            Runtime = item.Runtime,
            ExternalID = item.Link
        };
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var title = htmlDocument
        ?.DocumentNode
        ?.SelectSingleNode("//meta[contains(@property, 'og:title')]")
        ?.GetAttributeValue("content", string.Empty)
        .Trim()
        ?? string.Empty;

        return WebUtility.HtmlDecode(title) ?? string.Empty;
    }

    public static async Task<YoutubeItem> GetYoutubeItem<T>(string url) where T : IItem
    {
        var htmlDocument = await HtmlHelper.DownloadWebpage(url);

        if (htmlDocument is null)
        {
            return default!;
        }

        var node = htmlDocument.DocumentNode.SelectSingleNode("//title");
        var videoTitle = node.InnerHtml.Trim();

        var artist = GetArtist(videoTitle);
        var musicTitle = GetMusicTitle(videoTitle);

        if (musicTitle == "YouTube")
        {
            musicTitle = artist;
            artist = htmlDocument
            ?.DocumentNode
            ?.SelectSingleNode("//meta[contains(@property, 'og:video:tag')]")
            ?.GetAttributeValue("content", string.Empty)
            ?.Trim()
            ?? string.Empty;
        }

        if (htmlDocument is null)
        {
            return default!;
        }

        var link = GetUrl(url);
        var year = GetYear(htmlDocument);
        int runtime = GetRuntime(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        var title = GetTitle(htmlDocument);
        var author = GetChannelName(htmlDocument);

        var destinationFile = Paths.GetTempPath<T>();
        await HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        artist = WebUtility.HtmlDecode(artist) ?? string.Empty;
        musicTitle = WebUtility.HtmlDecode(musicTitle);

        return new YoutubeItem(
            musicTitle,
            artist,
            year,
            runtime,
            link,
            title,
            author);
    }

    private static int GetRuntime(HtmlDocument htmlDocument)
    {
        var runtimeNode = htmlDocument.DocumentNode.SelectSingleNode(
            "//meta[contains(@itemprop, 'duration')]"
        );
        var runtimeText = runtimeNode.GetAttributeValue("content", string.Empty).Trim();
        var runtimeSplit = runtimeText.TrimStart("PT").TrimEnd("S").Split('M');
        var runtimeMinutes = Convert.ToInt32(runtimeSplit?.FirstOrDefault() ?? "0");
        var runtimeSeconds = Convert.ToInt32(runtimeSplit?.LastOrDefault() ?? "0");
        var runtime = runtimeMinutes + (runtimeSeconds > 30 ? 1 : 0);
        return runtime;
    }

    private static string GetUrl(string url)
    {
        return url
        ?.Split(new string[] { "&list=" }, StringSplitOptions.None)
        ?.FirstOrDefault()
        ?? string.Empty;
    }

    private static int GetYear(HtmlDocument htmlDocument)
    {
        var yearNode = htmlDocument
        ?.DocumentNode
        ?.SelectSingleNode("//meta[contains(@itemprop, 'datePublished')]");

        if (yearNode == null)
        {
            return 0;
        }

        var yearText = yearNode
        ?.GetAttributeValue("content", string.Empty)
        ?.Trim()
        ?.Split('-')
        ?.FirstOrDefault()
        ?? string.Empty;

        return Convert.ToInt32(yearText);
    }

    private static string GetImageUrl(HtmlDocument htmlDocument)
    {
        return htmlDocument
        ?.DocumentNode
        ?.SelectSingleNode("//meta[contains(@property, 'og:image')]")
        ?.GetAttributeValue("content", string.Empty)
        ?.Trim()
        ?? string.Empty;
    }

    private static string GetMusicTitle(string videoTitle)
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
            "(Audio)",
            "[FULL ALBUM]",
            "HIGH QUALITY"};

        var split = videoTitle.Split(" - ");

        if (split.Length <= 1)
        {
            return videoTitle;
        }

        videoTitle = split[1];

        foreach (var item in toRemoveList)
        {
            videoTitle = videoTitle.Replace(item, string.Empty, ignoreCase: true, CultureInfo.InvariantCulture);
        }

        return videoTitle.Replace("  ", " ").Trim();
    }

    private static string GetArtist(string videoTitle)
    {
        return videoTitle
        ?.Split(" - ")
        .FirstOrDefault()
        ?.Trim()
        ?? string.Empty;
    }

    private static string GetChannelName(HtmlDocument htmlDocument)
    {
        var text = htmlDocument.ParsedText;
        var match = Regex.Match(text, "\"author\":\"(.*?)\"");

        if (match.Success)
        {
            var author = match
            ?.Groups[1]
            ?.Value
            ?? string.Empty;

            return WebUtility.HtmlDecode(author);
        }

        return string.Empty;
    }
}
