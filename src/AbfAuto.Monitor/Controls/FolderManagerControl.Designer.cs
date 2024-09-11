namespace AbfAuto.Monitor;

partial class FolderManagerControl
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tbFolder = new TextBox();
        btnScan = new Button();
        btnReset = new Button();
        groupBox1 = new GroupBox();
        listBox1 = new ListBox();
        label1 = new Label();
        groupBox2 = new GroupBox();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // tbFolder
        // 
        tbFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tbFolder.Location = new Point(6, 30);
        tbFolder.Name = "tbFolder";
        tbFolder.Size = new Size(489, 31);
        tbFolder.TabIndex = 0;
        // 
        // btnScan
        // 
        btnScan.Location = new Point(6, 67);
        btnScan.Name = "btnScan";
        btnScan.Size = new Size(95, 35);
        btnScan.TabIndex = 1;
        btnScan.Text = "Analyze";
        btnScan.UseVisualStyleBackColor = true;
        // 
        // btnReset
        // 
        btnReset.Location = new Point(107, 67);
        btnReset.Name = "btnReset";
        btnReset.Size = new Size(73, 35);
        btnReset.TabIndex = 3;
        btnReset.Text = "Reset";
        btnReset.UseVisualStyleBackColor = true;
        // 
        // groupBox1
        // 
        groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        groupBox1.Controls.Add(listBox1);
        groupBox1.Location = new Point(3, 118);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(507, 183);
        groupBox1.TabIndex = 4;
        groupBox1.TabStop = false;
        groupBox1.Text = "Queue";
        // 
        // listBox1
        // 
        listBox1.Dock = DockStyle.Fill;
        listBox1.FormattingEnabled = true;
        listBox1.ItemHeight = 25;
        listBox1.Location = new Point(3, 27);
        listBox1.Name = "listBox1";
        listBox1.Size = new Size(501, 153);
        listBox1.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(186, 72);
        label1.Name = "label1";
        label1.Size = new Size(84, 25);
        label1.TabIndex = 5;
        label1.Text = "Waiting...";
        // 
        // groupBox2
        // 
        groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBox2.Controls.Add(tbFolder);
        groupBox2.Controls.Add(label1);
        groupBox2.Controls.Add(btnScan);
        groupBox2.Controls.Add(btnReset);
        groupBox2.Location = new Point(6, 3);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new Size(501, 109);
        groupBox2.TabIndex = 6;
        groupBox2.TabStop = false;
        groupBox2.Text = "Path";
        // 
        // FolderManagerControl
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(groupBox2);
        Controls.Add(groupBox1);
        Name = "FolderManagerControl";
        Size = new Size(513, 304);
        groupBox1.ResumeLayout(false);
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TextBox tbFolder;
    private Button btnScan;
    private Button btnReset;
    private GroupBox groupBox1;
    private ListBox listBox1;
    private Label label1;
    private GroupBox groupBox2;
}
