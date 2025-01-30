using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class LtpMeasure : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        double[] peakAmplitudes = new double[abf.SweepCount];
        double[] peakTimesSec = ScottPlot.Generate.Consecutive(abf.SweepCount, abf.SweepLength);
        double[] peakTimesMinutes = peakTimesSec.Select(x => x / 60).ToArray();

        double stimTime = abf.Epochs[4].StartTime;

        double baselineStartTime = stimTime - 0.5;
        double baselineEndTime = stimTime - 0.05;

        double measureStartTime = stimTime + 0.004;
        double measureEndTime = measureStartTime + 0.020;

        double viewStartTime = measureStartTime - 0.05;
        double viewEndTime = viewStartTime + 0.15;

        int stimIndex = (int)(stimTime * abf.SampleRate);
        int viewStartIndex = (int)(viewStartTime * abf.SampleRate);
        int viewEndIndex = (int)(viewEndTime * abf.SampleRate);

        Plot plotSweep = new();
        plotSweep.YLabel("Δ Current (pA)");
        plotSweep.XLabel("Time (seconds)");

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep(i).SmoothHanning(TimeSpan.FromMilliseconds(1));
            double baselineMean = sweep.MeanOfTimeRange(baselineStartTime, baselineEndTime);
            sweep.SubtractInPlace(baselineMean);
            sweep = sweep.SubSweepByIndex(viewStartIndex, viewEndIndex);

            peakAmplitudes[i] = Math.Abs(sweep.Values.Min());

            var sig = plotSweep.Add.Signal(sweep.Values, sweep.SamplePeriod * 1000);
            sig.Color = Colors.C0.WithAlpha(.5);
            plotSweep.WithSignalLineWidth(1.5).WithSignalHighQualityRendering();
        }

        plotSweep.Add.HorizontalLine(0, 1, Colors.Black, LinePattern.Dashed);

        plotSweep.Axes.AutoScale();
        AxisLimits limits = plotSweep.Axes.GetDataLimits();

        plotSweep.Axes.SetLimitsX(limits.Left, limits.Right);
        plotSweep.Axes.SetLimitsY(limits.Bottom * 1.02, limits.Top * 0.1);

        Plot plotPeaks = new();
        plotPeaks.Add.Scatter(peakTimesMinutes, peakAmplitudes);
        plotPeaks.YLabel("Amplitude (pA)");
        plotPeaks.XLabel("Time (minutes)");
        plotPeaks.Axes.AutoScale();
        plotPeaks.Axes.SetLimitsY(0, plotPeaks.Axes.GetLimits().Top);

        MultiPlot2 mp = new();
        mp.AddSubplot(plotSweep, 0, 1, 0, 2);
        mp.AddSubplot(plotPeaks, 0, 1, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
