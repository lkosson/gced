using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	public partial class OutlineParameters : Form
	{
		public float Thickness { get; set; }
		public bool LeftFirst { get; set; }
		public bool SkipRight { get; set; }
		public bool SkipLeft { get; set; }
		public bool BothForward { get; set; }

		public OutlineParameters()
		{
			InitializeComponent();
			textBoxThickness.Text = "1.0";
			comboBoxOrder.SelectedIndex = 0;
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				if (!Single.TryParse(textBoxThickness.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var thickess))
				{
					e.Cancel = true;
					DialogResult = DialogResult.None;
					return;
				}

				Thickness = thickess;
				LeftFirst = comboBoxOrder.SelectedIndex == 3 || comboBoxOrder.SelectedIndex == 4 || comboBoxOrder.SelectedIndex == 5;
				SkipRight = comboBoxOrder.SelectedIndex == 5;
				SkipLeft = comboBoxOrder.SelectedIndex == 2;
				BothForward = comboBoxOrder.SelectedIndex == 1 || comboBoxOrder.SelectedIndex == 4;
			}
			base.OnClosing(e);
		}
	}
}
