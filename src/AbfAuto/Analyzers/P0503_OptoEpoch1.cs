using AbfSharp;

namespace AbfAuto.Analyzers;
internal class P0503_OptoEpoch1 : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        MultiPlot2 mp = CommonPlots.Opto.RepeatedSweeps(abf, 1);
        return AnalysisResult.Single(mp);
    }
}
