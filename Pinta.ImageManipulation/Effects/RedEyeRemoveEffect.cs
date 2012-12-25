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
	public class RedEyeRemoveEffect : BaseEffect
	{
		private RedEyeRemoveOp op;

		public RedEyeRemoveEffect (int tolerance, int saturation)
		{
			if (tolerance < 0 || tolerance > 100)
				throw new ArgumentOutOfRangeException ("tolerance");
			if (saturation < 0 || saturation > 100)
				throw new ArgumentOutOfRangeException ("saturation");

			op = new RedEyeRemoveOp (tolerance, saturation);
		}

		#region Algorithm Code Ported From PDN
		protected unsafe override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			op.Apply (src, dest, roi);
		}
		#endregion
	}
}
