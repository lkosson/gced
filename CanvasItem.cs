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
	abstract class CanvasItem : IDisposable
	{
		public GOperation Operation { get; set; }
		public RectangleF AbsBoundingBox { get; protected set; }

		public CanvasItem(GOperation operation)
		{
			Operation = operation;
		}

		public virtual void ViewMatrixChanged(Matrix viewMatrix)
		{
		}

		public virtual void Draw(Graphics g, CanvasStyle style)
		{
		}

		public virtual float Distance(PointF p)
		{
			return Single.PositiveInfinity;
		}

		public virtual void Dispose()
		{
		}
	}
}
