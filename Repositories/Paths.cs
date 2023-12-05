using System.IO;

namespace Repositories;

public static class Paths
{
    private static string _root = "../../Data";
    public static string Images => Path.Combine(_root, "Images");

    public static string GetTempPath<T>() => Path.Combine(_root, "Temp", typeof(T).ToString());
}
