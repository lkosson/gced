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
			canvas.ViewState = viewState;
			operationsList.ViewState = viewState;
			viewState.LoadProgram("test.nc");
		}

		private void ViewState_OperationsChanged()
		{
			var line = operationProperties.Operation?.Line;
			if (line != null)
			{
				var newOperation = viewState.Operations.FirstOrDefault(operation => operation.Line == line);
				if (newOperation != null) operationProperties.Operation = newOperation;
			}
		}

		private void operationProperties_OperationUpdated(object sender, System.EventArgs e)
		{
			viewState.RunProgram();
		}
	}
}