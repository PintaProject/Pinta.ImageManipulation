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
	/// Blends pixels with the specified constant color.
	/// </summary>
	public class BlendConstantOp : UnaryPixelOp
	{
		private ColorBgra blend_color;

		public BlendConstantOp (ColorBgra blendColor)
		{
			blend_color = blendColor;
		}

		public override ColorBgra Apply (ColorBgra color)
		{
			int a = blend_color.A;
			int invA = 255 - a;

			int r = ((color.R * invA) + (blend_color.R * a)) / 256;
			int g = ((color.G * invA) + (blend_color.G * a)) / 256;
			int b = ((color.B * invA) + (blend_color.B * a)) / 256;
			byte a2 = ComputeAlpha (color.A, blend_color.A);

			return ColorBgra.FromBgra ((byte)b, (byte)g, (byte)r, a2);
		}

		/// <summary>
		/// Computes alpha for r OVER l operation.
		/// </summary>
		private static byte ComputeAlpha (byte la, byte ra)
		{
			return (byte)(((la * (256 - (ra + (ra >> 7)))) >> 8) + ra);
		}
	}
}
