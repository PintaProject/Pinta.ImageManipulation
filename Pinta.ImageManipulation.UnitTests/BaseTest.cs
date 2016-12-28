// 
// BaseTest.cs
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

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Pinta.ImageManipulation.UnitTests
{
	public abstract class BaseTest
	{
        private List<Cairo.Surface> surfaces;

        [SetUp]
        public void Init ()
        {
            surfaces = new List<Cairo.Surface> ();
        }

        [TearDown]
        public void Dispose ()
        {
            foreach (var surface in surfaces)
                surface.Dispose ();

            surfaces.Clear ();
        }

        private BaseSurface GetImage (string prefix, string name)
        {
            var surface = new Cairo.ImageSurface (Path.Combine (prefix, name));
            surfaces.Add (surface);
            return new CairoSurfaceWrapper (surface);
        }

        protected BaseSurface GetSourceImage (string name)
		{
            return GetImage ("SourceBitmaps", name);
		}

        protected BaseSurface GetExpectedImage (string name)
		{
            return GetImage ("ExpectedBitmaps", name);
		}

        protected unsafe void Compare (BaseSurface actual, string expected)
		{
			var exp_img = GetExpectedImage (expected);

            var act_data = actual.GetRowAddress (0);
            var exp_data = exp_img.GetRowAddress (0);

			var diff = 0;

			for (var i = 0; i < actual.Width * actual.Height; i++)
                if (*(act_data++) != *(exp_data++))
					diff++;

            // Save out the incorrect file.
            if (diff != 0)
            {
                string results_dir = "TestResults";
                Directory.CreateDirectory (results_dir);
                var surface = ((CairoSurfaceWrapper)actual).Surface;
                surface.WriteToPng (Path.Combine (results_dir, expected));
            }
            
			Assert.AreEqual (0, diff);
		}
	}
}
