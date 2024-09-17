using AbfAuto.Core.EventDetection;

namespace AbfAuto.Core.SortLater;

public class Trace
{
    public double[] Values { get; }
    public double SamplePeriod { get; }
    public double SampleRate => 1 / SamplePeriod;
    public double StartTimeInSweep { get; set; } = 0;
    public double StartTimeInFile { get; set; } = 0;

    public Trace(double[] values, double samplePeriod)
    {
        Values = values;
        SamplePeriod = samplePeriod;
    }

    public Trace(AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        float[] values = abf.GetSweep(sweepIndex, channelIndex);
        Values = new double[values.Length];
        Array.Copy(values, Values, values.Length);
        SamplePeriod = abf.SamplePeriod;
    }

    public Trace SubTraceByIndex(int i1, int i2)
    {
        int length = i2 - i1;
        double[] values = new double[length];
        Array.Copy(Values, i1, values, 0, length);
        return new Trace(values, SamplePeriod);
    }

    public Trace SubTraceByFraction(double frac1, double frac2)
    {
        int i1 = Math.Clamp((int)(frac1 * Values.Length), 0, Values.Length - 1);
        int i2 = Math.Clamp((int)(frac2 * Values.Length), 0, Values.Length - 1);
        return SubTraceByIndex(i1, i2);
    }

    public Trace SubTraceByEpoch(Epoch epoch)
    {
        return SubTraceByIndex(epoch.IndexFirst, epoch.IndexLast);
    }

    public double Mean()
    {
        return Values.Average();
    }

    public double Min()
    {
        return Values.Min();
    }

    public double Max()
    {
        return Values.Max();
    }

    public Trace Derivative(int delta = 1)
    {
        double[] deriv = new double[Values.Length - delta];
        for (int i = 0; i < deriv.Length; i++)
        {
            deriv[i] = Values[i + delta] - Values[i];
        }

        return new Trace(deriv, SamplePeriod);
    }

    public EventCollection DerivativeThresholdCrossings(double threshold = 10, double timeSec = 0.010)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        int derivativePoints = (int)(SampleRate * timeSec);
        Trace deriv = Derivative(derivativePoints);
        EventCollection events = GetIndexesRising(deriv.Values, threshold);
        events.RemoveHighFrequencyEvents(100);

        Console.WriteLine(
            $"Detected {events.Count:N2} events " +
            $"from a {SamplePeriod * Values.Length / 60:N2} minute ABF " +
            $"in {sw.Elapsed.TotalSeconds:N2} sec");

        return events;
    }

    public EventCollection GetIndexesRising(double[] values, double threshold)
    {
        EventCollection events = new(SampleRate);

        bool isAbove = false;
        for (int i = 0; i < values.Length; i++)
        {
            // just crossed above
            if (values[i] > threshold && !isAbove)
            {
                isAbove = true;
                events.AddIndex(i);
                continue;
            }

            // just fell below
            if (values[i] < threshold && isAbove)
            {
                isAbove = false;
            }
        }

        return events;
    }

    public void SubtractInPlace(double value)
    {
        for (int i = 0; i < Values.Length; i++)
        {
            Values[i] -= value;
        }
    }

    public Trace SmoothedMsec(double msec)
    {
        int pointCount = (int)(msec / 1000 / SamplePeriod);
        return SmoothedPoints(pointCount);
    }

    public Trace SmoothedPoints(int pointCount)
    {
        double[] smooth = new double[Values.Length];

        double runningSum = 0;

        for (int i = 0; i < smooth.Length; i++)
        {
            runningSum += Values[i];

            if (i >= pointCount)
            {
                runningSum -= Values[i - pointCount];
                smooth[i] = runningSum / pointCount;
            }
            else
            {
                smooth[i] = runningSum / (i + 1);
            }
        }

        return new Trace(smooth, SamplePeriod);
    }

    public int GetMaximumIndex()
    {
        int maxIndex = 0;
        double maxValue = double.NegativeInfinity;
        for (int i = 0; i < Values.Length; i++)
        {
            if (Values[i] > maxValue)
            {
                maxValue = Values[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    public int GetFirstIndexBelow(double target, int firstIndex = 0)
    {
        for (int i = firstIndex; i < Values.Length; i++)
        {
            if (Values[i] < target)
                return i;
        };

        throw new InvalidOperationException("values never went below target");
    }
}
