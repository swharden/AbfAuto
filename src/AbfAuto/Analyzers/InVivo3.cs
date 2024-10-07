using AbfSharp;
using ScottPlot;
using AbfAuto.CycleDetection;

namespace AbfAuto.Analyzers;

/// <summary>
/// In-vivo recording using 3 channels: EEG, Respiration, and ECG
/// </summary>
internal class InVivo3 : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        // plot raw traces
        MultiPlot2 mp = new();
        mp.AddSubplot(PlotFullSweep(abf, 0, "Brain"), 0, 3, 0, 3);
        mp.AddSubplot(PlotFullSweep(abf, 1, "Respiration"), 1, 3, 0, 3);
        mp.AddSubplot(PlotFullSweep(abf, 2, "Cardiac"), 2, 3, 0, 3);

        // plot respiration
        Cycle[] breaths = DetectBreaths(abf);
        breaths = DiscardSmallCycles(breaths);
        BinnedEvents binnedBreaths = new(breaths, abf.AbfLength, 60);
        mp.AddSubplot(PlotFreq(abf, binnedBreaths, "Breaths/Minute"), 1, 3, 1, 3);
        mp.AddSubplot(PlotAmp(abf, binnedBreaths, "Amplitude (%)"), 1, 3, 2, 3);

        // plot cardiac
        Cycle[] heartbeats = DetectHeartbeats(abf);
        heartbeats = DiscardSmallCycles(heartbeats);
        BinnedEvents binnedHeartbeats = new(heartbeats, abf.AbfLength, 60);
        mp.AddSubplot(PlotFreq(abf, binnedHeartbeats, "Beats/Minute"), 2, 3, 1, 3);
        mp.AddSubplot(PlotAmp(abf, binnedHeartbeats, "Amplitude (%)"), 2, 3, 2, 3);

        return AnalysisResult.Single(mp);
    }

    ScottPlot.Plot PlotFullSweep(ABF abf, int channel, string name)
    {
        Sweep sweep = abf.GetAllData(channel).Smooth(100).Detrend(1000);
        Plot plot = new();
        plot.Add.Signal(sweep.Values, sweep.SamplePeriod / 60);
        plot.Add.HorizontalLine(0, 1, Colors.Black, LinePattern.DenselyDashed);
        return plot
            .WithYLabel(name)
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithTightHorizontalMargins()
            .WithPercentileVerticalMargins()
            .WithNoLeftTicks();
    }

    ScottPlot.Plot PlotFreq(ABF abf, BinnedEvents events, string name)
    {
        Plot plot = new();
        double[] values = events.FreqMinutes[..^1];
        double baseline = values.Take(5).Average();
        plot.Add.Scatter(events.TimesMinutes[..^1], values);
        plot.Add.HorizontalLine(baseline, 1, Colors.Black, LinePattern.DenselyDashed);
        plot.Axes.SetLimitsY(0, events.FreqMinutes.Max() * 1.1);
        return plot
            .WithYLabel(name)
            .WithVerticalLinesAtTagTimes(abf);
    }

    ScottPlot.Plot PlotAmp(ABF abf, BinnedEvents events, string name, bool normalize = true)
    {
        Plot plot = new();
        double[] values = events.MeanAmplitude[..^1].ToArray();

        if (normalize)
        {
            double baseline = values.Take(5).Average();
            values = values.Select(x => x / baseline * 100).ToArray();
        }

        plot.Add.Scatter(events.TimesMinutes[..^1], values);

        if (normalize)
        {
            plot.Add.HorizontalLine(100, 1, Colors.Black, LinePattern.DenselyDashed);
        }

        plot.Axes.SetLimitsY(0, values.Max() * 1.1);

        return plot
            .WithYLabel(name)
            .WithVerticalLinesAtTagTimes(abf);
    }

    public static Cycle[] DiscardSmallCycles(Cycle[] cycles, double baselineSec = 60 * 5, double minFraction = 0.2)
    {
        double meanBaselineAmplitude = cycles.Where(x => x.StartTime < baselineSec).Select(x => x.Amplitude).Average();
        double thresholdAmplitude = meanBaselineAmplitude * minFraction;
        return cycles.Where(x => x.Amplitude >= thresholdAmplitude).ToArray();
    }

    public static Cycle[] DetectBreaths(ABF abf, int channelIndex = 1)
    {
        Sweep sweep = abf.GetAllData(channelIndex: channelIndex);
        CycleDetector detector = new(sweep.Values, sweep.SampleRate);
        detector.ApplySuccessiveSmoothing(300);
        detector.ApplySuccessiveDetrend(3_000);
        return detector.GetDownwardCycles();
    }

    public static Cycle[] DetectHeartbeats(ABF abf, int channelIndex = 2)
    {
        Sweep sweep = abf.GetAllData(channelIndex: 2);
        CycleDetector detector = new(sweep.Values, sweep.SampleRate);
        detector.ApplyDerivative();
        detector.ApplyDerivative();
        detector.ApplyRectify();
        double[] original = detector.Trace.ToArray();
        detector.ApplySuccessiveSmoothing(33);
        detector.ApplySuccessiveDetrend(300);
        return detector.GetDownwardCycles();
    }
}
