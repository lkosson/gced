﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

		public event Action? OperationsChanged;
		public event Action? SelectedOperationsChanged;
		public event Action? CanvasFocused;
		public event Action? LineEditorFocused;

		private GProgram program;
		private List<GOperation> operations;
		private HashSet<GOperation> selectedOperations;
		private List<GProgram> undoBuffer;

		public ViewState()
		{
			undoBuffer = new List<GProgram>();
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
		}

		public void LoadProgram(string file)
		{
			program = new GProgram();
			program.Load(file);
			RunProgram();
			CurrentFile = file;
		}

		public void SaveProgram(string file)
		{
			program.Save(file);
			CurrentFile = file;
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
			for (var node = program.Lines.First; node != null; node = node.Next)
			{
				if (!linesToDelete.Contains(node.Value)) continue;
				lineNodesToDelete.Add(node);
			}
			foreach (var node in lineNodesToDelete)
			{
				program.Lines.Remove(node);
			}
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
			undoBuffer.Add(program.Clone());
		}

		public void Undo()
		{
			if (undoBuffer.Count == 0) return;
			program = undoBuffer[undoBuffer.Count - 1];
			undoBuffer.RemoveAt(undoBuffer.Count - 1);
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

		public void AppendProgram(GOperation? baseOperation, GProgram subprogram)
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

			if (baseLine == null) foreach (var line in subprogram.Lines) program.Lines.AddLast(line);
			else foreach (var line in subprogram.Lines) baseLine = program.Lines.AddAfter(baseLine, line);

			RunProgram();
			var newLines = new HashSet<GLine>(subprogram.Lines);
			var newOperations = operations.Where(operation => newLines.Contains(operation.Line));
			SetSelection(newOperations);
		}
	}
}
