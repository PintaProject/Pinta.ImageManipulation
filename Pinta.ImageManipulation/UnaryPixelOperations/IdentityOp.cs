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
	/// Passes through the given color value.
	/// result(color) = color
	/// </summary>
	public class IdentityOp : UnaryPixelOp
	{
		public override ColorBgra Apply (ColorBgra color)
		{
			return color;
		}

		public unsafe override void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			for (int i = 0; i < length; i++) {
				*dst = *src;
				dst++;
				src++;
			}
		}

		public unsafe override void Apply (ColorBgra* ptr, int length)
		{
			return;
		}
	}
}
