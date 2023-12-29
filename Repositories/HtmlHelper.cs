using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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

    internal static void DownloadPNG(string webFile, string destinationFile)
    {
        if (string.IsNullOrEmpty(webFile))
        {
            return;
        }

        destinationFile = $"{destinationFile}.png";

        FileRepsitory.Delete(destinationFile);

        if (File.Exists(destinationFile))
        {
            return;
        }

        var directory = Path.GetDirectoryName(destinationFile);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (webFile == null || webFile == "N/A")
        {
            return;
        }

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(new Uri(webFile), destinationFile);
        }
    }

    public static string CleanUrl(string url)
    {
        return url?.Split('?')?.FirstOrDefault()?.Trim() ?? string.Empty;
    }
}
