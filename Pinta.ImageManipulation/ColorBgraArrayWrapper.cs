// 
// ByteArrayWrapper.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2012 Jonathan Pobst
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

using System;

namespace Pinta.ImageManipulation
{
	public class ColorBgraArrayWrapper : BaseSurface
	{
		private unsafe ColorBgra* data_ptr;

		public unsafe ColorBgraArrayWrapper (ColorBgra* data, int width, int height)
		{
			data_ptr = data;
			this.height = height;
			this.width = width;
		}

		protected unsafe override ColorBgra* data {
			get { return data_ptr; }
		}

		public unsafe override int Stride {
			get { return width * sizeof (ColorBgra*); }
		}

		public unsafe override void BeginUpdate ()
		{
		}

		public override void EndUpdate ()
		{
		}
	}
}
