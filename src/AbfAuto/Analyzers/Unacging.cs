using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class Uncaging : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plot = new();

        Epoch epoch = abf.Epochs[4];

        double padSec = 0.5;
        int padPoints = (int)(padSec * abf.SampleRate);
        int i1 = epoch.IndexFirst - padPoints;
        int i2 = epoch.IndexFirst + padPoints;

        for (int i = 0; i < abf.SweepCount; i++)
        {
            var sweep = abf.GetSweep(i);
            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Data.MinimumIndex = i1;
            sig.Data.MaximumIndex = i2;
        }

        plot.Add.VerticalLine(epoch.StartTime, 2, Colors.Red.WithAlpha(.5));
        plot.Axes.Margins(0, 0.1);

        return AnalysisResult.Single(plot);
    }
}
