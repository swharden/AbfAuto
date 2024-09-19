using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

public class Crashed(Exception exception) : IAnalyzer
{
    Exception Exception { get; } = exception;

    public AnalysisResult Analyze(ABF abf)
    {
        Plot plot = new();
        plot.Title($"{Path.GetFileName(abf.FilePath)}\n{abf.Header.Protocol}");
        plot.DataBackground.Color = Colors.Red.WithAlpha(.3);

        var an = plot.Add.Annotation($"Crashed during analysis:\n{Exception.Message}\n{Exception}");
        an.LabelFontSize = 14;
        an.Alignment = Alignment.UpperLeft;
        an.LabelFontName = ScottPlot.Fonts.Monospace;
        an.LabelBold = true;

        return AnalysisResult.Single(plot)
            .WithTextFile("exception", Exception.ToString());
    }
}
