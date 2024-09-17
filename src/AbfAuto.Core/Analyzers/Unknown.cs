using AbfAuto.Core.Extensions;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class Unknown : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Sweep sweep = abf.GetAllData();

        Plot plot = CommonPlots.AllSweeps.ConsecutiveMinutes(sweep)
            .WithSignalLineWidth(1.5)
            .WithTightHorizontalMargins();

        var an = plot.Add.Annotation($"Unsupported Protocol");
        an.LabelFontSize = 26;
        an.Alignment = Alignment.UpperRight;
        an.LabelFontName = ScottPlot.Fonts.Monospace;
        an.LabelBold = true;

        plot.Title($"{Path.GetFileName(abf.FilePath)}\n{abf.Protocol()}");
        plot.DataBackground.Color = Colors.Red.WithAlpha(.1);

        return AnalysisResult.Single(plot);
    }
}
