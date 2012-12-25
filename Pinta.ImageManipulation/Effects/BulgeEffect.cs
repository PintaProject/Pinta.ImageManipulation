/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Olivier Dufour <olivier.duff@gmail.com>                 //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class BulgeEffect : BaseEffect
	{
		private int amount;
		private PointD offset;

		/// <summary>
		/// Creates a new effect that will add a bulge to an image at a specified point.
		/// </summary>
		/// <param name="amount">Amount to bulge. Valid range is -200 - 100.</param>
		/// <param name="offset">Bulge origin point.</param>
		public BulgeEffect (int amount = 45, PointD offset = new PointD ())
		{
			if (amount < -200 || amount > 100)
				throw new ArgumentOutOfRangeException ("amount");

			this.amount = amount;
			this.offset = offset;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			var bulge = (float)amount;

			var hw = dst.Width / 2f;
			var hh = dst.Height / 2f;
			var maxrad = Math.Min (hw, hh);
			var amt = bulge / 100f;

			hh = hh + (float)offset.Y * hh;
			hw = hw + (float)offset.X * hw;

			for (var y = rect.Top; y <= rect.Bottom; y++) {
				var dstPtr = dst.GetPointAddress (rect.Left, y);
				var srcPtr = src.GetPointAddress (rect.Left, y);
				var v = y - hh;

				for (var x = rect.Left; x <= rect.Right; x++) {
					var u = x - hw;
					var r = (float)Math.Sqrt (u * u + v * v);
					var rscale1 = (1f - (r / maxrad));

					if (rscale1 > 0) {
						var rscale2 = 1 - amt * rscale1 * rscale1;

						var xp = u * rscale2;
						var yp = v * rscale2;

						*dstPtr = Utility.GetBilinearSampleClamped (src, xp + hw, yp + hh);
					} else {
						*dstPtr = *srcPtr;
					}

					++dstPtr;
					++srcPtr;
				}
			}
		}
		#endregion
	}
}
