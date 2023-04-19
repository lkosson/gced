using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class ViewState
	{
		public GProgram Program { get => program; }
		public IEnumerable<GOperation> Operations { get => operations; }
		public IReadOnlySet<GOperation> SelectedOperations { get => selectedOperations; }
		public GOperation? LastSelectedOperation { get; private set; }
		public string? CurrentFile { get; set; }
		public bool IsDirty { get; private set; }

		public event Action? OperationsChanged;
		public event Action? SelectedOperationsChanged;
		public event Action? CanvasFocused;
		public event Action? LineEditorFocused;

		private GProgram program;
		private List<GOperation> operations;
		private HashSet<GOperation> selectedOperations;
		private List<GProgram> undoBuffer;
		private List<GProgram> redoBuffer;

		public ViewState()
		{
			undoBuffer = new List<GProgram>();
			redoBuffer = new List<GProgram>();
			program = new GProgram();
			operations = new List<GOperation>();
			selectedOperations = new HashSet<GOperation>(GOperation.ByGLineEqualityComparer);
		}

		public void NewProgram()
		{
			CurrentFile = null;
			program = new GProgram();
			program.New();
			RunProgram();
			IsDirty = false;
		}

		public void LoadProgram(string file)
		{
			program = new GProgram();
			program.Load(file);
			RunProgram();
			CurrentFile = file;
			IsDirty = false;
		}

		public void SaveProgram(string file)
		{
			program.Save(file);
			CurrentFile = file;
			IsDirty = false;
		}

		public void RunProgram()
		{
			var newOperations = program.Run();
			operations.Clear();
			operations.AddRange(newOperations);
			OperationsChanged?.Invoke();
			var newSelection = new List<GOperation>();
			foreach (var operation in operations)
			{
				if (selectedOperations.Contains(operation))
					newSelection.Add(operation);
			}
			SetSelection(newSelection);
		}

		public void SetSelection(IEnumerable<GOperation> newSelection)
		{
			selectedOperations.Clear();
			foreach (var operation in newSelection)
				selectedOperations.Add(operation);
			LastSelectedOperation = newSelection.LastOrDefault();
			SelectedOperationsChanged?.Invoke();
		}

		public void DeleteOperations(IEnumerable<GOperation> operations)
		{
			var linesToDelete = new HashSet<GLine>(operations.Select(operation => operation.Line));
			var lineNodesToDelete = new List<LinkedListNode<GLine>>();
			GLine? newSelection = null;
			for (var node = program.Lines.First; node != null; node = node.Next)
			{
				if (!linesToDelete.Contains(node.Value)) continue;
				if (node.Previous != null && newSelection == null) newSelection = node.Previous.Value;
				lineNodesToDelete.Add(node);
			}
			foreach (var node in lineNodesToDelete)
			{
				program.Lines.Remove(node);
			}
			if (newSelection != null) SetSelection(Operations.Where(operation => operation.Line == newSelection));
			RunProgram();
		}

		public void ConvertToAbsolute(IEnumerable<GOperation> operations)
		{
			var linesToConvert = new HashSet<GLine>(operations.Where(operation => !operation.Absolute).Select(operation => operation.Line));
			var lineNodesToConvert = new List<LinkedListNode<GLine>>();
			for (var node = program.Lines.First; node != null; node = node.Next)
			{
				if (!linesToConvert.Contains(node.Value)) continue;
				lineNodesToConvert.Add(node);
			}
			foreach (var node in lineNodesToConvert)
			{
				if (node.Value.Instruction == GInstruction.G90 || node.Value.Instruction == GInstruction.G91) continue;
				if (node.Previous != null && node.Previous.Value.Instruction == GInstruction.G91) program.Lines.Remove(node.Previous);
				else if (node.Previous == null || node.Previous.Value.Instruction != GInstruction.G90) program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G90 });

				if (node.Next != null && node.Next.Value.Instruction == GInstruction.G90) program.Lines.Remove(node.Next);
				else if (node.Next == null || node.Next.Value.Instruction != GInstruction.G91) program.Lines.AddAfter(node, new GLine { Instruction = GInstruction.G91 });
			}
			foreach (var operation in operations)
			{
				operation.Line.XY = operation.AbsEnd;
			}
			RunProgram();
		}

		public void ConvertToRelative(IEnumerable<GOperation> operations)
		{
			var linesToConvert = new HashSet<GLine>(operations.Where(operation => operation.Absolute).Select(operation => operation.Line));
			var lineNodesToConvert = new List<LinkedListNode<GLine>>();
			for (var node = program.Lines.First; node != null; node = node.Next)
			{
				if (!linesToConvert.Contains(node.Value)) continue;
				lineNodesToConvert.Add(node);
			}
			foreach (var node in lineNodesToConvert)
			{
				if (node.Value.Instruction == GInstruction.G90 || node.Value.Instruction == GInstruction.G91) continue;
				if (node.Previous != null && node.Previous.Value.Instruction == GInstruction.G90) program.Lines.Remove(node.Previous);
				else if (node.Previous == null || node.Previous.Value.Instruction != GInstruction.G91) program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G91 });

				if (node.Next != null && node.Next.Value.Instruction == GInstruction.G91) program.Lines.Remove(node.Next);
				else if (node.Next == null || node.Next.Value.Instruction != GInstruction.G90) program.Lines.AddAfter(node, new GLine { Instruction = GInstruction.G90 });
			}
			foreach (var operation in operations)
			{
				operation.Line.XY = operation.AbsEnd - operation.AbsStart;
			}
			RunProgram();
		}

		public void Translate(IEnumerable<GOperation> operations, decimal offsetX, decimal offsetY)
		{
			var operationsToTranslate = new Dictionary<GLine, GOperation>();
			foreach (var operation in operations)
			{
				operationsToTranslate[operation.Line] = operation;
			}

			var first = true;
			GOperation? last = null;
			for (var node = program.Lines.First; node != null; node = node.Next)
			{
				if (!node.Value.IsVisible) continue;
				if (!operationsToTranslate.TryGetValue(node.Value, out var operation))
				{
					if (!first && last != null)
					{
						if (last.Line.Instruction == GInstruction.G0)
						{
							last.Line.X = last.Line.X.GetValueOrDefault() - offsetX;
							last.Line.Y = last.Line.Y.GetValueOrDefault() - offsetY;
						}
						else if (node.Value.Instruction == GInstruction.G0)
						{
							if (!last.Absolute)
							{
								node.Value.X = node.Value.X.GetValueOrDefault() - offsetX;
								node.Value.Y = node.Value.Y.GetValueOrDefault() - offsetY;
							}
						}
						else
						{
							if (last.Absolute) program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G0, X = (decimal)last.AbsXEnd, Y = (decimal)last.AbsYEnd });
							else program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G0, X = -offsetX, Y = -offsetY });
						}
					}
					first = true;
					continue;
				}

				if (first)
				{
					var previousLine = node.Previous?.Value;
					if (previousLine != null && previousLine.Instruction == GInstruction.G0)
					{
						previousLine.X = previousLine.X.GetValueOrDefault() + offsetX;
						previousLine.Y = previousLine.Y.GetValueOrDefault() + offsetY;
					}
					else if (node.Value.Instruction == GInstruction.G0)
					{
						if (!operation.Absolute)
						{
							node.Value.X = node.Value.X.GetValueOrDefault() + offsetX;
							node.Value.Y = node.Value.Y.GetValueOrDefault() + offsetY;
						}
					}
					else
					{
						if (operation.Absolute) program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G0, X = (decimal)operation.AbsXStart + offsetX, Y = (decimal)operation.AbsYStart + offsetY });
						else program.Lines.AddBefore(node, new GLine { Instruction = GInstruction.G0, X = offsetX, Y = offsetY });
					}

					first = false;
				}

				if (operation.Absolute)
				{
					node.Value.X = node.Value.X.GetValueOrDefault() + offsetX;
					node.Value.Y = node.Value.Y.GetValueOrDefault() + offsetY;
				}

				last = operation;
			}

			RunProgram();
		}

		public void FocusCanvas()
		{
			CanvasFocused?.Invoke();
		}

		public void FocusLineEditor()
		{
			LineEditorFocused?.Invoke();
		}

		public void SaveUndoState()
		{
			redoBuffer.Clear();
			undoBuffer.Add(program.Clone());
			IsDirty = true;
		}

		public void Undo()
		{
			if (undoBuffer.Count == 0) return;
			redoBuffer.Add(program.Clone());
			program = undoBuffer[^1];
			undoBuffer.RemoveAt(undoBuffer.Count - 1);
			RunProgram();
		}

		public void Redo()
		{
			if (redoBuffer.Count == 0) return;
			undoBuffer.Add(program.Clone());
			program = redoBuffer[^1];
			redoBuffer.RemoveAt(redoBuffer.Count - 1);
			RunProgram();
		}

		public void AppendNewLine(GOperation? baseOperation, bool before = false, GLine? line = null)
		{
			if (line == null) line = new GLine();
			if (baseOperation == null)
			{
				if (program.Lines.Count == 0 || before) program.Lines.AddFirst(line);
				else program.Lines.AddLast(line);
			}
			else
			{
				for (var node = program.Lines.First; node != null; node = node.Next)
				{
					if (node.Value != baseOperation.Line) continue;
					if (before) program.Lines.AddBefore(node, line);
					else program.Lines.AddAfter(node, line);
					break;
				}
			}
			RunProgram();
			var newOperation = operations.First(operation => operation.Line == line);
			SetSelection(new[] { newOperation });
		}

		public void AppendProgram(GOperation? baseOperation, GProgram subprogram, bool before = false)
		{
			var baseLine = program.Lines.Last;
			if (baseOperation != null)
			{
				for (var node = program.Lines.First; node != null; node = node.Next)
				{
					if (node.Value != baseOperation.Line) continue;
					baseLine = node;
					break;
				}
			}

			if (before)
			{
				var marker = baseLine == null ? program.Lines.AddFirst(new GLine()) : program.Lines.AddBefore(baseLine, new GLine());
				baseLine = marker;
				foreach (var line in subprogram.Lines) baseLine = program.Lines.AddAfter(baseLine, line);
				program.Lines.Remove(marker);
			}
			else
			{
				if (baseLine == null) foreach (var line in subprogram.Lines) program.Lines.AddLast(line);
				else foreach (var line in subprogram.Lines) baseLine = program.Lines.AddAfter(baseLine, line);
			}

			RunProgram();
			var newLines = new HashSet<GLine>(subprogram.Lines);
			var newOperations = operations.Where(operation => newLines.Contains(operation.Line));
			SetSelection(newOperations);
		}

		public void ConvertToOutline(IEnumerable<GOperation> operations, float thickness, bool leftFirst, bool skipLeft, bool skipRight, bool bothForward)
		{
			var elements = GetLineAndNodeForOperations(operations);

			var leftOutline = new GProgram();
			var rightOutline = new GProgram();

			foreach (var (line, operation, node, previousInExtent, nextInExtent) in elements)
			{
				if (!line.IsVisible) continue;
				if (Geometry.LineLength(line.XY) == 0) continue;

				var currTangents = Geometry.TangentsForOperation(operation);
				var (startTangent, endTangent) = Geometry.TangentsForOperation(operation, previousInExtent, nextInExtent);
				var startNormal = Geometry.Normal(startTangent);
				var endNormal = Geometry.Normal(endTangent);
				var startDistance = thickness / Vector2.Dot(currTangents.startTangent, startTangent);
				var endDistance = thickness / Vector2.Dot(currTangents.endTangent, endTangent);
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
					var radius = Geometry.LineLength(operation.AbsStart, operation.AbsOffset) + (operation.Line.Instruction == GInstruction.G2 ? thickness : -thickness);
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
					var radius = Geometry.LineLength(operation.AbsStart, operation.AbsOffset) + (operation.Line.Instruction == GInstruction.G2 ? -thickness : thickness);
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
					if (leftFirst) foreach (var newLine in leftOutline.Lines) insertionPoint = Program.Lines.AddAfter(insertionPoint, newLine);
					if (!skipRight) foreach (var newLine in rightOutline.Lines) insertionPoint = Program.Lines.AddAfter(insertionPoint, newLine);
					if (!leftFirst && !skipLeft) foreach (var newLine in leftOutline.Lines) insertionPoint = Program.Lines.AddAfter(insertionPoint, newLine);

					rightOutline.Lines.Clear();
					leftOutline.Lines.Clear();
				}

				Program.Lines.Remove(node);
			}

			RunProgram();
		}

		private IEnumerable<(GLine line, GOperation operation, LinkedListNode<GLine> node, GOperation? previousInExtent, GOperation? nextInExtent)> GetLineAndNodeForOperations(IEnumerable<GOperation> operations)
		{
			var operationToLineMapping = new Dictionary<GLine, GOperation>();
			foreach (var operation in operations)
			{
				operationToLineMapping[operation.Line] = operation;
			}
			var result = new List<(GLine line, GOperation operation, LinkedListNode<GLine> node, GOperation? previousInExtent, GOperation? nextInExtent)>();

			for (var node = program.Lines.First; node != null; node = node.Next)
			{
				if (!operationToLineMapping.TryGetValue(node.Value, out var operation)) continue;
				result.Add((node.Value, operation, node, node.Previous == null ? null : operationToLineMapping.GetValueOrDefault(node.Previous.Value), node.Next == null ? null : operationToLineMapping.GetValueOrDefault(node.Next.Value)));
			}
			return result;

		}
	}
}
