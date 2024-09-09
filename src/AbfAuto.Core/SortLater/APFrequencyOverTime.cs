using AbfAuto.Core.EventDetection;
using AbfAuto.Core.Extensions;
using ScottPlot;

namespace AbfAuto.Core.SortLater;

    /*
public class APFrequencyOverTime : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        Sweep sweep = abf.GetAllData();

        Plot rawPlot = new();
        rawPlot.Add.Signal(sweep.Values, abf.SamplePeriod / 60);
        rawPlot.AddVerticalLinesAtCommentMinutes(abf);
        rawPlot.YLabel("Potential (mV)");
        rawPlot.Title($"{abf.AbfID()}: AP Frequency in 10 second bins");

        EventCollection events = sweep.DerivativeThresholdCrossings(threshold: 10, timeSec: 0.01);
        (double[] bins, double[] freqs) = events.GetBinnedFrequency(abf.AbfLength(), binSizeSec: 10);

        Plot freqPlot = new();
        freqPlot.Add.Scatter(bins, freqs);
        freqPlot.Axes.AutoScale();
        freqPlot.Axes.SetLimits(bottom: 0);
        freqPlot.AddVerticalLinesAtCommentMinutes(abf);
        freqPlot.YLabel("Frequency (Hz)");
        freqPlot.XLabel("Time (minutes)");

        MultiPlot2 mp = new();
        mp.AddSubplot(rawPlot, 0, 2, 0, 1);
        mp.AddSubplot(freqPlot, 1, 2, 0, 1);

        return AnalysisResult.WithSingleMultiPlot(mp);
    }
}
    */
