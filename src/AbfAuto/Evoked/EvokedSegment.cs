using AbfSharp;
using ScottPlot;

namespace AbfAuto.Evoked;

public class EvokedSegment
{
    public double[] Values { get; }
    public double SamplePeriod { get; }

    public double MeasureTimeStart { get; }
    public double MeasureTimeEnd { get; }

    public double Min { get; }
    public double Max { get; }
    public double Mean { get; }
    public double Area { get; }

    public EvokedSegment(Sweep sweep, Epoch epoch, EvokedSettings settings)
    {
        double baselineEndTime = epoch.StartTime - settings.BaselineBackup;
        double baselineStartTime = baselineEndTime - settings.BaselineDuration;

        double measureStartTime = epoch.StartTime + settings.MeasurePadding;
        double measureEndTime = measureStartTime + settings.MeasureDuration;

        double viewStartTime = epoch.StartTime - settings.ViewPaddingLeft;
        double viewEndTime = viewStartTime + settings.ViewDuration;

        MeasureTimeStart = (measureStartTime - viewStartTime) * 1000;
        MeasureTimeEnd = (measureEndTime - viewStartTime) * 1000;

        int viewStartIndex = (int)(viewStartTime * sweep.SampleRate);
        int viewEndIndex = (int)(viewEndTime * sweep.SampleRate);

        double sampleRate = 1.0 / epoch.SamplePeriod;
        int silenceStartIndex = epoch.IndexFirst - (int)(settings.StimulusArtifactPadLeft * sampleRate);
        int silenceLastIndex = epoch.IndexLast + (int)(settings.StimulusArtifactPadRight * sampleRate);

        if (settings.BaselineSubtract)
        {
            double baselineMean = sweep.MeanOfTimeRange(baselineStartTime, baselineEndTime);
            sweep.SubtractInPlace(baselineMean);
        }

        if (settings.RemoveStimulusArtifact)
        {
            for (int i = silenceStartIndex; i <= silenceLastIndex; i++)
            {
                sweep.Values[i] = 0;
            }
        }

        if (settings.Smooth)
        {
            sweep = sweep.SmoothHanning(TimeSpan.FromMilliseconds(settings.SmoothMilliseconds));
        }

        sweep = sweep.SubSweepByIndex(viewStartIndex, viewEndIndex);

        Values = sweep.Values;
        SamplePeriod = epoch.SamplePeriod;

        int measurementIndex1 = (int)(MeasureTimeStart / 1000 * sampleRate);
        int measurementIndex2 = (int)(MeasureTimeEnd / 1000 * sampleRate);
        double[] measurementValues = Values[measurementIndex1..measurementIndex2];
        Min = measurementValues.Min();
        Max = measurementValues.Max();
        Area = measurementValues.Sum();
        Mean = Area / measurementValues.Length;
    }
}