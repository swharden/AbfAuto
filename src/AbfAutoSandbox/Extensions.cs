using AbfSharp;
using ScottPlot;

namespace AbfAutoSandbox;

public static class Extensions
{
    public static void LaunchPlot(this CycleDetector detector)
    {
        Plot plot = new();

        //var sig1 = plot.Add.Signal(detector.OriginalTrace, detector.SamplePeriodMin);
        //sig1.Color = Colors.Gray.WithAlpha(.5);

        var sig2 = plot.Add.Signal(detector.Trace, detector.SamplePeriodMin);
        sig2.Color = Colors.Black;
        sig2.LineWidth = 2;

        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }

    public static void LaunchPlot(this CycleDetector detector, double[] original)
    {
        Plot plot = new();

        var sig1 = plot.Add.Signal(original, detector.SamplePeriodMin);
        sig1.Color = Colors.Gray.WithAlpha(.5);

        var sig2 = plot.Add.Signal(detector.Trace, detector.SamplePeriodMin);
        sig2.Color = Colors.Black;
        sig2.LineWidth = 2;

        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }

    public  static void ShowRateOverTime(this BinnedEvents bin)
    {
        Plot plot = new();
        plot.Add.Scatter(bin.Times, bin.FreqMinutes);
        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }

    public static void ShowEventTraces(this BinnedEvents events, ABF abf, int channelIndex)
    {
        Sweep sweep = abf.GetAllData(channelIndex);
        Plot plot = new();
        foreach (Cycle cycle in events.AllCycles)
        {
            double[] segment = sweep.Values[cycle.Index1..cycle.Index2];
            var sig = plot.Add.Signal(segment, sweep.SamplePeriod / 60);
            sig.Data.XOffset = cycle.StartTime / 60;
        }
        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }
}
