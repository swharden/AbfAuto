using AbfAuto.Core;
using System.Diagnostics;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string folderPath = @"X:\Data\zProjects\VNS\VNS vs CTRL rat\STDP\2024-07-24-VNS114";

        Stopwatch sw = Stopwatch.StartNew();
        AbfFolderAnalyzer analyzer = new(folderPath);
        for (int i = 0; i < 8; i++)
        {
            analyzer.AnalyzeIndex(i);
        }

        Console.WriteLine($"Finished everything in {sw.Elapsed.TotalSeconds:N2} sec");
    }
}