namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        string abfPath = @"X:/Data/zProjects/SST diabetes/LTS neuron SST/abfs/2024-06-20-DIC1/2024_06_20_0018.abf";
        AbfSharp.ABF abf = new(abfPath);

        AbfSharp.Sweep sweep = abf.GetSweep(0);
        AbfSharp.Sweep deriv = sweep.FirstDerivative(0.010);

        ScottPlot.Plot plot = new();
        plot.Add.Signal(deriv.Values, deriv.SamplePeriod);

        ScottPlot.WinForms.FormsPlotViewer.Launch(plot);
    }
}