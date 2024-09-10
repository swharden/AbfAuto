namespace AbfAuto.Core;

public class TemporaryConsoleColor : IDisposable
{
    private readonly ConsoleColor OriginalColor;

    public TemporaryConsoleColor(ConsoleColor color)
    {
        OriginalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
    }

    public void Dispose()
    {
        Console.ForegroundColor = OriginalColor;
    }
}
