using ScottPlot;

namespace AbfAuto.Core;
public class Figure
{
    public static SavedImageInfo SaveAnalysisFigure(Plot plot, AbfSharp.ABF abf, string name, int width = 800, int height = 600)
    {
        string abfFolder = Path.GetDirectoryName(Path.GetFullPath(abf.FilePath))
            ?? throw new InvalidOperationException();

        string analysisFolder = Path.Combine(abfFolder, "_autoanalysis");

        if (!Directory.Exists(analysisFolder))
        {
            Directory.CreateDirectory(analysisFolder);
        }

        string saveAs = Path.Combine(analysisFolder, $"{abf.AbfID}_AbfSharp_{name}.png");
        return plot.SavePng(saveAs, width, height);
    }
}
