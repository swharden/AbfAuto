namespace AbfAuto;

public class AbfFolderAnalyzer
{
    public string[] AbfFilePaths { get; }
    public int NextIndexToAnalyze { get; private set; } = 0;

    public AbfFolderAnalyzer(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException(folderPath);

        AbfFilePaths = Directory.GetFiles(folderPath, "*.abf");
    }

    public void AnalyzeAll(bool overwrite = true)
    {
        while (NextIndexToAnalyze < AbfFilePaths.Length)
        {
            AnalyzeNext();
        }
    }

    public void AnalyzeIndex(int index, bool overwrite = true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Analyzing ABF {index + 1} of {AbfFilePaths.Length}");

        AbfFileAnalyzer analyzer = new(AbfFilePaths[index]);
        string[] savedFiles = analyzer.Analyze(overwrite);

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(string.Join("\n", savedFiles));

        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public void AnalyzeNext(bool overwrite = true)
    {
        AnalyzeIndex(NextIndexToAnalyze++);
    }
}
