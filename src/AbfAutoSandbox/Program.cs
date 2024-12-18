/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

namespace AbfAutoSandbox;

public static class Program
{
    public static void Main()
    {
        string folderOfDailyFolders = @"X:\Data\zProjects\practice\2024-12-02 rotation ECl";
        foreach (string subFolder in Directory.GetDirectories(folderOfDailyFolders))
        {
            ManualAnalysis.AnalyzeFolder(subFolder, "0625");
        }
        //ManualAnalysis.AnalyzeFile(@"X:\Data\zProjects\practice\2024-12-02 rotation ECl\2024-12-17\2024_12_17_0017.abf");
    }
}