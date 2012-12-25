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

namespace Pinta.ImageManipulation.UnaryPixelOperations
{
	public class HueSaturationLightnessOp : UnaryPixelOp
	{
		private int hue_delta;
		private int sat_factor;
		private UnaryPixelOp blend_op;

		public HueSaturationLightnessOp (int hueDelta, int satDelta, int lightness)
		{
			hue_delta = hueDelta;
			sat_factor = (satDelta * 1024) / 100;

			if (lightness == 0)
				blend_op = new IdentityOp ();
			else if (lightness > 0)
				blend_op = new BlendConstantOp (ColorBgra.FromBgra (255, 255, 255, (byte)((lightness * 255) / 100)));
			else
				blend_op = new BlendConstantOp (ColorBgra.FromBgra (0, 0, 0, (byte)((-lightness * 255) / 100)));
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			// Adjust saturation
			var intensity = color.GetIntensityByte ();
			color.R = Utility.ClampToByte ((intensity * 1024 + (color.R - intensity) * sat_factor) >> 10);
			color.G = Utility.ClampToByte ((intensity * 1024 + (color.G - intensity) * sat_factor) >> 10);
			color.B = Utility.ClampToByte ((intensity * 1024 + (color.B - intensity) * sat_factor) >> 10);

			var hsvColor = (new RgbColor (color.R, color.G, color.B)).ToHsv ();
			int hue = hsvColor.Hue;

			hue += hue_delta;

			while (hue < 0)
				hue += 360;

			while (hue > 360)
				hue -= 360;

			hsvColor.Hue = hue;

			var rgbColor = hsvColor.ToRgb ();
			var newColor = ColorBgra.FromBgr ((byte)rgbColor.Blue, (byte)rgbColor.Green, (byte)rgbColor.Red);
			
			newColor = blend_op.Apply (newColor);
			newColor.A = color.A;

			return newColor;
		}
	}
}
