using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	partial class OperationProperties : UserControl
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
				}
				viewState = value;
				viewState.SelectedOperationsChanged += ViewState_SelectedOperationsChanged;
			}
		}

		private ViewState viewState;
		private GOperation? operation;
		private TextBox? textBoxChangeInProgress;
		private bool operationChangeInProgress;

		public OperationProperties()
		{
			InitializeComponent();
		}

		private void ViewState_SelectedOperationsChanged()
		{
			var hasOperation = ViewState.LastSelectedOperation != null;
			var isRapid = ViewState.LastSelectedOperation != null && ViewState.LastSelectedOperation.Line.Instruction == GInstruction.G0;
			var isLine = ViewState.LastSelectedOperation != null && ViewState.LastSelectedOperation.Line.Instruction == GInstruction.G1;
			var isArc = ViewState.LastSelectedOperation != null && (ViewState.LastSelectedOperation.Line.Instruction == GInstruction.G2 || ViewState.LastSelectedOperation.Line.Instruction == GInstruction.G3);
			var isAbsolute = ViewState.LastSelectedOperation != null && ViewState.LastSelectedOperation.Absolute;
			var isMove = isRapid || isLine || isArc;

			operationChangeInProgress = true;
			txtRelEndX.Enabled = hasOperation && isMove;
			txtRelEndY.Enabled = hasOperation && isMove;
			txtRelEndZ.Enabled = hasOperation && isMove;
			txtRelCenterI.Enabled = hasOperation && isArc;
			txtRelCenterJ.Enabled = hasOperation && isArc;
			txtRelCenterK.Enabled = hasOperation && isArc;
			txtRelF.Enabled = false;
			txtRelS.Enabled = false;

			txtAbsStartX.Enabled = false;
			txtAbsStartY.Enabled = false;
			txtAbsStartZ.Enabled = false;
			txtAbsEndX.Enabled = hasOperation && isMove;
			txtAbsEndY.Enabled = hasOperation && isMove;
			txtAbsEndZ.Enabled = hasOperation && isMove;
			txtAbsCenterI.Enabled = hasOperation && isArc;
			txtAbsCenterJ.Enabled = hasOperation && isArc;
			txtAbsCenterK.Enabled = hasOperation && isArc;
			txtAbsF.Enabled = hasOperation && isMove;
			txtAbsS.Enabled = hasOperation && (isLine || isArc);

			txtAbsEndX.ForeColor = isAbsolute ? SystemColors.WindowText : SystemColors.GrayText;
			txtAbsEndY.ForeColor = isAbsolute ? SystemColors.WindowText : SystemColors.GrayText;
			txtAbsEndZ.ForeColor = isAbsolute ? SystemColors.WindowText : SystemColors.GrayText;

			txtRelEndX.ForeColor = isAbsolute ? SystemColors.GrayText : SystemColors.WindowText;
			txtRelEndY.ForeColor = isAbsolute ? SystemColors.GrayText : SystemColors.WindowText;
			txtRelEndZ.ForeColor = isAbsolute ? SystemColors.GrayText : SystemColors.WindowText;

			txtRelCenterI.ForeColor = SystemColors.WindowText;
			txtRelCenterJ.ForeColor = SystemColors.WindowText;
			txtRelCenterK.ForeColor = SystemColors.WindowText;

			txtAbsCenterI.ForeColor = SystemColors.GrayText;
			txtAbsCenterJ.ForeColor = SystemColors.GrayText;
			txtAbsCenterK.ForeColor = SystemColors.GrayText;

			if (ViewState.LastSelectedOperation == null)
			{
				txtRelEndX.Text = "";
				txtRelEndY.Text = "";
				txtRelEndZ.Text = "";
				txtRelCenterI.Text = "";
				txtRelCenterJ.Text = "";
				txtRelCenterK.Text = "";
				txtRelF.Text = "";
				txtRelS.Text = "";

				txtAbsStartX.Text = "";
				txtAbsStartY.Text = "";
				txtAbsStartZ.Text = "";
				txtAbsEndX.Text = "";
				txtAbsEndY.Text = "";
				txtAbsEndZ.Text = "";
				txtAbsCenterI.Text = "";
				txtAbsCenterJ.Text = "";
				txtAbsCenterK.Text = "";

				txtAbsF.Text = "";
				txtAbsS.Text = "";
			}
			else
			{
				if (textBoxChangeInProgress != txtRelEndX) txtRelEndX.Text = Fmt(ViewState.LastSelectedOperation.AbsXEnd - ViewState.LastSelectedOperation.AbsXStart);
				if (textBoxChangeInProgress != txtRelEndY) txtRelEndY.Text = Fmt(ViewState.LastSelectedOperation.AbsYEnd - ViewState.LastSelectedOperation.AbsYStart);
				if (textBoxChangeInProgress != txtRelEndZ) txtRelEndZ.Text = Fmt(ViewState.LastSelectedOperation.AbsZEnd - ViewState.LastSelectedOperation.AbsZStart);
				if (textBoxChangeInProgress != txtRelCenterI) txtRelCenterI.Text = Fmt(ViewState.LastSelectedOperation.Line.I);
				if (textBoxChangeInProgress != txtRelCenterJ) txtRelCenterJ.Text = Fmt(ViewState.LastSelectedOperation.Line.J);
				if (textBoxChangeInProgress != txtRelCenterK) txtRelCenterK.Text = Fmt(ViewState.LastSelectedOperation.Line.K);
				if (textBoxChangeInProgress != txtRelF) txtRelF.Text = Fmt(ViewState.LastSelectedOperation.F);
				if (textBoxChangeInProgress != txtRelS) txtRelS.Text = Fmt(ViewState.LastSelectedOperation.S);
				if (textBoxChangeInProgress != txtAbsStartX) txtAbsStartX.Text = Fmt(ViewState.LastSelectedOperation.AbsXStart);
				if (textBoxChangeInProgress != txtAbsStartY) txtAbsStartY.Text = Fmt(ViewState.LastSelectedOperation.AbsYStart);
				if (textBoxChangeInProgress != txtAbsStartZ) txtAbsStartZ.Text = Fmt(ViewState.LastSelectedOperation.AbsZStart);
				if (textBoxChangeInProgress != txtAbsEndX) txtAbsEndX.Text = Fmt(ViewState.LastSelectedOperation.AbsXEnd);
				if (textBoxChangeInProgress != txtAbsEndY) txtAbsEndY.Text = Fmt(ViewState.LastSelectedOperation.AbsYEnd);
				if (textBoxChangeInProgress != txtAbsEndZ) txtAbsEndZ.Text = Fmt(ViewState.LastSelectedOperation.AbsZEnd);
				if (textBoxChangeInProgress != txtAbsCenterI) txtAbsCenterI.Text = ViewState.LastSelectedOperation.Line.I.HasValue ? Fmt(ViewState.LastSelectedOperation.AbsI) : "";
				if (textBoxChangeInProgress != txtAbsCenterJ) txtAbsCenterJ.Text = ViewState.LastSelectedOperation.Line.J.HasValue ? Fmt(ViewState.LastSelectedOperation.AbsJ) : "";
				if (textBoxChangeInProgress != txtAbsCenterK) txtAbsCenterK.Text = ViewState.LastSelectedOperation.Line.K.HasValue ? Fmt(ViewState.LastSelectedOperation.AbsK) : "";
				if (textBoxChangeInProgress != txtAbsF) txtAbsF.Text = Fmt(ViewState.LastSelectedOperation.Line.F);
				if (textBoxChangeInProgress != txtAbsS) txtAbsS.Text = Fmt(ViewState.LastSelectedOperation.Line.S);
			}
			operationChangeInProgress = false;
		}

		private static string Fmt(float val) => val.ToString("0.000", CultureInfo.InvariantCulture);
		private static string Fmt(decimal? val) => val.HasValue ? val.Value.ToString("0.000", CultureInfo.InvariantCulture) : "";
		private static decimal? ValDecimal(string txt) => String.IsNullOrWhiteSpace(txt) ? null : Decimal.TryParse(txt, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var val) ? val : null;

		private void SetVal(object textboxSender, Action<decimal?> setter)
		{
			if (textboxSender is not TextBox textbox) return;
			if (operationChangeInProgress) return;
			if (textBoxChangeInProgress != null) return;
			textBoxChangeInProgress = textbox;

			if (String.IsNullOrWhiteSpace(textbox.Text))
			{
				textbox.BackColor = SystemColors.Window;
				setter(null);
				if (!operationChangeInProgress)
				{
					ViewState.RunProgram();
				}
			}
			else
			{
				if (Decimal.TryParse(textbox.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var val))
				{
					textbox.BackColor = SystemColors.Window;
					setter(val);
					if (!operationChangeInProgress)
					{
						ViewState.RunProgram();
					}
				}
				else
				{
					textbox.BackColor = Color.Red;
				}
			}
			textBoxChangeInProgress = null;
		}

		private void txtRelEndX_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.X = ViewState.LastSelectedOperation!.Absolute ? (decimal)ViewState.LastSelectedOperation!.AbsXStart + value : value);
		private void txtRelEndY_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.Y = ViewState.LastSelectedOperation!.Absolute ? (decimal)ViewState.LastSelectedOperation!.AbsYStart + value : value);
		private void txtRelEndZ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.Z = ViewState.LastSelectedOperation!.Absolute ? (decimal)ViewState.LastSelectedOperation!.AbsZStart + value : value);
		private void txtRelCenterI_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.I = value);
		private void txtRelCenterJ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.J = value);
		private void txtRelCenterK_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.K = value);
		private void txtAbsEndX_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.X = value.HasValue ? ViewState.LastSelectedOperation!.Absolute ? value.Value : value.Value - (decimal)ViewState.LastSelectedOperation!.AbsXStart : null);
		private void txtAbsEndY_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.Y = value.HasValue ? ViewState.LastSelectedOperation!.Absolute ? value.Value : value.Value - (decimal)ViewState.LastSelectedOperation!.AbsYStart : null);
		private void txtAbsEndZ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.Z = value.HasValue ? ViewState.LastSelectedOperation!.Absolute ? value.Value : value.Value - (decimal)ViewState.LastSelectedOperation!.AbsZStart : null);
		private void txtAbsCenterI_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.I = value.HasValue ? value.Value - (decimal)ViewState.LastSelectedOperation!.AbsXStart : null);
		private void txtAbsCenterJ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.J = value.HasValue ? value.Value - (decimal)ViewState.LastSelectedOperation!.AbsYStart : null);
		private void txtAbsCenterK_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.K = value.HasValue ? value.Value - (decimal)ViewState.LastSelectedOperation!.AbsZStart : null);
		private void txtAbsF_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.F = value);
		private void txtAbsS_TextChanged(object sender, EventArgs e) => SetVal(sender, value => ViewState.LastSelectedOperation!.Line.S = value);
	}
}
