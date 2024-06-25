namespace AbfAuto.Core.Analyses;

public class P0201_Memtest : IAnalysis
{
    public Multiplot Analyze(AbfSharp.ABFFIO.ABF abf)
    {
        ScottPlot.Color[] colors = new ScottPlot.Colormaps.Turbo().GetColors(abf.SweepCount, .1, .9);

        ScottPlot.Plot plot = new();

        for (int i = 0; i < abf.SweepCount; i++)
        {
            AbfSweep sweep = AbfSweep.FromAbf(abf, i);
            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Color = colors[i].WithAlpha(.8);
            sig.LineWidth = 2;
        }

        plot.XLabel("Sweep Time (sec)");
        plot.YLabel("Current (pA)");
        plot.Axes.Margins(horizontal: 0);

        return Multiplot.WithSinglePlot(plot, 600, 400);
    }
}