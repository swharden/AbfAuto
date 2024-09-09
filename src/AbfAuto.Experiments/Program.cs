using AbfAuto.Core;
using AbfAuto.Core.EventDetection;
using System.Diagnostics;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string filePath = @"X:\Data\zProjects\SST diabetes\LTS neuron SST\abfs\2024-08-16-DIC1\2024_08_16_0010.abf";
        AbfSharp.ABF abf = new(filePath);

        int[][] apsPerSweep = APDetection.GetApIndexesPerSweep(abf);

        Epoch step1 = abf.GetEpoch(1);
        Epoch step2 = abf.GetEpoch(4);

        for (int i = 0; i < apsPerSweep.Length; i++)
        {
            int step1count = apsPerSweep[i].Where(x => x >= step1.IndexFirst && x <= step1.IndexLast).Count();
            int step2count = apsPerSweep[i].Where(x => x >= step2.IndexFirst && x <= step2.IndexLast).Count();
            Console.WriteLine($"Sweep {i + 1:00}\t{step1count}\t{step2count}");
        }
    }

    private static void MakeFigures()
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