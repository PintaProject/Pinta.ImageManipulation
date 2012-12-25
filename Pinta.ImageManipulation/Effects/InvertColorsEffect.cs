/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Jonathan Pobst <monkey@jpobst.com>                      //
/////////////////////////////////////////////////////////////////////////////////

using System;
using Pinta.ImageManipulation.UnaryPixelOperations;

namespace Pinta.ImageManipulation.Effects
{
	public class InvertColorsEffect : BaseEffect
	{
		private InvertOp op = new InvertOp ();

		#region Algorithm Code Ported From PDN
		protected override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			op.Apply (src, dest, roi);
		}
		#endregion
	}
}
