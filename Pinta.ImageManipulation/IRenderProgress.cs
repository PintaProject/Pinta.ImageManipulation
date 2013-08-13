using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.ImageManipulation
{
	public class IRenderProgress
	{
		public Rectangle CompletedRoi { get; set; }
		public double PercentComplete { get; set; }
	}
}
