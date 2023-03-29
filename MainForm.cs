using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GCEd
{
	public partial class MainForm : Form
	{
		private GProgram program;
		private IEnumerable<GOperation> operations;

		public MainForm()
		{
			InitializeComponent();
			DoubleBuffered = true;
			program = new GProgram();
			program.Read("test.nc");
			RunProgram();
		}

		private void RunProgram()
		{
			operations = program.Run();
			canvas.Operations = operations;
			operationsList.Operations = operations;
			var line = operationProperties.Operation?.Line;
			if (line != null)
			{
				var newOperation = operations.FirstOrDefault(operation => operation.Line == line);
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
			RunProgram();
		}

		private void operationsList_SelectedOperationsChanged(object sender, System.EventArgs e)
		{
			var operations = operationsList.SelectedOperations;
			canvas.SelectedOperations = operations;
			operationProperties.Operation = operations.FirstOrDefault();
		}
	}
}