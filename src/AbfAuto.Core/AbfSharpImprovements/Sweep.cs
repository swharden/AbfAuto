using ScottPlot.Colormaps;
using ScottPlot.TickGenerators.TimeUnits;

namespace AbfAuto.Core;

public class Sweep
{
    public IReadOnlyList<double> Values { get; }
    public double SampleRate { get; }
    public double SamplePeriod { get; }
    public int SweepIndex { get; }
    public int ChannelIndex { get; }
    public double FileStartTime { get; }
    public double Duration => Values.Count / SampleRate;

    public Sweep(IReadOnlyList<double> values, double sampleRate, int sweepIndex, int channelIndex, double fileStartTime)
    {
        Values = values;
        SampleRate = sampleRate;
        SamplePeriod = 1.0 / sampleRate;
        SweepIndex = sweepIndex;
        ChannelIndex = channelIndex;
        FileStartTime = fileStartTime;
    }

    public Sweep(AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        float[] valuesF = abf.GetSweep(sweepIndex, channelIndex);
        double[] values = new double[valuesF.Length];
        for (int i = 0; i < valuesF.Length; i++)
        {
            values[i] = valuesF[i];
        }

        Values = values;
        SampleRate = abf.SampleRate;
        SamplePeriod = abf.SamplePeriod;
        SweepIndex = sweepIndex;
        ChannelIndex = channelIndex;
        FileStartTime = abf.SweepLength() * sweepIndex;
    }
}

public static class SweepExtensions
{
    public static Sweep WithValues(this Sweep sweep, double[] newValues)
    {
        return new Sweep(newValues, sweep.SampleRate, sweep.SweepIndex, sweep.ChannelIndex, sweep.FileStartTime);
    }

    public static Sweep WithStartTime(this Sweep sweep, double newStartTime)
    {
        return new Sweep(sweep.Values, sweep.SampleRate, sweep.SweepIndex, sweep.ChannelIndex, newStartTime);
    }

    public static Sweep WithSampleRate(this Sweep sweep, double newSampleRate)
    {
        return new Sweep(sweep.Values, newSampleRate, sweep.SweepIndex, sweep.ChannelIndex, sweep.FileStartTime);
    }

    public static Sweep SubSweepByIndex(this Sweep sweep, int i1, int i2)
    {
        double[] newValues = sweep.Values.Skip(i1).Take(i2 - i1).ToArray();
        double newStartTime = sweep.FileStartTime + i1 * sweep.SamplePeriod;
        return sweep.WithValues(newValues).WithStartTime(newStartTime);
    }

    public static Sweep Derivative(this Sweep sweep, int delta = 1)
    {
        double[] newValues = new double[sweep.Values.Count - delta];
        for (int i = 0; i < newValues.Length; i++)
        {
            newValues[i] = sweep.Values[i + delta] - sweep.Values[i];
        }

        return sweep.WithValues(newValues);
    }

    public static Sweep Rectified(this Sweep sweep)
    {
        double[] newValues = new double[sweep.Values.Count];

        for (int i = 0; i < newValues.Length; i++)
        {
            newValues[i] = Math.Abs(sweep.Values[i]);
        }

        return sweep.WithValues(newValues);
    }

    public static Sweep Smooth(this Sweep sweep, TimeSpan timeSpan)
    {
        int pointCount = (int)(timeSpan.TotalMilliseconds / 1000.0 / sweep.SamplePeriod);
        return sweep.Smooth(pointCount);
    }

    public static Sweep Smooth(this Sweep sweep, int pointCount)
    {
        double[] smooth = new double[sweep.Values.Count];

        // smooth from left to right
        double runningSum = 0;
        int pointsInSum = 0;
        for (int i = 0; i < smooth.Length; i++)
        {
            runningSum += sweep.Values[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= sweep.Values[i - pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // smooth from right to left
        runningSum = 0;
        pointsInSum = 0;
        for (int i = smooth.Length - 1; i >= 0; i--)
        {
            runningSum += sweep.Values[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= sweep.Values[i + pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // average the two directions
        for (int i = 0; i < smooth.Length; i++)
        {
            smooth[i] /= 2;
        }

        return sweep.WithValues(smooth);
    }

    public static Sweep Detrend(this Sweep sweep, int pointCount)
    {
        Sweep trend = sweep.Smooth(pointCount);

        double[] values2 = new double[sweep.Values.Count];
        for (int i = 0; i < values2.Length; i++)
        {
            values2[i] = sweep.Values[i] - trend.Values[i];
        }

        return sweep.WithValues(values2);
    }

    public static Sweep Decimate(this Sweep sweep, int count)
    {
        int length = sweep.Values.Count / count;
        double[] values = new double[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = sweep.Values[i * count];
        }

        return sweep.WithValues(values).WithSampleRate(sweep.SampleRate / count);
    }
}
