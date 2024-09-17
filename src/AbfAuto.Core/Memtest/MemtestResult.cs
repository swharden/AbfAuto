using System.Text;

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

    public readonly string GetMessage()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Holding current: {Ih:N2} pA");
        sb.AppendLine(Rm < 1000 
            ? $"Membrane Resistance: {Rm:N2} MΩ" 
            : $"Membrane Resistance: {Rm/1000:N2} GΩ");
        sb.AppendLine($"Access Resistance: {Ra:N2} MΩ");
        sb.AppendLine($"Capacitance (Step): {CmStep:N2} pA");
        if (CmRamp > 0)
            sb.AppendLine($"Capacitance (Ramp): {CmRamp:N2} pA");
        return sb.ToString().Trim();
    }
}
