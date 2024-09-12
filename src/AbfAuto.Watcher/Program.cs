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

    string[] files = GetFilesNeedingAnalysis(paths);
    AutoAnalyze(files);

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

static void AutoAnalyze(string[] filePaths)
{
    if (filePaths.Length == 0)
        return;

    Console.WriteLine();
    Console.WriteLine($"Located {filePaths.Length} file(s) needing analysis");
    foreach (string filePath in filePaths)
    {
        ProcessStartInfo processInfo = new(AUTO_ANALYSIS_EXE, "\"" + filePath + "\"")
        {
            CreateNoWindow = false,
        };

        Process? process = Process.Start(processInfo);
        process?.WaitForExit();
    }
}

static string[] GetFilesNeedingAnalysis(string[] folderPaths)
{
    List<string> filesNeedingAnalysis = [];

    foreach (string folderPath in folderPaths)
    {
        string[] tifFilePaths = Directory.GetFiles(folderPath, "*.tif");

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
            filesNeedingAnalysis.AddRange(abfFilePaths);
            filesNeedingAnalysis.AddRange(tifFilePaths);
            continue;
        }

        string[] pngFilenames = Directory
            .GetFiles(analysisFolder, "*.png")
            .Select(x => Path.GetFileName(x))
            .ToArray();

        foreach (string tifFilePath in tifFilePaths)
        {
            string pngFilename = Path.GetFileName(tifFilePath) + ".png";
            if (!pngFilenames.Contains(pngFilename))
                filesNeedingAnalysis.Add(tifFilePath);
        }

        foreach (string abfFilePath in abfFilePaths)
        {
            string abfID = Path.GetFileNameWithoutExtension(abfFilePath);

            bool hasAnalysisImages = pngFilenames
                .Where(x => x.StartsWith(abfID, StringComparison.InvariantCultureIgnoreCase))
                .Any();

            if (!hasAnalysisImages)
                filesNeedingAnalysis.Add(abfFilePath);
        }
    }

    return filesNeedingAnalysis.ToArray();
}

static void Status(string message)
{
    message = message.Trim().PadRight(50);
    message = $"[{DateTime.Now}] {message}";
    Console.CursorVisible = false;
    Console.CursorLeft = 0;
    Console.ForegroundColor = ConsoleColor.Gray;
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