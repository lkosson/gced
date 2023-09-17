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
			var operationsLookup = operations.ToHashSet();
			var newSelection = new List<GOperation>();
			var selectNext = false;
			foreach (var operation in Operations)
			{
				if (operationsLookup.Contains(operation)) selectNext = true;
				else if (selectNext) { newSelection.Add(operation); selectNext = false;}
			}
			Mutator.Delete(program, operations);
			if (newSelection != null) SetSelection(newSelection);
			RunProgram();
		}

		public void ConvertToAbsolute(IEnumerable<GOperation> operations)
		{
			Mutator.ConvertToAbsolute(program, operations);
			RunProgram();
		}

		public void ConvertToRelative(IEnumerable<GOperation> operations)
		{
			Mutator.ConvertToRelative(program, operations);
			RunProgram();
		}

		public void Translate(IEnumerable<GOperation> operations, Vector2 offset)
		{
			Mutator.Translate(program, operations, offset);
			RunProgram();
		}

		public void Rotate(IEnumerable<GOperation> operations, Vector2 center, float angle)
		{
			Mutator.Rotate(program, operations, center, angle);
			RunProgram();
		}

		public void Scale(IEnumerable<GOperation> operations, Vector2 center, float amount)
		{
			Mutator.Scale(program, operations, center, amount);
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
			Mutator.AppendLines(program, baseOperation, before, new[] { line });
			RunProgram();
			var newOperation = operations.First(operation => operation.Line == line);
			SetSelection(new[] { newOperation });
		}

		public void AppendProgram(GOperation? baseOperation, GProgram subprogram, bool before = false)
		{
			Mutator.AppendLines(program, baseOperation, before, subprogram.Lines);
			RunProgram();
			var newLines = new HashSet<GLine>(subprogram.Lines);
			var newOperations = operations.Where(operation => newLines.Contains(operation.Line));
			SetSelection(newOperations);
		}

		public void ConvertToOutline(IEnumerable<GOperation> operations, float thickness, bool leftFirst, bool skipLeft, bool skipRight, bool bothForward)
		{
			Mutator.ConvertToOutline(program, operations, thickness / 2, leftFirst, skipLeft, skipRight, bothForward);
			RunProgram();
		}
	}
}
