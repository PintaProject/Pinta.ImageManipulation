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
	public class TwistEffect : BaseEffect
	{
		private int amount;
		private int antialias;

		public TwistEffect (int amount, int antialias)
		{
			if (amount < -100 || amount > 100)
				throw new ArgumentOutOfRangeException ("amount");
			if (antialias < 0 || antialias > 5)
				throw new ArgumentOutOfRangeException ("antialias");

			this.amount = amount;
			this.antialias = antialias;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			float twist = amount;

			float hw = dst.Width / 2.0f;
			float hh = dst.Height / 2.0f;
			float maxrad = Math.Min (hw, hh);

			twist = twist * twist * Math.Sign (twist);

			int aaLevel = antialias;
			int aaSamples = aaLevel * aaLevel + 1;
			PointD* aaPoints = stackalloc PointD[aaSamples];

			for (int i = 0; i < aaSamples; ++i) {
				PointD pt = new PointD (
					((i * aaLevel) / (float)aaSamples),
					i / (float)aaSamples);

				pt.X -= (int)pt.X;
				aaPoints[i] = pt;
			}

			for (int y = rect.Top; y <= rect.Bottom; y++) {
				float j = y - hh;
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);
				ColorBgra* srcPtr = src.GetPointAddress (rect.Left, y);

				for (int x = rect.Left; x <= rect.Right; x++) {
					float i = x - hw;

					if (i * i + j * j > (maxrad + 1) * (maxrad + 1)) {
						*dstPtr = *srcPtr;
					} else {
						int b = 0;
						int g = 0;
						int r = 0;
						int a = 0;

						for (int p = 0; p < aaSamples; ++p) {
							float u = i + (float)aaPoints[p].X;
							float v = j + (float)aaPoints[p].Y;
							double rad = Math.Sqrt (u * u + v * v);
							double theta = Math.Atan2 (v, u);

							double t = 1 - rad / maxrad;

							t = (t < 0) ? 0 : (t * t * t);

							theta += (t * twist) / 100;

							ColorBgra sample = src.GetPoint (
								(int)(hw + (float)(rad * Math.Cos (theta))),
								(int)(hh + (float)(rad * Math.Sin (theta))));

							b += sample.B;
							g += sample.G;
							r += sample.R;
							a += sample.A;
						}

						*dstPtr = ColorBgra.FromBgra (
							(byte)(b / aaSamples),
							(byte)(g / aaSamples),
							(byte)(r / aaSamples),
							(byte)(a / aaSamples));
					}

					++dstPtr;
					++srcPtr;
				}
			}
		}
		#endregion
	}
}
