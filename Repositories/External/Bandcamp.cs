using System;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Bandcamp
{
    public static string UrlIdentifier => "bandcamp.com";

    public static Music GetAlbumInfoBandcamp(string url)
    {
        using var client = new WebClient();
        var content = client.DownloadData(url);
        using var stream = new MemoryStream(content);
        var text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(text);

        var title = GetTitle(htmlDocument);
        var artist = GetArtist(htmlDocument);
        var year = GetYear(htmlDocument);
        var bandcampLink = GetLink(htmlDocument);
        var runtime = GetRuntime(htmlDocument);

        string destinationFile = Paths.GetTempPath<Music>();
        HtmlHelper.DownloadPNG(
            htmlDocument.DocumentNode.SelectSingleNode("//a[@class='popupImage']").Attributes[
                "href"
            ].Value.Trim(),
            destinationFile
        );

        return new Music
        {
            Artist = artist,
            Title = title,
            Year = year,
            _1001 = false,
            Runtime = runtime,
            SpotifyID = bandcampLink
        };
    }

    private static int GetRuntime(HtmlDocument htmlDocument)
    {
        var result = 0;

        try
        {
            var totalMinutes = 0;
            var totalSeconds = 0;

            foreach (
                var item in htmlDocument.DocumentNode.SelectNodes(
                    "//span[contains(@class, 'time secondaryText')]"
                )
            )
            {
                var timeString = item.InnerText.Trim();
                var split = timeString.Split(':');

                if (split.Count() != 2)
                {
                    continue;
                }

                var minutes = Convert.ToInt32(split[0]);
                var seconds = Convert.ToInt32(split[1]);

                totalMinutes += minutes;
                totalSeconds += seconds;
            }

            result =
                totalMinutes + (int)Math.Round(totalSeconds / 60f, MidpointRounding.AwayFromZero);
        }
        catch { }

        return result;
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
            result = Convert.ToInt32(
                HtmlHelper.GetYear(
                    htmlDocument.DocumentNode
                        .SelectSingleNode("//div[@class='tralbumData tralbum-credits']")
                        .InnerText.Trim()
                )
            );
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
