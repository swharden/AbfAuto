namespace AbfAutoSandbox;

public static class ManualAnalysis
{
    public static void AnalyzeFolder(string folder, string? protocol = null)
    {
        string[] paths = [.. Directory.GetFiles(folder, "*.abf", SearchOption.AllDirectories)];

        if (!string.IsNullOrEmpty(protocol))
        {
            List<string> paths2 = [];
            foreach (string path in paths)
            {
                AbfSharp.ABF abf = new(path, preloadSweepData: false);
                if (abf.Header.Protocol.Contains(protocol, StringComparison.OrdinalIgnoreCase))
                {
                    paths2.Add(path);
                }
            }
            paths = [.. paths2];
        }

        Analyze(paths);
    }

    public static void AnalyzeFile(string file)
    {
        string[] paths = [file];
        Analyze(paths);
    }

    private static void Analyze(string[] abfPaths, bool deleteOldAnalyses = false)
    {
        for (int i = 0; i < abfPaths.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nAnalyzing {i + 1}/{abfPaths.Length}");

            if (deleteOldAnalyses)
            {
                string abfID = Path.GetFileNameWithoutExtension(abfPaths[i]);
                string analysisFolder = Path.Join(Path.GetDirectoryName(abfPaths[i]), "_autoanalysis");
                string analysisFile = Path.Join(analysisFolder, $"{abfID}_autoanalysis.png");
                if (File.Exists(analysisFile))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Deleting old analysis image: {Path.GetFileName(analysisFile)}");
                    File.Delete(analysisFile);
                }
            }

            string[] savedFiles = AbfAuto.Analyze.AbfFile(abfPaths[i]);
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (string savedFile in savedFiles)
            {
                Console.WriteLine($"Saved: {savedFile}");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
