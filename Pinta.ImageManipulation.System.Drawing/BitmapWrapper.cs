using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace Pinta.ImageManipulation
{
	public class BitmapWrapper : BaseSurface
	{
		private System.Drawing.Bitmap surface;
		private BitmapData bitmap_data;
		private unsafe ImageManipulation.ColorBgra* data_ptr;
		private int lock_count = 0;

		public unsafe BitmapWrapper (System.Drawing.Bitmap surface)
		{
			this.surface = surface;
			height = surface.Height;
			width = surface.Width;
		}

		protected unsafe override ImageManipulation.ColorBgra* data {
			get { return data_ptr; }
		}

		public override int Stride {
			get { return bitmap_data.Stride; }
		}

		public unsafe override void BeginUpdate ()
		{
			lock_count++;

			if (lock_count > 1)
				return;

			bitmap_data = surface.LockBits (new System.Drawing.Rectangle (0, 0, surface.Width, surface.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			data_ptr = (ImageManipulation.ColorBgra*)bitmap_data.Scan0;
		}

		public override void EndUpdate ()
		{
			lock_count--;

			if (lock_count == 0)
				surface.UnlockBits (bitmap_data);
		}
	}
}
