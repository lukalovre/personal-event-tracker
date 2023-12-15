using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Repositories;

public static class HtmlHelper
{
    public static string GetYear(string str)
    {
        return Regex.Match(str, @"\d{4}").Value;
    }

    public static void OpenLink(string link)
    {
        Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
    }

    internal static void DownloadPNG(string webFile, string destinationFile)
    {
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
        return url.Split('?').FirstOrDefault().Trim();
    }
}
