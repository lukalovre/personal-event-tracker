using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AvaloniaApplication1.ViewModels.Extensions;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Goodreads
{
    public static string UrlIdentifier => "goodreads.com";

    internal static async Task<GoodreadsItem> GetGoodredsItem<T>(string url)
    {
        var htmlDocument = await HtmlHelper.DownloadWebpage(url);

        var title = GetTitle(htmlDocument);
        var writer = GetWriter(htmlDocument);
        var year = GetYear(htmlDocument);
        var goodreadsID = GetGoogreadsID(url);
        var pages = GetPages(htmlDocument);
        var imageUrl = GetImageUrl(htmlDocument);
        string illustrator = GetIllustrator(htmlDocument);

        var destinationFile = Paths.GetTempPath<T>();
        await HtmlHelper.DownloadPNG(imageUrl, destinationFile);

        return new GoodreadsItem(
            title.Trim(),
            writer.Trim(),
            illustrator.Trim(),
            year,
            goodreadsID,
            pages,
            imageUrl.Trim());
    }

    private static string GetIllustrator(HtmlDocument htmlDocument)
    {
        return htmlDocument
                    ?.DocumentNode
                    ?.SelectNodes("//span[contains(@class, 'ContributorLink__name')]")
                    ?.LastOrDefault()
                    ?.InnerText.Trim()
                    ?.TrimEnd("(Illustrator)")
                    ?.Trim()
                    ?? string.Empty;
    }

    private static string GetImageUrl(HtmlDocument htmlDocument)
    {
        return htmlDocument
                        ?.DocumentNode
                        ?.SelectNodes("//img[contains(@class, 'ResponsiveImage')]")
                        ?.FirstOrDefault()
                        ?.Attributes["src"]
                        ?.Value
                        ?? string.Empty;

    }

    private static int GetGoogreadsID(string url)
    {
        return Convert.ToInt32(
            url.TrimStart("https://www.goodreads.com/book/show/")
                .TrimStart("https://www.goodreads.com/en/book/show/")
                .Trim()
                .Split('.')
                .First()
                .Split('-')
                .First()
        );
    }

    private static int GetYear(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
                        ?.DocumentNode
                        ?.SelectNodes("//p[contains(@data-testid, 'publicationInfo')]")
                        ?.FirstOrDefault()
                        ?.InnerText
                        ?.Trim()
                        ?? string.Empty;

        return Convert.ToInt32(HtmlHelper.GetYear(result));
    }

    private static string GetWriter(HtmlDocument htmlDocument)
    {
        var result = htmlDocument.DocumentNode
               ?.SelectNodes("//span[contains(@class, 'ContributorLink__name')]")
               ?.FirstOrDefault()
               ?.InnerText
               ?.Replace("(Goodreads Author)", "")
               ?.Trim()
               ?.TrimEnd(',')
               ?? string.Empty;

        return WebUtility.HtmlDecode(result);
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var result = htmlDocument
               ?.DocumentNode
               ?.SelectNodes("//h1[contains(@class, 'Text Text__title1')]")
               ?.FirstOrDefault()
               ?.InnerText.Trim()
               ?.TrimEnd(", Volume 1")
               ?.TrimEnd(", Vol. 1")
               ?.Trim()
               ?? string.Empty;

        return WebUtility.HtmlDecode(result);
    }

    public static int GetPages(HtmlDocument htmlDocument)
    {
        var str = htmlDocument
        ?.DocumentNode
        ?.SelectNodes("//p[contains(@data-testid, 'pagesFormat')]")
        ?.FirstOrDefault()
        ?.InnerText
        ?.Trim()
        ?? string.Empty;

        var rows = str.Split('\n');
        var pagesRow = rows.FirstOrDefault(o => o.Contains("pages"));

        if (pagesRow == null)
        {
            return 0;
        }

        var pageString = Regex.Match(pagesRow, @"\d+").Value;

        var result = int.Parse(pageString);

        return result;
    }
}
