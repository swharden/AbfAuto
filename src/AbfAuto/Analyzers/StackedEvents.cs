using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class StackedEvents : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        double deltaY = 10;

        Plot plot = new();

        ScottPlot.Plottables.ScaleBar scalebar = new()
        {
            Width = 0.1,
            Height = deltaY,
            XLabel = "100 ms",
            YLabel = "10 pA",
        };

        plot.Add.Plottable(scalebar);

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep(i)
                .Detrend(TimeSpan.FromMilliseconds(200))
                .Smooth(TimeSpan.FromMilliseconds(3))
                .SubTraceByFraction(0.7, 0.9);

            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Color = Colors.C0.WithAlpha(.8);
            sig.LineWidth = 1.5f;
            sig.Data.YOffset = i * deltaY;
        }

        plot.Axes.Frameless();
        plot.HideGrid();

        return AnalysisResult.Single(plot);
    }
}
