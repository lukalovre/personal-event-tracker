using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Bandcamp : IExternal<Music>, IExternal<Song>
{
    public static string UrlIdentifier => "bandcamp.com";

    public async Task<Music> GetItem(string url)
    {
        var htmlDocument = await HtmlHelper.DownloadWebpage(url);

        var title = GetTitle(htmlDocument);
        var artist = GetArtist(htmlDocument);
        var year = GetYear(htmlDocument);
        var bandcampLink = GetLink(htmlDocument);
        var runtime = GetRuntime(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        var destinationFile = Paths.GetTempPath<Music>();
        HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new Music
        {
            Artist = artist,
            Title = title,
            Year = year,
            _1001 = false,
            Runtime = runtime,
            ExternalID = bandcampLink
        };
    }

    async Task<Song> IExternal<Song>.GetItem(string url)
    {
        var htmlDocument = await HtmlHelper.DownloadWebpage(url);

        var title = GetTitle(htmlDocument);
        var artist = GetArtist(htmlDocument);
        var year = GetYear(htmlDocument);
        var bandcampLink = GetLink(htmlDocument);
        var runtime = GetRuntimeSong(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        string destinationFile = Paths.GetTempPath<Song>();
        HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new Song
        {
            Title = title,
            Artist = artist,
            Year = year,
            Runtime = runtime,
            Link = url
        };
    }

    private static string GetImageUrl(HtmlDocument htmlDocument)
    {
        return htmlDocument
            ?.DocumentNode
            ?.SelectSingleNode("//a[@class='popupImage']")
            ?.Attributes["href"]
            ?.Value
            ?.Trim()
            ?? string.Empty;
    }

    private static int GetRuntime(HtmlDocument htmlDocument)
    {
        var totalHours = 0;
        var totalMinutes = 0;
        var totalSeconds = 0;

        var timeNodes = htmlDocument
            ?.DocumentNode
            ?.SelectNodes("//span[contains(@class, 'time secondaryText')]");

        if (timeNodes == null)
        {
            return 0;
        }

        foreach (var item in timeNodes)
        {
            var timeString = item.InnerText.Trim();
            var split = timeString.Split(':');

            var hours = 0;
            var minutes = 0;
            var seconds = 0;

            if (split.Length == 2)
            {
                minutes = Convert.ToInt32(split[0]);
                seconds = Convert.ToInt32(split[1]);
            }

            if (split.Length == 3)
            {
                hours = Convert.ToInt32(split[0]);
                minutes = Convert.ToInt32(split[1]);
                seconds = Convert.ToInt32(split[2]);
            }

            totalHours += hours;
            totalMinutes += minutes;
            totalSeconds += seconds;
        }

        return totalMinutes
        + (int)Math.Round(totalSeconds / 60f, MidpointRounding.AwayFromZero)
        + totalHours * 60;
    }

    private static int GetRuntimeSong(HtmlDocument htmlDocument)
    {

        var totalHours = 0;
        var totalMinutes = 0;
        var totalSeconds = 0;

        var timeNodes = htmlDocument
            ?.DocumentNode
            ?.SelectNodes("//span[contains(@class, 'time_total')]");

        if (timeNodes == null)
        {
            return 0;
        }

        foreach (var item in timeNodes)
        {
            var timeString = item.InnerText.Trim();
            var split = timeString.Split(':');

            var hours = 0;
            var minutes = 0;
            var seconds = 0;

            if (split.Length == 2)
            {
                minutes = Convert.ToInt32(split[0]);
                seconds = Convert.ToInt32(split[1]);
            }

            if (split.Length == 3)
            {
                hours = Convert.ToInt32(split[0]);
                minutes = Convert.ToInt32(split[1]);
                seconds = Convert.ToInt32(split[2]);
            }

            totalHours += hours;
            totalMinutes += minutes;
            totalSeconds += seconds;
        }

        return totalMinutes
        + (int)Math.Round(totalSeconds / 60f, MidpointRounding.AwayFromZero)
        + totalHours * 60;
    }

    private static string GetLink(HtmlDocument htmlDocument)
    {
        var result = string.Empty;

        try
        {
            result = htmlDocument.DocumentNode
                .SelectSingleNode("//meta[@property='og:url']")
                .Attributes["content"].Value.Trim();
        }
        catch { }

        return result;
    }

    private static int GetYear(HtmlDocument htmlDocument)
    {
        var result = 0;

        try
        {
            result = HtmlHelper.GetYear(
                    htmlDocument.DocumentNode
                        .SelectSingleNode("//div[@class='tralbumData tralbum-credits']")
                        .InnerText.Trim());
        }
        catch { }

        return result;
    }

    private static string GetArtist(HtmlDocument htmlDocument)
    {
        var result = string.Empty;

        try
        {
            result = htmlDocument.DocumentNode
                .SelectSingleNode("//meta[@property='og:title']")
                .Attributes["content"].Value
                .Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault()
                .Trim();
        }
        catch { }

        return WebUtility.HtmlDecode(result);
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var result = string.Empty;

        try
        {
            result = htmlDocument.DocumentNode
                .SelectSingleNode("//meta[@property='og:title']")
                .Attributes["content"].Value
                .Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()
                .Trim();
        }
        catch { }

        return WebUtility.HtmlDecode(result);
    }
}
