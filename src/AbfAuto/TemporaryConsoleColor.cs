namespace AbfAuto;

public class TemporaryConsoleColor : IDisposable
{
    private readonly ConsoleColor OriginalForegroundColor;
    private readonly ConsoleColor OriginalBackgroundColor;

    public TemporaryConsoleColor(ConsoleColor foreground)
    {
        OriginalForegroundColor = Console.ForegroundColor;
        OriginalBackgroundColor = Console.BackgroundColor;
        Console.ForegroundColor = foreground;
    }

    public TemporaryConsoleColor(ConsoleColor foreground, ConsoleColor background)
    {
        OriginalForegroundColor = Console.ForegroundColor;
        OriginalBackgroundColor = Console.BackgroundColor;
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
    }

    public void Dispose()
    {
        Console.ForegroundColor = OriginalForegroundColor;
        Console.BackgroundColor = OriginalBackgroundColor;
    }
}
