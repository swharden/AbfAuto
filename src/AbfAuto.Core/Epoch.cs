namespace AbfAuto.Core;

public class Epoch
{
    public int EpochIndex { get; }
    public string EpochName => ((char)('A' + (char)EpochIndex)).ToString();
    public int EpochTypeCode { get; }
    public double Level { get; }
    public int IndexFirst { get; }
    public int IndexLast { get; }
    public int PulsePeriodSamples { get; }
    public int PulseWidthSamples { get; }
    public double SamplePeriod { get; }
    public double StartTime => SamplePeriod * IndexFirst;
    public double EndTime => SamplePeriod * IndexLast;

    public Epoch(AbfSharp.ABFFIO.ABF abf, int epochIndex)
    {
        EpochIndex = epochIndex;

        int totalRecordingLength = abf.Header.lActualAcqLength;
        int sweepCount = abf.Header.lActualEpisodes;
        int sweepLength = totalRecordingLength / sweepCount;
        int firstEpochIndex = sweepLength / 64;

        int previousEpochDurations = Enumerable
            .Range(0, epochIndex)
            .Select(x => abf.Header.lEpochInitDuration[x])
            .Sum();

        int i1 = firstEpochIndex + previousEpochDurations;
        int i2 = i1 + abf.Header.lEpochInitDuration[epochIndex];

        IndexFirst = i1;
        IndexLast = i2;
        Level = abf.Header.fEpochInitLevel[epochIndex];
        SamplePeriod = abf.SamplePeriod;
        EpochTypeCode = abf.Header.nEpochType[epochIndex];

        PulsePeriodSamples = abf.Header.lEpochPulsePeriod[epochIndex];
        PulseWidthSamples = abf.Header.lEpochPulseWidth[epochIndex];
    }
}
