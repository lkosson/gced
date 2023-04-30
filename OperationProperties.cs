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
		private TextBox? textBoxChangeInProgress;
		private bool operationChangeInProgress;

		public OperationProperties()
		{
			viewState = new ViewState();
			InitializeComponent();
			txtRelEndX.Tag = new OperationPropertyAccessors(operation => operation.AbsXEnd - operation.AbsXStart, (operation, value) => operation.Line.X = operation.Absolute ? (decimal)operation.AbsXStart + value : value);
			txtRelEndY.Tag = new OperationPropertyAccessors(operation => operation.AbsYEnd - operation.AbsYStart, (operation, value) => operation.Line.Y = operation.Absolute ? (decimal)operation.AbsYStart + value : value);
			txtRelEndZ.Tag = new OperationPropertyAccessors(operation => operation.AbsZEnd - operation.AbsZStart, (operation, value) => operation.Line.Z = operation.Absolute ? (decimal)operation.AbsZStart + value : value);
			txtRelCenterI.Tag = new OperationPropertyAccessors(operation => operation.Line.I, (operation, value) => operation.Line.I = value);
			txtRelCenterJ.Tag = new OperationPropertyAccessors(operation => operation.Line.J, (operation, value) => operation.Line.J = value);
			txtRelCenterK.Tag = new OperationPropertyAccessors(operation => operation.Line.K, (operation, value) => operation.Line.K = value);
			txtRelF.Tag = new OperationPropertyAccessors(operation => operation.Line.F);
			txtRelS.Tag = new OperationPropertyAccessors(operation => operation.Line.S);
			txtAbsStartX.Tag = new OperationPropertyAccessors(operation => operation.AbsXStart);
			txtAbsStartY.Tag = new OperationPropertyAccessors(operation => operation.AbsYStart);
			txtAbsStartZ.Tag = new OperationPropertyAccessors(operation => operation.AbsZStart);
			txtAbsEndX.Tag = new OperationPropertyAccessors(operation => operation.AbsXEnd, (operation, value) => operation.Line.X = value.HasValue ? operation.Absolute ? value : value - (decimal)operation.AbsXStart : null);
			txtAbsEndY.Tag = new OperationPropertyAccessors(operation => operation.AbsYEnd, (operation, value) => operation.Line.Y = value.HasValue ? operation.Absolute ? value : value - (decimal)operation.AbsYStart : null);
			txtAbsEndZ.Tag = new OperationPropertyAccessors(operation => operation.AbsZEnd, (operation, value) => operation.Line.Z = value.HasValue ? operation.Absolute ? value : value - (decimal)operation.AbsZStart : null);
			txtAbsCenterI.Tag = new OperationPropertyAccessors(operation => operation.Line.I.HasValue ? (decimal?)operation.AbsI : null, (operation, value) => operation.Line.I = value.HasValue ? value - (decimal)operation.AbsXStart : null);
			txtAbsCenterJ.Tag = new OperationPropertyAccessors(operation => operation.Line.J.HasValue ? (decimal?)operation.AbsJ : null, (operation, value) => operation.Line.J = value.HasValue ? value - (decimal)operation.AbsYStart : null);
			txtAbsCenterK.Tag = new OperationPropertyAccessors(operation => operation.Line.K.HasValue ? (decimal?)operation.AbsK : null, (operation, value) => operation.Line.K = value.HasValue ? value - (decimal)operation.AbsZStart : null);
			txtAbsF.Tag = new OperationPropertyAccessors(operation => operation.Line.F, (operation, value) => operation.Line.F = value);
			txtAbsS.Tag = new OperationPropertyAccessors(operation => operation.Line.S, (operation, value) => operation.Line.S = value);

			foreach (var textbox in tableLayoutPanel.Controls.Cast<Control>().OfType<TextBox>())
			{
				textbox.KeyDown += txtKeyDown;
				textbox.Validating += txtValidating;
				textbox.Validated += txtValidated;
			}
		}

		private void txtValidated(object? sender, EventArgs e)
		{
			if (sender is not TextBox textBox) return;
			if (textBox.Tag is not OperationPropertyAccessors accessors) return;
			if (operationChangeInProgress) return;
			if (textBoxChangeInProgress != null) return;
			if (ViewState.LastSelectedOperation == null) return;
			var valueChanged = false;
			textBoxChangeInProgress = textBox;

			var oldValue = accessors.Getter(ViewState.LastSelectedOperation);
			if (String.IsNullOrWhiteSpace(textBox.Text))
			{
				accessors.Setter(ViewState.LastSelectedOperation, null);
				valueChanged = oldValue != null;
			}
			else
			{
				Decimal.TryParse(textBox.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var newValue);
				valueChanged = oldValue == null || Math.Abs(oldValue.Value - newValue) > 0.00001m;
				if (valueChanged)
				{
					accessors.Setter(ViewState.LastSelectedOperation, newValue);
					textBox.Text = Fmt(newValue);
				}
			}
			if (!operationChangeInProgress && valueChanged)
			{
				ViewState.RunProgram();
			}
			textBoxChangeInProgress = null;
		}

		private void txtValidating(object? sender, CancelEventArgs e)
		{
			if (sender is not TextBox textBox) return;
			if (String.IsNullOrWhiteSpace(textBox.Text) || Decimal.TryParse(textBox.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var _))
			{
				textBox.BackColor = SystemColors.Window;
			}
			else
			{
				textBox.BackColor = Color.Red;
				e.Cancel = true;
			}
		}

		private void txtKeyDown(object? sender, KeyEventArgs e)
		{
			if (sender is not TextBox textBox) return;
			if (textBox.Tag is not OperationPropertyAccessors accessors) return;
			if (ViewState.LastSelectedOperation == null) return;
			if (e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				SelectNextControl(ActiveControl, true, true, true, true);
			}
			else if (e.KeyCode == Keys.Escape)
			{
				textBox.Text = Fmt(accessors.Getter(ViewState.LastSelectedOperation));
			}
		}

		private void ViewState_SelectedOperationsChanged()
		{
			var hasOperation = ViewState.LastSelectedOperation != null;
			var isRapid = ViewState.LastSelectedOperation != null && ViewState.LastSelectedOperation.Line.Instruction == GInstruction.G0;
			var isLine = ViewState.LastSelectedOperation != null && ViewState.LastSelectedOperation.Line.Instruction == GInstruction.G1;
			var isArc = ViewState.LastSelectedOperation != null && ViewState.LastSelectedOperation.Line.IsArc;
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
			txtAbsS.Enabled = hasOperation && isMove;

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
				foreach (var textBox in tableLayoutPanel.Controls.Cast<Control>().OfType<TextBox>())
				{
					textBox.Text = "";
				}
			}
			else
			{
				foreach (var textBox in tableLayoutPanel.Controls.Cast<Control>().OfType<TextBox>())
				{
					if (textBox == textBoxChangeInProgress) continue;
					if (textBox.Tag is not OperationPropertyAccessors accessors) continue;
					textBox.Text = Fmt(accessors.Getter(ViewState.LastSelectedOperation));
				}
			}
			operationChangeInProgress = false;
		}

		private static string Fmt(decimal? val) => val.HasValue ? val.Value.ToString("0.000", CultureInfo.InvariantCulture) : "";

		class OperationPropertyAccessors
		{
			public Func<GOperation, decimal?> Getter { get; init; }
			public Action<GOperation, decimal?> Setter { get; init; }

			public OperationPropertyAccessors(Func<GOperation, decimal?> getter, Action<GOperation, decimal?>? setter = null)
			{
				Getter = getter;
				Setter = setter ?? delegate { };
			}

			public OperationPropertyAccessors(Func<GOperation, float> getter, Action<GOperation, decimal?>? setter = null)
			{
				Getter = operation => (decimal)getter(operation);
				Setter = setter ?? delegate { };
			}
		}
	}
}
