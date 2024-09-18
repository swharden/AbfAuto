namespace AbfAuto.Memtest;

public class ExponentialFitter
{
    public double Offset { get; }
    public double Scale { get; }
    public double RateConstant { get; }
    public double Tau => Math.Abs(1.0 / RateConstant);

    public ExponentialFitter(double[] values, double steadyState)
    {
        Offset = steadyState;
        double[] xs = Enumerable.Range(0, values.Length).Select(x => (double)x).ToArray();
        double[] ys = values.Select(x => x - Offset).ToArray();
        double[] logYs = ys.Select(x => Math.Log(x)).ToArray();
        (RateConstant, double intercept) = LeastSquaresFit(xs, logYs);
        Scale = Math.Exp(intercept);
    }

    public double GetY(double x)
    {
        return Offset + Scale * Math.Exp(x * RateConstant);
    }

    public double[] GetYs(double[] xs)
    {
        return xs.Select(GetY).ToArray();
    }

    private static (double slope, double intercept) LeastSquaresFit(double[] xs, double[] ys)
    {
        double sumX = 0, sumY = 0, sumX2 = 0, sumXY = 0;

        for (int i = 0; i < xs.Length; i++)
        {
            sumX += xs[i];
            sumY += ys[i];
            sumX2 += xs[i] * xs[i];
            sumXY += xs[i] * ys[i];
        }

        double avgX = sumX / xs.Length;
        double avgY = sumY / xs.Length;
        double avgX2 = sumX2 / xs.Length;
        double avgXY = sumXY / xs.Length;
        double slope = (avgXY - avgX * avgY) / (avgX2 - avgX * avgX);
        double intercept = (avgX2 * avgY - avgXY * avgX) / (avgX2 - avgX * avgX);
        return (slope, intercept);
    }
}
