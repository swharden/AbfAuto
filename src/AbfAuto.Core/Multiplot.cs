using ScottPlot;
using ScottPlot.AxisLimitManagers;
using SkiaSharp;

namespace AbfAuto.Core;

public class Multiplot
{
    public int Width { get; }
    public int Height { get; }

    readonly SKSurface Surface;
    SKCanvas Canvas => Surface.Canvas;

    public static Multiplot WithSinglePlot(Plot plot, int width, int height)
    {
        Multiplot mp = new(width, height);
        mp.AddFullSize(plot);
        return mp;
    }

    public Multiplot(int width, int height)
    {
        SKImageInfo imageInfo = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        Width = width;
        Height = height;
        Surface = SKSurface.Create(imageInfo);
    }

    public void AddFullSize(Plot plot)
    {
        PixelRect pxRect = new(0, Width, Height, 0);
        plot.Render(Canvas, pxRect);
    }

    public void AddSubplot(Plot plot, int rowIndex, int totalRows, int columnIndex, int totalColumns)
    {
        PixelSize size = new(Width / totalColumns, Height / totalRows);
        Pixel corner = new(size.Width * columnIndex, size.Height * rowIndex);
        PixelRect pxRect = new(corner, size);
        plot.RenderManager.ClearCanvasBeforeEachRender = false;
        plot.Render(Canvas, pxRect);
    }

    public void SavePng(string path)
    {
        using SKImage skImage = Surface.Snapshot();
        ImageFormat format = ImageFormat.Png;
        SKEncodedImageFormat skFormat = format.ToSKFormat();
        using SKData data = skImage.Encode(skFormat, 100);
        byte[] bytes = data.ToArray();
        File.WriteAllBytes(path, bytes);
    }
}
