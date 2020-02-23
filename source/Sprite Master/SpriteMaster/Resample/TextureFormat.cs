using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System.Reflection;
using System.Runtime.CompilerServices;
using TeximpNet.Compression;

namespace SpriteMaster.Resample {
	internal struct TextureFormat {

		private readonly SurfaceFormat surfaceFormat;
		private readonly CompressionFormat compressionFormat;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal TextureFormat (SurfaceFormat surfaceFormat, CompressionFormat compressionFormat) {
			this.surfaceFormat = surfaceFormat;
			this.compressionFormat = compressionFormat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SurfaceFormat (TextureFormat format) {
			return format.surfaceFormat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator CompressionFormat (TextureFormat format) {
			return format.compressionFormat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal readonly long SizeBytes(int area) {
			return surfaceFormat.SizeBytes(area);
		}

		internal static readonly SurfaceFormat SF_DXT1a = SurfaceFormatExt.GetSurfaceFormat("Dxt1a");

		internal static readonly TextureFormat Color = new TextureFormat(SurfaceFormat.Color, CompressionFormat.BGRA);
		internal static readonly TextureFormat DXT5 = new TextureFormat(SurfaceFormat.Dxt5, CompressionFormat.DXT5);
		internal static readonly TextureFormat DXT3 = new TextureFormat(SurfaceFormat.Dxt3, CompressionFormat.DXT3);
		// https://github.com/labnation/MonoGame/blob/master/MonoGame.Framework/Graphics/SurfaceFormat.cs#L126
		internal static readonly TextureFormat DXT1a = new TextureFormat(SF_DXT1a, CompressionFormat.DXT1a);
		internal static readonly TextureFormat DXT1 = new TextureFormat(SurfaceFormat.Dxt1, CompressionFormat.DXT1);

		private static TextureFormat SupportedOr(TextureFormat format, TextureFormat other) {
			return Config.Resample.SupportedFormats.Contains(format) ? format : other;
		}

		internal static readonly TextureFormat WithAlpha = SupportedOr(DXT5, Color);
		internal static readonly TextureFormat WithHardAlpha = SupportedOr(DXT3, Color);
		internal static readonly TextureFormat WithPunchthroughAlpha = SupportedOr(DXT1a, WithHardAlpha);
		internal static readonly TextureFormat NoAlpha = SupportedOr(DXT1, WithPunchthroughAlpha);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static TextureFormat? Get (CompressionFormat format) {
			var fields = typeof(TextureFormat).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
			foreach (var field in fields) {
				if (field.FieldType != typeof(TextureFormat))
					continue;
				var formatField = (TextureFormat)field.GetValue(null);
				if (formatField == format)
					return formatField;
			}
			return null;
		}
	}
}
