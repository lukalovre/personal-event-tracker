using System.IO;

namespace Repositories;

public static class Paths
{
    private static readonly string _root = "../../Data";
    public static string Images => Path.Combine(_root, "Images");
    public static string APIKeys => Path.Combine(_root, ".Keys");
    public static string Data => _root;

    public static string GetTempPath<T>() => Path.Combine(_root, ".Temp", typeof(T).ToString());

    public static string GetImagesPath<T>() => Path.Combine(Images, typeof(T).ToString());

    public static string GetImagePath<T>(int itemID) =>
        Path.Combine(Images, typeof(T).ToString(), $"{itemID}.png");
}
