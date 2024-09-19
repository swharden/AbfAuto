using AbfSharp;

namespace AbfAuto.Analyzers;

/// <summary>
/// Used for ABFs where opto is applied once per sweep.
/// Shows the region around the opto stimulation, for every sweep and for the mean sweep.
/// </summary>
internal class OptoMeanEpoch2 : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        MultiPlot2 mp = CommonPlots.Opto.RepeatedSweeps(abf, 2);
        return AnalysisResult.Single(mp);
    }
}
