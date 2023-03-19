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
	class CanvasItemArc : CanvasItem
	{
		private float radius;
		private float angle;
		private float sweep;

		public CanvasItemArc(GOperation operation)
			 : base(operation)
		{
			var startDeltaX = Operation.AbsXStart - Operation.AbsI;
			var startDeltaY = Operation.AbsYStart - Operation.AbsJ;
			var endDeltaX = Operation.AbsXEnd - Operation.AbsI;
			var endDeltaY = Operation.AbsYEnd - Operation.AbsJ;
			radius = (float)Math.Sqrt(startDeltaX * startDeltaX + startDeltaY * startDeltaY);
			var startAngle = (float)(Math.Atan2(startDeltaX, startDeltaY) * 180 / Math.PI);
			var endAngle = (float)(Math.Atan2(endDeltaX, endDeltaY) * 180 / Math.PI);
			if (Operation.Line.Instruction == GInstruction.G2 && endAngle < startAngle) endAngle += 360;
			if (Operation.Line.Instruction == GInstruction.G3 && startAngle < endAngle) startAngle += 360;
			startAngle = 90 - startAngle;
			endAngle = 90 - endAngle;
			angle = startAngle;
			sweep = endAngle - startAngle;
			AbsBoundingBox = new RectangleF(Operation.AbsI - radius, Operation.AbsJ - radius, 2 * radius, 2 * radius);
		}

		public override void ViewMatrixChanged(Matrix viewMatrix)
		{
			base.ViewMatrixChanged(viewMatrix);
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			g.DrawArc(style.ActivePen, Operation.AbsI - radius, Operation.AbsJ - radius, 2 * radius, 2 * radius, angle, sweep);
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
