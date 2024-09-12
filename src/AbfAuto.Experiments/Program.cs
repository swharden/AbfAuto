using AbfAuto.Core;

string filePath = @"X:\Data\zProjects\OT-Tom dendritic conductivity and Calcium homeostasis\Experiments\alpha MSH\2024-09-03_ 1 MicroM\abfs\2024_09_03_0000.abf";
AnalyzeAbfFile(filePath);

static void AnalyzeAbfFile(string filePath)
{
    AbfFileAnalyzer analyzer = new(filePath);
    string[] saved = analyzer.Analyze();
    Console.WriteLine(string.Join("\n", saved));
}