using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using static SpriteMaster.ScaledTexture;

namespace SpriteMaster {
	internal sealed class Upscaler {
		internal enum Scaler : int {
			xBRZ = 0,
			ImageMagick
		}

		internal static void PurgeHash (Texture2D reference) {
			reference.Meta().CachedData = null;
		}

		// https://stackoverflow.com/a/12996028
		private static ulong HashULong (ulong x) {
			unchecked {
				x = (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9ul;
				x = (x ^ (x >> 27)) * 0x94d049bb133111ebul;
				x = x ^ (x >> 31);
			}
			return x;
		}

		internal static ulong GetHash (SpriteInfo input, bool desprite) {
			// Need to make Hashing.CombineHash work better.
			ulong hash;
			if (!Config.Resample.EnableDynamicScale) {
				hash = Hashing.CombineHash(input.Reference.SafeName()?.GetHashCode(), input.Reference.Meta().GetHash(input));
			}
			else {
				hash = Hashing.CombineHash(input.Reference.SafeName()?.GetHashCode(), input.Reference.Meta().GetHash(input), HashULong(unchecked((ulong)input.ExpectedScale)));
			}
			if (desprite) {
				hash = Hashing.CombineHash(hash, input.Size.Hash());
			}
			else {

			}
			return hash;
		}

		private static readonly WeakSet<Texture2D> GarbageMarkSet = Config.GarbageCollectAccountUnownedTextures ? new WeakSet<Texture2D>() : null;

		// TODO : use MemoryFailPoint class. Extensively.

		private static void Trace(string msg) {
			return;
			Debug.TraceLn($"[CreateNewTexture] {msg}");
		}

		private static unsafe void CreateNewTexture (
			ScaledTexture texture,
			SpriteInfo input,
			bool desprite,
			bool isWater,
			in Bounds spriteBounds,
			in Vector2I textureSize,

			string hashString,

			ref Vector2B wrapped,
			ref int scale,
			out Vector2I size,
			out TextureFormat format,
			out Vector2I padding,
			out Vector2I blockPadding,
			out byte[] data
		) {
			Trace("Entry");
			padding = Vector2I.Zero;
			blockPadding = Vector2I.Zero;

			var inputSize = desprite ? spriteBounds.Extent : textureSize;

			var rawTextureData = input.Data;
			byte[] bitmapData;

			wrapped.Set(false);

			Trace("Scale Loop");
			if (Config.Resample.Scale) {
				int originalScale = scale;
				scale = 2;
				foreach (int s in originalScale..2) {
					var newDimensions = inputSize * s;
					if (newDimensions.X <= Config.PreferredMaxTextureDimension && newDimensions.Y <= Config.PreferredMaxTextureDimension) {
						scale = s;
						break;
					}
				}
			}
			Trace("~Scale Loop");

			var scaledSize = inputSize * scale;
			var newSize = scaledSize.Min(Config.ClampDimension);

			var scaledDimensions = spriteBounds.Extent * scale;

			Trace("GetDumpBitmap");
			Bitmap GetDumpBitmap (Bitmap source) {
				var dump = (Bitmap)source.Clone();
				foreach (int y in 0..dump.Height)
					foreach (int x in 0..dump.Width) {
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

				return dump;
				return null;
			}
			Trace("~GetDumpBitmap");


			Vector2B WrappedX;
			Vector2B WrappedY;

			// Water in the game is pre-upscaled by 4... which is weird.
			Trace("IsWater");
			Span<int> rawData;
			if (isWater) {
				var veryRawData = new Span<byte>(rawTextureData).As<int>();
				rawData = new Span<int>(new int[veryRawData.Length / 16]);
				foreach (int y in 0..textureSize.Height) {
					var ySourceOffset = (y * 4) * input.ReferenceSize.Width;
					var yDestinationOffset = y * textureSize.Width;
					foreach (int x in 0..textureSize.Width) {
						rawData[yDestinationOffset + x] = veryRawData[ySourceOffset + (x * 4)];
					}
				}
			}
			else {
				rawData = new Span<byte>(rawTextureData).As<int>();
			}
			Trace("~IsWater");

			Trace("EdgeResults");
			var edgeResults = Edge.AnalyzeLegacy(
				reference: input.Reference,
				data: rawData,
				rawSize: textureSize,
				spriteSize: spriteBounds,
				Wrapped: input.Wrapped
			);
			Trace("~EdgeResults");

			WrappedX = edgeResults.WrappedX;
			WrappedY = edgeResults.WrappedY;
			wrapped = edgeResults.Wrapped;

			wrapped &= Config.Resample.EnableWrappedAddressing;

			Trace("DumpReference");
			if (Config.Debug.Sprite.DumpReference) {
				using (var filtered = Textures.CreateBitmap(rawData.As<byte>().ToArray(), textureSize, PixelFormat.Format32bppArgb)) {
					using (var submap = (Bitmap)filtered.Clone(spriteBounds, filtered.PixelFormat)) {
						var dump = GetDumpBitmap(submap);
						var path = Cache.GetDumpPath($"{input.Reference.SafeName().Replace("/", ".")}.{hashString}.reference.png");
						File.Delete(path);
						dump.Save(path, System.Drawing.Imaging.ImageFormat.Png);
					}
				}
			}
			Trace("~DumpReference");

			Trace("Smoothing");
			if (Config.Resample.Smoothing) {
				var scalerConfig = new xBRZ.Config(wrappedX: (wrapped.X && false) || isWater, wrappedY: (wrapped.Y && false) || isWater);

				// Do we need to pad the sprite?
				var prescaleData = rawData;
				var prescaleSize = textureSize;

				var shouldPad = new Vector2B(
					!(WrappedX.Positive || WrappedX.Negative) && Config.Resample.Padding.Enabled && inputSize.X > 1,
					!(WrappedY.Positive || WrappedX.Negative) && Config.Resample.Padding.Enabled && inputSize.Y > 1
				);

				var outputSize = spriteBounds;

				if (
					(
						prescaleSize.X <= Config.Resample.Padding.MinimumSizeTexels &&
						prescaleSize.Y <= Config.Resample.Padding.MinimumSizeTexels
					) ||
					(Config.Resample.Padding.IgnoreUnknown && !input.Reference.Name.IsBlank())
				) {
					shouldPad = Vector2B.False;
				}

				// TODO : make X and Y variants of the whitelist and blacklist
				if (input.Reference.Name != null) {
					if (Config.Resample.Padding.Whitelist.Contains(input.Reference.SafeName())) {
						shouldPad = Vector2B.True;
					}
					else if (Config.Resample.Padding.Blacklist.Contains(input.Reference.SafeName())) {
						shouldPad = Vector2B.False;
					}
				}

				Trace("Padding");
				if (shouldPad.Any) {
					int expectedPadding = Math.Max(1, scale / 2);

					// TODO we only need to pad the edge that has texels. Double padding is wasteful.
					var paddedSize = inputSize.Clone();
					var spriteSize = inputSize.Clone();

					var actualPadding = Vector2I.Zero;

					if (shouldPad.X) {
						if ((paddedSize.X + expectedPadding * 2) * scale > Config.ClampDimension) {
							shouldPad.X = false;
						}
						else {
							paddedSize.X += expectedPadding * 2;
							actualPadding.X = expectedPadding;
						}
					}
					if (shouldPad.Y) {
						if ((paddedSize.Y + expectedPadding * 2) * scale > Config.ClampDimension) {
							shouldPad.Y = false;
						}
						else {
							paddedSize.Y += expectedPadding * 2;
							actualPadding.Y = expectedPadding;
						}
					}

					paddedSize += texture.BlockPadding;

					var hasPadding = shouldPad;

					if (hasPadding.Any) {
						int[] paddedData = new int[paddedSize.Area];

						int y = 0;

						// TODO : when writing padding, we might want to consider color clamping but without alpha, especially for block padding.

						const int padConstant = 0x00000000;

						padding = actualPadding * scale * 2;

						void WritePaddingY () {
							if (!hasPadding.Y)
								return;
							foreach (int i in 0..actualPadding.Y) {
								int strideOffset = y * paddedSize.Width;
								foreach (int x in 0..paddedSize.Width) {
									paddedData[strideOffset + x] = padConstant;
								}
								++y;
							}
						}

						WritePaddingY();

						foreach (int i in 0..spriteSize.Height) {
							int strideOffset = y * paddedSize.Width;
							int strideOffsetRaw = (i + spriteBounds.Top) * prescaleSize.Width;
							// Write a padded X line
							int xOffset = strideOffset;
							void WritePaddingX () {
								if (!hasPadding.X)
									return;
								foreach (int x in 0..actualPadding.X) {
									paddedData[xOffset++] = padConstant;
								}
							}
							WritePaddingX();
							foreach (int x in 0..spriteSize.Width) {
								paddedData[xOffset++] = rawData[strideOffsetRaw + x + spriteBounds.Left];
							}
							WritePaddingX();
							++y;
						}

						WritePaddingY();

						prescaleData = new Span<int>(paddedData);
						prescaleSize = paddedSize;
						scaledDimensions = scaledSize = newSize = prescaleSize * scale;
						outputSize = prescaleSize;
						//scaledDimensions = originalPaddedSize * scale;
					}
				}
				Trace("~Padding");

				bitmapData = new byte[scaledSize.Area * sizeof(int)];

				try {
					switch (Config.Resample.Scaler) {
						case Scaler.ImageMagick: {
							throw new NotImplementedException("ImageMagick Scaling is not implemented");
							/*
							fixed (int* ptr = prescaleData) {
								using var bitmapStream = new UnmanagedMemoryStream((byte*)ptr, prescaleData.Length * sizeof(int), prescaleData.Length * sizeof(int), FileAccess.Read);
								using (var image = new MagickImage(bitmapStream, new PixelReadSettings(prescaleSize.Width, prescaleSize.Height, StorageType.Char, PixelMapping.ABGR))) {
									var magickSize = new MagickGeometry(scaledSize.Width, scaledSize.Height) {
										IgnoreAspectRatio = true
									};

									//image.ColorSpace = ImageMagick.ColorSpace.sRGB;
									//image.ColorType = ColorType.TrueColorAlpha;

									//image.Depth = 8;
									//image.BitDepth(Channels.Alpha, 8);

									image.HasAlpha = true;

									image.Crop(new MagickGeometry(outputSize.X, outputSize.Y, outputSize.Width, outputSize.Height));

									image.RePage();

									image.FilterType = FilterType.Box;
									image.Resize(magickSize);
									//image.Resample(scaledSize.Width, scaledSize.Height);

									image.RePage();

									//image.Endian = Endian.MSB;

									image.Depth = 8;
									image.BitDepth(Channels.Alpha, 8);

									using (var outputStream = bitmapData.Stream()) {
										image.Write(outputStream, MagickFormat.Rgba);
									}

									var intData = new Span<byte>(bitmapData).CastAs<byte, uint>();
									foreach (int i in 0..intData.Length) {
										unchecked {
											var color = intData[i];

											// RGBA to ABGR
											color =
												((color >> 24) & 0xFF) |
												((color >> 8) & 0xFF00) |
												((color << 8) & 0xFF0000) |
												((color << 24) & 0xFF000000);

											intData[i] = color;
										}
									}
								}
							}
							*/
						}
						break;
						case Scaler.xBRZ: {
							var outData = new Span<byte>(bitmapData).As<uint>();

							new xBRZ.Scaler(
								scaleMultiplier: scale,
								sourceData: prescaleData.As<uint>(),
								sourceSize: prescaleSize,
								sourceTarget: outputSize,
								targetData: ref outData,
								configuration: scalerConfig
							);

							fixed (uint* ptr = outData) {
								byte* bptr = (byte*)ptr;
								foreach (int i in 0..bitmapData.Length) {
									bitmapData[i] = bptr[i];
								}
							}

							bitmapData = Recolor.Enhance(bitmapData, scaledSize);

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
			Trace("~Smoothing");

			Trace("DumpResample");
			if (Config.Debug.Sprite.DumpResample) {
				using (var filtered = Textures.CreateBitmap(bitmapData, scaledDimensions, PixelFormat.Format32bppArgb)) {
					using (var dump = GetDumpBitmap(filtered)) {
						var path = Cache.GetDumpPath($"{input.Reference.SafeName().Replace("/", ".")}.{hashString}.resample-{WrappedX}-{WrappedY}-{padding.X}-{padding.Y}.png");
						File.Delete(path);
						dump.Save(path, System.Drawing.Imaging.ImageFormat.Png);
					}
				}
			}
			Trace("~DumpResample");

			Trace("Rescale");
			if (scaledDimensions != newSize) {
				// This should be incredibly rare - we very rarely need to scale back down.
				using (var filtered = Textures.CreateBitmap(bitmapData, scaledDimensions, PixelFormat.Format32bppArgb)) {
					using (var resized = filtered.Resize(newSize, System.Drawing.Drawing2D.InterpolationMode.Bicubic)) {
						var resizedData = resized.LockBits(new Bounds(resized), ImageLockMode.ReadOnly, resized.PixelFormat);

						try {
							bitmapData = new byte[resized.Width * resized.Height * sizeof(int)];
							var dataSize = resizedData.Stride * resizedData.Height;
							var dataPtr = resizedData.Scan0;
							var widthSize = resizedData.Width;

							var dataBytes = new byte[dataSize];
							int offsetSource = 0;
							int offsetDest = 0;
							foreach (int y in 0..resizedData.Height) {
								Marshal.Copy(dataPtr + offsetSource, bitmapData, offsetDest, widthSize);
								offsetSource += resizedData.Stride;
								offsetDest += widthSize;
							}
						}
						finally {
							resized.UnlockBits(resizedData);
						}
					}
				}
			}
			Trace("~Rescale");

			// TODO : We can technically allocate the block padding before the scaling phase, and pass it a stride
			// so it will just ignore the padding areas. That would be more efficient than this.
			var requireBlockPadding = new Vector2B(
				newSize.X >= 4 && !BlockCompress.IsBlockMultiple(newSize.X),
				newSize.Y >= 4 && !BlockCompress.IsBlockMultiple(newSize.Y)
			) & Config.Resample.UseBlockCompression;

			Trace("BlockPadding");
			if (requireBlockPadding.Any) {
				var blockPaddedSize = (newSize + 3) & ~3;

				var newBuffer = new byte[blockPaddedSize.Area * sizeof(int)];
				var intSpanSrc = new Span<byte>(bitmapData).As<int>();
				var intSpanDst = new Span<byte>(newBuffer).As<int>();

				int y;
				for (y = 0; y < newSize.Y; ++y) {
					int newBufferOffset = y * blockPaddedSize.X;
					int bitmapOffset = y * newSize.X;
					int x;
					for (x = 0; x < newSize.X; ++x) {
						intSpanDst[newBufferOffset + x] = intSpanSrc[bitmapOffset + x];
					}
					int lastX = x - 1;
					for (; x < blockPaddedSize.X; ++x) {
						intSpanDst[newBufferOffset + x] = intSpanSrc[bitmapOffset + lastX];
					}
				}
				int lastY = y - 1;
				int sourceOffset = lastY * newSize.X;
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
			Trace("~BlockPadding");

			Trace("SpecialCase");
			// Check for special cases
			bool HasAlpha = true;
			bool IsPunchThroughAlpha = false;
			bool IsMasky = false;
			bool hasR = true;
			bool hasG = true;
			bool hasB = true;
			{
				const int MaxShades = 256;
				Trace("StackAlloc");
				var alpha = stackalloc int[MaxShades];
				var blue = stackalloc int[MaxShades];
				var green = stackalloc int[MaxShades];
				var red = stackalloc int[MaxShades];
				Trace("~StackAlloc");

				Trace("intData");
				var intData = bitmapData.CastAs<byte, int>();
				Trace("~IntData");

				Trace("Loop");
				int idx = 0;
				foreach (var color in intData) {
					var aValue = color.ExtractByte(24);
					if (aValue == 0) {
						// Clear out all other colors for alpha of zero.
						intData[idx] = 0;
					}
					alpha[aValue]++;
					blue[color.ExtractByte(16)]++;
					green[color.ExtractByte(8)]++;
					red[color.ExtractByte(0)]++;

					++idx;
				}
				Trace("~Loop");


				hasR = red[0] != intData.Length;
				hasG = green[0] != intData.Length;
				hasB = blue[0] != intData.Length;

				//Debug.WarningLn($"Punch-through Alpha: {intData.Length}");
				IsPunchThroughAlpha = IsMasky = ((alpha[0] + alpha[MaxShades - 1]) == intData.Length);
				HasAlpha = (alpha[MaxShades - 1] != intData.Length);

				Trace("Standard Deviation");
				if (HasAlpha && !IsPunchThroughAlpha) {
					var alphaDeviation = Extensions.Statistics.StandardDeviation(alpha, MaxShades, 1, MaxShades - 2);
					IsMasky = alphaDeviation < Config.Resample.BlockHardAlphaDeviationThreshold;
				}
				Trace("~Standard Deviation");

				bool grayscale = true;
				foreach (int i in 0..256) {
					if (blue[i] != green[i] || green[i] != red[i]) {
						grayscale = false;
						break;
					}
				}
				if (grayscale) {
					//Debug.WarningLn($"Grayscale: {intData.Length}");
				}
			}
			Trace("~SpecialCase");

			bool useBlockCompression = Config.Resample.UseBlockCompression && BlockCompress.IsBlockMultiple(newSize);

			format = TextureFormat.Color;

			Trace("BlockCompress");
			if (useBlockCompression) {
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
			Trace("~BlockCompress");

			size = newSize;
			data = bitmapData;
		}

		internal static ManagedTexture2D Upscale (ScaledTexture texture, ref int scale, SpriteInfo input, bool desprite, ulong hash, ref Vector2B wrapped, bool async) {
			// Try to process the texture twice. Garbage collect after a failure, maybe it'll work then.
			foreach (var _ in 0.To(1)) {
				try {
					return UpscaleInternal(
						texture: texture,
						scale: ref scale,
						input: input,
						desprite: desprite,
						hash: hash,
						wrapped: ref wrapped,
						async: async
					);
				}
				catch (OutOfMemoryException) {
					Garbage.Collect(compact: true, blocking: true, background: false);
				}
			}

			texture.Texture = null;
			return null;
		}

		private static unsafe ManagedTexture2D UpscaleInternal (ScaledTexture texture, ref int scale, SpriteInfo input, bool desprite, ulong hash, ref Vector2B wrapped, bool async) {
			var spriteFormat = TextureFormat.Color;

			if (Config.GarbageCollectAccountUnownedTextures && GarbageMarkSet.Add(input.Reference)) {
				Garbage.Mark(input.Reference);
				input.Reference.Disposing += (object obj, EventArgs args) => {
					Garbage.Unmark((Texture2D)obj);
				};
			}

			var hashString = hash.ToString("x");
			var cachePath = Cache.GetPath($"{hashString}.cache");

			var spriteBounds = input.Size;
			var textureSize = input.ReferenceSize;
			var inputSize = desprite ? spriteBounds.Extent : textureSize;

			bool isWater = input.Size.Right <= 640 && input.Size.Top >= 2000 && input.Size.Width >= 4 && input.Size.Height >= 4 && texture.SafeName() == "LooseSprites/Cursors";

			if (isWater) {
				spriteBounds.X /= 4;
				spriteBounds.Y /= 4;
				spriteBounds.Width /= 4;
				spriteBounds.Height /= 4;

				textureSize.Width /= 4;
				textureSize.Height /= 4;
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
					CreateNewTexture(
						texture: texture,
						input: input,
						desprite: desprite,
						isWater: isWater,
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

				bool isAsync = Config.AsyncScaling.Enabled && async;
				if (isAsync && !Config.RendererSupportsAsyncOps) {
					var reference = input.Reference;
					Action asyncCall = () => {
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
					};
					ScaledTexture.AddPendingAction(asyncCall);
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
