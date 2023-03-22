﻿namespace GCEd
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
			this.components = new System.ComponentModel.Container();
			GCEd.GProgram gProgram1 = new GCEd.GProgram();
			this.canvas = new GCEd.Canvas();
			this.timerFPSCounter = new System.Windows.Forms.Timer(this.components);
			this.operationProperties = new GCEd.OperationProperties();
			this.SuspendLayout();
			// 
			// canvas
			// 
			this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.canvas.FrameCount = 92;
			this.canvas.ItemCount = 0;
			this.canvas.Location = new System.Drawing.Point(12, 12);
			this.canvas.Name = "canvas";
			this.canvas.PaintTime = 841;
			this.canvas.Program = gProgram1;
			this.canvas.Size = new System.Drawing.Size(570, 426);
			this.canvas.TabIndex = 0;
			this.canvas.VisCount = 0;
			this.canvas.SelectedOperationChanged += new System.EventHandler(this.canvas_SelectedOperationChanged);
			// 
			// timerFPSCounter
			// 
			this.timerFPSCounter.Enabled = true;
			this.timerFPSCounter.Tick += new System.EventHandler(this.timerFPSCounter_Tick);
			// 
			// operationProperties
			// 
			this.operationProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.operationProperties.Location = new System.Drawing.Point(585, 12);
			this.operationProperties.Margin = new System.Windows.Forms.Padding(0);
			this.operationProperties.Name = "operationProperties";
			this.operationProperties.Operation = null;
			this.operationProperties.Size = new System.Drawing.Size(206, 426);
			this.operationProperties.TabIndex = 1;
			this.operationProperties.OperationUpdated += new System.EventHandler(this.operationProperties_OperationUpdated);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.operationProperties);
			this.Controls.Add(this.canvas);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);

		}

		#endregion

		private Canvas canvas;
		private System.Windows.Forms.Timer timerFPSCounter;
		private OperationProperties operationProperties;
	}
}