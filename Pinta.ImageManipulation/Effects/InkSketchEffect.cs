/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Jonathan Pobst <monkey@jpobst.com>                      //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Pinta.ImageManipulation.UnaryPixelOperations;
using Pinta.ImageManipulation.PixelBlendOperations;

namespace Pinta.ImageManipulation.Effects
{
	public class InkSketchEffect : BaseEffect
	{
		private static readonly int[][] conv;
		private const int size = 5;
		private const int radius = (size - 1) / 2;

		private GlowEffect glow_effect;
		private DesaturateOp desaturate_op;
		private DarkenBlendOp darken_op;

		private int ink_outline;
		private int coloring;

		static InkSketchEffect ()
		{
			conv = new int[5][];

			for (var i = 0; i < conv.Length; ++i)
				conv[i] = new int[5];

			conv[0] = new int[] { -1, -1, -1, -1, -1 };
			conv[1] = new int[] { -1, -1, -1, -1, -1 };
			conv[2] = new int[] { -1, -1, 30, -1, -1 };
			conv[3] = new int[] { -1, -1, -1, -1, -1 };
			conv[4] = new int[] { -1, -1, -5, -1, -1 };
		}

		public InkSketchEffect (int inkOutline, int coloring)
		{
			if (inkOutline < 0 || inkOutline > 99)
				throw new ArgumentOutOfRangeException ("inkOutline");
			if (coloring < 0 || coloring > 100)
				throw new ArgumentOutOfRangeException ("coloring");

			ink_outline = inkOutline;
			this.coloring = coloring;

			glow_effect = new GlowEffect (6, -(coloring - 50) * 2, -(coloring - 50) * 2);
			desaturate_op = new DesaturateOp ();
			darken_op = new DarkenBlendOp ();
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			// Glow backgound 
			glow_effect.Render (src, dest, roi);

			// Create black outlines by finding the edges of objects 
			for (int y = roi.Top; y <= roi.Bottom; ++y) {
				int top = y - radius;
				int bottom = y + radius + 1;

				if (top < 0) {
					top = 0;
				}

				if (bottom > dest.Height) {
					bottom = dest.Height;
				}

				ColorBgra* srcPtr = src.GetPointAddress (roi.X, y);
				ColorBgra* dstPtr = dest.GetPointAddress (roi.X, y);

				for (int x = roi.Left; x <= roi.Right; ++x) {
					int left = x - radius;
					int right = x + radius + 1;

					if (left < 0) {
						left = 0;
					}

					if (right > dest.Width) {
						right = dest.Width;
					}

					int r = 0;
					int g = 0;
					int b = 0;

					for (int v = top; v < bottom; v++) {
						ColorBgra* pRow = src.GetRowAddress (v);
						int j = v - y + radius;

						for (int u = left; u < right; u++) {
							int i1 = u - x + radius;
							int w = conv[j][i1];

							ColorBgra* pRef = pRow + u;

							r += pRef->R * w;
							g += pRef->G * w;
							b += pRef->B * w;
						}
					}

					ColorBgra topLayer = ColorBgra.FromBgr (
						Utility.ClampToByte (b),
						Utility.ClampToByte (g),
						Utility.ClampToByte (r));

					// Desaturate 
					topLayer = this.desaturate_op.Apply (topLayer);

					// Adjust Brightness and Contrast 
					if (topLayer.R > (ink_outline * 255 / 100)) {
						topLayer = ColorBgra.FromBgra (255, 255, 255, topLayer.A);
					} else {
						topLayer = ColorBgra.FromBgra (0, 0, 0, topLayer.A);
					}

					// Change Blend Mode to Darken 
					ColorBgra myPixel = this.darken_op.Apply (topLayer, *dstPtr);
					*dstPtr = myPixel;

					++srcPtr;
					++dstPtr;
				}
			}
		}
		#endregion
	}
}
