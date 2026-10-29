using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using HtmlAgilityPack;

namespace Repositories;

public static class HtmlHelper
{
    private static readonly ConditionalWeakTable<HtmlDocument, LocalHtmlSource> LocalHtmlSources = new();
    private static readonly ConcurrentDictionary<string, byte> ForbiddenImageHosts = new(StringComparer.OrdinalIgnoreCase);

    internal static Func<Task<string?>>? LocalHtmlFilePicker { get; set; }

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
        OpenLink(GetLinkUrl(link, arguments));
    }

    public static string GetLinkUrl(string link, List<string> arguments)
    {
        if (string.IsNullOrWhiteSpace(link))
        {
            // Make this a search engine choice in settings
            link = $"https://duckduckgo.com/?q={string.Join("+", arguments)}";
        }

        return link;
    }

    internal async static Task DownloadPNG(string webFile, string destinationFile, HtmlDocument? sourceDocument = null)
    {
        if (string.IsNullOrWhiteSpace(webFile))
        {
            return;
        }

        destinationFile = $"{destinationFile}.png";

        FileRepository.Delete(destinationFile);

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

        if (sourceDocument is not null && LocalHtmlSources.TryGetValue(sourceDocument, out _))
        {
            if (TryResolveLocalImage(webFile, sourceDocument, out var localImagePath))
            {
                using var bitmap = new Bitmap(localImagePath);
                using var destination = File.Create(destinationFile);
                bitmap.Save(destination);
            }

            return;
        }

        if (!Uri.TryCreate(webFile, UriKind.Absolute, out var imageUri)
            || (imageUri.Scheme != Uri.UriSchemeHttp && imageUri.Scheme != Uri.UriSchemeHttps))
        {
            return;
        }

        await DownloadFile(imageUri.AbsoluteUri, destinationFile);
    }

    private async static Task DownloadFile(string imageUrl, string imagePath)
    {
        var imageUri = new Uri(imageUrl);
        if (ForbiddenImageHosts.ContainsKey(imageUri.Host))
        {
            return;
        }

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(imageUri, HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            ForbiddenImageHosts.TryAdd(imageUri.Host, 0);
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            return;
        }

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
        if (!url.StartsWith("https://"))
        {
            url = $"https://{url}";
        }

        using var client = new HttpClient();
        try
        {
            var text = await client.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(text);
            return htmlDocument;
        }
        catch (HttpRequestException)
        {
            if (LocalHtmlFilePicker is null)
            {
                throw;
            }

            var localHtmlPath = await LocalHtmlFilePicker();
            if (string.IsNullOrWhiteSpace(localHtmlPath))
            {
                throw;
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(localHtmlPath);
            LocalHtmlSources.Add(htmlDocument, new LocalHtmlSource(localHtmlPath));
            return htmlDocument;
        }
    }

    private static bool TryResolveLocalImage(string imageUrl, HtmlDocument document, out string localImagePath)
    {
        localImagePath = string.Empty;
        if (!LocalHtmlSources.TryGetValue(document, out var localHtmlSource))
        {
            return false;
        }

        var htmlDirectory = Path.GetDirectoryName(localHtmlSource.FilePath) ?? string.Empty;
        var imageReference = imageUrl;
        var isRemoteUrl = Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri)
            && (imageUri.Scheme == Uri.UriSchemeHttp || imageUri.Scheme == Uri.UriSchemeHttps);

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out imageUri) && imageUri.IsFile)
        {
            imageReference = imageUri.LocalPath;
        }
        else if (isRemoteUrl)
        {
            imageReference = imageUri!.AbsolutePath;
        }
        else
        {
            imageReference = imageReference.Split('?', '#')[0];
            var relativePath = Uri.UnescapeDataString(imageReference).TrimStart('/', '\\');
            var directPath = Path.GetFullPath(Path.Combine(htmlDirectory, relativePath));
            var pathFromHtmlDirectory = Path.GetRelativePath(htmlDirectory, directPath);
            if (!pathFromHtmlDirectory.StartsWith("..", StringComparison.Ordinal) && File.Exists(directPath))
            {
                localImagePath = directPath;
                return true;
            }
        }

        var imageFileName = Path.GetFileName(Uri.UnescapeDataString(imageReference));
        if (string.IsNullOrWhiteSpace(imageFileName))
        {
            return false;
        }

        var pageName = Path.GetFileNameWithoutExtension(localHtmlSource.FilePath);
        var resourceDirectories = new[]
        {
            Path.Combine(htmlDirectory, $"{pageName}_files"),
            Path.Combine(htmlDirectory, $"{pageName}.files"),
            Path.Combine(htmlDirectory, pageName)
        };

        var sameDirectoryImage = Path.Combine(htmlDirectory, imageFileName);
        if (File.Exists(sameDirectoryImage))
        {
            localImagePath = sameDirectoryImage;
            return true;
        }

        foreach (var resourceDirectory in resourceDirectories.Distinct())
        {
            if (!Directory.Exists(resourceDirectory))
            {
                continue;
            }

            var match = Directory.EnumerateFiles(resourceDirectory, "*", SearchOption.AllDirectories)
                .FirstOrDefault(path => string.Equals(Path.GetFileName(path), imageFileName, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                localImagePath = match;
                return true;
            }
        }

        return false;
    }

    private sealed record LocalHtmlSource(string FilePath);
}
