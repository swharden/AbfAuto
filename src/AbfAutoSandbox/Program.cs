/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

namespace AbfAUtoSandbox;

public static class Program
{
    public static void Main()
    {
        string[] abfPaths = Directory.GetFiles(@"X:\Data\Alchem\IN-VIVO\Phase-4\abfs\", "*.abf", SearchOption.AllDirectories);
        //string[] abfPaths = [@"X:\Software\ABF protocol tests\abfs\EEG-3 EEG Resp ECG.abf"];
        for (int i = 0; i < abfPaths.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Analyzing {i + 1}/{abfPaths.Length}");
            Console.ForegroundColor = ConsoleColor.Gray;
            string[] savedFiles = AbfAuto.Analyze.AbfFile(abfPaths[i]);
            foreach (string savedFile in savedFiles)
            {
                Console.WriteLine($"Saved: {savedFile}");
            }
        }
    }
}