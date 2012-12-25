/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzysztof Marecki <marecki.krzysztof@gmail.com>         //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class SharpenEffect : LocalHistogramEffect
	{
		private int amount;

		public SharpenEffect (int amount)
		{
			if (amount < 1 || amount > 20)
				throw new ArgumentOutOfRangeException ("amount");

			this.amount = amount;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			RenderRect (amount, src, dest, rect);
		}

		public unsafe override ColorBgra Apply (ColorBgra src, int area, int* hb, int* hg, int* hr, int* ha)
		{
			var median = GetPercentile (50, area, hb, hg, hr, ha);
			return ColorBgra.Lerp (src, median, -0.5f);
		}
		#endregion
	}
}
