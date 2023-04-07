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
		public bool Selected { get; set; }
		public bool Hovered { get; set; }

		public CanvasItem(GOperation operation)
		{
			Operation = operation;
			OperationChanged();
		}

		public virtual void OperationChanged()
		{
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

		public static CanvasItem? FromOperation(GOperation operation)
		{
			if (operation.Line.IsLine) return new CanvasItemLine(operation);
			else if (operation.Line.IsArc) return new CanvasItemArc(operation);
			else if (operation.Line.Instruction == GInstruction.Directive && operation.Line.Comment == "background") return new CanvasItemBackground(operation);
			else return null;
		}
	}
}
