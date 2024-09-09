using AbfAuto.Core.Extensions;
using AbfAuto.Core.SortLater;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class P0202_IV : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        TimeRange measureRange = new(2.4, 2.5);
        double[] currents = new double[abf.SweepCount];
        double[] voltages = Generate.Consecutive(abf.SweepCount, 10, -110);
        Color[] colors = new ScottPlot.Colormaps.Turbo().GetColors(abf.SweepCount, .1, .9);

        Plot plot1 = new();

        for (int i = 0; i < abf.SweepCount; i++)
        {
            AbfSweep sweep = AbfSweep.FromAbf(abf, i).WithSmoothing(200);
            var sig = plot1.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Color = colors[i].WithAlpha(.8);
            sig.LineWidth = 2;

            currents[i] = sweep.GetMean(measureRange);
        }

        plot1.XLabel("Sweep Time (sec)");
        plot1.YLabel("Current (pA)");
        plot1.Axes.Margins(horizontal: 0);

        Plot plot2 = new();
        var sp = plot2.Add.Scatter(voltages, currents);
        sp.LineWidth = 2;
        sp.MarkerSize = 10;
        plot2.XLabel("Membrane Potential (mV)");
        plot2.YLabel("Current (pA)");

        AnalysisResult result = new();
        result.Plots.Add(new SizedPlot(plot1, new PixelSize(800, 600)));
        result.Plots.Add(new SizedPlot(plot2, new PixelSize(800, 600)));

        return result;
    }
}
