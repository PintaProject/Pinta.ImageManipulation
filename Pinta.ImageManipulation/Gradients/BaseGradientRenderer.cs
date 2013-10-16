/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pinta.ImageManipulation.Gradients
{
	public abstract class BaseGradientRenderer
	{
		private BinaryPixelOp normalBlendOp;
		private ColorBgra startColor;
		private ColorBgra endColor;

		private bool lerpCacheIsValid = false;
		private byte[] lerpAlphas;
		private ColorBgra[] lerpColors;

		public bool AlphaBlending { get; set; }
		public bool AlphaOnly { get; set; }
		public PointD StartPoint { get; set; }
		public PointD EndPoint { get; set; }

		protected internal BaseGradientRenderer (bool alphaOnly, BinaryPixelOp normalBlendOp)
		{
			this.normalBlendOp = normalBlendOp;
			this.AlphaOnly = alphaOnly;
		}

		public ColorBgra StartColor {
			get { return this.startColor; }
			set {
				if (this.startColor != value) {
					this.startColor = value;
					this.lerpCacheIsValid = false;
				}
			}
		}

		public ColorBgra EndColor {
			get { return this.endColor; }
			set {
				if (this.endColor != value) {
					this.endColor = value;
					this.lerpCacheIsValid = false;
				}
			}
		}

		public virtual void BeforeRender ()
		{
			if (!this.lerpCacheIsValid) {
				byte startAlpha;
				byte endAlpha;
				
				if (this.AlphaOnly) {
					ComputeAlphaOnlyValuesFromColors (this.startColor, this.endColor, out startAlpha, out endAlpha);
				} else {
					startAlpha = this.startColor.A;
					endAlpha = this.endColor.A;
				}
				
				this.lerpAlphas = new byte[256];
				this.lerpColors = new ColorBgra[256];
				
				for (int i = 0; i < 256; ++i) {
					byte a = (byte)i;
					this.lerpColors[a] = ColorBgra.Blend (this.startColor, this.endColor, a);
					this.lerpAlphas[a] = (byte)(startAlpha + ((endAlpha - startAlpha) * a) / 255);
				}
				
				this.lerpCacheIsValid = true;
			}
		}

		public virtual void AfterRender ()
		{
		}

		public abstract byte ComputeByteLerp(int x, int y);

		public unsafe void Render (ISurface surface, params Rectangle[] rois)
		{
			byte startAlpha;
			byte endAlpha;
			
			if (this.AlphaOnly) {
				ComputeAlphaOnlyValuesFromColors (this.startColor, this.endColor, out startAlpha, out endAlpha);
			} else {
				startAlpha = this.startColor.A;
				endAlpha = this.endColor.A;
			}
			
			surface.BeginUpdate ();
			
			for (int ri = 0; ri < rois.Length; ++ri) {
				var rect = rois[ri];
				
				if (this.StartPoint.X == this.EndPoint.X && this.StartPoint.Y == this.EndPoint.Y) {
					// Start and End point are the same ... fill with solid color.
					for (int y = rect.Top; y <= rect.Bottom; ++y) {
						var pixelPtr = surface.GetPointAddress(rect.Left, y);
						
						for (int x = rect.Left; x <= rect.Right; ++x) {
							ColorBgra result;
							
							if (this.AlphaOnly && this.AlphaBlending) {
								byte resultAlpha = (byte)Utility.FastDivideShortByByte ((ushort)(pixelPtr->A * endAlpha), 255);
								result = *pixelPtr;
								result.A = resultAlpha;
							} else if (this.AlphaOnly && !this.AlphaBlending) {
								result = *pixelPtr;
								result.A = endAlpha;
							} else if (!this.AlphaOnly && this.AlphaBlending) {
								result = this.normalBlendOp.Apply (*pixelPtr, this.endColor);
							//if (!this.alphaOnly && !this.alphaBlending)
							} else {
								result = this.endColor;
							}
							
							*pixelPtr = result;
							++pixelPtr;
						}
					}
				} else {
					var mainrect = rect;
					Parallel.ForEach(Enumerable.Range (rect.Top, rect.Bottom + 1),
						(y) => ProcessGradientLine(startAlpha, endAlpha, y, mainrect, surface));
				}
			}
			
			surface.EndUpdate ();
			AfterRender ();
		}

		private static void ComputeAlphaOnlyValuesFromColors (ColorBgra startColor, ColorBgra endColor, out byte startAlpha, out byte endAlpha)
		{
			startAlpha = startColor.A;
			endAlpha = (byte)(255 - endColor.A);
		}

		private unsafe bool ProcessGradientLine (byte startAlpha, byte endAlpha, int y, Rectangle rect, ISurface surface)
		{
			var pixelPtr = surface.GetPointAddress (rect.Left, y);
			var right = rect.Right;
			if (AlphaOnly && AlphaBlending)
			{
				for (var x = rect.Left; x <= right; ++x)
				{
					var lerpByte = ComputeByteLerp(x, y);
					var lerpAlpha = lerpAlphas[lerpByte];
					var resultAlpha = Utility.FastScaleByteByByte(pixelPtr->A, lerpAlpha);
					pixelPtr->A = resultAlpha;
					++pixelPtr;
				}
			}
			else if (AlphaOnly && !AlphaBlending)
			{
				for (var x = rect.Left; x <= right; ++x)
				{
					var lerpByte = ComputeByteLerp(x, y);
					var lerpAlpha = lerpAlphas[lerpByte];
					pixelPtr->A = lerpAlpha;
					++pixelPtr;
				}
			}
			else if (!AlphaOnly && (AlphaBlending && (startAlpha != 255 || endAlpha != 255)))
			{
				// If we're doing all color channels, and we're doing alpha blending, and if alpha blending is necessary
				for (var x = rect.Left; x <= right; ++x)
				{
					var lerpByte = ComputeByteLerp(x, y);
					var lerpColor = lerpColors[lerpByte];
					var result = normalBlendOp.Apply((*pixelPtr), lerpColor);
					*pixelPtr = result;
					++pixelPtr;
				}
				//if (!this.alphaOnly && !this.alphaBlending) // or sC.A == 255 && eC.A == 255
			}
			else
			{
				for (var x = rect.Left; x <= right; ++x)
				{
					var lerpByte = ComputeByteLerp(x, y);
					var lerpColor = lerpColors[lerpByte];
					*pixelPtr = lerpColor;
					++pixelPtr;
				}
			}
			return true;
		}
	}
}
