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
			listViewOperations = new DoubleBufferedListView();
			SuspendLayout();
			// 
			// listViewOperations
			// 
			listViewOperations.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			listViewOperations.FullRowSelect = true;
			listViewOperations.Location = new System.Drawing.Point(3, 3);
			listViewOperations.Name = "listViewOperations";
			listViewOperations.ShowItemToolTips = true;
			listViewOperations.Size = new System.Drawing.Size(348, 292);
			listViewOperations.TabIndex = 0;
			listViewOperations.UseCompatibleStateImageBehavior = false;
			listViewOperations.View = System.Windows.Forms.View.Details;
			listViewOperations.SelectedIndexChanged += listBoxOperations_SelectedIndexChanged;
			listViewOperations.KeyDown += listViewOperations_KeyDown;
			listViewOperations.KeyPress += listViewOperations_KeyPress;
			listViewOperations.KeyUp += listViewOperations_KeyUp;
			listViewOperations.MouseUp += listViewOperations_MouseUp;
			// 
			// OperationsList
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(listViewOperations);
			Name = "OperationsList";
			Size = new System.Drawing.Size(354, 298);
			ResumeLayout(false);
		}

		#endregion

		private DoubleBufferedListView listViewOperations;
	}
}
