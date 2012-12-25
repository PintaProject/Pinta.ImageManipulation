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
	public class FrostedGlassEffect : BaseEffect
	{
		private int amount;
		private Random random = new Random ();

		public FrostedGlassEffect (int amount)
		{
			if (amount < 1 || amount > 10)
				throw new ArgumentOutOfRangeException ("amount");

			this.amount = amount;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dst, Rectangle rect)
		{
			int width = src.Width;
			int height = src.Height;
			int r = amount;
			Random localRandom = this.random;

			int* intensityCount = stackalloc int[256];
			uint* avgRed = stackalloc uint[256];
			uint* avgGreen = stackalloc uint[256];
			uint* avgBlue = stackalloc uint[256];
			uint* avgAlpha = stackalloc uint[256];
			byte* intensityChoices = stackalloc byte[(1 + (r * 2)) * (1 + (r * 2))];

			int rectTop = rect.Top;
			int rectBottom = rect.Bottom;
			int rectLeft = rect.Left;
			int rectRight = rect.Right;

			for (int y = rectTop; y <= rectBottom; ++y) {
				ColorBgra* dstPtr = dst.GetPointAddress (rect.Left, y);

				int top = y - r;
				int bottom = y + r + 1;

				if (top < 0) {
					top = 0;
				}

				if (bottom > height) {
					bottom = height;
				}

				for (int x = rectLeft; x <= rectRight; ++x) {
					int intensityChoicesIndex = 0;

					for (int i = 0; i < 256; ++i) {
						intensityCount[i] = 0;
						avgRed[i] = 0;
						avgGreen[i] = 0;
						avgBlue[i] = 0;
						avgAlpha[i] = 0;
					}

					int left = x - r;
					int right = x + r + 1;

					if (left < 0) {
						left = 0;
					}

					if (right > width) {
						right = width;
					}

					for (int j = top; j < bottom; ++j) {
						if (j < 0 || j >= height) {
							continue;
						}

						ColorBgra* srcPtr = src.GetPointAddress (left, j);

						for (int i = left; i < right; ++i) {
							byte intensity = srcPtr->GetIntensityByte ();

							intensityChoices[intensityChoicesIndex] = intensity;
							++intensityChoicesIndex;

							++intensityCount[intensity];

							avgRed[intensity] += srcPtr->R;
							avgGreen[intensity] += srcPtr->G;
							avgBlue[intensity] += srcPtr->B;
							avgAlpha[intensity] += srcPtr->A;

							++srcPtr;
						}
					}

					int randNum;

					lock (localRandom) {
						randNum = localRandom.Next (intensityChoicesIndex);
					}

					byte chosenIntensity = intensityChoices[randNum];

					byte R = (byte)(avgRed[chosenIntensity] / intensityCount[chosenIntensity]);
					byte G = (byte)(avgGreen[chosenIntensity] / intensityCount[chosenIntensity]);
					byte B = (byte)(avgBlue[chosenIntensity] / intensityCount[chosenIntensity]);
					byte A = (byte)(avgAlpha[chosenIntensity] / intensityCount[chosenIntensity]);

					*dstPtr = ColorBgra.FromBgra (B, G, R, A);
					++dstPtr;

					// prepare the array for the next loop iteration
					for (int i = 0; i < intensityChoicesIndex; ++i) {
						intensityChoices[i] = 0;
					}
				}
			}
		}
		#endregion
	}
}
