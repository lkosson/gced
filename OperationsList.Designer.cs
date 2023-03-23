namespace GCEd
{
	partial class OperationsList
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
			this.listBoxOperations = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// listBoxOperations
			// 
			this.listBoxOperations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxOperations.FormattingEnabled = true;
			this.listBoxOperations.ItemHeight = 15;
			this.listBoxOperations.Location = new System.Drawing.Point(3, 3);
			this.listBoxOperations.Name = "listBoxOperations";
			this.listBoxOperations.Size = new System.Drawing.Size(348, 289);
			this.listBoxOperations.TabIndex = 0;
			this.listBoxOperations.SelectedIndexChanged += new System.EventHandler(this.listBoxOperations_SelectedIndexChanged);
			// 
			// OperationsList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listBoxOperations);
			this.Name = "OperationsList";
			this.Size = new System.Drawing.Size(354, 298);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxOperations;
	}
}
