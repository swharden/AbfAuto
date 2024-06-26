namespace AbfAuto.Core;

public class AbfChannel
{
    public AbfSweep[] Sweeps { get; }
    public int SweepCount => Sweeps.Length;

    public AbfChannel(string path, int channelIndex = 0)
    {
        string abfFilePath = Path.GetFullPath(path);

        AbfSharp.ABF abf = new(abfFilePath);

        Sweeps = Enumerable
            .Range(0, abf.SweepCount)
            .Select(x => AbfSweep.FromAbf(abf, x, channelIndex))
            .ToArray();
    }
}
