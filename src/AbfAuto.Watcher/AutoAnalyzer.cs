using System.Diagnostics;

namespace AbfAuto.Watcher;

internal static class AutoAnalyzer
{
    const string AUTO_ANALYSIS_EXE = @"X:\Software\AbfAuto\Analyze\AbfAuto.Analyze.exe";

    public static void Analyze(string fileToAnalyze)
    {
        ProcessStartInfo processInfo = new(AUTO_ANALYSIS_EXE, "\"" + fileToAnalyze + "\"")
        {
            CreateNoWindow = false,
        };

        Process? process = Process.Start(processInfo);
        process?.WaitForExit();
    }
}
