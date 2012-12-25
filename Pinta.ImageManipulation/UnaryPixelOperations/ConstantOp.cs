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
	/// Always returns a constant color.
	/// </summary>
	public class ConstantOp : UnaryPixelOp
	{
		private ColorBgra set_color;

		public ConstantOp (ColorBgra setColor)
		{
			set_color = setColor;
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			return set_color;
		}

		public unsafe override void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				*dst = set_color;
				++dst;
				--length;
			}
		}

		public unsafe override void Apply (ColorBgra* ptr, int length)
		{
			while (length > 0) {
				*ptr = set_color;
				++ptr;
				--length;
			}
		}
	}
}
