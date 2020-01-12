using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using System.Reflection;
using TeximpNet.Compression;

namespace SpriteMaster.Resample {
	internal struct TextureFormat {

		private readonly SurfaceFormat surfaceFormat;
		private readonly CompressionFormat compressionFormat;

		internal TextureFormat (SurfaceFormat surfaceFormat, CompressionFormat compressionFormat) {
			this.surfaceFormat = surfaceFormat;
			this.compressionFormat = compressionFormat;
		}

		public static implicit operator SurfaceFormat (TextureFormat format) {
			return format.surfaceFormat;
		}

		public static implicit operator CompressionFormat (TextureFormat format) {
			return format.compressionFormat;
		}

		internal readonly long SizeBytes(int area) {
			return surfaceFormat.SizeBytes(area);
		}

		internal static readonly TextureFormat Color = new TextureFormat(SurfaceFormat.Color, CompressionFormat.BGRA);
		internal static readonly TextureFormat WithAlpha = new TextureFormat(SurfaceFormat.Dxt5, CompressionFormat.DXT5);
		internal static readonly TextureFormat WithHardAlpha = new TextureFormat(SurfaceFormat.Dxt3, CompressionFormat.DXT3);
		internal static readonly TextureFormat WithPunchthroughAlpha = new TextureFormat(SurfaceFormat.Dxt1, CompressionFormat.DXT1a);
		internal static readonly TextureFormat NoAlpha = new TextureFormat(SurfaceFormat.Dxt1, CompressionFormat.DXT1);

		internal static TextureFormat? Get (CompressionFormat format) {
			var fields = typeof(TextureFormat).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
			foreach (var field in fields) {
				if (field.FieldType != typeof(TextureFormat))
					continue;
				var formatField = (TextureFormat)field.GetValue(null);
				if ((CompressionFormat)formatField == format)
					return formatField;
			}
			return null;
		}
	}
}
