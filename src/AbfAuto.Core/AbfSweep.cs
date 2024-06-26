namespace AbfAuto.Core;

public class AbfSweep
{
    public required double[] Values { get; init; }
    public required double SampleRate { get; init; }
    public required double StartTime { get; init; }
    public double SamplePeriod => 1.0 / SampleRate;

    public double[] GetTimes()
    {
        double[] xs = new double[Values.Length];

        for (int i = 0; i < Values.Length; i++)
        {
            xs[i] = i / SampleRate;
        }

        return xs;
    }

    public static AbfSweep FromAbf(AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        float[] values = abf.GetSweepF(sweepIndex, channelIndex);
        double[] values2 = new double[values.Length];
        Array.Copy(values, values2, values.Length);

        return new AbfSweep()
        {
            Values = values2,
            SampleRate = abf.SampleRate,
            StartTime = 0,
        };
    }

    public double GetMean(TimeRange timeRange) => GetMean(timeRange.ToIndexRange(SampleRate, StartTime));

    public double GetMean(IndexRange indexRange)
    {
        double sum = 0;

        for (int i = indexRange.MinIndex; i < indexRange.MaxIndex; i++)
        {
            sum += Values[i];
        }

        return sum / indexRange.Count;
    }

    public double GetMin(TimeRange timeRange) => GetMin(timeRange.ToIndexRange(SampleRate, StartTime));

    public double GetMin(IndexRange indexRange)
    {
        double min = double.PositiveInfinity;

        for (int i = indexRange.MinIndex; i < indexRange.MaxIndex; i++)
        {
            min = Math.Min(min, Values[i]);
        }

        return min;
    }

    public (double value, int index) GetMinValueAndIndex() => GetMinValueAndIndex(new IndexRange(0, Values.Length - 1));

    public (double value, int index) GetMinValueAndIndex(IndexRange indexRange)
    {
        double value = Values[0];
        int index = indexRange.MinIndex;

        for (int i = indexRange.MinIndex; i < indexRange.MaxIndex; i++)
        {
            if (Values[i] < value)
            {
                value = Values[i];
                index = i;
            }
        }

        return (value, index);
    }

    public AbfSweep WithSubtraction(TimeRange timeRange)
    {
        double baseline = GetMean(timeRange);
        return this.WithSubtraction((float)baseline);
    }

    public AbfSweep WithSubtraction(double value)
    {
        double[] values2 = new double[Values.Length];
        for (int i = 0; i < Values.Length; i++)
        {
            values2[i] = Values[i] - value;
        }

        return new()
        {
            Values = values2,
            SampleRate = SampleRate,
            StartTime = 0,
        };
    }

    public AbfSweep WithTrim(double time1, double time2) => WithTrim(new TimeRange(time1, time2));
    public AbfSweep WithTrim(TimeRange timeRange) => WithTrim(timeRange.ToIndexRange(SampleRate, StartTime));
    public AbfSweep WithTrim(IndexRange range)
    {
        double[] values2 = new double[range.Count];

        Array.Copy(Values, range.MinIndex, values2, 0, range.Count);

        return new AbfSweep()
        {
            Values = values2,
            SampleRate = SampleRate,
            StartTime = range.MinIndex / SampleRate,
        };
    }

    public AbfSweep WithSmoothing(int points)
    {
        // TODO: this can be significantly optimized for performance

        double[] values2 = ScottPlot.Statistics.Series.MovingAverage(Values, points, false);

        return new AbfSweep()
        {
            Values = values2,
            SampleRate = SampleRate,
            StartTime = StartTime,
        };
    }
}