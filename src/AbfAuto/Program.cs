namespace AbfAuto;

/// <summary>
/// Code in this file runs when the application is called from the command line.
/// It analyzes a file (ABF or TIF) given as a command line argument.
/// If a folder is given, it will analyze all files in that folder.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            string testPath = @"X:\Data\Alchem\NIMP\11-04-2021\2021_11_04_DIC1_0002.abf";
            args = [testPath];
        }

        if (args.Length != 1)
            throw new ArgumentException("Expected a single argument (path to an ABF file)");

        if (!Path.Exists(args[0]))
            throw new ArgumentException($"Path does not exist: {args[0]}");

        Analyze.Path(args[0]);
    }
}