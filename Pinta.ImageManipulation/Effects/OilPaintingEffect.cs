/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Jonathan Pobst <monkey@jpobst.com>                      //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class OilPaintingEffect : BaseEffect
	{
		private int brush_size;
		private int coarseness;

		/// <summary>
		/// Creates a new effect that will make the image look like an oil painting.
		/// </summary>
		/// <param name="brushSize">Size of the brush to use. Valid range is 1 - 8.</param>
		/// <param name="coarseness">Coarseness of the brush to use. Valid range is 3 - 255.</param>
		public OilPaintingEffect (int brushSize = 3, int coarseness = 50)
		{
			if (brushSize < 1 || brushSize > 8)
				throw new ArgumentOutOfRangeException ("brushSize");
			if (coarseness < 3 || coarseness > 255)
				throw new ArgumentOutOfRangeException ("coarseness");

			this.brush_size = brushSize;
			this.coarseness = coarseness;
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle rect)
		{
			int width = src.Width;
			int height = src.Height;

			int arrayLens = 1 + coarseness;

			int localStoreSize = arrayLens * 5 * sizeof (int);

			byte* localStore = stackalloc byte[localStoreSize];
			byte* p = localStore;

			int* intensityCount = (int*)p;
			p += arrayLens * sizeof (int);

			uint* avgRed = (uint*)p;
			p += arrayLens * sizeof (uint);

			uint* avgGreen = (uint*)p;
			p += arrayLens * sizeof (uint);

			uint* avgBlue = (uint*)p;
			p += arrayLens * sizeof (uint);

			uint* avgAlpha = (uint*)p;
			p += arrayLens * sizeof (uint);

			byte maxIntensity = (byte)coarseness;

			int rectTop = rect.Top;
			int rectBottom = rect.Bottom;
			int rectLeft = rect.Left;
			int rectRight = rect.Right;

			for (int y = rectTop; y <= rectBottom; ++y) {
				ColorBgra* dstPtr = dest.GetPointAddress (rect.Left, y);

				int top = y - brush_size;
				int bottom = y + brush_size + 1;

				if (top < 0) {
					top = 0;
				}

				if (bottom > height) {
					bottom = height;
				}

				for (int x = rectLeft; x <= rectRight; ++x) {
					SetToZero (localStore, (ulong)localStoreSize);

					int left = x - brush_size;
					int right = x + brush_size + 1;

					if (left < 0) {
						left = 0;
					}

					if (right > width) {
						right = width;
					}

					int numInt = 0;

					for (int j = top; j < bottom; ++j) {
						ColorBgra* srcPtr = src.GetPointAddress (left, j);

						for (int i = left; i < right; ++i) {
							byte intensity = Utility.FastScaleByteByByte (srcPtr->GetIntensityByte (), maxIntensity);

							++intensityCount[intensity];
							++numInt;

							avgRed[intensity] += srcPtr->R;
							avgGreen[intensity] += srcPtr->G;
							avgBlue[intensity] += srcPtr->B;
							avgAlpha[intensity] += srcPtr->A;

							++srcPtr;
						}
					}

					byte chosenIntensity = 0;
					int maxInstance = 0;

					for (int i = 0; i <= maxIntensity; ++i) {
						if (intensityCount[i] > maxInstance) {
							chosenIntensity = (byte)i;
							maxInstance = intensityCount[i];
						}
					}

					// TODO: correct handling of alpha values?

					byte R = (byte)(avgRed[chosenIntensity] / maxInstance);
					byte G = (byte)(avgGreen[chosenIntensity] / maxInstance);
					byte B = (byte)(avgBlue[chosenIntensity] / maxInstance);
					byte A = (byte)(avgAlpha[chosenIntensity] / maxInstance);

					*dstPtr = ColorBgra.FromBgra (B, G, R, A);
					++dstPtr;
				}
			}
		}

		// This is slow, and gets called a lot
		private unsafe static void SetToZero (byte* dst, ulong length)
		{
			int* ptr = (int*)dst;

			for (ulong i = 0; i < length / 4; i++) {
				*ptr = 0;
				ptr++;
			}
		}
		#endregion
	}
}
