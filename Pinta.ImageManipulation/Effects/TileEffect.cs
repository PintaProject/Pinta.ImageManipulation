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
	public class TileEffect : BaseEffect
	{
		private double rotation;
		private int tile_size;
		private int intensity;

		/// <summary>
		/// Creates a new effect that will apply a tile effect to an image.
		/// </summary>
		/// <param name="rotation">Angle to rotate the tiles.</param>
		/// <param name="tileSize">Size of the tiles. Valid range is 2 - 200.</param>
		/// <param name="intensity">Intensity of the tiling effect. Valid range is -20 - 20.</param>
		public TileEffect (double rotation = 30, int tileSize = 40, int intensity = 8)
		{
			if (tileSize < 2 || tileSize > 200)
				throw new ArgumentOutOfRangeException ("tileSize");
			if (intensity < -20 || intensity > 20)
				throw new ArgumentOutOfRangeException ("intensity");

			this.rotation = rotation;
			this.tile_size = tileSize;
			this.intensity = intensity;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			int width = dst.Width;
			int height = dst.Height;
			float hw = width / 2f;
			float hh = height / 2f;
			float sin = (float)Math.Sin (rotation * Math.PI / 180.0);
			float cos = (float)Math.Cos (rotation * Math.PI / 180.0);
			float scale = (float)Math.PI / tile_size;
            float intensity = this.intensity;

			intensity = intensity * intensity / 10 * Math.Sign (intensity);

			int aaLevel = 4;
			int aaSamples = aaLevel * aaLevel + 1;
			PointD* aaPoints = stackalloc PointD[aaSamples];

			for (int i = 0; i < aaSamples; ++i) {
				double x = (i * aaLevel) / (double)aaSamples;
				double y = i / (double)aaSamples;

				x -= (int)x;

				// RGSS + rotation to maximize AA quality
				aaPoints[i] = new PointD ((double)(cos * x + sin * y), (double)(cos * y - sin * x));
			}

			for (int y = rect.Top; y <= rect.Bottom; y++) {
				float j = y - hh;
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

				for (int x = rect.Left; x <= rect.Right; x++) {
					int b = 0;
					int g = 0;
					int r = 0;
					int a = 0;
					float i = x - hw;

					for (int p = 0; p < aaSamples; ++p) {
						PointD pt = aaPoints[p];

						float u = i + (float)pt.X;
						float v = j - (float)pt.Y;

						float s = cos * u + sin * v;
						float t = -sin * u + cos * v;

						s += intensity * (float)Math.Tan (s * scale);
						t += intensity * (float)Math.Tan (t * scale);
						u = cos * s - sin * t;
						v = sin * s + cos * t;

						int xSample = (int)(hw + u);
						int ySample = (int)(hh + v);

						xSample = (xSample + width) % width;
						// This makes it a little faster
						if (xSample < 0) {
							xSample = (xSample + width) % width;
						}

						ySample = (ySample + height) % height;
						// This makes it a little faster
						if (ySample < 0) {
							ySample = (ySample + height) % height;
						}

						ColorBgra sample = *src.GetPointAddress (xSample, ySample);

						b += sample.B;
						g += sample.G;
						r += sample.R;
						a += sample.A;
					}

					*(dstPtr++) = ColorBgra.FromBgra ((byte)(b / aaSamples), (byte)(g / aaSamples),
						(byte)(r / aaSamples), (byte)(a / aaSamples));
				}
			}
		}
		#endregion
	}
}
