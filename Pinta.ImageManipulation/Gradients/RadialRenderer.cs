/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Gradients
{
	public sealed class RadialRenderer : BaseGradientRenderer
	{
		private double invDistanceScale;
		int _startX;
		int _startY;

		public RadialRenderer (bool alphaOnly, BinaryPixelOp normalBlendOp) : base (alphaOnly, normalBlendOp)
		{
		}

		public override void BeforeRender ()
		{
			var distanceScale = StartPoint.Distance (EndPoint);

			_startX = (int)StartPoint.X;
			_startY = (int)StartPoint.Y;

			if (distanceScale == 0)
				invDistanceScale = 0;
			else
				invDistanceScale = 1f / distanceScale;

			base.BeforeRender ();
		}

		public override byte ComputeByteLerp (int x, int y)
		{
			var dx = x - _startX;
			var dy = y - _startY;

			var distance = Math.Sqrt (dx * dx + dy * dy);

			var result = distance * invDistanceScale;

			if (result < 0.0)
				return 0;

			return result > 1.0 ? (byte)255 : (byte)(result * 255f);
		}
	}
}
