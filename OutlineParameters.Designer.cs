namespace GCEd
{
	partial class OutlineParameters
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
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			buttonOK = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			textBoxThickness = new System.Windows.Forms.TextBox();
			comboBoxOrder = new System.Windows.Forms.ComboBox();
			tableLayoutPanel1.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Controls.Add(label2, 0, 1);
			tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 2);
			tableLayoutPanel1.Controls.Add(textBoxThickness, 1, 0);
			tableLayoutPanel1.Controls.Add(comboBoxOrder, 1, 1);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			tableLayoutPanel1.Size = new System.Drawing.Size(192, 92);
			tableLayoutPanel1.TabIndex = 1;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 7);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(58, 15);
			label1.TabIndex = 0;
			label1.Text = "Thickness";
			// 
			// label2
			// 
			label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(24, 36);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(37, 15);
			label2.TabIndex = 0;
			label2.Text = "Order";
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 2);
			flowLayoutPanel1.Controls.Add(buttonOK);
			flowLayoutPanel1.Controls.Add(buttonCancel);
			flowLayoutPanel1.Location = new System.Drawing.Point(3, 61);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(186, 29);
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
			// textBoxThickness
			// 
			textBoxThickness.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			textBoxThickness.Location = new System.Drawing.Point(67, 3);
			textBoxThickness.Name = "textBoxThickness";
			textBoxThickness.Size = new System.Drawing.Size(122, 23);
			textBoxThickness.TabIndex = 2;
			// 
			// comboBoxOrder
			// 
			comboBoxOrder.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			comboBoxOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxOrder.FormattingEnabled = true;
			comboBoxOrder.Items.AddRange(new object[] { "Right, then left in reverse", "Right, then left forward", "Only right", "Left, then right in reverse", "Left, then right forward", "Only left" });
			comboBoxOrder.Location = new System.Drawing.Point(67, 32);
			comboBoxOrder.Name = "comboBoxOrder";
			comboBoxOrder.Size = new System.Drawing.Size(122, 23);
			comboBoxOrder.TabIndex = 8;
			// 
			// OutlineParameters
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			CancelButton = buttonCancel;
			ClientSize = new System.Drawing.Size(250, 92);
			Controls.Add(tableLayoutPanel1);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "OutlineParameters";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Outline parameters";
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			flowLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxThickness;
		private System.Windows.Forms.ComboBox comboBoxOrder;
	}
}