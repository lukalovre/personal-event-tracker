using System.IO;
using Avalonia.Media.Imaging;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;

namespace Repositories;

public class FileRepository
{
    public static bool ImageExists<T>(int itemID) where T : IItem
    {
        var filePath = Path.Combine(Paths.Images, Helpers.GetClassName<T>(), $"{itemID}.png");
        return File.Exists(filePath);
    }

    public static Bitmap? GetImage<T>(int itemID) where T : IItem
    {
        return GetImage(Helpers.GetClassName<T>(), itemID, typeof(T).Name == "Music");
    }

    public static Bitmap? GetImage(string type, int itemID, bool normalize = false)
    {
        var filePath = Path.Combine(Paths.Images, type, $"{itemID}.png");

        if (!File.Exists(filePath))
        {
            return null;
        }

        if (normalize)
        {
            ImageNormalizer.Normalize(filePath);
            using var normalizedStream = File.OpenRead(filePath);
            return new Bitmap(normalizedStream);
        }

        using var stream = File.OpenRead(filePath);
        return new Bitmap(stream);
    }

    public static Bitmap? GetImageTemp<T>() where T : IItem
    {
        var filePath = Path.Combine($"{Paths.GetTempPath<T>()}.png");

        if (!File.Exists(filePath))
        {
            return null;
        }

        if (typeof(T).Name == "Music")
        {
            ImageNormalizer.Normalize(filePath);
            using var normalizedStream = File.OpenRead(filePath);
            return new Bitmap(normalizedStream);
        }

        using var stream = File.OpenRead(filePath);
        return new Bitmap(stream);
    }

    public static void Delete(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        File.Delete(filePath);
    }

    public static void MoveTempImage<T>(int itemID)
    {
        var tempFile = $"{Paths.GetTempPath<T>()}.png";
        var destinationFile = Path.Combine(Paths.GetImagesPath<T>(), $"{itemID}.png");

        if (!File.Exists(tempFile))
        {
            return;
        }

        if (typeof(T).Name == "Music")
        {
            ImageNormalizer.Normalize(tempFile);
        }
        File.Copy(tempFile, destinationFile);
        File.Delete(tempFile);
    }
}
