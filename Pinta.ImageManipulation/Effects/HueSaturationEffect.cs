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

		public HueSaturationEffect (int hue, int saturation, int lightness)
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
