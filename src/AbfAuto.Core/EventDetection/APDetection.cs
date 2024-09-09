namespace AbfAuto.Core.EventDetection;

public static class APDetection
{
    public static int[][] GetApIndexesPerSweep(AbfSharp.ABF abf)
    {
        var settings = DerivativeThreshold.Settings.AP;

        int[][] apIndexes = new int[abf.SweepCount][];
        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep2(i);
            apIndexes[i] = DerivativeThreshold.GetIndexes(sweep, settings);
        }

        return apIndexes;
    }
}
