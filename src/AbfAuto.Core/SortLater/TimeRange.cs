namespace AbfAuto.Core.SortLater;

public readonly record struct TimeRange(double Time1, double Time2)
{
    public IndexRange ToIndexRange(double SampleRate, double sweepStart) => new()
    {
        Index1 = (int)((Time1 - sweepStart) * SampleRate),
        Index2 = (int)((Time2 - sweepStart) * SampleRate),
    };

    public double MinTime => Math.Min(Time1, Time2);
    public double MaxTime => Math.Max(Time1, Time2);
    public double CenterTime => (Time1 + Time2) / 2;
}