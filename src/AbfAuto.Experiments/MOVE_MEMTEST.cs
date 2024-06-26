using AbfAuto.Core;
using ScottPlot;

namespace AbfAuto.Experiments;

internal static class MOVE_MEMTEST
{
    public static void Test()
    {
        //Memtest("X:/Data/zProjects/Aging and DA/BLA LTP/abfs/2024-06-14-DIC2-aged/2024_06_14_0021.abf");
    }

    public static void Memtest(string path)
    {
        AbfSharp.ABF abf = new(path);
        Epoch[] epochs = Enumerable.Range(0, abf.Header.AbfFileHeader.fEpochInitLevel.Length).Select(x => new Epoch(abf, x)).ToArray();
        Trace sweepTrace = new(abf, 0);

        Trace preStepTrace = sweepTrace.SubTraceByEpoch(epochs[0]);
        double current1 = preStepTrace.Mean();

        Trace stepTrace = sweepTrace.SubTraceByEpoch(epochs[1]);
        double current2 = stepTrace.SubTraceByFraction(0.75, 1).Mean();

        double deltaV_mV = Math.Abs(epochs[0].Level - epochs[1].Level);
        double deltaI_pA = Math.Abs(current1 - current2);
        double resistance_MOhm = deltaV_mV / deltaI_pA * 1000; // TODO: subtract Ra

        Plot plot = new();
        plot.Add.Signal(sweepTrace.Values, sweepTrace.SamplePeriod);
        plot.Add.HorizontalLine(current1, pattern: LinePattern.Dashed);
        plot.Add.HorizontalLine(current2, pattern: LinePattern.Dashed);

        var an = plot.Add.Annotation(
            $"Holding current: {current1:N2} pA\n" +
            $"Resistance: {resistance_MOhm:N2} MOhm", Alignment.UpperRight);
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Gray.Lighten(.8);
        an.LabelBorderWidth = 0;
        an.LabelFontSize = 16;
        an.LabelFontName = "Consolas";
        //an.LabelAlignment = Alignment.UpperRight;

        plot.XLabel("Time (sec)");
        plot.YLabel("Current (pA)");
        plot.HideGrid();
        plot.SavePng("test.png", 800, 600);
    }
}
