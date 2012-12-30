// 
// BaseTest.cs
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;

namespace Pinta.ImageManipulation.UnitTests
{
	public abstract class BaseTest
	{
		protected Bitmap GetSourceImage (string name)
		{
			var bitmap = new Bitmap (Path.Combine ("SourceBitmaps", name));

			return bitmap;
		}

		protected Bitmap GetExpectedImage (string name)
		{
			var bitmap = new Bitmap (Path.Combine ("ExpectedBitmaps", name));

			return bitmap;
		}

		protected unsafe void Compare (Bitmap actual, string expected)
		{
			if (!File.Exists (Path.Combine ("ExpectedBitmaps", expected))) {
				actual.Save (Path.Combine (@"C:\Users\Jonathan\Desktop\Bitmaps", expected));
				return;
			}

			var exp_img = GetExpectedImage (expected);

			var act_data = actual.LockBits (new System.Drawing.Rectangle (0, 0, actual.Width, actual.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			var exp_data = exp_img.LockBits (new System.Drawing.Rectangle (0, 0, actual.Width, actual.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			var act_ptr = (ColorBgra*)act_data.Scan0;
			var exp_ptr = (ColorBgra*)exp_data.Scan0;

			var diff = 0;

			for (var i = 0; i < actual.Width * actual.Height; i++)
				if (*(act_ptr++) != *(exp_ptr++))
					diff++;

			actual.UnlockBits (act_data);
			exp_img.UnlockBits (exp_data);

			Assert.AreEqual (0, diff);
		}
	}
}
