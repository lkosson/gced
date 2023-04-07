using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class CanvasItemBackground : CanvasItem
	{
		private Image originalImage;
		private Image scaledImage;
		private float scale;

		public CanvasItemBackground(GOperation operation)
			 : base(operation)
		{
			if (!String.IsNullOrEmpty(operation.Line.P)) originalImage = Bitmap.FromFile(operation.Line.P);
			else originalImage = new Bitmap(1, 1);
			scaledImage = new Bitmap(1, 1);
		}

		public override void OperationChanged()
		{
			base.OperationChanged();
			AbsBoundingBox = new RectangleF(Operation.AbsXStart, Operation.AbsYStart, Operation.AbsI - Operation.AbsXStart, Operation.AbsJ - Operation.AbsYStart);
		}

		public override void ViewMatrixChanged(Matrix viewMatrix)
		{
			base.ViewMatrixChanged(viewMatrix);
		}

		public override void Draw(Graphics g, CanvasStyle style)
		{
			if (style.PixelSize != scale)
			{
				scale = style.PixelSize;
				if (scaledImage != null) scaledImage.Dispose();
				var scaledWidth = (int)(Operation.AbsI / scale);
				var scaledHeight = (int)(Operation.AbsJ / scale);

				if (scaledWidth > originalImage.Width || scaledHeight > originalImage.Height)
				{
					scaledWidth = originalImage.Width;
					scaledHeight = originalImage.Height;
				}

				scaledImage = new Bitmap(scaledWidth, scaledHeight, PixelFormat.Format32bppPArgb);
				using var sg = Graphics.FromImage(scaledImage);
				using var ia = new ImageAttributes();
				ia.SetColorMatrix(new ColorMatrix { [3, 3] = (float)Operation.Line.S.GetValueOrDefault(100) / 100f });
				sg.InterpolationMode = InterpolationMode.HighQualityBicubic;
				sg.DrawImage(originalImage, new Rectangle(0, 0, scaledImage.Width, scaledImage.Height), 0, originalImage.Height, originalImage.Width, -originalImage.Height, GraphicsUnit.Pixel, ia);
			}
			g.DrawImage(scaledImage, Operation.AbsXStart, Operation.AbsYStart, Operation.AbsI - Operation.AbsXStart, Operation.AbsJ - Operation.AbsYStart);
		}

		public override void Dispose()
		{
			scaledImage.Dispose();
			originalImage.Dispose();
			base.Dispose();
		}
	}
}
