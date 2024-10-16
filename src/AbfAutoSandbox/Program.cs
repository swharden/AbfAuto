/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

using System.Text;

namespace AbfAutoSandbox;

public static class Program
{
    public static void Main()
    {
        string folder = @"X:\Data\zProjects\OT-Tom dendritic conductivity and Calcium homeostasis\Experiments";
        string protocol = "0804";
        ManualAnalysis.AnalyzeFolder(folder, protocol);
        //ManualAnalysis.AnalyzeFile(@"X:\Data\zProjects\OT-Tom dendritic conductivity and Calcium homeostasis\Experiments\Time-matched CTs\2023-01-25\abfs\2023_01_25_0031.abf");
    }
}