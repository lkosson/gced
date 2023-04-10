﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	public partial class TextGenerator : Form
	{
		private GraphicsPath path;
		private List<CanvasItem> items;

		public TextGenerator()
		{
			InitializeComponent();
			path = new GraphicsPath();
			items = new List<CanvasItem>();
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
			if (path != null) path.Dispose();
			path = new GraphicsPath();
			path.AddString(textBoxText.Text, new FontFamily(textBoxFont.Text), 0, 20, new Point(0, 0), null);
			var pathData = path.PathData;
			if (pathData.Points == null || pathData.Types == null || pathData.Points.Length != pathData.Types.Length) return;

			var program = new GProgram();

			var currentPoint = new PointF(0, 0);
			var startPoint = new PointF(0, 0);
			var bezierPoints = new List<PointF>(4);

			for (int i = 0; i < pathData.Points.Length; i++)
			{
				var type = (PathPointType)pathData.Types[i];
				var point = pathData.Points[i];
				var start = type == PathPointType.Start;
				var end = (type & PathPointType.CloseSubpath) == PathPointType.CloseSubpath;
				var line = (type & PathPointType.PathTypeMask) == PathPointType.Line;
				var curve = (type & PathPointType.PathTypeMask) == PathPointType.Bezier;

				point = new PointF(point.X, -point.Y);

				if (start)
				{
					program.Lines.AddLast(new GLine { Instruction = GInstruction.G0, X = (decimal)point.X, Y = (decimal)point.Y });
					startPoint = point;
				}
				else if (line)
				{
					program.Lines.AddLast(new GLine { Instruction = GInstruction.G1, X = (decimal)point.X, Y = (decimal)point.Y });
				}
				else if (curve)
				{
					if (bezierPoints.Count == 0) bezierPoints.Add(currentPoint);
					bezierPoints.Add(point);
					if (bezierPoints.Count == 4)
					{
						var midPoint24 = BezierEval(bezierPoints, 0.50);

						var centerPoint = Geometry.CircleCenterFromThreePoints(bezierPoints[0], midPoint24, bezierPoints[3]);
						var side = (bezierPoints[3].X - bezierPoints[0].X) * (centerPoint.Y - bezierPoints[0].Y)
							- (bezierPoints[3].Y - bezierPoints[0].Y) * (centerPoint.X - bezierPoints[0].X);

						if (Single.IsInfinity(centerPoint.X) || Single.IsInfinity(centerPoint.Y)) program.Lines.AddLast(new GLine { Instruction = GInstruction.G1, X = (decimal)point.X, Y = (decimal)point.Y });
						else program.Lines.AddLast(new GLine { Instruction = side > 0 ? GInstruction.G3 : GInstruction.G2, X = (decimal)point.X, Y = (decimal)point.Y, I = (decimal)(centerPoint.X - bezierPoints[0].X), J = (decimal)(centerPoint.Y - bezierPoints[0].Y) });
						bezierPoints.Clear();
					}
				}
				if (end)
				{
					program.Lines.AddLast(new GLine { Instruction = GInstruction.G1, X = (decimal)startPoint.X, Y = (decimal)startPoint.Y });
				}

				currentPoint = point;
			}

			items.Clear();
			var operations = program.Run();
			foreach (var operation in operations)
			{
				var item = CanvasItem.FromOperation(operation);
				if (item == null) continue;
				items.Add(item);
			}

			panelPreview.Invalidate();
		}

		private void panelPreview_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
			if (!items.Any()) return;

			var maxX = Single.MinValue;
			var maxY = Single.MinValue;
			var minX = Single.MaxValue;
			var minY = Single.MaxValue;
			foreach (var item in items)
			{
				if (item.Operation.Line.Instruction == GInstruction.G0) continue;
				if (item.AbsBoundingBox.X < minX) minX = item.AbsBoundingBox.X;
				if (item.AbsBoundingBox.Y < minY) minY = item.AbsBoundingBox.Y;
				if (item.AbsBoundingBox.X + item.AbsBoundingBox.Width > maxX) maxX = item.AbsBoundingBox.X + item.AbsBoundingBox.Width;
				if (item.AbsBoundingBox.Y + item.AbsBoundingBox.Height > maxY) maxY = item.AbsBoundingBox.Y + item.AbsBoundingBox.Height;
			}
			var width = maxX - minX;
			var height = maxY - minY;

			var scaleX = 1f * panelPreview.ClientSize.Width / width;
			var scaleY = 1f * panelPreview.ClientSize.Height / height;

			var scale = scaleX > scaleY ? scaleY : scaleX;

			e.Graphics.ScaleTransform(scale, scale);
			if (scaleX > scaleY) e.Graphics.TranslateTransform((panelPreview.ClientSize.Width / scale - width) / 2, 0);
			else e.Graphics.TranslateTransform(0, (panelPreview.ClientSize.Height / scaleX - height) / 2);

			using var style = new TextGeneratorCanvasStyle();
			style.ViewMatrixChanged(e.Graphics.Transform);

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			using var penOriginal = new Pen(Color.LightGreen, 5 / scale);
			using var penRapid = new Pen(Color.LightGray, 1 / scale);
			using var penLine = new Pen(Color.Black, 2 / scale);
			using var penCurve = new Pen(Color.Violet, 2 / scale);
			using var penOutline = new Pen(Color.Red, 1 / scale);

			e.Graphics.TranslateTransform(-minX, maxY);
			e.Graphics.DrawPath(penOriginal, path);

			e.Graphics.ScaleTransform(1, -1);
			e.Graphics.DrawRectangle(penOutline, minX, minY, width, height);

			foreach (var item in items)
			{
				item.Draw(e.Graphics, style);
			}

		}

		private PointF BezierEval(IList<PointF> points, double t)
		{
			var t1 = 1 - t;
			var c1 = t1 * t1 * t1;
			var c2 = 3 * t1 * t1 *t;
			var c3 = 3 * t1* t *t;
			var c4 = t * t * t;
			var x = points[0].X * c1 + points[1].X * c2 + points[2].X * c3 + points[3].X * c4;
			var y = points[0].Y * c1 + points[1].Y * c2 + points[2].Y * c3 + points[3].Y * c4;
			return new PointF((float)x, (float)y);
		}

		private class DoubleBufferedPanel : Panel
		{
			public DoubleBufferedPanel()
			{
				DoubleBuffered = true;
			}
		}

		private class TextGeneratorCanvasStyle : CanvasStyle
		{
			public override void ViewMatrixChanged(Matrix viewMatrix)
			{
				var probe = new[] { new Point(100, 0) };
				viewMatrix.VectorTransformPoints(probe);
				var len = Math.Max(probe[0].X / 100f, 0.0001f);
				PixelSize = 1 / len;
				IdlePen = new Pen(Color.Black, PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 10f, 10f } };
				ActivePen = new Pen(Color.Black, PixelSize);
				HoveredIdlePen = ActivePen;
				HoveredActivePen = ActivePen;
				SelectedIdlePen = ActivePen;
				SelectedActivePen = ActivePen;

				MinorGridPen = new Pen(Color.Transparent);
				MajorGridPen = MinorGridPen;
				OriginGridPen = MinorGridPen;
				SelectionPen = MinorGridPen;
				TextBrush = new SolidBrush(Color.Transparent);
				BackgroundBrush = TextBrush;
				SelectionBrush = TextBrush;
			}
		}
	}
}
