// 
// BaseEffect.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2010 Jonathan Pobst
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Pinta.ImageManipulation.UnaryPixelOperations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pinta.ImageManipulation
{
	public abstract class BaseEffect
	{
		/// <summary>
		/// Render the effect on the specified surface.
		/// </summary>
		/// <param name="surface">Surface to use a the source and destination.</param>
		public virtual void Render (ISurface surface)
		{
			Render (surface, surface.Bounds);
		}

		/// <summary>
		/// Render the effect from the source surface to the destination surface.
		/// </summary>
		/// <param name="src">The source surface.</param>
		/// <param name="dst">The destination surface.</param>
		public virtual void Render (ISurface src, ISurface dst)
		{
			if (src.Bounds != dst.Bounds)
				throw new InvalidOperationException ("Source and destination surfaces must be the same size.");

			Render (src, dst, src.Bounds);
		}

		/// <summary>
		/// Render the effect on the specified surface within the specified rectangle of interest.
		/// </summary>
		/// <param name="surface">Surface to use a the source and destination.</param>
		/// <param name="roi">A rectangle of interest (roi) specifying the area(s) to modify. Only these areas should be modified.</param>
		public void Render (ISurface surface, Rectangle roi)
		{
			RenderLoop (surface, roi, CancellationToken.None, null);
		}

		/// <summary>
		/// Render the effect from the source surface to the destination surface.
		/// </summary>
		/// <param name="src">The source surface.</param>
		/// <param name="dst">The destination surface.</param>
		/// <param name="roi">A rectangle of interest (roi) specifying the area(s) to modify. Only these areas should be modified.</param>
		public void Render (ISurface src, ISurface dst, Rectangle roi)
		{
			RenderLoop (src, dst, roi, CancellationToken.None, null);
		}

		public Task RenderAsync (ISurface surface)
		{
			return RenderAsync (surface, CancellationToken.None);
		}

		public Task RenderAsync (ISurface src, ISurface dst)
		{
			return RenderAsync (src, dst, CancellationToken.None);
		}

		public Task RenderAsync (ISurface surface, CancellationToken token)
		{
			return RenderAsync (surface, surface.Bounds, token);
		}

		public Task RenderAsync (ISurface src, ISurface dst, CancellationToken token)
		{
			if (src.Bounds != dst.Bounds)
				throw new InvalidOperationException ("Source and destination surfaces must be the same size.");

			return RenderAsync (src, dst, src.Bounds, token);
		}

		public Task RenderAsync (ISurface surface, Rectangle roi)
		{
			return RenderAsync (surface, roi, CancellationToken.None);
		}

		public Task RenderAsync (ISurface src, ISurface dst, Rectangle roi)
		{
			return RenderAsync (src, dst, roi, CancellationToken.None);
		}

		public Task RenderAsync (ISurface surface, Rectangle roi, CancellationToken token)
		{
			return Task.Factory.StartNew (() => RenderLoop (surface, roi, token, null));
		}

		public Task RenderAsync (ISurface src, ISurface dst, Rectangle roi, CancellationToken token)
		{
			return Task.Factory.StartNew (() => RenderLoop (src, dst, roi, token, null));
		}

		public Task RenderAsync (ISurface surface, Rectangle roi, CancellationToken token, IRenderProgress progress)
		{
			return Task.Factory.StartNew (() => RenderLoop (surface, roi, token, progress));
		}

		public Task RenderAsync (ISurface src, ISurface dst, Rectangle roi, CancellationToken token, IRenderProgress progress)
		{
			return Task.Factory.StartNew (() => RenderLoop (src, dst, roi, token, progress));
		}

		protected virtual void RenderLoop (ISurface src, ISurface dst, Rectangle roi, CancellationToken token, IRenderProgress progress)
		{
			src.BeginUpdate ();
			dst.BeginUpdate ();

			OnBeginRender (src, dst, roi);

			var completed_lines = new bool[roi.Height];
			var last_completed_index = 0;

			if (Settings.SingleThreaded || roi.Height <= 1) {
				for (var y = roi.Y; y <= roi.Bottom; ++y) {
					if (token.IsCancellationRequested)
						return;

					RenderLine (src, dst, new Rectangle (roi.X, y, roi.Width, 1));

					completed_lines[y - roi.Top] = true;

					if (progress != null) {
						var last_y = FindLastCompletedLine (completed_lines, last_completed_index);
						last_completed_index = last_y;
						progress.CompletedRoi = new Rectangle (roi.X, roi.Y, roi.Width, last_y);
						progress.PercentComplete = (float)last_y / (float)roi.Height;
					}
				}
			} else {
				ParallelExtensions.OrderedFor (roi.Y, roi.Bottom + 1, token, (y) => {
					RenderLine (src, dst, new Rectangle (roi.X, y, roi.Width, 1));

					completed_lines[y - roi.Top] = true;

					if (progress != null) {
						var last_y = FindLastCompletedLine (completed_lines, last_completed_index);
						last_completed_index = last_y;
						progress.CompletedRoi = new Rectangle (roi.X, roi.Y, roi.Width, last_y);
						progress.PercentComplete = (float)last_y / (float)roi.Height;
					}
				});
			}

			src.EndUpdate ();
			dst.EndUpdate ();
		}

		protected unsafe virtual void RenderLoop (ISurface surface, Rectangle roi, CancellationToken token, IRenderProgress progress)
		{
			var dst = new ColorBgra[surface.Height * surface.Width];

			fixed (ColorBgra* dst_ptr = dst) {
				var dst_wrap = new ColorBgraArrayWrapper (dst_ptr, surface.Width, surface.Height);

				surface.BeginUpdate ();
				dst_wrap.BeginUpdate ();

				OnBeginRender (surface, dst_wrap, roi);

				var completed_lines = new bool[roi.Height];
				var last_completed_index = 0;

				if (Settings.SingleThreaded || roi.Height <= 1) {
					for (var y = roi.Y; y <= roi.Bottom; ++y) {
						if (token.IsCancellationRequested)
							return;

						RenderLine (surface, dst_wrap, new Rectangle (roi.X, y, roi.Width, 1));

						completed_lines[y - roi.Top] = true;

						if (progress != null) {
							var last_y = FindLastCompletedLine (completed_lines, last_completed_index);
							last_completed_index = last_y;
							progress.CompletedRoi = new Rectangle (roi.X, roi.Y, roi.Width, last_y);
							progress.PercentComplete = (float)last_y / (float)roi.Height;
						}
					}
				} else {
					ParallelExtensions.OrderedFor (roi.Y, roi.Bottom + 1, token, (y) => {
						RenderLine (surface, dst_wrap, new Rectangle (roi.X, y, roi.Width, 1));

						completed_lines[y - roi.Top] = true;

						if (progress != null) {
							var last_y = FindLastCompletedLine (completed_lines, last_completed_index);
							last_completed_index = last_y;
							progress.CompletedRoi = new Rectangle (roi.X, roi.Y, roi.Width, last_y);
							progress.PercentComplete = (float)last_y / (float)roi.Height;
						}
					});
				}

				// Copy the result from our temp destination back into the source
				var op = new IdentityOp ();
				op.ApplyAsync (dst_wrap, surface, token).Wait ();

				surface.EndUpdate ();
				dst_wrap.EndUpdate ();
			}
		}

		/// <summary>
		/// Performs the actual work of rendering an effect. Do not call base.Render ().
		/// </summary>
		/// <param name="src">The source surface. DO NOT MODIFY.</param>
		/// <param name="dst">The destination surface.</param>
		/// <param name="roi">A rectangle of interest (roi) specifying the area to modify. Only these areas should be modified</param>
		protected unsafe virtual void RenderLine (ISurface src, ISurface dst, Rectangle roi)
		{
			var srcPtr = src.GetPointAddress (roi.X, roi.Y);
			var dstPtr = dst.GetPointAddress (roi.X, roi.Y);
			Render (srcPtr, dstPtr, roi.Width);
		}

		/// <summary>
		/// Performs the actual work of rendering an effect. This overload represent a single line of the image. Do not call base.Render ().
		/// </summary>
		/// <param name="src">The source surface. DO NOT MODIFY.</param>
		/// <param name="dst">The destination surface.</param>
		/// <param name="length">The number of pixels to render.</param>
		protected unsafe virtual void Render (ColorBgra* src, ColorBgra* dst, int length)
		{
			while (length > 0) {
				*dst = Render (*src);
				++dst;
				++src;
				--length;
			}
		}

		/// <summary>
		/// Performs the actual work of rendering an effect. This overload represent a single pixel of the image.
		/// </summary>
		/// <param name="color">The color of the source surface pixel.</param>
		/// <returns>The color to be used for the destination pixel.</returns>
		protected virtual ColorBgra Render (ColorBgra color)
		{
			return color;
		}

		protected virtual void OnBeginRender (ISurface src, ISurface dst, Rectangle roi)
		{
		}

		// We always want to return a contiguous roi of lines completed, even
		// if it means we don't report some lines that we've already completed.
		private int FindLastCompletedLine (bool[] lines, int start)
		{
			for (var i = start; i < lines.Length; i++)
				if (!lines[i])
					return Math.Max (i - 1, 0);

			return lines.Length - 1;
		}
	}
}
