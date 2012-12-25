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
	public class LevelOp : ChannelCurveOp, ICloneable
	{
		private ColorBgra colorInLow;
		private ColorBgra colorInHigh;
		private ColorBgra colorOutLow;
		private ColorBgra colorOutHigh;

		private float[] gamma = new float[3];
		public bool isValid = true;

		public LevelOp ()
			: this (ColorBgra.Black,
			       ColorBgra.White,
			       new float[] { 1, 1, 1 },
			       ColorBgra.Black,
			       ColorBgra.White)
		{
		}

		public LevelOp (ColorBgra in_lo, ColorBgra in_hi, float[] gamma, ColorBgra out_lo, ColorBgra out_hi)
		{
			colorInLow = in_lo;
			colorInHigh = in_hi;
			colorOutLow = out_lo;
			colorOutHigh = out_hi;

			if (gamma.Length != 3)
				throw new ArgumentException ("gamma", "gamma must be a float[3]");

			this.gamma = gamma;
			UpdateLookupTable ();
		}

		public ColorBgra Apply (float r, float g, float b)
		{
			ColorBgra ret = new ColorBgra ();
			float[] input = new float[] { b, g, r };

			for (int i = 0; i < 3; i++) {
				float v = (input[i] - colorInLow[i]);

				if (v < 0) {
					ret[i] = colorOutLow[i];
				} else if (v + colorInLow[i] >= colorInHigh[i]) {
					ret[i] = colorOutHigh[i];
				} else {
					ret[i] = (byte)Utility.Clamp (
					    colorOutLow[i] + (colorOutHigh[i] - colorOutLow[i]) * Math.Pow (v / (colorInHigh[i] - colorInLow[i]), gamma[i]),
					    0.0f,
					    255.0f);
				}
			}

			return ret;
		}

		public ColorBgra ColorInLow
		{
			get
			{
				return colorInLow;
			}

			set
			{
				if (value.R == 255) {
					value.R = 254;
				}

				if (value.G == 255) {
					value.G = 254;
				}

				if (value.B == 255) {
					value.B = 254;
				}

				if (colorInHigh.R < value.R + 1) {
					colorInHigh.R = (byte)(value.R + 1);
				}

				if (colorInHigh.G < value.G + 1) {
					colorInHigh.G = (byte)(value.R + 1);
				}

				if (colorInHigh.B < value.B + 1) {
					colorInHigh.B = (byte)(value.R + 1);
				}

				colorInLow = value;
				UpdateLookupTable ();
			}
		}

		public ColorBgra ColorInHigh
		{
			get
			{
				return colorInHigh;
			}

			set
			{
				if (value.R == 0) {
					value.R = 1;
				}

				if (value.G == 0) {
					value.G = 1;
				}

				if (value.B == 0) {
					value.B = 1;
				}

				if (colorInLow.R > value.R - 1) {
					colorInLow.R = (byte)(value.R - 1);
				}

				if (colorInLow.G > value.G - 1) {
					colorInLow.G = (byte)(value.R - 1);
				}

				if (colorInLow.B > value.B - 1) {
					colorInLow.B = (byte)(value.R - 1);
				}

				colorInHigh = value;
				UpdateLookupTable ();
			}
		}

		public ColorBgra ColorOutLow
		{
			get
			{
				return colorOutLow;
			}

			set
			{
				if (value.R == 255) {
					value.R = 254;
				}

				if (value.G == 255) {
					value.G = 254;
				}

				if (value.B == 255) {
					value.B = 254;
				}

				if (colorOutHigh.R < value.R + 1) {
					colorOutHigh.R = (byte)(value.R + 1);
				}

				if (colorOutHigh.G < value.G + 1) {
					colorOutHigh.G = (byte)(value.G + 1);
				}

				if (colorOutHigh.B < value.B + 1) {
					colorOutHigh.B = (byte)(value.B + 1);
				}

				colorOutLow = value;
				UpdateLookupTable ();
			}
		}

		public ColorBgra ColorOutHigh
		{
			get
			{
				return colorOutHigh;
			}

			set
			{
				if (value.R == 0) {
					value.R = 1;
				}

				if (value.G == 0) {
					value.G = 1;
				}

				if (value.B == 0) {
					value.B = 1;
				}

				if (colorOutLow.R > value.R - 1) {
					colorOutLow.R = (byte)(value.R - 1);
				}

				if (colorOutLow.G > value.G - 1) {
					colorOutLow.G = (byte)(value.G - 1);
				}

				if (colorOutLow.B > value.B - 1) {
					colorOutLow.B = (byte)(value.B - 1);
				}

				colorOutHigh = value;
				UpdateLookupTable ();
			}
		}

		public float GetGamma (int index)
		{
			if (index < 0 || index >= 3) {
				throw new ArgumentOutOfRangeException ("index", index, "Index must be between 0 and 2");
			}

			return gamma[index];
		}

		public void SetGamma (int index, float val)
		{
			if (index < 0 || index >= 3) {
				throw new ArgumentOutOfRangeException ("index", index, "Index must be between 0 and 2");
			}

			gamma[index] = Utility.Clamp (val, 0.1f, 10.0f);
			UpdateLookupTable ();
		}

		public static LevelOp AutoFromLoMdHi (ColorBgra lo, ColorBgra md, ColorBgra hi)
		{
			float[] gamma = new float[3];

			for (int i = 0; i < 3; i++) {
				if (lo[i] < md[i] && md[i] < hi[i]) {
					gamma[i] = (float)Utility.Clamp (Math.Log (0.5, (float)(md[i] - lo[i]) / (float)(hi[i] - lo[i])), 0.1, 10.0);
				} else {
					gamma[i] = 1.0f;
				}
			}

			return new LevelOp (lo, hi, gamma, ColorBgra.Black, ColorBgra.White);
		}

		private void UpdateLookupTable ()
		{
			for (int i = 0; i < 3; i++) {
				if (colorOutHigh[i] < colorOutLow[i] ||
				    colorInHigh[i] <= colorInLow[i] ||
				    gamma[i] < 0) {
					isValid = false;
					return;
				}

				for (int j = 0; j < 256; j++) {
					ColorBgra col = Apply (j, j, j);
					CurveB[j] = col.B;
					CurveG[j] = col.G;
					CurveR[j] = col.R;
				}
			}
		}

		public void UnApply (ColorBgra after, float[] beforeOut, float[] slopesOut)
		{
			if (beforeOut.Length != 3) {
				throw new ArgumentException ("before must be a float[3]", "before");
			}

			if (slopesOut.Length != 3) {
				throw new ArgumentException ("slopes must be a float[3]", "slopes");
			}

			for (int i = 0; i < 3; i++) {
				beforeOut[i] = colorInLow[i] + (colorInHigh[i] - colorInLow[i]) *
				    (float)Math.Pow ((float)(after[i] - colorOutLow[i]) / (colorOutHigh[i] - colorOutLow[i]), 1 / gamma[i]);

				slopesOut[i] = (float)(colorInHigh[i] - colorInLow[i]) / ((colorOutHigh[i] - colorOutLow[i]) * gamma[i]) *
				    (float)Math.Pow ((float)(after[i] - colorOutLow[i]) / (colorOutHigh[i] - colorOutLow[i]), 1 / gamma[i] - 1);

				if (float.IsInfinity (slopesOut[i]) || float.IsNaN (slopesOut[i])) {
					slopesOut[i] = 0;
				}
			}
		}

		public object Clone ()
		{
			LevelOp copy = new LevelOp (colorInLow, colorInHigh, (float[])gamma.Clone (), colorOutLow, colorOutHigh);

			copy.CurveB = (byte[])this.CurveB.Clone ();
			copy.CurveG = (byte[])this.CurveG.Clone ();
			copy.CurveR = (byte[])this.CurveR.Clone ();

			return copy;
		}
	}
}
