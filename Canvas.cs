using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

		public bool ShowFPS { get; set; } = false;
		public bool ShowCursorCoords { get; set; } = true;
		public bool ShowItemCoords { get; set; } = true;
		public bool ShowMinorGrid { get; set; } = true;
		public bool ShowMajorGrid { get; set; } = true;
		public bool ShowOriginGrid { get; set; } = true;
		public bool SnapToGrid { get; set; } = true;
		public bool SnapToItems { get; set; } = true;
		public bool SnapToAxes { get; set; } = true;

		private ViewState viewState;
		private Matrix viewMatrix;
		private Matrix inverseViewMatrix;
		private Point mouseStart;
		private Vector2 translateDistance;
		private bool matrixUpdated;
		private CanvasStyle style;
		private IEnumerable<CanvasItem> items;
		private Interaction interaction;
		private bool panningToSelectionSuspended;

		private enum Interaction { None, Select, Pan, EndMove, OffsetMove, PanDuringEndMove, PanDuringOffsetMove, Translate }

		private float GridMinorStep => (float)Math.Pow(10, 1 + Math.Floor(Math.Log10(ViewToAbs(10))));
		private float GridMajorStep => GridMinorStep * 10;
		private float HintSnapDistance => 2;

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
				var item = CanvasItem.FromOperation(operation);
				if (item == null) continue;
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

		public void PanZoomViewToFit()
		{
			var absX1 = Single.MaxValue;
			var absY1 = Single.MaxValue;
			var absX2 = Single.MinValue;
			var absY2 = Single.MinValue;

			foreach (var item in items)
			{
				var bounding = item.AbsBoundingBox;
				bounding.Inflate(20, 20);
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

		public void PanViewToSelection()
		{
			if (panningToSelectionSuspended) return;
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
			if (ClientRectangle.IntersectsWith(viewSelectedBounds)) return;

			viewMatrix.Translate(-viewSelectedBounds.X + Width / 2 - viewSelectedBounds.Width / 2, -viewSelectedBounds.Y + Height / 2 - viewSelectedBounds.Height / 2, MatrixOrder.Append);

			matrixUpdated = true;
			Invalidate();
		}

		public void SuspendPanningToSelection()
		{
			panningToSelectionSuspended = true;
		}

		public void ResumePanningToSelection()
		{
			panningToSelectionSuspended = false;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var sw = Stopwatch.StartNew();

			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
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
			var viewSelectionRectangle = new Rectangle(mouseStart.X, mouseStart.Y, mousePosition.X - mouseStart.X, mousePosition.Y - mouseStart.Y);
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

		private Vector2 ViewToAbs(Point point)
		{
			var probe = new[] { new PointF(point.X, point.Y) };
			inverseViewMatrix.TransformPoints(probe);
			return (Vector2)probe[0];
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

		private Point AbsToView(Vector2 point)
		{
			var probe = new[] { (PointF)point };
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

		private Vector2 Snap(Vector2 point, params Vector2[] hints)
		{
			if ((ModifierKeys & Keys.Shift) == Keys.Shift) return point;

			var bestXHintDistance = HintSnapDistance;
			var bestYHintDistance = HintSnapDistance;
			var bestXHint = Single.NaN;
			var bestYHint = Single.NaN;

			if (SnapToAxes)
			{
				foreach (var hint in hints)
				{
					var hintXDistance = Math.Abs(hint.X - point.X);
					var hintYDistance = Math.Abs(hint.Y - point.Y);
					if (hintXDistance < bestXHintDistance)
					{
						bestXHint = hint.X;
						bestXHintDistance = hintXDistance;
					}
					if (hintYDistance < bestYHintDistance)
					{
						bestYHint = hint.Y;
						bestYHintDistance = hintYDistance;
					}
				}
			}

			if (SnapToItems)
			{
				foreach (var item in items)
				{
					if (item.Selected) continue;
					var distance = Geometry.LineLength(item.Operation.AbsEnd, point);
					if (distance > bestXHintDistance && distance > bestYHintDistance) continue;
					bestXHint = item.Operation.AbsXEnd;
					bestYHint = item.Operation.AbsYEnd;
					bestXHintDistance = distance;
					bestYHintDistance = distance;
				}
			}

			if (!Single.IsNaN(bestXHint)) point = new Vector2(bestXHint, point.Y);
			if (!Single.IsNaN(bestYHint)) point = new Vector2(point.X, bestYHint);

			if (!SnapToGrid) return point;
			var step = GridMinorStep;
			return new Vector2((float)Math.Floor((point.X + step / 2) / step) * step, (float)Math.Floor((point.Y + step / 2) / step) * step);
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

		private void StartMousePan()
		{
			if (interaction == Interaction.None) interaction = Interaction.Pan;
			else if (interaction == Interaction.EndMove) interaction = Interaction.PanDuringEndMove;
			else if (interaction == Interaction.OffsetMove) interaction = Interaction.PanDuringOffsetMove;
			mouseStart = PointToClient(MousePosition);
		}

		private void UpdateMousePan(Point mouseLocation)
		{
			viewMatrix.Translate(mouseLocation.X - mouseStart.X, mouseLocation.Y - mouseStart.Y, MatrixOrder.Append);
			matrixUpdated = true;

			Cursor = Cursors.SizeAll;
			mouseStart = mouseLocation;
			Invalidate();
		}

		private void FinishMousePan()
		{
			if (interaction == Interaction.Pan) interaction = Interaction.None;
			if (interaction == Interaction.PanDuringEndMove) interaction = Interaction.EndMove;
			if (interaction == Interaction.PanDuringOffsetMove) interaction = Interaction.OffsetMove;
		}

		private void StartMouseSelect()
		{
			interaction = Interaction.Select;
			mouseStart = PointToClient(MousePosition);
		}

		private void UpdateMouseSelect(Point mouseLocation)
		{
			var viewSelectionRectangle = new Rectangle(mouseStart.X, mouseStart.Y, mouseLocation.X - mouseStart.X, mouseLocation.Y - mouseStart.Y);
			var absSelectionRectangle = ViewToAbs(viewSelectionRectangle);
			var skipRapid = (ModifierKeys & Keys.Alt) == Keys.Alt;
			foreach (var item in items)
			{
				var hovered = item.AbsBoundingBox.IntersectsWith(absSelectionRectangle);
				if (skipRapid && item.Operation.Line.Instruction == GInstruction.G0) hovered = false;
				if (item.Operation.Line.Instruction == GInstruction.Directive) hovered = false;
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

		public void StartMouseEndMove()
		{
			if (!viewState.SelectedOperations.Any() || viewState.LastSelectedOperation == null) return;
			SaveOriginalValuesForSelection();
			interaction = Interaction.EndMove;
			UpdateMouseEndMove(PointToClient(MousePosition));
		}

		private void UpdateMouseEndMove(Point mouseLocation)
		{
			var changeArcSweepAngle = (ModifierKeys & Keys.Alt) == Keys.Alt;
			var hints = new List<Vector2>();
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				hints.Add(item.Operation.AbsStart);
			}
			var absPos = Snap(ViewToAbs(mouseLocation), hints.ToArray());
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				if (item.Operation.Line.IsArc)
				{
					if (changeArcSweepAngle)
					{
						if (item.Operation.Line.I.HasValue && item.Operation.Line.J.HasValue) absPos = Geometry.ConstrainToCircle(new Vector2(item.Operation.AbsI, item.Operation.AbsJ), item.Operation.AbsStart, absPos);
						item.Operation.AbsXEnd = absPos.X;
						item.Operation.AbsYEnd = absPos.Y; 
					}
					else
					{
						item.Operation.AbsXEnd = absPos.X;
						item.Operation.AbsYEnd = absPos.Y;
						var newAbsOffset = Geometry.SimilarTriangle(item.Operation.AbsStart, item.Operation.OriginalValues!.AbsEnd, new Vector2(item.Operation.AbsXStart + (float)item.Operation.Line.I.GetValueOrDefault(), item.Operation.AbsYStart + (float)item.Operation.Line.J.GetValueOrDefault()), item.Operation.AbsStart, item.Operation.AbsEnd);
						if (Single.IsFinite(newAbsOffset.X) && Single.IsFinite(newAbsOffset.Y))
						{
							item.Operation.AbsI = newAbsOffset.X;
							item.Operation.AbsJ = newAbsOffset.Y;
						}
					}
				}
				else
				{
					item.Operation.AbsXEnd = absPos.X;
					item.Operation.AbsYEnd = absPos.Y;
				}
				item.OperationChanged();
			}

			Invalidate();
		}

		private void FinishMouseEndMove()
		{
			viewState.SaveUndoState();
			var needsOffset = false;
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				item.Operation.Line.XY = item.Operation.Absolute ? item.Operation.AbsEnd : item.Operation.AbsEnd - item.Operation.AbsStart;
				if (item.Operation.Line.IsArc)
				{
					if (!item.Operation.Line.I.HasValue || !item.Operation.Line.J.HasValue) needsOffset = true;
					else
					{
						item.Operation.Line.IJ = item.Operation.AbsOffset - item.Operation.AbsStart;
					}
				}
				item.OperationChanged();
			}

			viewState.RunProgram();
			if (needsOffset) StartMouseOffsetMove();
			else interaction = Interaction.None;
			Invalidate();
		}

		public void StartMouseOffsetMove()
		{
			if (!viewState.SelectedOperations.Any()) return;
			var newSelection = new List<GOperation>();
			var skipped = false;
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				if (!item.Operation.Line.IsArc)
				{
					skipped = true;
					continue;
				}
				newSelection.Add(item.Operation);
				break;
			}
			if (!newSelection.Any()) return;
			if (skipped) ViewState.SetSelection(newSelection);
			interaction = Interaction.OffsetMove;
		}

		private void UpdateMouseOffsetMove(Point mouseLocation)
		{
			var absPos = ViewToAbs(mouseLocation);
			var byThreePoints = (ModifierKeys & Keys.Alt) != Keys.Alt;
			foreach (var item in items)
			{
				if (!item.Selected) continue;

				var offset =
					byThreePoints ? Geometry.CircleCenterFromThreePoints(item.Operation.AbsStart, item.Operation.AbsEnd, Snap(absPos, item.Operation.AbsStart, item.Operation.AbsEnd))
					: Geometry.ClosestPointOnNormal(item.Operation.AbsStart, item.Operation.AbsEnd, Snap(absPos, item.Operation.AbsStart, item.Operation.AbsEnd));

				if (byThreePoints)
				{
					var side = (item.Operation.AbsXEnd - item.Operation.AbsXStart) * (absPos.Y - item.Operation.AbsYStart)
						- (item.Operation.AbsYEnd - item.Operation.AbsYStart) * (absPos.X - item.Operation.AbsXStart);
					item.Operation.Line.Instruction = side > 0 ? GInstruction.G2 : GInstruction.G3;
				}

				item.Operation.AbsI = offset.X;
				item.Operation.AbsJ = offset.Y;
				item.OperationChanged();
			}

			Invalidate();
		}

		private void FinishMouseOffsetMove()
		{
			viewState.SaveUndoState();
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				if (Single.IsFinite(item.Operation.AbsOffset.X) && Single.IsFinite(item.Operation.AbsOffset.Y))
				{
					item.Operation.Line.IJ = item.Operation.AbsOffset - item.Operation.AbsStart;
				}
				else
				{
					item.Operation.Line.I = null;
					item.Operation.Line.J = null;
					item.Operation.Line.Instruction = GInstruction.G1;
				}
				item.OperationChanged();
			}

			viewState.RunProgram();
			interaction = Interaction.None;
			Invalidate();
		}

		public void StartMouseTranslate()
		{
			SaveOriginalValuesForSelection();
			interaction = Interaction.Translate;
			mouseStart = PointToClient(MousePosition);
			translateDistance = new Vector2();
		}

		private void UpdateMouseTranslate(Point mouseLocation)
		{
			var absMouse = ViewToAbs(mouseLocation);
			var absStart = ViewToAbs(mouseStart);
			var absDeltaX = absMouse.X - absStart.X;
			var absDeltaY = absMouse.Y - absStart.Y;
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				item.Operation.AbsXStart += absDeltaX;
				item.Operation.AbsYStart += absDeltaY;
				item.Operation.AbsXEnd += absDeltaX;
				item.Operation.AbsYEnd += absDeltaY;
				item.Operation.AbsI += absDeltaX;
				item.Operation.AbsJ += absDeltaY;
				item.OperationChanged();
			}

			translateDistance = new Vector2(translateDistance.X + absDeltaX, translateDistance.Y + absDeltaY);
			mouseStart = mouseLocation;
			Invalidate();
		}

		private void FinishMouseTranslate()
		{
			RestoreOriginalValuesForSelection();
			viewState.SaveUndoState();
			viewState.Translate(viewState.SelectedOperations, (decimal)translateDistance.X, (decimal)translateDistance.Y);
			interaction = Interaction.None;
			Invalidate();
		}

		private void SaveOriginalValuesForSelection()
		{
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				item.Operation.SaveOriginalValues();
			}
		}

		private void RestoreOriginalValuesForSelection()
		{
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				item.Operation.RestoreOriginalValues();
			}
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

		private void AbortMove()
		{
			var operationsToDelete = new List<GOperation>();
			foreach (var item in items)
			{
				if (!item.Selected) continue;
				if (!item.Operation.Line.IsVisible) continue;
				if (!item.Operation.Line.IsEmpty) continue;
				operationsToDelete.Add(item.Operation);
			}

			var previousOperation = items.Reverse().Select(item => item.Operation).SkipWhile(operation => !operationsToDelete.Contains(operation)).Skip(1).FirstOrDefault();
			if (operationsToDelete.Any()) viewState.DeleteOperations(operationsToDelete);
			if (previousOperation != null) viewState.SetSelection(new[] { previousOperation });
			viewState.RunProgram();
			interaction = Interaction.None;
			Invalidate();
		}

		public void Abort()
		{
			AbortMove();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && interaction == Interaction.None) StartMouseSelect();
			else if (e.Button == MouseButtons.Middle) StartMousePan();
			else if (e.Button == MouseButtons.Right) viewState.SetSelection(Enumerable.Empty<GOperation>());
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (interaction == Interaction.Select) FinishMouseSelect();
			else if (interaction == Interaction.EndMove) FinishMouseEndMove();
			else if (interaction == Interaction.OffsetMove) FinishMouseOffsetMove();
			else if (interaction == Interaction.Pan) FinishMousePan();
			else if (interaction == Interaction.PanDuringEndMove) FinishMousePan();
			else if (interaction == Interaction.PanDuringOffsetMove) FinishMousePan();
			else if (interaction == Interaction.Translate) FinishMouseTranslate();
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (interaction == Interaction.Pan || interaction == Interaction.PanDuringEndMove || interaction == Interaction.PanDuringOffsetMove) UpdateMousePan(e.Location);
			else if (interaction == Interaction.Select) UpdateMouseSelect(e.Location);
			else if (interaction == Interaction.EndMove) UpdateMouseEndMove(e.Location);
			else if (interaction == Interaction.OffsetMove) UpdateMouseOffsetMove(e.Location);
			else if (interaction == Interaction.Translate) UpdateMouseTranslate(e.Location);
			else if (interaction == Interaction.None) UpdateMouseHover(e.Location);
			if (ShowCursorCoords) Invalidate(new Rectangle(0, Height - 40, 500, 40));
			base.OnMouseMove(e);
		}
	}
}
