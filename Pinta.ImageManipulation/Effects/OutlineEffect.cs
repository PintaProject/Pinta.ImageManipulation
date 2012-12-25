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
	public class OutlineEffect : LocalHistogramEffect
	{
		private int thickness;
		private int intensity;

		public OutlineEffect (int thickness, int intensity)
		{
			if (thickness < 1 || thickness > 200)
				throw new ArgumentOutOfRangeException ("thickness");
			if (intensity < 0 || intensity > 100)
				throw new ArgumentOutOfRangeException ("intensity");

			this.thickness = thickness;
			this.intensity = intensity;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			RenderRect (thickness, src, dest, rect);
		}

		public unsafe override ColorBgra Apply (ColorBgra src, int area, int* hb, int* hg, int* hr, int* ha)
		{
			int minCount1 = area * (100 - this.intensity) / 200;
			int minCount2 = area * (100 + this.intensity) / 200;

			int bCount = 0;
			int b1 = 0;

			while (b1 < 255 && hb[b1] == 0)
				++b1;

			while (b1 < 255 && bCount < minCount1) {
				bCount += hb[b1];
				++b1;
			}

			int b2 = b1;
			while (b2 < 255 && bCount < minCount2) {
				bCount += hb[b2];
				++b2;
			}

			int gCount = 0;
			int g1 = 0;
			while (g1 < 255 && hg[g1] == 0)
				++g1;

			while (g1 < 255 && gCount < minCount1) {
				gCount += hg[g1];
				++g1;
			}

			int g2 = g1;
			while (g2 < 255 && gCount < minCount2) {
				gCount += hg[g2];
				++g2;
			}

			int rCount = 0;
			int r1 = 0;
			while (r1 < 255 && hr[r1] == 0)
				++r1;

			while (r1 < 255 && rCount < minCount1) {
				rCount += hr[r1];
				++r1;
			}

			int r2 = r1;
			while (r2 < 255 && rCount < minCount2) {
				rCount += hr[r2];
				++r2;
			}

			int aCount = 0;
			int a1 = 0;
			while (a1 < 255 && hb[a1] == 0)
				++a1;

			while (a1 < 255 && aCount < minCount1) {
				aCount += ha[a1];
				++a1;
			}

			int a2 = a1;
			while (a2 < 255 && aCount < minCount2) {
				aCount += ha[a2];
				++a2;
			}

			return ColorBgra.FromBgra (
				(byte)(255 - (b2 - b1)),
				(byte)(255 - (g2 - g1)),
				(byte)(255 - (r2 - r1)),
				(byte)(a2));
		}
		#endregion
	}
}
