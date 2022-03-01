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
	internal static string NormalizedName(this string name) => name.NormalizeNameInternal();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this string name, in DrawingColor color) => name.NormalizeNameInternal().Pastel(color);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture texture) => texture.Name.NormalizedName();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture texture, in DrawingColor color) => texture.Name.NormalizedName(in color);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture2D texture) => texture.Name.NormalizedName();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this Texture2D texture, in DrawingColor color) => texture.Name.NormalizedName(in color);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedTexture2D texture) => texture.Name;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedTexture2D texture, in DrawingColor color) => texture.Name.Pastel(color);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedSpriteInstance instance) => instance.Name;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string NormalizedName(this ManagedSpriteInstance instance, in DrawingColor color) => instance.Name.Pastel(color);

	internal static void DumpTexture(string path, byte[] source, in Vector2I sourceSize, in double? adjustGamma = null, in Bounds? destBounds = null, in (int i0, int i1, int i2, int i3)? swap = null) {
		DumpTexture(path, source.AsSpan().Cast<Color8>(), sourceSize, adjustGamma, destBounds, swap);
	}

	internal static void DumpTexture(string path, ReadOnlySpan<Color8> source, in Vector2I sourceSize, in double? adjustGamma = null, in Bounds? destBounds = null, in (int i0, int i1, int i2, int i3)? swap = null) {
		DumpTexture8(path, source, sourceSize, adjustGamma, destBounds, swap);
	}

	internal static void DumpTexture(string path, ReadOnlySpan<Color16> source, in Vector2I sourceSize, in double? adjustGamma = null, in Bounds? destBounds = null, in (int i0, int i1, int i2, int i3)? swap = null) {
		DumpTexture16(path, source, sourceSize, adjustGamma, destBounds, swap);
	}

	private static void DumpTexture8(string path, ReadOnlySpan<Color8> source, in Vector2I sourceSize, in double? adjustGamma = null, in Bounds? destBounds = null, in (int i0, int i1, int i2, int i3)? swap = null) {
		byte[] subData;
		Bounds destBound;
		if (destBounds.HasValue) {
			destBound = destBounds.Value;
			subData = GC.AllocateUninitializedArray<byte>(destBound.Area * 4);
			var destSpan = subData.AsSpan<uint>();
			var sourceSpan = source.Cast<Color8, uint>();
			int sourceOffset = (sourceSize.Width * destBound.Top) + destBound.Left;
			int destOffset = 0;
			for (int y = 0; y < destBound.Height; ++y) {
				sourceSpan.Slice(sourceOffset, destBound.Width).CopyTo(destSpan.Slice(destOffset, destBound.Width));
				destOffset += destBound.Width;
				sourceOffset += sourceSize.Width;
			}
		}
		else {
			subData = source.Cast<Color8, byte>().ToArray();
			destBound = sourceSize;
		}

		SynchronizedTaskScheduler.Instance.QueueImmediate(() => {
			try {
				using var dumpTexture = new DumpTexture2D(
					StardewValley.Game1.graphics.GraphicsDevice,
					destBound.Width,
					destBound.Height,
					mipmap: false,
					format: SurfaceFormat.Color
				) {
					Name = "Dump Texture"
				};

				// PlatformSetData(0, data, 0, data.Length);
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

	private static void DumpTexture16(string path, ReadOnlySpan<Color16> source, in Vector2I sourceSize, in double? adjustGamma = null, in Bounds? destBounds = null, in (int i0, int i1, int i2, int i3)? swap = null) {
		ulong[] subData;
		Bounds destBound;
		if (destBounds.HasValue) {
			destBound = destBounds.Value;
			subData = GC.AllocateUninitializedArray<ulong>(destBound.Area);
			var destSpan = subData.AsSpan();
			var sourceSpan = source.Cast<Color16, ulong>();
			int sourceOffset = (sourceSize.Width * destBound.Top) + destBound.Left;
			int destOffset = 0;
			for (int y = 0; y < destBound.Height; ++y) {
				sourceSpan.Slice(sourceOffset, destBound.Width).CopyTo(destSpan.Slice(destOffset, destBound.Width));
				destOffset += destBound.Width;
				sourceOffset += sourceSize.Width;
			}
		}
		else {
			subData = source.Cast<Color16, ulong>().ToArray();
			destBound = sourceSize;
		}

		SynchronizedTaskScheduler.Instance.QueueImmediate(() => {
			try {
				using var dumpTexture = new DumpTexture2D(
					StardewValley.Game1.graphics.GraphicsDevice,
					destBound.Width,
					destBound.Height,
					mipmap: false,
					format: SurfaceFormat.Rgba64
				) {
					Name = "Dump Texture"
				};

				// PlatformSetData(0, data, 0, data.Length);
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
