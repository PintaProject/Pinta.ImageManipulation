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
	public class LuminosityCurveOp : UnaryPixelOp
	{
		public byte[] Curve = new byte[256];

		public LuminosityCurveOp ()
		{
			for (int i = 0; i < 256; ++i)
				Curve[i] = (byte)i;
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			byte lumi = color.GetIntensityByte ();
			int diff = Curve[lumi] - lumi;

			return ColorBgra.FromBgraClamped (
			    color.B + diff,
			    color.G + diff,
			    color.R + diff,
			    color.A);
		}
	}
}
