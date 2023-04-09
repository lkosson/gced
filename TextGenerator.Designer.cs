namespace GCEd
{
	partial class TextGenerator
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
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			textBoxText = new System.Windows.Forms.TextBox();
			textBoxFont = new System.Windows.Forms.TextBox();
			textBoxWidth = new System.Windows.Forms.TextBox();
			textBoxHeight = new System.Windows.Forms.TextBox();
			buttonFont = new System.Windows.Forms.Button();
			panelPreview = new System.Windows.Forms.Panel();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			buttonOK = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			fontDialog = new System.Windows.Forms.FontDialog();
			tableLayoutPanel1.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 3;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Controls.Add(label2, 0, 1);
			tableLayoutPanel1.Controls.Add(label3, 0, 2);
			tableLayoutPanel1.Controls.Add(label4, 0, 3);
			tableLayoutPanel1.Controls.Add(textBoxText, 1, 0);
			tableLayoutPanel1.Controls.Add(textBoxFont, 1, 1);
			tableLayoutPanel1.Controls.Add(textBoxWidth, 1, 2);
			tableLayoutPanel1.Controls.Add(textBoxHeight, 1, 3);
			tableLayoutPanel1.Controls.Add(buttonFont, 2, 1);
			tableLayoutPanel1.Controls.Add(panelPreview, 0, 4);
			tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 5);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 6;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(18, 22);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(28, 15);
			label1.TabIndex = 0;
			label1.Text = "Text";
			// 
			// label2
			// 
			label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 68);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(31, 15);
			label2.TabIndex = 0;
			label2.Text = "Font";
			// 
			// label3
			// 
			label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(7, 98);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(39, 15);
			label3.TabIndex = 0;
			label3.Text = "Width";
			// 
			// label4
			// 
			label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(3, 127);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(43, 15);
			label4.TabIndex = 0;
			label4.Text = "Height";
			// 
			// textBoxText
			// 
			textBoxText.AcceptsReturn = true;
			textBoxText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			tableLayoutPanel1.SetColumnSpan(textBoxText, 2);
			textBoxText.Location = new System.Drawing.Point(52, 3);
			textBoxText.Multiline = true;
			textBoxText.Name = "textBoxText";
			textBoxText.Size = new System.Drawing.Size(745, 54);
			textBoxText.TabIndex = 1;
			// 
			// textBoxFont
			// 
			textBoxFont.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			textBoxFont.Location = new System.Drawing.Point(52, 64);
			textBoxFont.Name = "textBoxFont";
			textBoxFont.Size = new System.Drawing.Size(713, 23);
			textBoxFont.TabIndex = 2;
			// 
			// textBoxWidth
			// 
			textBoxWidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
			textBoxWidth.Location = new System.Drawing.Point(52, 94);
			textBoxWidth.Name = "textBoxWidth";
			textBoxWidth.Size = new System.Drawing.Size(49, 23);
			textBoxWidth.TabIndex = 3;
			// 
			// textBoxHeight
			// 
			textBoxHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
			textBoxHeight.Location = new System.Drawing.Point(52, 123);
			textBoxHeight.Name = "textBoxHeight";
			textBoxHeight.Size = new System.Drawing.Size(49, 23);
			textBoxHeight.TabIndex = 4;
			// 
			// buttonFont
			// 
			buttonFont.AutoSize = true;
			buttonFont.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			buttonFont.Location = new System.Drawing.Point(771, 63);
			buttonFont.Name = "buttonFont";
			buttonFont.Size = new System.Drawing.Size(26, 25);
			buttonFont.TabIndex = 5;
			buttonFont.Text = "...";
			buttonFont.UseVisualStyleBackColor = true;
			// 
			// panelPreview
			// 
			panelPreview.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			tableLayoutPanel1.SetColumnSpan(panelPreview, 3);
			panelPreview.Location = new System.Drawing.Point(3, 152);
			panelPreview.Name = "panelPreview";
			panelPreview.Size = new System.Drawing.Size(794, 260);
			panelPreview.TabIndex = 6;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 3);
			flowLayoutPanel1.Controls.Add(buttonOK);
			flowLayoutPanel1.Controls.Add(buttonCancel);
			flowLayoutPanel1.Location = new System.Drawing.Point(3, 418);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(794, 29);
			flowLayoutPanel1.TabIndex = 7;
			// 
			// buttonOK
			// 
			buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOK.Location = new System.Drawing.Point(3, 3);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(75, 23);
			buttonOK.TabIndex = 0;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCancel.Location = new System.Drawing.Point(84, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(75, 23);
			buttonCancel.TabIndex = 1;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// fontDialog
			// 
			fontDialog.ShowEffects = false;
			// 
			// TextGenerator
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 450);
			Controls.Add(tableLayoutPanel1);
			Name = "TextGenerator";
			Text = "Text generator";
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			flowLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxText;
		private System.Windows.Forms.TextBox textBoxFont;
		private System.Windows.Forms.TextBox textBoxWidth;
		private System.Windows.Forms.TextBox textBoxHeight;
		private System.Windows.Forms.Button buttonFont;
		private System.Windows.Forms.Panel panelPreview;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.FontDialog fontDialog;
	}
}