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
	public class FragmentEffect : BaseEffect
	{
		private int fragments;
		private int distance;
		private double rotation;

		public FragmentEffect (int fragments, int distance, double rotation)
		{
			if (fragments < 2 || fragments > 50)
				throw new ArgumentOutOfRangeException ("fragments");
			if (distance < 0 || distance > 100)
				throw new ArgumentOutOfRangeException ("distance");

			this.fragments = fragments;
			this.distance = distance;
			this.rotation = rotation;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			var pointOffsets = RecalcPointOffsets (fragments, rotation, distance);

			int poLength = pointOffsets.Length;
			Point* pointOffsetsPtr = stackalloc Point[poLength];

			for (int i = 0; i < poLength; ++i)
				pointOffsetsPtr[i] = pointOffsets[i];

			ColorBgra* samples = stackalloc ColorBgra[poLength];

			int src_width = src.Width;
			int src_height = src.Height;

			for (int y = rect.Top; y <= rect.Bottom; y++) {
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

				for (int x = rect.Left; x <= rect.Right; x++) {
					int sampleCount = 0;

					for (int i = 0; i < poLength; ++i) {
						int u = x - pointOffsetsPtr[i].X;
						int v = y - pointOffsetsPtr[i].Y;

						if (u >= 0 && u < src_width && v >= 0 && v < src_height) {
							samples[sampleCount] = src.GetPoint (u, v);
							++sampleCount;
						}
					}

					*dstPtr = ColorBgra.Blend (samples, sampleCount);
					++dstPtr;
				}
			}
		}

		private Point[] RecalcPointOffsets (int fragments, double rotationAngle, int distance)
		{
			double pointStep = 2 * Math.PI / (double)fragments;
			double rotationRadians = ((rotationAngle - 90.0) * Math.PI) / 180.0;

			Point[] pointOffsets = new Point[fragments];

			for (int i = 0; i < fragments; i++) {
				double currentRadians = rotationRadians + (pointStep * i);

				pointOffsets[i] = new Point (
					(int)Math.Round (distance * -Math.Sin (currentRadians), MidpointRounding.AwayFromZero),
					(int)Math.Round (distance * -Math.Cos (currentRadians), MidpointRounding.AwayFromZero));
			}

			return pointOffsets;
		}
		#endregion
	}
}
