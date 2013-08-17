namespace Controller
{
	partial class ShowOutputControl
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
			this.components = new System.ComponentModel.Container();
			this.LogsTextbox = new System.Windows.Forms.TextBox();
			this.OutputTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// LogsTextbox
			// 
			this.LogsTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LogsTextbox.BackColor = System.Drawing.SystemColors.Window;
			this.LogsTextbox.Location = new System.Drawing.Point(3, 3);
			this.LogsTextbox.Multiline = true;
			this.LogsTextbox.Name = "LogsTextbox";
			this.LogsTextbox.ReadOnly = true;
			this.LogsTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.LogsTextbox.Size = new System.Drawing.Size(294, 451);
			this.LogsTextbox.TabIndex = 0;
			// 
			// OutputTimer
			// 
			this.OutputTimer.Interval = 1000;
			this.OutputTimer.Tick += new System.EventHandler(this.OutputTimer_Tick);
			// 
			// ShowOutputControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.LogsTextbox);
			this.Name = "ShowOutputControl";
			this.Size = new System.Drawing.Size(300, 457);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox LogsTextbox;
		private System.Windows.Forms.Timer OutputTimer;

	}
}
