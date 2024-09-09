using AbfAuto.Core.Extensions;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class Unknown : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Sweep sweep = abf.GetAllData();

        Plot plot = new();

        plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        plot.DataBackground.Color = Colors.Red.WithAlpha(.1);
        var an = plot.Add.Annotation("Unsupported Protocol");
        an.LabelFontSize = 26;
        an.Alignment = Alignment.UpperRight;
        an.LabelFontName = ScottPlot.Fonts.Monospace;
        an.LabelBold = true;

        return AnalysisResult.WithSinglePlot(plot);
    }
}
