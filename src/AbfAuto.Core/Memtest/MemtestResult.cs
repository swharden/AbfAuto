namespace AbfAuto.Core.Memtest;

public record struct MemtestResult
{
    public double dV; // mV
    public double dI; // pA
    public double Ih; // pA
    public double Rm; // MOhm
    public double Ra; // MOhm
    public double CmRamp; // pF
    public double CmStep; // pF
    public double Tau; // msec
}
