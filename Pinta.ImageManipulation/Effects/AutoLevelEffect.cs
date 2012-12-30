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
using System.Threading;

namespace Pinta.ImageManipulation.Effects
{
	public class AutoLevelEffect : BaseEffect
	{
		private LevelOp op;

		/// <summary>
		/// Creates a new effect that will apply an automatic leveling to an image.
		/// </summary>
		public AutoLevelEffect ()
		{
		}

		#region Algorithm Code Ported From PDN
		protected override void OnBeginRender (ISurface src, ISurface dst, Rectangle roi)
		{
			var histogram = new HistogramRgb ();
			histogram.UpdateHistogram (src, src.Bounds);

			op = histogram.MakeLevelsAuto ();
		}

		protected override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			if (op.isValid)
				op.Apply (src, dest, roi);
		}
		#endregion
	}
}
