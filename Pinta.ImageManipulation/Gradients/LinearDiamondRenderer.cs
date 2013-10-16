/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Gradients
{
	public sealed class LinearDiamondRenderer : BaseLinearStraightGradientRenderer
	{
		public LinearDiamondRenderer (bool alphaOnly, BinaryPixelOp normalBlendOp) : base (alphaOnly, normalBlendOp)
		{
		}

		public override byte ComputeByteLerp (int x, int y)
		{
			var dx = x - StartPoint.X;
			var dy = y - StartPoint.Y;

			var lerp1 = (dx * dtdx) + (dy * dtdy);
			var lerp2 = (dx * dtdy) - (dy * dtdx);

			var absLerp1 = Math.Abs (lerp1);
			var absLerp2 = Math.Abs (lerp2);

			return BoundLerp (absLerp1 + absLerp2);
		}

		private byte BoundLerp (double t)
		{
			return (byte)(Utility.Clamp (t, 0, 1) * 255f);
		}
	}
}
