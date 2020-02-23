using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions {
	internal static class Textures {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Area (this Texture2D texture) {
			return texture.Width * texture.Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2I Extent (this Texture2D texture) {
			return new Vector2I(texture.Width, texture.Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static long SizeBytes (this SurfaceFormat format, int texels) {
			switch (format) {
				case SurfaceFormat.Dxt1:
				case var _ when format == TextureFormat.DXT1a:
					return texels / 2;
			}

			long elementSize = format switch {
				SurfaceFormat.Color => 4,
				SurfaceFormat.Bgr565 => 2,
				SurfaceFormat.Bgra5551 => 2,
				SurfaceFormat.Bgra4444 => 2,
				SurfaceFormat.Dxt3 => 1,
				SurfaceFormat.Dxt5 => 1,
				SurfaceFormat.NormalizedByte2 => 2,
				SurfaceFormat.NormalizedByte4 => 4,
				SurfaceFormat.Rgba1010102 => 4,
				SurfaceFormat.Rg32 => 4,
				SurfaceFormat.Rgba64 => 8,
				SurfaceFormat.Alpha8 => 1,
				SurfaceFormat.Single => 4,
				SurfaceFormat.Vector2 => 8,
				SurfaceFormat.Vector4 => 16,
				SurfaceFormat.HalfSingle => 2,
				SurfaceFormat.HalfVector2 => 4,
				SurfaceFormat.HalfVector4 => 8,
				_ => throw new ArgumentException(nameof(format))
			};

			return texels * elementSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static long SizeBytes (this Texture2D texture) {
			return texture.Format.SizeBytes(texture.Area());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static long SizeBytes (this ManagedTexture2D texture) {
			return (long)texture.Area() * 4;
		}

		internal static Bitmap Resize (this Bitmap source, in Vector2I size, InterpolationMode filter = InterpolationMode.HighQualityBicubic, bool discard = true) {
			if (size == new Vector2I(source)) {
				try {
					return new Bitmap(source);
				}
				finally {
					if (discard) {
						source.Dispose();
					}
				}
			}
			var output = new Bitmap(size.Width, size.Height);
			try {
				using (var g = Graphics.FromImage(output)) {
					g.InterpolationMode = filter;
					g.DrawImage(source, 0, 0, output.Width, output.Height);
				}
				if (discard) {
					source.Dispose();
				}
				return output;
			}
			catch {
				output.Dispose();
				throw;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool Anonymous (this Texture2D texture) {
			return texture.Name.IsBlank();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool Anonymous (this ScaledTexture texture) {
			return texture.Name.IsBlank();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string SafeName (this string name) {
			return name.IsBlank() ? "Unknown" : name.Replace("\\", "/").Replace("//", "/");
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string SafeName (this Texture2D texture) {
			return texture.Name.SafeName();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string SafeName (this ScaledTexture texture) {
			return texture.Name.SafeName();
		}

		internal static Bitmap CreateBitmap (byte[] source, in Vector2I size, PixelFormat format = PixelFormat.Format32bppArgb) {
			var newImage = new Bitmap(size.Width, size.Height, format);
			var rectangle = new Bounds(newImage);
			var newBitmapData = newImage.LockBits(rectangle, ImageLockMode.WriteOnly, format);
			// Get the address of the first line.
			var newBitmapPointer = newBitmapData.Scan0;
			//http://stackoverflow.com/a/1917036/294804
			// Copy the RGB values back to the bitmap

			bool hasPadding = newBitmapData.Stride != (newBitmapData.Width * sizeof(int));

			// Handle stride correctly as input data does not have any stride?
			const bool CopyWithPadding = true;
			if (CopyWithPadding && hasPadding) {
				var rowElements = newImage.Width;
				var rowSize = newBitmapData.Stride;

				int sourceOffset = 0;
				foreach (int row in 0..newImage.Height) {
					Marshal.Copy(source, sourceOffset, newBitmapPointer, rowElements * sizeof(int));
					sourceOffset += rowElements;
					newBitmapPointer += rowSize;
				}
			}
			else {
				var intCount = newBitmapData.Stride * newImage.Height / sizeof(int);
				Marshal.Copy(source, 0, newBitmapPointer, intCount);
			}
			// Unlock the bits.
			newImage.UnlockBits(newBitmapData);

			return newImage;
		}
	}
}
