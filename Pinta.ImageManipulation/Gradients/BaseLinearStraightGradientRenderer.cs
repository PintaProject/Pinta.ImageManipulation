/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Gradients
{
	public abstract class BaseLinearStraightGradientRenderer : BaseLinearGradientRenderer
	{
		private int _startY;
		private int _startX;

		protected internal BaseLinearStraightGradientRenderer (bool alphaOnly, BinaryPixelOp normalBlendOp) : base (alphaOnly, normalBlendOp)
		{
		}

		public override void BeforeRender ()
		{
			base.BeforeRender ();

			_startX = (int)StartPoint.X;
			_startY = (int)StartPoint.Y;
		}

		public override byte ComputeByteLerp (int x, int y)
		{
			var dx = x - _startX;
			var dy = y - _startY;

			var lerp = (dx * dtdx) + (dy * dtdy);

			return BoundLerp (lerp);
		}

		private static byte BoundLerp (double t)
		{
			return (byte)(Utility.Clamp (Math.Abs (t), 0, 1) * 255f);
		}
	}
}
