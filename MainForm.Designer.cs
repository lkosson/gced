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
			openFileDialog = new System.Windows.Forms.OpenFileDialog();
			saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			splitContainer = new System.Windows.Forms.SplitContainer();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// canvas
			// 
			canvas.Dock = System.Windows.Forms.DockStyle.Fill;
			canvas.Location = new System.Drawing.Point(0, 0);
			canvas.Name = "canvas";
			canvas.ShowCursorCoords = true;
			canvas.ShowFPS = true;
			canvas.ShowItemCoords = true;
			canvas.ShowMajorGrid = true;
			canvas.ShowMinorGrid = true;
			canvas.ShowOriginGrid = true;
			canvas.Size = new System.Drawing.Size(547, 538);
			canvas.SnapToGrid = false;
			canvas.SnapToItems = true;
			canvas.TabIndex = 0;
			// 
			// operationProperties
			// 
			operationProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			operationProperties.Location = new System.Drawing.Point(3, 3);
			operationProperties.Name = "operationProperties";
			operationProperties.Size = new System.Drawing.Size(243, 334);
			operationProperties.TabIndex = 1;
			// 
			// operationsList
			// 
			operationsList.Dock = System.Windows.Forms.DockStyle.Fill;
			operationsList.Location = new System.Drawing.Point(3, 379);
			operationsList.Name = "operationsList";
			operationsList.Size = new System.Drawing.Size(243, 156);
			operationsList.TabIndex = 2;
			// 
			// lineEditor
			// 
			lineEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			lineEditor.Location = new System.Drawing.Point(3, 343);
			lineEditor.Name = "lineEditor";
			lineEditor.Size = new System.Drawing.Size(243, 30);
			lineEditor.TabIndex = 3;
			// 
			// openFileDialog
			// 
			openFileDialog.Filter = "G-Code files (*.nc, *.gcode)|*.nc;*.gcode|All files|*.*";
			openFileDialog.RestoreDirectory = true;
			// 
			// saveFileDialog
			// 
			saveFileDialog.Filter = "G-Code files (*.nc, *.gcode)|*.nc;*.gcode|All files|*.*";
			saveFileDialog.RestoreDirectory = true;
			// 
			// splitContainer
			// 
			splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			splitContainer.Location = new System.Drawing.Point(0, 0);
			splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			splitContainer.Panel1.Controls.Add(canvas);
			// 
			// splitContainer.Panel2
			// 
			splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
			splitContainer.Size = new System.Drawing.Size(800, 538);
			splitContainer.SplitterDistance = 547;
			splitContainer.TabIndex = 5;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(operationProperties, 0, 0);
			tableLayoutPanel1.Controls.Add(operationsList, 0, 2);
			tableLayoutPanel1.Controls.Add(lineEditor, 0, 1);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new System.Drawing.Size(249, 538);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 538);
			Controls.Add(splitContainer);
			Name = "MainForm";
			Text = "GCEd";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
			splitContainer.ResumeLayout(false);
			tableLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Canvas canvas;
		private OperationProperties operationProperties;
		private OperationsList operationsList;
		private LineEditor lineEditor;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}