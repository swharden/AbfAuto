namespace AbfAuto.Core;

public static class TifFile
{
    public static string AutoAnalyze(string tifFilePath)
    {
        tifFilePath = Path.GetFullPath(tifFilePath);
        string folder = Path.GetDirectoryName(tifFilePath)!;
        string folderOut = Path.Combine(folder, "_autoanalysis");
        if (!Directory.Exists(folderOut))
            Directory.CreateDirectory(folderOut);
        string pngFileName = Path.GetFileName(tifFilePath) + ".png";
        string pngFilePath = Path.Combine(folderOut, pngFileName);
        Convert(tifFilePath, pngFilePath);
        return pngFilePath;
    }

    private static void Convert(string tifFilePath, string pngFilePath, double ignorePercent = .2)
    {
        SciTIF.TifFile tif = new(tifFilePath);
        SciTIF.Image img = tif.GetImage();
        img.AutoScale(ignorePercent, 100 - ignorePercent);
        img.Save(pngFilePath);
    }
}
