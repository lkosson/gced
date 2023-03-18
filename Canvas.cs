using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	public partial class Canvas : UserControl
	{
		public int PaintTime { get; set; }
		public int FrameCount { get; set; }

		public Canvas()
		{
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var sw = Stopwatch.StartNew();

			e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);
			var rnd = new Random();

			e.Graphics.TranslateTransform(1000, 1200);

			for (int i = 0; i < 1000; i++)
			{
				e.Graphics.DrawLine(Pens.Blue, rnd.Next(-1000, 1000), rnd.Next(-1000, 1000), rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
			}

			sw.Stop();
			PaintTime += (int)sw.ElapsedMilliseconds;
			FrameCount++;
		}
	}
}
