using ScottPlot;

namespace AbfAuto.Analyzers;

public class Crashed(Exception exception) : IAnalyzer
{
    Exception Exception { get; } = exception;

    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        return GetResult(Exception, $"Crashed Analyzing {Path.GetFileName(abf.FilePath)}\nProtocol: {abf.Header.Protocol}");
    }

    public static AnalysisResult LoadingAbf(string abfFilePath, Exception ex)
    {
        return GetResult(ex, $"Crashed Loading {Path.GetFileName(abfFilePath)}");
    }

    private static AnalysisResult GetResult(Exception ex, string title)
    {
        Plot plot = new();
        plot.Title(title);
        plot.DataBackground.Color = Colors.Red.WithAlpha(.3);

        string message =
            $"Crashed during analysis!\n" +
            $"{ex.Message}\n" +
            $"Exception details and stack trace are in a crash file\n" +
            $"saved in the auto-analysis folder next to this ABF.";

        var an = plot.Add.Annotation(message);
        an.LabelFontSize = 14;
        an.Alignment = Alignment.UpperLeft;
        an.LabelFontName = ScottPlot.Fonts.Monospace;
        an.LabelBold = true;

        return AnalysisResult.Single(plot)
            .WithTextFile("exception", ex.ToString());
    }
}
