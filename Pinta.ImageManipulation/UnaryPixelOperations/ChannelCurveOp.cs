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
	public class ChannelCurveOp : UnaryPixelOp
	{
		public byte[] CurveB = new byte[256];
		public byte[] CurveG = new byte[256];
		public byte[] CurveR = new byte[256];

		public ChannelCurveOp ()
		{
			for (int i = 0; i < 256; ++i) {
				CurveB[i] = (byte)i;
				CurveG[i] = (byte)i;
				CurveR[i] = (byte)i;
			}
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			return ColorBgra.FromBgra (CurveB[color.B], CurveG[color.G], CurveR[color.R], color.A);
		}

		public override unsafe void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (--length >= 0) {
				dst->B = CurveB[src->B];
				dst->G = CurveG[src->G];
				dst->R = CurveR[src->R];
				dst->A = src->A;

				++dst;
				++src;
			}
		}

		public override unsafe void Apply (ColorBgra* ptr, int length)
		{
			while (--length >= 0) {
				ptr->B = CurveB[ptr->B];
				ptr->G = CurveG[ptr->G];
				ptr->R = CurveR[ptr->R];

				++ptr;
			}
		}
	}
}
