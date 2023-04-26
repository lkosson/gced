using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class CanvasItemGuideLine : CanvasItem
	{
		public CanvasItemGuideLine(GOperation operation)
			 : base(operation)
		{
		}

		public override void OperationChanged()
		{
			base.OperationChanged();
			var x1 = Math.Min(Operation.AbsXStart, Operation.AbsXEnd);
			var y1 = Math.Min(Operation.AbsYStart, Operation.AbsYEnd);
			var x2 = Math.Max(Operation.AbsXStart, Operation.AbsXEnd);
			var y2 = Math.Max(Operation.AbsYStart, Operation.AbsYEnd);
			AbsBoundingBox = new RectangleF(x1, y1, x2 - x1, y2 - y1);
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			var pen = Selected ? style.SelectedGuidePen
				: Hovered ? style.HoveredGuidePen
				: style.GuidePen;

			g.DrawLine(pen, Operation.AbsXStart, Operation.AbsYStart, Operation.AbsXEnd, Operation.AbsYEnd);
		}

		public override float Distance(Vector2 p)
		{
			if (AbsBoundingBox == default) return base.Distance(p);
			return Geometry.DistanceBetweenLineAndPoint(Operation.AbsStart, Operation.AbsEnd, p);
		}
	}
}
