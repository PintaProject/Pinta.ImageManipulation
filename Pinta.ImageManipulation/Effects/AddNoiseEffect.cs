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
	public class AddNoiseEffect : BaseEffect
	{
		[ThreadStatic]
		private static Random threadRand;
		private const int tableSize = 16384;
		private static int[] lookup;

		private int intensity;
		private int color_saturation;
		private double coverage;

		int dev;
		int sat;

		/// <summary>
		/// Creates a new effect that will add noise to an image.
		/// </summary>
		/// <param name="intensity">The intensity of the effect. Valid range is 0 - 100.</param>
		/// <param name="colorSaturation">The color saturation of the effect. Valid range is 0 - 400.</param>
		/// <param name="coverage">The coverage density of the effect. Valid range is 0 - 100.</param>
		public AddNoiseEffect (int intensity = 64, int colorSaturation = 100, double coverage = 100.0)
		{
			if (intensity < 0 || intensity > 100)
				throw new ArgumentOutOfRangeException ("intensity");
			if (colorSaturation < 0 || colorSaturation > 400)
				throw new ArgumentOutOfRangeException ("colorSaturation");
			if (coverage < 0 || coverage > 100)
				throw new ArgumentOutOfRangeException ("coverage");

			this.intensity = intensity;
			this.color_saturation = colorSaturation;
			this.coverage = coverage * 0.01;

			dev = this.intensity * this.intensity / 4;
			sat = this.color_saturation * 4096 / 100;

		}

		static AddNoiseEffect ()
		{
			InitLookup ();
		}

		#region Algorithm Code Ported From PDN
		protected override unsafe void Render (ColorBgra* src, ColorBgra* dst, int length)
		{
			if (threadRand == null)
				threadRand = new Random ();

			var localLookup = lookup;

			for (var x = 0; x < length; ++x) {
				if (threadRand.NextDouble () > coverage) {
					*dst = *src;
				} else {
					int r;
					int g;
					int b;
					int i;

					r = localLookup[threadRand.Next (tableSize)];
					g = localLookup[threadRand.Next (tableSize)];
					b = localLookup[threadRand.Next (tableSize)];

					i = (4899 * r + 9618 * g + 1867 * b) >> 14;


					r = i + (((r - i) * sat) >> 12);
					g = i + (((g - i) * sat) >> 12);
					b = i + (((b - i) * sat) >> 12);

					dst->R = Utility.ClampToByte (src->R + ((r * dev + 32768) >> 16));
					dst->G = Utility.ClampToByte (src->G + ((g * dev + 32768) >> 16));
					dst->B = Utility.ClampToByte (src->B + ((b * dev + 32768) >> 16));
					dst->A = src->A;
				}

				++src;
				++dst;
			}
		}

		private static double NormalCurve (double x, double scale)
		{
			return scale * Math.Exp (-x * x / 2);
		}

		private static void InitLookup ()
		{
			double l = 5;
			double r = 10;
			double scale = 50;
			double sum = 0;

			while (r - l > 0.0000001) {
				sum = 0;
				scale = (l + r) * 0.5;

				for (var i = 0; i < tableSize; ++i) {
					sum += NormalCurve (16.0 * ((double)i - tableSize / 2) / tableSize, scale);

					if (sum > 1000000) {
						break;
					}
				}

				if (sum > tableSize) {
					r = scale;
				} else if (sum < tableSize) {
					l = scale;
				} else {
					break;
				}
			}

			lookup = new int[tableSize];
			sum = 0;
			int roundedSum = 0, lastRoundedSum;

			for (var i = 0; i < tableSize; ++i) {
				sum += NormalCurve (16.0 * ((double)i - tableSize / 2) / tableSize, scale);
				lastRoundedSum = roundedSum;
				roundedSum = (int)sum;

				for (var j = lastRoundedSum; j < roundedSum; ++j) {
					lookup[j] = (i - tableSize / 2) * 65536 / tableSize;
				}
			}
		}
		#endregion
	}
}
