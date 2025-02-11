using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class LtpInduction : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plot = new();

        // plot the sweep
        Sweep sweep = abf.GetAllData(0).Smooth(TimeSpan.FromMilliseconds(2));
        plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        plot.WithSignalLineWidth(1.5).WithSignalHighQualityRendering();

        // share the stimulation region
        int epochIndex = 2;
        double pulseStart = abf.Epochs[epochIndex].StartTime;
        double pulseEnd = abf.Epochs[epochIndex].EndTime;
        var span1 = plot.Add.HorizontalSpan(pulseStart, pulseEnd);
        span1.FillColor = Colors.Yellow.WithAlpha(.1);
        span1.LineWidth = 0;

        return AnalysisResult.Single(plot);
    }
}
