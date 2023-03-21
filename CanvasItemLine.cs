using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class CanvasItemLine : CanvasItem
	{
		public CanvasItemLine(GOperation operation)
			 : base(operation)
		{
			var x1 = Math.Min(Operation.AbsXStart, Operation.AbsXEnd);
			var y1 = Math.Min(Operation.AbsYStart, Operation.AbsYEnd);
			var x2 = Math.Max(Operation.AbsXStart, Operation.AbsXEnd);
			var y2 = Math.Max(Operation.AbsYStart, Operation.AbsYEnd);
			AbsBoundingBox = new RectangleF(x1, y1, x2 - x1, y2 - y1);
		}

		public override void ViewMatrixChanged(Matrix viewMatrix)
		{
			base.ViewMatrixChanged(viewMatrix);
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			var pen = Selected ? style.SelectedPen
				: Hovered ? (Operation.Line.Instruction == GInstruction.G0 ? style.HoveredIdlePen : style.HoveredActivePen)
				: Operation.Line.Instruction == GInstruction.G0 ? style.IdlePen
				: style.ActivePen;
			g.DrawLine(pen, Operation.AbsXStart, Operation.AbsYStart, Operation.AbsXEnd, Operation.AbsYEnd);
		}

		public override float Distance(PointF p)
		{
			if (AbsBoundingBox == default) return base.Distance(p);

			return (float)(Math.Abs((Operation.AbsXEnd - Operation.AbsXStart) * (Operation.AbsYStart - p.Y) - (Operation.AbsXStart - p.X) * (Operation.AbsYEnd - Operation.AbsYStart))
				/ Math.Sqrt((Operation.AbsXEnd - Operation.AbsXStart) * (Operation.AbsXEnd - Operation.AbsXStart) + (Operation.AbsYEnd - Operation.AbsYStart) * (Operation.AbsYEnd - Operation.AbsYStart)));
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
