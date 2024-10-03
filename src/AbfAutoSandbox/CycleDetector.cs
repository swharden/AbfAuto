using System.Collections.ObjectModel;

namespace AbfAutoSandbox;

public class CycleDetector
{
    public ReadOnlyCollection<double> OriginalTrace { get; }
    public double SampleRate { get; }
    public double SamplePeriodSec => 1.0 / SampleRate;
    public double SamplePeriodMin => SamplePeriodSec / 60.0;

    /// <summary>
    /// This array starts as <see cref="OriginalTrace"/> but its contents get manipulated by "Apply" methods in this class.
    /// This is the array used for event detection when <see cref="DetectEvents"/> is called.
    /// </summary>
    public double[] Trace { get; private set; }

    public CycleDetector(double[] values, double sampleRate)
    {
        if (values is null || values.Length == 0)
            throw new InvalidDataException(nameof(values));

        OriginalTrace = values.AsReadOnly();
        SampleRate = sampleRate;

        Trace = new double[values.Length];
        Array.Copy(values, Trace, values.Length);
    }

    public double[] GetTimes()
    {
        double[] times = new double[OriginalTrace.Count];

        for (int i = 0; i < times.Length; i++)
        {
            times[i] = i / SampleRate;
        }

        return times;
    }

    public void ApplySmoothing(double ms)
    {
        int count = (int)(SampleRate * ms / 1000);
        Trace = ArrayOperations.Smooth(Trace, count);
    }

    public void ApplySuccessiveSmoothing(double ms, int divisions = 5)
    {
        double msStep = ms / divisions;
        for (int i = 1; i <= divisions; i++)
        {
            ApplySmoothing(i * msStep);
        }
    }

    public void ApplySuccessiveDetrend(double ms)
    {
        double[] startingTrace = Trace;
        ApplySuccessiveSmoothing(ms);
        for (int i = 0; i < Trace.Length; i++)
        {
            Trace[i] = startingTrace[i] - Trace[i];
        }
    }

    public void ApplyDerivative()
    {
        for (int i = Trace.Length - 1; i > 0; i--)
        {
            Trace[i] = Trace[i] - Trace[i - 1];
        }
        Trace[0] = Trace[1];
    }

    public void ApplyRectify()
    {
        for (int i = 0; i<Trace.Length; i++)
        {
            Trace[i] = Math.Abs(Trace[i]);
        }
    }

    public Cycle[] GetDownwardCycles()
    {
        int[] starts = ArrayOperations.GetIndexesCrossingBelow(Trace, 0);
        return GetCycles(starts);
    }

    public BinnedEvents GetDownwardCyclesBinned(double binSize = 60)
    {
        Cycle[] cycles = GetDownwardCycles();
        double recordingLength = Trace.Length / SampleRate;
        return new(cycles, recordingLength, binSize);
    }

    public Cycle[] GetUpwardCycles()
    {
        int[] starts = ArrayOperations.GetIndexesCrossingAbove(Trace, 0);
        return GetCycles(starts);
    }

    public BinnedEvents GetUpwardCyclesBinned(double binSize = 60)
    {
        Cycle[] cycles = GetUpwardCycles();
        double recordingLength = Trace.Length / SampleRate;
        return new(cycles, recordingLength, binSize);
    }

    private Cycle[] GetCycles(int[] starts)
    {
        List<Cycle> cycles = [];

        for (int i = 0; i < starts.Length - 1; i++)
        {
            Cycle cycle = new(Trace, SampleRate, starts[i], starts[i + 1]);
            cycles.Add(cycle);
        }

        return [.. cycles];
    }
}
