using AbfAuto.Core;
using AbfAuto.Core.Extensions;
using AbfAuto.Core.SortLater;

namespace AbfAuto.Experiments;

internal class MOVE_AP
{
    public static void Run()
    {
        string folder = @"X:\Data\zProjects\SST diabetes\LTS neuron SST\abfs\";
        string[] abfFiles = Directory.GetFiles(folder, "*.abf", SearchOption.AllDirectories);

        for (int i = 0; i < abfFiles.Length; i++)
        {
            AbfSharp.AbfFileInfo info = new(abfFiles[i]);
            Console.WriteLine($"ABF {i + 1} of {abfFiles.Length}: {info.Protocol}");

            if (info.Protocol.Contains("0301"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                AbfSharp.ABF abf = new(abfFiles[i]);
                IAnalyzer analysis = new APFrequencyOverTime();
                Multiplot mp = analysis.Analyze(abf);
                mp.SaveForLabWebsite(abf);
                System.GC.Collect();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
