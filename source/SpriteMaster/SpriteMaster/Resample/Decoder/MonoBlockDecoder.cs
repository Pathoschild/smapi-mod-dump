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
using SpriteMaster.Types;
using System;
using System.IO;
using System.Reflection;

namespace SpriteMaster.Resample.Decoder;

static class MonoBlockDecoder {
	private static readonly Type? DxtUtil = typeof(Microsoft.Xna.Framework.Graphics.Texture2D).Assembly.GetType("Microsoft.Xna.Framework.Graphics.DxtUtil");

	internal delegate byte[] DecompressDelegateArray(byte[] data, int width, int height);
	internal delegate byte[] DecompressDelegateStream(Stream data, int width, int height);

	internal static readonly DecompressDelegateArray? DecompressDXT1Array = GetDelegate<DecompressDelegateArray>("DecompressDxt1");
	internal static readonly DecompressDelegateStream? DecompressDXT1Stream = GetDelegate<DecompressDelegateStream>("DecompressDxt1");

	internal static readonly DecompressDelegateArray? DecompressDXT3Array = GetDelegate<DecompressDelegateArray>("DecompressDxt3");
	internal static readonly DecompressDelegateStream? DecompressDXT3Stream = GetDelegate<DecompressDelegateStream>("DecompressDxt3");

	internal static readonly DecompressDelegateArray? DecompressDXT5Array = GetDelegate<DecompressDelegateArray>("DecompressDxt5");
	internal static readonly DecompressDelegateStream? DecompressDXT5Stream = GetDelegate<DecompressDelegateStream>("DecompressDxt5");

	private static T? GetDelegate<T>(string name) where T : Delegate => DxtUtil?.GetMethod(
		name,
		BindingFlags.NonPublic | BindingFlags.Static,
		null,
		new Type[] { typeof(T) == typeof(DecompressDelegateArray) ? typeof(byte[]) : typeof(Stream), typeof(int), typeof(int) },
		null
	)?.CreateDelegate<T>();

	internal static Span<byte> Decode(ReadOnlySpan<byte> data, Vector2I size, SurfaceFormat format) {
		switch (format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt1a:
				if (DecompressDXT1Array is null) {
					return null;
				}
				return DecompressDXT1Array(data.ToArray(), size.Width, size.Height);
			case SurfaceFormat.Dxt3:
			case SurfaceFormat.Dxt3SRgb:
				if (DecompressDXT3Array is null) {
					return null;
				}
				return DecompressDXT3Array(data.ToArray(), size.Width, size.Height);
			case SurfaceFormat.Dxt5:
			case SurfaceFormat.Dxt5SRgb:
				if (DecompressDXT5Array is null) {
					return null;
				}
				return DecompressDXT5Array(data.ToArray(), size.Width, size.Height);
			default:
				return default;
		}
	}
}
