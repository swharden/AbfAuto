using AbfAuto.Core.EventDetection;
using AbfAuto.Core.Extensions;
using AbfAuto.Core.SortLater;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class P0111_AP : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        Sweep sweep = abf.GetAllData();

        DerivativeThreshold.Settings settings = new();
        int[] indexes = DerivativeThreshold.GetIndexes(sweep, settings);
        int firstApIndex = indexes.FirstOrDefault();

        int i1 = Math.Max(0, firstApIndex - 1000);
        int i2 = firstApIndex + 1000;
        Sweep apTrace = sweep.SubSweepByIndex(i1, i2);
        Sweep dvdtTrace = apTrace.Derivative();

        Plot plot1 = new();
        Plot plot2 = new();
        Plot plot3 = new();
        Plot plot4 = new();

        plot1.Title("First AP (V)");
        plot2.Title("First AP (dV)");
        if (indexes.Length > 0)
        {
            var sig1 = plot1.AddSignalMS(apTrace);
            sig1.Color = Colors.Blue;
            sig1.AlwaysUseLowDensityMode = true;
            sig1.LineWidth = 1.5f;

            var sig2 = plot2.AddSignalMS(dvdtTrace);
            sig2.Color = Colors.Red;
            sig2.AlwaysUseLowDensityMode = true;
            sig2.LineWidth = 1.5f;

            plot2.Axes.AutoScale();
            plot2.Axes.ZoomIn(fracX: 5);
        }

        plot3.Title("Full Trace");
        var sig3 = plot3.AddSignalMS(sweep);
        sig3.AlwaysUseLowDensityMode = true;
        sig3.LineWidth = 1.5f;

        plot3.Axes.Margins(horizontal: 0);

        plot4.Title("First AP (dV/dt)");
        var sp = plot4.Add.ScatterLine((List<double>)apTrace.Values, (List<double>)dvdtTrace.Values);
        sp.LineColor = Colors.C1;
        sp.LineWidth = 1.5f;

        MultiPlot2 mp = new();
        mp.AddSubplot(plot1, 0, 2, 0, 2);
        mp.AddSubplot(plot2, 0, 2, 1, 2);
        mp.AddSubplot(plot3, 1, 2, 0, 2);
        mp.AddSubplot(plot4, 1, 2, 1, 2);

        return AnalysisResult.WithSingleMultiPlot(mp);
    }
}
