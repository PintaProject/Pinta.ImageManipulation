/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Olivier Dufour <olivier.duff@gmail.com>                 //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Pinta.ImageManipulation.Effects
{
	public class CloudsEffect : BaseEffect
	{
		private int scale;
		private int power;
		private int seed;
		private BlendMode blend_mode;
		private ColorBgra from_color;
		private ColorBgra to_color;
		private UserBlendOp blend_op;

		// This is so that repetition of the effect with CTRL+F actually shows up differently.
		//private byte instanceSeed = unchecked ((byte)DateTime.Now.Ticks);
		static private int[] permuteLookup = new int[512];

		public CloudsEffect (int scale, int power, int seed, ColorBgra fromColor, ColorBgra toColor, BlendMode blendMode)
		{
			if (scale < 2 || scale > 1000)
				throw new ArgumentOutOfRangeException ("scale");
			if (power < 0 || power > 100)
				throw new ArgumentOutOfRangeException ("radius");

			this.scale = scale;
			this.power = power;
			this.seed = seed;
			this.from_color = fromColor;
			this.to_color = toColor;
			this.blend_mode = blendMode;

			blend_op = Utility.GetBlendModeOp (blend_mode);
		}

		static CloudsEffect ()
		{
			for (int i = 0; i < 256; i++) {
				permuteLookup[256 + i] = permutationTable[i];
				permuteLookup[i] = permutationTable[i];
			}
		}

		#region Algorithm Code Ported From PDN
		// Adapted to 2-D version in C# from 3-D version in Java from http://mrl.nyu.edu/~perlin/noise/
		protected override void RenderLine (ISurface src, ISurface dst, Rectangle roi)
		{
			RenderClouds (dst, roi, scale, (byte)(seed), power / 100.0, from_color, to_color);
			blend_op.Apply (src, dst, dst, roi);
		}

		private static double Fade (double t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		private static double Grad (int hash, double x, double y)
		{
			int h = hash & 15;
			double u = h < 8 ? x : y;
			double v = h < 4 ? y : h == 12 || h == 14 ? x : 0;

			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		private static double Noise (byte ix, byte iy, double x, double y, byte seed)
		{
			double u = Fade (x);
			double v = Fade (y);

			int a = permuteLookup[ix + seed] + iy;
			int aa = permuteLookup[a];
			int ab = permuteLookup[a + 1];
			int b = permuteLookup[ix + 1 + seed] + iy;
			int ba = permuteLookup[b];
			int bb = permuteLookup[b + 1];

			double gradAA = Grad (permuteLookup[aa], x, y);
			double gradBA = Grad (permuteLookup[ba], x - 1, y);

			double edge1 = Utility.Lerp (gradAA, gradBA, u);

			double gradAB = Grad (permuteLookup[ab], x, y - 1);
			double gradBB = Grad (permuteLookup[bb], x - 1, y - 1);

			double edge2 = Utility.Lerp (gradAB, gradBB, u);

			return Utility.Lerp (edge1, edge2, v);
		}

		private unsafe static void RenderClouds (ISurface surface, Rectangle rect, int scale, byte seed, double power, ColorBgra colorFrom, ColorBgra colorTo)
		{
			int w = surface.Width;
			int h = surface.Height;

			for (int y = rect.Top; y <= rect.Bottom; ++y) {
				ColorBgra* ptr = surface.GetPointAddress (rect.Left, y);
				int dy = 2 * y - h;

				for (int x = rect.Left; x <= rect.Right; ++x) {
					int dx = 2 * x - w;
					double val = 0;
					double mult = 1;
					int div = scale;

					for (int i = 0; i < 12 && mult > 0.03 && div > 0; ++i) {
						double dxr = 65536 + (double)dx / (double)div;
						double dyr = 65536 + (double)dy / (double)div;

						int dxd = (int)dxr;
						int dyd = (int)dyr;

						dxr -= dxd;
						dyr -= dyd;

						double noise = Noise (
						    unchecked ((byte)dxd),
						    unchecked ((byte)dyd),
						    dxr, //(double)dxr / div,
						    dyr, //(double)dyr / div,
						    (byte)(seed ^ i));

						val += noise * mult;
						div /= 2;
						mult *= power;
					}

					*ptr = ColorBgra.Lerp (colorFrom, colorTo, (val + 1) / 2);
					++ptr;
				}
			}
		}

		static private int[] permutationTable = new int[]
		{
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7,
			225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6,
			148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35,
			11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171,
			168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231,
			83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245,
			40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76,
			132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
			164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
			5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47,
			16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2,
			44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39,
			253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218,
			246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162,
			241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181,
			199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150,
			254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128,
			195, 78, 66, 215, 61, 156, 180
		};
		#endregion
	}
}
