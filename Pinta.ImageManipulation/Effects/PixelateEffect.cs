/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Marco Rolappe <m_rolappe@gmx.net>                       //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class PixelateEffect : BaseEffect
	{
		private int cell_size;

		public PixelateEffect (int cellSize)
		{
			if (cellSize < 0 || cellSize > 100)
				throw new ArgumentOutOfRangeException ("cellSize");

			this.cell_size = cellSize;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			for (int y = rect.Top; y <= rect.Bottom; ++y) {
				int yEnd = y + 1;

				for (int x = rect.Left; x <= rect.Right; ++x) {
					var cellRect = GetCellBox (x, y, cell_size);
					cellRect.Intersect (dest.Bounds);
					var color = ComputeCellColor (x, y, src, cell_size, src.Bounds);

					int xEnd = Math.Min (rect.Right, cellRect.Right);
					yEnd = Math.Min (rect.Bottom, cellRect.Bottom);

					for (int y2 = y; y2 <= yEnd; ++y2) {
						ColorBgra* ptr = dest.GetPointAddress (x, y2);

						for (int x2 = x; x2 <= xEnd; ++x2) {
							ptr->Bgra = color.Bgra;
							++ptr;
						}
					}

					x = xEnd;
				}

				y = yEnd;
			}
		}

		private ColorBgra ComputeCellColor (int x, int y, ISurface src, int cellSize, Rectangle srcBounds)
		{
			var cell = GetCellBox (x, y, cellSize);
			cell.Intersect (srcBounds);

			var left = cell.Left;
			var right = cell.Right;
			var bottom = cell.Bottom;
			var top = cell.Top;

			var colorTopLeft = src.GetPoint (left, top);
			var colorTopRight = src.GetPoint (right, top);
			var colorBottomLeft = src.GetPoint (left, bottom);
			var colorBottomRight = src.GetPoint (right, bottom);

			var c = ColorBgra.BlendColors4W16IP (colorTopLeft, 16384, colorTopRight, 16384, colorBottomLeft, 16384, colorBottomRight, 16384);

			return c;
		}

		private Rectangle GetCellBox (int x, int y, int cellSize)
		{
			int widthBoxNum = x % cellSize;
			int heightBoxNum = y % cellSize;
			var leftUpper = new Point (x - widthBoxNum, y - heightBoxNum);

			var returnMe = new Rectangle (leftUpper, new Size (cellSize, cellSize));

			return returnMe;
		}
		#endregion
	}
}
