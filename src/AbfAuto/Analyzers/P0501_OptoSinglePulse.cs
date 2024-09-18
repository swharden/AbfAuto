using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class P0501_OptoSinglePulse : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        // determine where the opto pulse is
        int pulseEpochIndex = 2;
        double pulseStart = abf.Epochs[pulseEpochIndex].StartTime;
        double pulseEnd = abf.Epochs[pulseEpochIndex].EndTime;
        double viewPad = 0.2;
        double viewStart = pulseStart - viewPad;
        double viewEnd = pulseStart + viewPad;
        int i1 = (int)(viewStart * abf.SampleRate);
        int i2 = (int)(viewEnd * abf.SampleRate);

        // isolate data from each sweep around the opto pulse
        double[][] segments = new double[abf.SweepCount][];
        for (int i = 0; i < abf.SweepCount; i++)
        {
            segments[i] = abf.GetSweep(i).Values[i1..i2];
        }

        // create a mean sweep
        double[] meanSegment = new double[segments[0].Length];
        for (int i = 0; i < meanSegment.Length; i++)
        {
            for (int j = 0; j < segments.Length; j++)
            {
                meanSegment[i] += segments[j][i];
            }

            meanSegment[i] /= segments.Length;
        }

        // plot things

        Plot plot1 = new();

        var span1 = plot1.Add.HorizontalSpan(pulseStart, pulseEnd);
        span1.FillColor = Colors.Orange.WithAlpha(.5);
        span1.LineWidth = 0;

        for (int i = 0; i < segments.Length; i++)
        {
            var sig1 = plot1.Add.Signal(segments[i], abf.SamplePeriod);
            sig1.Data.XOffset = viewStart;
            sig1.Data.YOffset = 100 * i;
            sig1.Color = Colors.Blue;
            sig1.LineWidth = 1.5f;
        }

        plot1.Title("All Sweeps");
        plot1.HideGrid();
        plot1.Axes.Left.TickLabelStyle.ForeColor = Colors.Transparent;
        plot1.Axes.SetLimitsX(viewStart, viewEnd);

        Plot plot2 = new();

        var span2 = plot2.Add.HorizontalSpan(pulseStart, pulseEnd);
        span2.FillColor = Colors.Orange.WithAlpha(.5);
        span2.LineWidth = 0;

        for (int i = 0; i < segments.Length; i++)
        {
            var sig2 = plot2.Add.Signal(segments[i], abf.SamplePeriod);
            sig2.Data.XOffset = viewStart;
            sig2.Color = Colors.Gray.WithAlpha(.2);
        }

        var sigMean = plot2.Add.Signal(meanSegment, abf.SamplePeriod);
        sigMean.LineWidth = 1.5f;
        sigMean.Color = Colors.Blue;
        sigMean.Data.XOffset = viewStart;

        plot2.Title("Mean Sweep");
        plot2.HideGrid();
        plot2.Axes.SetLimitsX(viewStart, viewEnd);

        MultiPlot2 mp = new();
        mp.AddSubplot(plot1, 0, 2, 0, 1);
        mp.AddSubplot(plot2, 1, 2, 0, 1);

        return AnalysisResult.Single(mp);
    }
}
