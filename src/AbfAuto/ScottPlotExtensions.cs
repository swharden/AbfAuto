﻿using AbfSharp;
using ScottPlot;

namespace AbfAuto;

public static class ScottPlotExtensions
{
    public static ScottPlot.Plottables.Signal AddSignalMS(this ScottPlot.Plot plot, Sweep sweep)
    {
        var sig = plot.Add.Signal(sweep.Values, sweep.SamplePeriod * 1000);
        plot.XLabel("Time (msec)");
        plot.Axes.Margins(horizontal: 0);
        return sig;
    }

    public static Plot WithSignalColor(this Plot plot, ScottPlot.Color color)
    {
        foreach (var sig in plot.GetPlottables<ScottPlot.Plottables.Signal>())
        {
            sig.Color = color;
        }
        return plot;
    }

    public static Plot WithSignalColor(this Plot plot, IColormap colormap)
    {
        var sigs = plot.GetPlottables<ScottPlot.Plottables.Signal>().ToArray();
        for (int i = 0; i < sigs.Length; i++)
        {
            sigs[i].Color = colormap.GetColor(i, sigs.Length);
        }
        return plot;
    }

    public static Plot WithSignalRainbow(this Plot plot)
    {
        Color[] colors = "#466be3 #29bbec #30f199 #edd03a #fb8023 #d23104"
            .Split(" ")
            .Select(x => new Color(x))
            .ToArray();

        IColormap colormap = new LinearSegmented(colors);
        return WithSignalColor(plot, colormap);
    }

    public static Plot WithSignalAlpha(this Plot plot, double alpha)
    {
        foreach (var sig in plot.GetPlottables<ScottPlot.Plottables.Signal>())
        {
            sig.Color = sig.Color.WithAlpha(alpha);
        }
        return plot;
    }

    public static Plot WithScatterLineWidth(this Plot plot, double lineWidth = 1.5)
    {
        foreach (var scatter in plot.GetPlottables<ScottPlot.Plottables.Scatter>())
        {
            scatter.LineWidth = (float)lineWidth;
        }
        return plot;
    }

    public static Plot WithSignalLineWidth(this Plot plot, double lineWidth)
    {
        foreach (var sig in plot.GetPlottables<ScottPlot.Plottables.Signal>())
        {
            sig.LineWidth = (float)lineWidth;
        }
        return plot;
    }

    public static Plot WithSignalHighQualityRendering(this Plot plot)
    {
        foreach (var sig in plot.GetPlottables<ScottPlot.Plottables.Signal>())
        {
            sig.AlwaysUseLowDensityMode = true;
        }
        return plot;
    }

    public static Plot OutlineEpoch(this Plot plot, Epoch epoch)
    {
        plot.Add.VerticalLine(epoch.StartTime, 2, Colors.Black.WithAlpha(.5), LinePattern.DenselyDashed);
        plot.Add.VerticalLine(epoch.EndTime, 2, Colors.Black.WithAlpha(.5), LinePattern.DenselyDashed);
        return plot;
    }

    public static Plot ZoomToEpoch(this Plot plot, Epoch epoch, double padTime = 0.1)
    {
        plot.Axes.SetLimitsX(epoch.StartTime - padTime, epoch.EndTime + padTime);
        return plot;
    }

    public static Plot WithTightHorizontalMargins(this Plot plot)
    {
        plot.Axes.Margins(horizontal: 0);
        return plot;
    }

    public static Plot WithNoLeftTicks(this Plot plot)
    {
        plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.EmptyTickGenerator();
        return plot;
    }

    public static Plot WithBottomTicksOnly(this Plot plot)
    {
        plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.EmptyTickGenerator();
        plot.Axes.Left.FrameLineStyle.IsVisible = false;
        plot.Axes.Right.FrameLineStyle.IsVisible = false;
        plot.Axes.Top.FrameLineStyle.IsVisible = false;
        return plot;
    }

    public static Plot WithYLabel(this Plot plot, string label)
    {
        plot.YLabel(label);
        return plot;
    }

    public static Plot WithYLabelVoltage(this Plot plot)
    {
        return WithYLabel(plot, "Potential (mV)");
    }

    public static Plot WithYLabelCurrent(this Plot plot)
    {
        return WithYLabel(plot, "Current (pA)");
    }

    public static Plot WithXLabel(this Plot plot, string label)
    {
        plot.XLabel(label);
        return plot;
    }

    public static Plot WithXLabelSeconds(this Plot plot)
    {
        return WithXLabel(plot, "Time (seconds)");
    }

    public static Plot WithXLabelMinutes(this Plot plot)
    {
        return WithXLabel(plot, "Time (minutes)");
    }

    public static Plot WithVerticalLinesAtTagTimes(this Plot plot, AbfSharp.ABF abf)
    {
        foreach (var tag in abf.Tags)
        {
            plot.Add.VerticalLine(tag.Time / 60, 2, Colors.Red.WithAlpha(.5), LinePattern.DenselyDashed);
        }

        return plot;
    }
}