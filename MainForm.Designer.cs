namespace GCEd
{
	partial class MainForm
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
			canvas = new Canvas();
			operationProperties = new OperationProperties();
			operationsList = new OperationsList();
			lineEditor = new LineEditor();
			SuspendLayout();
			// 
			// canvas
			// 
			canvas.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			canvas.Location = new System.Drawing.Point(12, 12);
			canvas.Name = "canvas";
			canvas.ShowCursorCoords = true;
			canvas.ShowFPS = true;
			canvas.ShowItemCoords = true;
			canvas.ShowMajorGrid = true;
			canvas.ShowMinorGrid = true;
			canvas.ShowOriginGrid = true;
			canvas.Size = new System.Drawing.Size(570, 514);
			canvas.TabIndex = 0;
			// 
			// operationProperties
			// 
			operationProperties.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			operationProperties.Location = new System.Drawing.Point(585, 12);
			operationProperties.Margin = new System.Windows.Forms.Padding(0);
			operationProperties.Name = "operationProperties";
			operationProperties.Size = new System.Drawing.Size(206, 334);
			operationProperties.TabIndex = 1;
			// 
			// operationsList
			// 
			operationsList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			operationsList.Location = new System.Drawing.Point(591, 387);
			operationsList.Name = "operationsList";
			operationsList.Size = new System.Drawing.Size(200, 139);
			operationsList.TabIndex = 2;
			// 
			// lineEditor
			// 
			lineEditor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			lineEditor.Location = new System.Drawing.Point(591, 351);
			lineEditor.Name = "lineEditor";
			lineEditor.Size = new System.Drawing.Size(200, 30);
			lineEditor.TabIndex = 3;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 538);
			Controls.Add(lineEditor);
			Controls.Add(operationsList);
			Controls.Add(operationProperties);
			Controls.Add(canvas);
			Name = "MainForm";
			Text = "MainForm";
			ResumeLayout(false);
		}

		#endregion

		private Canvas canvas;
		private OperationProperties operationProperties;
		private OperationsList operationsList;
		private LineEditor lineEditor;
	}
}