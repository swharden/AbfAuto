namespace AbfAuto.Core;

public class ExponentialFitter
{
    public double A { get; private set; } = 1;
    public double B { get; private set; } = 0;
    public double Tau { get; private set; } = 20;

    public ExponentialFitter(double[] values)
    {
        A = values.First() - values.Last();
        B = values.Last();

        int maxAttempts = 1_000;
        int maxFlips = 50;

        double tauDelta = 1;
        bool previouslyTooHigh = false;
        int flips = 0;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            double err = GetTotalError(values);
            bool currentlyTooHigh = err > 0;
            bool signFlipped = currentlyTooHigh != previouslyTooHigh;
            if (signFlipped)
            {
                tauDelta *= 0.8;
                flips += 1;
            }
            double nextTauDelta = currentlyTooHigh ? tauDelta : -tauDelta;
            Tau += nextTauDelta;
            previouslyTooHigh = currentlyTooHigh;
            if (flips >= maxFlips)
            {
                break;
            }
        }
    }

    public double GetTotalError(double[] values)
    {
        double totalError = 0;
        for (int i = 0; i < values.Length; i++)
        {
            double error = values[i] - GetY(i);
            totalError += error;
        }
        return totalError;
    }

    public double GetY(double x)
    {
        return A * Math.Exp(-x / Tau) + B;
    }

    public double[] GetYs(double[] xs)
    {
        return xs.Select(GetY).ToArray();
    }
}
