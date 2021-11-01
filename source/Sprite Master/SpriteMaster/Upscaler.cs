/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster {
	internal sealed class Upscaler {
		internal enum Scaler : int {
			xBRZ = 0,
			Bilinear,
			Bicubic,
			ImageMagick
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void PurgeHash (Texture2D reference) {
			reference.Meta().CachedData = null;
		}

		// https://stackoverflow.com/a/12996028
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private static ulong HashULong (ulong x) {
			if (x == 0) {
				x = ulong.MaxValue;
			}
			unchecked {
				x = (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9ul;
				x = (x ^ (x >> 27)) * 0x94d049bb133111ebul;
				x ^= x >> 31;
			}
			return x;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static ulong GetHash (SpriteInfo input, TextureType textureType) {
			// Need to make Hashing.CombineHash work better.
			ulong hash = Hash.Combine(input.Reference.SafeName()?.GetHashCode(), input.Reference.Meta().GetHash(input));

			if (Config.Resample.EnableDynamicScale) {
				unchecked {
					hash = Hash.Combine(hash, HashULong((ulong)input.ExpectedScale));
				}
			}

			if (textureType == TextureType.Sprite) {
				hash = Hash.Combine(hash, input.Size.Hash());
			}
			return hash;
		}

		private static readonly WeakSet<Texture2D> GarbageMarkSet = Config.Garbage.CollectAccountUnownedTextures ? new() : null;

		// This basically just changes it from AXYZ to AZYX, which is what's expected in output.
		private static Bitmap GetDumpBitmap (Bitmap source) {
			var dump = (Bitmap)source.Clone();
			foreach (int y in 0.RangeTo(dump.Height)) {
				foreach (int x in 0.RangeTo(dump.Width)) {
					unchecked {
						var pixel = dump.GetPixel(x, y);
						var ipixel = (uint)pixel.ToArgb();

						ipixel =
							(ipixel & 0xFF00FF00) |
							(ipixel & 0x00FF0000) >> 16 |
							(ipixel & 0x000000FF) << 16;

						dump.SetPixel(
							x, y,
							System.Drawing.Color.FromArgb((int)ipixel)
						);
					}
				}
			}

			return dump;
		}

		private sealed class Tracer : IDisposable {
#if REALLY_TRACE
			private readonly string Name;
			private static int Depth = 0;
#endif

#if REALLY_TRACE
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			[Conditional("REALLY_TRACE")]
			private static void Trace (string msg) {
				Debug.TraceLn($"[CreateNewTexture] {new string(' ', Depth)}{msg}");
			}
#endif

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			internal Tracer (string name) {
#if REALLY_TRACE
				Name = name;

				Trace(Name);
				++Depth;
#endif
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {
#if REALLY_TRACE
				--Depth;
				Trace("~" + Name);
#endif
			}
		}

		private const uint WaterBlock = 4;
		private const uint FontBlock = 1;

		private static bool IsFont (SpriteInfo input) {
			switch (input.Reference.Format) {
				case SurfaceFormat.Dxt1:
				case SurfaceFormat.Dxt3:
				case SurfaceFormat.Dxt5:
					return Math.Min(input.Size.Extent.MinOf, input.ReferenceSize.MinOf) >= FontBlock;
				default:
					return false;
			}
		}

		private static bool IsWater(SpriteInfo input) {
			var bounds = input.Size;
			var texture = input.Reference;
			if (bounds.Right <= 640 && bounds.Top >= 2000 && bounds.Extent.MinOf >= WaterBlock && texture.SafeName() == "LooseSprites/Cursors") {
				return true;
			}
			return false;
		}

		private static FixedSpan<int> DownSample (byte[] data, in Bounds bounds, uint referenceWidth, uint block, bool blend = false) {
			uint blockSize = block * block;
			uint halfBlock = blend ? 0 : (block >> 1);
			var blockOffset = bounds.Offset * (int)block;

			// Rescale the data down, doing an effective point sample from 4x4 blocks to 1 texel.
			var veryRawData = new FixedSpan<byte>(data).As<uint>();
			var rawData = new FixedSpan<int>(new int[bounds.Area]);
			foreach (uint y in 0.RangeTo(bounds.Extent.Height)) {
				var ySourceOffset = (((y * block) + (uint)blockOffset.Y) + halfBlock) * referenceWidth;
				var yDestinationOffset = y * (uint)bounds.Extent.X;
				foreach (uint x in 0.RangeTo(bounds.Extent.Width)) {
					if (blend) {
						uint max_a = 0;
						uint a = 0;
						uint b = 0;
						uint g = 0;
						uint r = 0;
						var ySourceOffsetAdjusted = ySourceOffset;
						foreach (uint innerY in 0U.RangeTo(block)) {
							foreach (uint innerX in 0U.RangeTo(block)) {
								var sample = veryRawData[ySourceOffsetAdjusted + ((x * block) + (uint)blockOffset.X + innerX)];
								var aa = (sample >> 24) & 0xFFU;
								max_a = Math.Max(aa, max_a);
								a += aa;
								b += (sample >> 16) & 0xFFU;
								g += (sample >> 8) & 0xFFU;
								r += sample & 0xFFU;
							}
							ySourceOffsetAdjusted += referenceWidth;
						}

						a /= (uint)blockSize;
						b /= (uint)blockSize;
						g /= (uint)blockSize;
						r /= (uint)blockSize;

						a = (a * 3u + max_a) >> 2;

						rawData[yDestinationOffset + x] = (int)(
							(a & 0xFFU) << 24 |
							(b & 0xFFU) << 16 |
							(g & 0xFFU) << 8 |
							(r & 0xFFU)
						);
					}
					else {
						rawData[yDestinationOffset + x] = unchecked((int)veryRawData[ySourceOffset + ((x * block) + (uint)blockOffset.X + halfBlock)]);
					}
				}
			}

			return rawData;
		}

		// TODO : use MemoryFailPoint class. Extensively.

		private static byte[] ExtractBitmap(Bitmap image) {
			var resizedData = image.LockBits(new Bounds(image), ImageLockMode.ReadOnly, image.PixelFormat);

			// TODO : what if PixelFormat doesn't match sizeof(int)?
			byte[] data = null;

			try {
				data = new byte[image.Width * image.Height * sizeof(int)];
				var dataSize = resizedData.Stride * resizedData.Height;
				var dataPtr = resizedData.Scan0;
				var widthSize = resizedData.Width;

				var dataBytes = new byte[dataSize];
				int offsetSource = 0;
				int offsetDest = 0;
				foreach (int y in 0.RangeTo(resizedData.Height)) {
					Marshal.Copy(dataPtr + offsetSource, data, offsetDest, widthSize);
					offsetSource += resizedData.Stride;
					offsetDest += widthSize;
				}
			}
			finally {
				image.UnlockBits(resizedData);
			}

			return data;
		}

		private static unsafe void CreateNewTexture (
			ScaledTexture texture,
			bool async,
			SpriteInfo input,
			TextureType textureType,
			bool isWater,
			bool isFont,
			in Bounds spriteBounds,
			in Vector2I textureSize,

			string hashString,

			ref Vector2B wrapped,
			ref uint scale,
			out Vector2I size,
			out TextureFormat format,
			out Vector2I padding,
			out Vector2I blockPadding,
			out byte[] data
		) {
			padding = Vector2I.Zero;
			blockPadding = Vector2I.Zero;

			var rawSize = textureSize;
			Bounds rawBounds = textureSize;

			Vector2I inputSize;
			Bounds inputBounds;
			switch (textureType) {
				case TextureType.Sprite:
					inputSize = spriteBounds.Extent;
					inputBounds = spriteBounds;
					break;
				case TextureType.Image:
					inputSize = rawSize;
					inputBounds = rawSize;
					break;
				case TextureType.SlicedImage:
					throw new NotImplementedException("Sliced Images not yet implemented");
				default:
					throw new NotImplementedException("Unknown Texture Type provided");
			}

			var rawTextureData = input.Data;

			switch (input.Reference.Format) {
				case SurfaceFormat.Dxt1:
				case SurfaceFormat.Dxt3:
				case SurfaceFormat.Dxt5:
					//rawTextureData = BlockCompress.Decompress(rawTextureData, input);
					
					// This... actually is faster than our version in dotNet...
					using (var tempTexture = new Texture2D(DrawState.Device, input.Reference.Width, input.Reference.Height, false, input.Reference.Format)) {
						tempTexture.SetData(rawTextureData);
						using var pngStream = new MemoryStream();
						tempTexture.SaveAsPng(pngStream, tempTexture.Width, tempTexture.Height);
						//using (var fileStream = new FileStream("D:\\font.png", FileMode.Create)) {
						//	tempTexture.SaveAsPng(fileStream, tempTexture.Width, tempTexture.Height);
						//}
						pngStream.Flush();
						using var pngTexture = Texture2D.FromStream(DrawState.Device, pngStream);
						rawTextureData = new byte[pngTexture.Width * pngTexture.Height * sizeof(int)];
						pngTexture.GetData(rawTextureData);
					}
					break;
			}

			byte[] bitmapData;

			wrapped.Set(false);

			if (isFont) {
				scale = Config.Resample.MaxScale;
			}

			if (Config.Resample.Scale) {
				var originalScale = scale;
				scale = 2;
				foreach (uint s in originalScale.RangeTo(2U)) {
					var newDimensions = inputSize * s;
					if (newDimensions.X <= Config.PreferredMaxTextureDimension && newDimensions.Y <= Config.PreferredMaxTextureDimension) {
						scale = s;
						break;
					}
				}
			}

			var scaledSize = inputSize * scale;
			var newSize = scaledSize.Min(Config.ClampDimension);

			var scaledDimensions = spriteBounds.Extent * scale;

			// Water in the game is pre-upscaled by 4... which is weird.
			FixedSpan<int> rawData;
			if (isWater && WaterBlock != 1) {
				rawData = DownSample(data: rawTextureData, bounds: inputBounds, referenceWidth: (uint)input.ReferenceSize.Width, block: WaterBlock);
				rawSize = inputBounds.Extent;
				rawBounds = rawSize;
				inputBounds = rawSize;
			}
			else if (isFont && FontBlock != 1) {
				rawData = DownSample(data: rawTextureData, bounds: inputBounds, referenceWidth: (uint)input.ReferenceSize.Width, block: FontBlock, blend: true);
				rawSize = inputBounds.Extent;
				rawBounds = rawSize;
				inputBounds = rawSize;
			}
			else {
				rawData = rawTextureData.AsFixedSpan().As<int>();
			}

			var edgeResults = Edge.AnalyzeLegacy(
				reference: input.Reference,
				data: rawData,
				rawSize: rawBounds,
				spriteSize: inputBounds,
				Wrapped: input.Wrapped
			);

			var WrappedX = edgeResults.WrappedX;
			var WrappedY = edgeResults.WrappedY;
			wrapped = edgeResults.Wrapped & Config.Resample.EnableWrappedAddressing;

			if (Config.Debug.Sprite.DumpReference) {
				using var filtered = Textures.CreateBitmap(rawData.As<byte>().ToArray(), rawSize, PixelFormat.Format32bppArgb);
				using var submap = filtered.Clone(inputBounds, filtered.PixelFormat);
				var dump = GetDumpBitmap(submap);
				var path = Cache.GetDumpPath($"{input.Reference.SafeName().Replace("/", ".")}.{hashString}.reference.png");
				File.Delete(path);
				dump.Save(path, ImageFormat.Png);
			}

			if (Config.Resample.Enabled) {
				var prescaleData = rawData;
				var prescaleSize = rawSize;

				var outputSize = inputBounds;

				// Do we need to pad the sprite?
				if (Config.Resample.Padding.Enabled) {
					var shouldPad = new Vector2B(
						!(WrappedX.Positive || WrappedX.Negative) && inputSize.X > 1,
						!(WrappedY.Positive || WrappedX.Negative) && inputSize.Y > 1
					);

					if (
						(
							prescaleSize.X <= Config.Resample.Padding.MinimumSizeTexels &&
							prescaleSize.Y <= Config.Resample.Padding.MinimumSizeTexels
						) ||
						(Config.Resample.Padding.IgnoreUnknown && !input.Reference.Anonymous())
					) {
						shouldPad = Vector2B.False;
					}

					// TODO : make X and Y variants of the whitelist and blacklist
					if (!input.Reference.Anonymous()) {
						if (Config.Resample.Padding.Whitelist.Contains(input.Reference.SafeName())) {
							shouldPad = Vector2B.True;
						}
						else if (isWater || Config.Resample.Padding.Blacklist.Contains(input.Reference.SafeName())) {
							shouldPad = Vector2B.False;
						}
					}

					if (shouldPad.Any) {
						var expectedPadding = Math.Max(1U, scale / 2);
						var expectedPaddingBoth = expectedPadding * 2;

						// TODO we only need to pad the edge that has texels. Double padding is wasteful.
						var paddedSize = inputSize;
						var spriteSize = inputSize;

						var actualPadding = Vector2I.Zero;

						if (shouldPad.X) {
							if ((paddedSize.X + expectedPaddingBoth) * scale > Config.ClampDimension) {
								shouldPad.X = false;
							}
							else {
								paddedSize.X += (int)expectedPaddingBoth;
								actualPadding.X = (int)expectedPadding;
							}
						}
						if (shouldPad.Y) {
							if ((paddedSize.Y + expectedPaddingBoth) * scale > Config.ClampDimension) {
								shouldPad.Y = false;
							}
							else {
								paddedSize.Y += (int)expectedPaddingBoth;
								actualPadding.Y = (int)expectedPadding;
							}
						}

						var hasPadding = shouldPad;

						if (hasPadding.Any) {
							int[] paddedData = new int[paddedSize.Area];

							int y = 0;

							const int padConstant = 0x00000000;

							padding = actualPadding * scale * 2;

							void WritePaddingY () {
								if (!hasPadding.Y)
									return;
								foreach (int i in 0.RangeTo(actualPadding.Y)) {
									var strideOffset = y * paddedSize.Width;
									foreach (int x in 0.RangeTo(paddedSize.Width)) {
										paddedData[strideOffset + x] = padConstant;
									}
									++y;
								}
							}

							WritePaddingY();

							foreach (int i in 0.RangeTo(spriteSize.Height)) {
								var strideOffset = y * paddedSize.Width;
								var strideOffsetRaw = (i + inputBounds.Top) * prescaleSize.Width;
								// Write a padded X line
								var xOffset = strideOffset;
								void WritePaddingX () {
									if (!hasPadding.X)
										return;
									foreach (int x in 0.RangeTo(actualPadding.X)) {
										paddedData[xOffset++] = padConstant;
									}
								}
								WritePaddingX();
								foreach (int x in 0.RangeTo(spriteSize.Width)) {
									paddedData[xOffset++] = rawData[strideOffsetRaw + x + inputBounds.Left];
								}
								WritePaddingX();
								++y;
							}

							WritePaddingY();

							prescaleData = paddedData.AsFixedSpan();
							prescaleSize = paddedSize;
							scaledDimensions = scaledSize = newSize = prescaleSize * scale;
							outputSize = prescaleSize;
							//scaledDimensions = originalPaddedSize * scale;
						}
					}
				}

				bitmapData = new byte[scaledSize.Area * sizeof(int)];

				try {
					switch (Config.Resample.Scaler) {
						case Scaler.xBRZ: {
							var outData = bitmapData.AsFixedSpan().As<uint>();

							var scalerConfig = new xBRZ.Config(
								wrapped: (wrapped & false) | isWater
							);

							new xBRZ.Scaler(
								scaleMultiplier: scale,
								sourceData: prescaleData.As<uint>(),
								sourceSize: prescaleSize,
								sourceTarget: outputSize,
								targetData: ref outData,
								configuration: scalerConfig
							);

							bitmapData = Recolor.Enhance(bitmapData, scaledSize);

						}
						break;
						case Scaler.ImageMagick: {
							throw new NotImplementedException("ImageMagick Scaling is not implemented");
						}
						break;
						case Scaler.Bilinear:
						case Scaler.Bicubic: {
							// TODO : use our own implementation eventually
							using var bitmap = Textures.CreateBitmap(bitmapData, prescaleSize, PixelFormat.Format32bppArgb);
							using var newBitmap = new Bitmap(scaledSize.X, scaledSize.Y);
							newBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
							using (var graphics = Graphics.FromImage(newBitmap)) {
								graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
								graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
								graphics.InterpolationMode = Config.Resample.Scaler switch {
									Scaler.Bilinear => InterpolationMode.HighQualityBilinear,
									Scaler.Bicubic => InterpolationMode.HighQualityBicubic,
									_ => throw new NotImplementedException("Attempted to scale bilinear or bicubic but the scaler type changed part-way through")
								};
								graphics.SmoothingMode = SmoothingMode.HighQuality;
								graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

								using (var wrapMode = new ImageAttributes()) {
									wrapMode.SetWrapMode(WrapMode.TileFlipXY);
									graphics.DrawImage(bitmap, new System.Drawing.Rectangle(System.Drawing.Point.Empty, scaledSize), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, wrapMode);
								}
							}

							bitmapData = ExtractBitmap(newBitmap);
						}
						break;
						default:
							throw new InvalidOperationException($"Unknown Scaler Type: {Config.Resample.Scaler}");
					}
				}
				catch (Exception ex) {
					ex.PrintError();
					throw;
				}
				//ColorSpace.ConvertLinearToSRGB(bitmapData, Texel.Ordering.ARGB);
			}
			else {
				bitmapData = rawData.As<byte>().ToArray();
			}

			if (Config.Debug.Sprite.DumpResample) {
				using var filtered = Textures.CreateBitmap(bitmapData, scaledDimensions, PixelFormat.Format32bppArgb);
				using var dump = GetDumpBitmap(filtered);
				var path = Cache.GetDumpPath($"{input.Reference.SafeName().Replace("/", ".")}.{hashString}.resample-{WrappedX}-{WrappedY}-{padding.X}-{padding.Y}.png");
				File.Delete(path);
				dump.Save(path, ImageFormat.Png);
			}

			if (scaledDimensions != newSize) {
				Debug.TraceLn($"Sprite {texture.SafeName()} requires rescaling");
				// This should be incredibly rare - we very rarely need to scale back down.
				using var filtered = Textures.CreateBitmap(bitmapData, scaledDimensions, PixelFormat.Format32bppArgb);
				using var resized = filtered.Resize(newSize, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);

				bitmapData = ExtractBitmap(resized);
			}

			format = TextureFormat.Color;

			// We don't want to use block compression if asynchronous loads are enabled but this is not an asynchronous load... unless that is explicitly enabled.
			if (Config.Resample.BlockCompression.Enabled && (Config.Resample.BlockCompression.Synchronized || !Config.AsyncScaling.Enabled || async) && newSize.MinOf >= 4) {
				// TODO : We can technically allocate the block padding before the scaling phase, and pass it a stride
				// so it will just ignore the padding areas. That would be more efficient than this.

				// Check for special cases
				bool HasAlpha = true;
				bool IsPunchThroughAlpha = false;
				bool IsMasky = false;
				bool hasR = true;
				bool hasG = true;
				bool hasB = true;
				{
					const int MaxShades = 256;

					var alpha = stackalloc int[MaxShades];
					var blue = stackalloc int[MaxShades];
					var green = stackalloc int[MaxShades];
					var red = stackalloc int[MaxShades];


					var intData = bitmapData.CastAs<byte, uint>();

					foreach (var color in intData) {
						alpha[color.ExtractByte(24)]++;
						blue[color.ExtractByte(16)]++;
						green[color.ExtractByte(8)]++;
						red[color.ExtractByte(0)]++;
					}


					hasR = red[0] != intData.Length;
					hasG = green[0] != intData.Length;
					hasB = blue[0] != intData.Length;

					//Debug.WarningLn($"Punch-through Alpha: {intData.Length}");
					IsPunchThroughAlpha = IsMasky = ((alpha[0] + alpha[MaxShades - 1]) == intData.Length);
					HasAlpha = (alpha[MaxShades - 1] != intData.Length);

					if (HasAlpha && !IsPunchThroughAlpha) {
						var alphaDeviation = Statistics.StandardDeviation(alpha, MaxShades, 1, MaxShades - 2);
						IsMasky = alphaDeviation < Config.Resample.BlockCompression.HardAlphaDeviationThreshold;
					}
				}

				if (!BlockCompress.IsBlockMultiple(newSize)) {
					var blockPaddedSize = (newSize + 3) & ~3;

					var newBuffer = new byte[blockPaddedSize.Area * sizeof(int)];
					var intSpanSrc = bitmapData.AsFixedSpan().As<int>();
					var intSpanDst = newBuffer.AsFixedSpan().As<int>();

					int y;
					for (y = 0; y < newSize.Y; ++y) {
						var newBufferOffset = y * blockPaddedSize.X;
						var bitmapOffset = y * newSize.X;
						int x;
						for (x = 0; x < newSize.X; ++x) {
							intSpanDst[newBufferOffset + x] = intSpanSrc[bitmapOffset + x];
						}
						int lastX = x - 1;
						for (; x < blockPaddedSize.X; ++x) {
							intSpanDst[newBufferOffset + x] = intSpanSrc[bitmapOffset + lastX];
						}
					}
					var lastY = y - 1;
					var sourceOffset = lastY * newSize.X;
					for (; y < blockPaddedSize.Y; ++y) {
						int newBufferOffset = y * blockPaddedSize.X;
						for (int x = 0; x < blockPaddedSize.X; ++x) {
							intSpanDst[newBufferOffset + x] = intSpanDst[sourceOffset + x];
						}
					}

					bitmapData = newBuffer;
					blockPadding = blockPaddedSize - newSize;
					newSize = blockPaddedSize;
				}

				BlockCompress.Compress(
					data: ref bitmapData,
					format: ref format,
					dimensions: newSize,
					HasAlpha: HasAlpha,
					IsPunchThroughAlpha: IsPunchThroughAlpha,
					IsMasky: IsMasky,
					HasR: hasR,
					HasG: hasG,
					HasB: hasB
				);
			}

			size = newSize;
			data = bitmapData;
		}

		internal static ManagedTexture2D Upscale (ScaledTexture texture, ref uint scale, SpriteInfo input, TextureType textureType, ulong hash, ref Vector2B wrapped, bool async) {
			try {
				// Try to process the texture twice. Garbage collect after a failure, maybe it'll work then.
				foreach (var _ in 0.To(1)) {
					try {
						return UpscaleInternal(
							texture: texture,
							scale: ref scale,
							input: input,
							textureType: textureType,
							hash: hash,
							wrapped: ref wrapped,
							async: async
						);
					}
					catch (OutOfMemoryException) {
						Debug.WarningLn("OutOfMemoryException encountered during Upscale, garbage collecting and deferring.");
						Garbage.Collect(compact: true, blocking: true, background: false);
					}
				}
			}
			catch (Exception ex) {
				Debug.Error($"Internal Error processing '{input}'", ex);
			}

			texture.Texture = null;
			return null;
		}

		private static unsafe ManagedTexture2D UpscaleInternal (ScaledTexture texture, ref uint scale, SpriteInfo input, TextureType textureType, ulong hash, ref Vector2B wrapped, bool async) {
			var spriteFormat = TextureFormat.Color;

			if (Config.Garbage.CollectAccountUnownedTextures && GarbageMarkSet.Add(input.Reference)) {
				Garbage.Mark(input.Reference);
				input.Reference.Disposing += (obj, _) => {
					Garbage.Unmark((Texture2D)obj);
				};
			}

			var hashString = hash.ToString("x");
			var cachePath = Cache.GetPath($"{hashString}.cache");

			var spriteBounds = input.Size;
			var textureSize = input.ReferenceSize;
			var inputSize = textureType switch {
				TextureType.Sprite => spriteBounds.Extent,
				TextureType.Image => textureSize,
				TextureType.SlicedImage => throw new NotImplementedException("Sliced Images not yet implemented"),
				_ => throw new NotImplementedException("Unknown Image Type provided")
			};

			var isWater = (textureType == TextureType.Sprite) && IsWater(input);
			var isFont = (textureType == TextureType.Sprite) && !isWater && IsFont(input);

			var BlockSize = isWater ? WaterBlock : isFont ? FontBlock : 0U;

			if (BlockSize != 0) {
				spriteBounds.Offset /= BlockSize;
				spriteBounds.Extent /= BlockSize;
				textureSize /= BlockSize;
			}

			byte[] bitmapData = null;
			try {
				var newSize = Vector2I.Zero;

				try {
					if (Cache.Fetch(
						path: cachePath,
						refScale: out var fetchScale,
						size: out newSize,
						format: out spriteFormat,
						wrapped: out wrapped,
						padding: out texture.Padding,
						blockPadding: out texture.BlockPadding,
						data: out bitmapData
					)) {
						scale = fetchScale;
					}
					else {
						bitmapData = null;
					}
				}
				catch (Exception ex) {
					ex.PrintWarning();
					bitmapData = null;
				}

				if (bitmapData == null) {
					try {
						CreateNewTexture(
							async: async,
							texture: texture,
							input: input,
							textureType: textureType,
							isWater: isWater,
							isFont: isFont,
							spriteBounds: in spriteBounds,
							textureSize: in textureSize,
							hashString: hashString,
							wrapped: ref wrapped,
							scale: ref scale,
							size: out newSize,
							format: out spriteFormat,
							padding: out texture.Padding,
							blockPadding: out texture.BlockPadding,
							data: out bitmapData
						);
					}
					catch (OutOfMemoryException) {
						Debug.Error($"OutOfMemoryException thrown trying to create texture [texture: {texture.SafeName()}, bounds: {spriteBounds}, textureSize: {textureSize}, scale: {scale}]");
						throw;
					}

					try {
						Cache.Save(cachePath, scale, newSize, spriteFormat, wrapped, texture.Padding, texture.BlockPadding, bitmapData);
					}
					catch { }
				}

				texture.UnpaddedSize = newSize - (texture.Padding + texture.BlockPadding);
				texture.AdjustedScale = (Vector2)texture.UnpaddedSize / inputSize;

				ManagedTexture2D CreateTexture(byte[] data) {
					if (input.Reference.GraphicsDevice.IsDisposed) {
						return null;
					}
					var newTexture = new ManagedTexture2D(
						texture: texture,
						reference: input.Reference,
						dimensions: newSize,
						format: spriteFormat
					);
					newTexture.SetData(data);
					return newTexture;
				}

				var isAsync = Config.AsyncScaling.Enabled && async;
				if (isAsync && !Config.AsyncScaling.ForceSynchronousStores) {
					var reference = input.Reference;
					void asyncCall () {
						if (reference.IsDisposed) {
							return;
						}
						ManagedTexture2D newTexture = null;
						try {
							newTexture = CreateTexture(bitmapData);
							texture.Texture = newTexture;
							texture.Finish();
						}
						catch (Exception ex) {
							ex.PrintError();
							if (newTexture != null) {
								newTexture.Dispose();
							}
							texture.Dispose();
						}
					}
					SynchronizedTasks.AddPendingLoad(asyncCall, bitmapData.Length);
					return null;
				}
				else {
					ManagedTexture2D newTexture = null;
					try {
						newTexture = CreateTexture(bitmapData);
						if (isAsync) {
							texture.Texture = newTexture;
							texture.Finish();
						}
						return newTexture;
					}
					catch (Exception ex) {
						ex.PrintError();
						if (newTexture != null) {
							newTexture.Dispose();
						}
					}
				}
			}
			catch (Exception ex) {
				ex.PrintError();
			}

			//TextureCache.Add(hash, output);
			return null;
		}
	}
}
