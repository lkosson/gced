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
			this.canvas = new GCEd.Canvas();
			this.operationProperties = new GCEd.OperationProperties();
			this.operationsList = new GCEd.OperationsList();
			this.SuspendLayout();
			// 
			// canvas
			// 
			this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.canvas.Location = new System.Drawing.Point(12, 12);
			this.canvas.Name = "canvas";
			this.canvas.Size = new System.Drawing.Size(570, 514);
			this.canvas.TabIndex = 0;
			// 
			// operationProperties
			// 
			this.operationProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.operationProperties.Location = new System.Drawing.Point(585, 12);
			this.operationProperties.Margin = new System.Windows.Forms.Padding(0);
			this.operationProperties.Name = "operationProperties";
			this.operationProperties.Size = new System.Drawing.Size(206, 334);
			this.operationProperties.TabIndex = 1;
			this.operationProperties.OperationUpdated += new System.EventHandler(this.operationProperties_OperationUpdated);
			// 
			// operationsList
			// 
			this.operationsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.operationsList.Location = new System.Drawing.Point(591, 349);
			this.operationsList.Name = "operationsList";
			this.operationsList.Size = new System.Drawing.Size(200, 177);
			this.operationsList.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 538);
			this.Controls.Add(this.operationsList);
			this.Controls.Add(this.operationProperties);
			this.Controls.Add(this.canvas);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);

		}

		#endregion

		private Canvas canvas;
		private OperationProperties operationProperties;
		private OperationsList operationsList;
	}
}