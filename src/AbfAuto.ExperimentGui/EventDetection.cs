using ScottPlot;
using System.Diagnostics;
using static AbfAuto.ExperimentGui.EventDetection;

namespace AbfAuto.ExperimentGui;

public static class EventDetection
{
    public readonly struct Settings
    {
        public required double Threshold { get; init; }
        public required double SmoothingMsec { get; init; }
        public required double TrendlineMsec { get; init; }
    }

    public readonly struct SweepAnalysisResult
    {
        public required Settings Settings { get; init; }
        public required TimeSpan Elapsed { get; init; }
        public required double SweepIntervalSec { get; init; }
        public required double TraceLengthSec { get; init; }
        public required EphysEvent[] Events { get; init; }
        public double MeanFrequency => Events.Length / TraceLengthSec;
        public double MeanAmplitude => Events.Length == 0
            ? double.NaN
            : Events.Select(x => x.HeightAboveTrendline).Average();
    }

    public readonly struct EphysEvent
    {
        public required int Index { get; init; }
        public required double Time { get; init; }
        public required double Value { get; init; }
        public required double HeightAboveTrendline { get; init; }
    }

    public static SweepAnalysisResult DetectEvents(AbfSharp.ABF Abf, int sweepIndex, Settings settings)
    {
        Stopwatch sw = Stopwatch.StartNew();

        AbfSharp.Sweep sweep = Abf.GetSweep(sweepIndex).WithSmoothingMilliseconds(settings.SmoothingMsec).SliceTimeEnd(1);
        AbfSharp.Sweep trendline = Abf.GetSweep(sweepIndex).WithSmoothingMilliseconds(settings.TrendlineMsec).SliceTimeEnd(1);

        List<EphysEvent> eventList = [];
        for (int i = 0; i < sweep.Values.Length; i++)
        {
            // proceed while the trace is below the trendline
            if (sweep.Values[i] <= trendline.Values[i])
                continue;

            // find peak in the segment above the trendline
            double trendlineValue = trendline.Values[i];
            int maxIndex = i;
            double maxValue = sweep.Values[maxIndex];
            while (i++ < sweep.Values.Length - 1)
            {
                if (sweep.Values[i] <= trendline.Values[i])
                    break;

                if (sweep.Values[i] > maxValue)
                {
                    maxIndex = i;
                    maxValue = sweep.Values[i];
                }
            }

            double heightAboveTrendline = maxValue - trendlineValue;
            if (heightAboveTrendline < settings.Threshold)
                continue;

            EphysEvent ev = new()
            {
                Index = maxIndex,
                Time = maxIndex / sweep.SampleRate,
                Value = maxValue,
                HeightAboveTrendline = heightAboveTrendline,
            };

            eventList.Add(ev);
        }

        sw.Stop();

        return new SweepAnalysisResult()
        {
            Settings = settings,
            Elapsed = sw.Elapsed,
            SweepIntervalSec = Abf.Header.SweepLength,
            TraceLengthSec = sweep.LengthSec,
            Events = [.. eventList],
        };
    }

    public static Plot PlotFreqOverTime(SweepAnalysisResult[] results)
    {
        Plot plot = new();
        double[] xs = Enumerable.Range(0, results.Length).Select(x => x * results[x].SweepIntervalSec).ToArray();
        double[] ys = results.Select(x => x.MeanFrequency).ToArray();
        var sp = plot.Add.Scatter(xs, ys);
        sp.LineWidth = 2;
        plot.YLabel("Frequency (Hz)");
        plot.XLabel("Time (min)");
        return plot;
    }

    public static Plot PlotAllTraces(AbfSharp.ABF Abf, SweepAnalysisResult[] results, double yOffset = 100)
    {
        Plot plot = new();

        for (int i = 0; i < Abf.SweepCount; i++)
        {
            AbfSharp.Sweep sweep = Abf.GetSweep(i).WithSmoothingMilliseconds(results[i].Settings.SmoothingMsec).SliceTimeEnd(1);
            AbfSharp.Sweep trendline = Abf.GetSweep(i).WithSmoothingMilliseconds(results[i].Settings.TrendlineMsec).SliceTimeEnd(1);

            // plot traces
            var sigSweep = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sigSweep.Color = Colors.C0;
            sigSweep.Data.YOffset = yOffset * i;

            // plot peaks
            double[] peakXs = results[i].Events.Select(x => x.Time).ToArray();
            double[] peakYs = results[i].Events.Select(x => x.Value + yOffset * i).ToArray();
            var peakMarker = plot.Add.Markers(peakXs, peakYs);
            peakMarker.MarkerShape = MarkerShape.Asterisk;
            peakMarker.Color = Colors.Red;

        }

        return plot;
    }
}
