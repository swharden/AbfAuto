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

    public string[] Analyze()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Analyzing {AbfPath}");

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

        IAnalyzer analyzer = ProtocolTable.GetAnalyzer(abf);
        string analyzerName = analyzer.ToString()!.Split(".").Last();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Protocol '{protocol}' uses the '{analyzerName}' analyzer");
        AnalysisResult result = ExecuteAnalysis(abf, analyzer);

        string saveAsName = analyzer.ToString()!.Split(".").Last();
        string saveAsBase = Path.Combine(AnalysisFolderPath, $"{AbfID}_AbfAuto_{saveAsName}");
        string[] filenames = result.SaveAll(saveAsBase);

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
