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
			listBoxOperations = new System.Windows.Forms.ListBox();
			SuspendLayout();
			// 
			// listBoxOperations
			// 
			listBoxOperations.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			listBoxOperations.FormattingEnabled = true;
			listBoxOperations.IntegralHeight = false;
			listBoxOperations.ItemHeight = 15;
			listBoxOperations.Location = new System.Drawing.Point(3, 3);
			listBoxOperations.Name = "listBoxOperations";
			listBoxOperations.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			listBoxOperations.Size = new System.Drawing.Size(348, 289);
			listBoxOperations.TabIndex = 0;
			listBoxOperations.SelectedIndexChanged += listBoxOperations_SelectedIndexChanged;
			listBoxOperations.KeyDown += listBoxOperations_KeyDown;
			listBoxOperations.KeyPress += listBoxOperations_KeyPress;
			// 
			// OperationsList
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(listBoxOperations);
			Name = "OperationsList";
			Size = new System.Drawing.Size(354, 298);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.ListBox listBoxOperations;
	}
}
