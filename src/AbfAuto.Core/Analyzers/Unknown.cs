using AbfAuto.Core.Extensions;
using AbfAuto.Core.SortLater;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class Unknown : IAnalyzer
{
    public Multiplot Analyze(ABF abf)
    {
        Trace trace = abf.GetAllData();

        Plot plot = new();

        plot.Add.Signal(trace.Values, trace.SamplePeriod);
        plot.DataBackground.Color = Colors.Red.WithAlpha(.1);
        var an = plot.Add.Annotation("Unsupported Protocol");
        an.LabelFontSize = 26;
        an.Alignment = Alignment.UpperRight;
        an.LabelFontName = "consolas";
        an.LabelBold = true;

        return Multiplot.WithSinglePlot(plot, 600, 400);
    }
}
