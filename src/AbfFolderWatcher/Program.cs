/* This console application runs continuously on the server.
 * It polls a file that has a list of watched folders
 * and ensures all ABF and TIF files that appear in those folders get analyzed.
 * When new files are seen, it calls AbfAuto.exe to analyze them.
 */

using AbfFolderWatcher;

while (true)
{
    string[] watchedFolders = AutoAnalysisFolders.GetWatchedFolders();
    Status.Watching(watchedFolders.Length);

    string[] filesNeedingAnalysis = AutoAnalysisFiles.GetFilesNeedingAnalysis(watchedFolders);
    foreach (string filePath in filesNeedingAnalysis)
    {
        Console.WriteLine();
        Status.Info($"Analyzing {filePath}");
        AutoAnalyzer.Analyze(filePath);
        Console.WriteLine();
    }

    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();

    Thread.Sleep(1000);
}