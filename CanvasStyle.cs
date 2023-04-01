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
		public Pen HoveredIdlePen { get; private set; } = default!;
		public Pen HoveredActivePen { get; private set; } = default!;
		public Pen SelectedPen { get; private set; } = default!;
		public Pen MinorGridPen { get; private set; } = default!;
		public Pen MajorGridPen { get; private set; } = default!;
		public Pen OriginGridPen { get; private set; } = default!;

		public CanvasStyle()
		{
			using var matrix = new Matrix();
			ViewMatrixChanged(matrix);
		}

		public virtual void ViewMatrixChanged(Matrix viewMatrix)
		{
			var probe = new[] { new Point(1, 0) };
			viewMatrix.VectorTransformPoints(probe);
			var len = Math.Max(probe[0].X, 0.0001f);
			IdlePen = new Pen(Color.LightBlue, 1 / len);
			ActivePen = new Pen(Color.Blue, 1 / len);
			HoveredIdlePen = new Pen(Color.LightBlue, 3 / len);
			HoveredActivePen = new Pen(Color.Blue, 3 / len);
			SelectedPen = new Pen(Color.DarkGreen, 3 / len);

			MinorGridPen = new Pen(Color.DarkGray, 1 / len) { DashStyle = DashStyle.Dot, DashPattern = new[] { 1f / len, 1f / len } };
			MajorGridPen = new Pen(Color.Black, 1 / len) { DashStyle = DashStyle.Dot, DashPattern = new[] { 1f / len, 1f / len } };
			OriginGridPen = new Pen(Color.Black, 1 / len);

			var arrowPath = new GraphicsPath();
			arrowPath.AddLines(new[] { new PointF(0f, 0f), new PointF(1f, -2f), new PointF(-1f, -2f), new PointF(0f, 0f) });
			HoveredActivePen.CustomEndCap = new CustomLineCap(null, arrowPath, LineCap.ArrowAnchor);

			var capPath = new GraphicsPath();
			capPath.AddLine(2f, 0f, -2f, 0f);
			HoveredActivePen.CustomStartCap = new CustomLineCap(null, capPath, LineCap.ArrowAnchor);

			SelectedPen.CustomEndCap = HoveredActivePen.CustomEndCap;
			SelectedPen.CustomStartCap = HoveredActivePen.CustomStartCap;
		}

		public virtual void Dispose()
		{
			IdlePen.Dispose();
			ActivePen.Dispose();
			HoveredIdlePen.Dispose();
			HoveredActivePen.Dispose();
			SelectedPen.Dispose();
			MinorGridPen.Dispose();
			MajorGridPen.Dispose();
			OriginGridPen.Dispose();
		}
	}
}

