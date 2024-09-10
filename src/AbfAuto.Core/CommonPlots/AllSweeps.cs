using AbfAuto.Core.Extensions;
using ScottPlot;

namespace AbfAuto.Core.CommonPlots;

public static class AllSweeps
{
    public static ScottPlot.Plot Overlapping(AbfSharp.ABF abf, double yOffset = 0, int smoothPoints = 0)
    {
        Plot plot = new();
        plot.HideGrid();

        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep sweep = abf.GetSweep2(i);

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

    public static ScottPlot.Plot Consecutive(AbfSharp.ABF abf, int channelIndex = 0)
    {
        Sweep sweep = abf.GetAllData(channelIndex);
        return Consecutive(sweep);
    }

    public static ScottPlot.Plot Consecutive(Sweep sweep)
    {
        Plot plot = new();
        plot.Add.Signal(sweep.Values, sweep.SamplePeriod / 60);
        return plot;
    }
}
