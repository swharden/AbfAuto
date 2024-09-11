using AbfAuto.Core.Extensions;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class Unknown : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plot = new();
        plot.DataBackground.Color = Colors.Red.WithAlpha(.1);

        Sweep sweep = abf.GetAllData();

        bool isLongAbf = abf.AbfLength() > 10 * 60;
        if (isLongAbf)
        {
            sweep = sweep.Decimate(50);
            plot.Add.Signal(sweep.Values, sweep.SamplePeriod * 60);
            plot.XLabel("Time (minutes)");
        }
        else
        {

            plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            plot.XLabel("Time (seconds)");
        }

        plot.WithSignalLineWidth(1.5);
        plot.WithTightHorizontalMargins();

        var an = plot.Add.Annotation($"Unsupported Protocol");
        an.LabelFontSize = 26;
        an.Alignment = Alignment.UpperRight;
        an.LabelFontName = ScottPlot.Fonts.Monospace;
        an.LabelBold = true;

        plot.Title($"{Path.GetFileName(abf.FilePath)}\n{abf.Protocol()}");

        return AnalysisResult.Single(plot);
    }
}
