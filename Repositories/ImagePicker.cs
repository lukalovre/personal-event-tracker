using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace Repositories;

public static class ImagePicker
{
    public static async Task<Bitmap?> PickAndSaveAsync(Window window, string destinationPath, bool normalize = false)
    {
        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Image files")
                {
                    Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif"]
                }
            ]
        });

        if (files.Count == 0)
        {
            return null;
        }

        await using var sourceStream = await files[0].OpenReadAsync();
        var bitmap = new Bitmap(sourceStream);
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

        bitmap.Save(destinationPath, new PngBitmapEncoderOptions());
        if (normalize)
        {
            ImageNormalizer.Normalize(destinationPath);
            using var normalizedStream = File.OpenRead(destinationPath);
            return new Bitmap(normalizedStream);
        }

        using var stream = File.OpenRead(destinationPath);
        return new Bitmap(stream);
    }
}