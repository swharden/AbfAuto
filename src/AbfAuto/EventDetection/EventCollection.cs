namespace AbfAuto.EventDetection;

public class EventCollection(double sampleRate)
{
    public int Count => Indexes.Count;
    private readonly List<int> Indexes = [];

    private readonly double SampleRate = sampleRate;

    public void AddIndex(int index) => Indexes.Add(index);
    public void AddIndexRange(int[] indexes) => Indexes.AddRange(indexes);
    public void AddTime(double timeSec) => Indexes.Add((int)(timeSec * SampleRate));

    public void RemoveHighFrequencyEvents(double maxFreq) => RemoveDuplicateEvents(1.0 / maxFreq);

    private void RemoveDuplicateEvents(double minSeparationSec)
    {
        if (Indexes.Count < 2)
            return;

        int minDistance = (int)(SampleRate * minSeparationSec);

        List<int> validIndexes = [Indexes[0]];
        for (int i = 1; i < Indexes.Count; i++)
        {
            int distance = Indexes[i] - validIndexes.Last();
            if (distance >= minDistance)
            {
                validIndexes.Add(Indexes[i]);
            }
        }

        Indexes.Clear();
        Indexes.AddRange(validIndexes);
    }

    public double[] GetEventTimesSec()
    {
        return Indexes.Select(x => x / SampleRate).ToArray();
    }

    public double[] GetEventIntervalsSec()
    {
        return Enumerable
            .Range(0, Count - 1)
            .Select(x => (Indexes[x + 1] - Indexes[x]) / SampleRate)
            .ToArray();
    }

    public double[] GetEventFrequencies()
    {
        return GetEventIntervalsSec().Select(x => 1 / x).ToArray();
    }

    public (double[] bins, double[] freqs) GetBinnedFrequency(double totalLengthSec, double binSizeSec, bool minutes = true)
    {
        totalLengthSec += binSizeSec;

        int binsInAbf = (int)(totalLengthSec / binSizeSec); // last partially filled bin omitted
        double[] binStarts = Enumerable.Range(0, binsInAbf).Select(x => x * binSizeSec).ToArray();
        List<double>[] binFreqs = Enumerable.Range(0, binsInAbf).Select(x => new List<double>()).ToArray();

        double[] eventFrequencies = GetEventFrequencies();
        double[] eventTimes = GetEventTimesSec();

        for (int i = 0; i < eventFrequencies.Length; i++)
        {
            int bin = (int)(eventTimes[i] / binSizeSec);
            binFreqs[bin].Add(eventFrequencies[i]);
        }

        double binDivisor = minutes ? 60 : 1;
        double[] binTimes = binStarts.Select(x => x / binDivisor).ToArray();
        double[] binFreqMeans = binFreqs.Select(x => x.Count == 0 ? 0 : x.Average()).ToArray();

        return (binTimes, binFreqMeans);
    }
}
