namespace AbfAuto;

public interface IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf);
}
