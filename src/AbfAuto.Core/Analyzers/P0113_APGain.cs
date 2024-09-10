using AbfAuto.Core.EventDetection;
using ScottPlot;
using System.Security;

namespace AbfAuto.Core.Analyzers;

public class P0113_APGain : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        Epoch epochStep1 = abf.GetEpoch(1);
        Epoch epochStep2 = abf.GetEpoch(4);

        double[] xs = epochStep1.GetLevelsBySweep();
        double[] ys1 = APDetection.FreqPerSweep(abf, epochStep1);
        double[] ys2 = APDetection.FreqPerSweep(abf, epochStep2);

        Plot plotAll = CommonPlots.AllSweeps.Overlapping(abf)
            .WithYLabelVoltage()
            .WithXLabelSeconds()
            .WithSignalRainbow()
            .WithSignalAlpha(.8)
            .WithTightHorizontalMargins()
            .WithSignalHighQualityRendering();

        Plot plotStep1 = CommonPlots.AllSweeps.Overlapping(abf, 100)
            .WithYLabel("From Rest")
            .WithSignalRainbow()
            .WithBottomTicksOnly()
            .ZoomToEpoch(epochStep1)
            .OutlineEpoch(epochStep1)
            .WithSignalHighQualityRendering();

        Plot plotStep2 = CommonPlots.AllSweeps.Overlapping(abf, 100)
            .WithYLabel("From Hyperpolarization")
            .WithSignalRainbow()
            .WithBottomTicksOnly()
            .ZoomToEpoch(epochStep2)
            .OutlineEpoch(epochStep2)
            .WithSignalHighQualityRendering();

        Plot plotGain = new();

        var sp1 = plotGain.Add.Scatter(xs, ys1);
        sp1.LegendText = "from rest";

        var sp2 = plotGain.Add.Scatter(xs, ys2);
        sp2.LegendText = "hyperpolarized";

        plotGain.Legend.Alignment = Alignment.UpperLeft;

        plotGain
            .WithScatterLineWidth()
            .WithYLabel("AP Frequency (Hz)")
            .WithXLabel("Applied Current (pA)");

        MultiPlot2 mp = new();
        mp.AddSubplot(plotAll, 0, 2, 0, 2);
        mp.AddSubplot(plotStep1, 1, 2, 0, 2);
        mp.AddSubplot(plotStep2, 1, 2, 1, 2);
        mp.AddSubplot(plotGain, 0, 2, 1, 2);

        return AnalysisResult.Single(mp);
    }
}

