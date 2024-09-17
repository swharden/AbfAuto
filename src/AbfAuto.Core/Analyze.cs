namespace AbfAuto.Core;

public static class Analyze
{
    public static void Path(string path)
    {
        if (Directory.Exists(path))
        {
            Folder(path);
        }
        else
        {
            File(path);
        }
    }

    public static void Folder(string folderPath)
    {
        string[] abfFiles = Directory.GetFiles(folderPath, "*.abf");
        string[] tifFiles = Directory.GetFiles(folderPath, "*.tif");
        string[] files = [.. abfFiles, .. tifFiles];
        Array.Sort(files);

        for (int i = 0; i < files.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Analyzing File {i + 1} of {files.Length}: {System.IO.Path.GetFileName(files[i])}");
            File(files[i]);
        }
    }

    public static void File(string path)
    {
        if (!System.IO.File.Exists(path))
            throw new FileNotFoundException(path);

        string[] saved;

        if (path.EndsWith(".abf"))
        {
            saved = AbfFile(path);
        }
        else if (path.EndsWith(".tif"))
        {
            saved = TifFile(path);
        }
        else
        {
            throw new NotFiniteNumberException($"do not know how to auto-analyze: {System.IO.Path.GetFileName(path)}");
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(string.Join("\n", saved));
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine();
    }

    public static string[] AbfFile(string abfPath)
    {
        return new AbfFileAnalyzer(abfPath).Analyze();
    }

    public static string[] TifFile(string tifFilePath)
    {
        return [Core.TifFile.AutoAnalyze(tifFilePath)];
    }
}
