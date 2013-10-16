/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation.Gradients
{
	public abstract class BaseLinearGradientRenderer : BaseGradientRenderer
	{
		protected double dtdx;
		protected double dtdy;

		protected internal BaseLinearGradientRenderer (bool alphaOnly, BinaryPixelOp normalBlendOp) : base (alphaOnly, normalBlendOp)
		{
		}

		public override void BeforeRender ()
		{
			var vec = new PointD (EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
			double mag = vec.Magnitude ();

			if (EndPoint.X == StartPoint.X)
				dtdx = 0;
			else
				dtdx = vec.X / (mag * mag);

			if (EndPoint.Y == StartPoint.Y)
				dtdy = 0;
			else
				dtdy = vec.Y / (mag * mag);

			base.BeforeRender ();
		}
	}
}
