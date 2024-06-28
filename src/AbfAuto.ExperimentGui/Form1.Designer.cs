namespace AbfAuto.ExperimentGui;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        formsPlot1 = new ScottPlot.WinForms.FormsPlot();
        textBox1 = new TextBox();
        btnLoadAbf = new Button();
        nudSweep = new NumericUpDown();
        cbAutoScale = new CheckBox();
        tbThresholdHeight = new TrackBar();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        btnAnalyze = new Button();
        ((System.ComponentModel.ISupportInitialize)nudSweep).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tbThresholdHeight).BeginInit();
        SuspendLayout();
        // 
        // formsPlot1
        // 
        formsPlot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        formsPlot1.DisplayScale = 1.5F;
        formsPlot1.Location = new Point(12, 75);
        formsPlot1.Name = "formsPlot1";
        formsPlot1.Size = new Size(1640, 774);
        formsPlot1.TabIndex = 1;
        // 
        // textBox1
        // 
        textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBox1.Location = new Point(12, 37);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(806, 31);
        textBox1.TabIndex = 2;
        // 
        // btnLoadAbf
        // 
        btnLoadAbf.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnLoadAbf.Location = new Point(824, 35);
        btnLoadAbf.Name = "btnLoadAbf";
        btnLoadAbf.Size = new Size(112, 34);
        btnLoadAbf.TabIndex = 3;
        btnLoadAbf.Text = "Load";
        btnLoadAbf.UseVisualStyleBackColor = true;
        // 
        // nudSweep
        // 
        nudSweep.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        nudSweep.Location = new Point(1060, 36);
        nudSweep.Name = "nudSweep";
        nudSweep.Size = new Size(102, 31);
        nudSweep.TabIndex = 4;
        // 
        // cbAutoScale
        // 
        cbAutoScale.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cbAutoScale.AutoSize = true;
        cbAutoScale.Checked = true;
        cbAutoScale.CheckState = CheckState.Checked;
        cbAutoScale.Location = new Point(1168, 37);
        cbAutoScale.Name = "cbAutoScale";
        cbAutoScale.Size = new Size(117, 29);
        cbAutoScale.TabIndex = 5;
        cbAutoScale.Text = "AutoScale";
        cbAutoScale.UseVisualStyleBackColor = true;
        // 
        // tbThresholdHeight
        // 
        tbThresholdHeight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        tbThresholdHeight.Location = new Point(1291, 37);
        tbThresholdHeight.Maximum = 100;
        tbThresholdHeight.Minimum = 1;
        tbThresholdHeight.Name = "tbThresholdHeight";
        tbThresholdHeight.Size = new Size(361, 69);
        tbThresholdHeight.TabIndex = 6;
        tbThresholdHeight.Value = 10;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 9);
        label1.Name = "label1";
        label1.Size = new Size(74, 25);
        label1.TabIndex = 7;
        label1.Text = "ABF File";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(1060, 9);
        label2.Name = "label2";
        label2.Size = new Size(64, 25);
        label2.TabIndex = 8;
        label2.Text = "Sweep";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(1291, 9);
        label3.Name = "label3";
        label3.Size = new Size(90, 25);
        label3.TabIndex = 9;
        label3.Text = "Threshold";
        // 
        // btnAnalyze
        // 
        btnAnalyze.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAnalyze.Location = new Point(942, 35);
        btnAnalyze.Name = "btnAnalyze";
        btnAnalyze.Size = new Size(112, 34);
        btnAnalyze.TabIndex = 10;
        btnAnalyze.Text = "Analyze";
        btnAnalyze.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1664, 861);
        Controls.Add(btnAnalyze);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(cbAutoScale);
        Controls.Add(nudSweep);
        Controls.Add(btnLoadAbf);
        Controls.Add(textBox1);
        Controls.Add(formsPlot1);
        Controls.Add(tbThresholdHeight);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)nudSweep).EndInit();
        ((System.ComponentModel.ISupportInitialize)tbThresholdHeight).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ScottPlot.WinForms.FormsPlot formsPlot1;
    private TextBox textBox1;
    private Button btnLoadAbf;
    private NumericUpDown nudSweep;
    private CheckBox cbAutoScale;
    private TrackBar tbThresholdHeight;
    private Label label1;
    private Label label2;
    private Label label3;
    private Button btnAnalyze;
}
