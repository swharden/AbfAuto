namespace AbfAuto.Core;

public static class Analyze
{
    public static void AnalyzeAbfFolder(string folderPath, bool overwrite = true)
    {
        string[] abfFilePaths = Directory.GetFiles(folderPath, "*.abf");
        for (int i = 0; i < abfFilePaths.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Analyzing ABF {i + 1} of {abfFilePaths.Length}");
            AnalyzeAbfFile(abfFilePaths[i], overwrite);
        }
    }

    public static string[] AnalyzeAbfFile(string abfPath, bool overwrite = true)
    {
        return new AbfFileAnalyzer(abfPath).Analyze(overwrite);
    }
}
