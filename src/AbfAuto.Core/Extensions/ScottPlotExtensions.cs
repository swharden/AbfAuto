using AbfAuto.Core.SortLater;
using ScottPlot.Colormaps;

namespace AbfAuto.Core.Extensions;

public static class ScottPlotExtensions
{
    public static ScottPlot.Color[] GetColors(this ScottPlot.IColormap cmap, int count, double start = 0, double end = 1)
    {
        double step = (end - start) / (count - 1);
        return Enumerable.Range(0, count)
            .Select(i => cmap.GetColor(i * step + start))
            .ToArray();
    }

    public static void SaveForLabWebsite(this Multiplot mp, AbfSharp.ABF abf)
    {
        string abfFolder = Path.GetDirectoryName(abf.FilePath)!;
        string analysisFolder = Path.Combine(abfFolder, "_autoanalysis");
        if (!Directory.Exists(analysisFolder))
            Directory.CreateDirectory(analysisFolder);
        string saveAs = Path.Combine(analysisFolder, $"{abf.AbfID}_ApTimeCourse.png");
        mp.SavePng(saveAs);
        Console.WriteLine(saveAs);
    }

    public static ScottPlot.SavedImageInfo SaveForLabWebsite(this ScottPlot.Plot plot, AbfSharp.ABF abf, int width = 600, int height = 400)
    {
        string abfFolder = Path.GetDirectoryName(abf.FilePath)!;
        string analysisFolder = Path.Combine(abfFolder, "_autoanalysis");
        if (!Directory.Exists(analysisFolder))
            Directory.CreateDirectory(analysisFolder);
        string saveAs = Path.Combine(analysisFolder, $"{abf.AbfID}_ApTimeCourse.png");
        var saved = plot.SavePng(saveAs, width, height);
        Console.WriteLine(saved.Path);
        return saved;
    }

    public static void AddVerticalLinesAtCommentMinutes(this ScottPlot.Plot plot, AbfSharp.ABF abf)
    {
        foreach (var tag in abf.Tags)
        {
            var vLine = plot.Add.VerticalLine(tag.Time / 60);
            vLine.LineColor = ScottPlot.Colors.Red.WithAlpha(.5);
            vLine.LineWidth = 3;
            vLine.LinePattern = ScottPlot.LinePattern.Dashed;
        }
    }
}
