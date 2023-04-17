using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	partial class TextGenerator : Form
	{
		private GraphicsPath path;
		private List<CanvasItem> items;
		private bool ready;
		public GProgram? Program { get; private set; }

		public TextGenerator()
		{
			InitializeComponent();
			path = new GraphicsPath();
			items = new List<CanvasItem>();
			textBoxText.Text = "Lorem ipsum";
			textBoxFont.Text = FontFamily.GenericSerif.Name;
			textBoxWidth.Text = "100";
			textBoxX.Text = "0";
			textBoxY.Text = "0";
			ready = true;
			UpdateCode();
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
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

		private void textBoxX_TextChanged(object sender, EventArgs e)
		{
			UpdateCode();
		}

		private void textBoxY_TextChanged(object sender, EventArgs e)
		{
			UpdateCode();
		}

		private void UpdateCode()
		{
			if (!ready) return;
			if (String.IsNullOrEmpty(textBoxText.Text)) return;
			if (String.IsNullOrEmpty(textBoxFont.Text)) return;
			if (!Single.TryParse(textBoxWidth.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var desiredWidth) && !String.IsNullOrEmpty(textBoxWidth.Text)) return;
			if (!Single.TryParse(textBoxHeight.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var desiredHeight) && !String.IsNullOrEmpty(textBoxHeight.Text)) return;
			if (!Decimal.TryParse(textBoxX.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var originX)) return;
			if (!Decimal.TryParse(textBoxY.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var originY)) return;
			if (path != null) path.Dispose();

			path = new GraphicsPath();
			path.AddString(textBoxText.Text, new FontFamily(textBoxFont.Text), (int)FontStyle.Regular, 20, new Point(0, 0), null);

			var pathData = path.PathData;
			if (pathData.Points == null || pathData.Types == null || pathData.Points.Length != pathData.Types.Length) return;
			var maxX = Single.MinValue;
			var maxY = Single.MinValue;
			var minX = Single.MaxValue;
			var minY = Single.MaxValue;
			for (int i = 0; i < pathData.Points.Length; i++)
			{
				var point = pathData.Points[i];
				if (point.X < minX) minX = point.X;
				if (point.Y < minY) minY = point.Y;
				if (point.X > maxX) maxX = point.X;
				if (point.Y > maxY) maxY = point.Y;
			}
			using var matrix = new Matrix();
			matrix.Translate(-minX, -maxY);
			if (desiredWidth > 0)
			{
				if (desiredHeight > 0) matrix.Scale(desiredWidth / (maxX - minX), 1, MatrixOrder.Append);
				else matrix.Scale(desiredWidth / (maxX - minX), desiredWidth / (maxX - minX), MatrixOrder.Append);
			}
			if (desiredHeight > 0)
			{
				if (desiredWidth > 0) matrix.Scale(1, desiredHeight / (maxY - minY), MatrixOrder.Append);
				else matrix.Scale(desiredHeight / (maxY - minY), desiredHeight / (maxY - minY), MatrixOrder.Append);
			}
			path.Transform(matrix);
			pathData = path.PathData;
			if (pathData.Points == null || pathData.Types == null || pathData.Points.Length != pathData.Types.Length) return;

			var program = new GProgram();

			var currentPoint = new Vector2(0, 0);
			var startPoint = new Vector2(0, 0);
			var bezierPoints = new List<Vector2>(4);

			for (int i = 0; i < pathData.Points.Length; i++)
			{
				var type = (PathPointType)pathData.Types[i];
				var pointF = pathData.Points[i];
				var start = type == PathPointType.Start;
				var end = (type & PathPointType.CloseSubpath) == PathPointType.CloseSubpath;
				var line = (type & PathPointType.PathTypeMask) == PathPointType.Line;
				var curve = (type & PathPointType.PathTypeMask) == PathPointType.Bezier;

				var point = new Vector2(pointF.X, -pointF.Y);

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
						var arcs = ArcsFromBezier(bezierPoints, 0, 1);
						foreach (var arc in arcs) program.Lines.AddLast(arc);
						bezierPoints.Clear();
					}
				}
				if (end)
				{
					program.Lines.AddLast(new GLine { Instruction = GInstruction.G1, X = (decimal)startPoint.X, Y = (decimal)startPoint.Y });
				}

				currentPoint = point;
			}

			foreach (var line in program.Lines)
			{
				line.X += originX;
				line.Y += originY;
			}

			var operations = program.Run();

			items.Clear();
			foreach (var operation in operations)
			{
				var item = CanvasItem.FromOperation(operation);
				if (item == null) continue;
				items.Add(item);
			}

			panelPreview.Invalidate();
			Program = program;
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
			//e.Graphics.DrawPath(penOriginal, path);

			e.Graphics.ScaleTransform(1, -1);
			e.Graphics.DrawRectangle(penOutline, minX, minY, width, height);

			foreach (var item in items)
			{
				item.Draw(e.Graphics, style);
			}

			textBoxWidth.PlaceholderText = width.ToString("0.000", CultureInfo.InvariantCulture);
			textBoxHeight.PlaceholderText = height.ToString("0.000", CultureInfo.InvariantCulture);
		}

		private Vector2 BezierEval(IList<Vector2> points, double t)
		{
			var t1 = 1 - t;
			var c1 = t1 * t1 * t1;
			var c2 = 3 * t1 * t1 * t;
			var c3 = 3 * t1 * t * t;
			var c4 = t * t * t;
			var x = points[0].X * c1 + points[1].X * c2 + points[2].X * c3 + points[3].X * c4;
			var y = points[0].Y * c1 + points[1].Y * c2 + points[2].Y * c3 + points[3].Y * c4;
			return new Vector2((float)x, (float)y);
		}

		private IEnumerable<GLine> ArcsFromBezier(IList<Vector2> bezierPoints, float start, float end)
		{
			var span = end - start;
			var startPoint = BezierEval(bezierPoints, start);
			var midPoint14 = BezierEval(bezierPoints, start + 0.25f * span);
			var midPoint24 = BezierEval(bezierPoints, start + 0.50f * span);
			var midPoint34 = BezierEval(bezierPoints, start + 0.75f * span);
			var endPoint = BezierEval(bezierPoints, end);

			var centerPoint = Geometry.CircleCenterFromThreePoints(startPoint, midPoint24, endPoint);
			var radius = Geometry.LineLength(centerPoint, startPoint);
			var error14 = Math.Abs(Geometry.LineLength(centerPoint, midPoint14) - radius) / radius;
			var error34 = Math.Abs(Geometry.LineLength(centerPoint, midPoint34) - radius) / radius;
			if (error14 > 0.01 || error34 > 0.01)
			{
				var firstHalf = ArcsFromBezier(bezierPoints, start, start + span / 2);
				var secondHalf = ArcsFromBezier(bezierPoints, start + span / 2, end);
				return Enumerable.Concat(firstHalf, secondHalf);
			}
			else
			{
				var side = (endPoint.X - startPoint.X) * (centerPoint.Y - startPoint.Y)
					- (endPoint.Y - startPoint.Y) * (centerPoint.X - startPoint.X);

				GLine line;
				if (Single.IsInfinity(centerPoint.X) || Single.IsInfinity(centerPoint.Y)) line = new GLine { Instruction = GInstruction.G1, X = (decimal)endPoint.X, Y = (decimal)endPoint.Y };
				else line = new GLine { Instruction = side > 0 ? GInstruction.G3 : GInstruction.G2, X = (decimal)endPoint.X, Y = (decimal)endPoint.Y, I = (decimal)(centerPoint.X - startPoint.X), J = (decimal)(centerPoint.Y - startPoint.Y) };

				return Enumerable.Repeat(line, 1);
			}
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
