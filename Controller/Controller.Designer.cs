namespace Controller
{
    partial class Controller
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.WuListBox = new System.Windows.Forms.ListBox();
			this.OverallProgressBar = new System.Windows.Forms.ProgressBar();
			this.OverallProgressLabel = new System.Windows.Forms.Label();
			this.StartTimer = new System.Windows.Forms.Timer(this.components);
			this.LockDeleter = new System.Windows.Forms.Timer(this.components);
			this.UDNumThreads = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.MemoryTimer = new System.Windows.Forms.Timer(this.components);
			this.AutoNumThreads = new System.Windows.Forms.CheckBox();
			this.MemInc = new System.Windows.Forms.NumericUpDown();
			this.MemDec = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.MaxThreads = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.NodecountProgressbar = new System.Windows.Forms.ProgressBar();
			this.NodecountProgressLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.UDNumThreads)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MemInc)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MemDec)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxThreads)).BeginInit();
			this.SuspendLayout();
			// 
			// WuListBox
			// 
			this.WuListBox.FormattingEnabled = true;
			this.WuListBox.Location = new System.Drawing.Point(1039, 57);
			this.WuListBox.Margin = new System.Windows.Forms.Padding(2);
			this.WuListBox.Name = "WuListBox";
			this.WuListBox.Size = new System.Drawing.Size(51, 17);
			this.WuListBox.TabIndex = 0;
			this.WuListBox.Visible = false;
			// 
			// OverallProgressBar
			// 
			this.OverallProgressBar.Location = new System.Drawing.Point(9, 11);
			this.OverallProgressBar.Margin = new System.Windows.Forms.Padding(2);
			this.OverallProgressBar.Name = "OverallProgressBar";
			this.OverallProgressBar.Size = new System.Drawing.Size(539, 19);
			this.OverallProgressBar.TabIndex = 1;
			// 
			// OverallProgressLabel
			// 
			this.OverallProgressLabel.AutoSize = true;
			this.OverallProgressLabel.Location = new System.Drawing.Point(552, 14);
			this.OverallProgressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.OverallProgressLabel.Name = "OverallProgressLabel";
			this.OverallProgressLabel.Size = new System.Drawing.Size(35, 13);
			this.OverallProgressLabel.TabIndex = 2;
			this.OverallProgressLabel.Text = "label1";
			// 
			// StartTimer
			// 
			this.StartTimer.Enabled = true;
			this.StartTimer.Interval = 5000;
			this.StartTimer.Tick += new System.EventHandler(this.StartTimer_Tick);
			// 
			// LockDeleter
			// 
			this.LockDeleter.Enabled = true;
			this.LockDeleter.Interval = 10000;
			this.LockDeleter.Tick += new System.EventHandler(this.LockDeleter_Tick);
			// 
			// UDNumThreads
			// 
			this.UDNumThreads.Location = new System.Drawing.Point(109, 59);
			this.UDNumThreads.Name = "UDNumThreads";
			this.UDNumThreads.Size = new System.Drawing.Size(52, 20);
			this.UDNumThreads.TabIndex = 3;
			this.UDNumThreads.ValueChanged += new System.EventHandler(this.UDNumThreads_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Number of threads:";
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel.AutoScroll = true;
			this.flowLayoutPanel.Location = new System.Drawing.Point(9, 85);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(1562, 483);
			this.flowLayoutPanel.TabIndex = 5;
			this.flowLayoutPanel.WrapContents = false;
			// 
			// MemoryTimer
			// 
			this.MemoryTimer.Interval = 60000;
			this.MemoryTimer.Tick += new System.EventHandler(this.MemoryTimer_Tick);
			// 
			// AutoNumThreads
			// 
			this.AutoNumThreads.AutoSize = true;
			this.AutoNumThreads.Location = new System.Drawing.Point(178, 61);
			this.AutoNumThreads.Name = "AutoNumThreads";
			this.AutoNumThreads.Size = new System.Drawing.Size(73, 17);
			this.AutoNumThreads.TabIndex = 6;
			this.AutoNumThreads.Text = "Automatic";
			this.AutoNumThreads.UseVisualStyleBackColor = true;
			this.AutoNumThreads.CheckedChanged += new System.EventHandler(this.AutoNumThreads_CheckedChanged);
			// 
			// MemInc
			// 
			this.MemInc.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.MemInc.Location = new System.Drawing.Point(288, 59);
			this.MemInc.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.MemInc.Name = "MemInc";
			this.MemInc.Size = new System.Drawing.Size(70, 20);
			this.MemInc.TabIndex = 7;
			this.MemInc.Value = new decimal(new int[] {
            6000,
            0,
            0,
            0});
			// 
			// MemDec
			// 
			this.MemDec.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.MemDec.Location = new System.Drawing.Point(398, 59);
			this.MemDec.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.MemDec.Name = "MemDec";
			this.MemDec.Size = new System.Drawing.Size(70, 20);
			this.MemDec.TabIndex = 8;
			this.MemDec.Value = new decimal(new int[] {
            4000,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(257, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Inc:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(364, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(28, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "dec:";
			// 
			// MaxThreads
			// 
			this.MaxThreads.Location = new System.Drawing.Point(509, 59);
			this.MaxThreads.Name = "MaxThreads";
			this.MaxThreads.Size = new System.Drawing.Size(41, 20);
			this.MaxThreads.TabIndex = 11;
			this.MaxThreads.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(474, 62);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(29, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "max:";
			// 
			// NodecountProgressbar
			// 
			this.NodecountProgressbar.Location = new System.Drawing.Point(9, 35);
			this.NodecountProgressbar.Name = "NodecountProgressbar";
			this.NodecountProgressbar.Size = new System.Drawing.Size(539, 19);
			this.NodecountProgressbar.TabIndex = 13;
			// 
			// NodecountProgressLabel
			// 
			this.NodecountProgressLabel.AutoSize = true;
			this.NodecountProgressLabel.Location = new System.Drawing.Point(552, 38);
			this.NodecountProgressLabel.Name = "NodecountProgressLabel";
			this.NodecountProgressLabel.Size = new System.Drawing.Size(35, 13);
			this.NodecountProgressLabel.TabIndex = 14;
			this.NodecountProgressLabel.Text = "label5";
			// 
			// Controller
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1583, 580);
			this.Controls.Add(this.NodecountProgressLabel);
			this.Controls.Add(this.NodecountProgressbar);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.MaxThreads);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.MemDec);
			this.Controls.Add(this.MemInc);
			this.Controls.Add(this.AutoNumThreads);
			this.Controls.Add(this.flowLayoutPanel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.UDNumThreads);
			this.Controls.Add(this.OverallProgressLabel);
			this.Controls.Add(this.OverallProgressBar);
			this.Controls.Add(this.WuListBox);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Controller";
			this.Text = "Controller";
			this.Load += new System.EventHandler(this.Main_Load);
			this.Shown += new System.EventHandler(this.Main_Shown);
			this.Click += new System.EventHandler(this.Main_Click);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Controller_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Controller_DragEnter);
			((System.ComponentModel.ISupportInitialize)(this.UDNumThreads)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MemInc)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MemDec)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxThreads)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox WuListBox;
        private System.Windows.Forms.ProgressBar OverallProgressBar;
        private System.Windows.Forms.Label OverallProgressLabel;
        private System.Windows.Forms.Timer StartTimer;
        private System.Windows.Forms.Timer LockDeleter;
		private System.Windows.Forms.NumericUpDown UDNumThreads;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private System.Windows.Forms.Timer MemoryTimer;
		private System.Windows.Forms.CheckBox AutoNumThreads;
		private System.Windows.Forms.NumericUpDown MemInc;
		private System.Windows.Forms.NumericUpDown MemDec;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown MaxThreads;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ProgressBar NodecountProgressbar;
		private System.Windows.Forms.Label NodecountProgressLabel;
    }
}

