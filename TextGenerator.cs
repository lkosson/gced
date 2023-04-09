using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	public partial class TextGenerator : Form
	{
		private PathData? pathData;

		public TextGenerator()
		{
			InitializeComponent();
			textBoxText.Text = "Lorem ipsum";
			textBoxFont.Text = FontFamily.GenericSerif.Name;
			textBoxWidth.Text = "100";
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			panelPreview.Invalidate();
		}

		private void textBoxText_TextChanged(object sender, EventArgs e)
		{
			UpdateCode();
		}

		private void textBoxFont_TextChanged(object sender, EventArgs e)
		{
			UpdateCode();
		}

		private void buttonFont_Click(object sender, EventArgs e)
		{
			using var font = new Font(textBoxFont.Text, 18);
			fontDialog.Font = font;
			if (fontDialog.ShowDialog() != DialogResult.OK) return;
			textBoxFont.Text = fontDialog.Font.Name;
			UpdateCode();
		}

		private void textBoxWidth_TextChanged(object sender, EventArgs e)
		{
			UpdateCode();
		}

		private void textBoxHeight_TextChanged(object sender, EventArgs e)
		{
			UpdateCode();
		}

		private void UpdateCode()
		{
			if (String.IsNullOrEmpty(textBoxText.Text)) return;
			if (String.IsNullOrEmpty(textBoxFont.Text)) return;
			using var gp = new GraphicsPath();
			gp.AddString(textBoxText.Text, new FontFamily(textBoxFont.Text), 0, 20, new Point(0, 0), null);
			var pd = gp.PathData;
			if (pd.Points == null || pd.Types == null || pd.Points.Length != pd.Types.Length) return;
			pathData = pd;
			panelPreview.Invalidate();
		}

		private void panelPreview_Paint(object sender, PaintEventArgs e)
		{
			if (pathData == null || pathData.Points == null || pathData.Types == null) return;

			e.Graphics.FillRectangle(Brushes.White, ClientRectangle);

			var maxX = 1f;
			var maxY = 1f;
			for (int i = 0; i < pathData.Points.Length; i++)
			{
				if (pathData.Points[i].X > maxX) maxX = pathData.Points[i].X;
				if (pathData.Points[i].Y > maxY) maxY = pathData.Points[i].Y;
			}

			var scaleX = 1f * panelPreview.ClientSize.Width / maxX;
			var scaleY = 1f * panelPreview.ClientSize.Height / maxY;

			var scale = scaleX > scaleY ? scaleY : scaleX;

			e.Graphics.ScaleTransform(scale, scale);
			if (scaleX > scaleY) e.Graphics.TranslateTransform((panelPreview.ClientSize.Width / scaleY - maxX) / 2, 0);
			else e.Graphics.TranslateTransform(0, (panelPreview.ClientSize.Height / scaleX - maxY) / 2);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			using var penOriginal = new Pen(Color.LightGreen, 5 / scale);
			using var penRapid = new Pen(Color.LightGray, 1 / scale);
			using var penLine = new Pen(Color.Black, 2 / scale);
			using var penCurve = new Pen(Color.Violet, 2 / scale);
			using var penOutline = new Pen(Color.Red, 1 / scale);
			using var gp = new GraphicsPath(pathData.Points, pathData.Types);

			e.Graphics.DrawRectangle(penOutline, 0, 0, maxX, maxY);
			e.Graphics.DrawPath(penOriginal, gp);

			var x = 0f;
			var y = 0f;
			var startx = 0f;
			var starty = 0f;
			var bezierStart = new PointF(0, 0);
			var bezier1 = new PointF(0, 0);
			var bezier2 = new PointF(0, 0);
			var bezierN = 0;
			for (int i = 0; i < pathData.Points.Length; i++)
			{
				var type = (PathPointType)pathData.Types[i];
				var point = pathData.Points[i];
				var start = type == PathPointType.Start;
				var end = (type & PathPointType.CloseSubpath) == PathPointType.CloseSubpath;
				var line = (type & PathPointType.PathTypeMask) == PathPointType.Line;
				var curve = (type & PathPointType.PathTypeMask) == PathPointType.Bezier;

				if (start)
				{
					e.Graphics.DrawLine(penRapid, x, y, point.X, point.Y);
					startx = point.X;
					starty = point.Y;
				}
				else if (line)
				{
					e.Graphics.DrawLine(penLine, x, y, point.X, point.Y);
				}
				else if (curve)
				{
					if (bezierN == 0)
					{
						bezierStart = new PointF(x, y);
						bezier1 = point;
						bezierN = 1;
					}
					else if (bezierN == 1)
					{
						bezier2 = point;
						bezierN = 2;
					}
					else if (bezierN == 2)
					{
						e.Graphics.DrawLine(penCurve, bezierStart.X, bezierStart.Y, point.X, point.Y);
						bezierN = 0;
					}
				}

				if (end) e.Graphics.DrawLine(penLine, point.X, point.Y, startx, starty);

				x = point.X;
				y = point.Y;
			}
		}

		private class DoubleBufferedPanel : Panel
		{
			public DoubleBufferedPanel()
			{
				DoubleBuffered = true;
			}
		}
	}
}
