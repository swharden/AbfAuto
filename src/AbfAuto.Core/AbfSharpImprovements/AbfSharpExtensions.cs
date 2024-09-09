namespace AbfAuto.Core;

public static class AbfSharpExtensions
{
    public static Sweep GetSweep2(this AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        return new Sweep(abf, sweepIndex, channelIndex);
    }

    public static double[] GetSweepD(this AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        float[] values = abf.GetSweep(sweepIndex, channelIndex);
        double[] values2 = new double[values.Length];
        for (int i = 0; i < values2.Length; i++)
        {
            values2[i] = values[i];
        }
        return values2;
    }

    public static string ProtocolPath(this AbfSharp.ABF abf)
    {
        return abf.Header.AbfFileHeader.sProtocolPath;
    }

    public static string Protocol(this AbfSharp.ABF abf)
    {
        return Path.GetFileName(abf.ProtocolPath());
    }

    public static string AbfID(this AbfSharp.ABF abf)
    {
        return Path.GetFileNameWithoutExtension(abf.FilePath);
    }

    public static double AbfLength(this AbfSharp.ABF abf)
    {
        return abf.Header.SweepCount * abf.SweepLength();
    }

    public static double SweepLength(this AbfSharp.ABF abf)
    {
        return abf.Header.AbfFileHeader.lNumSamplesPerEpisode / abf.Header.ChannelCount / abf.Header.SampleRate;
    }

    public static Epoch[] GetEpochs(this AbfSharp.ABF abf)
    {
        return Enumerable.Range(0, abf.Header.AbfFileHeader.fEpochInitLevel.Length)
            .Select(x => new Epoch(abf, x))
            .ToArray();
    }

    public static Epoch GetEpoch(this AbfSharp.ABF abf, int index)
    {
        return new Epoch(abf, index);
    }
}
