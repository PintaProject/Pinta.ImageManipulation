/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Gradients
{
	public sealed class ConicalRenderer : BaseGradientRenderer
	{
		private const double invPi = 1.0 / Math.PI;
		private double tOffset;

		public ConicalRenderer (bool alphaOnly, BinaryPixelOp normalBlendOp) : base (alphaOnly, normalBlendOp)
		{
		}

		public override void BeforeRender ()
		{
			var ax = EndPoint.X - StartPoint.X;
			var ay = EndPoint.Y - StartPoint.Y;

			var theta = Math.Atan2 (ay, ax);

			var t = theta * invPi;

			tOffset = -t;
			base.BeforeRender ();
		}

		public override byte ComputeByteLerp (int x, int y)
		{
			var ax = x - StartPoint.X;
			var ay = y - StartPoint.Y;

			var theta = Math.Atan2 (ay, ax);

			var t = theta * invPi;

			return (byte)(BoundLerp (t + tOffset) * 255f);
		}

		public double BoundLerp (double t)
		{
			if (t > 1)
				t -= 2;
			else if (t < -1)
				t += 2;

			return Utility.Clamp (Math.Abs (t), 0, 1);
		}
	}
}
