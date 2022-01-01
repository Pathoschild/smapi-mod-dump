/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

using SpriteMaster.Resample;
using SpriteMaster.Types;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions;

static class Textures {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Area(this Texture2D texture) => texture.Width * texture.Height;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I Extent(this Texture2D texture) => new(texture.Width, texture.Height);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SizeBytes(this SurfaceFormat format, int texels) {
		switch (format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt1a:
			case var _ when format == TextureFormat.DXT1a:
				return texels / 2;
		}

		long elementSize = format switch {
			SurfaceFormat.Color => 4,
			SurfaceFormat.Bgr565 => 2,
			SurfaceFormat.Bgra5551 => 2,
			SurfaceFormat.Bgra4444 => 2,
			SurfaceFormat.Dxt3 => 1,
			SurfaceFormat.Dxt3SRgb => 1,
			SurfaceFormat.Dxt5 => 1,
			SurfaceFormat.Dxt5SRgb => 1,
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool IsBlock(this SurfaceFormat format) {
		switch (format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt3:
			case SurfaceFormat.Dxt3SRgb:
			case SurfaceFormat.Dxt5:
			case SurfaceFormat.Dxt5SRgb:
			case SurfaceFormat.Dxt1a:
				return true;
			default:
				return false;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int BlockEdge(this SurfaceFormat format) {
		switch (format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt3:
			case SurfaceFormat.Dxt3SRgb:
			case SurfaceFormat.Dxt5:
			case SurfaceFormat.Dxt5SRgb:
			case SurfaceFormat.Dxt1a:
				return 4;
			default:
				return 1;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SizeBytes(this Texture2D texture) => texture.Format.SizeBytes(texture.Area());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SizeBytes(this ManagedTexture2D texture) => (long)texture.Area() * 4;

	internal static TeximpNet.Surface Resize(this TeximpNet.Surface source, in Vector2I size, TeximpNet.ImageFilter filter = TeximpNet.ImageFilter.Lanczos3, bool discard = true) {
		if (size == new Vector2I(source)) {
			try {
				return source.Clone();
			}
			finally {
				if (discard) {
					source.Dispose();
				}
			}
		}

		var output = source.Clone();
		try {
			if (!output.Resize(size.Width, size.Height, filter)) {
				throw new Exception("Failed to resize surface");
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Anonymous(this Texture2D texture) => texture.Name.IsBlank();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Anonymous(this ScaledTexture texture) => texture.Name.IsBlank();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string SafeName(this string name) => name.IsBlank() ? "Unknown" : name.Replace("\\", "/").Replace("//", "/");
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string SafeName(this Texture2D texture) => texture.Name.SafeName();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string SafeName(this ScaledTexture texture) => texture.Name.SafeName();

	private const int ImageElementSize = 4;
	internal static TeximpNet.Surface CreateSurface<T>(in FixedSpan<T> source, in Vector2I size, in Bounds region) where T : unmanaged {
		//TeximpNet.Surface.LoadFromStream

		int offset = ((region.Top * size.Width) + region.Left) * ImageElementSize;

		var surface = TeximpNet.Surface.LoadFromRawData(
			source.Pointer + offset,
			width: region.Width,
			height: region.Height,
			rowPitch: size.Width * ImageElementSize,
			isBGRA: false,
			isTopDown: true
		);

		return surface;
	}
	internal static TeximpNet.Surface CreateSurface<T>(T[] source, in Vector2I size, in Bounds region) where T : unmanaged => CreateSurface<T>(source.AsFixedSpan(), size, region);
	internal static TeximpNet.Surface CreateSurface<T>(in FixedSpan<T> source, in Vector2I size) where T : unmanaged => CreateSurface<T>(source, size, size);
	internal static TeximpNet.Surface CreateSurface<T>(T[] source, in Vector2I size) where T : unmanaged => CreateSurface<T>(source.AsFixedSpan(), size, size);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool IsValid(this ScaledTexture texture) {
		return texture != null && texture.Texture != null && !texture.Texture.IsDisposed;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool IsDisposed(this ScaledTexture texture) {
		return texture == null || (texture.Texture != null && texture.Texture.IsDisposed);
	}
}
