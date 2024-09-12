namespace AbfAuto.Gui;

public partial class Form1 : Form
{
    readonly System.Windows.Forms.Timer AnalyzeTimer = new()
    {
        Interval = 100,
        Enabled = true
    };

    public Form1()
    {
        InitializeComponent();

        label1.Text = string.Empty;
        tbPath.Text = @"X:\Data\Alchem\Donepezil\BLA\07-28-2021";

        btnAnalyze.Click += (s, e) => AnalyzePath(tbPath.Text);
        btnReset.Click += (s, e) => ResetFolder(tbPath.Text);
        AnalyzeTimer.Tick += (s, e) => AnalyzeNext();
    }

    public void SetStatus(string message)
    {
        label1.Text = message;
        if (richTextBox1.Text.Length > 0)
        {
            richTextBox1.AppendText("\r\n");
        }
        richTextBox1.AppendText(message);
        richTextBox1.ScrollToCaret();
    }

    private void AnalyzePath(string path)
    {
        listBox1.Items.Clear();

        path = Path.GetFullPath(path);

        if (File.Exists(path))
        {
            if (!path.EndsWith(".abf", StringComparison.InvariantCultureIgnoreCase))
            {
                SetStatus("ERROR: Path is not an ABF file");
                return;
            }
            listBox1.Items.Add(path);
            SetStatus($"Added 1 ABF file");
        }
        else if (Directory.Exists(path))
        {
            (string[] needAnalysis, string[] doNotNeedAnalysis) = AbfFolderScan.Scan(path);
            int totalAbfCount = needAnalysis.Length + doNotNeedAnalysis.Length;
            listBox1.Items.AddRange(needAnalysis);
            SetStatus($"Found {totalAbfCount} ABFs ({needAnalysis.Length} require analysis)");
        }
        else
        {
            SetStatus("ERROR: Path does not exist");
        }
    }

    private void ResetFolder(string folder)
    {
        folder = AbfFolderScan.GetAutoAnalysisFolder(folder);

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
