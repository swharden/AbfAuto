using ScottPlot;

namespace AbfAuto.Core;

public class AmpaNmdaRatio
{
    public readonly TimeRange BaselineRange = new(1.4405, 2.1288);
    public readonly TimeRange AmpaRange = new(2.159, 2.179);
    public readonly TimeRange NmdaRange = new(2.249, 2.269);
    public readonly AbfSweep[] Sweeps;

    public AmpaNmdaRatio(string path)
    {
        string abfFilePath = Path.GetFullPath(path);
        AbfSharp.ABF abf = new(abfFilePath);
        Sweeps = Enumerable.Range(0, abf.SweepCount).Select(x => AbfSweep.FromAbf(abf, x)).ToArray();
    }

    public double[] GetMeanBySweep(double time1, double time2)
    {
        TimeRange range = new(time1, time2);
        double[] values = new double[Sweeps.Length];

        for (int i = 0; i < values.Length; i++)
        {
            double baseline = Sweeps[i].GetMean(BaselineRange);
            double raw = Sweeps[i].GetMean(range);
            values[i] = raw - baseline;
        }

        return values;
    }

    public double[] GetMinBySweep(double time1, double time2)
    {
        TimeRange range = new(time1, time2);
        double[] values = new double[Sweeps.Length];

        for (int i = 0; i < values.Length; i++)
        {
            double baseline = Sweeps[i].GetMean(BaselineRange);
            double raw = Sweeps[i].GetMin(range);
            values[i] = raw - baseline;
        }

        return values;
    }

    public double[] GetAmpaValues()
    {
        double[] values = new double[Sweeps.Length];

        for (int i = 0; i < values.Length; i++)
        {
            double baseline = Sweeps[i].GetMean(BaselineRange);
            double raw = Sweeps[i].GetMean(AmpaRange);
            values[i] = raw - baseline;
        }

        return values;
    }

    public double[] GetNmdaValues()
    {
        double[] values = new double[Sweeps.Length];

        for (int i = 0; i < values.Length; i++)
        {
            double baseline = Sweeps[i].GetMean(BaselineRange);
            double raw = Sweeps[i].GetMean(NmdaRange);
            values[i] = raw - baseline;
        }

        return values;
    }

    public (ScottPlot.Plot, double[] values) PlotSweepsAmpa()
    {
        ScottPlot.Plot plot = new();

        double[] valuesBySweep = new double[Sweeps.Length];

        for (int i = 0; i < Sweeps.Length; i++)
        {
            AbfSweep sweep = Sweeps[i]
                .WithSubtraction(BaselineRange)
                .WithTrim(2.0, 2.5)
                .WithSmoothing(200);

            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Data.XOffset = sweep.StartTime;
            sig.Color = Colors.C0.WithAlpha(.5);

            (double value, int index) = sweep.GetMinValueAndIndex();
            double minTime = index * sweep.SamplePeriod + sweep.StartTime;
            double minValue = sweep.Values[index];
            valuesBySweep[i] = minValue;

            var mark = plot.Add.Marker(minTime, minValue);
            mark.Color = Colors.Black;
        }

        var hline = plot.Add.HorizontalLine(0);
        hline.Color = Colors.Black;
        hline.LinePattern = LinePattern.Dashed;

        double mean = ScottPlot.Statistics.Descriptive.Mean(valuesBySweep);
        double err = ScottPlot.Statistics.Descriptive.StandardError(valuesBySweep);

        var hlineMean = plot.Add.HorizontalLine(mean);
        hlineMean.Color = Colors.Black;
        hlineMean.LinePattern = LinePattern.Dotted;

        plot.Title($"AMPA: {Math.Abs(mean):N2} ± {err:N2} pA");
        plot.YLabel("Current (pA)");
        plot.XLabel("Sweep Time (sec)");

        return (plot, valuesBySweep);
    }

    public (ScottPlot.Plot, double[] values) PlotSweepsNmda()
    {
        ScottPlot.Plot plot = new();

        double[] valuesBySweep = new double[Sweeps.Length];

        for (int i = 0; i < Sweeps.Length; i++)
        {
            AbfSweep sweep = Sweeps[i]
                .WithSubtraction(BaselineRange)
                .WithTrim(2.0, 2.5)
                .WithSmoothing(200);

            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Data.XOffset = sweep.StartTime;
            sig.Color = Colors.C0.WithAlpha(.5);

            double nmdaMean = sweep.GetMean(NmdaRange);
            valuesBySweep[i] = nmdaMean;

            var mark = plot.Add.Marker(NmdaRange.CenterTime, nmdaMean);
            mark.Color = Colors.Black;
        }

        var hline = plot.Add.HorizontalLine(0);
        hline.Color = Colors.Black;
        hline.LinePattern = LinePattern.Dashed;

        var rangeSpan = plot.Add.HorizontalSpan(NmdaRange.MinTime, NmdaRange.MaxTime);
        rangeSpan.FillColor = Colors.Gray.WithAlpha(.2);

        double mean = ScottPlot.Statistics.Descriptive.Mean(valuesBySweep);
        double err = ScottPlot.Statistics.Descriptive.StandardError(valuesBySweep);

        var hlineMean = plot.Add.HorizontalLine(mean);
        hlineMean.Color = Colors.Black;
        hlineMean.LinePattern = LinePattern.Dotted;

        plot.Title($"NMDA: {Math.Abs(mean):N2} ± {err:N2} pA");
        plot.YLabel("Current (pA)");
        plot.XLabel("Sweep Time (sec)");

        return (plot, valuesBySweep);
    }
}
