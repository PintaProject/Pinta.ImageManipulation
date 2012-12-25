/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation.UnaryPixelOperations
{
	public class PosterizePixelOp : UnaryPixelOp
	{
		private byte[] red_levels;
		private byte[] green_levels;
		private byte[] blue_levels;

		public PosterizePixelOp (int red, int green, int blue)
		{
			this.red_levels = CalcLevels (red);
			this.green_levels = CalcLevels (green);
			this.blue_levels = CalcLevels (blue);
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			return ColorBgra.FromBgra (blue_levels[color.B], green_levels[color.G], red_levels[color.R], color.A);
		}

		public unsafe override void Apply (ColorBgra* ptr, int length)
		{
			while (length > 0) {
				ptr->B = blue_levels[ptr->B];
				ptr->G = green_levels[ptr->G];
				ptr->R = red_levels[ptr->R];

				++ptr;
				--length;
			}
		}

		public unsafe override void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				dst->B = blue_levels[src->B];
				dst->G = green_levels[src->G];
				dst->R = red_levels[src->R];
				dst->A = src->A;

				++dst;
				++src;
				--length;
			}
		}

		private static byte[] CalcLevels (int levelCount)
		{
			var t1 = new byte[levelCount];

			for (int i = 1; i < levelCount; i++)
				t1[i] = (byte)((255 * i) / (levelCount - 1));

			var levels = new byte[256];

			var j = 0;
			var k = 0;

			for (int i = 0; i < 256; i++) {
				levels[i] = t1[j];

				k += levelCount;

				if (k > 255) {
					k -= 255;
					j++;
				}
			}

			return levels;
		}
	}
}
