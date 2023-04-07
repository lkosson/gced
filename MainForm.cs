using System.Collections.Generic;
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

		private void NewFile()
		{
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

		private void AddG0()
		{
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G0 });
			canvas.StartMouseEndMove();
		}

		private void AddG1()
		{
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G1 });
			canvas.StartMouseEndMove();
		}

		private void AddG2()
		{
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G2 });
			canvas.StartMouseEndMove();
		}

		private void AddG3()
		{
			viewState.SaveUndoState();
			viewState.AppendNewLine(viewState.LastSelectedOperation, ModifierKeys == Keys.Shift, new GLine { Instruction = GInstruction.G3 });
			canvas.StartMouseEndMove();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			e.Handled = true;
			var editorFocused = ActiveControl is LineEditor || ActiveControl is OperationProperties;
			if (e.KeyCode == Keys.N && ModifierKeys == Keys.Control) NewFile();
			else if (e.KeyCode == Keys.O && ModifierKeys == Keys.Control) OpenFile();
			else if (e.KeyCode == Keys.S && ModifierKeys == Keys.Control) SaveFile();
			else if (e.KeyCode == Keys.S && ModifierKeys == (Keys.Control | Keys.Alt)) SaveFileAs();
			else if (e.KeyCode == Keys.Menu) { }
			else if (editorFocused) { e.Handled = false; }
			else if (e.KeyCode == Keys.A && (ModifierKeys & Keys.Control) == Keys.Control) SelectAll();
			else if (e.KeyCode == Keys.Z && ModifierKeys == Keys.Control) Undo();
			else if (e.KeyCode == Keys.A && ModifierKeys == Keys.None) ConvertToAbsolute();
			else if (e.KeyCode == Keys.I) AddNewLine();
			else if (e.KeyCode == Keys.R) ConvertToRelative();
			else if (e.KeyCode == Keys.Delete) DeleteSelected();
			else if (e.KeyCode == Keys.D0) AddG0();
			else if (e.KeyCode == Keys.D1) AddG1();
			else if (e.KeyCode == Keys.D2) AddG2();
			else if (e.KeyCode == Keys.D3) AddG3();
			else { e.Handled = false; }

			base.OnKeyDown(e);
		}
	}
}