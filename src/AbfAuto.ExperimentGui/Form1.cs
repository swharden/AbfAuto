using ScottPlot;
using System.Diagnostics;

namespace AbfAuto.ExperimentGui;

public partial class Form1 : Form
{
    AbfSharp.ABF? Abf = null;

    public Form1()
    {
        InitializeComponent();

        btnLoadAbf.Click += (s, e) => LoadAbf(textBox1.Text);

        btnAnalyze.Click += (s, e) => AnalyzeEvents();

        nudSweep.ValueChanged += (s, e) => LoadSweep((int)nudSweep.Value - 1);

        tbThresholdHeight.ValueChanged += (s, e) =>
        {
            label3.Text = $"Threshold: {GetThresholdValue():N2}  ";
            Application.DoEvents();
            LoadSweep((int)nudSweep.Value - 1);
        };

        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (Abf is null)
                return;
            if (e.KeyCode == Keys.OemPeriod)
                nudSweep.Value = Math.Min(nudSweep.Value + 1, Abf.SweepCount);
            if (e.KeyCode == Keys.Oemcomma)
                nudSweep.Value = Math.Max(nudSweep.Value - 1, 1);
        };

        AllowDrop = true;
        DragEnter += (o, e) =>
        {
            if (e.Data is null) return;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        };
        DragDrop += (o, e) =>
        {
            if (e.Data is null) return;
            string[]? paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths is null || paths.Length == 0) return;
            LoadAbf(paths[0]);
        };

        textBox1.Text = @"X:/Data/zProjects/SST diabetes/LTS neuron SST/abfs/2024-06-20-DIC1/2024_06_20_0018.abf";
        LoadAbf(textBox1.Text);
    }

    private double GetThresholdValue()
    {
        double fraction = (double)tbThresholdHeight.Value / tbThresholdHeight.Maximum;
        double scaled = Math.Pow(2, 3 * fraction) - 1;
        double mult = 20;
        return scaled * mult;
    }

    private EventDetection.Settings GetSettings()
    {
        return new()
        {
            Threshold = GetThresholdValue(),
            SmoothingMsec = 2,
            TrendlineMsec = 100,
        };
    }

    private void AnalyzeEvents()
    {
        if (Abf is null)
            return;

        List<EventDetection.SweepAnalysisResult> results = [];
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < Abf.SweepCount; i++)
        {
            EventDetection.SweepAnalysisResult sweepResult = EventDetection.DetectEvents(Abf, i, GetSettings());
            results.Add(sweepResult);
        };
        sw.Stop();

        int totalEventCount = results.Select(x => x.Events.Length).Sum();

        EventDetection.PlotAllTraces(Abf, results.ToArray(), 100).SavePng("test.png", 800, 800).LaunchInBrowser();

        Form2 form = new();
        form.SetEvents(results.ToArray());
        form.ShowDialog();
    }

    private void LoadSweep(int sweepIndex)
    {
        AxisLimits originalLimits = formsPlot1.Plot.Axes.GetLimits();

        formsPlot1.Plot.PlottableList.Clear();

        if (Abf is null)
            return;

        // TODO: use settings from event detector
        AbfSharp.Sweep sweep = Abf.GetSweep(sweepIndex).WithSmoothingMilliseconds(2).SliceTimeEnd(1);
        AbfSharp.Sweep trendline = Abf.GetSweep(sweepIndex).WithSmoothingMilliseconds(100).SliceTimeEnd(1);

        // plot traces
        var sigSweep = formsPlot1.Plot.Add.Signal(sweep.Values, sweep.SamplePeriod);
        sigSweep.Color = Colors.C0;

        // detect events
        var results = EventDetection.DetectEvents(Abf, sweepIndex, GetSettings());

        // plot peaks
        double[] peakXs = results.Events.Select(x => x.Time).ToArray();
        double[] peakYs = results.Events.Select(x => x.Value).ToArray();
        var peakMarker = formsPlot1.Plot.Add.Markers(peakXs, peakYs);
        peakMarker.MarkerShape = MarkerShape.Asterisk;
        peakMarker.Color = Colors.Red;

        formsPlot1.Plot.Axes.Margins(horizontal: 0);

        if (cbAutoScale.Checked)
            formsPlot1.Plot.Axes.AutoScale();
        else
            formsPlot1.Plot.Axes.SetLimits(originalLimits);

        formsPlot1.Refresh();
    }

    private void LoadAbf(string path)
    {
        Abf = null;
        nudSweep.Value = 1;

        Abf = new AbfSharp.ABF(path);
        nudSweep.Minimum = 1;
        nudSweep.Maximum = Abf.SweepCount;

        LoadSweep(0);
    }
}