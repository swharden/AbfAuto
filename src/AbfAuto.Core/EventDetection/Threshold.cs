namespace AbfAuto.Core.EventDetection;

public class Threshold
{
    public static int[] IndexesCrossingUp(double[] arr, double threshold = 0)
    {
        List<int> indices = [];

        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] >= threshold && arr[i - 1] < threshold)
            {
                indices.Add(i);
            }
        }

        return indices.ToArray();
    }

    public static int[] IndexesCrossingDown(double[] arr, double threshold = 0)
    {
        List<int> indices = [];

        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] <= threshold && arr[i - 1] > threshold)
            {
                indices.Add(i);
            }
        }

        return indices.ToArray();
    }
}
