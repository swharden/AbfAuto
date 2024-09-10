using ScottPlot;

namespace AbfAuto.Core;

public class AnalysisResult
{
    public List<SizedPlot> Plots { get; } = [];

    public static AnalysisResult Single(Plot plot, int width = 800, int height = 600)
    {
        SizedPlot sp = new(plot, new PixelSize(width, height));
        AnalysisResult result = new();
        result.Plots.Add(sp);
        return result;
    }

    public static AnalysisResult Single(MultiPlot2 multiPlot, int width = 800, int height = 600)
    {
        SizedPlot sp = new(multiPlot, new PixelSize(width, height));
        AnalysisResult result = new();
        result.Plots.Add(sp);
        return result;
    }

    public string[] SaveAll(string saveAsBase)
    {
        List<string> filenames = [];

        int count = 0;
        foreach (SizedPlot sp in Plots)
        {
            string filename = saveAsBase + $"_{count++}.png";
            sp.SavePng(filename);
            filenames.Add(filename);
        }

        return filenames.ToArray();
    }

    public void LaunchInteractive()
    {
        Plots.ForEach(x => x.LaunchInteractive());
    }
}
