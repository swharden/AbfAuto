namespace AbfAuto.Core;

public interface IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf);
}
