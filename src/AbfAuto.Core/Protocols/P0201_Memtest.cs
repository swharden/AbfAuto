using ScottPlot;

namespace AbfAuto.Core.Protocols;

public class P0201_Memtest : IAnalysis
{
    public Multiplot Analyze(AbfSharp.ABF abf)
    {
        MemtestResult[] allResults = Enumerable
            .Range(0, abf.SweepCount)
            .Select(x => GetMemtestValues(abf, x))
            .ToArray();

        MemtestResult mt = new()
        {
            Ih = allResults.Select(x => x.Ih).Average(),
            Ra = allResults.Select(x => x.Ra).Average(),
            Rm = allResults.Select(x => x.Rm).Average(),
            CmStep = allResults.Select(x => x.CmStep).Average(),
        };

        Plot plot = new();

        ScottPlot.Color[] colors = new ScottPlot.Colormaps.Turbo().GetColors(abf.SweepCount, .1, .9);

        var pal = new ScottPlot.Colormaps.Turbo();
        for (int i = 0; i < abf.SweepCount; i++)
        {
            Trace sweepTrace = new(abf, i);
            var sig = plot.Add.Signal(sweepTrace.Values, sweepTrace.SamplePeriod);
            sig.Color = colors[i];
        }

        var an = plot.Add.Annotation(
            $"Holding current: {mt.Ih:N2} pA\n" +
            $"Membrane Resistance: {mt.Rm:N2} MΩ\n" +
            $"Access Resistance: {mt.Ra:N2} MΩ\n" +
            $"Capacitance (Step): {mt.CmStep:N2} pA"
            , Alignment.UpperRight);
        an.LabelShadowColor = Colors.Transparent;
        an.LabelBackgroundColor = Colors.Gray.Lighten(.8);
        an.LabelFontSize = 16;
        an.LabelFontName = "Consolas";
        an.Label.BorderRadius = 10;
        an.LabelBorderWidth = 0;

        plot.Axes.Margins(horizontal: 0);
        plot.XLabel("Time (sec)");
        plot.YLabel("Current (pA)");
        plot.HideGrid();
        string saveAs = Path.GetFullPath("test.png");
        plot.SavePng(saveAs, 800, 600);
        Console.WriteLine(saveAs);

        return Multiplot.WithSinglePlot(plot, 600, 400);
    }

    public static MemtestResult GetMemtestValues(AbfSharp.ABF abf, int sweepIndex)
    {
        Trace sweepTrace = new(abf, sweepIndex);

        MemtestResult mt = new();

        // Get dV and dI from the hyperpolarizing step epoch relative to the pre-step epoch
        Epoch[] epochs = Enumerable.Range(0, abf.Header.AbfFileHeader.fEpochInitLevel.Length).Select(x => new Epoch(abf, x)).ToArray();

        Trace preStepTrace = sweepTrace.SubTraceByEpoch(epochs[0]);
        double preStepCurrentMean = preStepTrace.Mean();

        Trace stepTrace = sweepTrace.SubTraceByEpoch(epochs[1]);
        double stepCurrentMean = stepTrace.SubTraceByFraction(0.75, 1).Mean();

        mt.Ih = preStepCurrentMean;
        mt.dV = Math.Abs(epochs[0].Level - epochs[1].Level);
        mt.dI = Math.Abs(preStepCurrentMean - stepCurrentMean);

        // Calculate time constant from the post-step epoch.
        // This is because we know the level we expect it to return to
        // from the pre-pulse epoch.

        Trace postStepTrace = sweepTrace.SubTraceByEpoch(epochs[2]);
        postStepTrace.SubtractInPlace(preStepCurrentMean);

        // isolate region between a range of the height
        int maxIndex = GetMaximumIndex(postStepTrace.Values);
        double maxValue = postStepTrace.Values[maxIndex];
        int index1 = GetFirstIndexBelow(postStepTrace.Values, .8 * maxValue, maxIndex);
        int index2 = GetFirstIndexBelow(postStepTrace.Values, .2 * maxValue, index1);
        double[] valuesToFit = postStepTrace.Values[index1..index2];

        ExponentialFitter fitter = new(valuesToFit);
        mt.Tau = fitter.Tau / sweepTrace.SampleRate * 1000;
        double extrapolatedPeak = fitter.GetY(-maxIndex);
        double extrapolatedPeakDeltaI = extrapolatedPeak - preStepCurrentMean;
        mt.Ra = mt.dV / extrapolatedPeakDeltaI * 1000;

        // calculate Rm to account for Ra
        mt.Rm = ((mt.dV * 1e-3) - (mt.Ra * 1e6) * (mt.dI * 1e-12)) / (mt.dI * 1e-12) / 1e6;

        // calculate Cm now that Rm is known
        mt.CmStep = (mt.Tau * 1e-3) / (1 / (1 / (mt.Ra * 1e6) + 1 / (mt.Rm * 1e6))) * 1e12;

        return mt;
    }

    public static int GetMaximumIndex(double[] values)
    {
        int maxIndex = 0;
        double maxValue = double.NegativeInfinity;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > maxValue)
            {
                maxValue = values[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    public static int GetFirstIndexBelow(double[] values, double target, int firstIndex = 0)
    {
        for (int i = firstIndex; i < values.Length; i++)
        {
            if (values[i] < target)
                return i;
        };

        throw new InvalidOperationException("values never went below target");
    }
}