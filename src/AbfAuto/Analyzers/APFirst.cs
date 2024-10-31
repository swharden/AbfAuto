using AbfAuto.EventDetection;
using AbfSharp;

using ScottPlot;
using System.Runtime.InteropServices.Marshalling;

namespace AbfAuto.Analyzers;

/// <summary>
/// Analyze properties of the first AP in a slow IC ramp
/// </summary>
public class APFirst : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        Sweep sweep = abf.GetAllData(0);

        DerivativeThreshold.Settings settings = new();
        int[] indexes = DerivativeThreshold.GetIndexes(sweep, settings);
        int firstApIndex = indexes.FirstOrDefault();

        int i1 = Math.Max(0, firstApIndex - 1000);
        int i2 = firstApIndex + 1000;
        Sweep apTrace = sweep.SubSweepByIndex(i1, i2);
        Sweep dvdtTrace = apTrace.Derivative();

        double dvdtScale = dvdtTrace.SampleRate / 1000; // scale so units are "per ms"
        for (int i = 0; i < dvdtTrace.Values.Length; i++)
        {
            dvdtTrace.Values[i] *= dvdtScale;
        }

        Plot plot1 = new();
        Plot plot2 = new();
        Plot plot3 = new();
        Plot plot4 = new();

        plot1.Title("First AP (V)");
        plot1.YLabel("Potential (mV)");

        plot2.Title("First AP (dV)");
        plot2.YLabel("Velocity (mV/ms)");
        if (indexes.Length > 0)
        {
            /*
            var an = plot1.Add.Annotation($"Threshold: {threshold:0.00} mV", Alignment.UpperRight);
            an.LabelShadowColor = Colors.Transparent;
            an.LabelBackgroundColor = Colors.Gray.WithAlpha(.2);
            an.LabelFontSize = 16;
            an.LabelFontName = "Consolas";
            an.LabelStyle.BorderRadius = 10;
            an.LabelBorderWidth = 0;
            */

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
        plot3.YLabel("Potential (mV)");
        var sig3 = plot3.AddSignalSec(sweep);
        sig3.Color = Colors.Blue;
        sig3.AlwaysUseLowDensityMode = true;
        sig3.LineWidth = 1.5f;

        plot3.Axes.Margins(horizontal: 0);

        plot4.Title("Phase Plot");
        plot4.XLabel("Potential (mV)");
        plot4.YLabel("Velocity (mV/ms)");
        var sp = plot4.Add.ScatterLine(apTrace.Values.ToArray(), dvdtTrace.Values.ToArray());
        sp.LineColor = Colors.C1;
        sp.LineWidth = 1.5f;

        MultiPlot2 mp = new();
        mp.AddSubplot(plot1, 0, 2, 0, 2);
        mp.AddSubplot(plot2, 0, 2, 1, 2);
        mp.AddSubplot(plot3, 1, 2, 0, 2);
        mp.AddSubplot(plot4, 1, 2, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
