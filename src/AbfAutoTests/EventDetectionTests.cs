using AbfAuto;
using AbfSharp;
using ScottPlot;

namespace AbfAutoTests;

internal class EventDetectionTests
{
    [Test]
    public void Test_SlowEventDetection_Respiration()
    {
        string abfPath = Path.Combine(Paths.SampleAbfFolder, "EEG-3 EEG Resp ECG.abf");
        ABF abf = new(abfPath);

        Sweep sweep = abf.GetAllData(1)
            .SmoothHanning(TimeSpan.FromSeconds(3));

        double[] ddValues = sweep
            .SecondDerivative()
            .ClampedAbove(0) // use opposite clamp for upward events
            .SmoothHanning(TimeSpan.FromSeconds(3))
            .Detrend(TimeSpan.FromSeconds(10))
            .Values;

        int[] indexes = FindPositivePeaks(ddValues);

        LaunchEventInspector(sweep, indexes);
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

    public static void LaunchEventInspector(Sweep sweep, int[] indexes)
    {
        ScottPlot.Plot plot = new();
        var sm = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        sm.LineWidth = 2;
        sm.Color = Colors.Black;

        double[] times = indexes.Select(x => x / sweep.SampleRate).ToArray();
        double[] values = indexes.Select(x => sweep.Values[x]).ToArray();
        var ms = plot.Add.Markers(times, values);
        ms.MarkerColor = Colors.Red;

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
