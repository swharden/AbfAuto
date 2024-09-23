using AbfAuto.Memtest;
using ScottPlot;

namespace AbfAuto.Analyzers;

/// <summary>
/// Show memtest properties over time
/// </summary>
internal class MemtestRepeated : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        MemtestResult?[] mts = MemtestLogic.GetMemtestBySweep(abf);
        int[] validIndexes = Enumerable.Range(0, mts.Length).Where(x => mts[x] is not null).ToArray();

        double[] sweepTimes2 = validIndexes.Select(x => abf.SweepStartTimes[x] / 60).ToArray();
        MemtestResult[] mts2 = validIndexes.Select(x => (MemtestResult)mts[x]!).ToArray();

        Plot plotIh = new();
        var spIh = plotIh.Add.ScatterPoints(sweepTimes2, mts2.Select(x => x.Ih).ToArray());
        spIh.Color = Colors.Blue;
        plotIh.WithXLabelMinutes();
        plotIh.WithVerticalLinesAtTagTimes(abf);
        plotIh.Title("Holding Current");
        plotIh.Axes.AutoScale();
        plotIh.YLabel("Current (pA)");

        Plot plotRm = new();
        var spRm = plotRm.Add.ScatterPoints(sweepTimes2, mts2.Select(x => x.Rm).ToArray());
        spRm.Color = Colors.Red;
        plotRm.WithXLabelMinutes();
        plotRm.WithVerticalLinesAtTagTimes(abf);
        plotRm.Title("Membrane Resistance");
        plotRm.Axes.AutoScale();
        plotRm.Axes.SetLimits(bottom: 0);
        plotRm.YLabel("Resistance (MΩ)");

        Plot plotRa = new();
        var spRa = plotRa.Add.ScatterPoints(sweepTimes2, mts2.Select(x => x.Ra).ToArray());
        spRa.Color = Colors.Black;
        plotRa.WithXLabelMinutes();
        plotRa.WithVerticalLinesAtTagTimes(abf);
        plotRa.Title("Access Resistance");
        plotRa.Axes.AutoScale();
        plotRa.Axes.SetLimits(bottom: 0);
        plotRa.YLabel("Resistance (MΩ)");

        Plot plotFull = CommonPlots.AllSweeps.Consecutive(abf);
        plotFull.WithVerticalLinesAtTagTimes(abf);
        plotFull.Title("Full Recording");
        plotFull.YLabel("Current (pA)");

        MultiPlot2 mp = new();
        mp.AddSubplot(plotIh, 0, 2, 0, 2);
        mp.AddSubplot(plotRm, 0, 2, 1, 2);
        mp.AddSubplot(plotRa, 1, 2, 0, 2);
        mp.AddSubplot(plotFull, 1, 2, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
