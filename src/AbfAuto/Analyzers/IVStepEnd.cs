using ScottPlot;

namespace AbfAuto.Analyzers;

/// <summary>
/// Analyze a voltage-clamp IP protocol and also show the tail current
/// </summary>
public class IVStepEnd : IAnalyzer
{
    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        (double[] ssVoltages, double[] ssCurrents) = GetIvPoints(abf);

        Plot plot1 = CommonPlots.AllSweeps
            .Overlapping(abf, smoothPoints: 200)
            .WithSignalLineWidth(1.5)
            .WithSignalRainbow()
            .WithTightHorizontalMargins()
            .WithXLabelSeconds()
            .WithYLabelCurrent();

        Plot plot2 = new();
        var sp2 = plot2.Add.Scatter(ssVoltages, ssCurrents);
        sp2.LineWidth = 2;
        sp2.MarkerSize = 8;
        sp2.Color = Colors.Red;
        plot2.XLabel("Membrane Potential (mV)");
        plot2.YLabel("Steady State Current (pA)");

        MultiPlot2 mp = new();
        mp.AddSubplot(plot1, 0, 1, 0, 2);
        mp.AddSubplot(plot2, 0, 1, 1, 2);
        mp.EnableHighQualitySignalPlots();

        return AnalysisResult.Single(mp);
    }

    public static (double[] voltages, double[] currents) GetIvPoints(AbfSharp.ABF abf, int epoch = 1)
    {
        double[] voltages = Enumerable
            .Range(0, abf.SweepCount)
            .Select(x => abf.Epochs[epoch].Level + abf.Epochs[epoch].LevelDelta * x)
            .ToArray();

        double[] currents = Enumerable
            .Range(0, abf.SweepCount)
            .Select(x => abf.GetSweep(x).MeanOfFraction(.75, 1.0))
            .ToArray();

        return (voltages, currents);
    }
}
