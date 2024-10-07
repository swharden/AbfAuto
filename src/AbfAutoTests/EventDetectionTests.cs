using AbfAuto;
using AbfSharp;
using ScottPlot;

namespace AbfAutoTests;

internal class EventDetectionTests
{
    [Test]
    [Ignore("interactive")]
    public void Test_SlowEventDetection_Respiration()
    {
        string abfPath = Path.Combine(SampleAbfs.Folder, "EEG-3 EEG Resp ECG.abf");
        ABF abf = new(abfPath);

        Sweep sweep = abf.GetAllData(1)
            .SmoothHanning(TimeSpan.FromSeconds(3));

        double[] ddValues = sweep
            .SecondDerivative()
            .ClampedAbove(0) // use opposite clamp for upward events
            .SmoothHanning(TimeSpan.FromSeconds(3))
            .Detrend(TimeSpan.FromSeconds(10))
            .Values;

        int[] peakIndexes = FindPositivePeaks(ddValues);
        int[] antiPeakIndexes = GetPeaksBetween(peakIndexes, sweep.Values);

        LaunchEventInspector(sweep, peakIndexes, antiPeakIndexes);
    }

    public static int[] GetPeaksBetween(int[] indexes, double[] values)
    {
        int[] peakIndexes = new int[indexes.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            int i1 = indexes[i];
            int i2 = i < (indexes.Length - 1) ? indexes[i + 1] : values.Length - 1;
            peakIndexes[i] = -1;
            double peakValue = double.NegativeInfinity;
            for (int j = i1; j < i2; j++)
            {
                if (values[j] > peakValue)
                {
                    peakIndexes[i] = j;
                    peakValue = values[j];
                }
            }
        }
        return peakIndexes;
    }

    public static void LaunchInspector(Sweep sweep)
    {
        ScottPlot.Plot plot = new();
        var sm = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        sm.LineWidth = 2;
        sm.Color = Colors.Black;

        plot.Axes.SetLimitsX(1000, 1000 + 60);
        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }

    public static void LaunchEventInspector(Sweep sweep, int[] peakIndexes, int[] antiPeakIndexes)
    {
        ScottPlot.Plot plot = new();
        var sm = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        sm.LineWidth = 2;
        sm.Color = Colors.Black;

        double[] times1 = peakIndexes.Select(x => x / sweep.SampleRate).ToArray();
        double[] values1 = peakIndexes.Select(x => sweep.Values[x]).ToArray();
        var ms1 = plot.Add.Markers(times1, values1);
        ms1.MarkerColor = Colors.Red;

        double[] times2 = antiPeakIndexes.Select(x => x / sweep.SampleRate).ToArray();
        double[] values2 = antiPeakIndexes.Select(x => sweep.Values[x]).ToArray();
        var ms2 = plot.Add.Markers(times2, values2);
        ms2.MarkerColor = Colors.Blue;

        plot.Axes.SetLimitsX(1000, 1000 + 60);
        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }

    public static int[] FindNegativePeaks(double[] data, double threshold = 0)
    {
        List<int> indexes = [];

        for (int i = 1; i < data.Length - 1; i++)
        {
            if (data[i] <= threshold && data[i] < data[i - 1] && data[i] < data[i + 1])
            {
                indexes.Add(i);
            }
        }

        return [.. indexes];
    }

    public static int[] FindPositivePeaks(double[] data, double threshold = 0)
    {
        List<int> indexes = [];

        for (int i = 1; i < data.Length - 1; i++)
        {
            if (data[i] >= threshold && data[i] > data[i - 1] && data[i] > data[i + 1])
            {
                indexes.Add(i);
            }
        }

        return [.. indexes];
    }

    public static int[] RemoveCloseIndexes(int[] input, int minDistance)
    {
        if (input.Length < 2)
            return input;

        List<int> indexes = [input[0]];
        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] - indexes[^1] < minDistance)
            {
                indexes.Add(input[i]);
            }
        }

        return [.. indexes];
    }
}
