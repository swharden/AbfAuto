namespace AbfAuto.Watcher;
internal static class Status
{
    public static void Warning(string message)
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{DateTime.Now}] {message}");
    }

    public static void Info(string message)
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"[{DateTime.Now}] {message}");
    }

    public static void Error(string message)
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[{DateTime.Now}] {message}");
    }

    public static void Watching(int folderCount)
    {
        long memoryUsed = GC.GetTotalMemory(false);
        double memoryUsedInMB = memoryUsed / (1024.0 * 1024.0);
        string s = folderCount == 1 ? "" : "s";
        string message = $"Watching {folderCount} folder{s} ({memoryUsedInMB:N2} MB used)";
        message = message.Trim().PadRight(50);
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.CursorLeft = 0;
        Console.Write($"[{DateTime.Now}] {message}");
    }
}
