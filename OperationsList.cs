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
	partial class OperationsList : UserControl
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewState ViewState
		{
			get => viewState;
			set
			{
				if (viewState != null)
				{
					viewState.OperationsChanged -= ViewState_OperationsChanged;
					viewState.SelectedOperationsChanged -= ViewState_SelectedOperationsChanged;
				}
				viewState = value;
				viewState.OperationsChanged += ViewState_OperationsChanged;
				viewState.SelectedOperationsChanged += ViewState_SelectedOperationsChanged;
			}
		}

		private ViewState viewState;
		private bool selectionInProgress;

		public OperationsList()
		{
			viewState = new ViewState();
			InitializeComponent();
		}

		private void ViewState_OperationsChanged()
		{
			selectionInProgress = true;
			var topIndex = listBoxOperations.TopIndex;
			var selectedItem = (ListItem)listBoxOperations.SelectedItem;
			var items = viewState.Operations.Select((operation, n) => new ListItem(operation, n + 1)).Cast<object>().ToArray();
			listBoxOperations.BeginUpdate();
			listBoxOperations.Items.Clear();
			listBoxOperations.Items.AddRange(items);
			if (selectedItem != null)
			{
				var newSelectedItem = items.Cast<ListItem>().FirstOrDefault(item => item.Operation.Line == selectedItem.Operation.Line);
				if (newSelectedItem != null) listBoxOperations.SelectedItem = newSelectedItem;
			}
			listBoxOperations.EndUpdate();
			if (topIndex < items.Length) listBoxOperations.TopIndex = topIndex;
			else listBoxOperations.TopIndex = items.Length;
			selectionInProgress = false;
		}

		private void ViewState_SelectedOperationsChanged()
		{
			if (selectionInProgress) return;
			selectionInProgress = true;
			var newSelectedItems = new List<ListItem>();
			foreach (ListItem item in listBoxOperations.Items)
			{
				if (viewState.SelectedOperations.Contains(item.Operation)) newSelectedItems.Add(item);
			}
			listBoxOperations.BeginUpdate();
			listBoxOperations.SelectedItems.Clear();
			foreach (var item in newSelectedItems)
			{
				listBoxOperations.SelectedItems.Add(item);
			}
			listBoxOperations.EndUpdate();
			selectionInProgress = false;
		}

		private void listBoxOperations_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectionInProgress) return;
			selectionInProgress = true;
			var allVisible = true;
			var selectedOperations = new List<GOperation>();
			foreach (ListItem item in listBoxOperations.SelectedItems)
			{
				selectedOperations.Add(item.Operation);
				allVisible &= item.Operation.Line.Instruction == GInstruction.G0 || item.Operation.Line.Instruction == GInstruction.G1 || item.Operation.Line.Instruction == GInstruction.G2 || item.Operation.Line.Instruction == GInstruction.G3;
			}
			viewState.SetSelection(selectedOperations);
			//if (allVisible) viewState.FocusCanvas();
			selectionInProgress = false;
		}

		private void listBoxOperations_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				var selectedOperations = new List<GOperation>();
				foreach (ListItem item in listBoxOperations.SelectedItems)
				{
					selectedOperations.Add(item.Operation);
				}
				viewState.SaveUndoState();
				viewState.DeleteOperations(selectedOperations);
				e.Handled = true;
			}
		}

		private void listBoxOperations_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void listBoxOperations_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			var item = (ListItem)listBoxOperations.Items[e.Index];
			var color = e.ForeColor;
			var text = item.Operation.Line.ToString();
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) color = e.ForeColor;
			else if (item.Operation.Line.Instruction == GInstruction.Empty) { text = "(empty)"; color = Color.LightGray; }
			else if (item.Operation.Line.Instruction == GInstruction.Comment) color = Color.Gray;
			else if (item.Operation.Line.Instruction == GInstruction.Invalid) color = Color.Red;
			else if (item.Operation.Line.Instruction == GInstruction.Directive) color = Color.Blue;

			using var brush = new SolidBrush(color);
			var textBounds = new Rectangle(e.Bounds.X, e.Bounds.Y - 1, e.Bounds.Width - 20, e.Bounds.Height + 2);
			e.Graphics.DrawString(text, e.Font!, brush, textBounds, StringFormat.GenericDefault);

			using var lineNumberFont = new Font(e.Font!.FontFamily, e.Font!.Size * 0.85f, FontStyle.Italic);
			using var lineNumberFormat = new StringFormat();
			lineNumberFormat.Alignment = StringAlignment.Far;
			e.Graphics.DrawString(item.N.ToString(), lineNumberFont, Brushes.DarkGray, e.Bounds, lineNumberFormat);

			e.DrawFocusRectangle();
		}

		private class ListItem
		{
			public GOperation Operation { get; set; }
			public int N { get; set; }

			public ListItem(GOperation operation, int n)
			{
				Operation = operation;
				N = n;
			}
		}
	}
}
