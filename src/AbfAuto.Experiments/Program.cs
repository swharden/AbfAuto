namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string abfPath = @"X:/Data/zProjects/SST diabetes/LTS neuron SST/abfs/2024-06-20-DIC1/2024_06_20_0018.abf";
        AbfSharp.ABF abf = new(abfPath);

        ScottPlot.Plot plot = new();

        foreach (var sweep in abf.GetSweeps())
        {
            var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
            sig.Data.YOffset = 100 * sweep.Index;
        }

        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }
}