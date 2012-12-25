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
	/// Inverts a pixel's color, and passes through the alpha component.
	/// </summary>
	public class InvertOp : UnaryPixelOp
	{
		public override ColorBgra Apply (ColorBgra color)
		{
			return ColorBgra.FromBgra ((byte)(255 - color.B), (byte)(255 - color.G), (byte)(255 - color.R), color.A);
		}
	}
}
