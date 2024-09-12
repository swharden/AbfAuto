namespace AbfAuto.Gui;

public static class AbfFolderScan
{
    public static string GetAutoAnalysisFolder(string folder)
    {
        return Path.Combine(folder, "_autoanalysis");
    }

    public static (string[] needAnalysis, string[] doNotNeedAnalysis) Scan(string folder)
    {
        string[] abfFiles = Directory.GetFiles(folder, "*.abf");

        string analysisFolder = GetAutoAnalysisFolder(folder);
        string[] analysisFiles =
            Directory.Exists(analysisFolder)
            ? Directory.GetFiles(analysisFolder)
            : [];

        List<string> needAnalysis = [];
        List<string> doNotNeedAnalysis = [];
        foreach (string abfFile in abfFiles)
        {
            string abfID = Path.GetFileNameWithoutExtension(abfFile);
            bool haveAnalysisFiles = analysisFiles.Any(x => Path.GetFileName(x).StartsWith(abfID));

            if (haveAnalysisFiles)
            {
                doNotNeedAnalysis.Add(abfFile);
            }
            else
            {
                needAnalysis.Add(abfFile);
            }
        }

        return (needAnalysis.ToArray(), doNotNeedAnalysis.ToArray());
    }
}
