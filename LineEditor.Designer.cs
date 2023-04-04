namespace GCEd
{
	partial class LineEditor
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
			textBoxLine = new System.Windows.Forms.TextBox();
			SuspendLayout();
			// 
			// textBoxLine
			// 
			textBoxLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			textBoxLine.Location = new System.Drawing.Point(3, 3);
			textBoxLine.Name = "textBoxLine";
			textBoxLine.ReadOnly = true;
			textBoxLine.Size = new System.Drawing.Size(232, 23);
			textBoxLine.TabIndex = 0;
			textBoxLine.KeyDown += textBoxLine_KeyDown;
			// 
			// LineEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(textBoxLine);
			Name = "LineEditor";
			Size = new System.Drawing.Size(238, 30);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.TextBox textBoxLine;
	}
}
