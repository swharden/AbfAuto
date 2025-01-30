/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

using AbfSharp;

namespace AbfAutoSandbox;

public static class Program
{
    public static void Main()
    {
        string analyzeThis = @"X:\Data\zProjects\Aging and DA\BLA LTP\abfs\2025-01-30-DIC2-practice\2025_01_30_0027.abf";

        string? analyzeOnlyAbfsWithProtocol = "0615";

        if (File.Exists(analyzeThis))
        {
            ManualAnalysis.AnalyzeFile(analyzeThis);
        }
        else
        {
            ManualAnalysis.AnalyzeFolder(analyzeThis, analyzeOnlyAbfsWithProtocol);
        }
    }
}