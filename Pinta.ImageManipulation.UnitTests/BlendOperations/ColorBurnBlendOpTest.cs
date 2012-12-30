// 
// ColorBurnBlendOpTest.cs
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinta.ImageManipulation.PixelBlendOperations;

namespace Pinta.ImageManipulation.UnitTests.BlendOperations
{
	[TestClass]
	public class ColorBurnBlendOpTest : BaseTest
	{
		[TestMethod]
		public void ColorBurnBlendOp1 ()
		{
			var lhs = GetSourceImage ("blend1.png");
			var rhs = GetSourceImage ("blend2.png");

			var lhs_wrap = new BitmapWrapper (lhs);
			var rhs_wrap = new BitmapWrapper (rhs);

			var op = new ColorBurnBlendOp ();
			op.Apply (lhs_wrap, rhs_wrap);

			Compare (rhs, "colorburnblend1.png");
		}

		[TestMethod]
		public void ColorBurnBlendOp2 ()
		{
			var lhs = GetSourceImage ("blend1.png");
			var rhs = GetSourceImage ("blend2.png");

			var lhs_wrap = new BitmapWrapper (lhs);
			var rhs_wrap = new BitmapWrapper (rhs);

			var op = new ColorBurnBlendOp ();
			op.Apply (rhs_wrap, lhs_wrap);

			Compare (lhs, "colorburnblend2.png");
		}
	}
}
