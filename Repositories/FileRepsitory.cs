using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace Repositories;

public class FileRepsitory
{
    public static Bitmap? GetImage<T>(string fileName)
        where T : IItem
    {
        var filePath = Path.Combine(Paths.Images, typeof(T).ToString(), $"{fileName}.png");

        if (!File.Exists(filePath))
        {
            return null;
        }

        return new Bitmap(filePath);
    }

    public static Bitmap? GetImageTemp<T>()
        where T : IItem
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
}
