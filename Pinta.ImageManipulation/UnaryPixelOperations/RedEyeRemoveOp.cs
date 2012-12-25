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
	/// <summary>
	/// If the color is within the red tolerance, remove it
	/// </summary>
	public class RedEyeRemoveOp : UnaryPixelOp
	{
		private int tolerence;
		private double set_saturation;

		public RedEyeRemoveOp (int tol, int sat)
		{
			tolerence = tol;
			set_saturation = (double)sat / 100;
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			// The higher the saturation, the more red it is
			var saturation = GetSaturation (color);

			// The higher the difference between the other colors, the more red it is
			var difference = color.R - Math.Max (color.B, color.G);

			// If it is within tolerence, and the saturation is high
			if ((difference > tolerence) && (saturation > 100)) {
				var i = 255.0 * color.GetIntensity ();
				var ib = (byte)(i * set_saturation); // adjust the red color for user inputted saturation
				return ColorBgra.FromBgra ((byte)color.B, (byte)color.G, ib, color.A);
			} else {
				return color;
			}
		}

		//Saturation formula from RgbColor.cs, public HsvColor ToHsv()
		private int GetSaturation (ColorBgra color)
		{
			double min;
			double max;
			double delta;

			var r = (double)color.R / 255;
			var g = (double)color.G / 255;
			var b = (double)color.B / 255;

			double s;

			min = Math.Min (Math.Min (r, g), b);
			max = Math.Max (Math.Max (r, g), b);
			delta = max - min;

			if (max == 0 || delta == 0) {
				// R, G, and B must be 0, or all the same.
				// In this case, S is 0, and H is undefined.
				// Using H = 0 is as good as any...
				s = 0;
			} else {
				s = delta / max;
			}

			return (int)(s * 255);
		}
	}
}
