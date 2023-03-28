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
		public IEnumerable<GOperation> Operations { get => operations; set { operations = value; OnOperationsChanged(); } }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<GOperation> SelectedOperations { get => selectedOperations; set { selectedOperations.Clear(); selectedOperations.UnionWith(value); OnSelectedOperationsChanged(); } }

		public event EventHandler? SelectedOperationsChanged;

		private IEnumerable<GOperation> operations;
		private HashSet<GOperation> selectedOperations;
		private bool selectionInProgress;

		public OperationsList()
		{
			InitializeComponent();
			operations = Enumerable.Empty<GOperation>();
			selectedOperations = new HashSet<GOperation>(GOperation.ByGLineEqualityComparer);
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

		private void OnSelectedOperationsChanged()
		{
			if (selectionInProgress) return;
			selectionInProgress = true;
			var newSelectedItems = new List<ListItem>();
			foreach (ListItem item in listBoxOperations.Items)
			{
				if (selectedOperations.Contains(item.Operation)) newSelectedItems.Add(item);
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
			selectedOperations.Clear();
			foreach (ListItem item in listBoxOperations.SelectedItems)
			{
				selectedOperations.Add(item.Operation);
			}
			SelectedOperationsChanged?.Invoke(this, EventArgs.Empty);
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
