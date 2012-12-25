/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation.UnaryPixelOperations
{
	/// <summary>
	/// Averages the input color's red, green, and blue channels. The alpha component
	/// is unaffected.
	/// </summary>
	public class AverageChannelsOp : UnaryPixelOp
	{
		public override ColorBgra Apply (ColorBgra color)
		{
			var average = (byte)(((int)color.R + (int)color.G + (int)color.B) / 3);
			return ColorBgra.FromBgra (average, average, average, color.A);
		}
	}
}
