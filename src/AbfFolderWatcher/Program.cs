/* This console application runs continuously on the server.
 * It polls a file that has a list of watched folders
 * and ensures all ABF and TIF files that appear in those folders get analyzed.
 * When new files are seen, it calls AbfAuto.exe to analyze them.
 */

using AbfFolderWatcher;
using System.Diagnostics;

while (true)
{
    string[] watchedFolders = Debugger.IsAttached
        ? [@"X:\Data\zProjects\Oxytocin Biosensor\experiments\ChR2 stimulation\2024-09-18 ephys"] // set this for local testing
        : AutoAnalysisFolders.GetWatchedFolders();

    Status.Watching();

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