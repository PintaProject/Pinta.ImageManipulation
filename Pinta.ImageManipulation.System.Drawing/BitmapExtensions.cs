using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation
{
	public static class BitmapExtensions
	{
		public static void Render (this BaseEffect effect, Bitmap source)
		{
			var wrapper = new BitmapWrapper (source);

			effect.Render (wrapper);
		}
	}
}
