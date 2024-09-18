using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class P0501_OptoEpoch2 : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        MultiPlot2 mp = CommonPlots.Opto.RepeatedSweeps(abf, 2);
        return AnalysisResult.Single(mp);
    }
}
