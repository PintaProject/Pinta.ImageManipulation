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
	public class JuliaFractalEffect : BaseEffect
	{
		private int factor;
		private int quality;
		private int zoom;
		private double angle;

		private static readonly double log2_10000 = Math.Log (10000);

		/// <summary>
		/// Creates a new effect that will draw a Julia fractal.
		/// </summary>
		/// <param name="factor">Factor to use. Valid range is 1 - 10.</param>
		/// <param name="quality">Quality of the fractal. Valid range is 1 - 5.</param>
		/// <param name="zoom">Size of the fractal. Valid range is 0 - 50.</param>
		/// <param name="angle">Angle of the fractal to render.</param>
		public JuliaFractalEffect (int factor = 4, int quality = 2, int zoom = 1, double angle = 0)
		{
			if (factor < 1 || factor > 10)
				throw new ArgumentOutOfRangeException ("factor");
			if (quality < 1 || quality > 5)
				throw new ArgumentOutOfRangeException ("quality");
			if (zoom < 0 || zoom > 50)
				throw new ArgumentOutOfRangeException ("zoom");

			this.factor = factor;
			this.quality = quality;
			this.zoom = zoom;
			this.angle = angle;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			const double jr = 0.3125;
			const double ji = 0.03;

			int w = dst.Width;
			int h = dst.Height;
			double invH = 1.0 / h;
			double invZoom = 1.0 / zoom;
			double invQuality = 1.0 / quality;
			double aspect = (double)h / (double)w;
			int count = quality * quality + 1;
			double invCount = 1.0 / (double)count;
			double angleTheta = (angle * Math.PI * 2) / 360.0;

			for (int y = rect.Top; y <= rect.Bottom; y++) {
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

				for (int x = rect.Left; x <= rect.Right; x++) {
					int r = 0;
					int g = 0;
					int b = 0;
					int a = 0;

					for (double i = 0; i < count; i++) {
						double u = (2.0 * x - w + (i * invCount)) * invH;
						double v = (2.0 * y - h + ((i * invQuality) % 1)) * invH;

						double radius = Math.Sqrt ((u * u) + (v * v));
						double radiusP = radius;
						double theta = Math.Atan2 (v, u);
						double thetaP = theta + angleTheta;

						double uP = radiusP * Math.Cos (thetaP);
						double vP = radiusP * Math.Sin (thetaP);

						double jX = (uP - vP * aspect) * invZoom;
						double jY = (vP + uP * aspect) * invZoom;

						double j = Julia (jX, jY, jr, ji);

						double c = factor * j;

						b += Utility.ClampToByte (c - 768);
						g += Utility.ClampToByte (c - 512);
						r += Utility.ClampToByte (c - 256);
						a += Utility.ClampToByte (c - 0);
					}

					*dstPtr = ColorBgra.FromBgra (Utility.ClampToByte (b / count), Utility.ClampToByte (g / count), Utility.ClampToByte (r / count), Utility.ClampToByte (a / count));

					++dstPtr;
				}
			}
		}

		private static double Julia (double x, double y, double r, double i)
		{
			double c = 0;

			while (c < 256 && x * x + y * y < 10000) {
				double t = x;
				x = x * x - y * y + r;
				y = 2 * t * y + i;
				++c;
			}

			c -= 2 - 2 * log2_10000 / Math.Log (x * x + y * y);

			return c;
		}
		#endregion
	}
}
