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
	public class MotionBlurEffect : BaseEffect
	{
		private double angle;
		private int distance;
		private bool centered;

		public MotionBlurEffect (double angle, int distance, bool centered)
		{
			if (distance < 1 || distance > 200)
				throw new ArgumentOutOfRangeException ("distance");

			this.angle = angle;
			this.distance = distance;
			this.centered = centered;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			PointD start = new PointD (0, 0);
			double theta = ((double)(angle + 180) * 2 * Math.PI) / 360.0;
			double alpha = (double)distance;
			PointD end = new PointD ((float)alpha * Math.Cos (theta), (float)(-alpha * Math.Sin (theta)));

			if (centered) {
				start.X = -end.X / 2.0f;
				start.Y = -end.Y / 2.0f;

				end.X /= 2.0f;
				end.Y /= 2.0f;
			}

			PointD[] points = new PointD[((1 + distance) * 3) / 2];

			if (points.Length == 1) {
				points[0] = new PointD (0, 0);
			} else {
				for (int i = 0; i < points.Length; ++i) {
					float frac = (float)i / (float)(points.Length - 1);
					points[i] = Utility.Lerp (start, end, frac);
				}
			}

			ColorBgra* samples = stackalloc ColorBgra[points.Length];

			int src_width = src.Width;
			int src_height = src.Height;


			for (int y = rect.Top; y <= rect.Bottom; ++y) {
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

				for (int x = rect.Left; x <= rect.Right; ++x) {
					int sampleCount = 0;

					for (int j = 0; j < points.Length; ++j) {
						PointD pt = new PointD (points[j].X + (float)x, points[j].Y + (float)y);

						if (pt.X >= 0 && pt.Y >= 0 && pt.X <= (src_width - 1) && pt.Y <= (src_height - 1)) {
							samples[sampleCount] = Utility.GetBilinearSampleClamped (src, (float)pt.X, (float)pt.Y);
							++sampleCount;
						}
					}

					*dstPtr = ColorBgra.Blend (samples, sampleCount);
					++dstPtr;
				}
			}
		}
		#endregion
	}
}
