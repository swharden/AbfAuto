namespace AbfAuto.DeveloperTools;

internal static class Smoothing
{
    public static void SmoothInPlace(double[] ys, int pointCount = 5)
    {
        double[] smoothed = Smooth(ys, pointCount);
        Array.Copy(smoothed, ys, ys.Length);
    }

    public static double[] Smooth(double[] ys, int pointCount = 5)
    {
        double[] smooth = new double[ys.Length];

        // smooth from left to right
        double runningSum = 0;
        int pointsInSum = 0;
        for (int i = 0; i < smooth.Length; i++)
        {
            runningSum += ys[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= ys[i - pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // smooth from right to left
        runningSum = 0;
        pointsInSum = 0;
        for (int i = smooth.Length - 1; i >= 0; i--)
        {
            runningSum += ys[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= ys[i + pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // average the two directions
        for (int i = 0; i < smooth.Length; i++)
        {
            smooth[i] /= 2;
        }

        return smooth;
    }
}
