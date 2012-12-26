/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzystzof Marecki                                       //
/////////////////////////////////////////////////////////////////////////////////

// This effect was graciously provided by David Issel, aka BoltBait. His original
// copyright and license (MIT License) are reproduced below.

/*
PortraitEffect.cs 
Copyright (c) 2007 David Issel 
Contact Info: BoltBait@hotmail.com http://www.BoltBait.com 

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions: 

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of the Software. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
THE SOFTWARE. 
*/

using System;
using Pinta.ImageManipulation.UnaryPixelOperations;
using Pinta.ImageManipulation.PixelBlendOperations;

namespace Pinta.ImageManipulation.Effects
{
	public class SoftenPortraitEffect : BaseEffect
	{
		private int softness;
		private int lighting;
		private int warmth;

		private GaussianBlurEffect blur_effect;
		private BrightnessContrastEffect bac_adjustment;
		private DesaturateOp desaturate_op;
		private OverlayBlendOp overlay_op;

		/// <summary>
		/// Creates a new effect that will soften an image.
		/// </summary>
		/// <param name="softness">How much to soften the image. Valid range is 0 - 10.</param>
		/// <param name="lighting">Amount of lighting to apply. Valid range is -20 - 20.</param>
		/// <param name="warmth">Amount of warmth to apply. Valid range is 0 - 20.</param>
		public SoftenPortraitEffect (int softness = 5, int lighting = 0, int warmth = 10)
		{
			if (softness < 0 || softness > 10)
				throw new ArgumentOutOfRangeException ("softness");
			if (lighting < -20 || lighting > 20)
				throw new ArgumentOutOfRangeException ("lighting");
			if (warmth < 0 || warmth > 20)
				throw new ArgumentOutOfRangeException ("warmth");

			this.softness = softness;
			this.lighting = lighting;
			this.warmth = warmth;

			blur_effect = new GaussianBlurEffect (2);
			bac_adjustment = new BrightnessContrastEffect (0, 0);
			desaturate_op = new DesaturateOp ();
			overlay_op = new OverlayBlendOp ();
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			float redAdjust = 1.0f + (warmth / 100.0f);
			float blueAdjust = 1.0f - (warmth / 100.0f);

			this.blur_effect.Render (src, dest, roi);
			this.bac_adjustment.Render (src, dest, roi);

			for (int y = roi.Top; y <= roi.Bottom; ++y) {
				ColorBgra* srcPtr = src.GetPointAddress (roi.X, y);
				ColorBgra* dstPtr = dest.GetPointAddress (roi.X, y);

				for (int x = roi.Left; x <= roi.Right; ++x) {
					ColorBgra srcGrey = this.desaturate_op.Apply (*srcPtr);

					srcGrey.R = Utility.ClampToByte ((int)((float)srcGrey.R * redAdjust));
					srcGrey.B = Utility.ClampToByte ((int)((float)srcGrey.B * blueAdjust));

					ColorBgra mypixel = this.overlay_op.Apply (srcGrey, *dstPtr);
					*dstPtr = mypixel;

					++srcPtr;
					++dstPtr;
				}
			}
		}
		#endregion
	}
}
