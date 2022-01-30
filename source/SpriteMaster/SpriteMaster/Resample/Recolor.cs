/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;

namespace SpriteMaster.Resample;

static class Recolor {
	internal static Span<T> Enhance<T>(Span<T> data, Vector2I size) where T : unmanaged {
		return data;

		/*
		lock (typeof(Upscaler)) {
			using var bitmapStream = data.Stream();
			using (var image = new MagickImage(bitmapStream, new PixelReadSettings(size.Width, size.Height, StorageType.Char, PixelMapping.RGBA))) {
				image.ColorSpace = ImageMagick.ColorSpace.sRGB;
				image.ColorType = ColorType.TrueColorAlpha;

				//image.Depth = 8;
				//image.BitDepth(Channels.Alpha, 8);

				image.HasAlpha = true;

				image.Depth = 8;
				image.BitDepth(Channels.Alpha, 8);

				//image.AutoLevel(Channels.RGB);
				//image.BrightnessContrast(new Percentage(50), new Percentage(50));
				image.Contrast(false);
				//image.Emboss();
				//image.Enhance();
				//image.Equalize();
				//image.GammaCorrect(2.4);
				//image.Normalize();
				//image.AutoGamma();
				//image.RandomThreshold(new Percentage(0), new Percentage(100));
				//image.SelectiveBlur(6.0, 1.0, 1.0);
				image.UnsharpMask(6.0, 1.0);

				//image.Transpose();

				var outputArray = new T[data.Length];
				using (var outputStream = outputArray.Stream()) {
					image.Write(outputStream, MagickFormat.Rgba);
				}
				return outputArray;

			}
		}
		*/
	}
}
