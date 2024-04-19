using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Bandcamp
{
    public static string UrlIdentifier => "bandcamp.com";

    internal static async Task<BandcampItem> GetBandcampItem<T>(string url)
    {
        var htmlDocument = await HtmlHelper.DownloadWebpage(url);

        var title = GetTitle(htmlDocument);
        var artist = GetArtist(htmlDocument);
        var year = GetYear(htmlDocument);
        var bandcampLink = GetLink(htmlDocument);
        var runtime = GetRuntime(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        var destinationFile = Paths.GetTempPath<T>();
        await HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new BandcampItem(
            title.Trim(),
            artist.Trim(),
            year,
            runtime,
            imageUrl.Trim(),
            bandcampLink.Trim());
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
        return htmlDocument
            ?.DocumentNode
            ?.SelectSingleNode("//meta[@property='og:url']")
            ?.Attributes["content"]
            ?.Value
            ?.Trim()
            ?? string.Empty;
    }

    private static int GetYear(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
                    ?.DocumentNode
                    ?.SelectSingleNode("//div[@class='tralbumData tralbum-credits']")
                    ?.InnerText
                    ?.Trim()
                    ?? string.Empty;

        return HtmlHelper.GetYear(result);

    }

    private static string GetArtist(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
        ?.DocumentNode
        ?.SelectSingleNode("//meta[@property='og:title']")
        ?.Attributes["content"].Value
        ?.Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
        ?.LastOrDefault()
        ?.Trim()
        ?? string.Empty;

        return WebUtility.HtmlDecode(result);
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
            ?.DocumentNode
            ?.SelectSingleNode("//meta[@property='og:title']")
            ?.Attributes["content"].Value
            ?.Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
            ?.FirstOrDefault()
            ?.Trim()
            ?? string.Empty;

        return WebUtility.HtmlDecode(result);
    }
}
