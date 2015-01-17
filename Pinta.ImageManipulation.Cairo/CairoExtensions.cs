// 
// CairoExtensions.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2015 Jonathan Pobst
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

namespace Pinta.ImageManipulation
{
	public static class CairoExtensions
    {
        #region Points
        public static Cairo.Point ToCairoPoint (this Point point)
        {
            return new Cairo.Point (point.X, point.Y);
        }

        public static Cairo.PointD ToCairoPointD (this PointD point)
        {
            return new Cairo.PointD (point.X, point.Y);
        }

        public static Point ToPintaPoint (this Cairo.Point point)
        {
            return new Point (point.X, point.Y);
        }

        public static PointD ToPintaPointD (this Cairo.PointD point)
        {
            return new PointD (point.X, point.Y);
        }
        #endregion

        #region Rectangles
        public static Rectangle ToPintaRectangle (this Cairo.Rectangle r)
		{
			return new Rectangle ((int)Math.Floor (r.X), (int)Math.Floor (r.Y),
								  (int)Math.Ceiling (r.Width), (int)Math.Ceiling (r.Height));
		}

        public static Rectangle ToPintaRectangle (this Cairo.RectangleInt r)
        {
            return new Rectangle (r.X, r.Y, r.Width, r.Height);
        }

        public static RectangleD ToPintaRectangleD (this Cairo.Rectangle r)
        {
            return new RectangleD (r.X, r.Y, r.Width, r.Height);
        }

        public static Cairo.RectangleInt ToCairoRectangleInt (this Rectangle r)
        {
            var r2 = new Cairo.RectangleInt ();

            r2.X = r.X;
            r2.Y = r.Y;
            r2.Width = r.Width;
            r2.Height = r.Height;

            return r2;
        }

        public static Cairo.Rectangle ToCairoRectangle (this RectangleD r)
        {
            return new Cairo.Rectangle (r.X, r.Y, r.Width, r.Height);
        }
        #endregion

        #region Colors
        public static Cairo.Color ToCairoColor (this ColorBgra color)
        {
            return new Cairo.Color (color.R / 255d, color.G / 255d, color.B / 255d, color.A / 255d);
        }

		public static ColorBgra ToPintaColorBgra (this Cairo.Color color)
		{
			var c = new ColorBgra ();

			c.R = (byte)(color.R * 255);
			c.G = (byte)(color.G * 255);
			c.B = (byte)(color.B * 255);
			c.A = (byte)(color.A * 255);

			return c;
		}
        #endregion
	}
}
