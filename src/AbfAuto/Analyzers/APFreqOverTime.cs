﻿using AbfAuto.EventDetection;
using AbfSharp;
using ScottPlot;

namespace AbfAuto.Analyzers;

/// <summary>
/// AP Frequency per sweep over time
/// </summary>
internal class APFreqOverTime : IAnalyzer
{
    public AnalysisResult Analyze(ABF abf)
    {
        double[] freqPerSweep = APDetection.FreqPerSweep(abf);
        double[] sweepTimes = abf.SweepStartTimes.Select(x => x / 60).ToArray();

        Plot plotFull = CommonPlots.AllSweeps.Consecutive(abf)
            .WithSignalLineWidth(1.5)
            .WithTightHorizontalMargins()
            .WithYLabelVoltage()
            .WithVerticalLinesAtTagMinutes(abf);

        Plot plotRate = new();
        plotRate.Add.Scatter(sweepTimes, freqPerSweep);
        plotRate.WithTightHorizontalMargins()
            .WithYLabel("AP Frequency (Hz)")
            .WithXLabelMinutes()
            .WithVerticalLinesAtTagMinutes(abf);

        MultiPlot2 mp2 = new();
        mp2.AddSubplot(plotFull, 0, 2, 0, 1);
        mp2.AddSubplot(plotRate, 1, 2, 0, 1);

        return AnalysisResult.Single(mp2);
    }
}
