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
			AbsBoundingBox = new RectangleF((float)(Operation.Line.X.GetValueOrDefault() - Operation.Line.I.GetValueOrDefault()), (float)(Operation.Line.Y.GetValueOrDefault() - Operation.Line.J.GetValueOrDefault()),
				(float)(2 * Operation.Line.I.GetValueOrDefault()), (float)(2 * Operation.Line.J.GetValueOrDefault()));
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			var pen = Selected ? style.SelectedGuidePen
				: Hovered ? style.HoveredGuidePen
				: style.GuidePen;

			g.DrawEllipse(pen, (float)(Operation.Line.X.GetValueOrDefault() - Operation.Line.I.GetValueOrDefault()), (float)(Operation.Line.Y.GetValueOrDefault() - Operation.Line.J.GetValueOrDefault()),
				(float)(2 * Operation.Line.I.GetValueOrDefault()), (float)(2 * Operation.Line.J.GetValueOrDefault()));
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
