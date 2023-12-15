using System.IO;
using System.Linq;
using System.Net;
using AvaloniaApplication1.ViewModels.Extensions;
using HtmlAgilityPack;

namespace AvaloniaApplication1.Repositories.External;

public class Goodreads : IExternal<Book>
{
    public static string UrlIdentifier => "goodreads.com";

    public Book GetItem(string url)
    {
        throw new System.NotImplementedException();
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

    // private static int GetGoogreadsID(string url)
    // {
    //     return System.Convert.ToInt32(
    //         url.TrimStart("https://www.goodreads.com/book/show/")
    //             .TrimStart("https://www.goodreads.com/en/book/show/")
    //             .Trim()
    //             .Split('.')
    //             .First()
    //             .Split('-')
    //             .First()
    //     );
    // }

    // private static int GetYear(HtmlDocument htmlDocument)
    // {
    //     return System.Convert.ToInt32(
    //         Igdb.GetYear(
    //             htmlDocument.DocumentNode
    //                 .SelectNodes("//p[contains(@data-testid, 'publicationInfo')]")
    //                 .FirstOrDefault()
    //                 .InnerText.Trim()
    //         )
    //     );
    // }

    // private static string GetWriter(HtmlDocument htmlDocument)
    // {
    //     return htmlDocument.DocumentNode
    //         .SelectNodes("//span[contains(@class, 'ContributorLink__name')]")
    //         .FirstOrDefault()
    //         .InnerText.Replace("(Goodreads Author)", "")
    //         .Trim()
    //         .TrimEnd(',');
    // }

    // private static string GetTitle(HtmlDocument htmlDocument)
    // {
    //     return htmlDocument.DocumentNode
    //         .SelectNodes("//h1[contains(@class, 'Text Text__title1')]")
    //         .FirstOrDefault()
    //         .InnerText.Trim()
    //         .TrimEnd(", Volume 1")
    //         .TrimEnd(", Vol. 1")
    //         .Trim();
    // }

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

    // public static Model.Collection.Book GetGoodreadsDataBook(string url)
    // {
    //     Model.Collection.Book result = null;

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
    //             var pages = System.Convert.ToInt32(
    //                 GetPages(
    //                     htmlDocument.DocumentNode
    //                         .SelectNodes("//p[contains(@data-testid, 'pagesFormat')]")
    //                         .FirstOrDefault()
    //                         .InnerText.Trim()
    //                 )
    //             );

    //             result = new Model.Collection.Book
    //             {
    //                 Title = title,
    //                 Author = writer,
    //                 Year = year,
    //                 GoodreadsID = goodreadsID,
    //                 Pages = pages
    //             };
    //         }
    //     }

    //     return result;
    // }

    // public static string GetPages(string str)
    // {
    //     var rows = str.Split('\n');
    //     var pagesRow = rows.FirstOrDefault(o => o.Contains("pages"));

    //     if (pagesRow == null)
    //     {
    //         return null;
    //     }

    //     return Regex.Match(pagesRow, @"\d+").Value;
    // }

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
