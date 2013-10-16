// 
// Rectangle.cs
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
	public struct RectangleD
	{
		public double X;
		public double Y;
		public double Height;
		public double Width;

		public static readonly RectangleD Empty;

		public RectangleD (double x, double y, double width, double height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public RectangleD (Point location, Size size) : this (location.X, location.Y, size.Width, size.Height)
		{
		}

		public double Bottom { get { return Y + Height - 1; } }
		public double Left { get { return X; } }
		public PointD Location { get { return new PointD (X, Y); } }
		public double Right { get { return X + Width - 1; } }
		public SizeD Size { get { return new SizeD (Height, Width); } }
		public double Top { get { return Y; } }

		public bool Contains (int x, int y)
		{
			return ((x >= Left) && (x < Right) && (y >= Top) && (y < Bottom));
		}

		public void Intersect (RectangleD r)
		{
			var new_r = RectangleD.Intersect (this, r);

			X = new_r.X;
			Y = new_r.Y;
			Width = new_r.Width;
			Height = new_r.Height;
		}

		public static RectangleD Intersect (RectangleD a, RectangleD b)
		{
			return RectangleD.FromLTRB (
				Math.Max (a.Left, b.Left),
				Math.Max (a.Top, b.Top),
				Math.Min (a.Right, b.Right),
				Math.Min (a.Bottom, b.Bottom));
		}

		public override string ToString ()
		{
			return String.Format ("{{X={0},Y={1},Width={2},Height={3}}}", X, Y, Width, Height);
		}

		public static RectangleD FromLTRB (double left, double top, double right, double bottom)
		{
			return new RectangleD (left, top, right - left + 1,
			bottom - top + 1);
		}

		public static bool operator == (RectangleD r1, RectangleD r2)
		{
			return r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height;
		}

		public static bool operator != (RectangleD r1, RectangleD r2)
		{
			return !(r1 == r2);
		}
	}
}
