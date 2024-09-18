using System.Diagnostics;

namespace AbfFolderWatcher;

internal static class AutoAnalyzer
{
    public static void Analyze(string fileToAnalyze)
    {
        ProcessStartInfo processInfo = new(
            fileName: @"X:\Software\AbfAuto\Analyze\AbfAuto.exe",
            arguments: "\"" + fileToAnalyze + "\"");

        Process.Start(processInfo)?.WaitForExit();
    }
}