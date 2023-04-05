using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
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
					viewState.CanvasFocused -= ViewState_CanvasFocused;
				}
				viewState = value;
				viewState.OperationsChanged += ViewState_OperationsChanged;
				viewState.SelectedOperationsChanged += ViewState_SelectedOperationsChanged;
				viewState.CanvasFocused += ViewState_CanvasFocused;
			}
		}

		public bool ShowFPS { get; set; } = true;
		public bool ShowCursorCoords { get; set; } = true;
		public bool ShowItemCoords { get; set; } = true;
		public bool ShowMinorGrid { get; set; } = true;
		public bool ShowMajorGrid { get; set; } = true;
		public bool ShowOriginGrid { get; set; } = true;

		private ViewState viewState;
		private Matrix viewMatrix;
		private Matrix inverseViewMatrix;
		private Point mouseDragStart;
		private bool matrixUpdated;
		private CanvasStyle style;
		private IEnumerable<CanvasItem> items;
		private Interaction interaction;

		private enum Interaction { None, Select, Drag, EndMove, OffsetMove, DragEndMove, DragOffsetMove }
		private IEnumerable<GOperation> SelectedOperations => items.Where(item => item.Selected).Select(item => item.Operation);

		private float GridMinorStep => (float)Math.Pow(10, 1 + Math.Floor(Math.Log10(ViewToAbs(10))));
		private float GridMajorStep => GridMinorStep * 10;

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

		private void ViewState_CanvasFocused()
		{
			Focus();
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
			e.Graphics.FillRectangle(style.BackgroundBrush, viewClip.Left, viewClip.Top, viewClip.Width, viewClip.Height);

			if (DesignMode) return;

			PropagateMatrixChange();
			var absClipRect = ViewToAbs(viewClip);
			PaintGrid(e.Graphics, absClipRect);
			var (itemCount, visCount) = PaintItems(e.Graphics, absClipRect);
			PaintSelection(e.Graphics);

			sw.Stop();
			var paintTime = (int)sw.ElapsedMilliseconds;

			if (ShowFPS)
			{
				e.Graphics.DrawString($"{paintTime} ms\n{(paintTime == 0 ? 999 : 1000 / paintTime)} fps\n{itemCount} items\n{visCount} visible", Font, style.TextBrush, 0, 0);
			}

			if (ShowCursorCoords || ShowItemCoords)
			{
				var absMouse = ViewToAbs(PointToClient(MousePosition));
				var coords = "";
				if (ShowCursorCoords) coords += $"X={absMouse.X.ToString("0.000", CultureInfo.InvariantCulture)}, Y={absMouse.Y.ToString("0.000", CultureInfo.InvariantCulture)}";
				if (ShowCursorCoords && ShowItemCoords) coords += "\n";
				if (ShowItemCoords && ViewState.LastSelectedOperation != null)
					coords += ViewState.LastSelectedOperation.Line.Instruction
						+ " X=" + ViewState.LastSelectedOperation.AbsXStart.ToString("0.000", CultureInfo.InvariantCulture)
						+ ", Y=" + ViewState.LastSelectedOperation.AbsYStart.ToString("0.000", CultureInfo.InvariantCulture)
						+ " - X=" + ViewState.LastSelectedOperation.AbsXEnd.ToString("0.000", CultureInfo.InvariantCulture)
						+ ", Y=" + ViewState.LastSelectedOperation.AbsYEnd.ToString("0.000", CultureInfo.InvariantCulture);
				var bounds = e.Graphics.MeasureString(coords, Font);
				e.Graphics.DrawString(coords, Font, style.TextBrush, 0, Height - bounds.Height);
			}
		}

		private void PaintGrid(Graphics g, RectangleF absClipRect)
		{
			if (!ShowMajorGrid && !ShowMinorGrid && !ShowOriginGrid) return;

			var gs = g.Save();
			g.SmoothingMode = SmoothingMode.None;
			g.MultiplyTransform(viewMatrix);

			var absStepMinor = GridMinorStep;
			var absStepMajor = GridMajorStep;

			var absArea = ViewToAbs(ClientRectangle);
			var vertGridStart = absArea.Left - absArea.Left % absStepMajor - (absArea.Left < 0 ? absStepMajor : 0);
			var vertGridEnd = absArea.Right - absArea.Right % absStepMajor + (absArea.Right < 0 ? absStepMajor : 0) + absStepMajor;
			var horizGridStart = absArea.Top - absArea.Top % absStepMajor - (absArea.Top < 0 ? absStepMajor : 0);
			var horizGridEnd = absArea.Bottom - absArea.Bottom % absStepMajor + (absArea.Bottom < 0 ? absStepMajor : 0) + absStepMajor;

			if (ShowMinorGrid)
			{
				for (var x = vertGridStart; x < vertGridEnd; x += absStepMinor)
					g.DrawLine(style.MinorGridPen, x, absClipRect.Top, x, absClipRect.Bottom);

				for (var y = horizGridStart; y < horizGridEnd; y += absStepMinor)
					g.DrawLine(style.MinorGridPen, absClipRect.Left, y, absClipRect.Right, y);
			}

			if (ShowMajorGrid)
			{
				for (var x = vertGridStart; x < vertGridEnd; x += absStepMajor)
					g.DrawLine(style.MajorGridPen, x, absClipRect.Top, x, absClipRect.Bottom);

				for (var y = horizGridStart; y < horizGridEnd; y += absStepMajor)
					g.DrawLine(style.MajorGridPen, absClipRect.Left, y, absClipRect.Right, y);
			}

			if (ShowOriginGrid)
			{
				g.DrawLine(style.OriginGridPen, 0, absClipRect.Top, 0, absClipRect.Bottom);
				g.DrawLine(style.OriginGridPen, absClipRect.Left, 0, absClipRect.Right, 0);
			}

			g.Restore(gs);
		}

		private void PaintSelection(Graphics g)
		{
			if (interaction != Interaction.Select) return;

			var mousePosition = PointToClient(MousePosition);
			var viewSelectionRectangle = new Rectangle(mouseDragStart.X, mouseDragStart.Y, mousePosition.X - mouseDragStart.X, mousePosition.Y - mouseDragStart.Y);
			if (viewSelectionRectangle.Width < 0) viewSelectionRectangle = new Rectangle(viewSelectionRectangle.X + viewSelectionRectangle.Width, viewSelectionRectangle.Y, -viewSelectionRectangle.Width, viewSelectionRectangle.Height);
			if (viewSelectionRectangle.Height < 0) viewSelectionRectangle = new Rectangle(viewSelectionRectangle.X, viewSelectionRectangle.Y + viewSelectionRectangle.Height, viewSelectionRectangle.Width, -viewSelectionRectangle.Height);

			g.FillRectangle(style.SelectionBrush, viewSelectionRectangle);
			g.DrawRectangle(style.SelectionPen, viewSelectionRectangle);
		}

		private void PropagateMatrixChange()
		{
			if (!matrixUpdated) return;
			inverseViewMatrix.Reset();
			inverseViewMatrix.Multiply(viewMatrix);
			inverseViewMatrix.Invert();
			style.ViewMatrixChanged(viewMatrix);
			foreach (var item in items) item.ViewMatrixChanged(viewMatrix);
			matrixUpdated = false;
		}

		private (int itemCount, int visCount) PaintItems(Graphics g, RectangleF absClipRect)
		{
			var gs = g.Save();
			g.MultiplyTransform(viewMatrix);

			var pixelSize = ViewToAbs(1);

			var itemCount = 0;
			var visCount = 0;
			foreach (var item in items)
			{
				itemCount++;
				if (item.AbsBoundingBox.Width < pixelSize && item.AbsBoundingBox.Height < pixelSize) continue;
				if (!item.AbsBoundingBox.IntersectsWith(absClipRect)) continue;
				item.Draw(g, style);
				visCount++;
			}
			g.Restore(gs);

			return (itemCount, visCount);
		}

		private void Invalidate(CanvasItem item)
		{
			var absBounds = item.AbsBoundingBox;
			var viewBound = AbsToView(absBounds);
			viewBound.Inflate(10, 10);
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

		private PointF SnapToGrid(PointF origin, PointF point)
		{
			if ((ModifierKeys & Keys.Shift) == Keys.Shift) return point;
			var step = GridMinorStep;
			return new PointF((float)Math.Floor((point.X + step / 2) / step) * step, (float)Math.Floor((point.Y + step / 2) / step) * step);
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

		private void StartMouseDrag()
		{
			if (interaction == Interaction.None) interaction = Interaction.Drag;
			else if (interaction == Interaction.EndMove) interaction = Interaction.DragEndMove;
			else if (interaction == Interaction.OffsetMove) interaction = Interaction.DragOffsetMove;
			mouseDragStart = PointToClient(MousePosition);
		}

		private void UpdateMouseDrag(Point mouseLocation)
		{
			viewMatrix.Translate(mouseLocation.X - mouseDragStart.X, mouseLocation.Y - mouseDragStart.Y, MatrixOrder.Append);
			matrixUpdated = true;

			Cursor = Cursors.SizeAll;
			mouseDragStart = mouseLocation;
			Invalidate();
		}

		private void FinishMouseDrag()
		{
			if (interaction == Interaction.Drag) interaction = Interaction.None;
			if (interaction == Interaction.DragEndMove) interaction = Interaction.EndMove;
			if (interaction == Interaction.DragOffsetMove) interaction = Interaction.OffsetMove;
		}

		private void StartMouseSelect()
		{
			interaction = Interaction.Select;
			mouseDragStart = PointToClient(MousePosition);
		}

		private void UpdateMouseSelect(Point mouseLocation)
		{
			var viewSelectionRectangle = new Rectangle(mouseDragStart.X, mouseDragStart.Y, mouseLocation.X - mouseDragStart.X, mouseLocation.Y - mouseDragStart.Y);
			var absSelectionRectangle = ViewToAbs(viewSelectionRectangle);
			var skipRapid = (ModifierKeys & Keys.Alt) == Keys.Alt;
			foreach (var item in items)
			{
				var hovered = item.AbsBoundingBox.IntersectsWith(absSelectionRectangle);
				if (skipRapid && item.Operation.Line.Instruction == GInstruction.G0) hovered = false;
				if (hovered == item.Hovered) continue;
				item.Hovered = hovered;
			}
			Invalidate();
		}

		private void FinishMouseSelect()
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
				}
				else
				{
					if (!append && item.Selected)
					{
						item.Selected = false;
					}
				}
			}
			viewState.SetSelection(selectedOperations);
			interaction = Interaction.None;
			Invalidate();
		}

		private void StartMouseEndMove()
		{
			interaction = Interaction.EndMove;
		}

		private void UpdateMouseEndMove(Point mouseLocation)
		{
			var absPos = SnapToGrid(default, ViewToAbs(mouseLocation));
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				item.Operation.AbsXEnd = absPos.X;
				item.Operation.AbsYEnd = absPos.Y;
				item.OperationChanged();
			}

			Invalidate();
		}

		private void FinishMouseEndMove()
		{
			var absPos = SnapToGrid(default, ViewToAbs(PointToClient(MousePosition)));
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				item.Operation.Line.X = (decimal)(item.Operation.Absolute ? absPos.X : absPos.X - item.Operation.AbsXStart);
				item.Operation.Line.Y = (decimal)(item.Operation.Absolute ? absPos.Y : absPos.Y - item.Operation.AbsYStart);
				item.OperationChanged();
			}

			viewState.RunProgram();
			interaction = Interaction.None;
			Invalidate();
		}

		private void UpdateMouseHover(Point mouseLocation)
		{
			var absPos = ViewToAbs(mouseLocation);
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

			Cursor = bestItem == null ? Cursors.Arrow : Cursors.Hand;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && interaction == Interaction.None) StartMouseSelect();
			if (e.Button == MouseButtons.Middle) StartMouseDrag();
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (interaction == Interaction.Select) FinishMouseSelect();
			if (interaction == Interaction.EndMove) FinishMouseEndMove();
			if (interaction == Interaction.Drag) FinishMouseDrag();
			if (interaction == Interaction.DragEndMove) FinishMouseDrag();
			if (interaction == Interaction.DragOffsetMove) FinishMouseDrag();
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (interaction == Interaction.Drag || interaction == Interaction.DragEndMove || interaction == Interaction.DragOffsetMove) UpdateMouseDrag(e.Location);
			if (interaction == Interaction.Select) UpdateMouseSelect(e.Location);
			if (interaction == Interaction.EndMove) UpdateMouseEndMove(e.Location);
			if (interaction == Interaction.None) UpdateMouseHover(e.Location);
			if (ShowCursorCoords) Invalidate(new Rectangle(0, Height - 40, 500, 40));
			base.OnMouseMove(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.C)
			{
				ShowCursorCoords = !ShowCursorCoords;
				if (!ShowCursorCoords) ShowItemCoords = !ShowItemCoords;
			}
			else if (e.KeyCode == Keys.H)
			{
				PanZoomViewToFit();
			}
			else if (e.KeyCode == Keys.F)
			{
				ShowFPS = !ShowFPS;
				Invalidate();
			}
			else if (e.KeyCode == Keys.G)
			{
				if (ShowMinorGrid)
				{
					ShowMinorGrid = false;
					ShowMajorGrid = false;
					ShowOriginGrid = false;
				}
				else if (!ShowOriginGrid) ShowOriginGrid = true;
				else if (!ShowMajorGrid) ShowMajorGrid = true;
				else ShowMinorGrid = true;
				Invalidate();
			}
			else if (e.KeyCode == Keys.A && ModifierKeys == Keys.None)
			{
				viewState.SaveUndoState();
				viewState.ConvertToAbsolute(SelectedOperations);
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.R)
			{
				viewState.SaveUndoState();
				viewState.ConvertToRelative(SelectedOperations);
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Delete)
			{
				viewState.SaveUndoState();
				viewState.DeleteOperations(SelectedOperations);
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.A && (ModifierKeys & Keys.Control) == Keys.Control)
			{
				var skipRapid = (ModifierKeys & Keys.Alt) == Keys.Alt;
				var selectedOperations = new List<GOperation>();
				foreach (var item in items)
				{
					if (skipRapid && item.Operation.Line.Instruction == GInstruction.G0) continue;
					item.Selected = true;
					Invalidate(item);
					selectedOperations.Add(item.Operation);
				}
				viewState.SetSelection(selectedOperations);
			}
			else if (e.KeyCode == Keys.Z && ModifierKeys == Keys.Control)
			{
				viewState.Undo();
			}
			else if (e.KeyCode == Keys.I)
			{
				viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift);
			}
			else if (e.KeyCode == Keys.E)
			{
				StartMouseEndMove();
			}
			else if (e.KeyCode == Keys.D0)
			{
				viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G0 });
				StartMouseEndMove();
				UpdateMouseEndMove(PointToClient(MousePosition));
			}
			else if (e.KeyCode == Keys.D1)
			{
				viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G1 });
				StartMouseEndMove();
				UpdateMouseEndMove(PointToClient(MousePosition));
			}
			base.OnKeyDown(e);
		}
	}
}
