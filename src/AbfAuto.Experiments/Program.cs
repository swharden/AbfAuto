using AbfAuto.Core;
using System.Diagnostics;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        //string filePath = @"X:\Data\Alchem\Donepezil\BLA\07-28-2021\2021_07_28_DIC1_0002.abf";
        //AnalyzeAbfFile(filePath);

        string tifFile = @"X:\Data\Alchem\Donepezil\BLA\07-28-2021\2021_07_28_DIC1_0000.tif";
        string saved = AbfAuto.Core.TifFile.AutoAnalyze(tifFile);
        Console.WriteLine(saved);

        //string folderPath = @"X:\Data\zProjects\SST diabetes\LTS neuron SST\abfs\2024-08-16-DIC1";
        //AnalyzeAbfFolder(folderPath);
    }

    private static void AnalyzeAbfFile(string filePath)
    {
        AbfFileAnalyzer analyzer = new(filePath);
        string[] saved = analyzer.Analyze();
        Console.WriteLine(string.Join("\n", saved));
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