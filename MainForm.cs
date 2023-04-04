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
		}
	}
}