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
	public class ReduceNoiseEffect : LocalHistogramEffect
	{
		private int radius;
		private double strength;

		public ReduceNoiseEffect (int radius, double strength)
		{
			if (radius < 1 || radius > 200)
				throw new ArgumentOutOfRangeException ("radius");
			if (strength < 0 || strength > 1)
				throw new ArgumentOutOfRangeException ("strength");

			this.radius = radius;
			this.strength = -0.2 * strength;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			RenderRect (radius, src, dest, rect);
		}

		public override unsafe ColorBgra Apply (ColorBgra color, int area, int* hb, int* hg, int* hr, int* ha)
		{
			var normalized = GetPercentileOfColor (color, area, hb, hg, hr, ha);
			var lerp = strength * (1 - 0.75 * color.GetIntensity ());

			return ColorBgra.Lerp (color, normalized, lerp);
		}

		private static unsafe ColorBgra GetPercentileOfColor (ColorBgra color, int area, int* hb, int* hg, int* hr, int* ha)
		{
			int rc = 0;
			int gc = 0;
			int bc = 0;

			for (int i = 0; i < color.R; ++i)
				rc += hr[i];

			for (int i = 0; i < color.G; ++i)
				gc += hg[i];

			for (int i = 0; i < color.B; ++i)
				bc += hb[i];

			rc = (rc * 255) / area;
			gc = (gc * 255) / area;
			bc = (bc * 255) / area;

			return ColorBgra.FromBgr ((byte)bc, (byte)gc, (byte)rc);
		}
		#endregion
	}
}
