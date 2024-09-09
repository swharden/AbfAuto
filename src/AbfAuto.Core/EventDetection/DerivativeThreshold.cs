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
    }

    public static int[] GetIndexes(double[] ys, double period, Settings settings)
    {
        List<int> indexes = [];

        int dtPoints = (int)(settings.DeltaTime.TotalSeconds * period);

        int i = dtPoints;
        while (i < ys.Length)
        {
            double dv = ys[i] - ys[i - dtPoints];

            if (dv >= settings.DeltaAmplitude)
            {
                // register this event
                indexes.Add(i);

                // move forward until we reach the peak
                for (; i < ys.Length && ys[i] >= ys[i - 1]; i++) { }

                // move forward until we reach the antipeak
                for (; i < ys.Length && ys[i] <= ys[i - 1]; i++) { }
            }

            i++;
        }

        return indexes.ToArray();
    }
}
