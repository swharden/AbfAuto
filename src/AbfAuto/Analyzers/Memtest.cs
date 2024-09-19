using AbfAuto.Memtest;
using ScottPlot;
using AbfSharp;

namespace AbfAuto.Analyzers;

/// <summary>
/// Get membrane test for a cell using fast repeated voltage-clamp sweeps with a hyperpolarization step
/// </summary>
public class Memtest : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        MemtestResult mt = MemtestLogic.GetMeanMemtest(abf);

        Plot plot = new();

        Color[] colors = new ScottPlot.Colormaps.Turbo().GetColors(abf.SweepCount, .1, .9);

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweepTrace = new(abf, i);
            var sig = plot.Add.Signal(sweepTrace.Values, sweepTrace.SamplePeriod);
            sig.Color = colors[i];
            sig.AlwaysUseLowDensityMode = true;
            sig.LineWidth = 1.5f;
        }

        var an = plot.Add.Annotation(mt.GetMessage(), Alignment.UpperRight);
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Gray.WithAlpha(.2);
        an.LabelFontSize = 16;
        an.LabelFontName = "Consolas";
        an.LabelStyle.BorderRadius = 10;
        an.LabelBorderWidth = 0;

        plot.Axes.Margins(horizontal: 0);
        plot.XLabel("Time (sec)");
        plot.YLabel("Current (pA)");
        plot.HideGrid();

        return AnalysisResult.Single(plot);
    }
}