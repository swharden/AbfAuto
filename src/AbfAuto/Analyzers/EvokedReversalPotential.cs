using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

internal class EvokedReversalPotential : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        Plot plot1 = new();
        plot1.YLabel("Δ Current (pA)");
        plot1.XLabel("Time (sec)");
        plot1.Title(abf.AbfID);

        int stimEpochIndex = 2;

        // baseline to the 500 ms before the stimulation
        double baselineTime2 = abf.Epochs[stimEpochIndex].StartTime;
        double baselineTime1 = baselineTime2 - 0.5;

        // measure the first 50 ms after the stimulation
        double measureTime = 0.020;
        double measureTime1 = abf.Epochs[stimEpochIndex].EndTime;
        double measureTime2 = measureTime1 + measureTime;
        int measureIndex1 = (int)(measureTime1 * abf.SampleRate);
        int measureIndex2 = (int)(measureTime2 * abf.SampleRate);
        var hSpan = plot1.Add.HorizontalSpan(measureTime1, measureTime2);
        hSpan.FillColor = Colors.Yellow.WithAlpha(.2);

        // prepare index range for the stimulus artifact
        double artifactOffset = 0.003;
        int artifactIndex1 = (int)(abf.Epochs[stimEpochIndex].StartTime * abf.SampleRate);
        int artifactIndex2 = artifactIndex1 + (int)(artifactOffset * abf.SampleRate);

        var vline = plot1.Add.VerticalLine(abf.Epochs[stimEpochIndex].StartTime);
        vline.Color = Colors.Red.WithAlpha(.5);
        vline.LinePattern = LinePattern.DenselyDashed;
        vline.LineWidth = 2;

        var hline = plot1.Add.HorizontalLine(0, 2, Colors.Black, LinePattern.DenselyDashed);

        double[] currentsBySweep = new double[abf.SweepCount];
        double[] potentialsBySweep = new double[abf.SweepCount];

        for (int i = 0; i < abf.SweepCount; i++)
        {
            var sweep = abf.GetSweep(i);

            // baseline subtraction
            double baseline = sweep.MeanOfTimeRange(baselineTime1, baselineTime2);
            sweep.SubtractInPlace(baseline);

            // blank-out the artifact
            for (int j = artifactIndex1; j <= artifactIndex2; j++)
                sweep.Values[j] = 0;

            // measure area under the curve
            for (int j = measureIndex1; j <= measureIndex2; j++)
                currentsBySweep[i] += sweep.Values[j];
            currentsBySweep[i] /= (measureIndex2 - measureIndex1);

            // note the voltage
            potentialsBySweep[i] = abf.Epochs[stimEpochIndex].Level + abf.Epochs[stimEpochIndex].LevelDelta * i;

            var sig = plot1.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.LineWidth = 1.5f;
            sig.AlwaysUseLowDensityMode = true;
            sig.Color = Colors.C0.WithOpacity(.5);
        }

        plot1.Axes.SetLimitsX(measureTime1 - 0.1, measureTime1 + 0.1);

        Plot plot2 = new();
        plot2.YLabel("Current (pA)");
        plot2.XLabel("Potential (mV)");

        plot2.Add.HorizontalLine(0, 1, Colors.Black, LinePattern.DenselyDashed);

        var markers = plot2.Add.Markers(potentialsBySweep, currentsBySweep);
        markers.Color = Colors.C0;
        markers.MarkerShape = MarkerShape.OpenCircle;
        markers.MarkerSize = 14;
        markers.MarkerLineWidth = 2;

        List<Coordinates> pointsToFit = [new(potentialsBySweep[0], currentsBySweep[0])];
        for (int i = 1; i < abf.SweepCount; i++)
        {
            // stop fitting once the current starts decreasing (sign of unclamped AP)
            if (currentsBySweep[i] < currentsBySweep[i - 1])
            {
                for (int j = i; j < abf.SweepCount; j++)
                {
                    var xMark = plot2.Add.Marker(potentialsBySweep[j], currentsBySweep[j]);
                    xMark.Color = Colors.Red.WithAlpha(.5);
                    xMark.LineWidth = 3;
                    xMark.Shape = MarkerShape.Eks;
                    xMark.Size = 15;
                }
                break;
            }
            pointsToFit.Add(new(potentialsBySweep[i], currentsBySweep[i]));
        }

        ScottPlot.Statistics.LinearRegression fit = new(pointsToFit);
        double reversal = -fit.Offset / fit.Slope;
        plot2.Title($"Reversal = {reversal:0.00} mV");

        double x1 = pointsToFit.First().X;
        double x2 = pointsToFit.Last().X;
        Coordinates pt1 = new(x1, fit.GetValue(x1));
        Coordinates pt2 = new(x2, fit.GetValue(x2));
        var line = plot2.Add.Line(pt1, pt2);
        line.LineWidth = 3;
        line.LineColor = Colors.Red.WithAlpha(.5);
        line.LinePattern = LinePattern.DenselyDashed;

        MultiPlot2 mp = new();
        mp.AddSubplot(plot1, 0, 1, 0, 2);
        mp.AddSubplot(plot2, 0, 1, 1, 2);

        return AnalysisResult.Single(mp);
    }
}
