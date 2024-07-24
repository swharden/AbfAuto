using System.Diagnostics;
using AbfAuto.Core.SortLater;

namespace AbfAuto.Core;

public static class Analyze
{
    public static string GetAnalysisFilePath(string abfPath, IAnalyzer analysis)
    {
        string abfid = Path.GetFileNameWithoutExtension(abfPath);
        string analysisFolder = GetAnalysisFolder(abfPath);
        string analysisName = analysis.ToString()!.Split(".").Last();
        string filePath = Path.Combine(analysisFolder, $"{abfid}_AbfAuto_{analysisName}.png");
        return filePath;
    }

    public static void AnalyzeAbfFolder(string folderPath, bool overwrite)
    {
        string[] abfFilePaths = Directory.GetFiles(folderPath, "*.abf");
        for (int i = 0; i < abfFilePaths.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Analyzing ABF {i + 1} of {abfFilePaths.Length}");
            AnalyzeAbfFile(abfFilePaths[i], overwrite);
        }
    }

    public static void AnalyzeAbfFile(string abfPath, bool overwrite)
    {
        Stopwatch sw = Stopwatch.StartNew();

        abfPath = Path.GetFullPath(abfPath);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Analyzing: {abfPath}");

        AbfSharp.ABF abf = new(abfPath);
        string protocol = Path.GetFileNameWithoutExtension(abf.Header.AbfFileHeader.sProtocolPath);

        IAnalyzer analysis = AnalyzerLookup.GetAnalysis(abf);
        if (analysis is null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"ERROR: Unsupported protocol {protocol}");
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Protocol: {protocol}");
        Console.WriteLine($"Analysis: {analysis}");

        string filePath = GetAnalysisFilePath(abfPath, analysis);
        if (!overwrite && File.Exists(filePath))
        {
            Console.WriteLine($"Skipping: {Path.GetFileName(filePath)} exists");
            return;
        }

        Multiplot mp = analysis.Analyze(abf);
        mp.SavePng(filePath);
        Console.WriteLine($"Saved: {filePath}");

        Console.WriteLine($"Analysis completed in {sw.Elapsed.TotalMilliseconds:N2} ms");
    }

    private static string GetAnalysisFolder(string abfPath)
    {
        abfPath = Path.GetFullPath(abfPath);
        string abfFolder = Path.GetDirectoryName(abfPath)!;
        string analysisFolder = Path.Combine(abfFolder, "_autoanalysis");
        if (!Directory.Exists(analysisFolder))
            Directory.CreateDirectory(analysisFolder);
        return analysisFolder;
    }
}
