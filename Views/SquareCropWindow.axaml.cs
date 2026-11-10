using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace EventTracker.Views;

public partial class SquareCropWindow : Window
{
    private readonly string _sourcePath;
    private readonly Bitmap? _sourceBitmap;
    private bool _isDragging;
    private Point _dragStart;
    private Point _dragSelectionOrigin;
    private double _imageLeft;
    private double _imageTop;
    private double _imageDisplayWidth;
    private double _imageDisplayHeight;

    public SquareCropWindow()
        : this(string.Empty)
    {
    }

    public SquareCropWindow(string sourcePath)
    {
        _sourcePath = sourcePath;
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            _sourceBitmap = null;
        }
        else
        {
            using var stream = File.OpenRead(sourcePath);
            _sourceBitmap = new Bitmap(stream);
        }

        InitializeComponent();
        ImageView.Source = _sourceBitmap;

        Opened += OnOpened;
        ImageCanvas.PointerPressed += CropBox_PointerPressed;
        ImageCanvas.PointerMoved += CropBox_PointerMoved;
        ImageCanvas.PointerReleased += CropBox_PointerReleased;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (_sourceBitmap is null)
        {
            Close(false);
            return;
        }

        var sourceWidth = _sourceBitmap.PixelSize.Width;
        var sourceHeight = _sourceBitmap.PixelSize.Height;
        var scale = Math.Min(ImageCanvas.Bounds.Width / sourceWidth, ImageCanvas.Bounds.Height / sourceHeight);
        if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0)
        {
            scale = 1;
        }

        _imageDisplayWidth = sourceWidth * scale;
        _imageDisplayHeight = sourceHeight * scale;
        _imageLeft = (ImageCanvas.Bounds.Width - _imageDisplayWidth) / 2;
        _imageTop = (ImageCanvas.Bounds.Height - _imageDisplayHeight) / 2;

        ImageView.Width = _imageDisplayWidth;
        ImageView.Height = _imageDisplayHeight;
        Canvas.SetLeft(ImageView, _imageLeft);
        Canvas.SetTop(ImageView, _imageTop);

        var cropSize = Math.Min(_imageDisplayWidth, _imageDisplayHeight);
        CropBox.Width = cropSize;
        CropBox.Height = cropSize;

        var cropLeft = _imageLeft + ((_imageDisplayWidth - cropSize) / 2);
        var cropTop = _imageTop + ((_imageDisplayHeight - cropSize) / 2);

        Canvas.SetLeft(CropBox, cropLeft);
        Canvas.SetTop(CropBox, cropTop);
    }

    private void CropBox_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = true;
        _dragStart = e.GetPosition(ImageCanvas);
        _dragSelectionOrigin = new Point(Canvas.GetLeft(CropBox), Canvas.GetTop(CropBox));
    }

    private void CropBox_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        var current = e.GetPosition(ImageCanvas);
        var deltaX = current.X - _dragStart.X;
        var deltaY = current.Y - _dragStart.Y;

        var maxLeft = _imageLeft + (_imageDisplayWidth - CropBox.Width);
        var maxTop = _imageTop + (_imageDisplayHeight - CropBox.Height);

        var newLeft = Math.Clamp(_dragSelectionOrigin.X + deltaX, _imageLeft, maxLeft);
        var newTop = Math.Clamp(_dragSelectionOrigin.Y + deltaY, _imageTop, maxTop);

        Canvas.SetLeft(CropBox, newLeft);
        Canvas.SetTop(CropBox, newTop);
    }

    private void CropBox_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    private void OnApply(object? sender, RoutedEventArgs e)
    {
        using var source = SKBitmap.Decode(_sourcePath);
        if (source is null)
        {
            Close(false);
            return;
        }

        var cropLeft = Canvas.GetLeft(CropBox) - _imageLeft;
        var cropTop = Canvas.GetTop(CropBox) - _imageTop;
        var cropWidth = CropBox.Width;
        var cropHeight = CropBox.Height;

        var sourceX = (int)Math.Clamp(Math.Round((cropLeft / _imageDisplayWidth) * source.Width), 0, source.Width - 1);
        var sourceY = (int)Math.Clamp(Math.Round((cropTop / _imageDisplayHeight) * source.Height), 0, source.Height - 1);
        var sourceSize = (int)Math.Clamp(Math.Round((cropWidth / _imageDisplayWidth) * source.Width), 1, source.Width - sourceX);
        var sourceSizeY = (int)Math.Clamp(Math.Round((cropHeight / _imageDisplayHeight) * source.Height), 1, source.Height - sourceY);

        var finalSize = Math.Min(sourceSize, sourceSizeY);
        var adjustedX = Math.Min(sourceX, source.Width - finalSize);
        var adjustedY = Math.Min(sourceY, source.Height - finalSize);

        var cropped = new SKBitmap(finalSize, finalSize);
        using var canvas = new SKCanvas(cropped);
        var srcRect = new SKRectI(adjustedX, adjustedY, adjustedX + finalSize, adjustedY + finalSize);
        canvas.DrawBitmap(source, srcRect, new SKRect(0, 0, finalSize, finalSize));

        using var image = SKImage.FromBitmap(cropped);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using (var stream = File.Create(_sourcePath))
        {
            data.SaveTo(stream);
        }

        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
