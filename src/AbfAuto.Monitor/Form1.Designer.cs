namespace AbfAuto.Monitor;

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
        tbPath = new TextBox();
        label1 = new Label();
        btnAnalyze = new Button();
        btnReset = new Button();
        listBox1 = new ListBox();
        richTextBox1 = new RichTextBox();
        SuspendLayout();
        // 
        // tbFolder
        // 
        tbPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tbPath.Location = new Point(12, 12);
        tbPath.Name = "tbFolder";
        tbPath.Size = new Size(894, 31);
        tbPath.TabIndex = 6;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(192, 54);
        label1.Name = "label1";
        label1.Size = new Size(84, 25);
        label1.TabIndex = 9;
        label1.Text = "Waiting...";
        // 
        // btnScan
        // 
        btnAnalyze.Location = new Point(12, 49);
        btnAnalyze.Name = "btnScan";
        btnAnalyze.Size = new Size(95, 35);
        btnAnalyze.TabIndex = 7;
        btnAnalyze.Text = "Analyze";
        btnAnalyze.UseVisualStyleBackColor = true;
        // 
        // btnReset
        // 
        btnReset.Location = new Point(113, 49);
        btnReset.Name = "btnReset";
        btnReset.Size = new Size(73, 35);
        btnReset.TabIndex = 8;
        btnReset.Text = "Reset";
        btnReset.UseVisualStyleBackColor = true;
        // 
        // listBox1
        // 
        listBox1.BackColor = SystemColors.Control;
        listBox1.FormattingEnabled = true;
        listBox1.ItemHeight = 25;
        listBox1.Location = new Point(12, 90);
        listBox1.Name = "listBox1";
        listBox1.Size = new Size(894, 179);
        listBox1.TabIndex = 10;
        // 
        // richTextBox1
        // 
        richTextBox1.BackColor = SystemColors.ControlDark;
        richTextBox1.BorderStyle = BorderStyle.FixedSingle;
        richTextBox1.Location = new Point(12, 280);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.Size = new Size(894, 280);
        richTextBox1.TabIndex = 11;
        richTextBox1.Text = "";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(918, 572);
        Controls.Add(richTextBox1);
        Controls.Add(listBox1);
        Controls.Add(tbPath);
        Controls.Add(label1);
        Controls.Add(btnAnalyze);
        Controls.Add(btnReset);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox tbPath;
    private Label label1;
    private Button btnAnalyze;
    private Button btnReset;
    private ListBox listBox1;
    private RichTextBox richTextBox1;
}
