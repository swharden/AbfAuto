﻿using AbfSharp;

namespace AbfAuto;
public static class Extensions
{
    public static Sweep Detrend(this Sweep sweep, TimeSpan span)
    {
        int points = (int)(sweep.SampleRate * span.TotalSeconds);
        return sweep.Detrend(points);
    }

    public static Sweep SecondDerivative(this Sweep sweep)
    {
        return sweep.Derivative().Derivative();
    }

    public static Sweep SmoothHanning(this Sweep sweep, TimeSpan span)
    {
        int points = (int)(sweep.SampleRate * span.TotalSeconds);
        double[] kernel = Filter.Hanning(points);
        double[] smooth = Filter.ApplyKernel(sweep.Values, kernel);
        return sweep.WithValues(smooth);
    }

    public static Sweep WithClamp(this Sweep sweep, double min, double max)
    {
        double[] values = sweep.Values.ToArray();

        for (int i = 0; i < sweep.Values.Length; i++)
        {
            if (values[i] < min)
                values[i] = min;
            else if (values[i] > max)
                values[i] = max;
        }

        return sweep.WithValues(values);
    }

    public static Sweep ClampedAbove(this Sweep sweep, double min)
    {
        double[] values = sweep.Values.ToArray();

        for (int i = 0; i < sweep.Values.Length; i++)
        {
            if (values[i] < min)
                values[i] = min;
        }

        return sweep.WithValues(values);
    }

    public static Sweep ClampedBelow(this Sweep sweep, double max)
    {
        double[] values = sweep.Values.ToArray();

        for (int i = 0; i < sweep.Values.Length; i++)
        {
            if (values[i] > max)
                values[i] = max;
        }

        return sweep.WithValues(values);
    }
}