// 
// MandelbrotFractalEffectTest.cs
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
using Pinta.ImageManipulation.Effects;

namespace Pinta.ImageManipulation.UnitTests.Effects
{
	[TestClass]
	public class MandelbrotFractalEffectTest : BaseTest
	{
		[TestMethod]
		public void MandelbrotFractalEffect1 ()
		{
			var src = GetSourceImage ("input.png");

			var effect = new MandelbrotFractalEffect ();
			effect.Render (src);

			Compare (src, "mandelbrotfractal1.png");
		}

		[TestMethod]
		public void MandelbrotFractalEffect2 ()
		{
			var src = GetSourceImage ("input.png");

			var effect = new MandelbrotFractalEffect (6, 4, 25, 90, true);
			effect.Render (src);

			Compare (src, "mandelbrotfractal2.png");
		}
	}
}
