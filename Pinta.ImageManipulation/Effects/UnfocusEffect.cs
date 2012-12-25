/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Hanh Pham <hanh.pham@gmx.com>                           //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class UnfocusEffect : LocalHistogramEffect
	{
		private int radius;

		public UnfocusEffect (int radius)
		{
			if (radius < 1 || radius > 200)
				throw new ArgumentOutOfRangeException ("radius");

			this.radius = radius;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			RenderRectWithAlpha (radius, src, dest, rect);
		}

		public unsafe override ColorBgra ApplyWithAlpha (ColorBgra src, int area, int sum, int* hb, int* hg, int* hr)
		{
			//each slot of the histgram can contain up to area * 255. This will overflow an int when area > 32k
			if (area < 32768) {
				int b = 0;
				int g = 0;
				int r = 0;

				for (int i = 1; i < 256; ++i) {
					b += i * hb[i];
					g += i * hg[i];
					r += i * hr[i];
				}

				int alpha = sum / area;
				int div = area * 255;

				return ColorBgra.FromBgraClamped (b / div, g / div, r / div, alpha);
			} else {	//use a long if an int will overflow.
				long b = 0;
				long g = 0;
				long r = 0;

				for (long i = 1; i < 256; ++i) {
					b += i * hb[i];
					g += i * hg[i];
					r += i * hr[i];
				}

				int alpha = sum / area;
				int div = area * 255;

				return ColorBgra.FromBgraClamped (b / div, g / div, r / div, alpha);
			}
		}
		#endregion
	}
}
