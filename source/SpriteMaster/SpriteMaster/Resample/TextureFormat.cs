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
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TeximpNet.Compression;

namespace SpriteMaster.Resample;

[StructLayout(LayoutKind.Sequential)]
readonly struct TextureFormat {

	[MarshalAs(UnmanagedType.I4)]
	private readonly SurfaceFormat SurfaceFormat;
	[MarshalAs(UnmanagedType.I4)]
	private readonly CompressionFormat CompressionFormat;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal TextureFormat(SurfaceFormat surfaceFormat, CompressionFormat compressionFormat) {
		this.SurfaceFormat = surfaceFormat;
		this.CompressionFormat = compressionFormat;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator SurfaceFormat(TextureFormat format) => format.SurfaceFormat;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator CompressionFormat(TextureFormat format) => format.CompressionFormat;

	internal readonly bool IsSupported => Config.Resample.SupportedFormats.Contains(this);

	internal readonly TextureFormat? SupportedOr => IsSupported ? this : null;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly long SizeBytes(int area) => SurfaceFormat.SizeBytes(area);

	internal static readonly TextureFormat None = new((SurfaceFormat)(-1), (CompressionFormat)(-1));

	internal static readonly TextureFormat Color = new(SurfaceFormat.Color, CompressionFormat.BGRA);
	internal static readonly TextureFormat ColorS = new(SurfaceFormat.ColorSRgb, CompressionFormat.BGRA);

	internal static readonly TextureFormat ColorHalf = new(SurfaceFormat.Bgra4444, CompressionFormat.BGRA);
	internal static readonly TextureFormat ColorHalfPunchthroughAlpha = new(SurfaceFormat.Bgra5551, CompressionFormat.BGRA);
	internal static readonly TextureFormat ColorHalfNoAlpha = new(SurfaceFormat.Bgr565, CompressionFormat.BGRA);

	internal static readonly TextureFormat AlphaOnly = new(SurfaceFormat.Alpha8, CompressionFormat.BGRA);

	internal static readonly TextureFormat BC3 = new(SurfaceFormat.Dxt5, CompressionFormat.BC3);
	internal static readonly TextureFormat BC3S = new(SurfaceFormat.Dxt5SRgb, CompressionFormat.BC3);
	internal static readonly TextureFormat BC2 = new(SurfaceFormat.Dxt3, CompressionFormat.BC2);
	internal static readonly TextureFormat BC2S = new(SurfaceFormat.Dxt3SRgb, CompressionFormat.BC2);
	internal static readonly TextureFormat BC1a = new(SurfaceFormat.Dxt1a, CompressionFormat.BC1a);
	internal static readonly TextureFormat BC1 = new(SurfaceFormat.Dxt1, CompressionFormat.BC1);
	internal static readonly TextureFormat BC1S = new(SurfaceFormat.Dxt1SRgb, CompressionFormat.BC1);

	internal static readonly TextureFormat WithAlpha =							BC3.SupportedOr ?? BC2.SupportedOr ?? Color.SupportedOr ?? BC1a.SupportedOr ?? BC1;
	internal static readonly TextureFormat WithHardAlpha =					BC2.SupportedOr ?? WithAlpha;
	internal static readonly TextureFormat WithPunchthroughAlpha =	BC1a.SupportedOr ?? WithHardAlpha;
	internal static readonly TextureFormat WithNoAlpha =						BC1.SupportedOr ?? WithPunchthroughAlpha;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static TextureFormat? Get(CompressionFormat format) {
		var fields = typeof(TextureFormat).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
		foreach (var field in fields) {
			if (field.FieldType != typeof(TextureFormat))
				continue;
			var formatField = (TextureFormat)field.GetValue(null)!;
			if (formatField == format)
				return formatField;
		}
		return null;
	}

	public static bool operator ==(in TextureFormat left, in TextureFormat right) => left.SurfaceFormat == right.SurfaceFormat && left.CompressionFormat == right.CompressionFormat;

	public static bool operator !=(in TextureFormat left, in TextureFormat right) => left.SurfaceFormat != right.SurfaceFormat || left.CompressionFormat != right.CompressionFormat;

	public override bool Equals(object? obj) {
		if (obj is TextureFormat format) {
			return this == format;
		}
		return false;
	}

	public override int GetHashCode() => Hashing.Combine32(SurfaceFormat, CompressionFormat);
}
