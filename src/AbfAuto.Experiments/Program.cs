using AbfAuto.Core;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string abfPath = @"X:\Data\zProjects\VNS\VNS vs CTRL rat\Spont\2027-07-23-Sham3\2024_07_23_0000.abf";
        //AbfAnalyzer.AnalyzeAbfFile(abfPath, true);
        MOVE_MEMTEST.Memtest(abfPath);  
    }
}