Pinta.ImageManipulation
=======================

The same great effects that power Pinta are now available in a purely managed, graphics toolkit agnostic library you can easily use in your .NET app.  All effects are implemented as multi-threaded Tasks that support cancelation.  Originally from the excellent [Paint.NET](http://www.getpaint.net/), with a few lines of code you can use filters like:

- Gaussian Blur
- Brightness/Contrast
- AutoLevel
- Sepia
- Add Noise
- and 30+ others

##License

Pinta.ImageManipulation is licensed under the MIT license.
See `license-mit.txt` for the MIT License.

Code from Paint.Net 3.36 is used under the MIT License and retains the
original headers on source files.

See `license-pdn.txt` for Paint.Net's original license.

##Using the Pinta.ImageManipulation Library

Pinta.ImageManipulation.dll is graphics toolkit agnostic and works purely on BGRA byte arrays.  Some wrappers for common graphics toolkits are available.

For example, if you are using System.Drawing, add Pinta.ImageManipulation.dll and Pinta.ImageManipulation.System.Drawing.dll to your project.  Then to apply a Gaussian blur to an image, use:

```csharp
var src_bitmap = new System.Drawing.Bitmap (@"C:\pic.png");

var src_wrap = new BitmapWrapper (src_bitmap);

var blur = new GaussianBlurEffect ();
await blur.RenderAsync (src_wrap);

src_bitmap.Save (@"C:\pic2.png");
```