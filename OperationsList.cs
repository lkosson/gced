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
		public IEnumerable<GOperation> Operations { get => operations; set { operations = value; OnOperationsChanged(); } }
		public GOperation? SelectedOperation { get => selectedOperation; set { selectedOperation = value; OnSelectedOperationChanged(); } }
		public event EventHandler? SelectedOperationChanged;

		private IEnumerable<GOperation> operations;
		private GOperation? selectedOperation;
		private bool selectionInProgress;

		public OperationsList()
		{
			InitializeComponent();
			operations = Enumerable.Empty<GOperation>();
		}

		private void OnOperationsChanged()
		{
			selectionInProgress = true;
			var selectedItem = (ListItem)listBoxOperations.SelectedItem;
			var items = operations.Select(operation => new ListItem(operation)).Cast<object>().ToArray();
			listBoxOperations.BeginUpdate();
			listBoxOperations.Items.Clear();
			listBoxOperations.Items.AddRange(items);
			if (selectedItem != null)
			{
				var newSelectedItem = items.Cast<ListItem>().FirstOrDefault(item => item.Operation.Line == selectedItem.Operation.Line);
				if (newSelectedItem != null) listBoxOperations.SelectedItem = newSelectedItem;
			}
			listBoxOperations.EndUpdate();
			selectionInProgress = false;
		}

		private void OnSelectedOperationChanged()
		{
			if (selectionInProgress) return;
			selectionInProgress = true;
			listBoxOperations.BeginUpdate();
			listBoxOperations.SelectedItems.Clear();
			foreach (ListItem item in listBoxOperations.Items)
			{
				if (item.Operation == selectedOperation)
				{
					listBoxOperations.SelectedItems.Add(item);
					break;
				}
			}
			listBoxOperations.EndUpdate();
			selectionInProgress = false;
		}

		private void listBoxOperations_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectionInProgress) return;
			selectionInProgress = true;
			var item = (ListItem)listBoxOperations.SelectedItem;
			selectedOperation = item?.Operation;
			SelectedOperationChanged?.Invoke(this, EventArgs.Empty);
			selectionInProgress = false;
		}

		private class ListItem
		{
			public GOperation Operation { get; set; }

			public ListItem(GOperation operation)
			{
				Operation = operation;
			}

			public override string ToString()
			{
				return Operation.Line.ToString();
			}
		}
	}
}
