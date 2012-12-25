/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Jonathan Pobst <monkey@jpobst.com>                      //
/////////////////////////////////////////////////////////////////////////////////

using System;
using Pinta.ImageManipulation.UnaryPixelOperations;
using Pinta.ImageManipulation.PixelBlendOperations;

namespace Pinta.ImageManipulation.Effects
{
	public class PencilSketchEffect : BaseEffect
	{
		private GaussianBlurEffect blur_effect;
		private DesaturateOp desaturate_op;
		private InvertColorsEffect invert_effect;
		private BrightnessContrastEffect bac_adjustment;
		private ColorDodgeBlendOp color_dodge_op;

		private int pencil_size;
		private int color_range;

		public PencilSketchEffect (int pencilSize, int colorRange)
		{
			if (pencilSize < 1 || pencilSize > 20)
				throw new ArgumentOutOfRangeException ("pencilSize");
			if (colorRange < -20 || colorRange > 20)
				throw new ArgumentOutOfRangeException ("colorRange");

			this.pencil_size = pencilSize;
			this.color_range = colorRange;

			blur_effect = new GaussianBlurEffect (pencil_size);
			desaturate_op = new DesaturateOp ();
			invert_effect = new InvertColorsEffect ();
			bac_adjustment = new BrightnessContrastEffect (-color_range, -color_range);
			color_dodge_op = new ColorDodgeBlendOp ();
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			bac_adjustment.Render (src, dest, roi);
			blur_effect.Render (src, dest, roi);
			invert_effect.Render (dest, dest, roi);
			desaturate_op.Apply (dest, dest, roi);

			for (int y = roi.Top; y <= roi.Bottom; ++y) {
				var srcPtr = src.GetPointAddress (roi.X, y);
				var dstPtr = dest.GetPointAddress (roi.X, y);

				for (int x = roi.Left; x <= roi.Right; ++x) {
					var srcGrey = desaturate_op.Apply (*srcPtr);
					var sketched = color_dodge_op.Apply (srcGrey, *dstPtr);
					*dstPtr = sketched;

					++srcPtr;
					++dstPtr;
				}
			}
		}
		#endregion
	}
}
