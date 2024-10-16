using ScottPlot;
using AbfSharp;

namespace AbfAuto.CommonPlots;

public static class AllSweeps
{
    public static Plot Overlapping(AbfSharp.ABF abf, double yOffset = 0, int smoothPoints = 0)
    {
        Plot plot = new();
        plot.HideGrid();

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep(i);

            if (smoothPoints > 0)
            {
                sweep = sweep.Smooth(smoothPoints);
            }

            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.LineWidth = 1.5f;
            sig.Data.YOffset = yOffset * i;
        }

        return plot;
    }

    public static Plot OverlappingTimeRange(AbfSharp.ABF abf, double time1, double time2, int smoothPoints)
    {
        int i1 = (int)(time1 * abf.SampleRate);
        int i2 = (int)(time2 * abf.SampleRate);

        Plot plot = new();

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep(i).SubSweepByIndex(i1, i2);

            if (smoothPoints > 0)
                sweep = sweep.Smooth(smoothPoints);

            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.LineWidth = 1.5f;
        }

        return plot;
    }

    public static Plot Consecutive(AbfSharp.ABF abf, int channelIndex = 0)
    {
        return abf.AbfLength > 10 * 60
            ? ConsecutiveMinutes(abf, channelIndex)
            : ConsecutiveSeconds(abf, channelIndex);
    }

    public static Plot ConsecutiveSeconds(AbfSharp.ABF abf, int channelIndex = 0)
    {
        return ConsecutiveSeconds(abf.GetAllData(channelIndex));
    }

    public static Plot ConsecutiveSeconds(AbfSharp.Sweep sweep)
    {
        Plot plot = new();
        plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        plot.XLabel("Time (seconds)");
        plot.WithTightHorizontalMargins();
        return plot;
    }

    public static Plot ConsecutiveMinutes(AbfSharp.ABF abf, int channelIndex = 0)
    {
        return ConsecutiveMinutes(abf.GetAllDataDecimated(channelIndex));
    }

    public static Plot ConsecutiveMinutes(Sweep sweep)
    {
        Plot plot = new();
        plot.Add.Signal(sweep.Values, sweep.SamplePeriod / 60);
        plot.XLabel("Time (minutes)");
        plot.WithTightHorizontalMargins();
        return plot;
    }
}
