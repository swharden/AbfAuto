using System.Text;

namespace AbfAuto.Memtest;

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
            : $"Membrane Resistance: {Rm / 1000:N2} GΩ");
        sb.AppendLine($"Access Resistance: {Ra:N2} MΩ");
        sb.AppendLine($"Capacitance (Step): {CmStep:N2} pA");
        if (CmRamp > 0)
            sb.AppendLine($"Capacitance (Ramp): {CmRamp:N2} pA");
        return sb.ToString().Trim();
    }

    public readonly string GetShortMessage()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Ih: {Ih:N2} pA");
        sb.AppendLine(Rm < 1000
            ? $"Rm: {Rm:N2} MΩ"
            : $"Rm: {Rm / 1000:N2} GΩ");
        sb.AppendLine($"Ra: {Ra:N2} MΩ");
        sb.AppendLine($"Cm: {CmStep:N2} pA");
        if (CmRamp > 0)
            sb.AppendLine($"Cm: {CmRamp:N2} pA");
        return sb.ToString().Trim();
    }
}
