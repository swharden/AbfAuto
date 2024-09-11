using System.Diagnostics;

namespace AbfAuto.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        if (Debugger.IsAttached)
        {
            System.Console.WriteLine("DEBUG MODE");
            args = [@"X:\Data\Alchem\Donepezil\BLA\07-28-2021\2021_07_28_DIC1_0000.abf"];
        }

        if (args is null || args.Length != 1)
        {
            throw new ArgumentException("A single argument (ABF file path) is required");
        }

        string abfPath = args[0];

        if (!abfPath.EndsWith(".abf", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Path must be a .abf file");
        }

        if (!File.Exists(abfPath))
        {
            throw new FileNotFoundException(abfPath);
        }

        Core.Analyze.AnalyzeAbfFile(abfPath);
    }
}