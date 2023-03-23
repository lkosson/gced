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
using static System.Net.Mime.MediaTypeNames;

namespace GCEd
{
	partial class OperationProperties : UserControl
	{
		public GOperation? Operation { get => operation; set { operation = value; OnOperationChanged(); } }
		public event EventHandler? OperationUpdated;

		private GOperation? operation;
		private bool changeInProgress;

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

			changeInProgress = true;
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
				txtRelEndX.Text = Fmt(Operation.AbsXEnd - Operation.AbsXStart);
				txtRelEndY.Text = Fmt(Operation.AbsYEnd - Operation.AbsYStart);
				txtRelEndZ.Text = Fmt(Operation.AbsZEnd - Operation.AbsZStart);
				txtRelCenterI.Text = Fmt(Operation.Line.I);
				txtRelCenterJ.Text = Fmt(Operation.Line.J);
				txtRelCenterK.Text = Fmt(Operation.Line.K);
				txtRelF.Text = Fmt(Operation.F);
				txtRelS.Text = Fmt(Operation.S);

				txtAbsStartX.Text = Fmt(Operation.AbsXStart);
				txtAbsStartY.Text = Fmt(Operation.AbsYStart);
				txtAbsStartZ.Text = Fmt(Operation.AbsZStart);
				txtAbsEndX.Text = Fmt(Operation.AbsXEnd);
				txtAbsEndY.Text = Fmt(Operation.AbsYEnd);
				txtAbsEndZ.Text = Fmt(Operation.AbsZEnd);
				txtAbsCenterI.Text = Operation.Line.I.HasValue ? Fmt(Operation.AbsI) : "";
				txtAbsCenterJ.Text = Operation.Line.J.HasValue ? Fmt(Operation.AbsJ) : "";
				txtAbsCenterK.Text = Operation.Line.K.HasValue ? Fmt(Operation.AbsK) : "";
				txtAbsF.Text = Fmt(Operation.Line.F);
				txtAbsS.Text = Fmt(Operation.Line.S);
			}
			changeInProgress = false;
		}

		private static string Fmt(float val) => val.ToString("0.000", CultureInfo.InvariantCulture);
		private static string Fmt(decimal? val) => val.HasValue ? val.Value.ToString("0.000", CultureInfo.InvariantCulture) : "";
		private static decimal? ValDecimal(string txt) => String.IsNullOrWhiteSpace(txt) ? null : Decimal.TryParse(txt, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var val) ? val : null;

		private void SetVal(object textboxSender, Action<decimal?> setter)
		{
			if (changeInProgress) return;
			changeInProgress = true;
			if (textboxSender is not TextBox textbox) return;

			if (String.IsNullOrWhiteSpace(textbox.Text))
			{
				textbox.BackColor = SystemColors.Window;
				setter(null);
				OnOperationChanged();
				OperationUpdated?.Invoke(this, EventArgs.Empty);
			}
			else
			{
				if (Decimal.TryParse(textbox.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var val))
				{
					textbox.BackColor = SystemColors.Window;
					setter(val);
					OnOperationChanged();
					OperationUpdated?.Invoke(this, EventArgs.Empty);
				}
				else
				{
					textbox.BackColor = Color.Red;
				}
			}
			changeInProgress = false;
		}

		private void txtRelEndX_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.X = value);
		private void txtRelEndY_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Y = value);
		private void txtRelEndZ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Z = value);
		private void txtRelCenterI_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.I = value);
		private void txtRelCenterJ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.J = value);
		private void txtRelCenterK_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.K = value);
		private void txtAbsEndX_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.X = value.HasValue ? value.Value - (decimal)Operation!.AbsXStart : null);
		private void txtAbsEndY_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Y = value.HasValue ? value.Value - (decimal)Operation!.AbsYStart : null);
		private void txtAbsEndZ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.Z = value.HasValue ? value.Value - (decimal)Operation!.AbsZStart : null);
		private void txtAbsCenterI_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.I = value.HasValue ? value.Value - (decimal)Operation!.AbsXStart : null);
		private void txtAbsCenterJ_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.J = value.HasValue ? value.Value - (decimal)Operation!.AbsYStart : null);
		private void txtAbsCenterK_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.K = value.HasValue ? value.Value - (decimal)Operation!.AbsZStart : null);
		private void txtAbsF_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.F = value);
		private void txtAbsS_TextChanged(object sender, EventArgs e) => SetVal(sender, value => Operation!.Line.S = value);
	}
}
