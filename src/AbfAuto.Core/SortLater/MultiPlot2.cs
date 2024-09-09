using ScottPlot;
using SkiaSharp;

namespace AbfAuto.Core;

public class SizedPlot
{
    public Plot? Plot { get; }
    public MultiPlot2? MultiPlot { get; }
    public PixelSize Size { get; }

    public SizedPlot(Plot plot, PixelSize size)
    {
        Plot = plot;
        MultiPlot = null;
        Size = size;
    }

    public SizedPlot(MultiPlot2 multiPlot, PixelSize size)
    {
        Plot = null;
        MultiPlot = multiPlot;
        Size = size;
    }

    public void SavePng(string path)
    {
        Plot?.SavePng(path, (int)Size.Width, (int)Size.Height);
        MultiPlot?.SavePng(path, (int)Size.Width, (int)Size.Height);
    }
}

public struct SubplotRect
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double Left { get; set; }
    public double Top { get; set; }
    public bool Fractional { get; set; }

    public PixelRect GetPixelRect(int width, int height)
    {
        if (!Fractional)
        {
            Pixel topLeft = new(Left, Top);
            PixelSize size = new(Width, Height);
            return new PixelRect(topLeft, size);
        }
        else
        {
            Pixel topLeft = new(Left * width, Top * height);
            PixelSize size = new(Width * width, Height * height);
            return new PixelRect(topLeft, size);
        }
    }
}

public class Subplot(ScottPlot.Plot plot, SubplotRect rect)
{
    public ScottPlot.Plot Plot { get; set; } = plot;
    public SubplotRect Rect { get; set; } = rect;
    public PixelRect GetPixelRect(int width, int height) => Rect.GetPixelRect(width, height);

    public static Subplot FullSize(Plot plot)
    {
        SubplotRect rect = new()
        {
            Width = 1,
            Height = 1,
            Left = 0,
            Top = 0,
            Fractional = true,
        };

        return new Subplot(plot, rect);
    }
}

public class MultiPlot2
{
    public List<Subplot> Subplots { get; } = [];

    public MultiPlot2()
    {

    }

    public static MultiPlot2 WithSinglePlot(Plot plot)
    {
        MultiPlot2 mp = new();
        mp.AddFullSize(plot);
        return mp;
    }

    public void AddFullSize(Plot plot)
    {
        Subplots.Add(Subplot.FullSize(plot));
    }

    public void AddSubplot(Plot plot, int rowIndex, int totalRows, int columnIndex, int totalColumns)
    {
        double colWidth = 1.0 / totalColumns;
        double colHeight = 1.0 / totalRows;

        SubplotRect rect = new()
        {
            Width = colWidth,
            Height = colHeight,
            Left = colWidth * columnIndex,
            Top = colHeight * rowIndex,
            Fractional = true,
        };

        Subplots.Add(new(plot, rect));
    }

    public Image Render(int width, int height)
    {
        SKImageInfo imageInfo = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        SKSurface surface = SKSurface.Create(imageInfo);

        foreach (Subplot subplot in Subplots)
        {
            PixelRect rect = subplot.GetPixelRect(width, height);
            subplot.Plot.RenderManager.ClearCanvasBeforeEachRender = false;
            subplot.Plot.Render(surface.Canvas, rect);
        }

        return new(surface);
    }

    public SavedImageInfo SavePng(string filename, int width = 800, int height = 600)
    {
        return Render(width, height).SavePng(filename);
    }
}