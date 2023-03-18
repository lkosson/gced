using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCEd
{
	public partial class Canvas : UserControl
	{
		public int PaintTime { get; set; }
		public int FrameCount { get; set; }

		private Matrix viewMatrix;
		private Point mouseDragStart;
		private MouseButtons mouseDragButton;

		private IEnumerable<GOperation> ops;

		public Canvas()
		{
			DoubleBuffered = true;
			viewMatrix = new Matrix();
			var p = new GProgram();
			p.Read("test.nc");
			ops = p.Run();
		}

		protected override void OnLoad(EventArgs e)
		{
			viewMatrix.Scale(1f, -1f);
			viewMatrix.Translate(0f, -Height);
			base.OnLoad(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var sw = Stopwatch.StartNew();

			//e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);
			e.Graphics.MultiplyTransform(viewMatrix);

			foreach (var op in ops)
			{
				if (op.Line.Instruction == GInstruction.G0)
				{
					e.Graphics.DrawLine(Pens.LightBlue, op.AbsXStart, op.AbsYStart, op.AbsXEnd, op.AbsYEnd);
				}
				else if (op.Line.Instruction == GInstruction.G1)
				{
					e.Graphics.DrawLine(Pens.Blue, op.AbsXStart, op.AbsYStart, op.AbsXEnd, op.AbsYEnd);
				}
				else if (op.Line.Instruction == GInstruction.G2 || op.Line.Instruction == GInstruction.G3)
				{
					var dxs = op.AbsXStart - op.AbsI;
					var dys = op.AbsYStart - op.AbsJ;
					var dxe = op.AbsXEnd - op.AbsI;
					var dye = op.AbsYEnd - op.AbsJ;
					var r = (float)Math.Sqrt(dxs * dxs + dys * dys);
					var ths = (float)(Math.Atan2(dxs, dys) * 180 / Math.PI);
					var the = (float)(Math.Atan2(dxe, dye) * 180 / Math.PI);
					if (ths < 0) ths += 360;
					if (op.Line.Instruction == GInstruction.G2 && the < ths) the += 360;
					ths = 90 - ths;
					the = 90 - the;
					if (op.Line.Instruction == GInstruction.G2)
					{
						e.Graphics.DrawArc(Pens.Blue, op.AbsI - r, op.AbsJ - r, 2 * r, 2 * r, ths, the - ths);
					}
					else
					{
						e.Graphics.DrawArc(Pens.Blue, op.AbsI - r, op.AbsJ - r, 2 * r, 2 * r, ths, the - ths);
					}
				}

			}

			sw.Stop();
			PaintTime += (int)sw.ElapsedMilliseconds;
			FrameCount++;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			var scaleMatrix = new Matrix();

			scaleMatrix.Translate(e.X, e.Y);
			if (e.Delta > 0) scaleMatrix.Scale(1.1f, 1.1f);
			else scaleMatrix.Scale(1 / 1.1f, 1 / 1.1f);
			scaleMatrix.Translate(-e.X, -e.Y);

			scaleMatrix.Multiply(viewMatrix);

			viewMatrix = scaleMatrix;

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			mouseDragStart = e.Location;
			mouseDragButton = e.Button;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseDragButton = MouseButtons.None;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if ((mouseDragButton & MouseButtons.Left) == MouseButtons.Left)
			{
				var translateMatrix = new Matrix();
				translateMatrix.Translate(e.X - mouseDragStart.X, e.Y - mouseDragStart.Y);
				translateMatrix.Multiply(viewMatrix);
				viewMatrix = translateMatrix;

				mouseDragStart = e.Location;
				Invalidate();
			}
		}
	}
}
