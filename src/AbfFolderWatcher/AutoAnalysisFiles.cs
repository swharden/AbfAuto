namespace AbfFolderWatcher;

internal static class AutoAnalysisFiles
{
    /// <summary>
    /// Return a collection of file paths to TIF and ABF files requiring analysis
    /// </summary>
    public static string[] GetFilesNeedingAnalysis(string[] watchedFolders)
    {
        List<string> filesNeedingAnalysis = [];

        foreach (string watchedFolder in watchedFolders)
        {
            if (!Directory.Exists(watchedFolder))
            {
                Status.Error($"Watched folder path does not exist:\n{watchedFolder}");
                continue;
            }
            string[] files = GetFilesNeedingAnalysis(watchedFolder);
            filesNeedingAnalysis.AddRange(files);
        }

        return [.. filesNeedingAnalysis.Order()];
    }

    /// <summary>
    /// Return a collection of file paths to TIF and ABF files requiring analysis
    /// </summary>
    public static string[] GetFilesNeedingAnalysis(string watchedFolder)
    {
        // TIF files that may need analysis
        string[] tifFilePaths = Directory.GetFiles(watchedFolder, "*.tif");

        // ABF files that may need analysis
        List<string> abfFilePaths = [];
        foreach (string abfPath in Directory.GetFiles(watchedFolder, "*.abf"))
        {
            // exclude incomplete (recording in progress) ABF files
            string incompleteAbfFile = abfPath.Replace(".abf", ".rsv");
            if (File.Exists(incompleteAbfFile))
                continue;

            abfFilePaths.Add(abfPath);
        }

        // if no autoanalysis folder exists, all files need analysis
        string analysisFolder = Path.Combine(watchedFolder, "_autoanalysis");
        if (!Directory.Exists(analysisFolder))
        {
            return [.. abfFilePaths, .. tifFilePaths];
        }

        // scan the autoanalysis folder for analyses already done
        string[] pngFilenames = Directory
            .GetFiles(analysisFolder, "*.png")
            .Select(x => Path.GetFileName(x))
            .ToArray();

        List<string> filesNeedingAnalysis = [];

        // TIF files are already analyzed if there is a PNG with the same filename
        foreach (string tifFilePath in tifFilePaths)
        {
            string pngFilename = Path.GetFileName(tifFilePath) + ".png";
            if (!pngFilenames.Contains(pngFilename))
                filesNeedingAnalysis.Add(tifFilePath);
        }

        // ABF files are already analyzed if there are PNG files starting with the ABFID
        foreach (string abfFilePath in abfFilePaths)
        {
            string abfID = Path.GetFileNameWithoutExtension(abfFilePath);

            bool hasAnalysisImages = pngFilenames
                .Where(x => x.StartsWith(abfID, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Contains(".tif.png")) // converted TIFs don't count
                .Any();

            if (!hasAnalysisImages)
                filesNeedingAnalysis.Add(abfFilePath);
        }

        return [.. filesNeedingAnalysis];
    }
}
