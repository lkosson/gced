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
		public float PixelSize { get; protected set; }
		public Pen IdlePen { get; protected set; } = default!;
		public Pen ActivePen { get; protected set; } = default!;
		public Pen HoveredIdlePen { get; protected set; } = default!;
		public Pen HoveredActivePen { get; protected set; } = default!;
		public Pen SelectedIdlePen { get; protected set; } = default!;
		public Pen SelectedActivePen { get; protected set; } = default!;
		public Pen MinorGridPen { get; protected set; } = default!;
		public Pen MajorGridPen { get; protected set; } = default!;
		public Pen OriginGridPen { get; protected set; } = default!;
		public Pen SelectionPen { get; protected set; } = default!;
		public Brush TextBrush { get; protected set; } = default!;
		public Brush BackgroundBrush { get; protected set; } = default!;
		public Brush SelectionBrush { get; protected set; } = default!;

		public CanvasStyle()
		{
			using var matrix = new Matrix();
			ViewMatrixChanged(matrix);
		}

		public virtual void ViewMatrixChanged(Matrix viewMatrix)
		{
			var probe = new[] { new Point(100, 0) };
			viewMatrix.VectorTransformPoints(probe);
			var len = Math.Max(probe[0].X / 100f, 0.0001f);
			PixelSize = 1 / len;
			var selectedStrokeColor = Color.FromArgb(0xF5, 0xFA, 0xFF);
			var strokeColor = Color.FromArgb(0xA5, 0xCA, 0xFF);
			IdlePen = new Pen(strokeColor, PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 10f, 10f } };
			ActivePen = new Pen(strokeColor, 3 * PixelSize);
			HoveredIdlePen = new Pen(strokeColor, 4 * PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 2.5f, 2.5f } };
			HoveredActivePen = new Pen(strokeColor, 4 * PixelSize);
			SelectedIdlePen = new Pen(selectedStrokeColor, 4 * PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 2.5f, 2.5f } };
			SelectedActivePen = new Pen(selectedStrokeColor, 4 * PixelSize);

			MinorGridPen = new Pen(Color.FromArgb(0x30, 0x60, 0xA0), PixelSize);
			MajorGridPen = new Pen(Color.FromArgb(0x60, 0xA0, 0xD0), PixelSize);
			OriginGridPen = new Pen(Color.FromArgb(0xE0, 0xF0, 0xFF), 2 * PixelSize);

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
			SelectedIdlePen.CustomEndCap = endCap;
			SelectedIdlePen.CustomStartCap = startCap;
			SelectedActivePen.CustomEndCap = endCap;
			SelectedActivePen.CustomStartCap = startCap;
			SelectionPen = new Pen(Color.FromArgb(0xAF, 0xCF, 0xFF), PixelSize);

			TextBrush = new SolidBrush(Color.FromArgb(0xE5, 0xF5, 0xFF));
			BackgroundBrush = new SolidBrush(Color.FromArgb(0x20, 0x4A, 0x7F));
			SelectionBrush = new SolidBrush(Color.FromArgb(0x40, 0xE0, 0xEA, 0xF5));
		}

		public virtual void Dispose()
		{
			IdlePen.Dispose();
			ActivePen.Dispose();
			HoveredIdlePen.Dispose();
			HoveredActivePen.Dispose();
			SelectedIdlePen.Dispose();
			SelectedActivePen.Dispose();
			MinorGridPen.Dispose();
			MajorGridPen.Dispose();
			OriginGridPen.Dispose();
			SelectionPen.Dispose();
			TextBrush.Dispose();
			BackgroundBrush.Dispose();
			SelectionBrush.Dispose();
		}
	}
}

