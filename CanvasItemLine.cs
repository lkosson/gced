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
			if (Operation.Line.Instruction == GInstruction.G0) g.DrawLine(style.IdlePen, Operation.AbsXStart, Operation.AbsYStart, Operation.AbsXEnd, Operation.AbsYEnd);
			else g.DrawLine(style.ActivePen, Operation.AbsXStart, Operation.AbsYStart, Operation.AbsXEnd, Operation.AbsYEnd);
		}

		public override float Distance(PointF p)
		{
			return base.Distance(p);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
