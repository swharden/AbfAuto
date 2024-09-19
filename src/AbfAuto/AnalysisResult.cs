using ScottPlot;

namespace AbfAuto;

public class AnalysisResult
{
    List<SizedPlot> Plots { get; } = [];
    List<AnalysisTextFile> TextFiles { get; } = [];
    readonly record struct AnalysisTextFile(string Name, string Contents);

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

    public AnalysisResult WithTextFile(string name, string contents)
    {
        AnalysisTextFile file = new(name, contents);
        TextFiles.Add(file);
        return this;
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

        foreach (AnalysisTextFile file in TextFiles)
        {
            string filename = saveAsBase + $"_{file.Name}_{count++}.txt";
            File.WriteAllText(filename, file.Contents);
            filenames.Add(filename);
        }

        return [.. filenames];
    }
}
