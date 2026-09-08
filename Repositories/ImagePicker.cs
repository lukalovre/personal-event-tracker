using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace Repositories;

public static class ImagePicker
{
    public static async Task<Bitmap?> PickAndSaveAsync(Window window, string destinationPath)
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

        bitmap.Save(destinationPath);
        return bitmap;
    }
}