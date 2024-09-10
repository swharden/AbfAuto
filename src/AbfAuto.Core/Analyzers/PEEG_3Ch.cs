using AbfAuto.Core.EventDetection;
using AbfAuto.Core.Extensions;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

internal class PEEG_3Ch : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Sweep sweep1 = abf.GetAllData(0).Smooth(100).Detrend(1000);
        Plot ch1Full = CommonPlots.AllSweeps.Consecutive(sweep1)
            .WithYLabel("EEG")
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithTightHorizontalMargins();
        ch1Full.Axes.SetLimitsY(-.02, .02);
        Plot ch1freq = GetEegFreqPlot(sweep1).WithVerticalLinesAtTagTimes(abf);

        Sweep sweep2 = abf.GetAllData(1).Smooth(100).Detrend(1000);
        Plot ch2Full = CommonPlots.AllSweeps.Consecutive(sweep2)
            .WithYLabel("Respiration")
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithTightHorizontalMargins();
        Plot ch2freq = GetRespirationFreqPlot(sweep2).WithVerticalLinesAtTagTimes(abf);

        Sweep sweep3 = abf.GetAllData(2).Derivative().Derivative().Rectified().Smooth(10);
        Plot ch3Full = CommonPlots.AllSweeps.Consecutive(sweep3)
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithYLabel("ECG")
            .WithXLabelMinutes()
            .WithTightHorizontalMargins();
        ch3Full.Axes.SetLimitsY(0, 10);
        Plot ch3freq = GetEcgFreqPlot(sweep3).WithVerticalLinesAtTagTimes(abf);

        MultiPlot2 mp = new();
        mp.AddSubplot(ch1Full, 0, 3, 0, 2);
        mp.AddSubplot(ch1freq, 0, 3, 1, 2);
        mp.AddSubplot(ch2Full, 1, 3, 0, 2);
        mp.AddSubplot(ch2freq, 1, 3, 1, 2);
        mp.AddSubplot(ch3Full, 2, 3, 0, 2);
        mp.AddSubplot(ch3freq, 2, 3, 1, 2);

        return AnalysisResult.Single(mp);
    }

    public static Plot GetEegFreqPlot(Sweep sweep)
    {
        ScottPlot.Plot plot = new();
        plot.YLabel("Spindle (SPM)");
        plot.XLabel("Time (minutes)");
        return plot;
    }

    public static Plot GetEcgFreqPlot(Sweep sweep)
    {
        EventCollection ec = new(sweep.SampleRate);
        ec.AddIndexRange(Threshold.IndexesCrossingUp(sweep.Values.ToArray(), 4));
        (double[] bins, double[] freqs) = ec.GetBinnedFrequency(sweep.Duration, 60, true);
        freqs = freqs.Select(x => x * 60).ToArray();

        ScottPlot.Plot plot = new();
        plot.Add.ScatterPoints(bins, freqs);

        plot.YLabel("Heart Rate (BPM)");
        plot.XLabel("Time (minutes)");
        plot.Axes.SetLimitsY(0, 150);

        return plot;
    }

    public static Plot GetRespirationFreqPlot(Sweep sweep)
    {
        EventCollection ec = new(sweep.SampleRate);
        ec.AddIndexRange(Threshold.IndexesCrossingDown(sweep.Values.ToArray(), -10));
        (double[] bins, double[] freqs) = ec.GetBinnedFrequency(sweep.Duration, 60, true);
        freqs = freqs.Select(x => x * 60).ToArray();

        ScottPlot.Plot plot = new();
        plot.Add.ScatterPoints(bins, freqs);

        plot.YLabel("Respiration (BPM)");
        plot.XLabel("Time (minutes)");
        plot.Axes.SetLimits(bottom: 0);

        return plot;
    }
}
