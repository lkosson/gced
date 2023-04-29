using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GCEd
{
	static class Mutator
	{
		public static void Delete(GProgram program, IEnumerable<GOperation> operations)
		{
			var lnos = program.GetLNOForOperations(operations);
			foreach (var (_, node, _, _, _) in lnos)
			{
				program.Lines.Remove(node);
			}
		}

		public static void AppendLines(GProgram program, GOperation? baseOperation, bool before, IEnumerable<GLine> lines)
		{
			if (program.Lines.Count == 0)
			{
				foreach (var line in lines) program.Lines.AddLast(line);
				return;
			}

			LinkedListNode<GLine>? baseNode = null;
			LinkedListNode<GLine>? marker = null;

			if (baseOperation != null) baseNode = program.Lines.Find(baseOperation.Line);
			if (baseNode == null) baseNode = before ? program.Lines.First : program.Lines.Last;
			if (baseNode == null) baseNode = marker = program.Lines.AddFirst(new GLine());
			else if (before) baseNode = marker = program.Lines.AddBefore(baseNode, new GLine());

			foreach (var line in lines) baseNode = program.Lines.AddAfter(baseNode, line);

			if (marker != null) program.Lines.Remove(marker);
		}

		public static void ConvertToAbsolute(GProgram program, IEnumerable<GOperation> operations)
		{
			var lnos = program.GetLNOForOperations(operations.Where(operation => !operation.Absolute));
			foreach (var (line, node, operation, previousInExtent, nextInExtent) in lnos)
			{
				if (line.Instruction == GInstruction.G91) program.Lines.Remove(node);
				if (previousInExtent == null && node.Previous != null && node.Previous.Value is GLine previousLine)
				{
					if (previousLine.Instruction == GInstruction.G91) program.Lines.Remove(node.Previous!);
					else if (previousLine.Instruction != GInstruction.G90) program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G90 });
				}

				if (nextInExtent == null && node.Next != null && node.Next.Value is GLine nextLine)
				{
					if (nextLine.Instruction == GInstruction.G90) program.Lines.Remove(node.Next!);
					else if (nextLine.Instruction != GInstruction.G91) program.Lines.AddAfter(node, new GLine { Instruction = GInstruction.G91 });
				}

				if (line.IsVisible) line.XY = operation.AbsEnd;
			}
		}

		public static void ConvertToRelative(GProgram program, IEnumerable<GOperation> operations)
		{
			var lnos = program.GetLNOForOperations(operations.Where(operation => operation.Absolute));
			foreach (var (line, node, operation, previousInExtent, nextInExtent) in lnos)
			{
				if (line.Instruction == GInstruction.G90) program.Lines.Remove(node);
				if (previousInExtent == null && node.Previous != null && node.Previous.Value is GLine previousLine)
				{
					if (previousLine.Instruction == GInstruction.G90) program.Lines.Remove(node.Previous!);
					else if (previousLine.Instruction != GInstruction.G91) program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G91 });
				}

				if (nextInExtent == null && node.Next != null && node.Next.Value is GLine nextLine)
				{
					if (nextLine.Instruction == GInstruction.G91) program.Lines.Remove(node.Next!);
					else if (nextLine.Instruction != GInstruction.G90) program.Lines.AddAfter(node, new GLine { Instruction = GInstruction.G90 });
				}

				if (line.IsVisible) line.XY = operation.AbsEnd - operation.AbsStart;
			}
		}

		public static void Translate(GProgram program, IEnumerable<GOperation> operations, Vector2 offset)
		{
			var matrix = Matrix3x2.CreateTranslation(offset);
			Transform(program, operations, matrix);
		}

		public static void Rotate(GProgram program, IEnumerable<GOperation> operations, Vector2 center, float angle)
		{
			var matrix = Matrix3x2.CreateRotation((float)(angle * Math.PI / 180f), center);
			Transform(program, operations, matrix);
		}

		public static void Scale(GProgram program, IEnumerable<GOperation> operations, Vector2 center, float amount)
		{
			var matrix = Matrix3x2.CreateScale(amount, center);
			Transform(program, operations, matrix);
		}

		private static void Transform(GProgram program, IEnumerable<GOperation> operations, Matrix3x2 matrix)
		{
			var lnos = program.GetLNOForOperations(operations);
			foreach (var (line, node, operation, previousInExtent, nextInExtent) in lnos)
			{
				var newStart = Vector2.Transform(operation.AbsStart, matrix);
				var newEnd = Vector2.Transform(operation.AbsEnd, matrix);

				if (operation.Absolute)
				{
					line.XY = newEnd;

					if (previousInExtent == null && operation.AbsStart != newStart)
					{
						if (node.Previous?.Value is GLine previousLine && previousLine.Instruction == GInstruction.G0) previousLine.XY = newStart;
						else if (line.Instruction == GInstruction.G0) line.XY = newEnd;
						else program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G0, XY = newStart });
					}

					if (nextInExtent == null && operation.AbsEnd != newEnd)
					{
						if (node.Next?.Value is GLine nextLine && nextLine.Instruction == GInstruction.G0) { }
						else if (line.Instruction == GInstruction.G0) line.XY = operation.AbsEnd;
						else program.Lines.AddAfter(node, new GLine { Instruction = GInstruction.G0, XY = operation.AbsEnd });
					}
				}
				else
				{
					line.XY = newEnd - newStart;

					if (previousInExtent == null && operation.AbsStart != newStart)
					{
						if (node.Previous?.Value is GLine previousLine && previousLine.Instruction == GInstruction.G0) previousLine.XY += newStart - operation.AbsStart;
						else if (line.Instruction == GInstruction.G0) line.XY = newEnd - operation.AbsStart;
						else program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G0, XY = newStart - operation.AbsStart });
					}

					if (nextInExtent == null && operation.AbsEnd != newEnd)
					{
						if (node.Next?.Value is GLine nextLine && nextLine.Instruction == GInstruction.G0) nextLine.XY -= newEnd - operation.AbsEnd;
						else if (line.Instruction == GInstruction.G0) line.XY = operation.AbsEnd - newStart;
						else program.Lines.AddAfter(node, new GLine { Instruction = GInstruction.G0, XY = operation.AbsEnd - newEnd });
					}
				}

				if (line.IsArc)
				{
					var newOffset = Vector2.Transform(operation.AbsOffset, matrix);
					newOffset -= newStart;
					line.IJ = newOffset;
				}
			}
		}

		public static void ConvertToOutline(GProgram program, IEnumerable<GOperation> operations, float distance, bool leftFirst, bool skipLeft, bool skipRight, bool bothForward)
		{
			var leftOutline = new GProgram();
			var rightOutline = new GProgram();

			var lnos = program.GetLNOForOperations(operations);
			foreach (var (line, node, operation, previousInExtent, nextInExtent) in lnos)
			{
				if (!line.IsVisible) continue;
				if (Geometry.LineLength(line.XY) == 0) continue;

				var currTangents = Geometry.TangentsForOperation(operation);
				var (startTangent, endTangent) = Geometry.TangentsForOperation(operation, previousInExtent, nextInExtent);
				var startNormal = Geometry.Normal(startTangent);
				var endNormal = Geometry.Normal(endTangent);
				var startDistance = distance / Vector2.Dot(currTangents.startTangent, startTangent);
				var endDistance = distance / Vector2.Dot(currTangents.endTangent, endTangent);
				var leftStartOffset = startNormal * startDistance;
				var rightStartOffset = -startNormal * startDistance;
				var leftEndOffset = endNormal * endDistance;
				var rightEndOffset = -endNormal * endDistance;
				var leftStart = operation.AbsStart + leftStartOffset;
				var rightStart = operation.AbsStart + rightStartOffset;
				var leftEnd = operation.AbsEnd + leftEndOffset;
				var rightEnd = operation.AbsEnd + rightEndOffset;

				if (previousInExtent == null)
				{
					if (leftFirst || bothForward) leftOutline.Lines.AddLast(new GLine { Instruction = GInstruction.G0, XY = leftStart });
					if (!leftFirst || bothForward) rightOutline.Lines.AddLast(new GLine { Instruction = GInstruction.G0, XY = rightStart });
				}

				var leftSegment = line.Clone();
				leftSegment.XY = leftFirst || bothForward ? leftEnd : leftStart;
				if (leftSegment.IsArc)
				{
					if (!leftFirst && !bothForward) leftSegment.Instruction = leftSegment.Instruction == GInstruction.G2 ? GInstruction.G3 : GInstruction.G2;
					var (angle, sweep) = Geometry.AngleAndSweepForArc(operation.AbsStart, operation.AbsEnd, operation.AbsOffset, operation.Line.Instruction == GInstruction.G2);
					var radius = Geometry.LineLength(operation.AbsStart, operation.AbsOffset) + (operation.Line.Instruction == GInstruction.G2 ? distance : -distance);
					var midpointAngle = Math.PI * (angle + sweep / 2) / 180f;
					var midpointOffset = new Vector2((float)(Math.Cos(midpointAngle) * radius), (float)(Math.Sin(midpointAngle) * radius));
					var midpoint = operation.AbsOffset + midpointOffset;
					var center = Geometry.CircleCenterFromThreePoints(leftEnd, leftStart, midpoint);
					leftSegment.IJ = leftFirst || bothForward ? center - leftStart : center - leftEnd;
				}
				if (leftFirst || bothForward) leftOutline.Lines.AddLast(leftSegment);
				else leftOutline.Lines.AddFirst(leftSegment);

				var rightSegment = line.Clone();
				rightSegment.XY = !leftFirst || bothForward ? rightEnd : rightStart;
				if (rightSegment.IsArc)
				{
					if (leftFirst && !bothForward) rightSegment.Instruction = rightSegment.Instruction == GInstruction.G2 ? GInstruction.G3 : GInstruction.G2;
					var (angle, sweep) = Geometry.AngleAndSweepForArc(operation.AbsStart, operation.AbsEnd, operation.AbsOffset, operation.Line.Instruction == GInstruction.G2);
					var radius = Geometry.LineLength(operation.AbsStart, operation.AbsOffset) + (operation.Line.Instruction == GInstruction.G2 ? -distance : distance);
					var midpointAngle = Math.PI * (angle + sweep / 2) / 180f;
					var midpointOffset = new Vector2((float)(Math.Cos(midpointAngle) * radius), (float)(Math.Sin(midpointAngle) * radius));
					var midpoint = operation.AbsOffset + midpointOffset;
					var center = Geometry.CircleCenterFromThreePoints(rightEnd, rightStart, midpoint);
					rightSegment.IJ = !leftFirst || bothForward ? center - rightStart : center - rightEnd;
				}
				if (!leftFirst || bothForward) rightOutline.Lines.AddLast(rightSegment);
				else rightOutline.Lines.AddFirst(rightSegment);

				if (nextInExtent == null)
				{
					if (leftFirst && !bothForward) rightOutline.Lines.AddFirst(new GLine { Instruction = GInstruction.G0, XY = rightEnd });
					if (leftFirst) rightOutline.Lines.AddLast(new GLine { Instruction = GInstruction.G0, XY = operation.AbsEnd });
					if (!leftFirst && !bothForward) leftOutline.Lines.AddFirst(new GLine { Instruction = GInstruction.G0, XY = leftEnd });
					if (!leftFirst) leftOutline.Lines.AddLast(new GLine { Instruction = GInstruction.G0, XY = operation.AbsEnd });
				}

				if (nextInExtent == null)
				{
					var insertionPoint = node;
					if (leftFirst) foreach (var newLine in leftOutline.Lines) insertionPoint = program.Lines.AddAfter(insertionPoint, newLine);
					if (!skipRight) foreach (var newLine in rightOutline.Lines) insertionPoint = program.Lines.AddAfter(insertionPoint, newLine);
					if (!leftFirst && !skipLeft) foreach (var newLine in leftOutline.Lines) insertionPoint = program.Lines.AddAfter(insertionPoint, newLine);

					rightOutline.Lines.Clear();
					leftOutline.Lines.Clear();
				}

				program.Lines.Remove(node);
			}
		}
	}
}
