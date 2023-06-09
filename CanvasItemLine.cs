﻿using System;
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
	class CanvasItemLine : CanvasItem
	{
		public CanvasItemLine(GOperation operation)
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
			var pen = Selected ? (Operation.Line.Instruction == GInstruction.G0 ? style.SelectedIdlePen : style.SelectedActivePen)
				: Hovered ? (Operation.Line.Instruction == GInstruction.G0 ? style.HoveredIdlePen : style.HoveredActivePen)
				: Operation.Line.Instruction == GInstruction.G0 ? style.IdlePen
				: style.ActivePen;
			g.DrawLine(pen, Operation.AbsXStart, Operation.AbsYStart, Operation.AbsXEnd, Operation.AbsYEnd);
		}

		public override float Distance(Vector2 p)
		{
			if (AbsBoundingBox == default) return base.Distance(p);
			return Geometry.DistanceBetweenLineAndPoint(Operation.AbsStart, Operation.AbsEnd, p);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
