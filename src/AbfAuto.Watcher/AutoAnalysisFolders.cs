namespace AbfAuto.Watcher;

internal static class AutoAnalysisFolders
{
    const string AUTO_ANALYSIS_FILE = @"X:\Lab Documents\network\autoAnalysisFolders.txt";

    /// <summary>
    /// Return a collection of folder paths to watch for new ABFs requiring auto-analysis
    /// </summary>
    public static string[] GetWatchedFolders()
    {
        if (!File.Exists(AUTO_ANALYSIS_FILE))
            return [];

        try
        {
            return File.ReadAllLines(AUTO_ANALYSIS_FILE).Where(x => x.Contains(':')).ToArray();
        }
        catch (IOException ex)
        {
            Status.Error(ex.Message);
            return [];
        }
    }
}
