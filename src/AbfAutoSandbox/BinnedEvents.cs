namespace AbfAutoSandbox;

public readonly struct BinnedEvents
{
    public readonly int[] Counts;
    public readonly double[] Times;
    public readonly double[] Freqs;
    public readonly double[] FreqMinutes;
    public readonly Cycle[] AllCycles;

    public BinnedEvents(Cycle[] cycles, double recordingLength, double binSize)
    {
        AllCycles = cycles;

        int binCount = (int)Math.Ceiling(recordingLength / binSize);
        Times = Enumerable.Range(0, binCount).Select(x => x * binSize / 60).ToArray();

        List<Cycle>[] binnedCycles = Enumerable.Range(0, binCount).Select(x => new List<Cycle>()).ToArray();

        foreach (Cycle cycle in cycles)
        {
            int binIndex = (int)(cycle.StartTime / binSize);
            binnedCycles[binIndex].Add(cycle);
        }

        Counts = binnedCycles.Select(x => x.Count).ToArray();
        Freqs = binnedCycles.Select(x => GetFreq(x, binSize)).ToArray();
        FreqMinutes = Freqs.Select(x => x * 60).ToArray();
    }

    private static double GetFreq(List<Cycle> cycles, double binSize)
    {
        if (cycles.Count == 0)
            return 0;

        if (cycles.Count == 1)
            return 1.0 / binSize;

        // events per second
        return (cycles.Count - 1) / (cycles.Last().StartTime - cycles.First().StartTime);
    }
}
