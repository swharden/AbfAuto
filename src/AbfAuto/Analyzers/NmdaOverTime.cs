using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class NmdaOverTime : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        double[] levelsA = new double[abf.SweepCount];
        double[] levelsB = new double[abf.SweepCount];
        double[] levelsC = new double[abf.SweepCount];

        for (int i = 0; i < abf.SweepCount; i++)
        {
            var sweep = abf.GetSweep(i);
            levelsA[i] = sweep.SubTraceByEpoch(abf.Epochs[4]).Values.Average();
            levelsB[i] = sweep.SubTraceByEpoch(abf.Epochs[6]).Values.Average();
            levelsC[i] = sweep.SubTraceByEpoch(abf.Epochs[8]).Values.Average();
        }

        Plot plot = new();

        var spC = plot.Add.Scatter(abf.SweepStartTimes, levelsC);
        spC.LegendText = $"{abf.Epochs[8].Level} pA";

        var spB = plot.Add.Scatter(abf.SweepStartTimes, levelsB);
        spB.LegendText = $"{abf.Epochs[6].Level} pA";

        var spA = plot.Add.Scatter(abf.SweepStartTimes, levelsA);
        spA.LegendText = $"{abf.Epochs[4].Level} pA";

        plot.ShowLegend();

        return AnalysisResult.Single(plot);
    }
}
