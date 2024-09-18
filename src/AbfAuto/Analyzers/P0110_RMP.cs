using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class P0110_RMP : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Sweep sweep = abf.GetAllData(0).Smooth(TimeSpan.FromMilliseconds(2));
        double mean = sweep.Values.Average();

        Plot plot = new();
        plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        plot
            .WithSignalLineWidth(1.5)
            .WithSignalHighQualityRendering();

        plot.Add.HorizontalLine(mean, 2, Colors.Black, LinePattern.DenselyDashed);

        var an = plot.Add.Annotation($"RMP = {mean:N2} mV");
        an.Alignment = Alignment.UpperRight;
        an.LabelBorderWidth = 0;
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Transparent;
        an.LabelFontSize = 24;
        an.LabelBold = true;
        an.LabelFontName = Fonts.Monospace;

        plot.YLabel("Potential (mV)");
        plot.XLabel("Time (sec)");
        plot.Axes.Margins(horizontal: 0);
        plot.Axes.AutoScale();

        AxisLimits limits = plot.Axes.GetLimits();
        double minVerticalSpan = 10;
        if (limits.VerticalSpan < 10)
        {
            double yMin = limits.VerticalCenter - minVerticalSpan / 2;
            double yMax = limits.VerticalCenter + minVerticalSpan / 2;
            plot.Axes.SetLimitsY(yMin, yMax);
        }

        return AnalysisResult.Single(plot);
    }
}
