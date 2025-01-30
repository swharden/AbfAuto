using AbfSharp;
using ScottPlot;
using AbfAuto.Evoked;

namespace AbfAuto.Analyzers;

internal class LtpMeasure : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        const int EPOCH_INDEX = 4;

        double[] peakAmplitudes = new double[abf.SweepCount];
        double[] peakTimesMinutes = new double[abf.SweepCount];

        Plot plotOverlap = new();
        plotOverlap.YLabel("Δ Current (pA)");
        plotOverlap.XLabel("Time (ms)");

        Plot plotSequential = new();
        plotSequential.YLabel("Δ Current (pA)");
        plotSequential.XLabel("Time (sequential)");
        plotSequential.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.EmptyTickGenerator();

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Evoked.EvokedSegment segment = new(abf.GetSweep(i), abf.Epochs[EPOCH_INDEX], EvokedSettings.EvokedEpsc);
            peakAmplitudes[i] = Math.Abs(segment.Min);
            peakTimesMinutes[i] = abf.SweepLength * i / 60;

            var sig = plotOverlap.Add.Signal(segment.Values, segment.SamplePeriod * 1000);
            sig.Color = Colors.C0.WithAlpha(.5);

            var sig2 = plotSequential.Add.Signal(segment.Values, segment.SamplePeriod * 1000);
            sig2.Color = Colors.C0;
            sig2.Data.XOffset = (segment.SamplePeriod * 1000 * segment.Values.Length) * i;
        }
        plotOverlap.WithSignalLineWidth(1.5).WithSignalHighQualityRendering();
        plotOverlap.Axes.Margins(0, 0.1);
        plotOverlap.Add.HorizontalLine(0, 1, Colors.Black, LinePattern.Dashed);

        plotSequential.WithSignalLineWidth(1.5).WithSignalHighQualityRendering();
        plotSequential.Add.HorizontalLine(0, 1, Colors.Black, LinePattern.Dashed);
        plotSequential.Axes.Margins(0, 0.1);

        Plot plotPeaks = new();
        var bars = plotPeaks.Add.Bars(peakTimesMinutes, peakAmplitudes);
        foreach(var bar in bars.Bars)
        {
            bar.Size = abf.SweepLength / 60;
            bar.BorderColor = Colors.Gray;
            bar.FillColor = Colors.Gray;
        }

        plotPeaks.YLabel("Evoked Current Amplitude (pA)");
        plotPeaks.XLabel("Time (minutes)");
        plotPeaks.Axes.AutoScale();
        plotPeaks.Axes.SetLimits(bottom: 0);

        MultiPlot2 mp = new();
        mp.AddSubplot(plotOverlap, 0, 2, 0, 2);
        mp.AddSubplot(plotSequential, 1, 2, 0, 2);
        mp.AddSubplot(plotPeaks, 0, 1, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
