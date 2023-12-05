using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Repositories;

public class MusicRepository
{
    public static Music GetAlbumInfoBandcamp(string url)
    {
        using (var client = new WebClient())
        {
            var content = client.DownloadData(url);

            using (var stream = new MemoryStream(content))
            {
                var text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(text);

                var title = htmlDocument.DocumentNode
                    .SelectSingleNode("//meta[@property='og:title']")
                    .Attributes["content"].Value
                    .Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    .Trim();
                var artist = htmlDocument.DocumentNode
                    .SelectSingleNode("//meta[@property='og:title']")
                    .Attributes["content"].Value
                    .Split(new string[] { ", by" }, StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault()
                    .Trim();

                title = WebUtility.HtmlDecode(title);
                artist = WebUtility.HtmlDecode(artist);

                var year = Convert.ToInt32(
                    GetYear(
                        htmlDocument.DocumentNode
                            .SelectSingleNode("//div[@class='tralbumData tralbum-credits']")
                            .InnerText.Trim()
                    )
                );
                var bandcampLink = htmlDocument.DocumentNode
                    .SelectSingleNode("//meta[@property='og:url']")
                    .Attributes["content"].Value.Trim();

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

                var runtime =
                    totalMinutes
                    + (int)Math.Round(totalSeconds / 60f, MidpointRounding.AwayFromZero);

                // var destinationFile = Paths.TempAlbumCover;
                // File.Delete($"{destinationFile}.png");
                // Web.DownloadPNG(
                //     htmlDocument.DocumentNode
                //         .SelectSingleNode("//a[@class='popupImage']")
                //         .Attributes["href"].Value.Trim(),
                //     destinationFile
                // );

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
        }
    }

    public static string GetYear(string str)
    {
        return Regex.Match(str, @"\d{4}").Value;
    }
}
