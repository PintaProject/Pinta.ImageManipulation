// 
// BitmapWrapper.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2012 Jonathan Pobst
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Drawing.Imaging;
using System.Threading;

namespace Pinta.ImageManipulation
{
	public class BitmapWrapper : BaseSurface
	{
		private System.Drawing.Bitmap surface;
		private BitmapData bitmap_data;
		private unsafe ColorBgra* data_ptr;
		private int lock_count = 0;

		public unsafe BitmapWrapper (System.Drawing.Bitmap surface)
		{
			this.surface = surface;
			height = surface.Height;
			width = surface.Width;
		}

		protected unsafe override ColorBgra* data {
			get { return data_ptr; }
		}

		public override int Stride {
			get { return bitmap_data.Stride; }
		}

		public unsafe override void BeginUpdate ()
		{
			Interlocked.Increment (ref lock_count);

			if (lock_count > 1)
				return;

			bitmap_data = surface.LockBits (new System.Drawing.Rectangle (0, 0, surface.Width, surface.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			data_ptr = (ColorBgra*)bitmap_data.Scan0;
		}

		public override void EndUpdate ()
		{
			Interlocked.Decrement (ref lock_count);

			if (lock_count == 0)
				surface.UnlockBits (bitmap_data);
		}
	}
}
