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
			viewState.OperationsChanged += ViewState_OperationsChanged;
			viewState.LoadProgram("test.nc");
		}

		private void ViewState_OperationsChanged()
		{
			canvas.Operations = viewState.Operations;
			operationsList.Operations = viewState.Operations;
			var line = operationProperties.Operation?.Line;
			if (line != null)
			{
				var newOperation = viewState.Operations.FirstOrDefault(operation => operation.Line == line);
				if (newOperation != null) operationProperties.Operation = newOperation;
			}
		}

		private void canvas_SelectedOperationsChanged(object sender, System.EventArgs e)
		{
			var operations = canvas.SelectedOperations;
			operationProperties.Operation = operations.FirstOrDefault();
			operationsList.SelectedOperations = operations;
		}

		private void operationProperties_OperationUpdated(object sender, System.EventArgs e)
		{
			viewState.RunProgram();
		}

		private void operationsList_SelectedOperationsChanged(object sender, System.EventArgs e)
		{
			var operations = operationsList.SelectedOperations;
			canvas.SelectedOperations = operations;
			operationProperties.Operation = operations.FirstOrDefault();
		}
	}
}