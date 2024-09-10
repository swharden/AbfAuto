using AbfAuto.Core.Memtest;
using AbfAuto.Core.SortLater;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class P0201_Memtest : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        MemtestResult mt = MemtestLogic.GetMeanMemtest(abf);

        Plot plot = new();

        Color[] colors = new ScottPlot.Colormaps.Turbo().GetColors(abf.SweepCount, .1, .9);

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Trace sweepTrace = new(abf, i);
            var sig = plot.Add.Signal(sweepTrace.Values, sweepTrace.SamplePeriod);
            sig.Color = colors[i];
            sig.AlwaysUseLowDensityMode = true;
            sig.LineWidth = 1.5f;
        }

        var an = plot.Add.Annotation(
            $"Holding current: {mt.Ih:N2} pA\n" +
            $"Membrane Resistance: {mt.Rm:N2} MΩ\n" +
            $"Access Resistance: {mt.Ra:N2} MΩ\n" +
            $"Capacitance (Step): {mt.CmStep:N2} pA"
            , Alignment.UpperRight);
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