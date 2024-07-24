using AbfAuto.Core.Extensions;
using AbfAuto.Core.SortLater;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

internal class P0110_RMP : IAnalyzer
{
    public Multiplot Analyze(ABF abf)
    {
        Trace trace = abf.GetAllData().SmoothedMsec(2);
        double mean = trace.Values.Average();

        Plot plot = new();

        plot.Add.Signal(trace.Values, trace.SamplePeriod);
        plot.Add.HorizontalLine(mean, 2, Colors.Black, LinePattern.DenselyDashed);
        var an = plot.Add.Annotation($"RMP = {mean:N2} mV");
        an.Alignment = Alignment.UpperRight;
        an.LabelBorderWidth = 0;
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Transparent;
        an.LabelFontSize = 24;
        an.LabelBold = true;
        an.LabelFontName = "consolas";

        plot.YLabel("Potential (mV)");
        plot.XLabel("Time (sec)");

        return Multiplot.WithSinglePlot(plot, 600, 400);
    }
}
