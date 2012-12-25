// 
// BaseSurface.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation
{
	public unsafe abstract class BaseSurface : ISurface
	{
		protected abstract ColorBgra* data { get; }
		protected int height;
		protected int width;

		#region ISurface Members
		public Rectangle Bounds { get { return new Rectangle (Point.Empty, Size); } }
		public int Height { get { return height; } }
		public Size Size { get { return new Size (height, width); } }
		public abstract int Stride { get; }
		public int Width { get { return width; } }

		public virtual void BeginUpdate ()
		{			
		}

		public virtual void EndUpdate ()
		{			
		}

		public unsafe ColorBgra GetPoint (int x, int y)
		{
			return *(data + (x + (y * width)));
		}

		public unsafe ColorBgra* GetPointAddress (int x, int y)
		{
			return data + (x + (y * width));
		}

		public unsafe ColorBgra* GetPointAddress (Point point)
		{
			return data + (point.X + (point.Y * width));
		}

		public unsafe ColorBgra* GetRowAddress (int y)
		{
			return data + (y * width);
		}
		#endregion
	}
}
