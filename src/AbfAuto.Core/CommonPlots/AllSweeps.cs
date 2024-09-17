using ScottPlot;
using AbfSharp;

namespace AbfAuto.Core.CommonPlots;

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
        return ConsecutiveMinutes(abf.GetAllData(channelIndex));
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
