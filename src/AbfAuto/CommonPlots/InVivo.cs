using AbfAuto.EventDetection;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.CommonPlots;

internal static class InVivo
{
    public static Plot GetEegFreqPlot(Sweep sweep)
    {
        ScottPlot.Plot plot = new();
        plot.YLabel("Spindle (SPM)");
        plot.XLabel("Time (minutes)");
        return plot;
    }

    public static Plot GetEcgFreqPlot(Sweep sweep)
    {
        EventCollection ec = new(sweep.SampleRate);
        ec.AddIndexRange(Threshold.IndexesCrossingUp(sweep.Values.ToArray(), 4));
        (double[] bins, double[] freqs) = ec.GetBinnedFrequency(sweep.Duration, 60, true);
        freqs = freqs.Select(x => x * 60).ToArray();

        ScottPlot.Plot plot = new();
        plot.Add.ScatterPoints(bins, freqs);

        plot.YLabel("Heart Rate (BPM)");
        plot.XLabel("Time (minutes)");
        plot.Axes.SetLimitsY(0, 150);

        return plot;
    }

    public static Plot GetRespirationFreqPlot(Sweep sweep)
    {
        EventCollection ec = new(sweep.SampleRate);
        ec.AddIndexRange(Threshold.IndexesCrossingDown(sweep.Values.ToArray(), -10));
        (double[] bins, double[] freqs) = ec.GetBinnedFrequency(sweep.Duration, 60, true);
        freqs = freqs.Select(x => x * 60).ToArray();

        ScottPlot.Plot plot = new();
        plot.Add.ScatterPoints(bins, freqs);

        plot.YLabel("Respiration (BPM)");
        plot.XLabel("Time (minutes)");
        plot.Axes.SetLimits(bottom: 0);

        return plot;
    }
}
