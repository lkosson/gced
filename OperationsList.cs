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
		private bool selectionChanged;

		public OperationsList()
		{
			viewState = new ViewState();
			InitializeComponent();
			listViewOperations.Columns.Add(new ColumnHeader());
			listViewOperations.Columns.Add(new ColumnHeader() { TextAlign = HorizontalAlignment.Right });
			listViewOperations.HeaderStyle = ColumnHeaderStyle.None;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateColumnWidths();
		}

		private void UpdateColumnWidths()
		{
			if (listViewOperations.Columns.Count < 2) return;
			listViewOperations.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
			listViewOperations.Columns[0].Width = listViewOperations.ClientSize.Width - listViewOperations.Columns[1].Width;
		}

		private void ViewState_OperationsChanged()
		{
			selectionInProgress = true;
			listViewOperations.BeginUpdate();
			listViewOperations.Items.Clear();
			var items = new List<ListViewItem>();
			foreach (var operation in viewState.Operations)
			{
				var item = new ListViewItem();
				item.Text = operation.Line.ToString();
				item.Tag = operation;
				item.ToolTipText = operation.Line.Error;
				item.UseItemStyleForSubItems = false;
				var subitem = item.SubItems.Add((items.Count + 1).ToString());
				subitem.ForeColor = Color.DarkGray;

				if (operation.Line.Instruction == GInstruction.Empty) { item.Text = "(empty)"; item.ForeColor = Color.LightGray; }
				else if (operation.Line.Instruction == GInstruction.Comment) item.ForeColor = Color.Gray;
				else if (operation.Line.Instruction == GInstruction.Invalid) item.ForeColor = Color.Red;
				else if (operation.Line.Instruction == GInstruction.Directive) item.ForeColor = Color.Blue;

				items.Add(item);
			}
			listViewOperations.Items.AddRange(items.ToArray());
			listViewOperations.EndUpdate();
			selectionInProgress = false;
			UpdateColumnWidths();
		}

		private void ViewState_SelectedOperationsChanged()
		{
			if (selectionInProgress) return;
			selectionInProgress = true;
			listViewOperations.BeginUpdate();
			foreach (ListViewItem item in listViewOperations.Items)
			{
				var operation = (GOperation)item.Tag;
				item.Selected = viewState.SelectedOperations.Contains(operation);
				if (viewState.LastSelectedOperation == operation) listViewOperations.TopItem = listViewOperations.Items[Math.Max(0, item.Index - 3)];
			}
			listViewOperations.EndUpdate();
			selectionInProgress = false;
		}

		private void listBoxOperations_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectionChanged = true;
		}

		private void UpdateSelectionIfChanged()
		{
			if (!selectionChanged) return;
			selectionChanged = false;
			if (selectionInProgress) return;
			selectionInProgress = true;
			var allVisible = true;
			var selectedOperations = new List<GOperation>();
			foreach (ListViewItem item in listViewOperations.SelectedItems)
			{
				var operation = (GOperation)item.Tag;
				selectedOperations.Add(operation);
				allVisible &= operation.Line.IsVisible;
			}
			viewState.SetSelection(selectedOperations);
			selectionInProgress = false;
		}

		private void listViewOperations_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				var selectedOperations = new List<GOperation>();
				foreach (ListViewItem item in listViewOperations.SelectedItems)
				{
					var operation = (GOperation)item.Tag;
					selectedOperations.Add(operation);
				}
				viewState.SaveUndoState();
				viewState.DeleteOperations(selectedOperations);
				e.Handled = true;
			}
		}

		private void listViewOperations_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void listViewOperations_MouseUp(object sender, MouseEventArgs e)
		{
			UpdateSelectionIfChanged();
		}

		private void listViewOperations_KeyUp(object sender, KeyEventArgs e)
		{
			UpdateSelectionIfChanged();
		}

		private class DoubleBufferedListView : ListView
		{
			public DoubleBufferedListView()
			{
				DoubleBuffered = true;
			}
		}
	}
}
