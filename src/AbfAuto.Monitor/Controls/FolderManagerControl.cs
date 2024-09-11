namespace AbfAuto.Monitor;

public partial class FolderManagerControl : UserControl
{
    public EventHandler<string>? StatusChanged;

    public FolderManagerControl()
    {
        InitializeComponent();

        tbFolder.Text = @"X:\Data\Alchem\Donepezil\BLA\07-28-2021";

        btnScan.Click += (s, e) =>
        {
            listBox1.Items.Clear();

            if (!Directory.Exists(tbFolder.Text))
                return;

            (string[] needAnalysis, string[] doNotNeedAnalysis) = AbfFolderScan.Scan(tbFolder.Text);

            int totalAbfCount = needAnalysis.Length + doNotNeedAnalysis.Length;
            listBox1.Items.AddRange(needAnalysis);
            SetStatus($"Found {totalAbfCount} ABFs ({needAnalysis.Length} require analysis)");
        };

        btnReset.Click += (s, e) =>
        {
            string folder = AbfFolderScan.GetAutoAnalysisFolder(tbFolder.Text);

            if (MessageBox.Show(
                text: $"Do you want to delete analysis folder?\n{folder}",
                caption: "WARNING",
                buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
                SetStatus("Folder has been reset");
            }
        };
    }

    public void SetStatus(string message)
    {
        label1.Text = message;
        StatusChanged?.Invoke(this, message);
    }

    public void AnalyzeNext()
    {
        if (listBox1.Items.Count == 0)
            return;

        string? path = listBox1.Items[0].ToString();
        if (path is null)
            return;

        SetStatus($"Analyzing {Path.GetFileName(path)}");
        AbfAuto.Core.Analyze.AnalyzeAbfFile(path);

        listBox1.Items.Remove(path);

        if (listBox1.Items.Count == 0)
            SetStatus($"Analysis Complete");
    }
}
