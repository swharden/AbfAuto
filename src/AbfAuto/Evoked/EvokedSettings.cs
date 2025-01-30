namespace AbfAuto.Evoked;
public class EvokedSettings
{
    public bool Smooth = true;
    public double SmoothMilliseconds = 2;

    public bool RemoveStimulusArtifact = true;
    public double StimulusArtifactPadLeft = 0.001;
    public double StimulusArtifactPadRight = 0.003;

    public bool BaselineSubtract = true;
    /// <summary>
    /// distance (seconds) from end of baseline to start of stimulus
    /// </summary>
    public double BaselineBackup = 0.05;

    public double BaselineDuration = 0.5;

    /// <summary>
    /// Distance (seconds) from start of stimulus to when measurement begins
    /// </summary>
    public double MeasurePadding = 0.004;

    public double MeasureDuration = 0.020;

    public double ViewPaddingLeft = 0.05;

    public double ViewDuration = 0.15;

    public static EvokedSettings EvokedEpsc => new();
}
