using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Pinta.ImageManipulation.Effects;
using Pinta.ImageManipulation;
using System.IO;
using System.Threading;

namespace Pinta.TestHarness
{
	class Program
	{
		static void Main (string[] args)
		{
			var src_bitmap = new System.Drawing.Bitmap (@"C:\Users\Jonathan\Desktop\helo.png");
			var dst_bitmap = new System.Drawing.Bitmap (src_bitmap.Width, src_bitmap.Height);

			Console.WriteLine ("Image Size: {0}x{1}", src_bitmap.Width, src_bitmap.Height);
			Console.WriteLine ("-------------------------------");

			var src_wrap = new BitmapWrapper (src_bitmap);
			var dst_wrap = new BitmapWrapper (dst_bitmap);

			src_wrap.BeginUpdate ();
			dst_wrap.BeginUpdate ();

			int runs = 1;

			foreach (var effect in GetEffects ()) {
				Settings.SingleThreaded = false;

				// Run once to ensure effect is jitted
				effect.Render (src_wrap, dst_wrap);

				var sw = new Stopwatch ();
				sw.Start ();

				for (int i = 0; i < runs; i++) {
					var tcs = new CancellationTokenSource ();
					var t = effect.RenderAsync (src_wrap, dst_wrap, tcs.Token);
					//tcs.Cancel ();
					t.Wait ();
				}

				var multi = sw.ElapsedMilliseconds / runs;

				Settings.SingleThreaded = true;
				sw.Restart ();

				for (int i = 0; i < runs; i++) {
					var tcs = new CancellationTokenSource ();
					var t = effect.RenderAsync (src_wrap, dst_wrap, tcs.Token);
					//tcs.Cancel ();
					t.Wait ();
				}

				var single = sw.ElapsedMilliseconds / runs;

				Console.WriteLine (" {2} {0} | {1} | {3}", (single.ToString () + "ms").PadRight (7), (multi.ToString () + "ms").PadRight (7), (effect.GetType ().Name + ":").PadRight (30), true);// (single / multi) >= 2);
			}

			src_wrap.EndUpdate ();
			dst_wrap.EndUpdate ();


			//dst_bitmap.Save (@"C:\Users\Jonathan\Desktop\helo2.png");
			Console.WriteLine ();
			Console.WriteLine ("Finished");
			Console.ReadLine ();
		}

		public static IEnumerable<BaseEffect> GetEffects ()
		{
			yield return new AddNoiseEffect (64, 100, 100);
			yield return new AutoLevelEffect ();
			yield return new BlackAndWhiteEffect ();
			yield return new BrightnessContrastEffect (60, 60);
			yield return new BulgeEffect (45, PointD.Empty);
			yield return new CloudsEffect (5, 5, 5, ColorBgra.Black, ColorBgra.Blue, BlendMode.Normal);
			yield return new EdgeDetectEffect (45);
			yield return new EmbossEffect (45);
			yield return new FragmentEffect (5, 5, 45);
			yield return new FrostedGlassEffect (1);
			yield return new GaussianBlurEffect (2);
			yield return new GlowEffect (6, 10, 10);
			yield return new HueSaturationEffect (50, 50, 50);
			yield return new InkSketchEffect (50, 50);
			yield return new InvertColorsEffect ();
			yield return new JuliaFractalEffect (4, 2, 1, 0);
			yield return new MandelbrotFractalEffect (1, 2, 10, 0, false);
			yield return new MedianEffect (10, 50);
			yield return new MotionBlurEffect (25, 10, true);
			yield return new OilPaintingEffect (3, 50);
			yield return new OutlineEffect (3, 50);
			yield return new PencilSketchEffect (2, 10);
			yield return new PixelateEffect (10);
			yield return new PolarInversionEffect (2, 2, Point.Empty, WarpEdgeBehavior.Wrap, ColorBgra.Black, ColorBgra.Blue);
			yield return new RadialBlurEffect (2, PointD.Empty, 2);
			yield return new RedEyeRemoveEffect (70, 100);
			yield return new ReduceNoiseEffect (6, 0.4);
			yield return new ReliefEffect (45);
			yield return new SepiaEffect ();
			yield return new SharpenEffect (2);
			yield return new SoftenPortraitEffect (5, 5, 10);
			yield return new TileEffect (30, 40, 8);
			yield return new TwistEffect (45, 2);
			yield return new UnfocusEffect (4);
			yield return new ZoomBlurEffect (10, Point.Empty);
		}
	}
}
