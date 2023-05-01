using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace GCEd
{
	class CanvasStyle : IDisposable
	{
		public Settings Settings { get; set; }
		public float PixelSize { get; protected set; }
		public Pen IdlePen { get; protected set; } = default!;
		public Pen ActivePen { get; protected set; } = default!;
		public Pen GuidePen { get; protected set; } = default!;
		public Pen HoveredIdlePen { get; protected set; } = default!;
		public Pen HoveredActivePen { get; protected set; } = default!;
		public Pen HoveredGuidePen { get; protected set; } = default!;
		public Pen SelectedIdlePen { get; protected set; } = default!;
		public Pen SelectedActivePen { get; protected set; } = default!;
		public Pen SelectedGuidePen { get; protected set; } = default!;
		public Pen MinorGridPen { get; protected set; } = default!;
		public Pen MajorGridPen { get; protected set; } = default!;
		public Pen OriginGridPen { get; protected set; } = default!;
		public Pen SelectionPen { get; protected set; } = default!;
		public Brush TextBrush { get; protected set; } = default!;
		public Brush BackgroundBrush { get; protected set; } = default!;
		public Brush SelectionBrush { get; protected set; } = default!;

		public CanvasStyle()
		{
			Settings = new Settings();
			using var matrix = new Matrix();
			ViewMatrixChanged(matrix);
		}

		public virtual void ViewMatrixChanged(Matrix viewMatrix)
		{
			var probe = new[] { new PointF(100, 100) };
			var probeLength = Geometry.LineLength((Vector2)probe[0]);
			viewMatrix.TransformVectors(probe);
			var transformedProbeLength = Geometry.LineLength((Vector2)probe[0]);
			var len = Math.Max(transformedProbeLength / probeLength, 0.0001f);
			PixelSize = (float)(1 / len);
			IdlePen = new Pen(Settings.ItemColor, PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 10f, 10f } };
			ActivePen = new Pen(Settings.ItemColor, Settings.ActiveItemThickness * PixelSize);
			GuidePen = new Pen(Settings.GuideColor, Settings.GuideThickness * PixelSize) { DashStyle = DashStyle.Dot };
			HoveredIdlePen = new Pen(Settings.ItemColor, Settings.SelectedItemThickness * PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 2.5f, 2.5f } };
			HoveredActivePen = new Pen(Settings.ItemColor, Settings.SelectedItemThickness * PixelSize);
			HoveredGuidePen = new Pen(Settings.GuideColor, Settings.SelectedItemThickness * PixelSize) { DashStyle = DashStyle.Dot };
			SelectedIdlePen = new Pen(Settings.SelectedItemColor, Settings.SelectedItemThickness * PixelSize) { DashStyle = DashStyle.Dash, DashPattern = new[] { 2.5f, 2.5f } };
			SelectedActivePen = new Pen(Settings.SelectedItemColor, Settings.SelectedItemThickness * PixelSize);
			SelectedGuidePen = new Pen(Settings.SelectedItemColor, Settings.SelectedItemThickness * PixelSize) { DashStyle = DashStyle.Dot };

			MinorGridPen = new Pen(Settings.MinorGridColor, Settings.MinorGridThickness * PixelSize);
			MajorGridPen = new Pen(Settings.MajorGridColor, Settings.MajorGridThickness * PixelSize);
			OriginGridPen = new Pen(Settings.OriginGridColor, Settings.OriginGridThickness * PixelSize);

			var arrowPath = new GraphicsPath();
			arrowPath.AddLines(new[] { new PointF(0f, 0f), new PointF(1f, -2f), new PointF(-1f, -2f), new PointF(0f, 0f) });
			var endCap = new CustomLineCap(null, arrowPath, LineCap.ArrowAnchor);

			var capPath = new GraphicsPath();
			capPath.AddLine(2f, 0f, -2f, 0f);
			var startCap = new CustomLineCap(null, capPath, LineCap.ArrowAnchor);

			if (Settings.ItemEndCaps)
			{
				HoveredActivePen.CustomEndCap = endCap;
				HoveredActivePen.CustomStartCap = startCap;
				HoveredIdlePen.CustomEndCap = endCap;
				HoveredIdlePen.CustomStartCap = startCap;
				SelectedIdlePen.CustomEndCap = endCap;
				SelectedIdlePen.CustomStartCap = startCap;
				SelectedActivePen.CustomEndCap = endCap;
				SelectedActivePen.CustomStartCap = startCap;
			}

			SelectionPen = new Pen(Settings.SelectionBorderColor, PixelSize);
			SelectionBrush = new SolidBrush(Settings.SelectionAreaColor);
			TextBrush = new SolidBrush(Settings.TextColor);
			BackgroundBrush = new SolidBrush(Settings.BackgroundColor);
		}

		public virtual void Dispose()
		{
			IdlePen.Dispose();
			ActivePen.Dispose();
			GuidePen.Dispose();
			HoveredIdlePen.Dispose();
			HoveredActivePen.Dispose();
			HoveredGuidePen.Dispose();
			SelectedIdlePen.Dispose();
			SelectedActivePen.Dispose();
			SelectedGuidePen.Dispose();
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

