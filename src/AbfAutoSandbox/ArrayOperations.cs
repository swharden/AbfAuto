using AbfSharp;

namespace AbfAutoSandbox;

public static class ArrayOperations
{
    public static double[] Smooth(double[] values, int pointCount)
    {
        double[] smooth = new double[values.Length];

        // smooth from left to right
        double runningSum = 0;
        int pointsInSum = 0;
        for (int i = 0; i < smooth.Length; i++)
        {
            runningSum += values[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= values[i - pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // smooth from right to left
        runningSum = 0;
        pointsInSum = 0;
        for (int i = smooth.Length - 1; i >= 0; i--)
        {
            runningSum += values[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= values[i + pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // average the two directions
        for (int i = 0; i < smooth.Length; i++)
        {
            smooth[i] /= 2;
        }

        return smooth;
    }

    public static double[] Detrend(double[] values, int pointCount)
    {
        double[] trendline = Smooth(values, pointCount);

        double[] values2 = new double[values.Length];
        for (int i = 0; i < values2.Length; i++)
        {
            values2[i] = values[i] - trendline[i];
        }

        return values2;
    }

    public static int[] GetIndexesCrossingAbove(double[] values, double threshold)
    {
        List<int> indexes = [];

        for (int i = 1; i < values.Length; i++)
        {
            if (values[i - 1] < threshold && values[i] >= threshold)
                indexes.Add(i);
        }

        return [.. indexes];
    }

    public static int[] GetIndexesCrossingBelow(double[] values, double threshold)
    {
        List<int> indexes = [];

        for (int i = 1; i < values.Length; i++)
        {
            if (values[i - 1] > threshold && values[i] <= threshold)
                indexes.Add(i);
        }

        return [.. indexes];
    }
}
