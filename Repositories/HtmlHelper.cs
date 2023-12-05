using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Repositories;

public static class HtmlHelper
{
    public static string GetYear(string str)
    {
        return Regex.Match(str, @"\d{4}").Value;
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
}