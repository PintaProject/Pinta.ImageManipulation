/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Olivier Dufour <olivier.duff@gmail.com>                 //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class PolarInversionEffect : WarpEffect
	{
		private double amount;

		public PolarInversionEffect (double amount, int quality, Point centerOffset, WarpEdgeBehavior edgeBehavior, ColorBgra primaryColor, ColorBgra secondaryColor)
			: base (quality, centerOffset, edgeBehavior, primaryColor, secondaryColor)
		{
			if (amount < -4 || amount > 4)
				throw new ArgumentOutOfRangeException ("amount");

			this.amount = amount;
		}

		#region Algorithm Code Ported From PDN
		protected override void InverseTransform (ref TransformData transData)
		{
			double x = transData.X;
			double y = transData.Y;

			// NOTE: when x and y are zero, this will divide by zero and return NaN
			double invertDistance = Utility.Lerp (1.0, DefaultRadius2 / ((x * x) + (y * y)), amount);

			transData.X = x * invertDistance;
			transData.Y = y * invertDistance;
		}
		#endregion
	}
}
