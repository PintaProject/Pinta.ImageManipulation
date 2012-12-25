/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Marco Rolappe <m_rolappe@gmx.net>                       //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class ReliefEffect : ColorDifferenceEffect
	{
		private double angle;

		public ReliefEffect (double angle)
		{
			this.angle = angle;
		}

		#region Algorithm Code Ported From PDN
		protected override void RenderLine (ISurface src, ISurface dst, Rectangle roi)
		{
			base.RenderColorDifferenceEffect (Weights, src, dst, roi);
		}

		private double[][] Weights {
			get {
				// adjust and convert angle to radians
				var r = (double)angle * 2.0 * Math.PI / 360.0;

				// angle delta for each weight
				var dr = Math.PI / 4.0;

				// for r = 0 this builds an Relief filter pointing straight left
				var weights = new double[3][];

				for (uint idx = 0; idx < 3; ++idx)
					weights[idx] = new double[3];

				weights[0][0] = Math.Cos (r + dr);
				weights[0][1] = Math.Cos (r + 2.0 * dr);
				weights[0][2] = Math.Cos (r + 3.0 * dr);

				weights[1][0] = Math.Cos (r);
				weights[1][1] = 1;
				weights[1][2] = Math.Cos (r + 4.0 * dr);

				weights[2][0] = Math.Cos (r - dr);
				weights[2][1] = Math.Cos (r - 2.0 * dr);
				weights[2][2] = Math.Cos (r - 3.0 * dr);

				return weights;
			}
		}
		#endregion
	}
}
