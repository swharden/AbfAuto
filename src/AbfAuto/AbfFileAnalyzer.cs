using System.Diagnostics;

namespace AbfAuto;

public class AbfFileAnalyzer
{
    public string AbfPath { get; }
    private string AnalysisFolderPath => Path.Combine(Path.GetDirectoryName(AbfPath)!, "_autoanalysis");
    string AbfID => Path.GetFileNameWithoutExtension(AbfPath);

    public AbfFileAnalyzer(string abfPath)
    {
        if (!File.Exists(abfPath))
            throw new FileNotFoundException(abfPath);

        AbfPath = Path.GetFullPath(abfPath);
    }

    public string[] Analyze(bool overwrite = true)
    {
        CreateAnalysisFolder();

        Stopwatch sw = Stopwatch.StartNew();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Analyzing: {AbfPath}");

        AbfSharp.ABF abf = new(AbfPath);
        string protocol = Path.GetFileNameWithoutExtension(abf.Header.AbfFileHeader.sProtocolPath);

        IAnalyzer analysis = ProtocolTable.GetAnalysis(abf);
        if (analysis is null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"ERROR: Unsupported protocol {protocol}");
            return [];
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Protocol: {protocol}");
        Console.WriteLine($"Analysis: {analysis}");
        AnalysisResult result = analysis.Analyze(abf);

        string saveAsName = analysis.ToString()!.Split(".").Last();
        string saveAsBase = Path.Combine(AnalysisFolderPath, $"{AbfID}_AbfAuto_{saveAsName}");
        string[] filenames = result.SaveAll(saveAsBase);

        Console.WriteLine($"Completed in: {sw.Elapsed.TotalSeconds:N2} sec");

        return filenames;
    }

    private void CreateAnalysisFolder()
    {
        if (!Directory.Exists(AnalysisFolderPath))
            Directory.CreateDirectory(AnalysisFolderPath);
    }
}
