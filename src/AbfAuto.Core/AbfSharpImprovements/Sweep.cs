﻿using ScottPlot.Colormaps;

namespace AbfAuto.Core;

public class Sweep
{
    public IReadOnlyList<double> Values { get; }
    public double SampleRate { get; }
    public double SamplePeriod { get; }
    public int SweepIndex { get; }
    public int ChannelIndex { get; }
    public double FileStartTime { get; }

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

    public static Sweep Smooth(this Sweep sweep, TimeSpan timeSpan)
    {
        int pointCount = (int)(timeSpan.TotalMilliseconds / 1000.0 / sweep.SamplePeriod);
        return sweep.Smooth(pointCount);
    }

    public static Sweep Smooth(this Sweep sweep, int pointCount)
    {
        double[] smooth = new double[sweep.Values.Count];

        double runningSum = 0;

        for (int i = 0; i < smooth.Length; i++)
        {
            runningSum += sweep.Values[i];

            if (i >= pointCount)
            {
                runningSum -= sweep.Values[i - pointCount];
                smooth[i] = runningSum / pointCount;
            }
            else
            {
                smooth[i] = runningSum / (i + 1);
            }
        }

        return sweep.WithValues(smooth);
    }
}