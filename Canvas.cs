﻿using System;
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
		private bool mouseDragged;
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

			var viewClip = e.ClipRectangle;
			viewClip.Inflate(1, 1);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillRectangle(Brushes.LightGray, viewClip.Left, viewClip.Top, viewClip.Width, viewClip.Height);
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

			var absClipRect = ViewToAbs(viewClip);

			ItemCount = 0;
			VisCount = 0;
			foreach (var item in items)
			{
				ItemCount++;
				if (!item.AbsBoundingBox.IntersectsWith(absClipRect)) continue;
				item.Draw(e.Graphics, style);
				VisCount++;
			}

			sw.Stop();
			PaintTime += (int)sw.ElapsedMilliseconds;
			FrameCount++;
		}

		private void Invalidate(CanvasItem item)
		{
			var absBounds = item.AbsBoundingBox;
			var viewBound = AbsToView(absBounds);
			viewBound.Inflate(5, 5);
			Invalidate(viewBound);
		}

		private float ViewToAbs(int dist)
		{
			var abs = ViewToAbs(new Rectangle(0, 0, dist, dist));
			return (float)Math.Sqrt(abs.Width * abs.Width + abs.Height * abs.Height);
		}

		private PointF ViewToAbs(Point point)
		{
			var probe = new[] { new PointF(point.X, point.Y) };
			inverseViewMatrix.TransformPoints(probe);
			return probe[0];
		}

		private RectangleF ViewToAbs(Rectangle rectView)
		{
			var bounds = new[] { new PointF(rectView.X, rectView.Y), new PointF(rectView.X + rectView.Width, rectView.Y + rectView.Height) };
			inverseViewMatrix.TransformPoints(bounds);
			var absX1 = Math.Min(bounds[0].X, bounds[1].X);
			var absY1 = Math.Min(bounds[0].Y, bounds[1].Y);
			var absX2 = Math.Max(bounds[0].X, bounds[1].X);
			var absY2 = Math.Max(bounds[0].Y, bounds[1].Y);
			return new RectangleF(absX1, absY1, absX2 - absX1, absY2 - absY1);
		}

		private int AbsToView(float dist)
		{
			var abs = AbsToView(new RectangleF(0, 0, dist, dist));
			return (int)Math.Sqrt(abs.Width * abs.Width + abs.Height * abs.Height);
		}

		private Point AbsToView(PointF point)
		{
			var probe = new[] { point };
			viewMatrix.TransformPoints(probe);
			return new Point((int)Math.Ceiling(probe[0].X), (int)Math.Ceiling(probe[0].Y));
		}

		private Rectangle AbsToView(RectangleF rectAbs)
		{
			var bounds = new[] { new PointF(rectAbs.X, rectAbs.Y), new PointF(rectAbs.X + rectAbs.Width, rectAbs.Y + rectAbs.Height) };
			viewMatrix.TransformPoints(bounds);
			var viewX1 = (int)Math.Floor(Math.Min(bounds[0].X, bounds[1].X));
			var viewY1 = (int)Math.Floor(Math.Min(bounds[0].Y, bounds[1].Y));
			var viewX2 = (int)Math.Ceiling(Math.Max(bounds[0].X, bounds[1].X));
			var viewY2 = (int)Math.Ceiling(Math.Max(bounds[0].Y, bounds[1].Y));
			return new Rectangle(viewX1, viewY1, viewX2 - viewX1, viewY2 - viewY1);
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
			if (!mouseDragged)
			{
				foreach (var item in items)
				{
					if (item.Hovered && !item.Selected)
					{
						item.Selected = true;
						Invalidate(item);
					}
					else if (item.Selected)
					{
						item.Selected = false;
						Invalidate(item);
					}
				}
			}
			mouseDragged = false;
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
				mouseDragged = true;
				Invalidate();
			}

			if (mouseDragButton == MouseButtons.None)
			{
				var absPos = ViewToAbs(e.Location);
				var hoverDist = ViewToAbs(5);
				var bestItem = (CanvasItem?)null;
				var bestDistance = Single.MaxValue;

				foreach (var item in items)
				{
					var absInflatedBounds = item.AbsBoundingBox;
					absInflatedBounds.Inflate(hoverDist, hoverDist);
					var hovered = false;
					var distance = Single.MaxValue;
					if (absInflatedBounds.Left <= absPos.X && absInflatedBounds.Right >= absPos.X && absInflatedBounds.Top <= absPos.Y && absInflatedBounds.Bottom >= absPos.Y)
					{
						distance = item.Distance(absPos);
						if (distance < hoverDist) hovered = true;
					}
					if (hovered)
					{
						if (bestItem == null)
						{
							bestItem = item;
							bestDistance = distance;
						}
						else if (bestDistance > distance)
						{
							if (bestItem.Hovered)
							{
								bestItem.Hovered = false;
								Invalidate(bestItem);
							}
							bestDistance = distance;
							bestItem = item;
						}
						else
						{
							if (item.Hovered)
							{
								item.Hovered = false;
								Invalidate(item);
							}
						}
					}
					else
					{
						if (item.Hovered)
						{
							item.Hovered = false;
							Invalidate(item);
						}
					}
				}

				if (bestItem != null && !bestItem.Hovered)
				{
					bestItem.Hovered = true;
					Invalidate(bestItem);
				}
			}
		}
	}
}
