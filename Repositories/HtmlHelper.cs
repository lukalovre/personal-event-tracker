using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Repositories;

public static class HtmlHelper
{
    public static int GetYear(string str)
    {
        var years = Regex.Matches(str, @"\d{4}");
        var yearList = years.Select(o => Convert.ToInt32(o.Value));
        return yearList.FirstOrDefault(o => o > 1900 && o < 2999);
    }

    public static void OpenLink(string link)
    {
        Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
    }

    public static void OpenLink(string link, List<string> arguments)
    {
        if (string.IsNullOrWhiteSpace(link))
        {
            // Make this a search engine choice in settings
            link = $"https://duckduckgo.com/?q={string.Join("+", arguments)}";
        }

        OpenLink(link);
    }

    internal async static Task DownloadPNG(string webFile, string destinationFile)
    {
        if (string.IsNullOrWhiteSpace(webFile))
        {
            return;
        }

        destinationFile = $"{destinationFile}.png";

        FileRepsitory.Delete(destinationFile);

        if (File.Exists(destinationFile))
        {
            return;
        }

        var directory = Path.GetDirectoryName(destinationFile) ?? string.Empty;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (webFile == null || webFile == "N/A")
        {
            return;
        }

        await DownloadFile(webFile, destinationFile);
    }

    private async static Task DownloadFile(string imageUrl, string imagePath)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        using var ms = await response.Content.ReadAsStreamAsync();
        using var fs = File.Create(imagePath);
        await ms.CopyToAsync(fs);
        fs.Flush();
    }

    public static string CleanUrl(string url)
    {
        return url
        ?.Split('?')
        ?.FirstOrDefault()
        ?.Trim()
        ?? string.Empty;
    }

    internal async static Task<HtmlDocument> DownloadWebpage(string url)
    {
        using var client = new HttpClient();
        var text = await client.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(text);

        return htmlDocument;
    }
}
