// 
// MultiplyBlendOpTest.cs
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
using NUnit.Framework;
using Pinta.ImageManipulation.PixelBlendOperations;

namespace Pinta.ImageManipulation.UnitTests.BlendOperations
{
	[TestFixture]
	public class MultiplyBlendOpTest : BaseTest
	{
		[Test]
		public void MultiplyBlendOp1 ()
		{
			var lhs = GetSourceImage ("blend1.png");
			var rhs = GetSourceImage ("blend2.png");

			var op = new MultiplyBlendOp ();
			op.Apply (lhs, rhs);

			Compare (rhs, "multiplyblend1.png");
		}

		[Test]
		public void MultiplyBlendOp2 ()
		{
			var lhs = GetSourceImage ("blend1.png");
			var rhs = GetSourceImage ("blend2.png");

			var op = new MultiplyBlendOp ();
			op.Apply (rhs, lhs);

			Compare (lhs, "multiplyblend2.png");
		}
	}
}
