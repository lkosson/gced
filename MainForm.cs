using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GCEd
{
	public partial class MainForm : Form
	{
		private ViewState viewState;

		public MainForm()
		{
			InitializeComponent();
			DoubleBuffered = true;
			viewState = new ViewState();
			canvas.ViewState = viewState;
			operationsList.ViewState = viewState;
			operationProperties.ViewState = viewState;
			lineEditor.ViewState = viewState;
			viewState.LoadProgram("test.nc");
			KeyPreview = true;
		}

		private bool ConfirmAbandonDirty()
		{
			if (!viewState.IsDirty) return true;
			var result = MessageBox.Show(viewState.CurrentFile == null ? "Save current file?" : $"Save \"{viewState.CurrentFile}\"?", "GCEd", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if (result == DialogResult.Cancel) return false;
			if (result == DialogResult.No) return true;
			if (viewState.CurrentFile != null)
			{
				viewState.SaveProgram(viewState.CurrentFile);
			}
			else
			{
				if (saveFileDialog.ShowDialog() != DialogResult.OK) return false;
				viewState.SaveProgram(saveFileDialog.FileName);
			}
			return true;
		}

		private void NewFile()
		{
			if (!ConfirmAbandonDirty()) return;
			viewState.NewProgram();
		}

		private void SaveFile()
		{
			if (viewState.CurrentFile == null) SaveFileAs();
			else viewState.SaveProgram(viewState.CurrentFile);
		}

		private void SaveFileAs()
		{
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			viewState.SaveProgram(saveFileDialog.FileName);
		}

		private void OpenFile()
		{
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			if (!ConfirmAbandonDirty()) return;
			viewState.LoadProgram(openFileDialog.FileName);
		}

		private void SelectAll()
		{
			var skipRapid = (ModifierKeys & Keys.Alt) == Keys.Alt;
			var selectedOperations = new List<GOperation>();
			foreach (var operation in viewState.Operations)
			{
				if (!operation.Line.IsVisible) continue;
				if (operation.Line.Instruction == GInstruction.G0 && skipRapid) continue;
				selectedOperations.Add(operation);
			}
			viewState.SetSelection(selectedOperations);
		}

		private void DeleteSelected()
		{
			viewState.SaveUndoState();
			viewState.DeleteOperations(viewState.SelectedOperations);
		}

		private void Undo()
		{
			viewState.Undo();
		}

		private void Redo()
		{
			viewState.Redo();
		}

		private void ConvertToAbsolute()
		{
			viewState.SaveUndoState();
			viewState.ConvertToAbsolute(viewState.SelectedOperations);
		}

		private void ConvertToRelative()
		{
			viewState.SaveUndoState();
			viewState.ConvertToRelative(viewState.SelectedOperations);
		}

		private void AddNewLine()
		{
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift);
			viewState.FocusLineEditor();
		}

		private void AddComment()
		{
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine("; "));
			viewState.FocusLineEditor();
		}

		private void AddG0()
		{
			canvas.SuspendPanningToSelection();
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G0 });
			canvas.StartMouseEndMove();
			canvas.ResumePanningToSelection();
		}

		private void AddG1()
		{
			canvas.SuspendPanningToSelection();
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G1 });
			canvas.StartMouseEndMove();
			canvas.ResumePanningToSelection();
		}

		private void AddG2()
		{
			canvas.SuspendPanningToSelection();
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G2 });
			canvas.StartMouseEndMove();
			canvas.ResumePanningToSelection();
		}

		private void AddG3()
		{
			canvas.SuspendPanningToSelection();
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G3 });
			canvas.StartMouseEndMove();
			canvas.ResumePanningToSelection();
		}

		private void AddBackground()
		{
			using var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Images (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All files|*.*";
			openFileDialog.RestoreDirectory = true;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			viewState.AppendNewLine(null, true, new GLine(";.background X0 Y0 I100 J100 S100 P\"" + openFileDialog.FileName + "\""));
		}

		private void AddText()
		{
			using var textGenerator = new TextGenerator();
			if (textGenerator.ShowDialog() != DialogResult.OK || textGenerator.Program == null) return;
			var subProgram = textGenerator.Program;
			viewState.SaveUndoState();
			var baseOperation = viewState.LastSelectedOperation;
			if (baseOperation == null) baseOperation = viewState.Operations.LastOrDefault();
			if (baseOperation == null || !baseOperation.Absolute)
			{
				subProgram.Lines.AddFirst(new GLine { Instruction = GInstruction.G90 });
				subProgram.Lines.AddLast(new GLine { Instruction = GInstruction.G91 });
			}
			viewState.AppendProgram(viewState.LastSelectedOperation, subProgram);
		}

		private void Copy()
		{
			var code = String.Join(Environment.NewLine, viewState.SelectedOperations.Select(operation => operation.Line.ToString()));
			Clipboard.SetText(code);
		}

		private void Cut()
		{
			Copy();
			DeleteSelected();
		}

		private void Paste()
		{
			var code = Clipboard.GetText();
			if (String.IsNullOrWhiteSpace(code)) return;
			var before = (ModifierKeys & Keys.Shift) == Keys.Shift;
			var subProgram = new GProgram();
			subProgram.Load(new StringReader(code));
			viewState.SaveUndoState();
			viewState.AppendProgram(viewState.LastSelectedOperation, subProgram, before);
		}


		public void ToggleGrid()
		{
			if (canvas.ShowMinorGrid)
			{
				canvas.ShowMinorGrid = false;
				canvas.ShowMajorGrid = false;
				canvas.ShowOriginGrid = false;
			}
			else if (!canvas.ShowOriginGrid) canvas.ShowOriginGrid = true;
			else if (!canvas.ShowMajorGrid) canvas.ShowMajorGrid = true;
			else canvas.ShowMinorGrid = true;
			canvas.Invalidate();
		}

		public void ToggleSnapToGrid()
		{
			canvas.SnapToGrid = !canvas.SnapToGrid;
		}

		public void ToggleSnapToItems()
		{
			canvas.SnapToItems = !canvas.SnapToItems;
		}

		public void ToggleFPS()
		{
			canvas.ShowFPS = !canvas.ShowFPS;
			canvas.Invalidate();
		}

		public void ToggleCoords()
		{
			canvas.ShowCursorCoords = !canvas.ShowCursorCoords;
			if (!canvas.ShowCursorCoords) canvas.ShowItemCoords = !canvas.ShowItemCoords;
			canvas.Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			e.Handled = true;

			var editorFocused = false;
			IContainerControl? container = this;
			while (container != null)
			{
				editorFocused = container.ActiveControl is TextBox;
				container = container.ActiveControl as IContainerControl;
			}
			if (e.KeyCode == Keys.N && ModifierKeys == Keys.Control) NewFile();
			else if (e.KeyCode == Keys.O && ModifierKeys == Keys.Control) OpenFile();
			else if (e.KeyCode == Keys.S && ModifierKeys == Keys.Control) SaveFile();
			else if (e.KeyCode == Keys.S && ModifierKeys == (Keys.Control | Keys.Alt)) SaveFileAs();
			else if (e.KeyCode == Keys.Menu) { }
			else if (editorFocused) { e.Handled = false; }

			else if (e.KeyCode == Keys.D0 && ModifierKeys == Keys.None) AddG0();
			else if (e.KeyCode == Keys.D1 && ModifierKeys == Keys.None) AddG1();
			else if (e.KeyCode == Keys.D2 && ModifierKeys == Keys.None) AddG2();
			else if (e.KeyCode == Keys.D3 && ModifierKeys == Keys.None) AddG3();
			else if (e.KeyCode == Keys.A && ModifierKeys == Keys.None) ConvertToAbsolute();
			else if (e.KeyCode == Keys.A && ModifierKeys == Keys.Shift) ConvertToRelative();
			else if (e.KeyCode == Keys.A && (ModifierKeys & Keys.Control) == Keys.Control) SelectAll();
			else if (e.KeyCode == Keys.B && ModifierKeys == Keys.None) AddBackground();
			else if (e.KeyCode == Keys.C && ModifierKeys == Keys.None) ToggleCoords();
			else if (e.KeyCode == Keys.C && ModifierKeys == Keys.Control) Copy();
			else if (e.KeyCode == Keys.E && ModifierKeys == Keys.None) canvas.StartMouseEndMove();
			else if (e.KeyCode == Keys.E && ModifierKeys == Keys.Shift) canvas.StartMouseOffsetMove();
			else if (e.KeyCode == Keys.F && ModifierKeys == Keys.None) ToggleFPS();
			else if (e.KeyCode == Keys.G && ModifierKeys == Keys.None) ToggleGrid();
			else if (e.KeyCode == Keys.H && ModifierKeys == Keys.None) canvas.PanZoomViewToFit();
			else if (e.KeyCode == Keys.I && ModifierKeys == Keys.None) AddNewLine();
			else if (e.KeyCode == Keys.I && ModifierKeys == Keys.Shift) AddNewLine();
			else if (e.KeyCode == Keys.S && ModifierKeys == Keys.None) ToggleSnapToGrid();
			else if (e.KeyCode == Keys.S && ModifierKeys == Keys.Shift) ToggleSnapToItems();
			else if (e.KeyCode == Keys.T && ModifierKeys == Keys.None) AddText();
			else if (e.KeyCode == Keys.V && (ModifierKeys & Keys.Control) == Keys.Control) Paste();
			else if (e.KeyCode == Keys.X && ModifierKeys == Keys.Control) Cut();
			else if (e.KeyCode == Keys.Y && ModifierKeys == Keys.Control) Redo();
			else if (e.KeyCode == Keys.Z && ModifierKeys == Keys.Control) Undo();
			else if (e.KeyCode == Keys.Delete && ModifierKeys == Keys.None) DeleteSelected();
			else if (e.KeyCode == Keys.Escape) canvas.Abort();
			else if (e.KeyCode == Keys.OemSemicolon && ModifierKeys == Keys.None) AddComment();
			else if (e.KeyCode == Keys.OemSemicolon && ModifierKeys == Keys.Shift) AddComment();
			else { e.Handled = false; }

			base.OnKeyDown(e);
		}
	}
}