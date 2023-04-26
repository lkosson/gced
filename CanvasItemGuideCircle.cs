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
	class CanvasItemGuideCircle : CanvasItem
	{
		public CanvasItemGuideCircle(GOperation operation)
			 : base(operation)
		{
		}

		public override void OperationChanged()
		{
			base.OperationChanged();
			var axes = Operation.AbsEnd - Operation.AbsStart;
			AbsBoundingBox = new RectangleF(Operation.AbsXStart - axes.X, Operation.AbsYStart - axes.Y, axes.X * 2, axes.Y * 2);
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			var pen = Selected ? style.SelectedGuidePen
				: Hovered ? style.HoveredGuidePen
				: style.GuidePen;

			var axes = Operation.AbsEnd - Operation.AbsStart;

			g.DrawEllipse(pen, Operation.AbsXStart - axes.X, Operation.AbsYStart - axes.Y, axes.X * 2, axes.Y * 2);
		}

		public override float Distance(Vector2 p)
		{
			if (AbsBoundingBox == default) return base.Distance(p);
			var distance = Geometry.LineLength(new Vector2((float)Operation.Line.X.GetValueOrDefault(), (float)Operation.Line.Y.GetValueOrDefault()), p);
			var radius = (float)Operation.Line.I.GetValueOrDefault();
			return (float)Math.Abs(distance - radius);
		}
	}
}
