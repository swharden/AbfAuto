using AbfAuto.Memtest;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

/// <summary>
/// Voltage-clamp protocol that simulates a back propagating action potential with fast depolarizing voltage clamp pulses.
/// </summary>
public class BAP : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plotMemtest = CommonPlots.AllSweeps
            .OverlappingTimeRange(abf, 0.2, 0.8, 25)
            .WithSignalHighQualityRendering()
            .WithTightHorizontalMargins()
            .WithSignalRainbow()
            .WithTitle("Membrane Test");

        MemtestResult mt = MemtestLogic.GetMeanMemtest(abf);
        var an = plotMemtest.Add.Annotation(mt.GetShortMessage(), Alignment.UpperLeft);
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Transparent;
        an.LabelFontSize = 16;
        an.LabelFontName = "Consolas";
        an.LabelStyle.BorderRadius = 10;
        an.LabelBorderWidth = 0;

        Plot plotStim = CommonPlots.AllSweeps
            .OverlappingTimeRange(abf, 2.34, 2.38, 0)
            .WithSignalHighQualityRendering()
            .WithTightHorizontalMargins()
            .WithSignalRainbow()
            .WithTitle("First Stimulus");

        MultiPlot2 mp = new();
        mp.AddSubplot(plotMemtest, 0, 1, 0, 2);
        mp.AddSubplot(plotStim, 0, 1, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
