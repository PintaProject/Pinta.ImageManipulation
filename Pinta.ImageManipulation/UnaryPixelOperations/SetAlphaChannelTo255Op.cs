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
	/// Specialization of SetAlphaChannel that always sets alpha to 255.
	/// </summary>
	public class SetAlphaChannelTo255Op : UnaryPixelOp
	{
		public override ColorBgra Apply (ColorBgra color)
		{
			return ColorBgra.FromUInt32 (color.Bgra | 0xff000000);
		}

		public override unsafe void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				dst->Bgra = src->Bgra | 0xff000000;
				++dst;
				++src;
				--length;
			}
		}

		public override unsafe void Apply (ColorBgra* ptr, int length)
		{
			while (length > 0) {
				ptr->Bgra |= 0xff000000;
				++ptr;
				--length;
			}
		}
	}
}
