using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Soundcloud
{
    public static string UrlIdentifier => "soundcloud.com";

    internal static async Task<SoundcloudItem> GetSoundcloudItem<T>(string url)
    {
        var htmlDocument = await HtmlHelper.DownloadWebpage(url);
        var title = GetTitle(htmlDocument);
        var artist = GetArtist(htmlDocument);
        var year = GetYear(htmlDocument);
        var link = GetLink(htmlDocument);
        var runtime = GetRuntimeSong(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);

        string destinationFile = Paths.GetTempPath<T>();
        await HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new SoundcloudItem(
            title.Trim(),
            artist.Trim(),
            year,
            runtime,
            link.Trim());
    }

    private static string GetImageUrl(HtmlDocument htmlDocument)
    {
        return htmlDocument
                ?.DocumentNode
                ?.SelectSingleNode("//img")
                ?.Attributes["src"]
                ?.Value
                ?.Trim()
                ?? string.Empty;
    }

    private static int GetRuntimeSong(HtmlDocument htmlDocument)
    {
        // Timecode in format PT00H04M00S
        var timeCode = htmlDocument
                        ?.DocumentNode
                        ?.SelectSingleNode("//meta[contains(@itemprop, 'duration')]")
                        ?.GetAttributeValue("content", string.Empty);

        timeCode = timeCode?.Split("PT").LastOrDefault();

        var hourString = timeCode?.Split('H').FirstOrDefault();
        timeCode = timeCode?.Split('H').LastOrDefault();

        var minuteString = timeCode?.Split('M').FirstOrDefault();
        timeCode = timeCode?.Split('M').LastOrDefault();

        var secondString = timeCode?.Split('S').FirstOrDefault();

        var hours = Convert.ToInt32(hourString);
        var minutes = Convert.ToInt32(minuteString);
        var seconds = Convert.ToInt32(secondString);

        var result = minutes
        + (int)Math.Round(seconds / 60f, MidpointRounding.AwayFromZero)
        + hours * 60;

        return result;
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
                    ?.SelectSingleNode("//time")
                    ?.InnerText
                    ?? string.Empty;

        return HtmlHelper.GetYear(result);
    }

    private static string GetArtist(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
                    ?.DocumentNode
                    ?.SelectSingleNode("//title")
                    ?.InnerText
                    ?.Split(new string[] { " by " }, StringSplitOptions.RemoveEmptyEntries)
                    ?.LastOrDefault()
                    ?.Split("|")
                    ?.FirstOrDefault()
                    ?? string.Empty;

        return WebUtility.HtmlDecode(result.Trim());
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
                    ?.DocumentNode
                    ?.SelectSingleNode("//meta[@property='og:title']")
                    ?.Attributes["content"].Value
                    ?.Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
                    ?.FirstOrDefault()
                    ?? string.Empty;

        return WebUtility.HtmlDecode(result.Trim());
    }
}
