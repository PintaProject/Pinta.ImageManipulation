// 
// ParallelExtensions.cs
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
using System.Threading;
using System.Threading.Tasks;

namespace Pinta.ImageManipulation
{
	public static class ParallelExtensions
	{
		// The regular Parallel.For executes the loop out of order.  For image
		// processing, we want to go mainly in order so the live preview works
		// sequentially from top to bottom.  It won't be exact, but we just want
		// something that looks good enough.
		public static ParallelLoopResult OrderedFor (int fromInclusive, int toExclusive, CancellationToken token, Action<int> body)
		{
			int i = fromInclusive - 1;

			return Parallel.For (fromInclusive, toExclusive, (x) => {
				if (token.IsCancellationRequested)
					return;

				int y = Interlocked.Increment (ref i);
				body (y);
			});
		}
	}
}
