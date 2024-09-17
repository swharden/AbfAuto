using AbfSharp;

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
            CmRamp = valid.Select(x => x.CmRamp).Average(),
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

    private static MemtestResult CalculateMemtest(AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        Sweep sweep = abf.GetSweep(sweepIndex, channelIndex);

        // add values to the membrane test as they are calculated
        MemtestResult mt = new();

        // identify the membrane test epoch (the first hyperpolarization pulse)
        int downwardEpochIndex = GetHyperpolarizingStepIndex(abf);

        // isolate the segments before, during, and after the hyperpolarization pulse
        Sweep preStep = (downwardEpochIndex == 0)
            ? sweep.SubTraceByIndex(0, abf.Epochs[0].IndexFirst)
            : sweep.SubTraceByEpoch(abf.Epochs[downwardEpochIndex - 1]);
        Sweep step = sweep.SubTraceByEpoch(abf.Epochs[downwardEpochIndex]);
        int postIndex1 = abf.Epochs[downwardEpochIndex].IndexLast;
        int postIndex2 = postIndex1 + step.Values.Length;
        Sweep postStep = sweep.SubTraceByIndex(postIndex1, postIndex2);

        // determine steady state currents and dI
        double preStepCurrentMean = preStep.Values.Average();
        double stepCurrentMean = step.SubTraceByFraction(0.75, 1).Values.Average();
        mt.Ih = preStepCurrentMean;
        mt.dI = Math.Abs(preStepCurrentMean - stepCurrentMean);

        // determine the dV
        double preStepVoltage = (downwardEpochIndex == 0)
            ? abf.Header.AbfFileHeader.fDACHoldingLevel[0]
            : abf.Epochs[downwardEpochIndex - 1].Level;
        double stepVoltage = abf.Epochs[downwardEpochIndex].Level;
        mt.dV = Math.Abs(preStepVoltage - stepVoltage);

        // The capacitive transient after the hyperpolarization step will be further analyzed.
        // Subtract steady state current so the exponential curve ends at 0, simplifying curve fitting.
        postStep.SubtractInPlace(preStepCurrentMean);

        // isolate region to fit (points not too close to the peak or steady state level)
        int maxIndex = postStep.GetMaximumIndex();
        double maxValue = postStep.Values[maxIndex];
        int index1 = postStep.GetFirstIndexBelow(.8 * maxValue, maxIndex);
        int index2 = postStep.GetFirstIndexBelow(.2 * maxValue, index1);
        double[] valuesToFit = postStep.Values[index1..index2];

        // fit the curve to get the time constant
        ExponentialFitter fitter = new(valuesToFit, preStepCurrentMean);

        // get time constant from the fitter
        mt.Tau = fitter.Tau / sweep.SampleRate * 1000;

        // extrapolate backwards to predict the instantaneous peak
        double extrapolatedPeak = fitter.GetY(-maxIndex);
        double extrapolatedPeakDeltaI = extrapolatedPeak - preStepCurrentMean;

        // calculate the remaining memtest properties now that we have all the values
        mt.Ra = mt.dV / extrapolatedPeakDeltaI * 1000;
        mt.Rm = ((mt.dV * 1e-3) - (mt.Ra * 1e6) * (mt.dI * 1e-12)) / (mt.dI * 1e-12) / 1e6;
        mt.CmStep = (mt.Tau * 1e-3) / (1 / (1 / (mt.Ra * 1e6) + 1 / (mt.Rm * 1e6))) * 1e12;
        mt.CmRamp = GetCmRamp(sweep, abf);

        return mt;
    }

    private static double GetCmRamp(Sweep sweep, ABF abf)
    {
        int rampIndex = GetHyperpolarizingRampIndex(abf);
        if (rampIndex == 0)
            return 0;

        // isolate the two ramps
        Epoch falling = abf.Epochs[rampIndex];
        Epoch rising = abf.Epochs[rampIndex + 1];

        // measure the rate of the voltage change
        double dV = rising.Level - falling.Level;
        double dT = falling.EndTime - falling.StartTime;
        double dVslope = dV / dT;

        // get the values for each epoch and reverse one of them
        double[] values1 = sweep.Values[falling.IndexFirst..falling.IndexLast];
        double[] values2 = sweep.Values[rising.IndexFirst..rising.IndexLast];
        if (values1.Length != values2.Length)
            throw new InvalidOperationException("ramps must have equal length");
        Array.Reverse(values2);

        // get the mean difference between each ramp and the center
        int count = values1.Length / 3;
        double sum = 0;
        for (int i = count; i < count * 2; i++)
            sum += values2[i] - values1[i];
        double mean = sum / count;
        double rampDeltaI = mean / 2;

        // calculate capacitance as the current difference relative to the rate of voltage change
        double cmRamp = rampDeltaI / dVslope * 1000;

        return cmRamp;
    }

    private static int GetHyperpolarizingStepIndex(ABF abf)
    {
        for (int i = 1; i < abf.Epochs.Length; i++)
        {
            bool isStep = abf.Epochs[i].EpochType == AbfSharp.EpochType.Step;
            bool isHyperpolarizing = abf.Epochs[i].Level < abf.Epochs[i - 1].Level;
            if (isStep && isHyperpolarizing)
            {
                return i;
            }
        }

        return 0;
    }

    private static int GetHyperpolarizingRampIndex(ABF abf)
    {
        for (int i = 1; i < abf.Epochs.Length; i++)
        {
            bool isRamp = abf.Epochs[i].EpochType == AbfSharp.EpochType.Ramp;
            bool isHyperpolarizing = abf.Epochs[i].Level < abf.Epochs[i - 1].Level;
            if (isRamp && isHyperpolarizing)
            {
                return i;
            }
        }

        return 0;
    }
}
