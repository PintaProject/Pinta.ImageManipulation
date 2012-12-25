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
	public class MedianEffect : LocalHistogramEffect
	{
		private int radius;
		private int percentile;

		public MedianEffect (int radius, int percentile)
		{
			if (radius < 1 || radius > 200)
				throw new ArgumentOutOfRangeException ("radius");
			if (percentile < 0 || percentile > 100)
				throw new ArgumentOutOfRangeException ("percentile");

			this.radius = radius;
			this.percentile = percentile;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			RenderRect (radius, src, dest, rect);
		}

		public unsafe override ColorBgra Apply (ColorBgra src, int area, int* hb, int* hg, int* hr, int* ha)
		{
			ColorBgra c = GetPercentile (percentile, area, hb, hg, hr, ha);
			return c;
		}
		#endregion
	}
}
