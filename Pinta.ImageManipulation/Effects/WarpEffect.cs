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
	public abstract class WarpEffect : BaseEffect
	{
		private double defaultRadius = 2;
		private double defaultRadius2 = 4;

		protected int quality;
		protected Point center_offset;
		protected WarpEdgeBehavior edge_behavior;
		protected ColorBgra primary_color;
		protected ColorBgra secondary_color;

		protected double DefaultRadius { get { return this.defaultRadius; } }
		protected double DefaultRadius2 { get { return this.defaultRadius2; } }

		protected WarpEffect (int quality, Point centerOffset, WarpEdgeBehavior edgeBehavior, ColorBgra primaryColor, ColorBgra secondaryColor)
		{
			if (quality < 1 || quality > 5)
				throw new ArgumentOutOfRangeException ("quality");

			this.quality = quality;
			this.center_offset = centerOffset;
			this.edge_behavior = edgeBehavior;
			this.primary_color = primaryColor;
			this.secondary_color = secondaryColor;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			ColorBgra colTransparent = ColorBgra.Transparent;

			int aaSampleCount = quality * quality;
			PointD* aaPoints = stackalloc PointD[aaSampleCount];
			Utility.GetRgssOffsets (aaPoints, aaSampleCount, quality);
			ColorBgra* samples = stackalloc ColorBgra[aaSampleCount];

			TransformData td;

			for (int y = rect.Top; y <= rect.Bottom; y++) {
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

				double relativeY = y - center_offset.Y;

				for (int x = rect.Left; x <= rect.Right; x++) {
					double relativeX = x - center_offset.X;

					int sampleCount = 0;

					for (int p = 0; p < aaSampleCount; ++p) {
						td.X = relativeX + aaPoints[p].X;
						td.Y = relativeY - aaPoints[p].Y;

						InverseTransform (ref td);

						float sampleX = (float)(td.X + center_offset.X);
						float sampleY = (float)(td.Y + center_offset.Y);

						ColorBgra sample = primary_color;

						if (IsOnSurface (src, sampleX, sampleY)) {
							sample = Utility.GetBilinearSampleClamped (src, sampleX, sampleY);
						} else {
							switch (edge_behavior) {
								case WarpEdgeBehavior.Clamp:
									sample = Utility.GetBilinearSampleClamped (src, sampleX, sampleY);
									break;

								case WarpEdgeBehavior.Wrap:
									sample = Utility.GetBilinearSampleWrapped (src, sampleX, sampleY);
									break;

								case WarpEdgeBehavior.Reflect:
									sample = Utility.GetBilinearSampleClamped (src, ReflectCoord (sampleX, src.Width), ReflectCoord (sampleY, src.Height));

									break;

								case WarpEdgeBehavior.Primary:
									sample = primary_color;
									break;

								case WarpEdgeBehavior.Secondary:
									sample = secondary_color;
									break;

								case WarpEdgeBehavior.Transparent:
									sample = colTransparent;
									break;

								case WarpEdgeBehavior.Original:
									sample = src.GetPoint (x, y);
									break;
								default:

									break;
							}
						}

						samples[sampleCount] = sample;
						++sampleCount;
					}

					*dstPtr = ColorBgra.Blend (samples, sampleCount);
					++dstPtr;
				}
			}
		}

		protected abstract void InverseTransform (ref TransformData data);

		protected struct TransformData
		{
			public double X;
			public double Y;
		}

		private static bool IsOnSurface (ISurface src, float u, float v)
		{
			return (u >= 0 && u <= (src.Width - 1) && v >= 0 && v <= (src.Height - 1));
		}

		private static float ReflectCoord (float value, int max)
		{
			bool reflection = false;

			while (value < 0) {
				value += max;
				reflection = !reflection;
			}

			while (value > max) {
				value -= max;
				reflection = !reflection;
			}

			if (reflection) {
				value = max - value;
			}

			return value;
		}
		#endregion
	}

	public enum WarpEdgeBehavior
	{
		Clamp,
		Wrap,
		Reflect,
		Primary,
		Secondary,
		Transparent,
		Original,
	}
}
