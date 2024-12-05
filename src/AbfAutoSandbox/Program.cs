/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

namespace AbfAutoSandbox;

public static class Program
{
    public static void Main()
    {
        string folder = @"X:\Data\zProjects\practice\2024-12-02 rotation ECl\2024-12-04";
        string protocol = "0625";
        ManualAnalysis.AnalyzeFolder(folder, protocol);
        //ManualAnalysis.AnalyzeFile(@"X:\Data\zProjects\practice\2024-12-02 rotation ECl\2024-12-04\2024_12_04_0004.abf");
    }
}