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
	/// Used to set a given channel of a pixel to a given, predefined color.
	/// Useful if you want to set only the alpha value of a given region.
	/// </summary>
	public class SetChannelOp : UnaryPixelOp
	{
		private int channel;
		private byte set_value;

		public SetChannelOp (int channel, byte setValue)
		{
			this.channel = channel;
			this.set_value = setValue;
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			color[channel] = set_value;
			return color;
		}

		public override unsafe void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				*dst = *src;
				(*dst)[channel] = set_value;
				++dst;
				++src;
				--length;
			}
		}

		public override unsafe void Apply (ColorBgra* ptr, int length)
		{
			while (length > 0) {
				(*ptr)[channel] = set_value;
				++ptr;
				--length;
			}
		}
	}
}
