using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.Xml;
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
			if (angle < 0) angle += 360;

			var absMinX = Operation.AbsXStart;
			var absMaxX = Operation.AbsXStart;
			var absMinY = Operation.AbsYStart;
			var absMaxY = Operation.AbsYStart;

			if (Operation.AbsXEnd < absMinX) absMinX = Operation.AbsXEnd;
			if (Operation.AbsXEnd > absMaxX) absMaxX = Operation.AbsXEnd;
			if (Operation.AbsYEnd < absMinY) absMinY = Operation.AbsYEnd;
			if (Operation.AbsYEnd > absMaxY) absMaxY = Operation.AbsYEnd;

			var minAngle = sweep > 0 ? angle : angle + sweep;
			var maxAngle = sweep > 0 ? angle + sweep : angle;
			if (maxAngle >= 360) { maxAngle -= 360; minAngle -= 360; }

			if (minAngle <= 0 && maxAngle > 0 && Operation.AbsI + radius > absMaxX) absMaxX = Operation.AbsI + radius;
			if (minAngle <= 90 && maxAngle > 90 && Operation.AbsJ + radius > absMaxY) absMaxY = Operation.AbsJ + radius;
			if (minAngle <= 180 && maxAngle > 180 && Operation.AbsI - radius < absMinX) absMinX = Operation.AbsI - radius;
			if (minAngle <= 270 && maxAngle > 270 && Operation.AbsJ - radius < absMinY) absMinY = Operation.AbsJ - radius;

			AbsBoundingBox = new RectangleF(absMinX, absMinY, absMaxX - absMinX, absMaxY - absMinY);
		}

		public override void ViewMatrixChanged(Matrix viewMatrix)
		{
			base.ViewMatrixChanged(viewMatrix);
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			var pen = Selected ? style.SelectedPen
				: Hovered ? style.HoveredActivePen
				: style.ActivePen;

			g.DrawArc(pen, Operation.AbsI - radius, Operation.AbsJ - radius, 2 * radius, 2 * radius, angle, sweep);
		}

		public override float Distance(PointF p)
		{
			var dx = p.X - Operation.AbsI;
			var dy = p.Y - Operation.AbsJ;
			var pointAngle = 90 - (float)(Math.Atan2(dx, dy) * 180 / Math.PI);
			if (pointAngle < 0) pointAngle += 360;

			var onArc = false;

			if (sweep > 0)
			{
				if (angle + sweep < 360)
				{
					onArc = pointAngle >= angle && pointAngle <= angle + sweep;
				}
				else
				{
					onArc = pointAngle >= angle || pointAngle < angle + sweep - 360;
				}
			}
			else
			{
				if (angle + sweep > 0)
				{
					onArc = pointAngle <= angle && pointAngle >= angle + sweep;
				}
				else
				{
					onArc = pointAngle <= angle || pointAngle >= angle + sweep + 360;
				}
			}

			if (onArc)
			{
				return (float)Math.Abs(Math.Sqrt(dx * dx + dy * dy) - radius);
			}
			else
			{
				var d1 = (float)Math.Sqrt((p.X - Operation.AbsXStart) * (p.X - Operation.AbsXStart) + (p.Y - Operation.AbsYStart) * (p.Y - Operation.AbsYStart));
				var d2 = (float)Math.Sqrt((p.X - Operation.AbsXEnd) * (p.X - Operation.AbsXEnd) + (p.Y - Operation.AbsYEnd) * (p.Y - Operation.AbsYEnd));
				return Math.Min(d1, d2);
			}
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
