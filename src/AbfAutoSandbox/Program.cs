/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

namespace AbfAutoSandbox;

public static class Program
{
    public static void Main()
    {
        string folder = @"X:\Data\Alchem\IN-VIVO\Phase-3";
        string protocol = "EEG-3-";
        ManualAnalysis.AnalyzeFolder(folder, protocol);
        //ManualAnalysis.AnalyzeFile(@"X:\Data\Alchem\IN-VIVO\Phase-3\03-03-2023\2023_03_03_EEG_0000.abf");
    }
}