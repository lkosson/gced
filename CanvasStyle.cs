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
		public Brush TextBrush { get; private set; } = default!;
		public Brush BackgroundBrush { get; private set; } = default!;

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
			var hoveredStrokeColor = Color.FromArgb(0xE5, 0xF5, 0xFF);
			var selectedStrokeColor = Color.FromArgb(0xF5, 0xFA, 0xFF);
			var strokeColor = Color.FromArgb(0xA5, 0xCA, 0xFF);
			IdlePen = new Pen(strokeColor, 1 / len) { DashStyle = DashStyle.Dash, DashPattern = new[] { 10f, 10f } };
			ActivePen = new Pen(strokeColor, 3 / len);
			HoveredIdlePen = new Pen(hoveredStrokeColor, 2 / len);
			HoveredActivePen = new Pen(hoveredStrokeColor, 4 / len);
			SelectedPen = new Pen(selectedStrokeColor, 4 / len);

			MinorGridPen = new Pen(Color.FromArgb(0x30, 0x60, 0xA0), 1 / len);// { DashStyle = DashStyle.Dot, DashPattern = new[] { 1f / len, 1f / len } };
			MajorGridPen = new Pen(Color.FromArgb(0x60, 0xA0, 0xD0), 1 / len);// { DashStyle = DashStyle.Dot, DashPattern = new[] { 1f / len, 1f / len } };
			OriginGridPen = new Pen(Color.FromArgb(0xE0, 0xF0, 0xFF), 2 / len);

			var arrowPath = new GraphicsPath();
			arrowPath.AddLines(new[] { new PointF(0f, 0f), new PointF(1f, -2f), new PointF(-1f, -2f), new PointF(0f, 0f) });
			var endCap = new CustomLineCap(null, arrowPath, LineCap.ArrowAnchor);

			var capPath = new GraphicsPath();
			capPath.AddLine(2f, 0f, -2f, 0f);
			var startCap = new CustomLineCap(null, capPath, LineCap.ArrowAnchor);

			HoveredActivePen.CustomEndCap = endCap;
			HoveredActivePen.CustomStartCap = startCap;
			HoveredIdlePen.CustomEndCap = endCap;
			HoveredIdlePen.CustomStartCap = startCap;
			SelectedPen.CustomEndCap = endCap;
			SelectedPen.CustomStartCap = startCap;

			TextBrush = new SolidBrush(hoveredStrokeColor);
			BackgroundBrush = new SolidBrush(Color.FromArgb(0x20, 0x4A, 0x7F));
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
			TextBrush.Dispose();
			BackgroundBrush.Dispose();
		}
	}
}

