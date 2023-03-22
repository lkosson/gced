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
	public partial class VerticalLabel : Label
	{
		public override Size GetPreferredSize(Size proposedSize)
		{
			var original = base.GetPreferredSize(proposedSize);
			return new Size(original.Height, original.Width);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			using var brush = new SolidBrush(ForeColor);
			var textSize = GetPreferredSize(Size.Empty);
			var centerOrigin = new Point((Width - textSize.Width) / 2, (Height - textSize.Height) / 2);

			if (TextAlign == ContentAlignment.TopLeft || TextAlign == ContentAlignment.TopCenter || TextAlign == ContentAlignment.TopRight) e.Graphics.TranslateTransform(Width, 0);
			if (TextAlign == ContentAlignment.MiddleLeft || TextAlign == ContentAlignment.MiddleCenter || TextAlign == ContentAlignment.MiddleRight) e.Graphics.TranslateTransform(centerOrigin.X + textSize.Width, 0);
			if (TextAlign == ContentAlignment.BottomLeft || TextAlign == ContentAlignment.BottomCenter || TextAlign == ContentAlignment.BottomRight) e.Graphics.TranslateTransform(textSize.Width, 0);

			if (TextAlign == ContentAlignment.TopLeft || TextAlign == ContentAlignment.MiddleLeft || TextAlign == ContentAlignment.BottomLeft) e.Graphics.TranslateTransform(0, 0);
			if (TextAlign == ContentAlignment.TopCenter || TextAlign == ContentAlignment.MiddleCenter || TextAlign == ContentAlignment.BottomCenter) e.Graphics.TranslateTransform(0, centerOrigin.Y);
			if (TextAlign == ContentAlignment.TopRight || TextAlign == ContentAlignment.MiddleRight || TextAlign == ContentAlignment.BottomRight) e.Graphics.TranslateTransform(0, Height - textSize.Height);

			//e.Graphics.TranslateTransform((Width - textSize.Width) / 2 + textSize.Width, (Height - textSize.Height) / 2);
			e.Graphics.RotateTransform(90);
			e.Graphics.DrawString(Text, Font, brush, 0f, 0f);
		}
	}
}
