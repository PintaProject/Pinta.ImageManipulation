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
	/// Specialization of SetChannel that sets the alpha channel.
	/// </summary>
	/// <remarks>This class depends on the system being litte-endian with the alpha channel 
	/// occupying the 8 most-significant-bits of a ColorBgra instance.
	/// By the way, we use addition instead of bitwise-OR because an addition can be
	/// perform very fast (0.5 cycles) on a Pentium 4.</remarks>
	public class SetAlphaChannelOp : UnaryPixelOp
	{
		private UInt32 add_value;

		public SetAlphaChannelOp (byte alphaValue)
		{
			add_value = (uint)alphaValue << 24;
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			return ColorBgra.FromUInt32 ((color.Bgra & 0x00ffffff) + add_value);
		}

		public override unsafe void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				dst->Bgra = (src->Bgra & 0x00ffffff) + add_value;
				++dst;
				++src;
				--length;
			}
		}

		public override unsafe void Apply (ColorBgra* ptr, int length)
		{
			while (length > 0) {
				ptr->Bgra = (ptr->Bgra & 0x00ffffff) + add_value;
				++ptr;
				--length;
			}
		}
	}
}
