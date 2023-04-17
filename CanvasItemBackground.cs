using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class CanvasItemBackground : CanvasItem
	{
		private Image originalImage = default!;
		private Image scaledImage = default!;
		private float scale;
		private float width;
		private float height;

		public CanvasItemBackground(GOperation operation)
			 : base(operation)
		{
		}

		public override void OperationChanged()
		{
			base.OperationChanged();

			if (!String.IsNullOrEmpty(Operation.Line.P) && File.Exists(Operation.Line.P)) originalImage = Bitmap.FromFile(Operation.Line.P);
			else originalImage = new Bitmap(1, 1);
			scaledImage = new Bitmap(1, 1);

			var initialScale = (float)Operation.Line.K.GetValueOrDefault(1);

			if (Operation.Line.I.HasValue) width = (float)Operation.Line.I.Value;
			else if (Operation.Line.J.HasValue) width = originalImage.Width * (float)Operation.Line.J / originalImage.Height;
			else width = originalImage.Width * initialScale / originalImage.HorizontalResolution * 25.4f;

			if (Operation.Line.J.HasValue) height = (float)Operation.Line.J.Value;
			else height = originalImage.Height * width / originalImage.Width;
			AbsBoundingBox = new RectangleF((float)Operation.Line.X.GetValueOrDefault(), (float)Operation.Line.Y.GetValueOrDefault(), width, height);
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
				var scaledWidth = (int)(width / scale);
				var scaledHeight = (int)(height / scale);

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
			g.DrawImage(scaledImage, (float)Operation.Line.X.GetValueOrDefault(), (float)Operation.Line.Y.GetValueOrDefault(), width, height);
		}

		public override void Dispose()
		{
			scaledImage.Dispose();
			originalImage.Dispose();
			base.Dispose();
		}
	}
}
