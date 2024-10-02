/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

using AbfAutoSandbox;
using ScottPlot;
using AbfSharp;

namespace AbfAUtoSandbox;

public static class Program
{
    public static void Main()
    {
        ABF abf = new(@"X:\Data\Alchem\IN-VIVO\Phase-4\abfs\2024-10-01\2024_10_01_EEG_0002.abf");
        Sweep sweep = abf.GetAllData(channelIndex: 1);

        CycleDetector detector = new(sweep.Values, sweep.SampleRate);
        detector.ApplySuccessiveSmoothing(1_000);
        detector.ApplyDetrend(10_000);
        BinnedEvents bin = detector.GetDownwardCyclesBinned();

        Plot plot = new();
        plot.Add.Scatter(bin.Times, bin.FreqMinutes);
        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }
}