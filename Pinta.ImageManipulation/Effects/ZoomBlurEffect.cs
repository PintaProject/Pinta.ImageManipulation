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
	public class ZoomBlurEffect : BaseEffect
	{
		private int amount;
		private Point offset;

		/// <summary>
		/// Creates a new effect that will apply a zoom blur.
		/// </summary>
		/// <param name="amount">Amount of zoom to apply. Valid values are 0 - 200.</param>
		/// <param name="offset">Origin point of the zoom blur.</param>
		public ZoomBlurEffect (int amount = 10, Point offset = new Point ())
		{
			if (amount < 0 || amount > 200)
				throw new ArgumentOutOfRangeException ("amount");

			this.amount = amount;
			this.offset = offset;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			if (amount == 0) {
				// Copy src to dest
				return;
			}

			var src_bounds = src.Bounds;

			long w = dst.Width;
			long h = dst.Height;
			long fox = (long)(dst.Width * offset.X * 32768.0);
			long foy = (long)(dst.Height * offset.Y * 32768.0);
			long fcx = fox + (w << 15);
			long fcy = foy + (h << 15);
			long fz = amount;

			const int n = 64;

			for (int y = rect.Top; y <= rect.Bottom; ++y) {
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);
				ColorBgra* srcPtr = src.GetPointAddress (rect.Left, y);

				for (int x = rect.Left; x <= rect.Right; ++x) {
					long fx = (x << 16) - fcx;
					long fy = (y << 16) - fcy;

					int sr = 0;
					int sg = 0;
					int sb = 0;
					int sa = 0;
					int sc = 0;

					sr += srcPtr->R * srcPtr->A;
					sg += srcPtr->G * srcPtr->A;
					sb += srcPtr->B * srcPtr->A;
					sa += srcPtr->A;
					++sc;

					for (int i = 0; i < n; ++i) {
						fx -= ((fx >> 4) * fz) >> 10;
						fy -= ((fy >> 4) * fz) >> 10;

						int u = (int)(fx + fcx + 32768 >> 16);
						int v = (int)(fy + fcy + 32768 >> 16);

						if (src_bounds.Contains (u, v)) {
							ColorBgra* srcPtr2 = src.GetPointAddress (u, v);

							sr += srcPtr2->R * srcPtr2->A;
							sg += srcPtr2->G * srcPtr2->A;
							sb += srcPtr2->B * srcPtr2->A;
							sa += srcPtr2->A;
							++sc;
						}
					}

					if (sa != 0) {
						*dstPtr = ColorBgra.FromBgra (
							Utility.ClampToByte (sb / sa),
							Utility.ClampToByte (sg / sa),
							Utility.ClampToByte (sr / sa),
							Utility.ClampToByte (sa / sc));
					} else {
						dstPtr->Bgra = 0;
					}

					++srcPtr;
					++dstPtr;
				}
			}
		}
		#endregion
	}
}
