/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzysztof Marecki <marecki.krzysztof@gmail.com>         //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Pinta.ImageManipulation.UnaryPixelOperations;

namespace Pinta.ImageManipulation.Effects
{
	public class CurvesEffect : BaseEffect
	{
		UnaryPixelOp op = null;

		public CurvesEffect (SortedList<int, int>[] controlPoints, ColorTransferMode mode)
		{
			op = MakeUop (controlPoints, mode);
		}

		#region Algorithm Code Ported From PDN
		protected override void RenderLine (ISurface src, ISurface dest, Rectangle roi)
		{
			op.Apply (src, dest, roi);
		}

		private UnaryPixelOp MakeUop (SortedList<int, int>[] controlPoints, ColorTransferMode mode)
		{
			UnaryPixelOp op;
			byte[][] transferCurves;
			int entries;

			switch (mode) {
				case ColorTransferMode.Rgb:
					var cc = new ChannelCurveOp ();
					transferCurves = new byte[][] { cc.CurveR, cc.CurveG, cc.CurveB };
					entries = 256;
					op = cc;
					break;

				case ColorTransferMode.Luminosity:
					var lc = new LuminosityCurveOp ();
					transferCurves = new byte[][] { lc.Curve };
					entries = 256;
					op = lc;
					break;

				default:
					throw new InvalidEnumArgumentException ();
			}


			int channels = transferCurves.Length;

			for (int channel = 0; channel < channels; channel++) {
				var channelControlPoints = controlPoints[channel];
				var xa = channelControlPoints.Keys;
				var ya = channelControlPoints.Values;
				SplineInterpolator interpolator = new SplineInterpolator ();
				int length = channelControlPoints.Count;

				for (int i = 0; i < length; i++) {
					interpolator.Add (xa[i], ya[i]);
				}

				for (int i = 0; i < entries; i++) {
					transferCurves[channel][i] = Utility.ClampToByte (interpolator.Interpolate (i));
				}
			}

			return op;
		}
		#endregion
	}
}
