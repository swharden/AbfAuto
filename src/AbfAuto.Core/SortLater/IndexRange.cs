namespace AbfAuto.Core.SortLater;
public readonly record struct IndexRange(int Index1, int Index2)
{
    public TimeRange ToTimeRange(double SampleRate) => new()
    {
        Time1 = Index1 / SampleRate,
        Time2 = Index2 / SampleRate,
    };

    public int MinIndex => Math.Min(Index1, Index2);
    public int MaxIndex => Math.Max(Index1, Index2);
    public int Count => MaxIndex - MinIndex;
}