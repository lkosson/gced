﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	static class Geometry
	{
		public static float LineLength(PointF lineStart, PointF lineEnd)
		{
			return (float)Math.Sqrt((lineEnd.X - lineStart.X) * (lineEnd.X - lineStart.X) + (lineEnd.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y));
		}

		public static float DistanceBetweenLineAndPoint(PointF lineStart, PointF lineEnd, PointF point)
		{
			return (float)(Math.Abs((lineEnd.X - lineStart.X) * (lineStart.Y - point.Y) - (lineStart.X - point.X) * (lineEnd.Y - lineStart.Y))
				/ LineLength(lineStart, lineEnd));
		}

		public static PointF CircleCenterFromThreePoints(PointF p1, PointF p2, PointF p3)
		{
			// https://math.stackexchange.com/questions/213658/
			var m11 = p1.X * p1.X + p1.Y * p1.Y;
			var m12 = p1.X;
			var m13 = p1.Y;
			var m14 = 1f;
			var m21 = p2.X * p2.X + p2.Y * p2.Y;
			var m22 = p2.X;
			var m23 = p2.Y;
			var m24 = 1f;
			var m31 = p3.X * p3.X + p3.Y * p3.Y;
			var m32 = p3.X;
			var m33 = p3.Y;
			var m34 = 1f;

			var d11 = m12 * m23 * m34 + m13 * m24 * m32 + m14 * m22 * m33 - m14 * m23 * m32 - m12 * m24 * m33 - m13 * m22 * m34;
			var d12 = m11 * m23 * m34 + m13 * m24 * m31 + m14 * m21 * m33 - m14 * m23 * m31 - m11 * m24 * m33 - m13 * m21 * m34;
			var d13 = m11 * m22 * m34 + m12 * m24 * m31 + m14 * m21 * m32 - m14 * m22 * m31 - m11 * m24 * m32 - m12 * m21 * m34;

			if (d11 == 0) return new PointF(Single.PositiveInfinity, Single.PositiveInfinity);
			var x0 = 0.5f * d12 / d11;
			var y0 = -0.5f * d13 / d11;

			return new PointF(x0, y0);
		}

		public static PointF ClosestPointOnNormal(PointF lineStart, PointF lineEnd, PointF point)
		{
			var distanceToLine = DistanceBetweenLineAndPoint(lineStart, lineEnd, point);
			var midPoint = new PointF((lineStart.X + lineEnd.X) / 2, (lineStart.Y + lineEnd.Y) / 2);
			var normal1 = new PointF(lineEnd.Y - lineStart.Y, lineStart.X - lineEnd.X);
			var normal2 = new PointF(lineStart.Y - lineEnd.Y, lineEnd.X - lineStart.X);
			var lineLength = LineLength(lineStart, lineEnd);
			var closest1 = new PointF(midPoint.X + normal1.X * distanceToLine / lineLength, midPoint.Y + normal1.Y * distanceToLine / lineLength);
			var closest2 = new PointF(midPoint.X + normal2.X * distanceToLine / lineLength, midPoint.Y + normal2.Y * distanceToLine / lineLength);
			var distance1 = LineLength(closest1, point);
			var distance2 = LineLength(closest2, point);
			if (distance1 < distance2) return closest1;
			return closest2;
		}
	}
}