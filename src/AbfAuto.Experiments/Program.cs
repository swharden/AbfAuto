using AbfAuto.Core;

namespace AbfAuto.Experiments;

public static class Program
{
    public static void Main()
    {
        //string abfPath = @"X:/Data/zProjects/SST diabetes/LTS neuron SST/abfs/2024-06-21-DIC1/2024_06_21_0041.abf";
        //ApDectection.Detect(abfPath);

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
                IAnalysis analysis = new Core.Analyses.APFrequencyOverTime();
                Multiplot mp = analysis.Analyze(abf);
                mp.SaveForLabWebsite(abf);
                System.GC.Collect();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        //ApDectection.Detect(@"X:/Data/zProjects/SST diabetes/LTS neuron SST/abfs/2024-06-18-DIC1/2024_06_18_0012.abf");
        //MeasurePulses(@"X:/Data/zProjects/OT-Tom NMDA signaling/Experiments/Testing NMDA unblock/Pulse train experiments/2024-06-21/2024_06_21_0009.abf");
        //MeasurePulses(@"X:/Data/zProjects/OT-Tom NMDA signaling/Experiments/Testing NMDA unblock/Pulse train experiments/2024-06-21/2024_06_21_0012.abf");
        //MeasurePulses(@"X:/Data/zProjects/OT-Tom NMDA signaling/Experiments/Testing NMDA unblock/Pulse train experiments/2024-06-21/2024_06_21_0015.abf");
    }

    /// <summary>
    /// Measure every pulse from all pulse epochs and save results
    /// as individual CSV files (one per epoch)
    /// </summary>
    public static void MeasurePulses(string abfFilePath)
    {
        AbfSharp.ABF abf = new(abfFilePath);

        Enumerable
            .Range(0, abf.Header.AbfFileHeader.fEpochInitLevel.Length)
            .Select(x => new Epoch(abf, x))
            .Where(x => x.EpochTypeCode == 3)
            .ToList().ForEach(x => MeasurePulses(abf, x));
    }

    /// <summary>
    /// Measure every pulse from the given epoch and save the result as a CSV file
    /// </summary>
    public static void MeasurePulses(AbfSharp.ABF abf, Epoch epoch)
    {
        Console.WriteLine($"Analyzing {Path.GetFileName(abf.FilePath)} epoch {epoch.EpochName}");

        SWHarden.CsvBuilder.CsvBuilder csv = new();

        for (int sweepIndex = 0; sweepIndex < abf.SweepCount; sweepIndex++)
        {
            string title = $"sweep {sweepIndex}";
            string units = "pA";
            string comments = $"epoch {epoch.EpochName}";
            double[] values = MeasurePulses(abf, epoch, sweepIndex);
            csv.Add(title, units, comments, values);
        }

        string saveFolder = Path.Combine(Path.GetDirectoryName(abf.FilePath)!, "_autoanalysis");
        string saveFilename = $"{Path.GetFileNameWithoutExtension(abf.FilePath)}_pulses_{epoch.EpochName}.csv";
        string savePath = Path.Combine(saveFolder, saveFilename);
        csv.SaveAs(savePath);
    }

    /// <summary>
    /// Return the mean value of the last 1ms of every pulse in the given epoch
    /// </summary>
    public static double[] MeasurePulses(AbfSharp.ABF abf, Epoch epoch, int sweepIndex)
    {
        float[] sweepValues = abf.GetSweep(sweepIndex);

        List<double> means = [];

        if (epoch.EpochTypeCode != 3)
            throw new InvalidOperationException("epoch is not a pulse epoch");

        for (int i = epoch.IndexFirst; i < epoch.IndexLast; i += epoch.PulsePeriodSamples)
        {
            int i2 = i + epoch.PulseWidthSamples;
            int i1 = i2 - (int)(abf.SampleRate / 1000); // 1 ms from end of pulse
            double mean = sweepValues[i1..i2].Average();
            means.Add(mean);
        }

        return means.ToArray();
    }
}