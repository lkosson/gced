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
				if (operation.Line.X.HasValue) operation.Line.X = (decimal)operation.AbsXEnd;
				if (operation.Line.Y.HasValue) operation.Line.Y = (decimal)operation.AbsYEnd;
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
				if (operation.Line.X.HasValue) operation.Line.X = (decimal)(operation.AbsXEnd - operation.AbsXStart);
				if (operation.Line.Y.HasValue) operation.Line.Y = (decimal)(operation.AbsYEnd - operation.AbsYStart);
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

		public void ConvertToOutline(IEnumerable<GOperation> operations, float thickness)
		{
			var elements = GetLineAndNodeForOperations(operations);

			var newLines = new HashSet<GLine>();
			var leftOutline = new GProgram();
			var rightOutline = new GProgram();

			foreach (var (line, operation, node, previousInExtent, nextInExtent) in elements)
			{
				if (line.IsLine)
				{
					var currDirection = Vector2.Normalize(operation.AbsEnd - operation.AbsStart);
					var prevDirection = previousInExtent == null ? currDirection : Vector2.Normalize(previousInExtent.AbsEnd - previousInExtent.AbsStart);
					var nextDirection = nextInExtent == null ? currDirection : Vector2.Normalize(nextInExtent.AbsEnd - nextInExtent.AbsStart);
					var startTangent = Vector2.Normalize((prevDirection + currDirection) / 2);
					var endTangent = Vector2.Normalize((currDirection + nextDirection) / 2);
					var startNormal = new Vector2(-startTangent.Y, startTangent.X);
					var endNormal = new Vector2(-endTangent.Y, endTangent.X);
					var startDistance = thickness / Vector2.Dot(currDirection, startTangent);
					var endDistance = thickness / Vector2.Dot(currDirection, endTangent);
					var leftStartOffset = startNormal * startDistance;
					var rightStartOffset = -startNormal * startDistance;
					var leftEndOffset = endNormal * endDistance;
					var rightEndOffset = -endNormal * endDistance;

					if (previousInExtent == null)
					{
						rightOutline.Lines.AddLast(new GLine { Instruction = GInstruction.G0, X = (decimal)(operation.AbsXStart + rightStartOffset.X), Y = (decimal)(operation.AbsYStart + rightStartOffset.Y) });
					}

					var leftSegment = line.Clone();
					leftSegment.X = (decimal)(operation.AbsXStart + leftStartOffset.X);
					leftSegment.Y = (decimal)(operation.AbsYStart + leftStartOffset.Y);
					leftOutline.Lines.AddFirst(leftSegment);

					var rightSegment = line.Clone();
					rightSegment.X = (decimal)(operation.AbsXEnd + rightEndOffset.X);
					rightSegment.Y = (decimal)(operation.AbsYEnd + rightEndOffset.Y);
					rightOutline.Lines.AddLast(rightSegment);

					if (nextInExtent == null)
					{
						leftOutline.Lines.AddFirst(new GLine { Instruction = GInstruction.G0, X = (decimal)(operation.AbsXEnd + leftEndOffset.X), Y = (decimal)(operation.AbsYEnd + leftEndOffset.Y) });
						leftOutline.Lines.AddLast(new GLine { Instruction = GInstruction.G0, X = (decimal)(operation.AbsXEnd), Y = (decimal)(operation.AbsYEnd) });
					}
				}
				else if (line.IsArc)
				{

				}
				else continue;

				if (nextInExtent == null)
				{
					var insertionPoint = node;
					foreach (var newLine in rightOutline.Lines)
					{
						newLines.Add(newLine);
						insertionPoint = Program.Lines.AddAfter(insertionPoint, newLine);
					}

					foreach (var newLine in leftOutline.Lines)
					{
						newLines.Add(newLine);
						insertionPoint = Program.Lines.AddAfter(insertionPoint, newLine);
					}

					rightOutline.Lines.Clear();
					leftOutline.Lines.Clear();
				}

				Program.Lines.Remove(node);
			}

			RunProgram();
			var newOperations = operations.Where(operation => newLines.Contains(operation.Line));
			SetSelection(newOperations);
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
