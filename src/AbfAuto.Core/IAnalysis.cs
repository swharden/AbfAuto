namespace AbfAuto.Core;
public interface IAnalysis
{
    public Multiplot Analyze(AbfSharp.ABFFIO.ABF abf);
}
