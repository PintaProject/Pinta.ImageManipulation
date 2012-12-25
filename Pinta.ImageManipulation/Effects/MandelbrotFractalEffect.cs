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
	public class MandelbrotFractalEffect : BaseEffect
	{
		private int factor;
		private int quality;
		private int zoom;
		private double angle;
		private bool invert_colors;

		private const double max = 100000;
		private static readonly double invLogMax = 1.0 / Math.Log (max);
		private static double zoomFactor = 20.0;
		private const double xOffsetBasis = -0.7;
		private double xOffset = xOffsetBasis;

		private const double yOffsetBasis = -0.29;
		private double yOffset = yOffsetBasis;

		public MandelbrotFractalEffect (int factor, int quality, int zoom, double angle, bool invertColors)
		{
			if (factor < 1 || factor > 10)
				throw new ArgumentOutOfRangeException ("factor");
			if (quality < 1 || quality > 5)
				throw new ArgumentOutOfRangeException ("quality");
			if (zoom < 0 || zoom > 100)
				throw new ArgumentOutOfRangeException ("zoom");

			this.factor = factor;
			this.quality = quality;
			this.zoom = zoom;
			this.angle = angle;
			this.invert_colors = invertColors;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			int w = dst.Width;
			int h = dst.Height;

			double invH = 1.0 / h;
			double zoom2 = 1 + zoomFactor * zoom;
			double invZoom = 1.0 / zoom2;

			double invQuality = 1.0 / (double)quality;

			int count = quality * quality + 1;
			double invCount = 1.0 / (double)count;
			double angleTheta = (angle * 2 * Math.PI) / 360;

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

						double m = Mandelbrot ((uP * invZoom) + this.xOffset, (vP * invZoom) + this.yOffset, factor);

						double c = 64 + factor * m;

						r += Utility.ClampToByte (c - 768);
						g += Utility.ClampToByte (c - 512);
						b += Utility.ClampToByte (c - 256);
						a += Utility.ClampToByte (c - 0);
					}

					*dstPtr = ColorBgra.FromBgra (Utility.ClampToByte (b / count), Utility.ClampToByte (g / count), Utility.ClampToByte (r / count), Utility.ClampToByte (a / count));

					++dstPtr;
				}
			}

			if (invert_colors) {
				for (int y = rect.Top; y <= rect.Bottom; y++) {
					ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

					for (int x = rect.Left; x <= rect.Right; ++x) {
						ColorBgra c = *dstPtr;

						c.B = (byte)(255 - c.B);
						c.G = (byte)(255 - c.G);
						c.R = (byte)(255 - c.R);

						*dstPtr = c;
						++dstPtr;
					}
				}
			}
		}

		private static double Mandelbrot (double r, double i, int factor)
		{
			int c = 0;
			double x = 0;
			double y = 0;

			while ((c * factor) < 1024 && ((x * x) + (y * y)) < max) {
				double t = x;

				x = x * x - y * y + r;
				y = 2 * t * y + i;

				++c;
			}

			return c - Math.Log (y * y + x * x) * invLogMax;
		}
		#endregion
	}
}
