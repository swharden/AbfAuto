using AbfAuto.Core;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string abfPath = @"X:\Data\zProjects\VNS\VNS vs CTRL rat\STDP\2024-07-24-VNS4\2024_07_24_0017.abf";
        AbfAnalyzer.AnalyzeAbfFile(abfPath, true);
    }
}