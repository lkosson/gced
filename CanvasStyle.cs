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
	class CanvasStyle : IDisposable
	{
		public Pen IdlePen { get; private set; } = default!;
		public Pen ActivePen { get; private set; } = default!;
		public Pen HoveredPen { get; private set; } = default!;
		public Pen SelectedPen { get; private set; } = default!;

		public CanvasStyle()
		{
		}

		public virtual void ViewMatrixChanged(Matrix viewMatrix)
		{
			var probe = new[] { new Point(1, 0) };
			viewMatrix.VectorTransformPoints(probe);
			var len = Math.Max(probe[0].X, 0.0001f);
			IdlePen = new Pen(Color.LightBlue, 1 / len);
			ActivePen = new Pen(Color.Blue, 1 / len);
			HoveredPen = new Pen(Color.DarkGreen, 1 / len);
			SelectedPen = new Pen(Color.DarkGreen, 2 / len);
		}

		public virtual void Dispose()
		{
			IdlePen.Dispose();
			ActivePen.Dispose();
			HoveredPen.Dispose();
			SelectedPen.Dispose();
		}
	}
}
