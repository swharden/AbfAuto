using AbfAuto.EventDetection;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.CommonPlots;

internal static class InVivo
{
    public static Plot GetEegFreqPlot()
    {
        Plot plot = new();
        // TODO: autoscale and detect EEG spindles
        plot.YLabel("Spindle (SPM)");
        plot.XLabel("Time (minutes)");
        return plot;
    }

    public static Plot GetEcgFreqPlot(Sweep sweep)
    {
        // TODO: autoscale and do not use hard threshold detection
        InVivoEvents events = new(sweep.SampleRate);
        events.AddIndexRange(Threshold.IndexesCrossingUp(sweep.Values.ToArray(), 4));
        (double[] bins, double[] freqs) = events.GetBinnedFrequency(sweep.Duration, 60, true);
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
        // TODO: autoscale and do not use hard threshold detection
        InVivoEvents events = new(sweep.SampleRate);
        events.AddIndexRange(Threshold.IndexesCrossingDown(sweep.Values.ToArray(), -10));
        (double[] bins, double[] freqs) = events.GetBinnedFrequency(sweep.Duration, 60, true);
        freqs = freqs.Select(x => x * 60).ToArray();

        ScottPlot.Plot plot = new();
        plot.Add.ScatterPoints(bins, freqs);

        plot.YLabel("Respiration (BPM)");
        plot.XLabel("Time (minutes)");
        plot.Axes.SetLimits(bottom: 0);

        return plot;
    }
}
