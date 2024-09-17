using AbfAuto.Watcher;

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