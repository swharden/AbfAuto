using AbfAuto.Core.Memtest;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class P0804_bAP : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plotStim = CommonPlots.AllSweeps
            .Overlapping(abf)
            .WithSignalHighQualityRendering()
            .WithSignalRainbow();

        plotStim.Axes.SetLimitsX(2.34, 2.38);

        Plot plotMemtest = CommonPlots.AllSweeps
            .Overlapping(abf)
            .WithSignalHighQualityRendering()
            .WithSignalRainbow();

        MemtestResult mt = MemtestLogic.GetMeanMemtest(abf);
        var an = plotMemtest.Add.Annotation(mt.GetMessage(), Alignment.UpperRight);
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Transparent;
        an.LabelFontSize = 16;
        an.LabelFontName = "Consolas";
        an.LabelStyle.BorderRadius = 10;
        an.LabelBorderWidth = 0;

        plotMemtest.Axes.SetLimitsX(0.2, 0.8);
        plotMemtest.Axes.SetLimitsY(-1000, 1000);

        MultiPlot2 mp = new();
        mp.AddSubplot(plotMemtest, 0, 2, 0, 1);
        mp.AddSubplot(plotStim, 1, 2, 0, 1);

        return AnalysisResult.Single(mp);
    }
}
