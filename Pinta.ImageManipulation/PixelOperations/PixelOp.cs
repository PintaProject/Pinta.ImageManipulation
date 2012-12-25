/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

namespace Pinta.ImageManipulation
{
	public abstract class PixelOp
	{
		public unsafe abstract void Apply (ColorBgra* src, ColorBgra* dst, int length);
	}
}
