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
		public GOperation? Operation { get => operation; set { operation = value; OnOperationChanged(); } }
		public event EventHandler? OperationUpdated;

		private GOperation? operation;
		private TextBox? textBoxChangeInProgress;
		private bool operationChangeInProgress;

		public OperationProperties()
		{
			InitializeComponent();
		}

		private void OnOperationChanged()
		{
			var hasOperation = Operation != null;
			var isRapid = Operation != null && Operation.Line.Instruction == GInstruction.G0;
			var isLine = Operation != null && Operation.Line.Instruction == GInstruction.G1;
			var isArc = Operation != null && (Operation.Line.Instruction == GInstruction.G2 || Operation.Line.Instruction == GInstruction.G3);
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

			if (Operation == null)
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
				if (textBoxChangeInProgress != txtRelEndX) txtRelEndX.Text = Fmt(Operation.AbsXEnd - Operation.AbsXStart);
				if (textBoxChangeInProgress != txtRelEndY) txtRelEndY.Text = Fmt(Operation.AbsYEnd - Operation.AbsYStart);
				if (textBoxChangeInProgress != txtRelEndZ) txtRelEndZ.Text = Fmt(Operation.AbsZEnd - Operation.AbsZStart);
				if (textBoxChangeInProgress != txtRelCenterI) txtRelCenterI.Text = Fmt(Operation.Line.I);
				if (textBoxChangeInProgress != txtRelCenterJ) txtRelCenterJ.Text = Fmt(Operation.Line.J);
				if (textBoxChangeInProgress != txtRelCenterK) txtRelCenterK.Text = Fmt(Operation.Line.K);
				if (textBoxChangeInProgress != txtRelF) txtRelF.Text = Fmt(Operation.F);
				if (textBoxChangeInProgress != txtRelS) txtRelS.Text = Fmt(Operation.S);
				if (textBoxChangeInProgress != txtAbsStartX) txtAbsStartX.Text = Fmt(Operation.AbsXStart);
				if (textBoxChangeInProgress != txtAbsStartY) txtAbsStartY.Text = Fmt(Operation.AbsYStart);
				if (textBoxChangeInProgress != txtAbsStartZ) txtAbsStartZ.Text = Fmt(Operation.AbsZStart);
				if (textBoxChangeInProgress != txtAbsEndX) txtAbsEndX.Text = Fmt(Operation.AbsXEnd);
				if (textBoxChangeInProgress != txtAbsEndY) txtAbsEndY.Text = Fmt(Operation.AbsYEnd);
				if (textBoxChangeInProgress != txtAbsEndZ) txtAbsEndZ.Text = Fmt(Operation.AbsZEnd);
				if (textBoxChangeInProgress != txtAbsCenterI) txtAbsCenterI.Text = Operation.Line.I.HasValue ? Fmt(Operation.AbsI) : "";
				if (textBoxChangeInProgress != txtAbsCenterJ) txtAbsCenterJ.Text = Operation.Line.J.HasValue ? Fmt(Operation.AbsJ) : "";
				if (textBoxChangeInProgress != txtAbsCenterK) txtAbsCenterK.Text = Operation.Line.K.HasValue ? Fmt(Operation.AbsK) : "";
				if (textBoxChangeInProgress != txtAbsF) txtAbsF.Text = Fmt(Operation.Line.F);
				if (textBoxChangeInProgress != txtAbsS) txtAbsS.Text = Fmt(Operation.Line.S);
			}
			operationChangeInProgress = false;
		}

		private static string Fmt(float val) => val.ToString("0.000", CultureInfo.InvariantCulture);
		private static string Fmt(decimal? val) => val.HasValue ? val.Value.ToString("0.000", CultureInfo.InvariantCulture) : "";
		private static decimal? ValDecimal(string txt) => String.IsNullOrWhiteSpace(txt) ? null : Decimal.TryParse(txt, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var val) ? val : null;

		private void SetVal(object textboxSender, Action<decimal?> setter)
		{
			if (textboxSender is not TextBox textbox) return;
			if (textBoxChangeInProgress != null) return;
			textBoxChangeInProgress = textbox;

			if (String.IsNullOrWhiteSpace(textbox.Text))
			{
				textbox.BackColor = SystemColors.Window;
				setter(null);
				if (!operationChangeInProgress)
				{
					OperationUpdated?.Invoke(this, EventArgs.Empty);
					OnOperationChanged();
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
						OperationUpdated?.Invoke(this, EventArgs.Empty);
						OnOperationChanged();
					}
				}
				else
				{
					textbox.BackColor = Color.Red;
				}
			}
			textBoxChangeInProgress = null;
		}

		private void txtRelEndX_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.X = Operation!.Absolute ? (decimal)Operation!.AbsXStart + value : value);
		private void txtRelEndY_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Y = Operation!.Absolute ? (decimal)Operation!.AbsYStart + value : value);
		private void txtRelEndZ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Z = Operation!.Absolute ? (decimal)Operation!.AbsZStart + value : value);
		private void txtRelCenterI_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.I = value);
		private void txtRelCenterJ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.J = value);
		private void txtRelCenterK_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.K = value);
		private void txtAbsEndX_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.X = value.HasValue ? Operation!.Absolute ? value.Value : value.Value - (decimal)Operation!.AbsXStart : null);
		private void txtAbsEndY_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Y = value.HasValue ? Operation!.Absolute ? value.Value : value.Value - (decimal)Operation!.AbsYStart : null);
		private void txtAbsEndZ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Z = value.HasValue ? Operation!.Absolute ? value.Value : value.Value - (decimal)Operation!.AbsZStart : null);
		private void txtAbsCenterI_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.I = value.HasValue ? value.Value - (decimal)Operation!.AbsXStart : null);
		private void txtAbsCenterJ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.J = value.HasValue ? value.Value - (decimal)Operation!.AbsYStart : null);
		private void txtAbsCenterK_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.K = value.HasValue ? value.Value - (decimal)Operation!.AbsZStart : null);
		private void txtAbsF_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.F = value);
		private void txtAbsS_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.S = value);
	}
}
