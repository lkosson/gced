using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	partial class LineEditor : UserControl
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewState ViewState
		{
			get => viewState;
			set
			{
				if (viewState != null)
				{
					viewState.SelectedOperationsChanged -= ViewState_SelectedOperationsChanged;
					viewState.LineEditorFocused -= ViewState_LineEditorFocused;
				}
				viewState = value;
				viewState.SelectedOperationsChanged += ViewState_SelectedOperationsChanged;
				viewState.LineEditorFocused += ViewState_LineEditorFocused;
			}
		}

		private ViewState viewState;

		public LineEditor()
		{
			viewState = new ViewState();
			InitializeComponent();
		}

		private void ViewState_SelectedOperationsChanged()
		{
			var operation = ViewState.LastSelectedOperation;
			if (operation == null)
			{
				textBoxLine.Text = "";
				textBoxLine.ReadOnly = true;
			}
			else
			{
				textBoxLine.Text = operation.Line.ToString();
				textBoxLine.ReadOnly = false;
			}
		}

		private void ViewState_LineEditorFocused()
		{
			Focus();
		}

		private void textBoxLine_KeyDown(object sender, KeyEventArgs e)
		{
			if (textBoxLine.ReadOnly) return;
			if (e.KeyCode == Keys.Enter)
			{
				var operation = ViewState.LastSelectedOperation;
				if (operation == null) return;
				var newLine = new GLine(textBoxLine.Text);
				if (String.IsNullOrEmpty(newLine.Error))
				{
					viewState.SaveUndoState();
					operation.Line.Parse(textBoxLine.Text);
					ViewState.RunProgram();
					ViewState.FocusCanvas();
				}
				else
				{
					MessageBox.Show(newLine.Error, "GCEd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textBoxLine.Select(newLine.ErrorPosition, 0);
				}
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Escape)
			{
				var operation = ViewState.LastSelectedOperation;
				if (operation == null) return;
				textBoxLine.Text = operation.Line.ToString();
			}
		}
	}
}
