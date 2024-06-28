using System.Data;

namespace AbfAuto.ExperimentGui;

public partial class Form2 : Form
{
    public Form2()
    {
        InitializeComponent();
    }

    public void SetEvents(EventDetection.SweepAnalysisResult[] results)
    {
        DataTable table = new();
        table.Columns.Add("Sweep", typeof(int));
        table.Columns.Add("Time", typeof(double));
        table.Columns.Add("Frequency", typeof(double));
        table.Columns.Add("Amplitude", typeof(double));

        for (int i = 0; i < results.Length; i++)
        {
            DataRow row = table.NewRow();
            row.SetField(0, i + 1);
            row.SetField(1, Math.Round(i * results[i].SweepIntervalSec / 60, 3));
            row.SetField(2, Math.Round(results[i].MeanFrequency, 3));
            row.SetField(3, Math.Round(results[i].MeanAmplitude, 3));

            table.Rows.Add(row);
        }

        formsPlot1.Reset(EventDetection.PlotFreqOverTime(results));

        dataGridView1.DataSource = table;
        dataGridView1.RowHeadersVisible = false;
        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dataGridView1.AutoResizeColumns();
    }
}
