using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	public partial class About : Form
	{
		public About()
		{
			InitializeComponent();
			var icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
			Icon = icon;
			pictureBoxLogo.Image = icon!.ToBitmap();
		}

		private void linkLabelUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://github.com/lkosson/gced") { UseShellExecute = true });
		}
	}
}
