using AbfAuto.Core;
using AbfAuto.Core.EventDetection;
using AbfAuto.Core.Extensions;
using ScottPlot;
using ScottPlot.WinForms;
using System.Diagnostics;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string filePath = @"X:\Data\Alchem\IN-VIVO\Phase-4\abfs\2024-08-26\2024_08_26_EEG_0000.abf";
        AnalyzeAbfFile(filePath);

        //string folderPath = @"X:\Data\zProjects\SST diabetes\LTS neuron SST\abfs\2024-08-16-DIC1";
        //AnalyzeAbfFolder(folderPath);
    }

    private static void AnalyzeAbfFile(string filePath)
    {
        AbfFileAnalyzer analyzer = new(filePath);
        string[] saved = analyzer.Analyze();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(string.Join("\n", saved));
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    private static void AnalyzeAbfFolder(string folderPath)
    {
        Stopwatch sw = Stopwatch.StartNew();
        AbfFolderAnalyzer analyzer = new(folderPath);
        for (int i = 0; i < analyzer.AbfFilePaths.Length; i++)
        {
            analyzer.AnalyzeIndex(i);
        }

        Console.WriteLine($"Analyzed {analyzer.AbfFilePaths.Length} ABF files in {sw.Elapsed.TotalSeconds:N2} sec");
    }
}