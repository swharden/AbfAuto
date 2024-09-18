namespace AbfFolderWatcher;
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

    public static void Watching()
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.CursorLeft = 0;
        Console.Write("Watching for new ABFs");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        //string[] spinner = { "..   ", "...  ", ".... ", ".....", " ....", "  ...", "   ..", ".   ." };
        string[] spinner = { "—", "\\", "|", "/" };
        string symbol = spinner[DateTime.Now.Second % spinner.Length];
        Console.Write($" {symbol} ");

        Console.ForegroundColor = ConsoleColor.Gray;
        long memoryUsed = GC.GetTotalMemory(false);
        double memoryUsedInMB = memoryUsed / (1024.0 * 1024.0);
        Console.Write($"{memoryUsedInMB:N3} MB in use");
        Console.Write("     ");
    }
}
