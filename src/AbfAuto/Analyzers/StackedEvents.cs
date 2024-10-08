using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class StackedEvents : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plot = new();

        double stackHeight = 10;

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep(i)
                .Detrend(TimeSpan.FromMilliseconds(200))
                .Smooth(TimeSpan.FromMilliseconds(3))
                .SubTraceByFraction(0.7, 0.9);

            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Color = Colors.C0.WithAlpha(.8);
            sig.LineWidth = 1.5f;
            sig.Data.YOffset = i * stackHeight;
        }

        plot.Axes.Frameless();
        plot.HideGrid();

        return AnalysisResult.Single(plot);
    }
}
