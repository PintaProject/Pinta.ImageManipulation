// 
// LightenBlendOpTest.cs
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
	public class LightenBlendOpTest : BaseTest
	{
		[TestMethod]
		public void LightenBlendOp1 ()
		{
			var lhs = GetSourceImage ("blend1.png");
			var rhs = GetSourceImage ("blend2.png");

			var lhs_wrap = new BitmapWrapper (lhs);
			var rhs_wrap = new BitmapWrapper (rhs);

			var op = new LightenBlendOp ();
			op.Apply (lhs_wrap, rhs_wrap);

			Compare (rhs, "lightenblend1.png");
		}

		[TestMethod]
		public void LightenBlendOp2 ()
		{
			var lhs = GetSourceImage ("blend1.png");
			var rhs = GetSourceImage ("blend2.png");

			var lhs_wrap = new BitmapWrapper (lhs);
			var rhs_wrap = new BitmapWrapper (rhs);

			var op = new LightenBlendOp ();
			op.Apply (rhs_wrap, lhs_wrap);

			Compare (lhs, "lightenblend2.png");
		}
	}
}
