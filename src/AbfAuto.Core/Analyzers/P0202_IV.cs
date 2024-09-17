using ScottPlot;

namespace AbfAuto.Core.Analyzers;

public class P0202_IV : IAnalyzer
{
    public record struct TimeRange(double Time1, double Time2);

    public AnalysisResult Analyze(AbfSharp.ABF abf)
    {
        TimeRange ssRange = new(2.3, 2.5);
        TimeRange tailRange = new(2.57, 2.7);

        (double[] ssVoltages, double[] ssCurrents) = GetIvPoints(abf, ssRange);
        (double[] tailVoltages, double[] tailCurrents) = GetIvPoints(abf, tailRange);

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
        plot2.Add.VerticalLine(-70, 2, Colors.Red.WithAlpha(.5), LinePattern.DenselyDashed);
        plot2.Add.HorizontalLine(0, 2, Colors.Red.WithAlpha(.5), LinePattern.DenselyDashed);
        plot2.XLabel("Membrane Potential (mV)");
        plot2.YLabel("Steady State Current (pA)");

        Plot plot3 = CommonPlots.AllSweeps
            .Overlapping(abf, smoothPoints: 200)
            .WithSignalLineWidth(1.5)
            .WithSignalRainbow()
            .WithXLabelSeconds()
            .WithYLabelCurrent();
        plot3.Axes.SetLimitsX(2.25, 2.8);
        plot3.Add.HorizontalSpan(ssRange.Time1, ssRange.Time2, Colors.Red.WithAlpha(.1));
        plot3.Add.HorizontalSpan(tailRange.Time1, tailRange.Time2, Colors.Blue.WithAlpha(.1));

        Plot plot4 = new();
        var sp4 = plot4.Add.Scatter(tailVoltages, tailCurrents);
        sp4.LineWidth = 2;
        sp4.MarkerSize = 8;
        sp4.Color = Colors.Blue;
        plot4.Add.VerticalLine(-70, 2, Colors.Red.WithAlpha(.5), LinePattern.DenselyDashed);
        plot4.Add.HorizontalLine(0, 2, Colors.Red.WithAlpha(.5), LinePattern.DenselyDashed);
        plot4.XLabel("Membrane Potential (mV)");
        plot4.YLabel("Steady State Current (pA)");

        MultiPlot2 mp = new();
        mp.AddSubplot(plot1, 0, 2, 0, 2);
        mp.AddSubplot(plot2, 0, 2, 1, 2);
        mp.AddSubplot(plot3, 1, 2, 0, 2);
        mp.AddSubplot(plot4, 1, 2, 1, 2);

        return AnalysisResult.Single(mp);
    }

    public static (double[] voltages, double[] currents) GetIvPoints(AbfSharp.ABF abf, TimeRange range)
    {
        int i1 = (int)(abf.SampleRate * range.Time1);
        int i2 = (int)(abf.SampleRate * range.Time2);
        double[] voltages = Generate.Consecutive(abf.SweepCount, 10, -110);
        double[] currents = Enumerable
            .Range(0, abf.SweepCount)
            .Select(x => abf.GetSweep(x).Values[i1..i2].Average())
            .ToArray();
        return (voltages, currents);
    }
}
