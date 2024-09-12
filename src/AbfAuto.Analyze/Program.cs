if (System.Diagnostics.Debugger.IsAttached)
    args = [@"X:\Data\Alchem\Donepezil\BLA\07-28-2021"];

if (args.Length != 1)
    throw new ArgumentException("Expected a single argument (path to an ABF file or folder)");

string path = Path.GetFullPath(args[0]);
string[] abfFilePaths = Directory.Exists(args[0]) ? Directory.GetFiles(path, "*.abf") : [path];

var sw = System.Diagnostics.Stopwatch.StartNew();
foreach (string abfFilePath in abfFilePaths)
{
    string[] savedFiles = AbfAuto.Core.Analyze.AnalyzeAbfFile(abfFilePath);
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine(string.Join("\n", savedFiles));
}

Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine($"Analyzed {abfFilePaths.Length} file(s) in {sw.Elapsed.TotalSeconds:0.00} sec");