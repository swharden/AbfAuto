namespace AbfAuto.Core;

public interface IAnalysis
{
    public Multiplot Analyze(AbfSharp.ABF abf);
}
