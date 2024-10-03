/* This program is used for manual testing and experimentation during development 
 * to create analysis routines which may eventually ybe moved into the AbfAuto project.
 */

using AbfAutoSandbox;
using ScottPlot;
using AbfSharp;

namespace AbfAUtoSandbox;

public static class Program
{
    public static void Main()
    {
        string pathHome = @"C:\Users\scott\University of Florida\Frazier Lab - Documents\Users_Public\Scott\2024-10-02 in vivo event detection\2024_10_01_EEG_0002.abf";
        string pathWork = @"X:\Data\Alchem\IN-VIVO\Phase-4\abfs\2024-10-01\2024_10_01_EEG_0002.abf";
        string abfPath = File.Exists(pathHome) ? pathHome : pathWork;
        ABF abf = new(abfPath);

        BinnedEvents bin = DetectEcg(abf);
        bin.ShowRateOverTime();
    }

    public static BinnedEvents DetectBreathing(ABF abf, int channelIndex = 1)
    {
        // TODO: support recordings that stop breathing entirely
        Sweep sweep = abf.GetAllData(channelIndex: channelIndex);
        CycleDetector detector = new(sweep.Values, sweep.SampleRate);
        detector.ApplySuccessiveSmoothing(1_000);
        detector.ApplySuccessiveDetrend(10_000);
        return detector.GetDownwardCyclesBinned();
    }

    public static BinnedEvents DetectEcg(ABF abf, int channelIndex = 2)
    {
        // TODO: support recordings that stop beating entirely
        Sweep sweep = abf.GetAllData(channelIndex: 2);
        CycleDetector detector = new(sweep.Values, sweep.SampleRate);
        detector.ApplyDerivative();
        detector.ApplyDerivative();
        detector.ApplyRectify();
        double[] original = detector.Trace.ToArray();
        detector.ApplySuccessiveSmoothing(100);
        detector.ApplySuccessiveDetrend(1_000);
        return detector.GetDownwardCyclesBinned();
    }
}