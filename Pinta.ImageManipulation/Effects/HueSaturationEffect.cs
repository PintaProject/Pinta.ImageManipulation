/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzysztof Marecki <marecki.krzysztof@gmail.com>         //
/////////////////////////////////////////////////////////////////////////////////

using System;
using Pinta.ImageManipulation.UnaryPixelOperations;

namespace Pinta.ImageManipulation.Effects
{
	public class HueSaturationEffect : BaseEffect
	{
		private UnaryPixelOp op;

		/// <summary>
		/// Creates a new effect that will adjust the hue, saturation, and lightness of an image.
		/// </summary>
		/// <param name="hue">Amount of hue to adjust. Valid range is -180 - 180.</param>
		/// <param name="saturation">Amount of saturation to adjust. Valid range is 0 - 200.</param>
		/// <param name="lightness">Amount of of lightness to adjust. Valid range is -100 - 100.</param>
		public HueSaturationEffect (int hue = 0, int saturation = 100, int lightness = 0)
		{
			if (hue < -180 || hue > 180)
				throw new ArgumentOutOfRangeException ("hue");
			if (saturation < 0 || saturation > 200)
				throw new ArgumentOutOfRangeException ("saturation");
			if (lightness < -100 || lightness > 100)
				throw new ArgumentOutOfRangeException ("lightness");

			if (hue == 0 && saturation == 100 && lightness == 0)
				op = new IdentityOp ();
			else
				op = new HueSaturationLightnessOp (hue, saturation, lightness);
		}

		#region Algorithm Code Ported From PDN
		protected override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			op.Apply (src, dest, roi);
		}
		#endregion
	}
}
