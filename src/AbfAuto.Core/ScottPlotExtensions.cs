namespace AbfAuto.Core;

public static class ScottPlotExtensions
{
    public static ScottPlot.Color[] GetColors(this ScottPlot.IColormap cmap, int count, double start = 0, double end = 1)
    {
        double step = (end - start) / (count - 1);
        return Enumerable.Range(0, count)
            .Select(i => cmap.GetColor(i * step + start))
            .ToArray();
    }
}
