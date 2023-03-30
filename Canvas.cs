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
	partial class Canvas : UserControl
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewState ViewState
		{
			get => viewState;
			set
			{
				if (viewState != null)
				{
					viewState.OperationsChanged -= ViewState_OperationsChanged;
					viewState.SelectedOperationsChanged -= ViewState_SelectedOperationsChanged;
				}
				viewState = value;
				viewState.OperationsChanged += ViewState_OperationsChanged;
				viewState.SelectedOperationsChanged += ViewState_SelectedOperationsChanged;
			}
		}

		public bool ShowFPS { get; set; } = true;

		private ViewState viewState;
		private Matrix viewMatrix;
		private Matrix inverseViewMatrix;
		private Point mouseDragStart;
		private bool matrixUpdated;
		private CanvasStyle style;
		private IEnumerable<CanvasItem> items;
		private Interaction interaction;

		private enum Interaction { None, DragOrSelect, Drag, DragSelect }

		public Canvas()
		{
			DoubleBuffered = true;
			viewState = new ViewState();
			viewMatrix = new Matrix();
			inverseViewMatrix = new Matrix();
			style = new CanvasStyle();
			items = Enumerable.Empty<CanvasItem>();
		}

		protected override void OnLoad(EventArgs e)
		{
			PanZoomViewToFit();
			base.OnLoad(e);
		}

		private void ViewState_OperationsChanged()
		{
			var items = new List<CanvasItem>();
			foreach (var operation in viewState.Operations)
			{
				CanvasItem item;
				if (operation.Line.Instruction == GInstruction.G0 || operation.Line.Instruction == GInstruction.G1) item = new CanvasItemLine(operation);
				else if (operation.Line.Instruction == GInstruction.G2 || operation.Line.Instruction == GInstruction.G3) item = new CanvasItemArc(operation);
				else continue;
				items.Add(item);
			}
			this.items = items;
			Invalidate();
		}

		private void ViewState_SelectedOperationsChanged()
		{
			foreach (var item in items)
			{
				var selected = viewState.SelectedOperations.Contains(item.Operation);
				if (!(selected ^ item.Selected)) continue;
				item.Selected = selected;
				Invalidate(item);
			}
			PanViewToSelection();
		}

		private void PanZoomViewToFit()
		{
			var absX1 = Single.MaxValue;
			var absY1 = Single.MaxValue;
			var absX2 = Single.MinValue;
			var absY2 = Single.MinValue;

			foreach (var item in items)
			{
				var bounding = item.AbsBoundingBox;
				bounding.Inflate(5, 5);
				if (absX1 > bounding.Left) absX1 = bounding.Left;
				if (absY1 > bounding.Top) absY1 = bounding.Top;
				if (absX2 < bounding.Right) absX2 = bounding.Right;
				if (absY2 < bounding.Bottom) absY2 = bounding.Bottom;
			}

			var ratioAbs = (absX2 - absX1) / (absY2 - absY1);
			var ratioView = 1.0f * Width / Height;

			if (ratioAbs < ratioView)
			{
				var viewWidth = Height * (absX2 - absX1) / (absY2 - absY1);
				var viewWidthOffset = (Width - viewWidth) / 2;
				viewMatrix = new Matrix(new RectangleF(absX1, absY1, absX2 - absX1, absY2 - absY1), new[] { new PointF(viewWidthOffset, Height), new PointF(viewWidthOffset + viewWidth, Height), new PointF(viewWidthOffset, 0) });
			}
			else
			{
				var viewHeight = Width * (absY2 - absY1) / (absX2 - absX1);
				var viewHeightOffset = (Height - viewHeight) / 2;
				viewMatrix = new Matrix(new RectangleF(absX1, absY1, absX2 - absX1, absY2 - absY1), new[] { new PointF(0, viewHeightOffset + viewHeight), new PointF(Width, viewHeightOffset + viewHeight), new PointF(0, viewHeightOffset) });
			}

			matrixUpdated = true;

			Invalidate();
		}

		private void PanViewToSelection()
		{
			var absX1 = Single.MaxValue;
			var absY1 = Single.MaxValue;
			var absX2 = Single.MinValue;
			var absY2 = Single.MinValue;

			foreach (var item in items)
			{
				if (!item.Selected) continue;
				var bounding = item.AbsBoundingBox;
				if (absX1 > bounding.Left) absX1 = bounding.Left;
				if (absY1 > bounding.Top) absY1 = bounding.Top;
				if (absX2 < bounding.Right) absX2 = bounding.Right;
				if (absY2 < bounding.Bottom) absY2 = bounding.Bottom;
			}

			if (absX1 == Single.MaxValue) return;

			var viewSelectedBounds = AbsToView(new RectangleF(absX1, absY1, absX2 - absX1, absY2 - absY1));
			if (ClientRectangle.Contains(viewSelectedBounds)) return;

			viewMatrix.Translate(-viewSelectedBounds.X + Width / 2 - viewSelectedBounds.Width / 2, -viewSelectedBounds.Y + Height / 2 - viewSelectedBounds.Height / 2, MatrixOrder.Append);

			matrixUpdated = true;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var sw = Stopwatch.StartNew();

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			var viewClip = e.ClipRectangle;
			viewClip.Inflate(1, 1);
			e.Graphics.FillRectangle(Brushes.LightGray, viewClip.Left, viewClip.Top, viewClip.Width, viewClip.Height);

			if (DesignMode) return;

			var gs = e.Graphics.Save();
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

			var itemCount = 0;
			var visCount = 0;
			foreach (var item in items)
			{
				itemCount++;
				if (!item.AbsBoundingBox.IntersectsWith(absClipRect)) continue;
				item.Draw(e.Graphics, style);
				visCount++;
			}
			e.Graphics.Restore(gs);

			sw.Stop();
			var paintTime = (int)sw.ElapsedMilliseconds;

			if (ShowFPS)
			{
				e.Graphics.DrawString($"{paintTime} ms\n{(paintTime == 0 ? 999 : 1000 / paintTime)} fps\n{itemCount} items\n{visCount} visible", Font, Brushes.Black, 0, 0);
			}
		}

		private void Invalidate(CanvasItem item)
		{
			var absBounds = item.AbsBoundingBox;
			var viewBound = AbsToView(absBounds);
			viewBound.Inflate(5, 5);
			Invalidate(viewBound);
			if (ShowFPS) Invalidate(new Rectangle(0, 0, 100, 100));
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

			viewMatrix.Translate(-e.X, -e.Y, MatrixOrder.Append);
			if (e.Delta > 0) viewMatrix.Scale(1.1f, 1.1f, MatrixOrder.Append);
			else viewMatrix.Scale(1 / 1.1f, 1 / 1.1f, MatrixOrder.Append);
			viewMatrix.Translate(e.X, e.Y, MatrixOrder.Append);

			matrixUpdated = true;

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				interaction = Interaction.DragOrSelect;
				mouseDragStart = e.Location;
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (interaction == Interaction.DragOrSelect)
			{
				var selectedOperations = new List<GOperation>();
				foreach (var item in items)
				{
					if (item.Hovered && !item.Selected)
					{
						item.Selected = true;
						selectedOperations.Add(item.Operation);
						Invalidate(item);
					}
					else if (item.Selected && (ModifierKeys & Keys.Control) != Keys.Control)
					{
						item.Selected = false;
						Invalidate(item);
					}
				}
				viewState.SetSelection(selectedOperations);
			}
			if (interaction == Interaction.DragSelect)
			{
				var selectedOperations = new List<GOperation>();
				var append = (ModifierKeys & Keys.Control) == Keys.Control;
				var skipRapid = (ModifierKeys & Keys.Alt) == Keys.Alt;
				if (append) selectedOperations.AddRange(viewState.SelectedOperations);
				foreach (var item in items)
				{
					if (skipRapid && item.Operation.Line.Instruction == GInstruction.G0) continue;
					if (item.Hovered)
					{
						item.Hovered = false;
						item.Selected = true;
						selectedOperations.Add(item.Operation);
						Invalidate(item);
					}
					else
					{
						if (!append && item.Selected)
						{
							item.Selected = false;
							Invalidate(item);
						}
					}
				}
				viewState.SetSelection(selectedOperations);
			}

			interaction = Interaction.None;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (interaction == Interaction.DragOrSelect)
			{
				if (Math.Abs(e.X - mouseDragStart.X) > 3 || Math.Abs(e.Y - mouseDragStart.Y) > 3)
				{
					if ((ModifierKeys & Keys.Shift) == Keys.Shift)
					{
						interaction = Interaction.DragSelect;
					}
					else
					{
						interaction = Interaction.Drag;
					}
				}
			}

			if (interaction == Interaction.Drag)
			{
				viewMatrix.Translate(e.X - mouseDragStart.X, e.Y - mouseDragStart.Y, MatrixOrder.Append);
				matrixUpdated = true;

				mouseDragStart = e.Location;
				Invalidate();
			}

			if (interaction == Interaction.DragSelect)
			{
				var absSelectionRectangle = ViewToAbs(new Rectangle(mouseDragStart.X, mouseDragStart.Y, e.Location.X - mouseDragStart.X, e.Location.Y - mouseDragStart.Y));
				var skipRapid = (ModifierKeys & Keys.Alt) == Keys.Alt;
				foreach (var item in items)
				{
					if (skipRapid && item.Operation.Line.Instruction == GInstruction.G0) continue;
					var hovered = item.AbsBoundingBox.IntersectsWith(absSelectionRectangle);
					if (hovered == item.Hovered) continue;
					item.Hovered = hovered;
					Invalidate(item);
				}
			}

			if (interaction == Interaction.None)
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
