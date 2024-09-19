using AbfAuto.ScottPlotMods;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

/// <summary>
/// In-vivo recording using 3 channels: EEG, Respiration, and ECG
/// </summary>
internal class InVivo3 : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Sweep sweep1 = abf.GetAllData(0).Smooth(100).Detrend(1000);
        Plot ch1Full = CommonPlots.AllSweeps.ConsecutiveMinutes(sweep1)
            .WithYLabel("EEG")
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithTightHorizontalMargins();
        ch1Full.Axes.SetLimitsY(-.02, .02);
        Plot ch1freq = CommonPlots.InVivo.GetEegFreqPlot().WithVerticalLinesAtTagTimes(abf);

        Sweep sweep2 = abf.GetAllData(1).Smooth(100).Detrend(1000);
        Plot ch2Full = CommonPlots.AllSweeps.ConsecutiveMinutes(sweep2)
            .WithYLabel("Respiration")
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithTightHorizontalMargins();
        Plot ch2freq = CommonPlots.InVivo.GetRespirationFreqPlot(sweep2).WithVerticalLinesAtTagTimes(abf);

        Sweep sweep3 = abf.GetAllData(2).Derivative().Derivative().Rectified().Smooth(10);
        Plot ch3Full = CommonPlots.AllSweeps.ConsecutiveMinutes(sweep3)
            .WithSignalLineWidth(1.5)
            .WithVerticalLinesAtTagTimes(abf)
            .WithYLabel("ECG")
            .WithXLabelMinutes()
            .WithTightHorizontalMargins();
        ch3Full.Axes.SetLimitsY(0, 10);
        Plot ch3freq = CommonPlots.InVivo.GetEcgFreqPlot(sweep3).WithVerticalLinesAtTagTimes(abf);

        MultiPlot2 mp = new();
        mp.AddSubplot(ch1Full, 0, 3, 0, 2);
        mp.AddSubplot(ch1freq, 0, 3, 1, 2);
        mp.AddSubplot(ch2Full, 1, 3, 0, 2);
        mp.AddSubplot(ch2freq, 1, 3, 1, 2);
        mp.AddSubplot(ch3Full, 2, 3, 0, 2);
        mp.AddSubplot(ch3freq, 2, 3, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
