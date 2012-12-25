/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pinta.ImageManipulation.PixelBlendOperations;

namespace Pinta.ImageManipulation
{
	class Utility
	{
		/// <summary>
		/// Allows you to find the bounding box for a "region" that is described as an
		/// array of bounding boxes.
		/// </summary>
		/// <param name="rectsF">The "region" you want to find a bounding box for.</param>
		/// <returns>A RectangleF structure that surrounds the Region.</returns>
		public static Rectangle GetRegionBounds (Rectangle[] rects, int startIndex, int length)
		{
			if (rects.Length == 0) {
				return Rectangle.Empty;
			}

			int left = rects[startIndex].Left;
			int top = rects[startIndex].Top;
			int right = rects[startIndex].Right;
			int bottom = rects[startIndex].Bottom;

			for (int i = startIndex + 1; i < startIndex + length; ++i) {
				Rectangle rect = rects[i];

				if (rect.Left < left) {
					left = rect.Left;
				}

				if (rect.Top < top) {
					top = rect.Top;
				}

				if (rect.Right > right) {
					right = rect.Right;
				}

				if (rect.Bottom > bottom) {
					bottom = rect.Bottom;
				}
			}

			return Rectangle.FromLTRB (left, top, right, bottom);
		}

		public static double Clamp (double x, double min, double max)
		{
			if (x < min) {
				return min;
			} else if (x > max) {
				return max;
			} else {
				return x;
			}
		}

		public static float Clamp (float x, float min, float max)
		{
			if (x < min) {
				return min;
			} else if (x > max) {
				return max;
			} else {
				return x;
			}
		}

		public static int Clamp (int x, int min, int max)
		{
			if (x < min) {
				return min;
			} else if (x > max) {
				return max;
			} else {
				return x;
			}
		}

		public static byte ClampToByte (double x)
		{
			if (x > 255) {
				return 255;
			} else if (x < 0) {
				return 0;
			} else {
				return (byte)x;
			}
		}

		public static byte ClampToByte (float x)
		{
			if (x > 255) {
				return 255;
			} else if (x < 0) {
				return 0;
			} else {
				return (byte)x;
			}
		}

		public static byte ClampToByte (int x)
		{
			if (x > 255) {
				return 255;
			} else if (x < 0) {
				return 0;
			} else {
				return (byte)x;
			}
		}

		public static unsafe ColorBgra GetBilinearSampleClamped (ISurface src, float x, float y)
		{
			if (!Utility.IsNumber (x) || !Utility.IsNumber (y))
				return ColorBgra.Transparent;

			float u = x;
			float v = y;

			if (u < 0)
				u = 0;
			else if (u > src.Width - 1)
				u = src.Width - 1;

			if (v < 0)
				v = 0;
			else if (v > src.Height - 1)
				v = src.Height - 1;

			unchecked {
				int iu = (int)Math.Floor (u);
				uint sxfrac = (uint)(256 * (u - (float)iu));
				uint sxfracinv = 256 - sxfrac;

				int iv = (int)Math.Floor (v);
				uint syfrac = (uint)(256 * (v - (float)iv));
				uint syfracinv = 256 - syfrac;

				uint wul = (uint)(sxfracinv * syfracinv);
				uint wur = (uint)(sxfrac * syfracinv);
				uint wll = (uint)(sxfracinv * syfrac);
				uint wlr = (uint)(sxfrac * syfrac);

				int sx = iu;
				int sy = iv;
				int sleft = sx;
				int sright;

				if (sleft == (src.Width - 1))
					sright = sleft;
				else
					sright = sleft + 1;

				int stop = sy;
				int sbottom;

				if (stop == (src.Height - 1))
					sbottom = stop;
				else
					sbottom = stop + 1;

				ColorBgra* cul = src.GetPointAddress (sleft, stop);
				ColorBgra* cur = cul + (sright - sleft);
				ColorBgra* cll = src.GetPointAddress (sleft, sbottom);
				ColorBgra* clr = cll + (sright - sleft);

				ColorBgra c = ColorBgra.BlendColors4W16IP (*cul, wul, *cur, wur, *cll, wll, *clr, wlr);
				return c;
			}
		}

		public static unsafe ColorBgra GetBilinearSampleWrapped (ISurface src, float x, float y)
		{
			return GetBilinearSampleWrapped (src, (ColorBgra*)src.GetRowAddress (0), src.Width, src.Height, x, y);
		}

		public static unsafe ColorBgra GetBilinearSampleWrapped (ISurface src, ColorBgra* srcDataPtr, int srcWidth, int srcHeight, float x, float y)
		{
			if (!Utility.IsNumber (x) || !Utility.IsNumber (y))
				return ColorBgra.Transparent;

			float u = x;
			float v = y;

			unchecked {
				int iu = (int)Math.Floor (u);
				uint sxfrac = (uint)(256 * (u - (float)iu));
				uint sxfracinv = 256 - sxfrac;

				int iv = (int)Math.Floor (v);
				uint syfrac = (uint)(256 * (v - (float)iv));
				uint syfracinv = 256 - syfrac;

				uint wul = (uint)(sxfracinv * syfracinv);
				uint wur = (uint)(sxfrac * syfracinv);
				uint wll = (uint)(sxfracinv * syfrac);
				uint wlr = (uint)(sxfrac * syfrac);

				int sx = iu;
				if (sx < 0)
					sx = (srcWidth - 1) + ((sx + 1) % srcWidth);
				else if (sx > (srcWidth - 1))
					sx = sx % srcWidth;

				int sy = iv;
				if (sy < 0)
					sy = (srcHeight - 1) + ((sy + 1) % srcHeight);
				else if (sy > (srcHeight - 1))
					sy = sy % srcHeight;

				int sleft = sx;
				int sright;

				if (sleft == (srcWidth - 1))
					sright = 0;
				else
					sright = sleft + 1;

				int stop = sy;
				int sbottom;

				if (stop == (srcHeight - 1))
					sbottom = 0;
				else
					sbottom = stop + 1;

				ColorBgra cul = src.GetPoint (sleft, stop);
				ColorBgra cur = src.GetPoint (sright, stop);
				ColorBgra cll = src.GetPoint (sleft, sbottom);
				ColorBgra clr = src.GetPoint (sright, sbottom);

				ColorBgra c = ColorBgra.BlendColors4W16IP (cul, wul, cur, wur, cll, wll, clr, wlr);

				return c;
			}
		}

		internal static bool IsNumber (float x)
		{
			return x >= float.MinValue && x <= float.MaxValue;
		}

		public static double Lerp (double from, double to, double frac)
		{
			return (from + frac * (to - from));
		}

		public static PointD Lerp (PointD from, PointD to, float frac)
		{
			return new PointD (Lerp (from.X, to.X, frac), Lerp (from.Y, to.Y, frac));
		}

		public static byte FastScaleByteByByte (byte a, byte b)
		{
			int r1 = a * b + 0x80;
			int r2 = ((r1 >> 8) + r1) >> 8;
			return (byte)r2;
		}

		public static UserBlendOp GetBlendModeOp (BlendMode mode)
		{
			switch (mode) {
				case BlendMode.Normal:
					return new NormalBlendOp ();
				case BlendMode.Multiply:
					return new MultiplyBlendOp ();
				case BlendMode.Additive:
					return new AdditiveBlendOp ();
				case BlendMode.ColorBurn:
					return new ColorBurnBlendOp ();
				case BlendMode.ColorDodge:
					return new ColorDodgeBlendOp ();
				case BlendMode.Reflect:
					return new ReflectBlendOp ();
				case BlendMode.Glow:
					return new GlowBlendOp ();
				case BlendMode.Overlay:
					return new OverlayBlendOp ();
				case BlendMode.Difference:
					return new DifferenceBlendOp ();
				case BlendMode.Negation:
					return new NegationBlendOp ();
				case BlendMode.Lighten:
					return new LightenBlendOp ();
				case BlendMode.Darken:
					return new DarkenBlendOp ();
				case BlendMode.Screen:
					return new ScreenBlendOp ();
				case BlendMode.Xor:
					return new XorBlendOp ();
			}

			throw new ArgumentOutOfRangeException ("mode");
		}

		public static unsafe void GetRgssOffsets (PointD* samplesArray, int sampleCount, int quality)
		{
			if (sampleCount < 1)
				throw new ArgumentOutOfRangeException ("sampleCount", "sampleCount must be [0, int.MaxValue]");

			if (sampleCount != quality * quality)
				throw new ArgumentOutOfRangeException ("sampleCount != (quality * quality)");

			if (sampleCount == 1) {
				samplesArray[0] = new PointD (0.0, 0.0);
			} else {
				for (int i = 0; i < sampleCount; ++i) {
					double y = (i + 1d) / (sampleCount + 1d);
					double x = y * quality;

					x -= (int)x;

					samplesArray[i] = new PointD (x - 0.5d, y - 0.5d);
				}
			}
		}

	}
}
