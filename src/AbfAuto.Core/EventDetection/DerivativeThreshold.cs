using static AbfAuto.Core.EventDetection.DerivativeThreshold;

namespace AbfAuto.Core.EventDetection;

public static class DerivativeThreshold
{
    public class Settings()
    {
        /// <summary>
        /// Detect events that change <see cref="DeltaAmplitude"/> over <see cref="DeltaTime"/>.
        /// </summary>
        public double DeltaAmplitude { get; init; } = 20;

        /// <summary>
        /// Detect events that change <see cref="DeltaAmplitude"/> over <see cref="DeltaTime"/>.
        /// </summary>
        public TimeSpan DeltaTime { get; init; } = TimeSpan.FromMilliseconds(1);

        public static Settings AP => new()
        {
            DeltaAmplitude = 20,
            DeltaTime = TimeSpan.FromMilliseconds(1),
        };
    }

    public static int[] GetIndexes(Sweep sweep, Settings settings)
    {
        List<int> indexes = [];

        int dtPoints = (int)Math.Ceiling(sweep.SampleRate * settings.DeltaTime.TotalSeconds);

        var ys = sweep.Values;

        int i = dtPoints;
        while (i < ys.Count)
        {
            double dv = ys[i] - ys[i - dtPoints];

            if (dv >= settings.DeltaAmplitude)
            {
                // register this event
                indexes.Add(i);

                // move forward until we reach the peak
                for (; i < ys.Count && ys[i] >= ys[i - 1]; i++) { }

                // move forward until we reach the nadir
                for (; i < ys.Count && ys[i] <= ys[i - 1]; i++) { }
            }

            i++;
        }

        return [.. indexes];
    }
}
