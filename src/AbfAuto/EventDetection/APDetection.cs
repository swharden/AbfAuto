namespace AbfAuto.EventDetection;
using AbfSharp;

public static class APDetection
{
    public static int[][] IndexesPerSweep(AbfSharp.ABF abf) // TODO: replace indexes with primitive
    {
        var settings = DerivativeThreshold.Settings.AP;

        int[][] apIndexes = new int[abf.SweepCount][];
        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep(i);
            apIndexes[i] = DerivativeThreshold.GetIndexes(sweep, settings);
        }

        return apIndexes;
    }

    public static int[][] IndexesPerSweep(AbfSharp.ABF abf, Epoch epoch)
    {
        int[][] apsPerSweep = IndexesPerSweep(abf);
        for (int i = 0; i < apsPerSweep.Length; i++)
        {
            apsPerSweep[i] = apsPerSweep[i].Where(x => x >= epoch.IndexFirst && x <= epoch.IndexLast).ToArray();
        }
        return apsPerSweep;
    }

    public static int[] CountPerSweep(AbfSharp.ABF abf, Epoch epoch)
    {
        return IndexesPerSweep(abf, epoch).Select(x => x.Length).ToArray();
    }

    public static double[] FreqPerSweep(AbfSharp.ABF abf, Epoch epoch)
    {
        return IndexesPerSweep(abf, epoch).Select(x => x.Length / epoch.Duration).ToArray();
    }

    public static double[] FreqPerSweep(AbfSharp.ABF abf)
    {
        return IndexesPerSweep(abf).Select(x => x.Length / abf.SweepLength).ToArray();
    }
}
