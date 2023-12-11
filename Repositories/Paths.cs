using System.IO;

namespace Repositories;

public static class Paths
{
    public static string Images => Path.Combine(GetRootPath(), "Images");
    public static string APIKeys => Path.Combine(GetRootPath(), ".Keys");
    public static string Data => GetRootPath();

    public static string GetTempPath<T>() =>
        Path.Combine(GetRootPath(), ".Temp", typeof(T).ToString());

    public static string GetImagesPath<T>() => Path.Combine(Images, typeof(T).ToString());

    public static string GetImagePath<T>(int itemID) =>
        Path.Combine(Images, typeof(T).ToString(), $"{itemID}.png");

    private static string GetRootPath()
    {
        var rootPath = "Data";

        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }

        return rootPath;
    }
}
