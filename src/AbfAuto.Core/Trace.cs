namespace AbfAuto.Core;

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

    public int[] DerivativeThresholdCrossings(double threshold = 10, double timeSec = 0.010)
    {
        int derivativePoints = (int)(SampleRate * timeSec);
        Trace deriv = Derivative(derivativePoints);
        int[] indexes = GetIndexesRising(deriv.Values, threshold);
        indexes = RemoveDoublets(indexes);
        return indexes;
    }

    public int[] RemoveDoublets(int[] values, double minSeparationSec = 0.01)
    {
        if (values.Length == 0)
            return [];

        int minDistance = (int)(SampleRate * minSeparationSec);

        List<int> values2 = [values[0]];
        for (int i = 1; i < values.Length; i++)
        {
            int distance = values[i] - values2.Last();
            if (distance >= minDistance)
            {
                values2.Add(values[i]);
            }
        }

        return values2.ToArray();
    }

    public static int[] GetIndexesRising(double[] values, double threshold)
    {
        List<int> indexes = [];

        bool isAbove = false;
        for (int i = 0; i < values.Length; i++)
        {
            // just crossed above
            if (values[i] > threshold && !isAbove)
            {
                isAbove = true;
                indexes.Add(i);
                continue;
            }

            // just fell below
            if (values[i] < threshold && isAbove)
            {
                isAbove = false;
            }
        }

        return indexes.ToArray();
    }
}
