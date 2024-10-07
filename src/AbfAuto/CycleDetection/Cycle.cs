namespace AbfAuto.CycleDetection;

public readonly struct Cycle
{
    public readonly int Index1;
    public readonly int Index2;
    public readonly double SampleRate;
    public readonly double Min;
    public readonly double Max;
    public readonly double Area;
    public readonly double Amplitude => Max - Min;
    public readonly double StartTime => Index1 / SampleRate;
    public readonly double Duration => (Index2 - Index1) / SampleRate;

    public Cycle(double[] values, double sampleRate, int index1, int index2)
    {
        Index1 = index1;
        Index2 = index2;
        SampleRate = sampleRate;
        Min = values[index1..index2].Min();
        Max = values[index1..index2].Max();
        Area = values[index1..index2].Select(Math.Abs).Sum();
    }
}