using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
		public int ItemCount { get; set; }
		public int VisCount { get; set; }

		private Matrix viewMatrix;
		private Matrix inverseViewMatrix;
		private Point mouseDragStart;
		private MouseButtons mouseDragButton;
		private bool matrixUpdated;
		private CanvasStyle style;

		private IEnumerable<CanvasItem> items;

		public Canvas()
		{
			DoubleBuffered = true;
			viewMatrix = new Matrix();
			inverseViewMatrix = new Matrix();
			style = new CanvasStyle();
			var p = new GProgram();
			p.Read("test.nc");
			var ops = p.Run();
			var items = new List<CanvasItem>();
			foreach (var op in ops)
			{
				if (op.Line.Instruction == GInstruction.G0 || op.Line.Instruction == GInstruction.G1) items.Add(new CanvasItemLine(op));
				else if (op.Line.Instruction == GInstruction.G2 || op.Line.Instruction == GInstruction.G3) items.Add(new CanvasItemArc(op));
			}
			this.items = items;
		}

		protected override void OnLoad(EventArgs e)
		{
			viewMatrix.Scale(1f, -1f);
			viewMatrix.Translate(0f, -Height);
			matrixUpdated = true;
			base.OnLoad(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var sw = Stopwatch.StartNew();

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);
			e.Graphics.MultiplyTransform(viewMatrix);

			if (matrixUpdated)
			{
				inverseViewMatrix.Reset();
				inverseViewMatrix.Multiply(viewMatrix);
				inverseViewMatrix.Invert();
				style.ViewMatrixChanged(viewMatrix);
				foreach (var item in items) item.ViewMatrixChanged(viewMatrix);
				matrixUpdated = false;
			}

			var clipBounds = new[] { new PointF(e.ClipRectangle.X, e.ClipRectangle.Y), new PointF(e.ClipRectangle.X + e.ClipRectangle.Width, e.ClipRectangle.Y + e.ClipRectangle.Height) };
			inverseViewMatrix.TransformPoints(clipBounds);
			var clipX1 = Math.Min(clipBounds[0].X, clipBounds[1].X);
			var clipY1 = Math.Min(clipBounds[0].Y, clipBounds[1].Y);
			var clipX2 = Math.Max(clipBounds[0].X, clipBounds[1].X);
			var clipY2 = Math.Max(clipBounds[0].Y, clipBounds[1].Y);
			var absClipRectangle = new RectangleF(clipX1, clipY1, clipX2 - clipX1, clipY2 - clipY1);

			ItemCount = 0;
			VisCount = 0;
			foreach (var item in items)
			{
				ItemCount++;
				if (!item.AbsBoundingBox.IntersectsWith(absClipRectangle)) continue;
				item.Draw(e.Graphics, style);
				VisCount++;
			}

			sw.Stop();
			PaintTime += (int)sw.ElapsedMilliseconds;
			FrameCount++;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			var scaleMatrix = new Matrix();

			scaleMatrix.Translate(e.X, e.Y);
			if (e.Delta > 0) scaleMatrix.Scale(1.1f, 1.1f);
			else scaleMatrix.Scale(1 / 1.1f, 1 / 1.1f);
			scaleMatrix.Translate(-e.X, -e.Y);

			scaleMatrix.Multiply(viewMatrix);

			viewMatrix = scaleMatrix;
			matrixUpdated = true;

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			mouseDragStart = e.Location;
			mouseDragButton = e.Button;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseDragButton = MouseButtons.None;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if ((mouseDragButton & MouseButtons.Left) == MouseButtons.Left)
			{
				var translateMatrix = new Matrix();
				translateMatrix.Translate(e.X - mouseDragStart.X, e.Y - mouseDragStart.Y);
				translateMatrix.Multiply(viewMatrix);
				viewMatrix = translateMatrix;
				matrixUpdated = true;

				mouseDragStart = e.Location;
				Invalidate();
			}
		}
	}
}
