using AbfAuto.Core;

namespace AbfAuto.Experiments;

public static class ApDectection
{
    public static void Detect(string abfPath)
    {
        AbfSharp.ABF abf = new(abfPath);
        Trace trace = abf.GetAllData();

        int[] eventIndexes = trace.DerivativeThresholdCrossings(10, 0.01); // 10 mV over 10 ms
        double[] eventTimes = eventIndexes.Select(x => x * trace.SamplePeriod).ToArray();
        double[] interEventIntervals = Enumerable
            .Range(0, eventTimes.Length - 1)
            .Select(x => eventTimes[x + 1] - eventTimes[x])
            .ToArray();
        double[] eventFrequencies = interEventIntervals.Select(x => 1 / x).ToArray();

        double[] eventTimes2 = eventTimes[..eventFrequencies.Length].Select(x => x / 60).ToArray();

        ScottPlot.Plot plot = new();
        plot.Add.ScatterPoints(eventTimes2, eventFrequencies);
        plot.YLabel("AP Frequency (Hz)");
        plot.XLabel("Time (minutes)");
        plot.Axes.AutoScale();
        plot.Axes.SetLimits(bottom: 0);

        string abfID = Path.GetFileNameWithoutExtension(abf.FilePath);
        plot.Title(abfID);

        string abfFolder = Path.GetDirectoryName(abf.FilePath)!;
        string analysisFolder = Path.Combine(abfFolder, "_autoanalysis");
        if (!Directory.Exists(analysisFolder))
            Directory.CreateDirectory(analysisFolder);
        string saveAs = Path.Combine(analysisFolder, $"{abfID}_ApTimeCourse.png");
        
        var saved = plot.SavePng(saveAs, 800, 600);
        saved.LaunchInBrowser();
        Console.WriteLine(saved.Path);

        //ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }
}
