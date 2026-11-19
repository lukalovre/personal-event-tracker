using System;
using System.IO;
using EventTracker.Models;
using SkiaSharp;

namespace Repositories;

public static class ImageNormalizer
{
    public const int TargetSize = 320;

    public static bool AppliesTo<T>() => typeof(T) == typeof(Music) || typeof(T) == typeof(Song);

    public static bool Normalize(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            return false;
        }

        using var source = SKBitmap.Decode(imagePath);
        if (source is null)
        {
            return false;
        }

        if (source.Width == TargetSize && source.Height == TargetSize)
        {
            return true;
        }

        var cropSize = Math.Min(source.Width, source.Height);
        var cropX = (source.Width - cropSize) / 2;
        var cropY = (source.Height - cropSize) / 2;
        var sourceRect = new SKRectI(cropX, cropY, cropX + cropSize, cropY + cropSize);
        var normalized = new SKBitmap(TargetSize, TargetSize);
        using var canvas = new SKCanvas(normalized);
        canvas.DrawBitmap(source, sourceRect, new SKRect(0, 0, TargetSize, TargetSize));
        canvas.Flush();

        using var image = SKImage.FromBitmap(normalized);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.Create(imagePath);
        data.SaveTo(stream);
        normalized.Dispose();
        return true;
    }
}
