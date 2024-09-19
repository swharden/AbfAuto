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
            string testPath = @"X:\Data\zProjects\Oxytocin Biosensor\experiments\ChR2 stimulation\2024-09-19 ephys\abfs";
            args = [testPath];
        }

        if (args.Length != 1)
            throw new ArgumentException("Expected a single argument (path to an ABF file)");

        Analyze.Path(args[0]);
    }
}