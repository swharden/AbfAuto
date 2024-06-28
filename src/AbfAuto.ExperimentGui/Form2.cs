using System.Data;

namespace AbfAuto.ExperimentGui;

public partial class Form2 : Form
{
    public Form2()
    {
        InitializeComponent();
    }

    public void SetEvents(EventDetection.SweepAnalysisResult[] resultsBySweep)
    {
        DataTable table = new();
        table.Columns.Add("Sweep", typeof(int));
        table.Columns.Add("Time", typeof(double));
        table.Columns.Add("Frequency", typeof(double));
        table.Columns.Add("Amplitude", typeof(double));

        for (int i = 0; i < resultsBySweep.Length; i++)
        {
            DataRow row = table.NewRow();
            row.SetField(0, i + 1);
            row.SetField(1, Math.Round(i * resultsBySweep[i].SweepIntervalSec / 60, 3));
            row.SetField(2, Math.Round(resultsBySweep[i].MeanFrequency, 3));
            row.SetField(3, Math.Round(resultsBySweep[i].MeanAmplitude, 3));

            table.Rows.Add(row);
        }

        double[] xs = Enumerable.Range(0, resultsBySweep.Length).Select(x => x * resultsBySweep[x].SweepIntervalSec).ToArray();
        double[] ys = resultsBySweep.Select(x => x.MeanFrequency).ToArray();
        var sp = formsPlot1.Plot.Add.Scatter(xs, ys);
        sp.LineWidth = 2;
        formsPlot1.Plot.YLabel("Frequency (Hz)");
        formsPlot1.Plot.XLabel("Time (min)");

        dataGridView1.DataSource = table;
        dataGridView1.RowHeadersVisible = false;
        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dataGridView1.AutoResizeColumns();
    }
}
