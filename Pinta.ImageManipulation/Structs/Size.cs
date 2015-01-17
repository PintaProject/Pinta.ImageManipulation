// 
// Size.cs
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

// Uses code from Mono's System.Drawing under the MIT X11 license:
// https://github.com/mono/mono/tree/master/mcs/class/System.Drawing

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation
{
	public struct Size
	{
		public int Height { get; set; }
		public int Width { get; set; }

		public Size (int height, int width) : this ()
		{
			Height = height;
			Width = width;
		}

		public static bool operator == (Size s1, Size s2)
		{
			return s1.Width == s2.Width && s1.Height == s2.Height;
		}

		public static bool operator != (Size s1, Size s2)
		{
			return !(s1 == s2);
		}

		public override string ToString ()
		{
			return string.Format ("{{Width = {0} Height = {1}}}", Width, Height);
		}
	}
}
