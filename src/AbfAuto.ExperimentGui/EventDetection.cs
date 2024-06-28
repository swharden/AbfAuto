using System.Diagnostics;

namespace AbfAuto.ExperimentGui;

public static class EventDetection
{
    public readonly struct SweepAnalysisResult
    {
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

    public static SweepAnalysisResult DetectEvents(AbfSharp.ABF Abf, int sweepIndex, double threshold)
    {
        Stopwatch sw = Stopwatch.StartNew();

        AbfSharp.Sweep sweep = Abf.GetSweep(sweepIndex).WithSmoothingMilliseconds(2).SliceTimeEnd(1);
        AbfSharp.Sweep trendline = Abf.GetSweep(sweepIndex).WithSmoothingMilliseconds(100).SliceTimeEnd(1);

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
            if (heightAboveTrendline < threshold)
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
            Elapsed = sw.Elapsed,
            SweepIntervalSec = Abf.Header.SweepLength,
            TraceLengthSec = sweep.LengthSec,
            Events = [.. eventList],
        };
    }
}
