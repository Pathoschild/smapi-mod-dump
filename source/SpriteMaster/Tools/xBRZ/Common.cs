/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System.Drawing.Imaging;

namespace xBRZ;

static class Common {
	internal static readonly SpriteMaster.Colors.ColorSpace ColorSpace = SpriteMaster.Colors.ColorSpace.sRGB_Precise;

	internal static unsafe Span<Color8> ReadFile(Uri path, out Vector2I size) {
		Console.WriteLine($"Reading {path}");

		using var rawImage = Image.FromFile(path.LocalPath);
		using var image = new Bitmap(rawImage.Width, rawImage.Height, PixelFormat.Format32bppArgb);
		using (Graphics g = Graphics.FromImage(image)) {
			g.DrawImage(rawImage, 0, 0, rawImage.Width, rawImage.Height);
		}

		if (image is null) {
			throw new NullReferenceException(nameof(image));
		}

		var imageData = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadOnly, image.PixelFormat);

		var imageSpan = SpanExt.MakeUninitialized<Color8>(image.Width * image.Height);
		var sourceSize = imageData.Height * imageData.Stride;
		var sourceData = new ReadOnlySpan<byte>(imageData.Scan0.ToPointer(), sourceSize).Cast<Color8>();
		int destOffset = 0;
		int sourceOffset = 0;
		for (int y = 0; y < imageData.Height; ++y) {
			sourceData.Slice(sourceOffset, imageData.Width).CopyTo(
				imageSpan.Slice(destOffset, imageData.Width)
			);

			destOffset += imageData.Width;
			sourceOffset += (imageData.Stride / sizeof(Color8));
		}

		image.UnlockBits(imageData);

		size = image.Size;
		return imageSpan;
	}
}
