using AbfAuto.Core.Operations;
using AbfAuto.Core.SortLater;

namespace AbfAuto.Core.Memtest;

public static class MemtestLogic
{
    public static MemtestResult GetMeanMemtest(AbfSharp.ABF abf)
    {
        MemtestResult?[] allResults = GetMemtestBySweep(abf);

        MemtestResult[] valid = allResults.Where(x => x is not null).Select(x => (MemtestResult)x!).ToArray();

        if (valid.Length == 0)
            return new();

        return new()
        {
            Ih = valid.Select(x => x.Ih).Average(),
            Ra = valid.Select(x => x.Ra).Average(),
            Rm = valid.Select(x => x.Rm).Average(),
            CmStep = valid.Select(x => x.CmStep).Average(),
            Tau = valid.Select(x => x.Tau).Average(),
        };
    }

    public static MemtestResult?[] GetMemtestBySweep(AbfSharp.ABF abf)
    {
        return Enumerable
            .Range(0, abf.SweepCount)
            .Select(x => TryGetSweepMemtest(abf, x))
            .ToArray();
    }

    public static MemtestResult? TryGetSweepMemtest(AbfSharp.ABF abf, int sweepIndex)
    {
        try
        {
            return CalculateMemtest(abf, sweepIndex);
        }
        catch
        {
            return null;
        }
    }

    private static MemtestResult CalculateMemtest(AbfSharp.ABF abf, int sweepIndex)
    {
        Trace sweepTrace = new(abf, sweepIndex);

        MemtestResult mt = new();

        // Get dV and dI from the hyperpolarizing step epoch relative to the pre-step epoch
        Epoch[] epochs = Enumerable.Range(0, abf.Header.AbfFileHeader.fEpochInitLevel.Length).Select(x => new Epoch(abf, x)).ToArray();

        int downwardEpochIndex = 0;
        for (int i = 1; i < epochs.Length; i++)
        {
            if (epochs[i].Level < epochs[i - 1].Level)
            {
                downwardEpochIndex = i;
                break;
            }
        }

        Trace preStepTrace = (downwardEpochIndex == 0)
            ? sweepTrace.SubTraceByIndex(0, epochs[0].IndexFirst)
            : sweepTrace.SubTraceByEpoch(epochs[downwardEpochIndex - 1]);

        Trace stepTrace = sweepTrace.SubTraceByEpoch(epochs[downwardEpochIndex]);

        int postStepIndex1 = epochs[downwardEpochIndex].IndexLast;
        int postStepIndex2 = postStepIndex1 + stepTrace.Values.Length;
        Trace postStepTrace = sweepTrace.SubTraceByIndex(postStepIndex1, postStepIndex2);

        double preStepCurrentMean = preStepTrace.Mean();
        double stepCurrentMean = stepTrace.SubTraceByFraction(0.75, 1).Mean();
        double beforeLevel = (downwardEpochIndex == 0)
            ? abf.Header.AbfFileHeader.fDACHoldingLevel[0]
            : epochs[downwardEpochIndex - 1].Level;
        double stepLevel = epochs[downwardEpochIndex].Level;

        mt.Ih = preStepCurrentMean;
        mt.dV = Math.Abs(beforeLevel - stepLevel);
        mt.dI = Math.Abs(preStepCurrentMean - stepCurrentMean);

        // Calculate time constant from the post-step epoch.
        // This is because we know the level we expect it to return to
        // from the pre-pulse epoch.

        postStepTrace.SubtractInPlace(preStepCurrentMean);

        // isolate region between a range of the height
        int maxIndex = postStepTrace.GetMaximumIndex();
        double maxValue = postStepTrace.Values[maxIndex];
        int index1 = postStepTrace.GetFirstIndexBelow(.8 * maxValue, maxIndex);
        int index2 = postStepTrace.GetFirstIndexBelow(.2 * maxValue, index1);
        double[] valuesToFit = postStepTrace.Values[index1..index2];

        ExponentialFitter fitter = new(valuesToFit, preStepCurrentMean);
        mt.Tau = fitter.Tau / sweepTrace.SampleRate * 1000;
        double extrapolatedPeak = fitter.GetY(-maxIndex);
        double extrapolatedPeakDeltaI = extrapolatedPeak - preStepCurrentMean;
        mt.Ra = mt.dV / extrapolatedPeakDeltaI * 1000;

        // calculate Rm to account for Ra
        mt.Rm = ((mt.dV * 1e-3) - (mt.Ra * 1e6) * (mt.dI * 1e-12)) / (mt.dI * 1e-12) / 1e6;

        // calculate Cm now that Rm is known
        mt.CmStep = (mt.Tau * 1e-3) / (1 / (1 / (mt.Ra * 1e6) + 1 / (mt.Rm * 1e6))) * 1e12;

        return mt;
    }
}
