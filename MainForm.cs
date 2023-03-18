using System.Windows.Forms;

namespace GCEd
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void timerFPSCounter_Tick(object sender, System.EventArgs e)
		{
			if (canvas.FrameCount == 0) return;
			Text = (canvas.PaintTime / canvas.FrameCount) + " ms";
			if (canvas.PaintTime == 0) return;
			Text += ", " + (1000 * canvas.FrameCount / canvas.PaintTime) + " fps";

			canvas.FrameCount = 0;
			canvas.PaintTime = 0;
		}
	}
}