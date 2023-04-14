namespace GCEd
{
	partial class About
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
			tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			buttonOK = new System.Windows.Forms.Button();
			pictureBoxLogo = new System.Windows.Forms.PictureBox();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			linkLabelUrl = new System.Windows.Forms.LinkLabel();
			tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			tableLayoutPanel.AutoSize = true;
			tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel.ColumnCount = 2;
			tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel.Controls.Add(buttonOK, 0, 1);
			tableLayoutPanel.Controls.Add(pictureBoxLogo, 0, 0);
			tableLayoutPanel.Controls.Add(flowLayoutPanel1, 1, 0);
			tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel.Name = "tableLayoutPanel";
			tableLayoutPanel.RowCount = 2;
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			tableLayoutPanel.Size = new System.Drawing.Size(278, 103);
			tableLayoutPanel.TabIndex = 0;
			// 
			// buttonOK
			// 
			buttonOK.AutoSize = true;
			buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOK.Location = new System.Drawing.Point(3, 75);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(75, 25);
			buttonOK.TabIndex = 0;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// pictureBoxLogo
			// 
			pictureBoxLogo.Location = new System.Drawing.Point(3, 3);
			pictureBoxLogo.Name = "pictureBoxLogo";
			pictureBoxLogo.Size = new System.Drawing.Size(75, 66);
			pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			pictureBoxLogo.TabIndex = 1;
			pictureBoxLogo.TabStop = false;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.Controls.Add(label1);
			flowLayoutPanel1.Controls.Add(label2);
			flowLayoutPanel1.Controls.Add(linkLabelUrl);
			flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			flowLayoutPanel1.Location = new System.Drawing.Point(84, 3);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(191, 66);
			flowLayoutPanel1.TabIndex = 4;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			label1.Location = new System.Drawing.Point(70, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(50, 21);
			label1.TabIndex = 2;
			label1.Text = "GCEd";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 21);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(181, 30);
			label2.TabIndex = 3;
			label2.Text = "Simple 2.5D Visual G-Code Editor\r\nAuthor: Łukasz Kosson";
			// 
			// linkLabelUrl
			// 
			linkLabelUrl.AutoSize = true;
			linkLabelUrl.Location = new System.Drawing.Point(3, 51);
			linkLabelUrl.Name = "linkLabelUrl";
			linkLabelUrl.Size = new System.Drawing.Size(185, 15);
			linkLabelUrl.TabIndex = 3;
			linkLabelUrl.TabStop = true;
			linkLabelUrl.Text = "https://github.com/lkosson/gced";
			linkLabelUrl.LinkClicked += linkLabelUrl_LinkClicked;
			// 
			// About
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			CancelButton = buttonOK;
			ClientSize = new System.Drawing.Size(278, 103);
			Controls.Add(tableLayoutPanel);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "About";
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "About GCEd";
			tableLayoutPanel.ResumeLayout(false);
			tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.PictureBox pictureBoxLogo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabelUrl;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label label2;
	}
}