using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TeximpNet.Compression;
using TeximpNet.DDS;

namespace SpriteMaster.Resample {
	internal static class BlockCompress {
		// We set this to false if block compression fails, as we assume that for whatever reason nvtt does not work on that system.
		private static bool BlockCompressionFunctional = true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void FlipColorBytes (byte[] p) {
			var span = new Span<byte>(p).As<uint>();
			foreach (int i in 0..span.Length) {
				var color = span[i];
				color =
					(color & 0xFF000000U) |
					(color & 0x0000FF00U) |
					((color & 0x00FF0000U) >> 16) |
					((color & 0x000000FFU) << 16);
				span[i] = color;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe byte[] Compress (byte[] data, ref TextureFormat format, Vector2I dimensions, bool HasAlpha, bool IsPunchThroughAlpha, bool IsMasky, bool HasR, bool HasG, bool HasB) {
			if (!BlockCompressionFunctional) {
				return null;
			}

			var oldSpriteFormat = format;

			FlipColorBytes(data);

			try {
				var bitmapData = data;

				using var compressor = new Compressor();
				compressor.Input.AlphaMode = (HasAlpha) ? AlphaMode.Premultiplied : AlphaMode.None;
				compressor.Input.GenerateMipmaps = false;
				var textureFormat =
					(!HasAlpha) ?
						TextureFormat.NoAlpha :
						((false && IsPunchThroughAlpha && Config.Resample.BlockCompression.Quality != CompressionQuality.Fastest) ?
							TextureFormat.WithPunchthroughAlpha :
							(IsMasky ?
								TextureFormat.WithHardAlpha :
								TextureFormat.WithAlpha));
				compressor.Compression.Format = textureFormat;
				compressor.Compression.Quality = Config.Resample.BlockCompression.Quality;
				compressor.Compression.SetQuantization(true, true, IsPunchThroughAlpha);

				{
					compressor.Compression.GetColorWeights(out var r, out var g, out var b, out var a);
					a = HasAlpha ? (a * 20.0f) : 0.0f;
					// Relative luminance of the various channels.
					r = HasR ? (r * 0.2126f) : 0.0f;
					g = HasG ? (g * 0.7152f) : 0.0f;
					b = HasB ? (b * 0.0722f) : 0.0f;

					compressor.Compression.SetColorWeights(r, g, b, a);
				}

				compressor.Output.IsSRGBColorSpace = true;
				compressor.Output.OutputHeader = false;

				//public MipData (int width, int height, int rowPitch, IntPtr data, bool ownData = true)

				fixed (byte* p = bitmapData) {
					using var mipData = new MipData(dimensions.Width, dimensions.Height, dimensions.Width * sizeof(int), (IntPtr)p, false);
					compressor.Input.SetData(mipData, true);
					var memoryBuffer = new byte[((SurfaceFormat)textureFormat).SizeBytes(dimensions.Area)];
					using var stream = memoryBuffer.Stream();
					if (compressor.Process(stream)) {
						format = textureFormat;
						return memoryBuffer;
					}
					else {
						Debug.WarningLn($"Failed to use {(CompressionFormat)textureFormat} compression: " + compressor.LastErrorString);
					}
				}
			}
			catch (Exception ex) {
				ex.PrintWarning();
				BlockCompressionFunctional = false;
			}
			format = oldSpriteFormat;
			FlipColorBytes(data);
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe bool Compress (ref byte[] data, ref TextureFormat format, Vector2I dimensions, bool HasAlpha, bool IsPunchThroughAlpha, bool IsMasky, bool HasR, bool HasG, bool HasB) {
			var oldFormat = format;

			try {
				// We do this ourselves because TexImpNet's allocator has an overflow bug which causes the conversion to fail if it converts it itself.
				var byteData = Compress(data, ref format, dimensions, HasAlpha, IsPunchThroughAlpha, IsMasky, HasR, HasG, HasB);
				if (byteData == null) {
					return false;
				}
				data = byteData;
				return true;
			}
			catch {
				format = oldFormat;
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsBlockMultiple(int value) {
			return (value % 4) == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsBlockMultiple (uint value) {
			return (value % 4) == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsBlockMultiple (Vector2I value) {
			return IsBlockMultiple(value.X) && IsBlockMultiple(value.Y);
		}

		// https://www.khronos.org/opengl/wiki/S3_Texture_Compression
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
		private unsafe struct ColorBlock {
			/* The endianness of the data in the documentation is a bit confusing to me. */

			// Color should appear as BGR, 565.
			[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2)]
			private unsafe struct Color565 {
				[FieldOffset(0)]
				internal ushort Packed;

				internal readonly uint PackedInt => Packed;

				internal Color565 (ushort packed) {
					Packed = packed;
				}

				private enum Mask : uint {
					M5 = unchecked((1U << 5) - 1U),
					M6 = unchecked((1U << 6) - 1U),

					B = (BitSize.B == 5) ? M5 : M6,
					G = (BitSize.G == 5) ? M5 : M6,
					R = (BitSize.R == 5) ? M5 : M6,
				}

				private enum Multiplier : uint {
					M5 = 255U / Mask.M5,
					M6 = 255U / Mask.M6,

					B = (BitSize.B == 5) ? M5 : M6,
					G = (BitSize.G == 5) ? M5 : M6,
					R = (BitSize.R == 5) ? M5 : M6,
				}

				private enum BitSize : uint {
					B = 5,
					G = 6,
					R = 5
				}

				private enum Offset : int {
					B = unchecked(0),
					G = unchecked(B + (int)BitSize.B),
					R = unchecked(G + (int)BitSize.G)
				}

				private readonly uint PackedB => (PackedInt >> (int)Offset.B) & (uint)Mask.B;
				private readonly uint PackedG => (PackedInt >> (int)Offset.G) & (uint)Mask.G;
				private readonly uint PackedR => (PackedInt >> (int)Offset.R) & (uint)Mask.R;


				internal readonly byte B => (byte)(((uint)Multiplier.B * PackedB) & 0xFF);
				internal readonly byte G => (byte)(((uint)Multiplier.G * PackedG) & 0xFF);
				internal readonly byte R => (byte)(((uint)Multiplier.R * PackedR) & 0xFF);

				// https://stackoverflow.com/a/2442609
				internal uint AsPacked {
					get {
						unchecked {
							return
								(uint)B << 16 |
								(uint)G << 8 |
								(uint)R;
						}
					}
				}
			}

			[FieldOffset(0)]
			private Color565 color0;
			[FieldOffset(2)]
			private Color565 color1;
			[FieldOffset(4)]
			private fixed byte codes[4];

			private readonly byte GetCode(uint x, uint y) {
				var code = codes[y];
				return (byte)((byte)(code >> (int)(x * 2)) & 0b11);
			}

			private static uint Pack(uint b, uint g, uint r) {
				unchecked {
					return
						(uint)b << 16 |
						(uint)g << 8 |
						(uint)r;
				}
			}

			// Returns the color, hopefully, as ABGR
			internal readonly uint GetColor(uint x, uint y) {
				if (color0.Packed > color1.Packed) {
					var code = GetCode(x, y);
					switch (code) {
						case 0:
							return color0.AsPacked;
						case 1:
							return color1.AsPacked;
						case 2: {
							var b = (uint)(2 * color0.B + color1.B) / 3;
							var g = (uint)(2 * color0.G + color1.G) / 3;
							var r = (uint)(2 * color0.R + color1.R) / 3;
							return Pack(b, g, r);
						}
						default:
						case 3: {
							var b = (uint)(color0.B + 2 * color1.B) / 3;
							var g = (uint)(color0.G + 2 * color1.G) / 3;
							var r = (uint)(color0.R + 2 * color1.R) / 3;
							return Pack(b, g, r);
						}
					}
				}
				else {
					return GetColorDXT3(x, y);
				}
			}

			internal readonly uint GetColorDXT3(uint x, uint y) {
				var code = GetCode(x, y);
				switch (code) {
					case 0:
						return color0.AsPacked;
					case 1:
						return color1.AsPacked;
					case 2: {
						var b = (uint)(color0.B + color1.B) / 2;
						var g = (uint)(color0.G + color1.G) / 2;
						var r = (uint)(color0.R + color1.R) / 2;
						return Pack(b, g, r);
					}
					default:
					case 3: {
						return 0U;
					}
				}
			}
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
		private unsafe struct ColorBlockDxt3 {
			[FieldOffset(0)]
			private fixed ushort AlphaBlock[4];
			[FieldOffset(8)]
			private ColorBlock Color;

			internal readonly uint GetColor(uint x, uint y) {
				var alphaValue = (byte)((AlphaBlock[y] >> (int)(x * 4)) & 0b1111);
				uint alpha = ((uint)alphaValue * 0b10001u) << 24;
				return Color.GetColorDXT3(x, y) | alpha;
			}
		}

			internal static byte[] Decompress (byte[] data, SpriteInfo info) {
			return Decompress(data, (uint)info.Reference.Width, (uint)info.Reference.Height, info.Reference.Format);
		}

		internal static byte[] Decompress(byte[] data, uint width, uint height, SurfaceFormat format) {
			if (!IsBlockMultiple(width) || !IsBlockMultiple(height)) {
				throw new ArgumentException(nameof(width));
			}

			switch (format) {
				case SurfaceFormat.Dxt1: {
					var blocks = data.AsSpan().As<ColorBlock>();
					var outData = new byte[width * height * sizeof(uint)];
					var outDataPacked = outData.AsSpan().As<uint>();
					var widthBlocks = width / 4;

					uint blockIndex = 0;
					foreach (var block in blocks) {
						var index = blockIndex++;
						var xOffset = (uint)(index % widthBlocks) * 4;
						var yOffset = (uint)(index / widthBlocks) * 4;

						foreach (uint y in 0..4) {
							var yOffsetInternal = yOffset + y;
							foreach (uint x in 0..4) {
								var xOffsetInternal = xOffset + x;
								var offset = (yOffsetInternal * (uint)width) + xOffsetInternal;
								outDataPacked[(int)offset] = block.GetColor(x, y) | 0xFF000000U;
							}
						}
					}

					return outData;
				} break;
				case SurfaceFormat.Dxt3: {
					var blocks = data.AsSpan().As<ColorBlockDxt3>();
					var outData = new byte[width * height * sizeof(uint)];
					var outDataPacked = outData.AsSpan().As<uint>();
					var widthBlocks = width / 4;

					uint blockIndex = 0;
					foreach (var block in blocks) {
						var index = blockIndex++;
						var xOffset = (uint)(index % widthBlocks) * 4;
						var yOffset = (uint)(index / widthBlocks) * 4;

						foreach (uint y in 0..4) {
							var yOffsetInternal = yOffset + y;
							foreach (uint x in 0..4) {
								var xOffsetInternal = xOffset + x;
								var offset = (yOffsetInternal * (uint)width) + xOffsetInternal;
								outDataPacked[(int)offset] = block.GetColor(x, y);
							}
						}
					}

					return outData;
				}
				break;
				default:
					throw new ArgumentOutOfRangeException(nameof(format));
			}
		}
	}
}
