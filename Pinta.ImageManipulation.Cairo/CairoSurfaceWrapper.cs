using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation
{
	public class CairoSurfaceWrapper : BaseSurface
	{
		private Cairo.ImageSurface surface;
		private unsafe ImageManipulation.ColorBgra* data_ptr;

		public unsafe CairoSurfaceWrapper (Cairo.ImageSurface surface)
		{
			this.surface = surface;
			this.data_ptr = (ImageManipulation.ColorBgra*)surface.DataPtr;
			height = surface.Height;
			width = surface.Width;
		}

		protected unsafe override ImageManipulation.ColorBgra* data {
			get { return data_ptr; }
		}

		public override int Stride {
			get { return surface.Stride; }
		}

		public override void BeginUpdate ()
		{
			surface.Flush ();
		}

		public override void EndUpdate ()
		{
			surface.MarkDirty ();
		}
	}
}
