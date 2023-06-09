﻿using System;
using System.Numerics;

namespace GCEd
{
	static class Geometry
	{
		public static Vector2 Normal(Vector2 vector)
		{
			return new Vector2(-vector.Y, vector.X);
		}

		public static float LineLength(Vector2 lineOffset)
		{
			return lineOffset.Length();
		}

		public static float LineLength(Vector2 lineStart, Vector2 lineEnd)
		{
			return (lineEnd - lineStart).Length();
		}

		public static double LineAngle(Vector2 lineStart, Vector2 lineEnd)
		{
			return (float)Math.Atan2(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
		}

		public static float ArcLength(Vector2 arcStart, Vector2 arcEnd, Vector2 arcCenter, bool clockwise)
		{
			var (_, sweep) = AngleAndSweepForArc(arcStart, arcEnd, arcCenter, clockwise);
			var radius = LineLength(arcStart, arcCenter);
			return (float)Math.Abs(2 * Math.PI * radius * sweep / 360f);
		}

		public static float DistanceBetweenLineAndPoint(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			return (float)(Math.Abs((lineEnd.X - lineStart.X) * (lineStart.Y - point.Y) - (lineStart.X - point.X) * (lineEnd.Y - lineStart.Y))
				/ LineLength(lineStart, lineEnd));
		}

		public static Vector2 CircleCenterFromThreePoints(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			// https://math.stackexchange.com/questions/213658/
			double m11 = p1.X * p1.X + p1.Y * p1.Y;
			double m12 = p1.X;
			double m13 = p1.Y;
			double m14 = 1f;
			double m21 = p2.X * p2.X + p2.Y * p2.Y;
			double m22 = p2.X;
			double m23 = p2.Y;
			double m24 = 1f;
			double m31 = p3.X * p3.X + p3.Y * p3.Y;
			double m32 = p3.X;
			double m33 = p3.Y;
			double m34 = 1f;

			var d11 = m12 * m23 * m34 + m13 * m24 * m32 + m14 * m22 * m33 - m14 * m23 * m32 - m12 * m24 * m33 - m13 * m22 * m34;
			var d12 = m11 * m23 * m34 + m13 * m24 * m31 + m14 * m21 * m33 - m14 * m23 * m31 - m11 * m24 * m33 - m13 * m21 * m34;
			var d13 = m11 * m22 * m34 + m12 * m24 * m31 + m14 * m21 * m32 - m14 * m22 * m31 - m11 * m24 * m32 - m12 * m21 * m34;

			if (d11 == 0) return new Vector2(Single.PositiveInfinity, Single.PositiveInfinity);
			var x0 = 0.5f * d12 / d11;
			var y0 = -0.5f * d13 / d11;

			return new Vector2((float)x0, (float)y0);
		}

		public static Vector2 ClosestPointOnNormal(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			var distanceToLine = DistanceBetweenLineAndPoint(lineStart, lineEnd, point);
			var midPoint = new Vector2((lineStart.X + lineEnd.X) / 2, (lineStart.Y + lineEnd.Y) / 2);
			var normal1 = new Vector2(lineEnd.Y - lineStart.Y, lineStart.X - lineEnd.X);
			var normal2 = new Vector2(lineStart.Y - lineEnd.Y, lineEnd.X - lineStart.X);
			var lineLength = LineLength(lineStart, lineEnd);
			var closest1 = new Vector2(midPoint.X + normal1.X * distanceToLine / lineLength, midPoint.Y + normal1.Y * distanceToLine / lineLength);
			var closest2 = new Vector2(midPoint.X + normal2.X * distanceToLine / lineLength, midPoint.Y + normal2.Y * distanceToLine / lineLength);
			var distance1 = LineLength(closest1, point);
			var distance2 = LineLength(closest2, point);
			if (distance1 < distance2) return closest1;
			return closest2;
		}

		public static Vector2 ConstrainToCircle(Vector2 center, Vector2 circlePoint, Vector2 pointToConstrain)
		{
			var directionX = center.X - pointToConstrain.X;
			var directionY = center.Y - pointToConstrain.Y;
			var radius = (float)Math.Sqrt((center.X - circlePoint.X) * (center.X - circlePoint.X) + (center.Y - circlePoint.Y) * (center.Y - circlePoint.Y));
			var distance = (float)Math.Sqrt(directionX * directionX + directionY * directionY);
			return new Vector2(center.X - directionX * radius / distance, center.Y - directionY * radius / distance);
		}

		public static Vector2 SimilarTriangle(Vector2 originalA, Vector2 originalB, Vector2 originalC, Vector2 newA, Vector2 newB)
		{
			var originalABLength = LineLength(originalA, originalB);
			var originalABAngle = LineAngle(originalA, originalB);
			var originalACLength = LineLength(originalA, originalC);
			var originalACAngle = LineAngle(originalA, originalC);
			var newABLength = LineLength(newA, newB);
			var newABAngle = LineAngle(newA, newB);
			var newACLength = originalACLength * newABLength / originalABLength;
			var newACDeltaX = (float)(newACLength * Math.Sin(originalACAngle + newABAngle - originalABAngle));
			var newACDeltaY = (float)(newACLength * Math.Cos(originalACAngle + newABAngle - originalABAngle));
			return new Vector2(newA.X + newACDeltaX, newA.Y + newACDeltaY);
		}

		public static (Vector2 startTangent, Vector2 endTangent) TangentsForOperation(GOperation operation)
		{
			if (operation.Line.IsLine)
			{
				var direction = Vector2.Normalize(operation.AbsEnd - operation.AbsStart);
				return (direction, direction);
			}
			else if (operation.Line.Instruction == GInstruction.G2)
			{
				var startRadial = Vector2.Normalize(operation.AbsOffset - operation.AbsStart);
				var endRadial = Vector2.Normalize(operation.AbsOffset - operation.AbsEnd);
				return (Normal(startRadial), Normal(endRadial));
			}
			else if (operation.Line.Instruction == GInstruction.G3)
			{
				var startRadial = Vector2.Normalize(operation.AbsStart - operation.AbsOffset);
				var endRadial = Vector2.Normalize(operation.AbsEnd - operation.AbsOffset);
				return (Normal(startRadial), Normal(endRadial));
			}
			else return (Vector2.Zero, Vector2.Zero);
		}

		public static (Vector2 startTangent, Vector2 endTangent) TangentsForOperation(GOperation operation, GOperation? prevOperation, GOperation? nextOperation)
		{
			var tangents = TangentsForOperation(operation);
			var prevTangent = prevOperation == null ? tangents.startTangent : TangentsForOperation(prevOperation).endTangent;
			var nextTangent = nextOperation == null ? tangents.endTangent : TangentsForOperation(nextOperation).startTangent;
			var startTangent = Vector2.Normalize((prevTangent + tangents.startTangent) / 2);
			var endTangent = Vector2.Normalize((tangents.endTangent + nextTangent) / 2);
			return (startTangent, endTangent);
		}

		public static (float angle, float sweep) AngleAndSweepForArc(Vector2 arcStart, Vector2 arcEnd, Vector2 arcCenter, bool clockwise)
		{
			var startAngle = (float)(Geometry.LineAngle(arcCenter, arcStart) * 180 / Math.PI);
			var endAngle = (float)(Geometry.LineAngle(arcCenter, arcEnd) * 180 / Math.PI);
			if (clockwise && endAngle < startAngle) endAngle += 360;
			if (!clockwise && startAngle < endAngle) startAngle += 360;
			startAngle = 90 - startAngle;
			endAngle = 90 - endAngle;
			var angle = startAngle;
			var sweep = endAngle - startAngle;
			if (angle < 0) angle += 360;
			return (angle, sweep);
		}
	}
}
