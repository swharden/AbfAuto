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

    public Trace(AbfSharp.ABFFIO.ABF abf, int sweepIndex, int channelIndex = 0)
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
}
