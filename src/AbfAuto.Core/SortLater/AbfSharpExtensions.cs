using AbfSharp.ABFFIO;
using ScottPlot;

namespace AbfAuto.Core;

public static class AbfSharpExtensions
{
    /*
    public static float[] GetSweepF(this AbfSharp.ABF abf, int sweepIndex, int channelIndex)
    {
        abf.GetSweepF
        throw new NotImplementedException();
        return [];
    }*/

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
        double pointsPerSweep = abf.Header.AbfFileHeader.lNumSamplesPerEpisode / abf.Header.ChannelCount;
        double sweepLength = pointsPerSweep / abf.Header.SampleRate;
        return abf.Header.SweepCount * sweepLength;
    }
}
