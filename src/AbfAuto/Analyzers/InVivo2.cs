using AbfSharp;
using ScottPlot;
using AbfAuto.CycleDetection;

namespace AbfAuto.Analyzers;

/// <summary>
/// In-vivo recording using 3 channels: EEG, Respiration, and ECG
/// </summary>
internal class InVivo2 : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        // plot raw traces
        MultiPlot2 mp = new();
        mp.AddSubplot(PlotFullSweep(abf, 0, "Brain EEG"), 0, 2, 0, 3);
        mp.AddSubplot(PlotFullSweep(abf, 1, "Respiration"), 1, 2, 0, 3);

        // EEG analysis
        mp.AddSubplot(Empty(), 0, 2, 1, 3);
        mp.AddSubplot(PlotEegActivity(abf), 0, 2, 2, 3);

        // respiration analysis
        Cycle[] breaths = DetectBreaths(abf);
        breaths = DiscardSmallCycles(breaths);
        BinnedEvents binnedBreaths = new(breaths, abf.AbfLength, 60);
        mp.AddSubplot(PlotFreq(abf, binnedBreaths, "Breaths/Minute"), 1, 2, 1, 3);
        mp.AddSubplot(PlotAmp(abf, binnedBreaths, "Amplitude (%)"), 1, 2, 2, 3);

        return AnalysisResult.Single(mp)
            .WithCsvFile("breaths", binnedBreaths.ToCsv());
    }

    ScottPlot.Plot Empty()
    {
        ScottPlot.Plot plot = new();
        plot.Axes.Frameless();
        plot.HideGrid();
        return plot;
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

    ScottPlot.Plot PlotEegActivity(ABF abf, double binSec = 60, int channel = 0)
    {
        Sweep sweep = abf.GetAllData(channel)
            .Smooth(100) // remove noise
            .Detrend(500) // center at 0
            .Rectified() // make all squiggles upward
            .Smooth(10_000); // aggressive smoothing makes the trace represent "squigglyness"

        int binCount = (int)(sweep.Duration / binSec) - 1;
        double[] values = new double[binCount];
        double[] binTimes = Enumerable.Range(0, binCount).Select(x => binSec * x / 60).ToArray();
        for (int i = 0; i < binCount; i++)
        {
            int i1 = (int)(binSec * i * abf.SampleRate);
            int i2 = (int)(binSec * (i + 1) * abf.SampleRate);
            Sweep seg = sweep.SubTraceByIndex(i1, i2);
            values[i] = seg.Values.Average();
        }

        double baseline = values.Take(5).Average();
        values = values.Select(x => x / baseline * 100).ToArray();

        /*
        // Inspect the "squigglyness" trace in a pop-up window
        Plot plot2 = new();
        plot2.Add.Signal(sweep.Values);
        ScottPlot.WinForms.FormsPlotViewer.Launch(plot2);
        */

        Plot plot = new();
        plot.Add.Scatter(binTimes, values);
        plot.Add.HorizontalLine(100, 1, Colors.Black, LinePattern.DenselyDashed);
        plot.Axes.SetLimitsY(0, values.Max() * 1.1);

        return plot
            .WithYLabel("Activity (%)")
            .WithVerticalLinesAtTagTimes(abf);
    }
}
