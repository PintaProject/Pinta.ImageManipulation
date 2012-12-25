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
	public class DesaturateOp : UnaryPixelOp
	{
		public override ColorBgra Apply (ColorBgra color)
		{
			var i = color.GetIntensityByte ();
			return ColorBgra.FromBgra (i, i, i, color.A);
		}

		public unsafe override void Apply (ColorBgra* ptr, int length)
		{
			while (length > 0) {
				var i = ptr->GetIntensityByte ();

				ptr->R = i;
				ptr->G = i;
				ptr->B = i;

				++ptr;
				--length;
			}
		}

		public unsafe override void Apply (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				var i = src->GetIntensityByte ();

				dst->B = i;
				dst->G = i;
				dst->R = i;
				dst->A = src->A;

				++dst;
				++src;
				--length;
			}
		}
	}
}
