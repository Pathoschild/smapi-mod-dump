/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using Pastel;
using SpriteMaster.Resample;
using SpriteMaster.Tasking;
using SpriteMaster.Types;

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class Textures {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Area(this Texture2D texture) => texture.Width * texture.Height;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I Extent(this Texture2D texture) => new(texture.Width, texture.Height);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Bounds Bounds(this Texture2D texture) => new(texture);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SizeBytes(this SurfaceFormat format, int texels) {
		switch (format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt1a:
			case var _ when format == TextureFormat.BC1a:
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
	internal static bool IsCompressed(this SurfaceFormat format) => format switch {
		SurfaceFormat.Dxt1 or
		SurfaceFormat.Dxt1SRgb or
		SurfaceFormat.Dxt3 or
		SurfaceFormat.Dxt3SRgb or
		SurfaceFormat.Dxt5 or
		SurfaceFormat.Dxt5SRgb or
		SurfaceFormat.Dxt1a
		=> true,
		_ => false
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool IsBlock(this SurfaceFormat format) => IsCompressed(format);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int BlockEdge(this SurfaceFormat format) => format switch {
		SurfaceFormat.Dxt1 or
		SurfaceFormat.Dxt1SRgb or
		SurfaceFormat.Dxt3 or
		SurfaceFormat.Dxt3SRgb or
		SurfaceFormat.Dxt5 or
		SurfaceFormat.Dxt5SRgb or
		SurfaceFormat.Dxt1a
		=> 4,
		_ => 1
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SizeBytes(this Texture2D texture) => texture.Format.SizeBytes(texture.Area());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SizeBytes(this ManagedTexture2D texture) => (long)texture.Area() * 4;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Anonymous(this Texture2D texture) => texture.Name.IsWhiteBlank();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Anonymous(this ManagedSpriteInstance texture) => texture.Name.IsWhiteBlank();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static string NormalizeNameInternal(this string? name) {
		/*
		if (name.IsWhiteBlank()) {
			return "[Unknown]";
		}

		name = name.Replace('/', '\\');
		string original;
		do {
			original = name;
			name = original.Replace(@"\\", @"\");
		}
		while (!object.ReferenceEquals(name, original));

		return name;
		*/

		if (name.IsWhiteBlank()) {
			return "[Unknown]";
		}

		return name.Replace('/', '\\').Replace(@"\\", @"\");
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static string? NormalizeNameOrNullInternal(this string? name) {
		/*
		if (name.IsWhiteBlank()) {
			return "[Unknown]";
		}

		name = name.Replace('/', '\\');
		string original;
		do {
			original = name;
			name = original.Replace(@"\\", @"\");
		}
		while (!object.ReferenceEquals(name, original));

		return name;
		*/

		if (name.IsWhiteBlank()) {
			return null;
		}

		return name.Replace('/', '\\').Replace(@"\\", @"\");
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string? NormalizedNameOrNull(this string name) => name.NormalizeNameOrNullInternal();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this string name) => name.NormalizeNameInternal();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this string name, in DrawingColor color) => name.NormalizeNameInternal().Pastel(color);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture texture) => texture.Name.NormalizedName();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture texture, in DrawingColor color) => texture.Name.NormalizedName(in color);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string? NormalizedNameOrNull(this Texture texture) => texture.Name.NormalizedNameOrNull();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture2D texture) => texture.Name.NormalizedName();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture2D texture, in DrawingColor color) => texture.Name.NormalizedName(in color);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string? NormalizedNameOrNull(this Texture2D texture) => texture.Name.NormalizedNameOrNull();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedTexture2D texture) => texture.Name;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedTexture2D texture, in DrawingColor color) => texture.Name.Pastel(color);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string? NormalizedNameOrNull(this ManagedTexture2D texture) => texture.Name.NormalizedNameOrNull();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedSpriteInstance instance) => instance.Name;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedSpriteInstance instance, in DrawingColor color) => instance.Name.Pastel(color);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string? NormalizedNameOrNull(this ManagedSpriteInstance texture) => texture.Name.NormalizedNameOrNull();

	internal static void DumpTexture(
		string path,
		byte[] source,
		in Vector2I sourceSize,
		SurfaceFormat format = SurfaceFormat.Color,
		in double? adjustGamma = null,
		in Bounds? destBounds = null,
		in (int i0, int i1, int i2, int i3)? swap = null
	) {
		DumpTexture(path, source.AsReadOnlySpan(), sourceSize, format, adjustGamma, destBounds, swap);
	}

	internal static void DumpTexture(
		string path,
		ReadOnlySpan<byte> source,
		in Vector2I sourceSize,
		SurfaceFormat format = SurfaceFormat.Color,
		in double? adjustGamma = null,
		in Bounds? destBounds = null,
		in (int i0, int i1, int i2, int i3)? swap = null
	) {
		if (format.IsCompressed()) {
			DumpTextureInternal<byte, byte, byte>(path, source, sourceSize, format, adjustGamma, destBounds, null);
		}
		else {
			DumpTexture(path, source.Cast<Color8>(), sourceSize, format, adjustGamma, destBounds, swap);
		}
	}

	internal static void DumpTexture(
		string path,
		ReadOnlySpan<Color8> source,
		in Vector2I sourceSize,
		SurfaceFormat format = SurfaceFormat.Color,
		in double? adjustGamma = null,
		in Bounds? destBounds = null,
		in (int i0, int i1, int i2, int i3)? swap = null
	) {
		DumpTextureInternal<Color8, uint, byte>(path, source, sourceSize, format, adjustGamma, destBounds, swap);
	}

	internal static void DumpTexture(
		string path,
		ReadOnlySpan<Color16> source,
		in Vector2I sourceSize,
		SurfaceFormat format = SurfaceFormat.Rgba64,
		in double? adjustGamma = null,
		in Bounds? destBounds = null,
		in (int i0, int i1, int i2, int i3)? swap = null
	) {
		DumpTextureInternal<Color16, ulong, ushort>(path, source, sourceSize, format, adjustGamma, destBounds, swap);
	}

	private static void DumpTextureInternal<ColorT, RawT, UnderlyingT>(
		string path,
		ReadOnlySpan<ColorT> source,
		in Vector2I sourceSize,
		SurfaceFormat format,
		in double? adjustGamma = null,
		in Bounds? destBounds = null,
		in (int i0, int i1, int i2, int i3)? swap = null
	)
		where ColorT : unmanaged
		where RawT : unmanaged
		where UnderlyingT : unmanaged {
		if (format.IsBlock()) {
			if (destBounds is not null) {
				throw new ArgumentException($"{nameof(destBounds)} must be null if {nameof(format)} ({format}) is compressed");
			}
			if (adjustGamma is not null) {
				throw new ArgumentException($"{nameof(adjustGamma)} must be null if {nameof(format)} ({format}) is compressed");
			}
			if (swap is not null) {
				throw new ArgumentException($"{nameof(swap)} must be null if {nameof(format)} ({format}) is compressed");
			}
		}

		RawT[] subData;
		Bounds destBound;
		if (destBounds.HasValue) {
			destBound = destBounds.Value;
			subData = GC.AllocateUninitializedArray<RawT>(destBound.Area);
			var destSpan = subData.AsSpan();
			var sourceSpan = source.Cast<ColorT, RawT>();
			int sourceOffset = (sourceSize.Width * destBound.Top) + destBound.Left;
			int destOffset = 0;
			for (int y = 0; y < destBound.Height; ++y) {
				sourceSpan.Slice(sourceOffset, destBound.Width).CopyTo(destSpan.Slice(destOffset, destBound.Width));
				destOffset += destBound.Width;
				sourceOffset += sourceSize.Width;
			}
		}
		else {
			subData = source.Cast<ColorT, RawT>().ToArray();
			destBound = sourceSize;
		}

		if (swap is not null) {
			var swapData = subData.AsSpan<RawT, UnderlyingT>();
			for (int i = 0; i < swapData.Length; i += 4) {
				var i0 = swapData[i + swap.Value.i0];
				var i1 = swapData[i + swap.Value.i1];
				var i2 = swapData[i + swap.Value.i2];
				var i3 = swapData[i + swap.Value.i3];
				swapData[0] = i0;
				swapData[1] = i1;
				swapData[2] = i2;
				swapData[3] = i3;
			}
		}

		SynchronizedTaskScheduler.Instance.QueueImmediate(() => {
			try {
				using var dumpTexture = new DumpTexture2D(
					StardewValley.Game1.graphics.GraphicsDevice,
					destBound.Width,
					destBound.Height,
					mipmap: false,
					format: format
				) {
					Name = "Dump Texture"
				};

				dumpTexture.SetData(subData);
				if (Path.GetDirectoryName(path) is string directory) {
					Directory.CreateDirectory(directory);
				}
				using var dumpFile = File.Create(path);
				dumpTexture.SaveAsPng(dumpFile, destBound.Width, destBound.Height);
			}
			catch (Exception ex) {
				Debug.Warning(ex);
			}
		});
	}
}
