using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AvaloniaApplication1.ViewModels.Extensions;
using HtmlAgilityPack;
using Repositories;

namespace AvaloniaApplication1.Repositories.External;

public class Goodreads : IExternal<Book>
{
    public static string UrlIdentifier => "goodreads.com";

    public Book GetItem(string url)
    {
        Book result = new();

        using (var client = new WebClient())
        {
            var content = client.DownloadData(url);
            using (var stream = new MemoryStream(content))
            {
                var text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(text);

                var title = GetTitle(htmlDocument);
                var writer = GetWriter(htmlDocument);
                var year = GetYear(htmlDocument);
                var goodreadsID = GetGoogreadsID(url);
                var pages = GetPages(htmlDocument);
                var imageUrl = GetImageUrl(htmlDocument);

                var destinationFile = Paths.GetTempPath<Book>();
                HtmlHelper.DownloadPNG(imageUrl, destinationFile);

                result = new Book
                {
                    Title = title,
                    Author = writer,
                    Year = year,
                    GoodreadsID = goodreadsID
                };
            }
        }

        return result;
    }

    private string GetImageUrl(HtmlDocument htmlDocument)
    {
        var result = htmlDocument.DocumentNode
            .SelectNodes("//img[contains(@class, 'ResponsiveImage')]")
            .FirstOrDefault()
            .Attributes["src"]
            .Value;

        return result;
    }

    // public static Comic GetGoodreadsDataComic(string url)
    // {
    //     Comic result = null;

    //     using (var client = new WebClient())
    //     {
    //         var content = client.DownloadData(url);
    //         using (var stream = new MemoryStream(content))
    //         {
    //             var text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
    //             var htmlDocument = new HtmlDocument();
    //             htmlDocument.LoadHtml(text);

    //             var title = GetTitle(htmlDocument);
    //             var writer = GetWriter(htmlDocument);
    //             var year = GetYear(htmlDocument);
    //             var goodreadsID = GetGoogreadsID(url);
    //             var illustrator = htmlDocument.DocumentNode
    //                 .SelectNodes("//span[contains(@class, 'ContributorLink__name')]")
    //                 .LastOrDefault()
    //                 .InnerText.Trim()
    //                 .TrimEnd("(Illustrator)")
    //                 .Trim();

    //             result = new Comic
    //             {
    //                 Title = title,
    //                 Writer = writer,
    //                 Illustrator = illustrator,
    //                 Year = year,
    //                 GoodreadsID = goodreadsID
    //             };
    //         }
    //     }

    //     return result;
    // }

    private static int GetGoogreadsID(string url)
    {
        return System.Convert.ToInt32(
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
        return System.Convert.ToInt32(
            HtmlHelper.GetYear(
                htmlDocument.DocumentNode
                    .SelectNodes("//p[contains(@data-testid, 'publicationInfo')]")
                    .FirstOrDefault()
                    .InnerText.Trim()
            )
        );
    }

    private static string GetWriter(HtmlDocument htmlDocument)
    {
        var result = htmlDocument.DocumentNode
            .SelectNodes("//span[contains(@class, 'ContributorLink__name')]")
            .FirstOrDefault()
            .InnerText.Replace("(Goodreads Author)", "")
            .Trim()
            .TrimEnd(',');

        return WebUtility.HtmlDecode(result);
    }

    private static string GetTitle(HtmlDocument htmlDocument)
    {
        var result = htmlDocument.DocumentNode
            .SelectNodes("//h1[contains(@class, 'Text Text__title1')]")
            .FirstOrDefault()
            .InnerText.Trim()
            .TrimEnd(", Volume 1")
            .TrimEnd(", Vol. 1")
            .Trim();

        return WebUtility.HtmlDecode(result);
    }

    // public static Model.Collection.Comic GetGoodreadsDataComicCollection(string url)
    // {
    //     var comic = GetGoodreadsDataComic(url);

    //     return new Model.Collection.Comic
    //     {
    //         Title = comic.Title,
    //         Writer = comic.Writer,
    //         Illustrator = comic.Illustrator,
    //         GoodreadsID = comic.GoodreadsID
    //     };
    // }

    public static int GetPages(HtmlDocument htmlDocument)
    {
        var str = htmlDocument.DocumentNode
                                           .SelectNodes("//p[contains(@data-testid, 'pagesFormat')]")
                                           .FirstOrDefault()
                                           .InnerText.Trim();

        var rows = str.Split('\n');
        var pagesRow = rows.FirstOrDefault(o => o.Contains("pages"));

        if (pagesRow == null)
        {
            return 0;
        }

        var pageString = Regex.Match(pagesRow, @"\d+").Value;

        return int.Parse(pageString);
    }

    // public static string GetTitle(string url)
    // {
    //     using (var client = new WebClient())
    //     {
    //         var content = client.DownloadData(url);
    //         using (var stream = new MemoryStream(content))
    //         {
    //             string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

    //             var htmlDocument = new HtmlDocument();
    //             htmlDocument.LoadHtml(result);
    //             var node = htmlDocument.DocumentNode.SelectSingleNode("//head/title");

    //             if (node == null || string.IsNullOrWhiteSpace(node.InnerText))
    //             {
    //                 return string.Empty;
    //             }

    //             return node.InnerText.Trim();
    //         }
    //     }
    // }
}
