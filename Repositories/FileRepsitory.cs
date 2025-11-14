using System.IO;
using Avalonia.Media.Imaging;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;

namespace Repositories;

public class FileRepsitory
{
    public static bool ImageExists<T>(int itemID) where T : IItem
    {
        var filePath = Path.Combine(Paths.Images, Helpers.GetClassName<T>(), $"{itemID}.png");
        return File.Exists(filePath);
    }

    public static Bitmap? GetImage<T>(int itemID) where T : IItem
    {
        return GetImage(Helpers.GetClassName<T>(), itemID);
    }

    public static Bitmap? GetImage(string type, int itemID)
    {
        var filePath = Path.Combine(Paths.Images, type, $"{itemID}.png");

        if (!File.Exists(filePath))
        {
            return null;
        }

        return new Bitmap(filePath);
    }

    public static Bitmap? GetImageTemp<T>() where T : IItem
    {
        var filePath = Path.Combine($"{Paths.GetTempPath<T>()}.png");

        if (!File.Exists(filePath))
        {
            return null;
        }

        return new Bitmap(filePath);
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

        File.Copy(tempFile, destinationFile);
        File.Delete(tempFile);
    }
}
