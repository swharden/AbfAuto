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
        Stopwatch sw = Stopwatch.StartNew();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Analyzing: {AbfPath}");

        AbfSharp.ABF abf;
        try
        {
            abf = new(AbfPath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Error Loading ABF: {ex.Message}");
            Console.WriteLine(ex);

            AnalysisResult result2 = Analyzers.Crashed.LoadingAbf(AbfPath, ex);
            string saveAsBase2 = Path.Combine(AnalysisFolderPath, $"{AbfID}_AbfAuto_CrashedLoading");
            string[] filenames2 = result2.SaveAll(saveAsBase2);
            return filenames2;
        }

        string protocol = Path.GetFileNameWithoutExtension(abf.Header.AbfFileHeader.sProtocolPath);

        IAnalyzer analysis = ProtocolTable.GetAnalyzer(abf);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Protocol: {protocol}");
        Console.WriteLine($"Analysis: {analysis}");
        AnalysisResult result = ExecuteAnalysis(abf, analysis);

        string saveAsName = analysis.ToString()!.Split(".").Last();
        string saveAsBase = Path.Combine(AnalysisFolderPath, $"{AbfID}_AbfAuto_{saveAsName}");
        string[] filenames = result.SaveAll(saveAsBase);

        Console.WriteLine($"Completed in: {sw.Elapsed.TotalSeconds:N2} sec");

        return filenames;
    }

    private AnalysisResult ExecuteAnalysis(AbfSharp.ABF abf, IAnalyzer analysis)
    {
        try
        {
            if (!Directory.Exists(AnalysisFolderPath))
                Directory.CreateDirectory(AnalysisFolderPath);
            return analysis.Analyze(abf);
        }
        catch (Exception ex)
        {
            return new Analyzers.Crashed(ex).Analyze(abf);
        }
    }
}
