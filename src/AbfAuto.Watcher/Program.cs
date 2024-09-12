/* This application runs continuously and analyzes new ABF files as they are saved.
 * Only "watched folders" defined in a text file on the X-drive are monitored.
 */

using System.Diagnostics;

const string AUTO_ANALYSIS_FILE = @"X:\Lab Documents\network\autoAnalysisFolders.txt";
const string AUTO_ANALYSIS_EXE = @"X:\Software\AbfAuto\Analyze\AbfAuto.Analyze.exe";

while (true)
{
    string[] paths = GetWatchedFolders();
    string s = paths.Length == 1 ? "" : "s";
    Status($"Watching {paths.Length} folder{s} ({GetMemoryUsed():N2} MB used)");

    string[] abfFilesNeedingAnalysis = GetAbfFilesNeedingAnalysis(paths);
    AutoAnalyzeAbfs(abfFilesNeedingAnalysis);

    Thread.Sleep(1000);
}

static string[] GetWatchedFolders()
{
    try
    {
        return File.ReadAllLines(AUTO_ANALYSIS_FILE).Where(x => x.Contains(':')).ToArray();
    }
    catch (IOException ex)
    {
        Status(ex.Message);
        return [];
    }
}

static void AutoAnalyzeAbfs(string[] abfFilesNeedingAnalysis)
{
    if (abfFilesNeedingAnalysis.Length == 0)
        return;

    Console.WriteLine();
    Console.WriteLine($"Located {abfFilesNeedingAnalysis.Length} ABF file(s) needing analysis");
    foreach (string abfFilePath in abfFilesNeedingAnalysis)
    {
        ProcessStartInfo processInfo = new(AUTO_ANALYSIS_EXE, abfFilePath)
        {
            CreateNoWindow = false,
        };

        Process? process = Process.Start(processInfo);
        process?.WaitForExit();
    }
}

static string[] GetAbfFilesNeedingAnalysis(string[] folderPaths)
{
    List<string> abfFilesNeedingAnalysis = [];

    foreach (string folderPath in folderPaths)
    {
        List<string> abfFilePaths = [];
        foreach (string abfPath in Directory.GetFiles(folderPath, "*.abf"))
        {
            string incompleteAbfFile = abfPath.Replace(".abf", ".rsv");
            if (File.Exists(incompleteAbfFile))
                continue;

            abfFilePaths.Add(abfPath);
        }

        string analysisFolder = Path.Combine(folderPath, "_autoanalysis");
        if (!Directory.Exists(analysisFolder))
        {
            abfFilesNeedingAnalysis.AddRange(abfFilePaths);
            continue;
        }

        string[] pngFiles = Directory.GetFiles(analysisFolder, "*.png");
        foreach (string abfFilePath in abfFilePaths)
        {
            string abfID = Path.GetFileNameWithoutExtension(abfFilePath);

            bool hasAnalysisImages = pngFiles
                .Select(x => Path.GetFileName(x))
                .Where(x => x.StartsWith(abfID, StringComparison.InvariantCultureIgnoreCase))
                .Any();

            if (!hasAnalysisImages)
                abfFilesNeedingAnalysis.Add(abfFilePath);
        }
    }

    return abfFilesNeedingAnalysis.ToArray();
}

static void Status(string message)
{
    message = message.Trim().PadRight(50);
    message = $"[{DateTime.Now}] {message}";
    Console.CursorVisible = false;
    Console.CursorLeft = 0;
    Console.Write(message);
}

static double GetMemoryUsed()
{
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    long memoryUsed = GC.GetTotalMemory(false);
    double memoryUsedInMB = memoryUsed / (1024.0 * 1024.0);
    return memoryUsedInMB;
}