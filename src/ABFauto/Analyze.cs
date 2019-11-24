using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABFauto
{
    public static class Analyze
    {
        public static void Memtest(ABFsharp.ABF abf)
        {
            var plt = new ScottPlot.Plot();
            plt.Ticks(useMultiplierNotation: false);

            for (int i = 0; i < abf.info.sweepCount; i++)
            {
                var sweep = abf.GetSweep(i);
                plt.PlotSignal(sweep.values, abf.info.sampleRate);
            }

            plt.AxisAuto(0, .1);

            string analysisFilePath = System.IO.Path.GetFullPath(abf.info.id + ".png");
            plt.SaveFig(analysisFilePath);
            Debug.WriteLine($"Saved analysis file: {analysisFilePath}");
        }

        public static void IvStep(ABFsharp.ABF abf)
        {
            var plt = new ScottPlot.Plot();
            plt.Ticks(useMultiplierNotation: false);

            for (int i = 0; i < abf.info.sweepCount; i++)
            {
                var sweep = abf.GetSweep(i);
                plt.PlotSignal(sweep.values, abf.info.sampleRate);
            }

            plt.AxisAuto(0, .1);

            string analysisFilePath = System.IO.Path.GetFullPath(abf.info.id + ".png");
            plt.SaveFig(analysisFilePath);
            Debug.WriteLine($"Saved analysis file: {analysisFilePath}");
        }

        public static void IvRamp(ABFsharp.ABF abf)
        {
            var plt = new ScottPlot.Plot(800, 400);
            plt.Ticks(useMultiplierNotation: false);

            for (int i = 0; i < abf.info.sweepCount; i++)
            {
                var sweep = abf.GetSweep(i);
                plt.PlotSignal(sweep.values, abf.info.sampleRate);
            }

            plt.AxisAuto(0, .1);

            string analysisFilePath = System.IO.Path.GetFullPath(abf.info.id + ".png");
            plt.SaveFig(analysisFilePath);
            Debug.WriteLine($"Saved analysis file: {analysisFilePath}");
        }

        public static void MemtestOverTime(ABFsharp.ABF abf)
        {

        }

        public static void ApFirst(ABFsharp.ABF abf)
        {

        }

        public static void ApGain(ABFsharp.ABF abf)
        {
            var plt = new ScottPlot.Plot();
            plt.Ticks(useMultiplierNotation: false);

            for (int i = 0; i < abf.info.sweepCount; i++)
            {
                var sweep = abf.GetSweep(i);
                plt.PlotSignal(sweep.values, abf.info.sampleRate);
            }

            plt.AxisAuto(0, .1);

            string analysisFilePath = System.IO.Path.GetFullPath(abf.info.id + ".png");
            plt.SaveFig(analysisFilePath);
            Debug.WriteLine($"Saved analysis file: {analysisFilePath}");
        }
    }
}
