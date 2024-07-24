namespace AbfAuto.Core;

public interface IAnalyzer
{
    public SortLater.Multiplot Analyze(AbfSharp.ABF abf);
}
